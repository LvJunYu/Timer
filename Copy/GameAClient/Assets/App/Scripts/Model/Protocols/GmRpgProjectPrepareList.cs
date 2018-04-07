// 编辑区剧情关卡列表 | 编辑区剧情关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmRpgProjectPrepareList : SyncronisticData<Msg_SC_DAT_GmRpgProjectPrepareList> {
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
        private List<GmPrepareProject> _projectList;

        // cs fields----------------------------------
        /// <summary>
        /// 起始下标
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
        public List<GmPrepareProject> ProjectList { 
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
		/// 编辑区剧情关卡列表
		/// </summary>
		/// <param name="startInx">起始下标.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            int startInx,
            int maxCount,
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
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_GmRpgProjectPrepareList msg = new Msg_CS_DAT_GmRpgProjectPrepareList();
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_GmRpgProjectPrepareList>(
                    SoyHttpApiPath.GmRpgProjectPrepareList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_GmRpgProjectPrepareList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _projectList = new List<GmPrepareProject>();
            for (int i = 0; i < msg.ProjectList.Count; i++) {
                _projectList.Add(new GmPrepareProject(msg.ProjectList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_GmRpgProjectPrepareList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _projectList) {
                _projectList = new List<GmPrepareProject>();
            }
            _projectList.Clear();
            for (int i = 0; i < msg.ProjectList.Count; i++) {
                _projectList.Add(new GmPrepareProject(msg.ProjectList[i]));
            }
            return true;
        } 

        public bool DeepCopy (GmRpgProjectPrepareList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.ProjectList) return false;
            if (null ==  _projectList) {
                _projectList = new List<GmPrepareProject>();
            }
            _projectList.Clear();
            for (int i = 0; i < obj.ProjectList.Count; i++){
                _projectList.Add(obj.ProjectList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_GmRpgProjectPrepareList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmRpgProjectPrepareList (Msg_SC_DAT_GmRpgProjectPrepareList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmRpgProjectPrepareList () { 
            _projectList = new List<GmPrepareProject>();
            OnCreate();
        }
        #endregion
    }
}