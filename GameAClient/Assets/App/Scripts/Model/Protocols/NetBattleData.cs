//  | 联机对战数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NetBattleData : SyncronisticData<Msg_NetBattleData> {
        #region 字段
        /// <summary>
        /// 可伤害目标类型
        /// </summary>
        private int _harmType;
        /// <summary>
        /// 时间限制
        /// </summary>
        private int _timeLimit;
        /// <summary>
        /// 最大人数
        /// </summary>
        private int _playerCount;
        /// <summary>
        /// 生命数
        /// </summary>
        private int _lifeCount;
        /// <summary>
        /// 复活时间
        /// </summary>
        private int _reviveTime;
        /// <summary>
        /// 复活无敌时间
        /// </summary>
        private int _reviveInvincibleTime;
        /// <summary>
        /// 复活无敌时间
        /// </summary>
        private int _reviveType;
        /// <summary>
        /// 时间胜利条件
        /// </summary>
        private int _timeWinCondition;
        /// <summary>
        /// 胜利所需分数
        /// </summary>
        private int _winScore;
        /// <summary>
        /// 到底终点得分
        /// </summary>
        private int _arriveScore;
        /// <summary>
        /// 收集兽牙得分
        /// </summary>
        private int _collectGemScore;
        /// <summary>
        /// 击杀怪物得分
        /// </summary>
        private int _killMonsterScore;
        /// <summary>
        /// 击杀玩家得分
        /// </summary>
        private int _killPlayerScore;
        /// <summary>
        /// 达到得分获胜
        /// </summary>
        private bool _scoreWinCondition;
        /// <summary>
        /// 无限生命
        /// </summary>
        private bool _infiniteLife;
        #endregion

        #region 属性
        /// <summary>
        /// 可伤害目标类型
        /// </summary>
        public int HarmType { 
            get { return _harmType; }
            set { if (_harmType != value) {
                _harmType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 时间限制
        /// </summary>
        public int TimeLimit { 
            get { return _timeLimit; }
            set { if (_timeLimit != value) {
                _timeLimit = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最大人数
        /// </summary>
        public int PlayerCount { 
            get { return _playerCount; }
            set { if (_playerCount != value) {
                _playerCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 生命数
        /// </summary>
        public int LifeCount { 
            get { return _lifeCount; }
            set { if (_lifeCount != value) {
                _lifeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 复活时间
        /// </summary>
        public int ReviveTime { 
            get { return _reviveTime; }
            set { if (_reviveTime != value) {
                _reviveTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 复活无敌时间
        /// </summary>
        public int ReviveInvincibleTime { 
            get { return _reviveInvincibleTime; }
            set { if (_reviveInvincibleTime != value) {
                _reviveInvincibleTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 复活无敌时间
        /// </summary>
        public int ReviveType { 
            get { return _reviveType; }
            set { if (_reviveType != value) {
                _reviveType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 时间胜利条件
        /// </summary>
        public int TimeWinCondition { 
            get { return _timeWinCondition; }
            set { if (_timeWinCondition != value) {
                _timeWinCondition = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 胜利所需分数
        /// </summary>
        public int WinScore { 
            get { return _winScore; }
            set { if (_winScore != value) {
                _winScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 到底终点得分
        /// </summary>
        public int ArriveScore { 
            get { return _arriveScore; }
            set { if (_arriveScore != value) {
                _arriveScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 收集兽牙得分
        /// </summary>
        public int CollectGemScore { 
            get { return _collectGemScore; }
            set { if (_collectGemScore != value) {
                _collectGemScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 击杀怪物得分
        /// </summary>
        public int KillMonsterScore { 
            get { return _killMonsterScore; }
            set { if (_killMonsterScore != value) {
                _killMonsterScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 击杀玩家得分
        /// </summary>
        public int KillPlayerScore { 
            get { return _killPlayerScore; }
            set { if (_killPlayerScore != value) {
                _killPlayerScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 达到得分获胜
        /// </summary>
        public bool ScoreWinCondition { 
            get { return _scoreWinCondition; }
            set { if (_scoreWinCondition != value) {
                _scoreWinCondition = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 无限生命
        /// </summary>
        public bool InfiniteLife { 
            get { return _infiniteLife; }
            set { if (_infiniteLife != value) {
                _infiniteLife = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_NetBattleData msg)
        {
            if (null == msg) return false;
            _harmType = msg.HarmType;     
            _timeLimit = msg.TimeLimit;     
            _playerCount = msg.PlayerCount;     
            _lifeCount = msg.LifeCount;     
            _reviveTime = msg.ReviveTime;     
            _reviveInvincibleTime = msg.ReviveInvincibleTime;     
            _reviveType = msg.ReviveType;     
            _timeWinCondition = msg.TimeWinCondition;     
            _winScore = msg.WinScore;     
            _arriveScore = msg.ArriveScore;     
            _collectGemScore = msg.CollectGemScore;     
            _killMonsterScore = msg.KillMonsterScore;     
            _killPlayerScore = msg.KillPlayerScore;     
            _scoreWinCondition = msg.ScoreWinCondition;     
            _infiniteLife = msg.InfiniteLife;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_NetBattleData msg)
        {
            if (null == msg) return false;
            _harmType = msg.HarmType;           
            _timeLimit = msg.TimeLimit;           
            _playerCount = msg.PlayerCount;           
            _lifeCount = msg.LifeCount;           
            _reviveTime = msg.ReviveTime;           
            _reviveInvincibleTime = msg.ReviveInvincibleTime;           
            _reviveType = msg.ReviveType;           
            _timeWinCondition = msg.TimeWinCondition;           
            _winScore = msg.WinScore;           
            _arriveScore = msg.ArriveScore;           
            _collectGemScore = msg.CollectGemScore;           
            _killMonsterScore = msg.KillMonsterScore;           
            _killPlayerScore = msg.KillPlayerScore;           
            _scoreWinCondition = msg.ScoreWinCondition;           
            _infiniteLife = msg.InfiniteLife;           
            return true;
        } 

        public bool DeepCopy (NetBattleData obj)
        {
            if (null == obj) return false;
            _harmType = obj.HarmType;           
            _timeLimit = obj.TimeLimit;           
            _playerCount = obj.PlayerCount;           
            _lifeCount = obj.LifeCount;           
            _reviveTime = obj.ReviveTime;           
            _reviveInvincibleTime = obj.ReviveInvincibleTime;           
            _reviveType = obj.ReviveType;           
            _timeWinCondition = obj.TimeWinCondition;           
            _winScore = obj.WinScore;           
            _arriveScore = obj.ArriveScore;           
            _collectGemScore = obj.CollectGemScore;           
            _killMonsterScore = obj.KillMonsterScore;           
            _killPlayerScore = obj.KillPlayerScore;           
            _scoreWinCondition = obj.ScoreWinCondition;           
            _infiniteLife = obj.InfiniteLife;           
            return true;
        }

        public void OnSyncFromParent (Msg_NetBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NetBattleData (Msg_NetBattleData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NetBattleData () { 
        }
        #endregion
    }
}