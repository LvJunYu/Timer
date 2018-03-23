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
        private HashSet<byte> _teamSiTouLe = new HashSet<byte>();
        private Dictionary<IntVec3, int> _playerScoreDic = new Dictionary<IntVec3, int>(MaxTeamCount);
        private Dictionary<IntVec3, int> _playerKillDic = new Dictionary<IntVec3, int>(MaxTeamCount);
        private Dictionary<IntVec3, int> _playerKilledDic = new Dictionary<IntVec3, int>(MaxTeamCount);
        private Dictionary<IntVec3, int> _playerKillMonsterDic = new Dictionary<IntVec3, int>(MaxTeamCount);
        private Dictionary<byte, List<long>> _playerDic = new Dictionary<byte, List<long>>(MaxTeamCount);

        private Dictionary<int, UnitExtraDynamic> _playerUnitExtraDic =
            new Dictionary<int, UnitExtraDynamic>(MaxTeamCount);

        private List<UnitEditData> _unitDatas = new List<UnitEditData>(MaxTeamCount);
        private List<byte> _teams = new List<byte>(MaxTeamCount);
        private ENetBattleTimeResult _eNetBattleTimeResult;
        private int _bestScore;
        private bool _scoreChanged;
        private PlayerBase _mainPlayer;

        public PlayerBase MainPlayer
        {
            get { return _mainPlayer; }
        }

        private Dictionary<byte, List<int>> _teamInxListDic;

        public byte MyTeamId
        {
            get
            {
                if (_mainPlayer == null)
                {
                    return 0;
                }

                return _mainPlayer.TeamId;
            }
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
            Messenger<PlayerBase, UnitBase>.AddListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
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
                _mainPlayer = player;
                Messenger.Broadcast(EMessengerType.OnTeamChanged);
            }
        }

        public void Reset()
        {
            _mainPlayer = null;
            _teamInxListDic = null;
            _curTeamCount = 0;
            _bestScore = 0;
            _eNetBattleTimeResult = ENetBattleTimeResult.None;
            _scoreChanged = false;
            Players.Clear();
            _scoreDic.Clear();
            _teamSiTouLe.Clear();
            _playerScoreDic.Clear();
            _playerKillDic.Clear();
            _playerKilledDic.Clear();
            _playerKillMonsterDic.Clear();
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
            Messenger<PlayerBase, UnitBase>.RemoveListener(EMessengerType.OnPlayerDead, OnPlayerKilled);
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

        public List<int> GetMyTeamInxList()
        {
            if (_teamInxListDic == null)
            {
                _teamInxListDic = new Dictionary<byte, List<int>>();
                var sortData = GetSortPlayerUnitExtras();
                for (int i = 0; i < sortData.Count; i++)
                {
                    var teamId = sortData[i].TeamId;
                    if (!_teamInxListDic.ContainsKey(teamId))
                    {
                        _teamInxListDic.Add(teamId, new List<int>());
                    }

                    _teamInxListDic[teamId].Add(i);
                }
            }

            List<int> list;
            if (_teamInxListDic.TryGetValue(MyTeamId, out list))
            {
                return list;
            }

            LogHelper.Error("GetMyTeamInxList fail");
            return null;
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

        public UnitBase GetMonsterTarget(MonsterBase unit, bool tryNearest = true)
        {
            int curPlayerCount = _players.Count;
            //根据距离确定目标
            if (tryNearest)
            {
                UnitBase target = null;
                int minRelX = int.MaxValue;
                for (int i = 0; i < curPlayerCount; i++)
                {
                    var relPos = _players[i].CenterDownPos - unit.CenterDownPos;
                    int relY = Mathf.Abs(relPos.y);
                    if (relY > 3 * ConstDefineGM2D.ServerTileScale)
                    {
                        continue;
                    }

                    int relX = Mathf.Abs(relPos.x);
                    if (relX < minRelX)
                    {
                        minRelX = relX;
                        target = _players[i];
                    }
                }

                if (target != null)
                {
                    return target;
                }
            }

            //随机一个目标
            int curStartIndex = GameRun.Instance.LogicFrameCnt;
            for (int i = 0; i < curPlayerCount; i++)
            {
                int index = (i + curStartIndex) % curPlayerCount;
                if (_players[index].TeamId == 0 || unit.TeamId == 0 || _players[index].TeamId != unit.TeamId)
                {
                    return _players[index];
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

            if (!_teamSiTouLe.Contains(teamId))
            {
                _teamSiTouLe.Add(teamId);
            }

            Messenger<int, int>.Broadcast(EMessengerType.OnScoreChanged, teamId, 0);
            return true;
        }

        public bool CheckLeftTeamCount(int count)
        {
            return _teams.Count - _teamSiTouLe.Count <= count;
        }

        #region MultiBattleStatistics 多人统计数据

        public int GetPlayerScore(IntVec3 playerGuid)
        {
            int score;
            _playerScoreDic.TryGetValue(playerGuid, out score);
            return score;
        }

        public int GetPlayerKillCount(IntVec3 playerGuid)
        {
            int count;
            _playerKillDic.TryGetValue(playerGuid, out count);
            return count;
        }

        public int GetPlayerKilledCount(IntVec3 playerGuid)
        {
            int count;
            _playerKilledDic.TryGetValue(playerGuid, out count);
            return count;
        }

        public int GetPlayerKillMonsterCount(IntVec3 playerGuid)
        {
            int count;
            _playerKillMonsterDic.TryGetValue(playerGuid, out count);
            return count;
        }

        public int GetMainPlayerScore()
        {
            int score;
            _playerScoreDic.TryGetValue(_mainPlayer.Guid, out score);
            return score;
        }

        public int GetMainPlayerKillCount()
        {
            int count;
            _playerKillDic.TryGetValue(_mainPlayer.Guid, out count);
            return count;
        }

        public int GetMainPlayerKilledCount()
        {
            int count;
            _playerKilledDic.TryGetValue(_mainPlayer.Guid, out count);
            return count;
        }

        public int GetMainPlayerKillMonsterCount()
        {
            int count;
            _playerKillMonsterDic.TryGetValue(_mainPlayer.Guid, out count);
            return count;
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
            return GetTeamScore(MyTeamId);
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

        private void AddScore(UnitBase unit, int score)
        {
            if (unit.IsPlayer)
            {
                var playerId = unit.Guid;
                if (!_playerScoreDic.ContainsKey(playerId))
                {
                    _playerScoreDic.Add(playerId, 0);
                }

                _playerScoreDic[playerId] += score;
                if (unit.IsMain)
                {
                    Messenger.Broadcast(EMessengerType.OnMainPlayerDataChange);
                }
            }

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

        private void AddScore(byte teamId, int score)
        {
            if (!_scoreDic.ContainsKey(teamId))
            {
                _scoreDic.Add(teamId, 0);
            }

            _scoreDic[teamId] += score;
            _scoreChanged = true;
            if (teamId == MyTeamId)
            {
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }

            Messenger<int, int>.Broadcast(EMessengerType.OnScoreChanged, teamId, _scoreDic[teamId]);
            PlayMode.Instance.SceneState.CheckNetBattleWin(_scoreDic[teamId], teamId == MyTeamId);
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

        private void OnPlayerKilled(PlayerBase deadPlayer, UnitBase killer)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            if (killer.IsPlayer)
            {
                //杀人统计
                var killerId = killer.Guid;
                if (!_playerKillDic.ContainsKey(killerId))
                {
                    _playerKillDic.Add(killerId, 0);
                }

                _playerKillDic[killerId]++;
                if (killer.IsMain)
                {
                    Messenger.Broadcast(EMessengerType.OnMainPlayerDataChange);
                }

                //被杀统计
                var playerId = deadPlayer.Guid;
                if (!_playerKilledDic.ContainsKey(playerId))
                {
                    _playerKilledDic.Add(playerId, 0);
                }

                _playerKilledDic[playerId]++;
                if (deadPlayer.IsMain)
                {
                    Messenger.Broadcast(EMessengerType.OnMainPlayerDataChange);
                }
            }

            AddScore(killer, PlayMode.Instance.SceneState.KillPlayerScore);
        }

        private void OnMonsterKilled(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            if (unit.IsPlayer)
            {
                var killerId = unit.Guid;
                if (!_playerKillMonsterDic.ContainsKey(killerId))
                {
                    _playerKillMonsterDic.Add(killerId, 0);
                }

                _playerKillMonsterDic[killerId]++;
                if (unit.IsMain)
                {
                    Messenger.Broadcast(EMessengerType.OnMainPlayerDataChange);
                }
            }

            AddScore(unit, PlayMode.Instance.SceneState.KillMonsterScore);
        }

        private void OnGemCollect(UnitBase unit)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti) return;
            AddScore(unit, PlayMode.Instance.SceneState.GemScore);
        }

        public List<SettlePlayerData> GetSettlePlayerDatas()
        {
            List<SettlePlayerData> _datas = new List<SettlePlayerData>();
            int WinTeamHighScore = -1;
            int mvpIndex = -1;
            for (int i = 0; i < Instance.Players.Count; i++)
            {
                SettlePlayerData onedata = new SettlePlayerData();
                onedata.killNum = Instance.GetPlayerKillCount(Instance.Players[i].Guid);
                onedata.KilledNum = Instance.GetPlayerKilledCount(Instance.Players[i].Guid);
                onedata.Score = Instance.GetPlayerScore(Instance.Players[i].Guid);
                onedata.Name = Instance.Players[i].RoomUser.Name;
                onedata.TeamId = Instance.Players[i].TeamId;
                onedata.TeamScore = Instance.GetTeamScore(onedata.TeamId);
                onedata.IsWin = Instance.CheckTeamWin(onedata.TeamId);
                onedata.MainPlayID = Instance.MainPlayer.RoomUser.Guid;
                onedata.PlayerId = Instance.Players[i].RoomUser.Guid;
                onedata.IsMvp = false;
                onedata.KillMonsterNum = Instance.GetPlayerKillMonsterCount(Instance._players[i].Guid);
                if (onedata.IsWin && onedata.Score > WinTeamHighScore)
                {
                    mvpIndex = i;
                    WinTeamHighScore = onedata.Score;
                }

                _datas.Add(onedata);
            }

            if (mvpIndex >= 0 && mvpIndex < _datas.Count)
            {
                _datas[mvpIndex].IsMvp = true;
            }

            return _datas;
        }

        #endregion

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
                    return Red;
                case 1:
                    return Green;
                case 2:
                    return Orange;
                case 3:
                    return Yellow;
                case 4:
                    return Cyan;
                case 5:
                    return Blue;
                case 6:
                    return Purple;
            }

            return Red;
        }

        private const string Red = "red";
        private const string Green = "green";
        private const string Orange = "orange";
        private const string Yellow = "yellow";
        private const string Cyan = "cyan";
        private const string Blue = "blue";
        private const string Purple = "purple";
    }
}