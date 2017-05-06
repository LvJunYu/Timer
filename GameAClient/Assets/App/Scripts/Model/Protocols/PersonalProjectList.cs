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
        private long _cs_minUpdateTime;
        /// <summary>
        /// 
        /// </summary>
        private long _cs_maxUpdateTime;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_maxCount;
        /// <summary>
        /// 
        /// </summary>
        private int _cs_totalCount;
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
        public long CS_MinUpdateTime { 
            get { return _cs_minUpdateTime; }
            set { _cs_minUpdateTime = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long CS_MaxUpdateTime { 
            get { return _cs_maxUpdateTime; }
            set { _cs_maxUpdateTime = value; }
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
        public int CS_TotalCount { 
            get { return _cs_totalCount; }
            set { _cs_totalCount = value; }
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
		/// <param name="minUpdateTime">.</param>
		/// <param name="maxUpdateTime">.</param>
		/// <param name="maxCount">.</param>
		/// <param name="totalCount">.</param>
        public void Request (
            long minUpdateTime,
            long maxUpdateTime,
            int maxCount,
            int totalCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_minUpdateTime != minUpdateTime) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxUpdateTime != maxUpdateTime) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_maxCount != maxCount) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_totalCount != totalCount) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_minUpdateTime = minUpdateTime;
                _cs_maxUpdateTime = maxUpdateTime;
                _cs_maxCount = maxCount;
                _cs_totalCount = totalCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_PersonalProjectList msg = new Msg_CS_DAT_PersonalProjectList();
                msg.MinUpdateTime = minUpdateTime;
                msg.MaxUpdateTime = maxUpdateTime;
                msg.MaxCount = maxCount;
                msg.TotalCount = totalCount;
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