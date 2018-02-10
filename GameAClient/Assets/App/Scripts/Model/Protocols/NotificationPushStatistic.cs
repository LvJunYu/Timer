// 获取通知统计 | 获取通知统计
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
        /// 时间
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 通知界面未读统计数据
        /// </summary>
        private List<NotificationPushStatisticItem> _notificationStatisticList;
        /// <summary>
        /// 推送未读统计数据
        /// </summary>
        private List<NotificationPushStatisticItem> _pushStatisticList;

        // cs fields----------------------------------
        /// <summary>
        /// 占位 无意义
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 时间
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 通知界面未读统计数据
        /// </summary>
        public List<NotificationPushStatisticItem> NotificationStatisticList { 
            get { return _notificationStatisticList; }
            set { if (_notificationStatisticList != value) {
                _notificationStatisticList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 推送未读统计数据
        /// </summary>
        public List<NotificationPushStatisticItem> PushStatisticList { 
            get { return _pushStatisticList; }
            set { if (_pushStatisticList != value) {
                _pushStatisticList = value;
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
                if (null != _notificationStatisticList) {
                    for (int i = 0; i < _notificationStatisticList.Count; i++) {
                        if (null != _notificationStatisticList[i] && _notificationStatisticList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                if (null != _pushStatisticList) {
                    for (int i = 0; i < _pushStatisticList.Count; i++) {
                        if (null != _pushStatisticList[i] && _pushStatisticList[i].IsDirty) {
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
		/// 获取通知统计
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
            _updateTime = msg.UpdateTime;           
            _notificationStatisticList = new List<NotificationPushStatisticItem>();
            for (int i = 0; i < msg.NotificationStatisticList.Count; i++) {
                _notificationStatisticList.Add(new NotificationPushStatisticItem(msg.NotificationStatisticList[i]));
            }
            _pushStatisticList = new List<NotificationPushStatisticItem>();
            for (int i = 0; i < msg.PushStatisticList.Count; i++) {
                _pushStatisticList.Add(new NotificationPushStatisticItem(msg.PushStatisticList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_NotificationPushStatistic msg)
        {
            if (null == msg) return false;
            _updateTime = msg.UpdateTime;           
            if (null ==  _notificationStatisticList) {
                _notificationStatisticList = new List<NotificationPushStatisticItem>();
            }
            _notificationStatisticList.Clear();
            for (int i = 0; i < msg.NotificationStatisticList.Count; i++) {
                _notificationStatisticList.Add(new NotificationPushStatisticItem(msg.NotificationStatisticList[i]));
            }
            if (null ==  _pushStatisticList) {
                _pushStatisticList = new List<NotificationPushStatisticItem>();
            }
            _pushStatisticList.Clear();
            for (int i = 0; i < msg.PushStatisticList.Count; i++) {
                _pushStatisticList.Add(new NotificationPushStatisticItem(msg.PushStatisticList[i]));
            }
            return true;
        } 

        public bool DeepCopy (NotificationPushStatistic obj)
        {
            if (null == obj) return false;
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.NotificationStatisticList) return false;
            if (null ==  _notificationStatisticList) {
                _notificationStatisticList = new List<NotificationPushStatisticItem>();
            }
            _notificationStatisticList.Clear();
            for (int i = 0; i < obj.NotificationStatisticList.Count; i++){
                _notificationStatisticList.Add(obj.NotificationStatisticList[i]);
            }
            if (null ==  obj.PushStatisticList) return false;
            if (null ==  _pushStatisticList) {
                _pushStatisticList = new List<NotificationPushStatisticItem>();
            }
            _pushStatisticList.Clear();
            for (int i = 0; i < obj.PushStatisticList.Count; i++){
                _pushStatisticList.Add(obj.PushStatisticList[i]);
            }
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
            _notificationStatisticList = new List<NotificationPushStatisticItem>();
            _pushStatisticList = new List<NotificationPushStatisticItem>();
            OnCreate();
        }
        #endregion
    }
}