//  | 用户改造可用地块数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMatchUnitData : SyncronisticData {
        #region 字段
        // 用户Id
        private long _userId;
        // 数据列表
        private List<MatchUnitItem> _itemList;
        #endregion

        #region 属性
        // 用户Id
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 数据列表
        public List<MatchUnitItem> ItemList { 
            get { return _itemList; }
            set { if (_itemList != value) {
                _itemList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_UserMatchUnitData msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;     
            _itemList = new List<MatchUnitItem>();
            for (int i = 0; i < msg.ItemList.Count; i++) {
                _itemList.Add(new MatchUnitItem(msg.ItemList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_UserMatchUnitData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMatchUnitData (Msg_UserMatchUnitData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserMatchUnitData () { 
            _itemList = new List<MatchUnitItem>();
        }
        #endregion
    }
}