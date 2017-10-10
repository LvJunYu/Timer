//  | 家园装饰
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class HomePartItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 下边的枚举
        /// </summary>
        private int _type;
        /// <summary>
        /// 
        /// </summary>
        private long _id;
        /// <summary>
        /// 状态
        /// </summary>
        private int _state;
        /// <summary>
        /// 等级
        /// </summary>
        private int _level;
        #endregion

        #region 属性
        /// <summary>
        /// 下边的枚举
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
        /// 状态
        /// </summary>
        public int State { 
            get { return _state; }
            set { if (_state != value) {
                _state = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { 
            get { return _level; }
            set { if (_level != value) {
                _level = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_HomePartItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _id = msg.Id;     
            _state = msg.State;     
            _level = msg.Level;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (HomePartItem obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_HomePartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public HomePartItem (Msg_HomePartItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public HomePartItem () { 
        }
        #endregion
    }
}