//  | 奖励条目
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class RewardItem : SyncronisticData {
        #region 字段
        // ERewardType奖励类型
        private int _type;
        // 奖励子类型
        private int _subType;
        // 奖励Id
        private long _id;
        // 奖励数量
        private long _count;
        #endregion

        #region 属性
        // ERewardType奖励类型
        public int Type { 
            get { return _type; }
            set { if (_type != value) {
                _type = value;
                SetDirty();
            }}
        }
        // 奖励子类型
        public int SubType { 
            get { return _subType; }
            set { if (_subType != value) {
                _subType = value;
                SetDirty();
            }}
        }
        // 奖励Id
        public long Id { 
            get { return _id; }
            set { if (_id != value) {
                _id = value;
                SetDirty();
            }}
        }
        // 奖励数量
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