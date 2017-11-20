//  | 结构体例子
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Struct : SyncronisticData<Msg_Struct> {
        #region 字段
        /// <summary>
        /// Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 名字
        /// </summary>
        private string _name;
        #endregion

        #region 属性
        /// <summary>
        /// Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 名字
        /// </summary>
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
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_Struct msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _name = msg.Name;           
            return true;
        } 

        public bool DeepCopy (Struct obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _name = obj.Name;           
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