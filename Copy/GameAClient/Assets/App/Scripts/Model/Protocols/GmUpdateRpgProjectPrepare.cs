//  | 编辑编辑区剧情关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class GmUpdateRpgProjectPrepare : SyncronisticData<Msg_GmUpdateRpgProjectPrepare> {
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
        public bool OnSync (Msg_GmUpdateRpgProjectPrepare msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;     
            _projectId = msg.ProjectId;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_GmUpdateRpgProjectPrepare msg)
        {
            if (null == msg) return false;
            _slotInx = msg.SlotInx;           
            _projectId = msg.ProjectId;           
            return true;
        } 

        public bool DeepCopy (GmUpdateRpgProjectPrepare obj)
        {
            if (null == obj) return false;
            _slotInx = obj.SlotInx;           
            _projectId = obj.ProjectId;           
            return true;
        }

        public void OnSyncFromParent (Msg_GmUpdateRpgProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateRpgProjectPrepare (Msg_GmUpdateRpgProjectPrepare msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public GmUpdateRpgProjectPrepare () { 
        }
        #endregion
    }
}