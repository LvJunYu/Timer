// 家园装饰 | 家园装饰
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class HomePartData : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 用户
        private long _userId;
        // 
        private List<HomePartItem> _itemDataList;

        // cs fields----------------------------------
        // 用户
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 用户
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 
        public List<HomePartItem> ItemDataList { 
            get { return _itemDataList; }
            set { if (_itemDataList != value) {
                _itemDataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户
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
		/// 家园装饰
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_HomePartData msg = new Msg_CS_DAT_HomePartData();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_HomePartData>(
                SoyHttpApiPath.HomePartData, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_HomePartData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _itemDataList = new List<HomePartItem>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new HomePartItem(msg.ItemDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_HomePartData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public HomePartData (Msg_SC_DAT_HomePartData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public HomePartData () { 
            _itemDataList = new List<HomePartItem>();
        }
        #endregion
    }
}