//  | 角色时装数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AvatarPartItem : SyncronisticData {
        #region 字段
        // 下边枚举
        private int _type;
        // 
        private long _id;
        // 是否在使用
        private bool _using;
        // 过期时间
        private long _expirationTime;
        #endregion

        #region 属性
        // 下边枚举
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        // 
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        // 是否在使用
        public bool Using { 
            get { return _using; }
            set { if (_using != value) {
                _using = value;
                SetDirty();
            }}
        }
        // 过期时间
        public long ExpirationTime { 
            get { return _expirationTime; }
            set { if (_expirationTime != value) {
                _expirationTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AvatarPartItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            _using = msg.Using;     
            _expirationTime = msg.ExpirationTime;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_AvatarPartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartItem (Msg_AvatarPartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartItem () { 
        }
        #endregion
    }
}