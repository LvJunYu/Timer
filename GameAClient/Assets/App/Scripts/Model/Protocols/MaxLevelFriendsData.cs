//  | 冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class MaxLevelFriendsData : SyncronisticData {
        #region 字段
        /// <summary>
        /// 章节
        /// </summary>
        private int _section;
        /// <summary>
        /// 关卡
        /// </summary>
        private int _level;
        /// <summary>
        /// 好友数据列表
        /// </summary>
        private List<UserInfoSimple> _friendsDataList;
        #endregion

        #region 属性
        /// <summary>
        /// 章节
        /// </summary>
        public int Section { 
            get { return _section; }
            set { if (_section != value) {
                _section = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 关卡
        /// </summary>
        public int Level { 
            get { return _level; }
            set { if (_level != value) {
                _level = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 好友数据列表
        /// </summary>
        public List<UserInfoSimple> FriendsDataList { 
            get { return _friendsDataList; }
            set { if (_friendsDataList != value) {
                _friendsDataList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_MaxLevelFriendsData msg)
        {
            if (null == msg) return false;
            _section = msg.Section;     
            _level = msg.Level;     
            _friendsDataList = new List<UserInfoSimple>();
            for (int i = 0; i < msg.FriendsDataList.Count; i++) {
                _friendsDataList.Add(new UserInfoSimple(msg.FriendsDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (MaxLevelFriendsData obj)
        {
            if (null == obj) return false;
            return true;
        }

        public void OnSyncFromParent (Msg_MaxLevelFriendsData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MaxLevelFriendsData (Msg_MaxLevelFriendsData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public MaxLevelFriendsData () { 
            _friendsDataList = new List<UserInfoSimple>();
        }
        #endregion
    }
}