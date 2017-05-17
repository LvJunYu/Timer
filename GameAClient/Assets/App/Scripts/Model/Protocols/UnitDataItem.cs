//  | 用户改造可用地块数据条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UnitDataItem : SyncronisticData {
        #region 字段
        // 地块Id
        private long _unitId;
        // 地块数量
        private int _unitCount;
        #endregion

        #region 属性
        // 地块Id
        public long UnitId { 
            get { return _unitId; }
            set { if (_unitId != value) {
                _unitId = value;
                SetDirty();
            }}
        }
        // 地块数量
        public int UnitCount { 
            get { return _unitCount; }
            set { if (_unitCount != value) {
                _unitCount = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_UnitDataItem msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;     
            _unitCount = msg.UnitCount;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_UnitDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitDataItem (Msg_UnitDataItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UnitDataItem () { 
        }
        #endregion
    }
}