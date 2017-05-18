//  | 最近玩过
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectPlayHistory : SyncronisticData {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private Project _data;
        /// <summary>
        /// 
        /// </summary>
        private long _lastPlayTime;
        /// <summary>
        /// 
        /// </summary>
        private int _completeCount;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public Project Data { 
            get { return _data; }
            set { if (_data != value) {
                _data = value;
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
        #endregion

        #region 方法
        public bool OnSync (Msg_ProjectPlayHistory msg)
        {
            if (null == msg) return false;
            if (null == _data) {
                _data = new Project(msg.Data);
            } else {
                _data.OnSyncFromParent(msg.Data);
            }
            _lastPlayTime = msg.LastPlayTime;     
            _completeCount = msg.CompleteCount;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectPlayHistory msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectPlayHistory (Msg_ProjectPlayHistory msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectPlayHistory () { 
            _data = new Project();
        }
        #endregion
    }
}