// 最近录像 | 高分排行榜
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectRecordRankList : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        // ECachedDataState
        private int _resultCode;
        // 
        private long _updateTime;
        // 
        private List<Record> _recordList;

        // cs fields----------------------------------
        // 
        private long _cs_projectId;
        // 
        private int _cs_startInx;
        // 
        private int _cs_maxCount;
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
        public List<Record> RecordList { 
            get { return _recordList; }
            set { if (_recordList != value) {
                _recordList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 
        public long CS_ProjectId { 
            get { return _cs_projectId; }
            set { _cs_projectId = value; }
        }
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

        public override bool IsDirty {
            get {
                if (null != _recordList) {
                    for (int i = 0; i < _recordList.Count; i++) {
                        if (null != _recordList[i] && _recordList[i].IsDirty) {
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
		/// 高分排行榜
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
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_WorldProjectRecordRankList msg = new Msg_CS_DAT_WorldProjectRecordRankList();
            msg.ProjectId = projectId;
            msg.StartInx = startInx;
            msg.MaxCount = maxCount;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_WorldProjectRecordRankList>(
                SoyHttpApiPath.WorldProjectRecordRankList, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_WorldProjectRecordRankList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _recordList = new List<Record>();
            for (int i = 0; i < msg.RecordList.Count; i++) {
                _recordList.Add(new Record(msg.RecordList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_WorldProjectRecordRankList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectRecordRankList (Msg_SC_DAT_WorldProjectRecordRankList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WorldProjectRecordRankList () { 
            _recordList = new List<Record>();
        }
        #endregion
    }
}