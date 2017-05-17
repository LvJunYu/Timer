//  | 结构体例子
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Struct : SyncronisticData {
        #region 字段
        // Id
        private long _id;
        // 名字
        private string _name;
        #endregion

        #region 属性
        // Id
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        // 名字
        public string Name { 
            get { return _name; }
            set { if (_name != value) {
                _name = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_Struct msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _name = msg.Name;     
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_Struct msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Struct (Msg_Struct msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Struct () { 
        }
        #endregion
    }
}