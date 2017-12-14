﻿using System;
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
        private Dictionary<byte, int> _scoreDic = new Dictionary<byte, int>(MaxTeamCount); //多人模式才会计算分数
        private Dictionary<byte, List<long>> _playerDic = new Dictionary<byte, List<long>>(MaxTeamCount);

        private byte _myTeamId;

        public byte MyTeamId
        {
            get { return _myTeamId; }
        }

        private TeamManager()
        {
            Messenger<UnitBase>.AddListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<UnitBase>.AddListener(EMessengerType.OnMonsterDead, OnMonsterKilled);
            Messenger<UnitBase>.AddListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
            Messenger<UnitBase>.AddListener(EMessengerType.OnPlayerArrive, OnPlayerArrive);
        }

        private void OnPlayerArrive(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            AddScore(unit, PlayMode.Instance.SceneState.ArriveScore);
        }

        private void OnPlayerKilled(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            AddScore(unit, PlayMode.Instance.SceneState.KillPlayerScore);
        }

        private void OnMonsterKilled(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            AddScore(unit, PlayMode.Instance.SceneState.KillMonsterScore);
        }

        private void OnGemCollect(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            AddScore(unit, PlayMode.Instance.SceneState.GemScore);
        }

        public void AddPlayer(PlayerBase player)
        {
            byte teamId = player.GetUnitExtra().TeamId;
            if (!_playerDic.ContainsKey(teamId))
            {
                _playerDic.Add(teamId, new List<long>());
            }
            if (_playerDic[teamId].Contains(player.RoomUser.Guid))
            {
                LogHelper.Error("player has been added to TeamManager");
                return;
            }
            _players.Add(player);
            _playerDic[teamId].Add(player.RoomUser.Guid);
            if (player.IsMain)
            {
                _myTeamId = player.TeamId;
            }
        }

        public void AddScore(UnitBase unit, int score)
        {
            var teamId = unit.TeamId;
            //teamId == 0 自己一伙
            if (teamId == 0)
            {
                if (unit.IsMain)
                {
                    AddScore(teamId, score);
                }
            }
            else
            {
                AddScore(teamId, score);
            }
        }

        public void AddScore(byte teamId, int score)
        {
            if (!_scoreDic.ContainsKey(teamId))
            {
                _scoreDic.Add(teamId, 0);
            }
            _scoreDic[teamId] += score;
            if (teamId == _myTeamId)
            {
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }
            Messenger<int, int>.Broadcast(EMessengerType.OnScoreChanged, teamId, _scoreDic[teamId]);
            PlayMode.Instance.SceneState.CheckNetBattleWin(_scoreDic[teamId], teamId == _myTeamId);
        }

        public int GetMyTeamScore()
        {
            int score;
            if (!_scoreDic.TryGetValue(MyTeamId, out score))
            {
                _scoreDic.Add(MyTeamId, 0);
            }
            return score;
        }

        public bool MyTeamHeighScore()
        {
            var myScore = GetMyTeamScore();
            foreach (var value in _scoreDic.Values)
            {
                if (value > myScore)
                {
                    return false;
                }
            }
            return true;
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
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnMonsterDead, OnMonsterKilled);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnPlayerArrive, OnPlayerArrive);
            _instance = null;
        }

        public UnitBase GetMonsterTarget(MonsterBase unit)
        {
            int curPlayerCount = _players.Count;
            int curStartIndex = GameRun.Instance.LogicFrameCnt % curPlayerCount;
            for (int i = 0; i < _players.Count; i++)
            {
                int index = (i + curStartIndex) % curPlayerCount;
                if (_players[index].TeamId == 0 || unit.TeamId == 0 || _players[index].TeamId != unit.TeamId)
                {
                    return _players[index];
                }
            }
            return unit; //找不到目标时返回自己
        }
    }
}