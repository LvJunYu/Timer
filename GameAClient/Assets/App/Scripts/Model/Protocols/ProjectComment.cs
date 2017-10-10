//  | 评论
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectComment : SyncronisticData {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 
        /// </summary>
        private UserInfoSimple _targetUserInfo;
        /// <summary>
        /// 
        /// </summary>
        private string _comment;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple UserInfo { 
            get { return _userInfo; }
            set { if (_userInfo != value) {
                _userInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserInfoSimple TargetUserInfo { 
            get { return _targetUserInfo; }
            set { if (_targetUserInfo != value) {
                _targetUserInfo = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public string Comment { 
            get { return _comment; }
            set { if (_comment != value) {
                _comment = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
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
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_ProjectComment msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            if (null == _targetUserInfo) {
                _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
            } else {
                _targetUserInfo.OnSyncFromParent(msg.TargetUserInfo);
            }
            _comment = msg.Comment;     
            _projectId = msg.ProjectId;     
            _createTime = msg.CreateTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (ProjectComment obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectComment msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectComment (Msg_ProjectComment msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectComment () { 
            _userInfo = new UserInfoSimple();
            _targetUserInfo = new UserInfoSimple();
        }
        #endregion
    }
}