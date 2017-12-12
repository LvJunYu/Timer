using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class TeamManager : IDisposable
    {
        private static TeamManager _instance;

        public static TeamManager Instance
        {
            get { return _instance ?? (_instance = new TeamManager()); }
        }

        public const int MaxTeamCount = 6;
        private List<PlayerBase> _players = new List<PlayerBase>(MaxTeamCount);
        private Dictionary<byte, int> _scoreDic = new Dictionary<byte, int>(MaxTeamCount);
        private Dictionary<byte, List<long>> _playerDic = new Dictionary<byte, List<long>>(MaxTeamCount);
        private byte _myTeamId;

        public byte MyTeamId
        {
            get { return _myTeamId; }
        }

        public void AddPlayer(PlayerBase player)
        {
            byte teamId = player.GetUnitExtra().TeamId;
            if (!_playerDic.ContainsKey(teamId))
            {
                _playerDic.Add(teamId, new List<long>());
            }
            if (_playerDic[teamId].Contains(player.PlayerId))
            {
                LogHelper.Error("player has been added to TeamManager");
                return;
            }
            _players.Add(player);
            _playerDic[teamId].Add(player.PlayerId);
            if (player.IsMain)
            {
                _myTeamId = player.TeamId;
            }
        }

        public void AddScore(byte teamId, int score)
        {
            if (!_scoreDic.ContainsKey(teamId))
            {
                _scoreDic.Add(teamId, 0);
            }
            _scoreDic[teamId] += score;
            PlayMode.Instance.SceneState.CheckNetBattleWin(_scoreDic[teamId]);
        }

        public int GetMyTeamScore()
        {
            int score;
            if (!_scoreDic.TryGetValue(MyTeamId, out score))
            {
                LogHelper.Error("cant find myTeamScore");
            }
            return score;
        }

        public void Reset()
        {
            _players.Clear();
            _scoreDic.Clear();
            _playerDic.Clear();
        }

        public void Dispose()
        {
            _players.Clear();
            _scoreDic.Clear();
            _playerDic.Clear();
            _instance = null;
        }

        public UnitBase GetMonsterTarget(MonsterBase unit)
        {
            byte teamId = unit.TeamId;
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].TeamId == 0 || unit.TeamId == 0 || _players[i].TeamId != unit.TeamId)
                {
                    return _players[i];
                }
            }
            //找不到目标时返回自己
            return unit;
        }
    }
}