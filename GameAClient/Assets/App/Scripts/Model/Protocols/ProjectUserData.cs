//  | 关卡用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectUserData : SyncronisticData {
        #region 字段
        // 
        private long _projectId;
        // 
        private long _userId;
        // 
        private bool _likeFlag;
        // 
        private int _rate;
        // 
        private bool _favorite;
        // 
        private int _completeCount;
        // 
        private long _lastPlayTime;
        #endregion

        #region 属性
        // 
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        // 
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 
        public bool LikeFlag { 
            get { return _likeFlag; }
            set { if (_likeFlag != value) {
                _likeFlag = value;
                SetDirty();
            }}
        }
        // 
        public int Rate { 
            get { return _rate; }
            set { if (_rate != value) {
                _rate = value;
                SetDirty();
            }}
        }
        // 
        public bool Favorite { 
            get { return _favorite; }
            set { if (_favorite != value) {
                _favorite = value;
                SetDirty();
            }}
        }
        // 
        public int CompleteCount { 
            get { return _completeCount; }
            set { if (_completeCount != value) {
                _completeCount = value;
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
        public bool OnSync (Msg_ProjectUserData msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;     
            _userId = msg.UserId;     
            _likeFlag = msg.LikeFlag;     
            _rate = msg.Rate;     
            _favorite = msg.Favorite;     
            _completeCount = msg.CompleteCount;     
            _lastPlayTime = msg.LastPlayTime;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectUserData (Msg_ProjectUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectUserData () { 
        }
        #endregion
    }
}