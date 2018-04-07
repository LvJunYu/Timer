//  | 排序官方推荐编辑区
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class SortOfficialRecommendProjectPrepareItem : SyncronisticData<Msg_SortOfficialRecommendProjectPrepareItem> {
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
        public bool OnSync (Msg_SortOfficialRecommendProjectPrepareItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            _oldProjectMainId = msg.OldProjectMainId;     
            _newProjectMainId = msg.NewProjectMainId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_SortOfficialRecommendProjectPrepareItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            _oldProjectMainId = msg.OldProjectMainId;           
            _newProjectMainId = msg.NewProjectMainId;           
            return true;
        } 

        public bool DeepCopy (SortOfficialRecommendProjectPrepareItem obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            _oldProjectMainId = obj.OldProjectMainId;           
            _newProjectMainId = obj.NewProjectMainId;           
            return true;
        }

        public void OnSyncFromParent (Msg_SortOfficialRecommendProjectPrepareItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SortOfficialRecommendProjectPrepareItem (Msg_SortOfficialRecommendProjectPrepareItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SortOfficialRecommendProjectPrepareItem () { 
        }
        #endregion
    }
}