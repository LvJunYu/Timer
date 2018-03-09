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
        private long _oldProjectId;
        /// <summary>
        /// 
        /// </summary>
        private long _newProjectId;
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
        public long OldProjectId { 
            get { return _oldProjectId; }
            set { if (_oldProjectId != value) {
                _oldProjectId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long NewProjectId { 
            get { return _newProjectId; }
            set { if (_newProjectId != value) {
                _newProjectId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_SortSelfRecommendProjectItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            _oldProjectId = msg.OldProjectId;     
            _newProjectId = msg.NewProjectId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_SortSelfRecommendProjectItem msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            _oldProjectId = msg.OldProjectId;           
            _newProjectId = msg.NewProjectId;           
            return true;
        } 

        public bool DeepCopy (SortSelfRecommendProjectItem obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            _oldProjectId = obj.OldProjectId;           
            _newProjectId = obj.NewProjectId;           
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