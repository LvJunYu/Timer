// 获取快捷聊天 | 获取快捷聊天
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RoomChatPreinstall : SyncronisticData<Msg_SC_DAT_RoomChatPreinstall> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 快捷聊天Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private string _data;

        // cs fields----------------------------------
        /// <summary>
        /// 快捷聊天id
        /// </summary>
        private long _cs_id;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 快捷聊天Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
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
        public string Data { 
            get { return _data; }
            set { if (_data != value) {
                _data = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 快捷聊天id
        /// </summary>
        public long CS_Id { 
            get { return _cs_id; }
            set { _cs_id = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取快捷聊天
		/// </summary>
		/// <param name="id">快捷聊天id.</param>
        public void Request (
            long id,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_id != id) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_id = id;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_RoomChatPreinstall msg = new Msg_CS_DAT_RoomChatPreinstall();
                msg.Id = id;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_RoomChatPreinstall>(
                    SoyHttpApiPath.RoomChatPreinstall, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_RoomChatPreinstall msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _createTime = msg.CreateTime;           
            _updateTime = msg.UpdateTime;           
            _data = msg.Data;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_RoomChatPreinstall msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _createTime = msg.CreateTime;           
            _updateTime = msg.UpdateTime;           
            _data = msg.Data;           
            return true;
        } 

        public bool DeepCopy (RoomChatPreinstall obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _createTime = obj.CreateTime;           
            _updateTime = obj.UpdateTime;           
            _data = obj.Data;           
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_RoomChatPreinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RoomChatPreinstall (Msg_SC_DAT_RoomChatPreinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RoomChatPreinstall () { 
            OnCreate();
        }
        #endregion
    }
}