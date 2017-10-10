//  | 成就数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AchievementStatisticItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// 统计数据类别
        /// </summary>
        private int _type;
        /// <summary>
        /// 统计数据发生次数
        /// </summary>
        private long _count;
        /// <summary>
        /// 最后发生时间
        /// </summary>
        private long _lastActionTime;
        #endregion

        #region 属性
        /// <summary>
        /// 统计数据类别
        /// </summary>
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 统计数据发生次数
        /// </summary>
        public long Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 最后发生时间
        /// </summary>
        public long LastActionTime { 
            get { return _lastActionTime; }
            set { if (_lastActionTime != value) {
                _lastActionTime = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AchievementStatisticItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _count = msg.Count;     
            _lastActionTime = msg.LastActionTime;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AchievementStatisticItem obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_AchievementStatisticItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementStatisticItem (Msg_AchievementStatisticItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementStatisticItem () { 
        }
        #endregion
    }
}