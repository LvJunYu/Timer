// 用户道具数据 | 用户道具数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserProp : SyncronisticData<Msg_SC_DAT_UserProp> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _userId;
        /// <summary>
        /// 
        /// </summary>
        private List<PropItem> _itemDataList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<PropItem> ItemDataList { 
            get { return _itemDataList; }
            set { if (_itemDataList != value) {
                _itemDataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _itemDataList) {
                    for (int i = 0; i < _itemDataList.Count; i++) {
                        if (null != _itemDataList[i] && _itemDataList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 用户道具数据
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserProp msg = new Msg_CS_DAT_UserProp();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserProp>(
                    SoyHttpApiPath.UserProp, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserProp msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _itemDataList = new List<PropItem>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new PropItem(msg.ItemDataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserProp msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            if (null ==  _itemDataList) {
                _itemDataList = new List<PropItem>();
            }
            _itemDataList.Clear();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new PropItem(msg.ItemDataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (UserProp obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            if (null ==  obj.ItemDataList) return false;
            if (null ==  _itemDataList) {
                _itemDataList = new List<PropItem>();
            }
            _itemDataList.Clear();
            for (int i = 0; i < obj.ItemDataList.Count; i++){
                _itemDataList.Add(obj.ItemDataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserProp msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserProp (Msg_SC_DAT_UserProp msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserProp () { 
            _itemDataList = new List<PropItem>();
            OnCreate();
        }
        #endregion
    }
}