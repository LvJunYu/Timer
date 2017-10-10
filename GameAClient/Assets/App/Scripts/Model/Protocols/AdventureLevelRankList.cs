// 请求单人模式关卡排行 | 请求单人模式关卡排行
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureLevelRankList : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 结果
        /// </summary>
        private int _resultCode;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private List<Record> _recordList;

        // cs fields----------------------------------
        /// <summary>
        /// 章节id
        /// </summary>
        private int _cs_section;
        /// <summary>
        /// 关卡类型
        /// </summary>
        private EAdventureProjectType _cs_projectType;
        /// <summary>
        /// 关卡id
        /// </summary>
        private int _cs_level;
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
        /// 结果
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
        public List<Record> RecordList { 
            get { return _recordList; }
            set { if (_recordList != value) {
                _recordList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 章节id
        /// </summary>
        public int CS_Section { 
            get { return _cs_section; }
            set { _cs_section = value; }
        }
        /// <summary>
        /// 关卡类型
        /// </summary>
        public EAdventureProjectType CS_ProjectType { 
            get { return _cs_projectType; }
            set { _cs_projectType = value; }
        }
        /// <summary>
        /// 关卡id
        /// </summary>
        public int CS_Level { 
            get { return _cs_level; }
            set { _cs_level = value; }
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
		/// 请求单人模式关卡排行
		/// </summary>
		/// <param name="section">章节id.</param>
		/// <param name="projectType">关卡类型.</param>
		/// <param name="level">关卡id.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            int section,
            EAdventureProjectType projectType,
            int level,
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_section != section) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_projectType != projectType) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                if (_cs_level != level) {
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
                _cs_section = section;
                _cs_projectType = projectType;
                _cs_level = level;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_AdventureLevelRankList msg = new Msg_CS_DAT_AdventureLevelRankList();
                msg.Section = section;
                msg.ProjectType = projectType;
                msg.Level = level;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureLevelRankList>(
                    SoyHttpApiPath.AdventureLevelRankList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_AdventureLevelRankList msg)
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

        public bool DeepCopy (AdventureLevelRankList obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.RecordList) return false;
            if (null ==  _recordList) {
                _recordList = new List<Record>();
            }
            _recordList.Clear();
            for (int i = 0; i < obj.RecordList.Count; i++){
                _recordList.Add(obj.RecordList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AdventureLevelRankList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureLevelRankList (Msg_SC_DAT_AdventureLevelRankList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureLevelRankList () { 
            _recordList = new List<Record>();
            OnCreate();
        }
        #endregion
    }
}