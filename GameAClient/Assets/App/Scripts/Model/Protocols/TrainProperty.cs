//  | 训练属性数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class TrainProperty : SyncronisticData {
        #region 字段
        /// <summary>
        /// 属性ID
        /// </summary>
        private ETrainPropertyType _property;
        /// <summary>
        /// 等级
        /// </summary>
        private int _level;
        /// <summary>
        /// 是否正在升级
        /// </summary>
        private bool _isTraining;
        /// <summary>
        /// 训练开始时间
        /// </summary>
        private long _trainStartTime;
        #endregion

        #region 属性
        /// <summary>
        /// 属性ID
        /// </summary>
        public ETrainPropertyType Property { 
            get { return _property; }
            set { if (_property != value) {
                _property = value;
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
        /// <summary>
        /// 是否正在升级
        /// </summary>
        public bool IsTraining { 
            get { return _isTraining; }
            set { if (_isTraining != value) {
                _isTraining = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 训练开始时间
        /// </summary>
        public long TrainStartTime { 
            get { return _trainStartTime; }
            set { if (_trainStartTime != value) {
                _trainStartTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_TrainProperty msg)
        {
            if (null == msg) return false;
            _property = msg.Property;     
            _level = msg.Level;     
            _isTraining = msg.IsTraining;     
            _trainStartTime = msg.TrainStartTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (TrainProperty obj)
        {
            if (null == obj) return false;
            _property = obj.Property;           
            _level = obj.Level;           
            _isTraining = obj.IsTraining;           
            _trainStartTime = obj.TrainStartTime;           
            return true;
        }

        public void OnSyncFromParent (Msg_TrainProperty msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public TrainProperty (Msg_TrainProperty msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public TrainProperty () { 
        }
        #endregion
    }
}