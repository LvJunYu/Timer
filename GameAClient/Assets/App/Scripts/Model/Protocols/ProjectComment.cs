//  | 评论
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectComment : SyncronisticData<Msg_ProjectComment> {
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
        /// <summary>
        /// 
        /// </summary>
        private int _likeCount;
        /// <summary>
        /// 
        /// </summary>
        private bool _userLike;
        /// <summary>
        /// 
        /// </summary>
        private int _replyCount;
        /// <summary>
        /// 
        /// </summary>
        private ProjectCommentReply _firstReply;
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
        /// <summary>
        /// 
        /// </summary>
        public int LikeCount { 
            get { return _likeCount; }
            set { if (_likeCount != value) {
                _likeCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UserLike { 
            get { return _userLike; }
            set { if (_userLike != value) {
                _userLike = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ReplyCount { 
            get { return _replyCount; }
            set { if (_replyCount != value) {
                _replyCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public ProjectCommentReply FirstReply { 
            get { return _firstReply; }
            set { if (_firstReply != value) {
                _firstReply = value;
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
            _likeCount = msg.LikeCount;     
            _userLike = msg.UserLike;     
            _replyCount = msg.ReplyCount;     
            if (null == _firstReply) {
                _firstReply = new ProjectCommentReply(msg.FirstReply);
            } else {
                _firstReply.OnSyncFromParent(msg.FirstReply);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_ProjectComment msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple(msg.UserInfo);
                }
                _userInfo.CopyMsgData(msg.UserInfo);
            }
            if(null != msg.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
                }
                _targetUserInfo.CopyMsgData(msg.TargetUserInfo);
            }
            _comment = msg.Comment;           
            _projectId = msg.ProjectId;           
            _createTime = msg.CreateTime;           
            _likeCount = msg.LikeCount;           
            _userLike = msg.UserLike;           
            _replyCount = msg.ReplyCount;           
            if(null != msg.FirstReply){
                if (null == _firstReply){
                    _firstReply = new ProjectCommentReply(msg.FirstReply);
                }
                _firstReply.CopyMsgData(msg.FirstReply);
            }
            return true;
        } 

        public bool DeepCopy (ProjectComment obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            if(null != obj.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple();
                }
                _userInfo.DeepCopy(obj.UserInfo);
            }
            if(null != obj.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple();
                }
                _targetUserInfo.DeepCopy(obj.TargetUserInfo);
            }
            _comment = obj.Comment;           
            _projectId = obj.ProjectId;           
            _createTime = obj.CreateTime;           
            _likeCount = obj.LikeCount;           
            _userLike = obj.UserLike;           
            _replyCount = obj.ReplyCount;           
            if(null != obj.FirstReply){
                if (null == _firstReply){
                    _firstReply = new ProjectCommentReply();
                }
                _firstReply.DeepCopy(obj.FirstReply);
            }
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
            _firstReply = new ProjectCommentReply();
        }
        #endregion
    }
}