// 搜索好友 | 搜索好友
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class SearchUser : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 结果
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 用户
        /// </summary>
        private UserInfoSimple _data;

        // cs fields----------------------------------
        /// <summary>
        /// 用户名
        /// </summary>
        private string _cs_userNickName;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 结果
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 用户
        /// </summary>
        public UserInfoSimple Data { 
            get { return _data; }
            set { if (_data != value) {
                _data = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户名
        /// </summary>
        public string CS_UserNickName { 
            get { return _cs_userNickName; }
            set { _cs_userNickName = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _data && _data.IsDirty) {
                    return true;
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 搜索好友
		/// </summary>
		/// <param name="userNickName">用户名.</param>
        public void Request (
            string userNickName,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userNickName != userNickName) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userNickName = userNickName;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_SearchUser msg = new Msg_CS_DAT_SearchUser();
                msg.UserNickName = userNickName;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_SearchUser>(
                    SoyHttpApiPath.SearchUser, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_SearchUser msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            if (null == _data) {
                _data = new UserInfoSimple(msg.Data);
            } else {
                _data.OnSyncFromParent(msg.Data);
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (SearchUser obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            if(null != obj.Data){
                if (null == _data){
                    _data = new UserInfoSimple();
                }
                _data.DeepCopy(obj.Data);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_SearchUser msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SearchUser (Msg_SC_DAT_SearchUser msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SearchUser () { 
            _data = new UserInfoSimple();
            OnCreate();
        }
        #endregion
    }
}