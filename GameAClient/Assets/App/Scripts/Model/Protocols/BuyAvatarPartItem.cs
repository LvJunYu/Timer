//  | 购买时装一条数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class BuyAvatarPartItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 类型
        /// </summary>
        private EAvatarPart _partType;
        /// <summary>
        /// 部件id
        /// </summary>
        private long _partId;
        /// <summary>
        /// 购买时长
        /// </summary>
        private EBuyAvatarPartDurationType _durationType;
        /// <summary>
        /// 
        /// </summary>
        private ECurrencyType _currencyType;
        /// <summary>
        /// Msg_AvatarPartDiscountCouponItem 的id
        /// </summary>
        private long _discountCouponId;
        #endregion

        #region 属性
        /// <summary>
        /// 类型
        /// </summary>
        public EAvatarPart PartType { 
            get { return _partType; }
            set { if (_partType != value) {
                _partType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 部件id
        /// </summary>
        public long PartId { 
            get { return _partId; }
            set { if (_partId != value) {
                _partId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 购买时长
        /// </summary>
        public EBuyAvatarPartDurationType DurationType { 
            get { return _durationType; }
            set { if (_durationType != value) {
                _durationType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public ECurrencyType CurrencyType { 
            get { return _currencyType; }
            set { if (_currencyType != value) {
                _currencyType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// Msg_AvatarPartDiscountCouponItem 的id
        /// </summary>
        public long DiscountCouponId { 
            get { return _discountCouponId; }
            set { if (_discountCouponId != value) {
                _discountCouponId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_BuyAvatarPartItem msg)
        {
            if (null == msg) return false;
            _partType = msg.PartType;     
            _partId = msg.PartId;     
            _durationType = msg.DurationType;     
            _currencyType = msg.CurrencyType;     
            _discountCouponId = msg.DiscountCouponId;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (BuyAvatarPartItem obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_BuyAvatarPartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public BuyAvatarPartItem (Msg_BuyAvatarPartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public BuyAvatarPartItem () { 
        }
        #endregion
    }
}