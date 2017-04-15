//  | 家园装饰
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class HomePartItem : SyncronisticData {
        #region 字段
        // 下边的枚举
        private int _type;
        // 
        private long _id;
        // 状态
        private int _state;
        // 等级
        private int _level;
        #endregion

        #region 属性
        // 下边的枚举
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        // 
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        // 状态
        public int State { 
            get { return _state; }
            set { if (_state != value) {
                _state = value;
                SetDirty();
            }}
        }
        // 等级
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