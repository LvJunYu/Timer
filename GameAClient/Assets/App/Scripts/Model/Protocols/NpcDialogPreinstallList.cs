// 获取预设NPC对话列表 | 获取预设NPC对话列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NpcDialogPreinstallList : SyncronisticData<Msg_SC_DAT_NpcDialogPreinstallList> {
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
        private List<NpcDialogPreinstall> _dataList;
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
        public List<NpcDialogPreinstall> DataList { 
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
		/// 获取预设NPC对话列表
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

                Msg_CS_DAT_NpcDialogPreinstallList msg = new Msg_CS_DAT_NpcDialogPreinstallList();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_NpcDialogPreinstallList>(
                    SoyHttpApiPath.NpcDialogPreinstallList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_NpcDialogPreinstallList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _dataList = new List<NpcDialogPreinstall>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new NpcDialogPreinstall(msg.DataList[i]));
            }
            _totalCount = msg.TotalCount;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_NpcDialogPreinstallList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _dataList) {
                _dataList = new List<NpcDialogPreinstall>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new NpcDialogPreinstall(msg.DataList[i]));
            }
            _totalCount = msg.TotalCount;           
            return true;
        } 

        public bool DeepCopy (NpcDialogPreinstallList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<NpcDialogPreinstall>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            _totalCount = obj.TotalCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_NpcDialogPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NpcDialogPreinstallList (Msg_SC_DAT_NpcDialogPreinstallList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NpcDialogPreinstallList () { 
            _dataList = new List<NpcDialogPreinstall>();
            OnCreate();
        }
        #endregion
    }
}