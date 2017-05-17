//  | 关卡用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectUserData : SyncronisticData {
        #region 字段
        /// <summary>
        /// 胜利条件枚举
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 
        /// </summary>
        private long _userId;
        /// <summary>
        /// 
        /// </summary>
        private EProjectLikeState _likeState;
        /// <summary>
        /// 
        /// </summary>
        private bool _favorite;
        /// <summary>
        /// 
        /// </summary>
        private int _completeCount;
        /// <summary>
        /// 
        /// </summary>
        private long _lastPlayTime;
        #endregion

        #region 属性
        /// <summary>
        /// 胜利条件枚举
        /// </summary>
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public EProjectLikeState LikeState { 
            get { return _likeState; }
            set { if (_likeState != value) {
                _likeState = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Favorite { 
            get { return _favorite; }
            set { if (_favorite != value) {
                _favorite = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int CompleteCount { 
            get { return _completeCount; }
            set { if (_completeCount != value) {
                _completeCount = value;
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
        public bool OnSync (Msg_ProjectUserData msg)
        {
            if (null == msg) return false;
            _projectId = msg.ProjectId;     
            _userId = msg.UserId;     
            _likeState = msg.LikeState;     
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