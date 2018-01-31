using System;
using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
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
        private Dictionary<int, UnitExtraDynamic> _playerUnitExtraDic = new Dictionary<int, UnitExtraDynamic>(MaxTeamCount);
        private List<UnitEditData> _unitDatas = new List<UnitEditData>(MaxTeamCount);
        private List<byte> _teams = new List<byte>(MaxTeamCount);
        private ENetBattleTimeResult _eNetBattleTimeResult;
        private int _bestScore;
        private bool _scoreChanged;

        private byte _myTeamId;

        public byte MyTeamId
        {
            get { return _myTeamId; }
        }

        public List<PlayerBase> Players
        {
            get { return _players; }
        }

        private int _curTeamCount;
        private List<UnitExtraDynamic> _roomPlayerUnitExtras = new List<UnitExtraDynamic>();

        public List<byte> Teams
        {
            get { return _teams; }
        }

        private TeamManager()
        {
            Messenger<UnitBase>.AddListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<UnitBase>.AddListener(EMessengerType.OnMonsterDead, OnMonsterKilled);
            Messenger<UnitBase>.AddListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
            Messenger<UnitBase>.AddListener(EMessengerType.OnPlayerArrive, OnPlayerArrive);
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

        public void Reset()
        {
            _curTeamCount = 0;
            _bestScore = 0;
            _eNetBattleTimeResult = ENetBattleTimeResult.None;
            _scoreChanged = false;
            Players.Clear();
            _scoreDic.Clear();
            _playerDic.Clear();
            _teams.Clear();
            _unitDatas.Clear();
            _playerUnitExtraDic.Clear();
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

        public void AddTeam(byte team)
        {
            if (!_teams.Contains(team))
            {
                _teams.Add(team);
            }
        }

        public List<UnitExtraDynamic> GetSortPlayerUnitExtras()
        {
            _roomPlayerUnitExtras.Clear();
            var dic = GetPlayerUnitExtraDic();
            for (int i = 0; i < MaxTeamCount; i++)
            {
                UnitExtraDynamic unitExtra;
                if (dic.TryGetValue(i, out unitExtra))
                {
                    _roomPlayerUnitExtras.Add(unitExtra);
                }
            }

            return _roomPlayerUnitExtras;
        }

        public Dictionary<int, UnitExtraDynamic> GetPlayerUnitExtraDic(UnitDesc? excludeUnitDesc = null)
        {
            _playerUnitExtraDic.Clear();
            var spawnDataScene = Scene2DManager.Instance.GetDataScene2D(Scene2DManager.Instance.SqawnSceneIndex);
            var spawnDatas = spawnDataScene.SpawnDatas;
            for (int i = 0; i < spawnDatas.Count; i++)
            {
                if (excludeUnitDesc != null && spawnDatas[i].Guid == excludeUnitDesc.Value.Guid)
                {
                    continue;
                }
                var spawnUnitExtra = spawnDataScene.GetUnitExtra(spawnDatas[i].Guid);
                var playerUnitExtras = spawnUnitExtra.InternalUnitExtras.ToList<UnitExtraDynamic>();
                for (int j = 0; j < playerUnitExtras.Count; j++)
                {
                    if (playerUnitExtras[j] != null)
                    {
                        if (_playerUnitExtraDic.ContainsKey(j))
                        {
                            LogHelper.Error("playerUnitExtraDic.ContainsKey(j)");
                        }
                        else
                        {
                            _playerUnitExtraDic.Add(j, playerUnitExtras[j]);
                        }
                    }
                }
            }

            return _playerUnitExtraDic;
        }

        public bool CheckSpawnPreinstall(UnitExtraKeyValuePair unitExtraKeyValuePair, UnitDesc unitDesc)
        {
            var playerUnitExtras = unitExtraKeyValuePair.InternalUnitExtras;
            var dic = GetPlayerUnitExtraDic(unitDesc);
            for (int i = 0; i < playerUnitExtras.Count; i++)
            {
                if (playerUnitExtras[i] != null && dic.ContainsKey(i))
                {
                    return false;
                }
            }
            return true;
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

        public static string GetTeamColorName(int teamId)
        {
            switch (teamId)
            {
                case 0:
                    return "red";
                case 1:
                    return "green";
                case 2:
                    return "orange";
                case 3:
                    return "yellow";
                case 4:
                    return "cyan";
                case 5:
                    return "blue";
                case 6:
                    return "purple";
            }

            return "red";
        }

        public PlayerBase GetNextPlayer(ref int curCameraPlayerIndex)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                int index = (curCameraPlayerIndex + i + 1) % _players.Count;
                if (!_players[index].IsMain && _players[index].IsAlive)
                {
                    curCameraPlayerIndex = index;
                    return _players[index];
                }
            }

            return PlayMode.Instance.MainPlayer;
        }

        public UnitBase GetMonsterTarget(MonsterBase unit)
        {
            int curPlayerCount = Players.Count;
            int curStartIndex = GameRun.Instance.LogicFrameCnt;
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

        // 根据当前队伍数匹配
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

        public bool CheckAllTeamerSiTouLe(byte teamId)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].TeamId != teamId)
                {
                    continue;
                }

                if (!_players[i].SiTouLe)
                {
                    return false;
                }
            }
            _scoreDic[teamId] = 0;
            Messenger<int, int>.Broadcast(EMessengerType.OnScoreChanged, teamId, 0);
            return true;
        }
        
        public bool CheckOnlyMyTeamLeft()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i].TeamId != _myTeamId && !_players[i].SiTouLe)
                {
                    return false;
                }
            }
            return true;
        }
        #region Score

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

        public void GameOver(ENetBattleTimeResult eNetBattleTimeResult)
        {
            _eNetBattleTimeResult = eNetBattleTimeResult;
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

        #endregion

    }
}