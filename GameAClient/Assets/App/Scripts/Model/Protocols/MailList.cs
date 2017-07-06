// 查询邮件 | 查询邮件
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MailList : SyncronisticData {
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
        private List<Mail> _dataList;
        /// <summary>
        /// 
        /// </summary>
        private int _totalCount;
        /// <summary>
        /// 
        /// </summary>
        private int _unreadCount;

        // cs fields----------------------------------
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
        public List<Mail> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int UnreadCount { 
            get { return _unreadCount; }
            set { if (_unreadCount != value) {
                _unreadCount = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
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
		/// 查询邮件
		/// </summary>
		/// <param name="startInx">.</param>
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

                Msg_CS_DAT_MailList msg = new Msg_CS_DAT_MailList();
                msg.StartInx = startInx;
                msg.MaxCount = maxCount;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_MailList>(
                    SoyHttpApiPath.MailList, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_MailList msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            _updateTime = msg.UpdateTime;           
            _dataList = new List<Mail>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new Mail(msg.DataList[i]));
            }
            _totalCount = msg.TotalCount;           
            _unreadCount = msg.UnreadCount;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_MailList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MailList (Msg_SC_DAT_MailList msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MailList () { 
            _dataList = new List<Mail>();
            OnCreate();
        }
        #endregion
    }
}