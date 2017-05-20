// 获取社交用户列表 | 获取社交用户列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RelationUserList : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ECachedDataState
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updataTime;
        /// <summary>
        /// 
        /// </summary>
        private List<UserInfoSimple> _dataList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        /// <summary>
        /// 
        /// </summary>
        private ERelationUserType _cs_dataType;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_startInx;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;
        /// <summary>
        /// 
        /// </summary>
        private ERelationUserOrderBy _cs_orderBy;
        /// <summary>
        /// 升序降序
        /// </summary>
        private EOrderType _cs_orderType;
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
        public long UpdataTime { 
            get { return _updataTime; }
            set { if (_updataTime != value) {
                _updataTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public List<UserInfoSimple> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ERelationUserType CS_DataType { 
            get { return _cs_dataType; }
            set { _cs_dataType = value; }
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
        /// <summary>
        /// 
        /// </summary>
        public ERelationUserOrderBy CS_OrderBy { 
            get { return _cs_orderBy; }
            set { _cs_orderBy = value; }
        }
        /// <summary>
        /// 升序降序
        /// </summary>
        public EOrderType CS_OrderType { 
            get { return _cs_orderType; }
            set { _cs_orderType = value; }
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
		/// 获取社交用户列表
		/// </summary>
		/// <param name="userId">用户id.</param>
		/// <param name="dataType">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="orderBy">.</param>
		/// <param name="orderType">升序降序.</param>
        public void Request (
            long userId,
            ERelationUserType dataType,
            int startInx,
            int maxCount,
            ERelationUserOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_dataType != dataType) {
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
                if (_cs_orderBy != orderBy) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_orderType != orderType) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                _cs_dataType = dataType;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_orderBy = orderBy;
                _cs_orderType = orderType;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_RelationUserList msg = new Msg_CS_DAT_RelationUserList();
                msg.UserId = userId;
                msg.DataType = dataType;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.OrderBy = orderBy;
                msg.OrderType = orderType;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_RelationUserList>(
                    SoyHttpApiPath.RelationUserList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_RelationUserList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updataTime = msg.UpdataTime;           
            _dataList = new List<UserInfoSimple>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new UserInfoSimple(msg.DataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_RelationUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RelationUserList (Msg_SC_DAT_RelationUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RelationUserList () { 
            _dataList = new List<UserInfoSimple>();
        }
        #endregion
    }
}