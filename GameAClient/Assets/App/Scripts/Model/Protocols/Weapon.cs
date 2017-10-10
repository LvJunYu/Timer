//  | 武器数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Weapon : SyncronisticData {
        #region 字段
        /// <summary>
        /// 武器Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 等级
        /// </summary>
        private int _level;
        #endregion

        #region 属性
        /// <summary>
        /// 武器Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
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
        public bool OnSync (Msg_Weapon msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _level = msg.Level;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (Weapon obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_Weapon msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Weapon (Msg_Weapon msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Weapon () { 
        }
        #endregion
    }
}