// 角色可以使用的时装数据 | 角色可以使用的时装数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ValidAvatarPart : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        // 用户
        private long _userId;
        // 
        private List<AvatarPartItem> _itemDataList;

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
        public List<AvatarPartItem> ItemDataList { 
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
		/// 角色可以使用的时装数据
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_ValidAvatarPart msg = new Msg_CS_DAT_ValidAvatarPart();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_ValidAvatarPart>(
                SoyHttpApiPath.ValidAvatarPart, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed();
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_ValidAvatarPart msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _itemDataList = new List<AvatarPartItem>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new AvatarPartItem(msg.ItemDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_ValidAvatarPart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ValidAvatarPart (Msg_SC_DAT_ValidAvatarPart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public ValidAvatarPart () { 
            _itemDataList = new List<AvatarPartItem>();
        }
        #endregion
    }
}