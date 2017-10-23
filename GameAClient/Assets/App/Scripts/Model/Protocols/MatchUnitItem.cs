//  | 用户改造可用地块数据条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MatchUnitItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 地块Id
        /// </summary>
        private long _unitId;
        /// <summary>
        /// 地块数量
        /// </summary>
        private int _unitCount;
        #endregion

        #region 属性
        /// <summary>
        /// 地块Id
        /// </summary>
        public long UnitId { 
            get { return _unitId; }
            set { if (_unitId != value) {
                _unitId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 地块数量
        /// </summary>
        public int UnitCount { 
            get { return _unitCount; }
            set { if (_unitCount != value) {
                _unitCount = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_MatchUnitItem msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;     
            _unitCount = msg.UnitCount;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (MatchUnitItem obj)
        {
            if (null == obj) return false;
            _unitId = obj.UnitId;           
            _unitCount = obj.UnitCount;           
            return true;
        }

        public void OnSyncFromParent (Msg_MatchUnitItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchUnitItem (Msg_MatchUnitItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MatchUnitItem () { 
        }
        #endregion
    }
}