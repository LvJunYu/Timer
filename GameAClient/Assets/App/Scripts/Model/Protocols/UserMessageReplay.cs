//  | 留言回复
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMessageReplay : SyncronisticData<Msg_UserMessageReplay> {
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
        /// 回复他人
        /// </summary>
        private bool _relayOther;
        /// <summary>
        /// 回复对象
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
        /// 回复他人
        /// </summary>
        public bool RelayOther { 
            get { return _relayOther; }
            set { if (_relayOther != value) {
                _relayOther = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 回复对象
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
        public bool OnSync (Msg_UserMessageReplay msg)
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
            _relayOther = msg.RelayOther;     
            if (null == _targetUserInfo) {
                _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
            } else {
                _targetUserInfo.OnSyncFromParent(msg.TargetUserInfo);
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_UserMessageReplay msg)
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
            _relayOther = msg.RelayOther;           
            if(null != msg.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple(msg.TargetUserInfo);
                }
                _targetUserInfo.CopyMsgData(msg.TargetUserInfo);
            }
            return true;
        } 

        public bool DeepCopy (UserMessageReplay obj)
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
            _relayOther = obj.RelayOther;           
            if(null != obj.TargetUserInfo){
                if (null == _targetUserInfo){
                    _targetUserInfo = new UserInfoSimple();
                }
                _targetUserInfo.DeepCopy(obj.TargetUserInfo);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_UserMessageReplay msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessageReplay (Msg_UserMessageReplay msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMessageReplay () { 
            _userInfo = new UserInfoSimple();
            _targetUserInfo = new UserInfoSimple();
        }
        #endregion
    }
}