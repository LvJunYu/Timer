// 查询未读邮件数量 | 查询未读邮件数量
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MailStatistic : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
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
        private int _cs_flag;
        #endregion

        #region 属性
        // sc properties----------------------------------
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
        public int CS_Flag { 
            get { return _cs_flag; }
            set { _cs_flag = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 查询未读邮件数量
		/// </summary>
		/// <param name="flag">.</param>
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

                Msg_CS_DAT_MailStatistic msg = new Msg_CS_DAT_MailStatistic();
                msg.Flag = flag;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_MailStatistic>(
                    SoyHttpApiPath.MailStatistic, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_MailStatistic msg)
        {
            if (null == msg) return false;
            _totalCount = msg.TotalCount;           
            _unreadCount = msg.UnreadCount;           
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (MailStatistic obj)
        {
            if (null == obj) return false;
            _totalCount = obj.TotalCount;           
            _unreadCount = obj.UnreadCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_MailStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MailStatistic (Msg_SC_DAT_MailStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MailStatistic () { 
            OnCreate();
        }
        #endregion
    }
}