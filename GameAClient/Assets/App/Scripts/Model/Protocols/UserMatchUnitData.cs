//  | 用户改造可用地块数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserMatchUnitData : SyncronisticData {
        #region 字段
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _userId;
        /// <summary>
        /// 数据列表
        /// </summary>
        private List<MatchUnitItem> _itemList;
        #endregion

        #region 属性
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 数据列表
        /// </summary>
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

        public bool DeepCopy (UserMatchUnitData obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            if (null ==  obj.ItemList) return false;
            if (null ==  _itemList) {
                _itemList = new List<MatchUnitItem>();
            }
            _itemList.Clear();
            for (int i = 0; i < obj.ItemList.Count; i++){
                _itemList.Add(obj.ItemList[i]);
            }
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