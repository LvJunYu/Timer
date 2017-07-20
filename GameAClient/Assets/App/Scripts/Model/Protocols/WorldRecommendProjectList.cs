// 推荐关卡 | 推荐关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldRecommendProjectList : SyncronisticData {
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
        /// 占位
        /// </summary>
        private int _cs_flag;
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
        /// 占位
        /// </summary>
        public int CS_Flag { 
            get { return _cs_flag; }
            set { _cs_flag = value; }
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
		/// <param name="flag">占位.</param>
        public void Request (
            int flag,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_flag != flag) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_flag = flag;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_WorldRecommendProjectList msg = new Msg_CS_DAT_WorldRecommendProjectList();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldRecommendProjectList>(
                    SoyHttpApiPath.WorldRecommendProjectList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_WorldRecommendProjectList msg)
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

        public void OnSyncFromParent (Msg_SC_DAT_WorldRecommendProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldRecommendProjectList (Msg_SC_DAT_WorldRecommendProjectList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldRecommendProjectList () { 
            _projectList = new List<Project>();
            OnCreate();
        }
        #endregion
    }
}