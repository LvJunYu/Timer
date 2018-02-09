// 获取通知 | 获取通知
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NotificationPushStatistic : SyncronisticData<Msg_SC_DAT_NotificationPushStatistic> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// ERoomChatPreinstallOperateResult
        /// </summary>
        private int _resultCode;

        // cs fields----------------------------------
        /// <summary>
        /// 占位 无意义
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// ERoomChatPreinstallOperateResult
        /// </summary>
        public int ResultCode { 
            get { return _resultCode; }
            set { if (_resultCode != value) {
                _resultCode = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 占位 无意义
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取通知
		/// </summary>
		/// <param name="userId">占位 无意义.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_NotificationPushStatistic msg = new Msg_CS_DAT_NotificationPushStatistic();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_NotificationPushStatistic>(
                    SoyHttpApiPath.NotificationPushStatistic, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_NotificationPushStatistic msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_NotificationPushStatistic msg)
        {
            if (null == msg) return false;
            _resultCode = msg.ResultCode;           
            return true;
        } 

        public bool DeepCopy (NotificationPushStatistic obj)
        {
            if (null == obj) return false;
            _resultCode = obj.ResultCode;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_NotificationPushStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushStatistic (Msg_SC_DAT_NotificationPushStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushStatistic () { 
            OnCreate();
        }
        #endregion
    }
}