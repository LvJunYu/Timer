//  | 时装打折券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AvatarPartDiscountCouponGroup : SyncronisticData {
        #region 字段
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
        private List<AvatarPartDiscountCouponItem> _itemDataList;
        #endregion

        #region 属性
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
        public List<AvatarPartDiscountCouponItem> ItemDataList { 
            get { return _itemDataList; }
            set { if (_itemDataList != value) {
                _itemDataList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AvatarPartDiscountCouponGroup msg)
        {
            if (null == msg) return false;
            _partType = msg.PartType;     
            _partId = msg.PartId;     
            _itemDataList = new List<AvatarPartDiscountCouponItem>();
            for (int i = 0; i < msg.ItemDataList.Count; i++) {
                _itemDataList.Add(new AvatarPartDiscountCouponItem(msg.ItemDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AvatarPartDiscountCouponGroup obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_AvatarPartDiscountCouponGroup msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartDiscountCouponGroup (Msg_AvatarPartDiscountCouponGroup msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AvatarPartDiscountCouponGroup () { 
            _itemDataList = new List<AvatarPartDiscountCouponItem>();
        }
        #endregion
    }
}