//  | 留言回复
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMessageReply : SyncronisticData<Msg_UserMessageReply> {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 所在留言ID
        /// </summary>
        private long _messageId;
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
        /// 所在留言ID
        /// </summary>
        public long MessageId { 
            get { return _messageId; }
            set { if (_messageId != value) {
                _messageId = value;
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
        public bool OnSync (Msg_UserMessageReply msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _messageId = msg.MessageId;     
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

        public bool CopyMsgData (Msg_UserMessageReply msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _messageId = msg.MessageId;           
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

        public bool DeepCopy (UserMessageReply obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _messageId = obj.MessageId;           
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

        public void OnSyncFromParent (Msg_UserMessageReply msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessageReply (Msg_UserMessageReply msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessageReply () { 
            _userInfo = new UserInfoSimple();
            _targetUserInfo = new UserInfoSimple();
        }
        #endregion
    }
}