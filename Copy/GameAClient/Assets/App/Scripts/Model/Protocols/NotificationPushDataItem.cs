//  | 获取推送数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class NotificationPushDataItem : SyncronisticData<Msg_NotificationPushDataItem> {
        #region 字段
        /// <summary>
        /// ENotificationDataType
        /// </summary>
        private ENotificationDataType _type;
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 时间发起者
        /// </summary>
        private UserInfoSimple _sender;
        /// <summary>
        /// 跳转内容的Id
        /// </summary>
        private long _contentId;
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
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 时间发起者
        /// </summary>
        public UserInfoSimple Sender { 
            get { return _sender; }
            set { if (_sender != value) {
                _sender = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 跳转内容的Id
        /// </summary>
        public long ContentId { 
            get { return _contentId; }
            set { if (_contentId != value) {
                _contentId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_NotificationPushDataItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            if (null == _sender) {
                _sender = new UserInfoSimple(msg.Sender);
            } else {
                _sender.OnSyncFromParent(msg.Sender);
            }
            _contentId = msg.ContentId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_NotificationPushDataItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;           
            _id = msg.Id;           
            if(null != msg.Sender){
                if (null == _sender){
                    _sender = new UserInfoSimple(msg.Sender);
                }
                _sender.CopyMsgData(msg.Sender);
            }
            _contentId = msg.ContentId;           
            return true;
        } 

        public bool DeepCopy (NotificationPushDataItem obj)
        {
            if (null == obj) return false;
            _type = obj.Type;           
            _id = obj.Id;           
            if(null != obj.Sender){
                if (null == _sender){
                    _sender = new UserInfoSimple();
                }
                _sender.DeepCopy(obj.Sender);
            }
            _contentId = obj.ContentId;           
            return true;
        }

        public void OnSyncFromParent (Msg_NotificationPushDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushDataItem (Msg_NotificationPushDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public NotificationPushDataItem () { 
            _sender = new UserInfoSimple();
        }
        #endregion
    }
}