//  | 获取冒险关卡用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserLevelData : SyncronisticData {
        #region 字段
        // 
        private long _highScore;
        // 
        private bool _star1Flag;
        // 
        private bool _star2Flag;
        // 
        private bool _star3Flag;
        // 
        private int _challengeCount;
        // 
        private int _successCount;
        // 
        private int _failureCount;
        // 
        private long _lastPlayTime;
        #endregion

        #region 属性
        // 
        public long HighScore { 
            get { return _highScore; }
            set { if (_highScore != value) {
                _highScore = value;
                SetDirty();
            }}
        }
        // 
        public bool Star1Flag { 
            get { return _star1Flag; }
            set { if (_star1Flag != value) {
                _star1Flag = value;
                SetDirty();
            }}
        }
        // 
        public bool Star2Flag { 
            get { return _star2Flag; }
            set { if (_star2Flag != value) {
                _star2Flag = value;
                SetDirty();
            }}
        }
        // 
        public bool Star3Flag { 
            get { return _star3Flag; }
            set { if (_star3Flag != value) {
                _star3Flag = value;
                SetDirty();
            }}
        }
        // 
        public int ChallengeCount { 
            get { return _challengeCount; }
            set { if (_challengeCount != value) {
                _challengeCount = value;
                SetDirty();
            }}
        }
        // 
        public int SuccessCount { 
            get { return _successCount; }
            set { if (_successCount != value) {
                _successCount = value;
                SetDirty();
            }}
        }
        // 
        public int FailureCount { 
            get { return _failureCount; }
            set { if (_failureCount != value) {
                _failureCount = value;
                SetDirty();
            }}
        }
        // 
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