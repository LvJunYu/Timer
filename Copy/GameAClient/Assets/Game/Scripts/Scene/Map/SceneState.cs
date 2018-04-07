/********************************************************************
** Filename : SceneState  
** Author : ake
** Date : 10/19/2016 5:09:00 PM
** Summary : SceneState  
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    [Serializable]
    public class SceneState
    {
        public enum ESceneState
        {
            Run,
            Win,
            Fail,
        }

        [SerializeField] private ESceneState _runState;
        private float _gameTimer;

        [SerializeField] private bool _arrived;
        [SerializeField] private int _gemGain;
        [SerializeField] private int _keyGain;
        [SerializeField] private int _monsterKilled;
        [SerializeField] private int _gemInWoodCase = -1;
        private Dictionary<long, int> _keyDic = new Dictionary<long, int>(PlayerManager.MaxTeamCount);

        private MapStatistics _mapStatistics = new MapStatistics();

        public bool IsMulti
        {
            get { return MapStatistics.IsMulti; }
        }

        public bool IsMainPlayerCreated
        {
            get { return MapStatistics.SpawnCount > 0; }
        }

        public bool HasKey
        {
            get { return MapStatistics.KeyCount > 0 || Scene2DManager.Instance.CheckKeyInBox(); }
        }

        public int SecondLeft
        {
            //get { return _secondLeft; }
            get
            {
//                if (_runState == ESceneState.Fail)
//                {
//                    return 0;
//                }

                return (int) (RunTimeTimeLimit - _gameTimer);
            }
        }

        /// <summary>
        /// 关卡运行时时间限制（受到增益道具的影响）
        /// </summary>
        /// <value>The run time time limit.</value>
        public int RunTimeTimeLimit
        {
            get
            {
//                if (PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_TimeAddPercent20))
//                {
//                    return _mapStatistics.TimeLimit * 10+60;
//                }else
                {
                    if (IsMulti)
                    {
                        return MapStatistics.NetBattleTimeLimit;
                    }

                    return MapStatistics.TimeLimit * 10;
                }
            }
        }

        public int FinalCount
        {
            get { return MapStatistics.FinalCount; }
        }

        public int TotalGem
        {
            get { return MapStatistics.GemCount + GemCountInWoodCase; }
        }

        public int GemCountInWoodCase
        {
            get
            {
                if (_gemInWoodCase == -1)
                {
                    _gemInWoodCase = Scene2DManager.Instance.GetGemCountInWoodCase();
                }

                return _gemInWoodCase;
            }
        }

        public int MonsterCount
        {
            get { return MapStatistics.MonsterCount + Scene2DManager.Instance.GetMonsterCountInCaves(); }
        }

        public int HeroCageCount
        {
            get { return MapStatistics.HeroCageCount; }
        }

        public int WinCondition
        {
            get { return MapStatistics.WinCondition; }
        }

        public int GemGain
        {
            get { return _gemGain; }
            set
            {
                _gemGain = value;
                UpdateWinState();
                Messenger.Broadcast(EMessengerType.OnGemCollect);
                Messenger.Broadcast(EMessengerType.OnWinDataChanged);
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }
        }

        public float PassedTime
        {
            get { return _gameTimer; }
        }

        public bool Arrived
        {
            get { return _arrived; }
            set
            {
                if (!IsMulti && !CheckWin(true))
                {
                    return;
                }

                _arrived = value;
                UpdateWinState();
                Messenger.Broadcast(EMessengerType.OnWinDataChanged);
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }
        }

        public int MonsterKilled
        {
            get { return _monsterKilled; }
            set
            {
                _monsterKilled = value;
                UpdateWinState();
                Messenger.Broadcast(EMessengerType.OnWinDataChanged);
                Messenger.Broadcast(EMessengerType.OnScoreChanged);
            }
        }

        public int KeyGain
        {
            get { return _keyGain; }
        }

        public int Life
        {
            get
            {
                if (IsMulti)
                {
                    if (_mapStatistics.InfiniteLife)
                    {
                        return 99;
                    }

                    return _mapStatistics.NetBattleLifeCount;
                }

                return _mapStatistics.LifeCount;
            }
        }

        public bool GameSucceed
        {
            get { return _runState == ESceneState.Win; }
        }

        public bool GameRunning
        {
            get { return _runState == ESceneState.Run; }
        }

        public bool GameFailed
        {
            get { return _runState == ESceneState.Fail; }
        }

        public int CurScore
        {
            get
            {
                if (IsMulti)
                {
                    return TeamManager.Instance.GetMyTeamScore();
                }

                // 总分 = 杀死怪物得分 + 拾取宝石得分 + 剩余时间得分 + 剩余生命
                return GetScore();
            }
        }

        public int GemScore
        {
            get { return _mapStatistics.NetBattleCollectGemScore; }
        }

        public int KillMonsterScore
        {
            get { return _mapStatistics.NetBattleKillMonsterScore; }
        }

        public int KillPlayerScore
        {
            get { return _mapStatistics.NetBattleKillPlayerScore; }
        }

        public int ArriveScore
        {
            get { return _mapStatistics.NetBattleArriveScore; }
        }

        public MapStatistics MapStatistics
        {
            get { return _mapStatistics; }
        }

        private int GetScore(bool forceCalculateAll = false)
        {
            int total = 0;
            if (_runState == ESceneState.Fail) return 0;
            if (_runState == ESceneState.Win || forceCalculateAll)
            {
                total += (int) (RunTimeTimeLimit - _gameTimer) * 10;
                total += PlayMode.Instance.MainPlayer.Life * 200;
            }

            //if (PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_ScoreAddPercent20))
            //{
            //    total += total / 5;
            //}
            total += _gemGain * 100;
            total += _monsterKilled * 200;
            return total;
        }

        public void Init(MapStatistics mapStatistics)
        {
            _mapStatistics = mapStatistics;
        }

        internal void Init(GM2DMapData levelData)
        {
            MapStatistics.WinCondition = (byte) levelData.WinCondition;
            MapStatistics.TimeLimit = levelData.TimeLimit;
            MapStatistics.LifeCount = levelData.LifeCount;
        }

        public void Reset()
        {
            _runState = ESceneState.Run;
            _gameTimer = 0;
            _arrived = false;
            _gemGain = 0;
            _monsterKilled = 0;
            _keyGain = 0;
            _gemInWoodCase = -1;
            _keyDic.Clear();
        }

        public void StartPlay()
        {
            if (MapStatistics.FinalCount == 0)
            {
                RemoveCondition(EWinCondition.WC_Arrive);
            }

            if (TotalGem == 0)
            {
                RemoveCondition(EWinCondition.WC_Collect);
            }

            if (MonsterCount == 0)
            {
                RemoveCondition(EWinCondition.WC_Monster);
            }

            _gameTimer = 0;
            _runState = ESceneState.Run;
            Messenger.Broadcast(EMessengerType.OnWinDataChanged);
        }

        public void Check(Table_Unit tableUnit)
        {
            MapStatistics.AddOrDeleteUnit(tableUnit, true);
        }

        /// <summary>
        /// 这个接口只在老的新手引导系统里用
        /// </summary>
        public void ForceSetTimeFinish()
        {
            _gameTimer = MapStatistics.TimeLimit * 10;
        }

        public bool HasWinCondition(EWinCondition eWinCondition)
        {
            return MapStatistics.HasWinCondition(eWinCondition);
        }

        private void RemoveCondition(EWinCondition eWinCondition)
        {
            MapStatistics.SetWinCondition(eWinCondition, false);
        }

        public void UpdateLogic(float deltaTime)
        {
            if (_runState != ESceneState.Run)
            {
                return;
            }

            //录像模式下如果出了问题至少保证时间超过录像长度就退出。
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.PlayRecord &&
                GameRun.Instance.LogicFrameCnt >=
                ((GameModePlayRecord) GM2DGame.Instance.GameMode).Record.UsedTime + 250)
            {
                _runState = ESceneState.Win;
                SocialApp.Instance.ReturnToApp();
                return;
            }

            _gameTimer += deltaTime;
            if (IsMulti)
            {
                if (NetBattleTimeOver())
                {
                    NetBattleOver();
                }
            }
            else
            {
                if (HasWinCondition(EWinCondition.WC_TimeLimit) && CheckWinTimeLimit())
                {
                    bool value = CheckWin();
                    _gameTimer = 0;
                    if (value)
                    {
                        _runState = ESceneState.Win;
                        Messenger.Broadcast(EMessengerType.GameFinishSuccess);
                    }
                    else
                    {
                        _runState = ESceneState.Fail;
                        // 因时间用完没有达到目标而失败
                        Messenger.Broadcast(EMessengerType.GameFinishFailed);
                    }

                    //_secondLeft = 0;
                }

                //_secondLeft = (int) (_mapStatistics.TimeLimit*10 - _gameTimer);
            }
        }

        private void NetBattleOver()
        {
            switch ((ENetBattleTimeResult) MapStatistics.NetBattleTimeWinCondition)
            {
                case ENetBattleTimeResult.Score:
                    NetBattleWin(TeamManager.Instance.MyTeamScoreBest());
                    break;
                case ENetBattleTimeResult.AllWin:
                    NetBattleWin(true);
                    break;
                case ENetBattleTimeResult.AllFail:
                    NetBattleWin(false);
                    break;
                default:
                    LogHelper.Error("NetBattleTimeWinCondition has beyonded limit");
                    break;
            }
        }

        public void AddKey(PlayerBase owner = null)
        {
            if (IsMulti)
            {
                if (owner != null)
                {
                    var playerId = owner.RoomUser.Guid;
                    if (!_keyDic.ContainsKey(playerId))
                    {
                        _keyDic.Add(playerId, 0);
                    }

                    _keyDic[playerId]++;
                }
            }
            else
            {
                _keyGain++;
                Messenger.Broadcast(EMessengerType.OnKeyChanged);
            }
        }

        public bool UseKey(PlayerBase owner = null)
        {
            if (IsMulti)
            {
                if (owner != null)
                {
                    var playerId = owner.RoomUser.Guid;
                    if (_keyDic.ContainsKey(playerId) && _keyDic[playerId] > 0)
                    {
                        _keyDic[playerId]--;
                        return true;
                    }
                }
            }
            else
            {
                if (_keyGain > 0)
                {
                    _keyGain--;
                    Messenger.Broadcast(EMessengerType.OnKeyChanged);
                    return true;
                }
            }

            return false;
        }

        public bool RpgUseKey()
        {
            if (_keyGain > 0)
            {
                _keyGain--;
                Messenger.Broadcast(EMessengerType.OnKeyChanged);
                return true;
            }

            return false;
        }

        public void MainUnitSiTouLe()
        {
            _runState = ESceneState.Fail;
        }

        private bool CheckWin(bool ignoreConditionArrived = false)
        {
            if (IsMulti) return false;
            if (_runState != ESceneState.Run)
            {
                return false;
            }

            if (MapStatistics.WinCondition == 0)
            {
                return false;
            }

            if (MapStatistics.WinCondition == 1 << (int) EWinCondition.WC_TimeLimit)
            {
                if (CheckWinTimeLimit())
                {
                    return true;
                }

                return false;
            }

            if (PlayMode.Instance.MainPlayer == null)
            {
                return false;
            }

            if (CheckWinCollectTreasure())
            {
                return false;
            }

            if (CheckWinKillMonster())
            {
                return false;
            }

            //if (CheckWinRescueHero())
            //{
            //    return false;
            //}
            if (!ignoreConditionArrived)
            {
                if (CheckWinArrived())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断乱入胜利条件
        /// </summary>
        /// <returns></returns>
        public bool CheckShadowWin()
        {
            int score = GetScore(true);
            int shadowScore = GM2DGame.Instance.GameMode.Record.Score;
//            int usedTime = GameRun.Instance.LogicFrameCnt;
//            int shadowUsedTime = GM2DGame.Instance.GameMode.Record.UsedTime;
            return score >= shadowScore;
        }

        private void UpdateWinState()
        {
            if (!CheckWin())
            {
                return;
            }

            _runState = ESceneState.Win;
            Messenger.Broadcast(EMessengerType.GameFinishSuccess);
            LogHelper.Debug("Win");
        }

        private bool CheckWinCollectTreasure()
        {
            return HasWinCondition(EWinCondition.WC_Collect) && _gemGain < TotalGem;
        }

        private bool CheckWinKillMonster()
        {
            return HasWinCondition(EWinCondition.WC_Monster) && _monsterKilled < MonsterCount;
        }

        //private bool CheckWinRescueHero()
        //{
        //    return HasWinCondition(EWinCondition.RescueHero) && _mapStatistics.HeroCageCount != _heroRescued;
        //}

        private bool CheckWinArrived()
        {
            return HasWinCondition(EWinCondition.WC_Arrive) && !_arrived;
        }

        private bool CheckWinTimeLimit()
        {
            return HasWinCondition(EWinCondition.WC_TimeLimit) && _gameTimer >= RunTimeTimeLimit;
        }

        public bool CanHarmType(EHarmType eHarmType)
        {
            return MapStatistics.CanHarmType(eHarmType);
        }

        public void CheckNetBattleWin(int score, bool myTeam)
        {
            if (!IsMulti) return;
            if (MapStatistics.NetBattleScoreWinCondition && score >= MapStatistics.NetBattleWinScore)
            {
                NetBattleWin(myTeam, true);
            }
        }

        private bool NetBattleTimeOver()
        {
            return _gameTimer >= RunTimeTimeLimit;
        }

        private void NetBattleWin(bool win, bool scoreWin = false, bool allFail = false)
        {
            var gameMode = GM2DGame.Instance.GameMode as GameModeNetPlay;
            if (gameMode != null && gameMode.CurGamePhase == GameModeNetPlay.EGamePhase.Wait)
            {
                return;
            }

            if (scoreWin)
            {
                TeamManager.Instance.GameOver(ENetBattleTimeResult.Score);
            }
            else if (allFail)
            {
                TeamManager.Instance.GameOver(ENetBattleTimeResult.AllFail);
            }
            else
            {
                TeamManager.Instance.GameOver((ENetBattleTimeResult) MapStatistics.NetBattleTimeWinCondition);
            }

            if (win)
            {
                _runState = ESceneState.Win;
                Messenger.Broadcast(EMessengerType.GameFinishSuccess);
            }
            else
            {
                _runState = ESceneState.Fail;
                Messenger.Broadcast(EMessengerType.GameFinishFailed);
            }
        }

        public void AllTeamerSiTouLe()
        {
            if (GM2DGame.Instance.GameMode.Project.ProjectType == EProjectType.PT_Cooperation)
            {
                NetBattleWin(false);
            }
            else if (GM2DGame.Instance.GameMode.Project.ProjectType == EProjectType.PT_Compete)
            {
                if (MapStatistics.NetBattleTimeWinCondition == (int) ENetBattleTimeResult.Score)
                {
                    if (TeamManager.Instance.CheckLeftTeamCount(1))
                    {
                        NetBattleWin(TeamManager.Instance.MyTeamScoreBest());
                    }
                }
                else
                {
                    if (TeamManager.Instance.CheckLeftTeamCount(0))
                    {
                        NetBattleWin(false, false, true);
                    }
                }
            }
        }

        public int GetGemGainScore()
        {
            return _gemGain * 100;
        }

        public int GetLastTimeScore()
        {
            return (int) (RunTimeTimeLimit - _gameTimer) * 10;
        }

        public int GetKillMonsterScore()
        {
            return _monsterKilled * 200;
        }
    }
}