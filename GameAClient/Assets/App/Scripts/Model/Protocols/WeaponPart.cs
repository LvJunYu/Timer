//  | 武器碎片
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WeaponPart : SyncronisticData {
        #region 字段
        /// <summary>
        /// 武器Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 数量
        /// </summary>
        private int _totalCount;
        #endregion

        #region 属性
        /// <summary>
        /// 武器Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int TotalCount { 
            get { return _totalCount; }
            set { if (_totalCount != value) {
                _totalCount = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_WeaponPart msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _totalCount = msg.TotalCount;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_WeaponPart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WeaponPart (Msg_WeaponPart msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public WeaponPart () { 
        }
        #endregion
    }
}