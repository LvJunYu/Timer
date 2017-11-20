// 工坊关卡 | 工坊关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PersonalProjectList : SyncronisticData<Msg_SC_DAT_PersonalProjectList> {
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
        private int _cs_startInx;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;
        /// <summary>
        /// 排序字段
        /// </summary>
        private EPersonalProjectOrderBy _cs_orderBy;
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
        /// 排序字段
        /// </summary>
        public EPersonalProjectOrderBy CS_OrderBy { 
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
		/// 工坊关卡
		/// </summary>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="orderBy">排序字段.</param>
		/// <param name="orderType">升序降序.</param>
        public void Request (
            int startInx,
            int maxCount,
            EPersonalProjectOrderBy orderBy,
            EOrderType orderType,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
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
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_orderBy = orderBy;
                _cs_orderType = orderType;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_PersonalProjectList msg = new Msg_CS_DAT_PersonalProjectList();
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.OrderBy = orderBy;
                msg.OrderType = orderType;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_PersonalProjectList>(
                    SoyHttpApiPath.PersonalProjectList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_PersonalProjectList msg)
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
        
        public bool CopyMsgData (Msg_SC_DAT_PersonalProjectList msg)
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

        public bool DeepCopy (PersonalProjectList obj)
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

        public void OnSyncFromParent (Msg_SC_DAT_PersonalProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PersonalProjectList (Msg_SC_DAT_PersonalProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PersonalProjectList () { 
            _projectList = new List<Project>();
            OnCreate();
        }
        #endregion
    }
}