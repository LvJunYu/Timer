using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoyEngine;

namespace GameA.Game
{
    /// <summary>
    /// 一次游戏（关卡）运行过程中的统计数据，涉及单人模式的三星条件、玩家成就等系统
    /// </summary>
    public class GameStatistic : IDisposable
    {
        #region fields

        /// <summary>
        /// 是否过关
        /// </summary>
        private bool _passed;

        /// <summary>
        /// 使用时间
        /// </summary>
        private int _usedTime;

        /// <summary>
        /// 玩家死亡次数
        /// </summary>
        private int _deathCnt;

        /// <summary>
        /// 得分
        /// </summary>
        private int _score;

        /// <summary>
        /// 收集宝石数量
        /// </summary>
        private int _collectGem;

        /// <summary>
        /// 怪物死亡次数
        /// </summary>
        private int _monsterDeathCnt;

        /// <summary>
        /// 怪物被激光杀死次数
        /// </summary>
        private int _monsterKilledByLazerCnt;

        /// <summary>
        /// 怪物因坠落死亡次数
        /// </summary>
        private int _monsterKilledByFallCnt;

        /// <summary>
        /// 移动距离
        /// </summary>
        private int _moveDistance;

        /// <summary>
        /// 跳跃次数
        /// </summary>
        private int _jumpCnt;

        /// <summary>
        /// 触发开关次数
        /// </summary>
        private int _switchTriggerCnt;

        /// <summary>
        /// 使用传送门次数
        /// </summary>
        private int _portalUsedCnt;

        /// <summary>
        /// 死于陷阱次数
        /// </summary>
        private int _killByTrapCnt;

        /// <summary>
        /// 死于怪物次数
        /// </summary>
        private int _killByMonsterCnt;

        /// <summary>
        /// 破怪砖块次数
        /// </summary>
        private int _breakBrickCnt;

        /// <summary>
        /// 踩碎云朵次数
        /// </summary>
        private int _trampCloudCnt;

        #endregion

        #region properties

        /// <summary>
        /// 是否过关
        /// </summary>
        public bool Passed
        {
            get { return _passed; }
        }

        /// <summary>
        /// 使用时间
        /// </summary>
        public int UsedTime
        {
            get { return _usedTime; }
        }

        /// <summary>
        /// 玩家死亡次数
        /// </summary>
        public int DeathCnt
        {
            get { return _deathCnt; }
        }

        /// <summary>
        /// 得分
        /// </summary>
        public int Score
        {
            get { return _score; }
        }

        /// <summary>
        /// 收集宝石数量
        /// </summary>
        public int CollectGem
        {
            get { return _collectGem; }
        }

        /// <summary>
        /// 怪物死亡次数
        /// </summary>
        public int MonsterDeathCnt
        {
            get { return _monsterDeathCnt; }
        }

        /// <summary>
        /// 怪物被激光杀死次数
        /// </summary>
        public int MonsterKilledByLazerCnt
        {
            get { return _monsterKilledByLazerCnt; }
        }

        /// <summary>
        /// 怪物因坠落死亡次数
        /// </summary>
        public int MonsterKilledByFallCnt
        {
            get { return _monsterKilledByFallCnt; }
        }

        /// <summary>
        /// 移动距离
        /// </summary>
        public int MoveDistance
        {
            get { return _moveDistance; }
        }

        /// <summary>
        /// 跳跃次数
        /// </summary>
        public int JumpCnt
        {
            get { return _jumpCnt; }
        }

        /// <summary>
        /// 触发开关次数
        /// </summary>
        public int SwitchTriggerCnt
        {
            get { return _switchTriggerCnt; }
        }

        /// <summary>
        /// 使用传送门次数
        /// </summary>
        public int PortalUsedCnt
        {
            get { return _portalUsedCnt; }
        }

        /// <summary>
        /// 死于陷阱次数
        /// </summary>
        public int KillByTrapCnt
        {
            get { return _killByTrapCnt; }
        }

        /// <summary>
        /// 死于怪物次数
        /// </summary>
        public int KillByMonsterCnt
        {
            get { return _killByMonsterCnt; }
        }

        /// <summary>
        /// 破怪砖块次数
        /// </summary>
        public int BreakBrickCnt
        {
            get { return _breakBrickCnt; }
        }

        /// <summary>
        /// 踩碎云朵次数
        /// </summary>
        public int TrampCloudCnt
        {
            get { return _trampCloudCnt; }
        }

        #endregion

        #region methods

        public GameStatistic()
        {
            _passed = false;
            _usedTime = 0;
            _deathCnt = 0;
            _score = 0;
            _collectGem = 0;
            _monsterDeathCnt = 0;
            _monsterKilledByFallCnt = 0;
            _monsterKilledByLazerCnt = 0;
            _moveDistance = 0;
            _jumpCnt = 0;
            _switchTriggerCnt = 0;
            _portalUsedCnt = 0;
            _killByTrapCnt = 0;
            _killByMonsterCnt = 0;
            _breakBrickCnt = 0;
            _trampCloudCnt = 0;
            //Messenger.AddListener (EMessengerType.GameFinishSuccess, OnGameFinishSuccess);
            //Messenger.AddListener (EMessengerType.GameFinishFailed, OnGameFinishFailed);
            Messenger.AddListener(EMessengerType.OnMainPlayerDead, OnMainPlayerDead);
            Messenger.AddListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<EDieType>.AddListener(EMessengerType.OnMonsterDead, OnMonsterDead);
            Messenger.AddListener(EMessengerType.OnPlayerJump, OnPlayerJump);
            Messenger.AddListener(EMessengerType.OnSwitchTriggered, OnSwitchTriggered);
            Messenger.AddListener(EMessengerType.OnPlayerEnterPortal, OnPlayerEnterPortal);
            Messenger<EKillerType>.AddListener(EMessengerType.OnMainPlayerDead, OnMainPlayerKilled);
            Messenger.AddListener(EMessengerType.OnBreakBrick, OnBreakBrick);
            Messenger.AddListener(EMessengerType.OnTrampCloud, OnTrampCloud);
            // todo movedistance
        }

        public void Reset()
        {
            _passed = false;
            _usedTime = 0;
            _deathCnt = 0;
            _score = 0;
            _collectGem = 0;
            _monsterDeathCnt = 0;
            _monsterKilledByFallCnt = 0;
            _monsterKilledByLazerCnt = 0;
            _moveDistance = 0;
            _jumpCnt = 0;
            _switchTriggerCnt = 0;
            _portalUsedCnt = 0;
            _killByTrapCnt = 0;
            _killByMonsterCnt = 0;
            _breakBrickCnt = 0;
            _trampCloudCnt = 0;
        }

        public void OnGameFinishSuccess()
        {
            _passed = true;
            _usedTime = (int) PlayMode.Instance.SceneState.PassedTime;
            _score = PlayMode.Instance.SceneState.CurScore;
        }

        public void OnGameFinishFailed()
        {
            _passed = false;
            _usedTime = (int) PlayMode.Instance.SceneState.PassedTime;
            _score = PlayMode.Instance.SceneState.CurScore;
        }

        private void OnMainPlayerDead()
        {
            _deathCnt++;
            _score = PlayMode.Instance.SceneState.CurScore;
        }

        private void OnMainPlayerKilled(EKillerType killerType)
        {
            switch (killerType)
            {
                case EKillerType.None:
                    break;
                case EKillerType.Trap:
                    _killByTrapCnt++;
                    break;
                case EKillerType.Monster:
                    _killByMonsterCnt++;
                    break;
                case EKillerType.Drowned:
                    break;
            }
        }

        private void OnGemCollect()
        {
            _collectGem++;
            _score = PlayMode.Instance.SceneState.CurScore;
        }

        private void OnPlayerJump()
        {
            _jumpCnt++;
        }

        private void OnMonsterDead(EDieType dieType)
        {
            _monsterDeathCnt++;
            if (dieType == EDieType.Lazer)
            {
                _monsterKilledByLazerCnt++;
            }
        }

        private void OnSwitchTriggered()
        {
            _switchTriggerCnt++;
        }

        private void OnPlayerEnterPortal()
        {
            _portalUsedCnt++;
        }

        private void OnBreakBrick()
        {
            _breakBrickCnt++;
        }

        private void OnTrampCloud()
        {
            _trampCloudCnt++;
        }

        #endregion

        public void Dispose()
        {
            Messenger.RemoveListener(EMessengerType.OnMainPlayerDead, OnMainPlayerDead);
            Messenger.RemoveListener(EMessengerType.OnGemCollect, OnGemCollect);
            Messenger<EDieType>.RemoveListener(EMessengerType.OnMonsterDead, OnMonsterDead);
            Messenger.RemoveListener(EMessengerType.OnPlayerJump, OnPlayerJump);
            Messenger.RemoveListener(EMessengerType.OnSwitchTriggered, OnSwitchTriggered);
            Messenger.RemoveListener(EMessengerType.OnPlayerEnterPortal, OnPlayerEnterPortal);
        }
    }
}