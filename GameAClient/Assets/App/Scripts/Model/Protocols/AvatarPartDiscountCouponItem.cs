//  | 时装打折券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AvatarPartDiscountCouponItem : SyncronisticData {
        #region 字段
        // 数据Id
        private long _id;
        // 配表Id
        private long _configTableId;
        // 部件
        private int _partType;
        // 
        private long _partId;
        // 
        private int _durationType;
        // 过期时间
        private long _expirationTime;
        // 折扣
        private int _discount;
        // 是否已被使用
        private bool _used;
        // 使用时间
        private long _useTime;
        // 是否有效
        private bool _isValid;
        #endregion

        #region 属性
        // 数据Id
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        // 配表Id
        public long ConfigTableId { 
            get { return _configTableId; }
            set { if (_configTableId != value) {
                _configTableId = value;
                SetDirty();
            }}
        }
        // 部件
        public int PartType { 
            get { return _partType; }
            set { if (_partType != value) {
                _partType = value;
                SetDirty();
            }}
        }
        // 
        public long PartId { 
            get { return _partId; }
            set { if (_partId != value) {
                _partId = value;
                SetDirty();
            }}
        }
        // 
        public int DurationType { 
            get { return _durationType; }
            set { if (_durationType != value) {
                _durationType = value;
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
        // 折扣
        public int Discount { 
            get { return _discount; }
            set { if (_discount != value) {
                _discount = value;
                SetDirty();
            }}
        }
        // 是否已被使用
        public bool Used { 
            get { return _used; }
            set { if (_used != value) {
                _used = value;
                SetDirty();
            }}
        }
        // 使用时间
        public long UseTime { 
            get { return _useTime; }
            set { if (_useTime != value) {
                _useTime = value;
                SetDirty();
            }}
        }
        // 是否有效
        public bool IsValid { 
            get { return _isValid; }
            set { if (_isValid != value) {
                _isValid = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AvatarPartDiscountCouponItem msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _configTableId = msg.ConfigTableId;     
            _partType = msg.PartType;     
            _partId = msg.PartId;     
            _durationType = msg.DurationType;     
            _expirationTime = msg.ExpirationTime;     
            _discount = msg.Discount;     
            _used = msg.Used;     
            _useTime = msg.UseTime;     
            _isValid = msg.IsValid;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_AvatarPartDiscountCouponItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartDiscountCouponItem (Msg_AvatarPartDiscountCouponItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartDiscountCouponItem () { 
        }
        #endregion
    }
}