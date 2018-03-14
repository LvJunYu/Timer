//  | 添加自荐关卡
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class SelfRecommendProjectOperateItem : SyncronisticData<Msg_SelfRecommendProjectOperateItem> {
        #region 字段
        /// <summary>
        /// 关卡
        /// </summary>
        private long _projectMainId;
        /// <summary>
        /// 
        /// </summary>
        private int _slotInx;
        #endregion

        #region 属性
        /// <summary>
        /// 关卡
        /// </summary>
        public long ProjectMainId { 
            get { return _projectMainId; }
            set { if (_projectMainId != value) {
                _projectMainId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int SlotInx { 
            get { return _slotInx; }
            set { if (_slotInx != value) {
                _slotInx = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_SelfRecommendProjectOperateItem msg)
        {
            if (null == msg) return false;
            _projectMainId = msg.ProjectMainId;     
            _slotInx = msg.SlotInx;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_SelfRecommendProjectOperateItem msg)
        {
            if (null == msg) return false;
            _projectMainId = msg.ProjectMainId;           
            _slotInx = msg.SlotInx;           
            return true;
        } 

        public bool DeepCopy (SelfRecommendProjectOperateItem obj)
        {
            if (null == obj) return false;
            _projectMainId = obj.ProjectMainId;           
            _slotInx = obj.SlotInx;           
            return true;
        }

        public void OnSyncFromParent (Msg_SelfRecommendProjectOperateItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SelfRecommendProjectOperateItem (Msg_SelfRecommendProjectOperateItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public SelfRecommendProjectOperateItem () { 
        }
        #endregion
    }
}