//  | 用户改造可用地块数据条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UnitDataItem : SyncronisticData<Msg_UnitDataItem> {
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
        public bool OnSync (Msg_UnitDataItem msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;     
            _unitCount = msg.UnitCount;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_UnitDataItem msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;           
            _unitCount = msg.UnitCount;           
            return true;
        } 

        public bool DeepCopy (UnitDataItem obj)
        {
            if (null == obj) return false;
            _unitId = obj.UnitId;           
            _unitCount = obj.UnitCount;           
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