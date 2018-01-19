using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA.Game
{
    public class TeamManager : IDisposable
    {
        private static TeamManager _instance;

        public static TeamManager Instance
        {
            get { return _instance ?? (_instance = new TeamManager()); }
        }

        public const int MaxTeamCount = PlayerManager.MaxTeamCount;
        private List<PlayerBase> _players = new List<PlayerBase>(MaxTeamCount);
        private Dictionary<byte, int> _scoreDic = new Dictionary<byte, int>(MaxTeamCount); //多人模式才会计算分数
        private Dictionary<byte, List<long>> _playerDic = new Dictionary<byte, List<long>>(MaxTeamCount);
        private List<UnitEditData> _unitDatas = new List<UnitEditData>(MaxTeamCount);
        private List<byte> _teams = new List<byte>(MaxTeamCount);
        private ENetBattleTimeResult _eNetBattleTimeResult;
        private int _bestScore;
        private bool _scoreChanged;
        private int _curCameraPlayerIndex;

        private byte _myTeamId;

        public byte MyTeamId
        {
            get { return _myTeamId; }
        }

        public List<PlayerBase> Players
        {
            get { return _players; }
        }

        public PlayerBase _cameraPlayer;
        private int _curTeamCount;

        public PlayerBase CameraPlayer
        {
            set { _cameraPlayer = value; }
            get
            {
                if (_cameraPlayer == null)
                {
                    _cameraPlayer = PlayMode.Instance.MainPlayer;
                }
                return _cameraPlayer;
            }
        }

        public List<byte> Teams
        {
            get { return _teams; }
        }

        public void ResetCameraPlayer()
        {
            _curCameraPlayerIndex = 0;
            _cameraPlayer = PlayMode.Instance.MainPlayer;
        }

        public void SetNextCameraPlayer()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int index = (_curCameraPlayerIndex + i + 1) % _players.Count;
                if (!_players[index].IsMain && _players[index].IsAlive)
                {
                    _curCameraPlayerIndex = index;
                    _cameraPlayer = _players[index];
                    break;
                }
            }
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

        public void AddPlayer(PlayerBase player, UnitEditData unitEditData)
        {
            if (!_unitDatas.Contains(unitEditData))
            {
                _unitDatas.Add(unitEditData);
            }
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
            Players.Add(player);
            _playerDic[teamId].Add(player.RoomUser.Guid);
            if (player.IsMain)
            {
                _myTeamId = player.TeamId;
                Messenger.Broadcast(EMessengerType.OnTeamChanged);
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
            _scoreChanged = true;
            if (teamId == _myTeamId)
            {
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }
            Messenger<int, int>.Broadcast(EMessengerType.OnScoreChanged, teamId, _scoreDic[teamId]);
            PlayMode.Instance.SceneState.CheckNetBattleWin(_scoreDic[teamId], teamId == _myTeamId);
        }

        public int GetTeamScore(int teamId)
        {
            int score;
            if (!_scoreDic.TryGetValue((byte) teamId, out score))
            {
                _scoreDic.Add((byte) teamId, 0);
            }
            return score;
        }

        public int GetMyTeamScore()
        {
            return GetTeamScore(_myTeamId);
        }

        public bool MyTeamScoreBest()
        {
            return GetMyTeamScore() >= GetBestScore();
        }

        public bool CheckTeamWin(int teamId)
        {
            switch (_eNetBattleTimeResult)
            {
                case ENetBattleTimeResult.Score:
                    return CheckTeamScoreBest(teamId);
                case ENetBattleTimeResult.AllWin:
                    return true;
                case ENetBattleTimeResult.AllFail:
                    return false;
            }
            return true;
        }

        private bool CheckTeamScoreBest(int teamId)
        {
            return GetTeamScore(teamId) >= GetBestScore();
        }

        private int GetBestScore()
        {
            if (!_scoreChanged) return _bestScore;
            _bestScore = 0;
            foreach (var value in _scoreDic.Values)
            {
                if (value > _bestScore)
                {
                    _bestScore = value;
                }
            }
            _scoreChanged = false;
            return _bestScore;
        }

        public void Reset()
        {
            _curCameraPlayerIndex = 0;
            _curTeamCount = 0;
            _bestScore = 0;
            _eNetBattleTimeResult = ENetBattleTimeResult.None;
            _cameraPlayer = null;
            _scoreChanged = false;
            Players.Clear();
            _scoreDic.Clear();
            _playerDic.Clear();
            Teams.Clear();
            _unitDatas.Clear();
        }

        public void Dispose()
        {
            Reset();
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnMonsterDead, OnMonsterKilled);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
            Messenger<UnitBase>.RemoveListener(EMessengerType.OnPlayerArrive, OnPlayerArrive);
            _instance = null;
        }

        public UnitBase GetMonsterTarget(MonsterBase unit)
        {
            int curPlayerCount = Players.Count;
            int curStartIndex = GameRun.Instance.LogicFrameCnt % curPlayerCount;
            for (int i = 0; i < Players.Count; i++)
            {
                int index = (i + curStartIndex) % curPlayerCount;
                if (Players[index].TeamId == 0 || unit.TeamId == 0 || Players[index].TeamId != unit.TeamId)
                {
                    return Players[index];
                }
            }
            return unit; //找不到目标时返回自己
        }

        public void AddTeam(byte team)
        {
            if (!Teams.Contains(team))
            {
                Teams.Add(team);
            }
        }

        /// 根据当前队伍数匹配
        public int GetSpawnIndex(List<UnitEditData> spawnDatas, int basicNum)
        {
            int spwanCount = spawnDatas.Count;
            for (int teamCount = _curTeamCount; teamCount < MaxTeamCount; teamCount++)
            {
                for (int i = 0; i < spawnDatas.Count; i++)
                {
                    var index = (basicNum + i) % spwanCount;
                    if (CheckTeamerCount(spawnDatas[index], teamCount))
                    {
                        _curTeamCount = teamCount;
                        return index;
                    }
                }
            }
            LogHelper.Error("can not find an appropriate spawn");
            return 0;
        }

        private bool CheckTeamerCount(UnitEditData unitEditData, int checkTeamCount = 0, bool checkTeam = true)
        {
            if (_unitDatas.Contains(unitEditData))
            {
                return false;
            }
            if (checkTeam)
            {
                var teamId = DataScene2D.CurScene.GetUnitExtra(unitEditData.UnitDesc.Guid).TeamId;
                return !_playerDic.ContainsKey(teamId) || _playerDic[teamId].Count <= checkTeamCount;
            }
            return true;
        }

        public void GameOver(ENetBattleTimeResult eNetBattleTimeResult)
        {
            _eNetBattleTimeResult = eNetBattleTimeResult;
        }
        
        public static Sprite GetSpawnSprite(int teamId)
        {
            return JoyResManager.Instance.GetSprite(string.Format("M1Spawn{0}Icon", teamId));
        }

        public static Color GetTeamColor(int teamId)
        {
            switch (teamId)
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.green;
                case 2:
                    return new Color(1, 164 / (float) 255, 0);
                case 3:
                    return Color.yellow;
                case 4:
                    return new Color(0, 1, 200 / (float) 255);
                case 5:
                    return new Color(0, 110 / (float) 255, 1);
                case 6:
                    return Color.magenta;
            }
            return Color.red;
        }
    }
}