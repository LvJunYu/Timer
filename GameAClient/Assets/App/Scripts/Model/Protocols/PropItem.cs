//  | 道具数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PropItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 
        /// </summary>
        private int _type;
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 
        /// </summary>
        private int _count;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public int Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_PropItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            _count = msg.Count;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (PropItem obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_PropItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PropItem (Msg_PropItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PropItem () { 
        }
        #endregion
    }
}