//  | 评论回复
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectCommentReply : SyncronisticData<Msg_ProjectCommentReply> {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 所在评论ID
        /// </summary>
        private long _commentId;
        /// <summary>
        /// 发送者信息
        /// </summary>
        private UserInfoSimple _userInfo;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private string _content;
        /// <summary>
        /// 回复对象信息
        /// </summary>
        private UserInfoSimple _targetUserInfo;
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
        /// 所在评论ID
        /// </summary>
        public long CommentId { 
            get { return _commentId; }
            set { if (_commentId != value) {
                _commentId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 发送者信息
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
        public string Content { 
            get { return _content; }
            set { if (_content != value) {
                _content = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 回复对象信息
        /// </summary>
        public UserInfoSimple TargetUserInfo { 
            get { return _targetUserInfo; }
            set { if (_targetUserInfo != value) {
                _targetUserInfo = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_ProjectCommentReply msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _commentId = msg.CommentId;     
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _createTime = msg.CreateTime;     
            _content = msg.Content;     
            if (null == _targetUserInfo) {
                _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
            } else {
                _targetUserInfo.OnSyncFromParent(msg.TargetUserInfo);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_ProjectCommentReply msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _commentId = msg.CommentId;           
            if(null != msg.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple(msg.UserInfo);
                }
                _userInfo.CopyMsgData(msg.UserInfo);
            }
            _createTime = msg.CreateTime;           
            _content = msg.Content;           
            if(null != msg.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
                }
                _targetUserInfo.CopyMsgData(msg.TargetUserInfo);
            }
            return true;
        } 

        public bool DeepCopy (ProjectCommentReply obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _commentId = obj.CommentId;           
            if(null != obj.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple();
                }
                _userInfo.DeepCopy(obj.UserInfo);
            }
            _createTime = obj.CreateTime;           
            _content = obj.Content;           
            if(null != obj.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple();
                }
                _targetUserInfo.DeepCopy(obj.TargetUserInfo);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_ProjectCommentReply msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectCommentReply (Msg_ProjectCommentReply msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ProjectCommentReply () { 
            _userInfo = new UserInfoSimple();
            _targetUserInfo = new UserInfoSimple();
        }
        #endregion
    }
}