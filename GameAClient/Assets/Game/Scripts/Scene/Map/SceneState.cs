/********************************************************************
** Filename : SceneState  
** Author : ake
** Date : 10/19/2016 5:09:00 PM
** Summary : SceneState  
***********************************************************************/


using System;
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

        [SerializeField]
        private ESceneState _runState;
        private float _gameTimer;

        [SerializeField] private bool _arrived;
        [SerializeField] private int _gemGain;
        [SerializeField] private int _keyGain;
        [SerializeField] private int _monsterKilled;

        private MapStatistics _mapStatistics = new MapStatistics();

        public bool IsMainPlayerCreated
        {
            get { return _mapStatistics.SpawnCount > 0; }
        }

        public bool HasKey
        {
            get { return _mapStatistics.KeyCount > 0; }
        }

        public int SecondLeft
        {
            //get { return _secondLeft; }
            get 
            {
                if (_runState == ESceneState.Fail || _runState == ESceneState.Fail)
                {
                    return 0;
                }
                return (int)(RunTimeTimeLimit - _gameTimer);
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
                    return _mapStatistics.TimeLimit * 10;
                }
            }
        }

        public int TotalGem
        {
            get { return _mapStatistics.GemCount; }
        }

        public int MonsterCount
        {
            get { return _mapStatistics.MonsterCount; }
        }

        public int HeroCageCount
        {
            get { return _mapStatistics.HeroCageCount; }
        }

        public int WinCondition
        {
            get { return _mapStatistics.WinCondition; }
        }

        public int GemGain
        {
            get { return _gemGain; }
            set
            {
                _gemGain = value;
                UpdateWinState();
                Messenger.Broadcast (EMessengerType.OnGemCollect);
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
                if (!CheckWin(true))
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
//                if (PlayMode.Instance.IsUsingBoostItem(EBoostItemType.BIT_ScoreAddPercent20))
//                {
//                    return _mapStatistics.LifeCount * 11/10;
//                }
//                else
                {
                    return _mapStatistics.LifeCount;
                }
               
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
                // 总分 = 杀死怪物得分 + 拾取宝石得分 + 剩余时间得分 + 剩余生命
                int total = 0;
                if (_runState == ESceneState.Fail) return 0;
                if (_runState == ESceneState.Win)
                {
                    total += ((int)(RunTimeTimeLimit - _gameTimer)) * 10;
                    total += PlayMode.Instance.MainPlayer.Life * 200;
                }
                total += _gemGain * 100;
                total += _monsterKilled * 200;
                //if (PlayMode.Instance.IsUsingBoostItem (EBoostItemType.BIT_ScoreAddPercent20))
                //{
                //    total += total / 5;
                //}
                return total;
            }
        }

        /// <summary>
        ///     �༭ģʽ�µ�����
        /// </summary>
        /// <param name="mapStatistics"></param>
        public void Init(MapStatistics mapStatistics)
        {
            _mapStatistics = mapStatistics;
        }

        internal void Init(GM2DMapData levelData)
        {
            _mapStatistics.WinCondition = (byte) levelData.WinCondition;
            _mapStatistics.TimeLimit = levelData.TimeLimit;
            _mapStatistics.LifeCount = levelData.LifeCount;
        }

        public void Reset()
        {
            _runState = ESceneState.Run;
            _gameTimer = 0;
            _arrived = false;
            _gemGain = 0;
            _monsterKilled = 0;
            _keyGain = 0;
        }

        public void StartPlay()
        {
            if (_mapStatistics.FinalCount == 0)
            {
                RemoveCondition(EWinCondition.Arrived);
            }
            if (_mapStatistics.GemCount == 0)
            {
                RemoveCondition(EWinCondition.CollectTreasure);
            }
            if (_mapStatistics.MonsterCount == 0)
            {
                RemoveCondition(EWinCondition.KillMonster);
            }
            _gameTimer = 0;
            _runState = ESceneState.Run;
            Messenger.Broadcast(EMessengerType.OnWinDataChanged);
        }

        public void Check(Table_Unit tableUnit)
        {
            _mapStatistics.AddOrDeleteUnit(tableUnit, true);
        }

        /// <summary>
        /// 这个接口只在老的新手引导系统里用
        /// </summary>
        public void ForceSetTimeFinish()
        {
            _gameTimer = _mapStatistics.TimeLimit*10;
        }

        public bool HasWinCondition(EWinCondition eWinCondition)
        {
            return _mapStatistics.HasWinCondition(eWinCondition);
        }

        private void RemoveCondition(EWinCondition eWinCondition)
        {
            _mapStatistics.RemoveCondition(eWinCondition);
        }

        public void UpdateLogic(float deltaTime)
        {
            if (_runState != ESceneState.Run)
            {
                return;
            }
            //录像模式下如果出了问题至少保证时间超过录像长度就退出。
            if (GM2DGame.Instance.GameMode.GameRunMode == EGameRunMode.PlayRecord &&
                _gameTimer >= ((GameModePlayRecord)GM2DGame.Instance.GameMode).Record.UsedTime + 5)
            {
                _runState = ESceneState.Win;
                SocialApp.Instance.ReturnToApp();
                return;
            }
            if (HasWinCondition(EWinCondition.TimeLimit))
            {
                _gameTimer += deltaTime;
                if (CheckWinTimeLimit())
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
                    return;
                }
                //_secondLeft = (int) (_mapStatistics.TimeLimit*10 - _gameTimer);
            }
        }

        public void AddKey()
        {
            _keyGain++;
            Messenger.Broadcast(EMessengerType.OnKeyChanged);
        }

        public bool UseKey()
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
            if (_runState != ESceneState.Run)
            {
                return false;
            }
            if (_mapStatistics.WinCondition == 0)
            {
                return false;
            }

            if (_mapStatistics.WinCondition == 1 << (int)EWinCondition.TimeLimit)
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
            return HasWinCondition(EWinCondition.CollectTreasure) && _gemGain < _mapStatistics.GemCount;
        }

        private bool CheckWinKillMonster()
        {
            return HasWinCondition(EWinCondition.KillMonster) && _monsterKilled < _mapStatistics.MonsterCount;
        }

        //private bool CheckWinRescueHero()
        //{
        //    return HasWinCondition(EWinCondition.RescueHero) && _mapStatistics.HeroCageCount != _heroRescued;
        //}

        private bool CheckWinArrived()
        {
            return HasWinCondition(EWinCondition.Arrived) && !_arrived;
        }

        private bool CheckWinTimeLimit()
        {
            return HasWinCondition(EWinCondition.TimeLimit) && _gameTimer >= RunTimeTimeLimit;
        }
    }
}