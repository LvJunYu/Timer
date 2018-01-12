//  | 预设数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Preinstall : SyncronisticData<Msg_Preinstall> {
        #region 字段
        /// <summary>
        /// 预设Id
        /// </summary>
        private int _unitId;
        /// <summary>
        /// 
        /// </summary>
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        private int _rotation;
        /// <summary>
        /// 
        /// </summary>
        private byte[] _data;
        #endregion

        #region 属性
        /// <summary>
        /// 预设Id
        /// </summary>
        public int UnitId { 
            get { return _unitId; }
            set { if (_unitId != value) {
                _unitId = value;
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
        /// <summary>
        /// 
        /// </summary>
        public int Rotation { 
            get { return _rotation; }
            set { if (_rotation != value) {
                _rotation = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { 
            get { return _data; }
            set { if (_data != value) {
                _data = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_Preinstall msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;     
            _name = msg.Name;     
            _rotation = msg.Rotation;     
            _data = msg.Data;     
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_Preinstall msg)
        {
            if (null == msg) return false;
            _unitId = msg.UnitId;           
            _name = msg.Name;           
            _rotation = msg.Rotation;           
            _data = msg.Data;           
            return true;
        } 

        public bool DeepCopy (Preinstall obj)
        {
            if (null == obj) return false;
            _unitId = obj.UnitId;           
            _name = obj.Name;           
            _rotation = obj.Rotation;           
            _data = obj.Data;           
            return true;
        }

        public void OnSyncFromParent (Msg_Preinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Preinstall (Msg_Preinstall msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Preinstall () { 
        }
        #endregion
    }
}