//  | 排序自荐关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class SortSelfRecommendProjectItem : SyncronisticData<Msg_SortSelfRecommendProjectItem> {
        #region 字段
        /// <summary>
        /// 槽位
        /// </summary>
        private int _slotInx;
        /// <summary>
        /// 
        /// </summary>
        private long _oldProjectMainId;
        /// <summary>
        /// 
        /// </summary>
        private long _newProjectMainId;
        #endregion

        #region 属性
        /// <summary>
        /// 槽位
        /// </summary>
        public int SlotInx { 
            get { return _slotInx; }
            set { if (_slotInx != value) {
                _slotInx = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long OldProjectMainId { 
            get { return _oldProjectMainId; }
            set { if (_oldProjectMainId != value) {
                _oldProjectMainId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long NewProjectMainId { 
            get { return _newProjectMainId; }
            set { if (_newProjectMainId != value) {
                _newProjectMainId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_SortSelfRecommendProjectItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            _oldProjectMainId = msg.OldProjectMainId;     
            _newProjectMainId = msg.NewProjectMainId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_SortSelfRecommendProjectItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            _oldProjectMainId = msg.OldProjectMainId;           
            _newProjectMainId = msg.NewProjectMainId;           
            return true;
        } 

        public bool DeepCopy (SortSelfRecommendProjectItem obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            _oldProjectMainId = obj.OldProjectMainId;           
            _newProjectMainId = obj.NewProjectMainId;           
            return true;
        }

        public void OnSyncFromParent (Msg_SortSelfRecommendProjectItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SortSelfRecommendProjectItem (Msg_SortSelfRecommendProjectItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SortSelfRecommendProjectItem () { 
        }
        #endregion
    }
}