//  | 获取冒险关卡用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserLevelData : SyncronisticData {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private long _highScore;
        /// <summary>
        /// 
        /// </summary>
        private bool _star1Flag;
        /// <summary>
        /// 
        /// </summary>
        private bool _star2Flag;
        /// <summary>
        /// 
        /// </summary>
        private bool _star3Flag;
        /// <summary>
        /// 
        /// </summary>
        private int _challengeCount;
        /// <summary>
        /// 
        /// </summary>
        private int _successCount;
        /// <summary>
        /// 
        /// </summary>
        private int _failureCount;
        /// <summary>
        /// 
        /// </summary>
        private long _lastPlayTime;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public long HighScore { 
            get { return _highScore; }
            set { if (_highScore != value) {
                _highScore = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Star1Flag { 
            get { return _star1Flag; }
            set { if (_star1Flag != value) {
                _star1Flag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Star2Flag { 
            get { return _star2Flag; }
            set { if (_star2Flag != value) {
                _star2Flag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Star3Flag { 
            get { return _star3Flag; }
            set { if (_star3Flag != value) {
                _star3Flag = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ChallengeCount { 
            get { return _challengeCount; }
            set { if (_challengeCount != value) {
                _challengeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int SuccessCount { 
            get { return _successCount; }
            set { if (_successCount != value) {
                _successCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int FailureCount { 
            get { return _failureCount; }
            set { if (_failureCount != value) {
                _failureCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long LastPlayTime { 
            get { return _lastPlayTime; }
            set { if (_lastPlayTime != value) {
                _lastPlayTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AdventureUserLevelData msg)
        {
            if (null == msg) return false;
            _highScore = msg.HighScore;     
            _star1Flag = msg.Star1Flag;     
            _star2Flag = msg.Star2Flag;     
            _star3Flag = msg.Star3Flag;     
            _challengeCount = msg.ChallengeCount;     
            _successCount = msg.SuccessCount;     
            _failureCount = msg.FailureCount;     
            _lastPlayTime = msg.LastPlayTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AdventureUserLevelData obj)
        {
            if (null == obj) return false;
            _highScore = obj.HighScore;           
            _star1Flag = obj.Star1Flag;           
            _star2Flag = obj.Star2Flag;           
            _star3Flag = obj.Star3Flag;           
            _challengeCount = obj.ChallengeCount;           
            _successCount = obj.SuccessCount;           
            _failureCount = obj.FailureCount;           
            _lastPlayTime = obj.LastPlayTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_AdventureUserLevelData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserLevelData (Msg_AdventureUserLevelData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserLevelData () { 
        }
        #endregion
    }
}