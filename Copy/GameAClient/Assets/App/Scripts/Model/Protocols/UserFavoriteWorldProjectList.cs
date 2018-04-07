// 用户收藏的关卡 | 用户收藏的关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserFavoriteWorldProjectList : SyncronisticData<Msg_SC_DAT_UserFavoriteWorldProjectList> {
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
        private List<Project> _projectList;

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
        /// <summary>
        /// 
        /// </summary>
        private int _cs_projectTypeMask;
        /// <summary>
        /// 排序字段
        /// </summary>
        private EFavoriteProjectOrderBy _cs_orderBy;
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
        public List<Project> ProjectList { 
            get { return _projectList; }
            set { if (_projectList != value) {
                _projectList = value;
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
        /// <summary>
        /// 
        /// </summary>
        public int CS_ProjectTypeMask { 
            get { return _cs_projectTypeMask; }
            set { _cs_projectTypeMask = value; }
        }
        /// <summary>
        /// 排序字段
        /// </summary>
        public EFavoriteProjectOrderBy CS_OrderBy { 
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
                if (null != _projectList) {
                    for (int i = 0; i < _projectList.Count; i++) {
                        if (null != _projectList[i] && _projectList[i].IsDirty) {
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
		/// 用户收藏的关卡
		/// </summary>
		/// <param name="userId">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="projectTypeMask">.</param>
		/// <param name="orderBy">排序字段.</param>
		/// <param name="orderType">升序降序.</param>
        public void Request (
            long userId,
            int startInx,
            int maxCount,
            int projectTypeMask,
            EFavoriteProjectOrderBy orderBy,
            EOrderType orderType,
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
                if (_cs_projectTypeMask != projectTypeMask) {
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
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_projectTypeMask = projectTypeMask;
                _cs_orderBy = orderBy;
                _cs_orderType = orderType;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserFavoriteWorldProjectList msg = new Msg_CS_DAT_UserFavoriteWorldProjectList();
                msg.UserId = userId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.ProjectTypeMask = projectTypeMask;
                msg.OrderBy = orderBy;
                msg.OrderType = orderType;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserFavoriteWorldProjectList>(
                    SoyHttpApiPath.UserFavoriteWorldProjectList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserFavoriteWorldProjectList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _projectList = new List<Project>();
            for (int i = 0; i < msg.ProjectList.Count; i++) {
                _projectList.Add(new Project(msg.ProjectList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserFavoriteWorldProjectList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _projectList) {
                _projectList = new List<Project>();
            }
            _projectList.Clear();
            for (int i = 0; i < msg.ProjectList.Count; i++) {
                _projectList.Add(new Project(msg.ProjectList[i]));
            }
            return true;
        } 

        public bool DeepCopy (UserFavoriteWorldProjectList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.ProjectList) return false;
            if (null ==  _projectList) {
                _projectList = new List<Project>();
            }
            _projectList.Clear();
            for (int i = 0; i < obj.ProjectList.Count; i++){
                _projectList.Add(obj.ProjectList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserFavoriteWorldProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserFavoriteWorldProjectList (Msg_SC_DAT_UserFavoriteWorldProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserFavoriteWorldProjectList () { 
            _projectList = new List<Project>();
            OnCreate();
        }
        #endregion
    }
}