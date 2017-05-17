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
        // ECachedDataState
        private int _resultCode;
        // 
        private long _updateTime;
        // 
        private List<ProjectPlayHistory> _historyList;

        // cs fields----------------------------------
        // 
        private long _cs_userId;
        // 
        private int _cs_startInx;
        // 
        private int _cs_maxCount;
        // 排序字段
        private EProjectPlayHistoryOrderBy _cs_orderBy;
        // 升序降序
        private EOrderType _cs_orderType;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // ECachedDataState
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        // 
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        // 
        public List<ProjectPlayHistory> HistoryList { 
            get { return _historyList; }
            set { if (_historyList != value) {
                _historyList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }
        // 
        public int CS_StartInx { 
            get { return _cs_startInx; }
            set { _cs_startInx = value; }
        }
        // 
        public int CS_MaxCount { 
            get { return _cs_maxCount; }
            set { _cs_maxCount = value; }
        }
        // 排序字段
        public EProjectPlayHistoryOrderBy CS_OrderBy { 
            get { return _cs_orderBy; }
            set { _cs_orderBy = value; }
        }
        // 升序降序
        public EOrderType CS_OrderType { 
            get { return _cs_orderType; }
            set { _cs_orderType = value; }
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
		/// <param name="orderBy">排序字段.</param>
		/// <param name="orderType">升序降序.</param>
        public void Request (
            long userId,
            int startInx,
            int maxCount,
            EProjectPlayHistoryOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserWorldProjectPlayHistoryList msg = new Msg_CS_DAT_UserWorldProjectPlayHistoryList();
            msg.UserId = userId;
            msg.StartInx = startInx;
            msg.MaxCount = maxCount;
            msg.OrderBy = orderBy;
            msg.OrderType = orderType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserWorldProjectPlayHistoryList>(
                SoyHttpApiPath.UserWorldProjectPlayHistoryList, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
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