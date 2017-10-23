//  | 奖励条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RewardItem : SyncronisticData {
        #region 字段
        /// <summary>
        /// ERewardType奖励类型
        /// </summary>
        private int _type;
        /// <summary>
        /// 奖励子类型
        /// </summary>
        private int _subType;
        /// <summary>
        /// 奖励Id
        /// </summary>
        private long _id;
        /// <summary>
        /// 奖励数量
        /// </summary>
        private long _count;
        #endregion

        #region 属性
        /// <summary>
        /// ERewardType奖励类型
        /// </summary>
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖励子类型
        /// </summary>
        public int SubType { 
            get { return _subType; }
            set { if (_subType != value) {
                _subType = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖励Id
        /// </summary>
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 奖励数量
        /// </summary>
        public long Count { 
            get { return _count; }
            set { if (_count != value) {
                _count = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_RewardItem msg)
        {
            if (null == msg) return false;
            _type = msg.Type;     
            _subType = msg.SubType;     
            _id = msg.Id;     
            _count = msg.Count;     
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (RewardItem obj)
        {
            if (null == obj) return false;
            _type = obj.Type;           
            _subType = obj.SubType;           
            _id = obj.Id;           
            _count = obj.Count;           
            return true;
        }

        public void OnSyncFromParent (Msg_RewardItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RewardItem (Msg_RewardItem msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public RewardItem () { 
        }
        #endregion
    }
}