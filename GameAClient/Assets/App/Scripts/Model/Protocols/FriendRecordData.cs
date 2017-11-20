// 好友通关录像 | 好友通关录像
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class FriendRecordData : SyncronisticData<Msg_SC_DAT_FriendRecordData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 录像数据
        /// </summary>
        private List<Record> _recordList;

        // cs fields----------------------------------
        /// <summary>
        /// 章节
        /// </summary>
        private int _cs_section;
        /// <summary>
        /// 关卡
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
        /// 录像数据
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
        /// 章节
        /// </summary>
        public int CS_Section { 
            get { return _cs_section; }
            set { _cs_section = value; }
        }
        /// <summary>
        /// 关卡
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
		/// 好友通关录像
		/// </summary>
		/// <param name="section">章节.</param>
		/// <param name="level">关卡.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            int section,
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
                _cs_level = level;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_FriendRecordData msg = new Msg_CS_DAT_FriendRecordData();
                msg.Section = section;
                msg.Level = level;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_FriendRecordData>(
                    SoyHttpApiPath.FriendRecordData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_FriendRecordData msg)
        {
            if (null == msg) return false;
            _recordList = new List<Record>();
            for (int i = 0; i < msg.RecordList.Count; i++) {
                _recordList.Add(new Record(msg.RecordList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_FriendRecordData msg)
        {
            if (null == msg) return false;
            if (null ==  _recordList) {
                _recordList = new List<Record>();
            }
            _recordList.Clear();
            for (int i = 0; i < msg.RecordList.Count; i++) {
                _recordList.Add(new Record(msg.RecordList[i]));
            }
            return true;
        } 

        public bool DeepCopy (FriendRecordData obj)
        {
            if (null == obj) return false;
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

        public void OnSyncFromParent (Msg_SC_DAT_FriendRecordData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendRecordData (Msg_SC_DAT_FriendRecordData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendRecordData () { 
            _recordList = new List<Record>();
            OnCreate();
        }
        #endregion
    }
}