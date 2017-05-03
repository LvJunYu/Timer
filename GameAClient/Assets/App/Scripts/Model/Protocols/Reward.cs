//  | 奖励数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Reward : SyncronisticData {
        #region 字段
        /// <summary>
        /// 奖励列表
        /// </summary>
        private List<RewardItem> _itemList;
        #endregion

        #region 属性
        /// <summary>
        /// 奖励列表
        /// </summary>
        public List<RewardItem> ItemList { 
            get { return _itemList; }
            set { if (_itemList != value) {
                _itemList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_Reward msg)
        {
            if (null == msg) return false;
            _itemList = new List<RewardItem>();
            for (int i = 0; i < msg.ItemList.Count; i++) {
                _itemList.Add(new RewardItem(msg.ItemList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_Reward msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Reward (Msg_Reward msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Reward () { 
            _itemList = new List<RewardItem>();
        }
        #endregion
    }
}