// 最近玩过 | 最近玩过
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserWorldProjectPlayHistoryList : SyncronisticData {
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
        /// 
        /// </summary>
        private List<ProjectPlayHistory> _historyList;

        // cs fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private long _cs_userId;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_startInx;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;
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
        /// 
        /// </summary>
        public List<ProjectPlayHistory> HistoryList { 
            get { return _historyList; }
            set { if (_historyList != value) {
                _historyList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CS_StartInx { 
            get { return _cs_startInx; }
            set { _cs_startInx = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CS_MaxCount { 
            get { return _cs_maxCount; }
            set { _cs_maxCount = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _historyList) {
                    for (int i = 0; i < _historyList.Count; i++) {
                        if (null != _historyList[i] && _historyList[i].IsDirty) {
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
		/// 最近玩过
		/// </summary>
		/// <param name="userId">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            long userId,
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_startInx != startInx) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxCount != maxCount) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserWorldProjectPlayHistoryList msg = new Msg_CS_DAT_UserWorldProjectPlayHistoryList();
                msg.UserId = userId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserWorldProjectPlayHistoryList>(
                    SoyHttpApiPath.UserWorldProjectPlayHistoryList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserWorldProjectPlayHistoryList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _historyList = new List<ProjectPlayHistory>();
            for (int i = 0; i < msg.HistoryList.Count; i++) {
                _historyList.Add(new ProjectPlayHistory(msg.HistoryList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserWorldProjectPlayHistoryList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserWorldProjectPlayHistoryList (Msg_SC_DAT_UserWorldProjectPlayHistoryList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserWorldProjectPlayHistoryList () { 
            _historyList = new List<ProjectPlayHistory>();
        }
        #endregion
    }
}