//  | 时装打折券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AvatarPartDiscountCouponItem : SyncronisticData<Msg_AvatarPartDiscountCouponItem> {
        #region 字段
        /// <summary>
        /// 数据Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 配表Id
        /// </summary>
        private long _configTableId;
        /// <summary>
        /// 部件
        /// </summary>
        private int _partType;
        /// <summary>
        /// 
        /// </summary>
        private long _partId;
        /// <summary>
        /// 
        /// </summary>
        private int _durationType;
        /// <summary>
        /// 过期时间
        /// </summary>
        private long _expirationTime;
        /// <summary>
        /// 折扣
        /// </summary>
        private int _discount;
        /// <summary>
        /// 是否已被使用
        /// </summary>
        private bool _used;
        /// <summary>
        /// 使用时间
        /// </summary>
        private long _useTime;
        /// <summary>
        /// 是否有效
        /// </summary>
        private bool _isValid;
        #endregion

        #region 属性
        /// <summary>
        /// 数据Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 配表Id
        /// </summary>
        public long ConfigTableId { 
            get { return _configTableId; }
            set { if (_configTableId != value) {
                _configTableId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 部件
        /// </summary>
        public int PartType { 
            get { return _partType; }
            set { if (_partType != value) {
                _partType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long PartId { 
            get { return _partId; }
            set { if (_partId != value) {
                _partId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int DurationType { 
            get { return _durationType; }
            set { if (_durationType != value) {
                _durationType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpirationTime { 
            get { return _expirationTime; }
            set { if (_expirationTime != value) {
                _expirationTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 折扣
        /// </summary>
        public int Discount { 
            get { return _discount; }
            set { if (_discount != value) {
                _discount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否已被使用
        /// </summary>
        public bool Used { 
            get { return _used; }
            set { if (_used != value) {
                _used = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 使用时间
        /// </summary>
        public long UseTime { 
            get { return _useTime; }
            set { if (_useTime != value) {
                _useTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 是否有效
        /// </summary>
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
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_AvatarPartDiscountCouponItem msg)
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
            return true;
        } 

        public bool DeepCopy (AvatarPartDiscountCouponItem obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _configTableId = obj.ConfigTableId;           
            _partType = obj.PartType;           
            _partId = obj.PartId;           
            _durationType = obj.DurationType;           
            _expirationTime = obj.ExpirationTime;           
            _discount = obj.Discount;           
            _used = obj.Used;           
            _useTime = obj.UseTime;           
            _isValid = obj.IsValid;           
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