// 工坊关卡 | 用户发布的关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserPublishedProjectList : SyncronisticData {
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
        /// 排序
        /// </summary>
        private EPublishedProjectOrderBy _cs_orderBy;
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
        /// 排序
        /// </summary>
        public EPublishedProjectOrderBy CS_OrderBy { 
            get { return _cs_orderBy; }
            set { _cs_orderBy = value; }
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
		/// 用户发布的关卡
		/// </summary>
		/// <param name="userId">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="orderBy">排序.</param>
        public void Request (
            long userId,
            int startInx,
            int maxCount,
            EPublishedProjectOrderBy orderBy,
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
                if (_cs_orderBy != orderBy) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_orderBy = orderBy;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserPublishedProjectList msg = new Msg_CS_DAT_UserPublishedProjectList();
                msg.UserId = userId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.OrderBy = orderBy;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserPublishedProjectList>(
                    SoyHttpApiPath.UserPublishedProjectList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserPublishedProjectList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _projectList = new List<Project>();
            for (int i = 0; i < msg.ProjectList.Count; i++) {
                _projectList.Add(new Project(msg.ProjectList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserPublishedProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserPublishedProjectList (Msg_SC_DAT_UserPublishedProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserPublishedProjectList () { 
            _projectList = new List<Project>();
        }
        #endregion
    }
}