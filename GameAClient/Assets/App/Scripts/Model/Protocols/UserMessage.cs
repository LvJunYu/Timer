//  | 留言
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMessage : SyncronisticData<Msg_UserMessage> {
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
        private int _replayCount;
        /// <summary>
        /// 
        /// </summary>
        private UserMessageReplay _firstReplay;
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
        public int ReplayCount { 
            get { return _replayCount; }
            set { if (_replayCount != value) {
                _replayCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public UserMessageReplay FirstReplay { 
            get { return _firstReplay; }
            set { if (_firstReplay != value) {
                _firstReplay = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_UserMessage msg)
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
            _replayCount = msg.ReplayCount;     
            if (null == _firstReplay) {
                _firstReplay = new UserMessageReplay(msg.FirstReplay);
            } else {
                _firstReplay.OnSyncFromParent(msg.FirstReplay);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_UserMessage msg)
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
            _replayCount = msg.ReplayCount;           
            if(null != msg.FirstReplay){
                if (null == _firstReplay){
                    _firstReplay = new UserMessageReplay(msg.FirstReplay);
                }
                _firstReplay.CopyMsgData(msg.FirstReplay);
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
            _replayCount = obj.ReplayCount;           
            if(null != obj.FirstReplay){
                if (null == _firstReplay){
                    _firstReplay = new UserMessageReplay();
                }
                _firstReplay.DeepCopy(obj.FirstReplay);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_UserMessage msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessage (Msg_UserMessage msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessage () { 
            _userInfo = new UserInfoSimple();
            _firstReplay = new UserMessageReplay();
        }
        #endregion
    }
}