// 推荐关卡 | 推荐关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldOfficialRecommendProjectList : SyncronisticData<Msg_SC_DAT_WorldOfficialRecommendProjectList> {
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
        /// 起始下标
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
        /// 起始下标
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
		/// 推荐关卡
		/// </summary>
		/// <param name="startInx">起始下标.</param>
		/// <param name="maxCount">.</param>
		/// <param name="projectTypeMask">.</param>
        public void Request (
            int startInx,
            int maxCount,
            int projectTypeMask,
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
                if (_cs_projectTypeMask != projectTypeMask) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                _cs_projectTypeMask = projectTypeMask;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_WorldOfficialRecommendProjectList msg = new Msg_CS_DAT_WorldOfficialRecommendProjectList();
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                msg.ProjectTypeMask = projectTypeMask;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldOfficialRecommendProjectList>(
                    SoyHttpApiPath.WorldOfficialRecommendProjectList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_WorldOfficialRecommendProjectList msg)
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
        
        public bool CopyMsgData (Msg_SC_DAT_WorldOfficialRecommendProjectList msg)
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

        public bool DeepCopy (WorldOfficialRecommendProjectList obj)
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

        public void OnSyncFromParent (Msg_SC_DAT_WorldOfficialRecommendProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldOfficialRecommendProjectList (Msg_SC_DAT_WorldOfficialRecommendProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldOfficialRecommendProjectList () { 
            _projectList = new List<Project>();
            OnCreate();
        }
        #endregion
    }
}