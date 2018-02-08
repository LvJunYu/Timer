// 获取快捷聊天列表 | 获取快捷聊天列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RoomChatPreinstallList : SyncronisticData<Msg_SC_DAT_RoomChatPreinstallList> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 预设列表
        /// </summary>
        private List<RoomChatPreinstall> _dataList;
        /// <summary>
        /// 
        /// </summary>
        private int _totalCount;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id（占位）
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 预设列表
        /// </summary>
        public List<RoomChatPreinstall> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户id（占位）
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _dataList) {
                    for (int i = 0; i < _dataList.Count; i++) {
                        if (null != _dataList[i] && _dataList[i].IsDirty) {
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
		/// 获取快捷聊天列表
		/// </summary>
		/// <param name="userId">用户id（占位）.</param>
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

                Msg_CS_DAT_RoomChatPreinstallList msg = new Msg_CS_DAT_RoomChatPreinstallList();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_RoomChatPreinstallList>(
                    SoyHttpApiPath.RoomChatPreinstallList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_RoomChatPreinstallList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _dataList = new List<RoomChatPreinstall>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new RoomChatPreinstall(msg.DataList[i]));
            }
            _totalCount = msg.TotalCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_RoomChatPreinstallList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _dataList) {
                _dataList = new List<RoomChatPreinstall>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new RoomChatPreinstall(msg.DataList[i]));
            }
            _totalCount = msg.TotalCount;           
            return true;
        } 

        public bool DeepCopy (RoomChatPreinstallList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<RoomChatPreinstall>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            _totalCount = obj.TotalCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_RoomChatPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RoomChatPreinstallList (Msg_SC_DAT_RoomChatPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RoomChatPreinstallList () { 
            _dataList = new List<RoomChatPreinstall>();
            OnCreate();
        }
        #endregion
    }
}