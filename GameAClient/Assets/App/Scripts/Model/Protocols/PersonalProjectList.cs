// 工坊关卡 | 工坊关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PersonalProjectList : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        // ECachedDataState
        private int _resultCode;
        // 
        private long _updateTime;
        // 
        private List<Project> _projectList;

        // cs fields----------------------------------
        // 
        private int _cs_startInx;
        // 
        private int _cs_maxCount;
        // 排序字段
        private EPersonalProjectOrderBy _cs_orderBy;
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
        public List<Project> ProjectList { 
            get { return _projectList; }
            set { if (_projectList != value) {
                _projectList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
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
        public EPersonalProjectOrderBy CS_OrderBy { 
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

        public bool OnSync (Msg_SC_DAT_PersonalProjectList msg)
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
        }
        #endregion
    }
}