//  | 预设数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class PreinstallData : SyncronisticData<Msg_PreinstallData> {
        #region 字段
        /// <summary>
        /// 预设ID
        /// </summary>
        private int _id;
        /// <summary>
        /// 
        /// </summary>
        private long _createTime;
        /// <summary>
        /// 
        /// </summary>
        private long _updateTime;
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        #endregion

        #region 属性
        /// <summary>
        /// 预设ID
        /// </summary>
        public int Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long CreateTime { 
            get { return _createTime; }
            set { if (_createTime != value) {
                _createTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public long UpdateTime { 
            get { return _updateTime; }
            set { if (_updateTime != value) {
                _updateTime = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
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
        public bool OnSync (Msg_PreinstallData msg)
        {
            if (null == msg) return false;
            _id = msg.Id;     
            _createTime = msg.CreateTime;     
            _updateTime = msg.UpdateTime;     
            _name = msg.Name;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_PreinstallData msg)
        {
            if (null == msg) return false;
            _id = msg.Id;           
            _createTime = msg.CreateTime;           
            _updateTime = msg.UpdateTime;           
            _name = msg.Name;           
            return true;
        } 

        public bool DeepCopy (PreinstallData obj)
        {
            if (null == obj) return false;
            _id = obj.Id;           
            _createTime = obj.CreateTime;           
            _updateTime = obj.UpdateTime;           
            _name = obj.Name;           
            return true;
        }

        public void OnSyncFromParent (Msg_PreinstallData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PreinstallData (Msg_PreinstallData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public PreinstallData () { 
        }
        #endregion
    }
}