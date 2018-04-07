//  | 编辑编辑区多人关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmUpdateNetProjectPrepare : SyncronisticData<Msg_GmUpdateNetProjectPrepare> {
        #region 字段
        /// <summary>
        /// 下标
        /// </summary>
        private int _slotInx;
        /// <summary>
        /// 
        /// </summary>
        private long _projectId;
        #endregion

        #region 属性
        /// <summary>
        /// 下标
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
        public long ProjectId { 
            get { return _projectId; }
            set { if (_projectId != value) {
                _projectId = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_GmUpdateNetProjectPrepare msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            _projectId = msg.ProjectId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_GmUpdateNetProjectPrepare msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            _projectId = msg.ProjectId;           
            return true;
        } 

        public bool DeepCopy (GmUpdateNetProjectPrepare obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            _projectId = obj.ProjectId;           
            return true;
        }

        public void OnSyncFromParent (Msg_GmUpdateNetProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateNetProjectPrepare (Msg_GmUpdateNetProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateNetProjectPrepare () { 
        }
        #endregion
    }
}