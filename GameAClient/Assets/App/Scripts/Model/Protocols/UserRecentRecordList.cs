// 最近录像 | 最近录像
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserRecentRecordList : SyncronisticData {
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
        private List<Record> _recordList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _cs_userId;
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
        public List<Record> RecordList { 
            get { return _recordList; }
            set { if (_recordList != value) {
                _recordList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
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
		/// 最近录像
		/// </summary>
		/// <param name="userId">用户Id.</param>
		/// <param name="startInx">.</param>
		/// <param name="maxCount">.</param>
        public void Request (
            long userId,
            int startInx,
            int maxCount,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
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
                _cs_userId = userId;
                _cs_startInx = startInx;
                _cs_maxCount = maxCount;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_UserRecentRecordList msg = new Msg_CS_DAT_UserRecentRecordList();
                msg.UserId = userId;
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserRecentRecordList>(
                    SoyHttpApiPath.UserRecentRecordList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserRecentRecordList msg)
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

        public bool DeepCopy (UserRecentRecordList obj)
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

        public void OnSyncFromParent (Msg_SC_DAT_UserRecentRecordList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRecentRecordList (Msg_SC_DAT_UserRecentRecordList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRecentRecordList () { 
            _recordList = new List<Record>();
            OnCreate();
        }
        #endregion
    }
}