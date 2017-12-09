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
        private Dictionary<byte, int> _scoreDic = new Dictionary<byte, int>(6);
        private Dictionary<byte, List<long>> _playerDic = new Dictionary<byte, List<long>>(6);
        private byte _myTeamId;

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
            if(!_scoreDic.TryGetValue(_myTeamId, out score))
            {
                LogHelper.Error("cant find myTeamScore");
            }
            return score;
        }

        public void Reset()
        {
            _scoreDic.Clear();
            _playerDic.Clear();
        }
        
        public void Dispose()
        {
            _scoreDic.Clear();
            _playerDic.Clear();
            _instance = null;
        }
    }
}