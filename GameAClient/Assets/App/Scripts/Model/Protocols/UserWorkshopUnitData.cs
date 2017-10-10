// 工坊可用地块数量 | Msg_CS_DAT_UserWorkshopUnitData
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserWorkshopUnitData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _userId;
        /// <summary>
        /// 数据列表
        /// </summary>
        private List<WorkshopUnitItem> _itemList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<WorkshopUnitItem> ItemList { 
            get { return _itemList; }
            set { if (_itemList != value) {
                _itemList = value;
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
                if (null != _itemList) {
                    for (int i = 0; i < _itemList.Count; i++) {
                        if (null != _itemList[i] && _itemList[i].IsDirty) {
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
		/// Msg_CS_DAT_UserWorkshopUnitData
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

                Msg_CS_DAT_UserWorkshopUnitData msg = new Msg_CS_DAT_UserWorkshopUnitData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserWorkshopUnitData>(
                    SoyHttpApiPath.UserWorkshopUnitData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserWorkshopUnitData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _itemList = new List<WorkshopUnitItem>();
            for (int i = 0; i < msg.ItemList.Count; i++) {
                _itemList.Add(new WorkshopUnitItem(msg.ItemList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (UserWorkshopUnitData obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            if (null ==  obj.ItemList) return false;
            if (null ==  _itemList) {
                _itemList = new List<WorkshopUnitItem>();
            }
            _itemList.Clear();
            for (int i = 0; i < obj.ItemList.Count; i++){
                _itemList.Add(obj.ItemList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserWorkshopUnitData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserWorkshopUnitData (Msg_SC_DAT_UserWorkshopUnitData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserWorkshopUnitData () { 
            _itemList = new List<WorkshopUnitItem>();
            OnCreate();
        }
        #endregion
    }
}