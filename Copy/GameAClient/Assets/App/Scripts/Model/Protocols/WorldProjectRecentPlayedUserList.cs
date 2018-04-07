// 最近玩过的用户 | 最近玩过的用户
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectRecentPlayedUserList : SyncronisticData<Msg_SC_DAT_WorldProjectRecentPlayedUserList> {
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
        private List<ProjectRecentPlayedUserData> _dataList;

        // cs fields----------------------------------
        /// <summary>
        /// 
        /// </summary>
        private long _cs_projectId;
        /// <summary>
        /// 
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
        public List<ProjectRecentPlayedUserData> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 
        /// </summary>
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
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
		/// 最近玩过的用户
		/// </summary>
		/// <param name="projectId">.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            long projectId,
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_projectId != projectId) {
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
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_projectId = projectId;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_WorldProjectRecentPlayedUserList msg = new Msg_CS_DAT_WorldProjectRecentPlayedUserList();
                msg.ProjectId = projectId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldProjectRecentPlayedUserList>(
                    SoyHttpApiPath.WorldProjectRecentPlayedUserList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_WorldProjectRecentPlayedUserList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _dataList = new List<ProjectRecentPlayedUserData>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new ProjectRecentPlayedUserData(msg.DataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_WorldProjectRecentPlayedUserList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            if (null ==  _dataList) {
                _dataList = new List<ProjectRecentPlayedUserData>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new ProjectRecentPlayedUserData(msg.DataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (WorldProjectRecentPlayedUserList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<ProjectRecentPlayedUserData>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_WorldProjectRecentPlayedUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectRecentPlayedUserList (Msg_SC_DAT_WorldProjectRecentPlayedUserList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectRecentPlayedUserList () { 
            _dataList = new List<ProjectRecentPlayedUserData>();
            OnCreate();
        }
        #endregion
    }
}