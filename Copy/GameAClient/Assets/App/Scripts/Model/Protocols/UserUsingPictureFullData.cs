// 角色装备的拼图数据 | 角色装备的拼图数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserUsingPictureFullData : SyncronisticData<Msg_SC_DAT_UserUsingPictureFullData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _userId;
        /// <summary>
        /// 
        /// </summary>
        private List<PictureFull> _itemDataList;
        /// <summary>
        /// 槽位数
        /// </summary>
        private int _slotCount;

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
        public List<PictureFull> ItemDataList { 
            get { return _itemDataList; }
            set { if (_itemDataList != value) {
                _itemDataList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 槽位数
        /// </summary>
        public int SlotCount { 
            get { return _slotCount; }
            set { if (_slotCount != value) {
                _slotCount = value;
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
		/// 角色装备的拼图数据
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

                Msg_CS_DAT_UserUsingPictureFullData msg = new Msg_CS_DAT_UserUsingPictureFullData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserUsingPictureFullData>(
                    SoyHttpApiPath.UserUsingPictureFullData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserUsingPictureFullData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _itemDataList = new List<PictureFull>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new PictureFull(msg.ItemDataList[i]));
            }
            _slotCount = msg.SlotCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserUsingPictureFullData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            if (null ==  _itemDataList) {
                _itemDataList = new List<PictureFull>();
            }
            _itemDataList.Clear();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new PictureFull(msg.ItemDataList[i]));
            }
            _slotCount = msg.SlotCount;           
            return true;
        } 

        public bool DeepCopy (UserUsingPictureFullData obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            if (null ==  obj.ItemDataList) return false;
            if (null ==  _itemDataList) {
                _itemDataList = new List<PictureFull>();
            }
            _itemDataList.Clear();
            for (int i = 0; i < obj.ItemDataList.Count; i++){
                _itemDataList.Add(obj.ItemDataList[i]);
            }
            _slotCount = obj.SlotCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserUsingPictureFullData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserUsingPictureFullData (Msg_SC_DAT_UserUsingPictureFullData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserUsingPictureFullData () { 
            _itemDataList = new List<PictureFull>();
            OnCreate();
        }
        #endregion
    }
}