// 获取关卡评论数据 | 获取关卡评论数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectComment : SyncronisticData<Msg_SC_DAT_ProjectComment> {
        #region 字段
        // sc fields----------------------------------
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
        private string _comment;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        /// <summary>
        /// 
        /// </summary>
        private long _projectMainId;
        /// <summary>
        /// 
        /// </summary>
        private int _projectVersion;
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

        // cs fields----------------------------------
        /// <summary>
        /// 评论Id
        /// </summary>
        private long _cs_id;
        #endregion

        #region 属性
        // sc properties----------------------------------
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
        public long ProjectMainId { 
            get { return _projectMainId; }
            set { if (_projectMainId != value) {
                _projectMainId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectVersion { 
            get { return _projectVersion; }
            set { if (_projectVersion != value) {
                _projectVersion = value;
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
        
        // cs properties----------------------------------
        /// <summary>
        /// 评论Id
        /// </summary>
        public long CS_Id { 
            get { return _cs_id; }
            set { _cs_id = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userInfo && _userInfo.IsDirty) {
                    return true;
                }
                if (null != _firstReply && _firstReply.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取关卡评论数据
		/// </summary>
		/// <param name="id">评论Id.</param>
        public void Request (
            long id,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_id != id) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_id = id;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_ProjectComment msg = new Msg_CS_DAT_ProjectComment();
                msg.Id = id;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_ProjectComment>(
                    SoyHttpApiPath.ProjectComment, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_ProjectComment msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _comment = msg.Comment;           
            _projectId = msg.ProjectId;           
            _projectMainId = msg.ProjectMainId;           
            _projectVersion = msg.ProjectVersion;           
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
        
        public bool CopyMsgData (Msg_SC_DAT_ProjectComment msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple(msg.UserInfo);
                }
                _userInfo.CopyMsgData(msg.UserInfo);
            }
            _comment = msg.Comment;           
            _projectId = msg.ProjectId;           
            _projectMainId = msg.ProjectMainId;           
            _projectVersion = msg.ProjectVersion;           
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
            _comment = obj.Comment;           
            _projectId = obj.ProjectId;           
            _projectMainId = obj.ProjectMainId;           
            _projectVersion = obj.ProjectVersion;           
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

        public void OnSyncFromParent (Msg_SC_DAT_ProjectComment msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectComment (Msg_SC_DAT_ProjectComment msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectComment () { 
            _userInfo = new UserInfoSimple();
            _firstReply = new ProjectCommentReply();
            OnCreate();
        }
        #endregion
    }
}