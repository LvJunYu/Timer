// 获取推送数据 | 获取推送数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NotificationPushData : SyncronisticData<Msg_SC_DAT_NotificationPushData> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 时间
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 数据
        /// </summary>
        private List<NotificationPushDataItem> _dataList;

        // cs fields----------------------------------
        /// <summary>
        /// 类型Mask
        /// </summary>
        private long _cs_typeMask;
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
        /// 数据
        /// </summary>
        public List<NotificationPushDataItem> DataList { 
            get { return _dataList; }
            set { if (_dataList != value) {
                _dataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 类型Mask
        /// </summary>
        public long CS_TypeMask { 
            get { return _cs_typeMask; }
            set { _cs_typeMask = value; }
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
		/// 获取推送数据
		/// </summary>
		/// <param name="typeMask">类型Mask.</param>
        public void Request (
            long typeMask,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_typeMask != typeMask) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_typeMask = typeMask;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_NotificationPushData msg = new Msg_CS_DAT_NotificationPushData();
                msg.TypeMask = typeMask;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_NotificationPushData>(
                    SoyHttpApiPath.NotificationPushData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_NotificationPushData msg)
        {
            if (null == msg) return false;
            _updateTime = msg.UpdateTime;           
            _dataList = new List<NotificationPushDataItem>();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new NotificationPushDataItem(msg.DataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_NotificationPushData msg)
        {
            if (null == msg) return false;
            _updateTime = msg.UpdateTime;           
            if (null ==  _dataList) {
                _dataList = new List<NotificationPushDataItem>();
            }
            _dataList.Clear();
            for (int i = 0; i < msg.DataList.Count; i++) {
                _dataList.Add(new NotificationPushDataItem(msg.DataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (NotificationPushData obj)
        {
            if (null == obj) return false;
            _updateTime = obj.UpdateTime;           
            if (null ==  obj.DataList) return false;
            if (null ==  _dataList) {
                _dataList = new List<NotificationPushDataItem>();
            }
            _dataList.Clear();
            for (int i = 0; i < obj.DataList.Count; i++){
                _dataList.Add(obj.DataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_NotificationPushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushData (Msg_SC_DAT_NotificationPushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushData () { 
            _dataList = new List<NotificationPushDataItem>();
            OnCreate();
        }
        #endregion
    }
}