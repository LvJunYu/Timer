//  | 获取通知统计
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NotificationPushStatisticItem : SyncronisticData<Msg_NotificationPushStatisticItem> {
        #region 字段
        /// <summary>
        /// ENotificationDataType
        /// </summary>
        private ENotificationDataType _type;
        /// <summary>
        /// 
        /// </summary>
        private int _count;
        #endregion

        #region 属性
        /// <summary>
        /// ENotificationDataType
        /// </summary>
        public ENotificationDataType Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_NotificationPushStatisticItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _count = msg.Count;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_NotificationPushStatisticItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;           
            _count = msg.Count;           
            return true;
        } 

        public bool DeepCopy (NotificationPushStatisticItem obj)
        {
            if (null == obj) return false;
            _type = obj.Type;           
            _count = obj.Count;           
            return true;
        }

        public void OnSyncFromParent (Msg_NotificationPushStatisticItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushStatisticItem (Msg_NotificationPushStatisticItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushStatisticItem () { 
        }
        #endregion
    }
}