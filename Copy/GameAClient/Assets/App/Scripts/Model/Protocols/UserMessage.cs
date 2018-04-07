// 获取留言数据 | 获取留言数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMessage : SyncronisticData<Msg_SC_DAT_UserMessage> {
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
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private string _content;
        /// <summary>
        /// 点赞数
        /// </summary>
        private int _likeNum;
        /// <summary>
        /// 已点赞
        /// </summary>
        private bool _userLike;
        /// <summary>
        /// 
        /// </summary>
        private int _replyCount;
        /// <summary>
        /// 
        /// </summary>
        private UserMessageReply _firstReply;

        // cs fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private long _cs_messageId;
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
        /// 点赞数
        /// </summary>
        public int LikeNum { 
            get { return _likeNum; }
            set { if (_likeNum != value) {
                _likeNum = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 已点赞
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
        public UserMessageReply FirstReply { 
            get { return _firstReply; }
            set { if (_firstReply != value) {
                _firstReply = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public long CS_MessageId { 
            get { return _cs_messageId; }
            set { _cs_messageId = value; }
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
		/// 获取留言数据
		/// </summary>
		/// <param name="messageId">.</param>
        public void Request (
            long messageId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_messageId != messageId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_messageId = messageId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserMessage msg = new Msg_CS_DAT_UserMessage();
                msg.MessageId = messageId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserMessage>(
                    SoyHttpApiPath.UserMessage, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserMessage msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if (null == _userInfo) {
                _userInfo = new UserInfoSimple(msg.UserInfo);
            } else {
                _userInfo.OnSyncFromParent(msg.UserInfo);
            }
            _createTime = msg.CreateTime;           
            _content = msg.Content;           
            _likeNum = msg.LikeNum;           
            _userLike = msg.UserLike;           
            _replyCount = msg.ReplyCount;           
            if (null == _firstReply) {
                _firstReply = new UserMessageReply(msg.FirstReply);
            } else {
                _firstReply.OnSyncFromParent(msg.FirstReply);
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserMessage msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            if(null != msg.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple(msg.UserInfo);
                }
                _userInfo.CopyMsgData(msg.UserInfo);
            }
            _createTime = msg.CreateTime;           
            _content = msg.Content;           
            _likeNum = msg.LikeNum;           
            _userLike = msg.UserLike;           
            _replyCount = msg.ReplyCount;           
            if(null != msg.FirstReply){
                if (null == _firstReply){
                    _firstReply = new UserMessageReply(msg.FirstReply);
                }
                _firstReply.CopyMsgData(msg.FirstReply);
            }
            return true;
        } 

        public bool DeepCopy (UserMessage obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            if(null != obj.UserInfo){
                if (null == _userInfo){
                    _userInfo = new UserInfoSimple();
                }
                _userInfo.DeepCopy(obj.UserInfo);
            }
            _createTime = obj.CreateTime;           
            _content = obj.Content;           
            _likeNum = obj.LikeNum;           
            _userLike = obj.UserLike;           
            _replyCount = obj.ReplyCount;           
            if(null != obj.FirstReply){
                if (null == _firstReply){
                    _firstReply = new UserMessageReply();
                }
                _firstReply.DeepCopy(obj.FirstReply);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserMessage msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessage (Msg_SC_DAT_UserMessage msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessage () { 
            _userInfo = new UserInfoSimple();
            _firstReply = new UserMessageReply();
            OnCreate();
        }
        #endregion
    }
}