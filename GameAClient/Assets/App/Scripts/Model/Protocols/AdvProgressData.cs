//  | 冒险模式关卡列表
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdvProgressData : SyncronisticData<Msg_AdvProgressData> {
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
        public bool OnSync (Msg_AdvProgressData msg)
        {
            if (null == msg) return false;
            _section = msg.Section;     
            _level = msg.Level;     
            _friendsDataList = new List<UserInfoSimple>();
            for (int i = 0; i < msg.FriendsDataList.Count; i++) {
                _friendsDataList.Add(new UserInfoSimple(msg.FriendsDataList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }

        public bool CopyMsgData (Msg_AdvProgressData msg)
        {
            if (null == msg) return false;
            _section = msg.Section;           
            _level = msg.Level;           
            if (null ==  _friendsDataList) {
                _friendsDataList = new List<UserInfoSimple>();
            }
            _friendsDataList.Clear();
            for (int i = 0; i < msg.FriendsDataList.Count; i++) {
                _friendsDataList.Add(new UserInfoSimple(msg.FriendsDataList[i]));
            }
            return true;
        } 

        public bool DeepCopy (AdvProgressData obj)
        {
            if (null == obj) return false;
            _section = obj.Section;           
            _level = obj.Level;           
            if (null ==  obj.FriendsDataList) return false;
            if (null ==  _friendsDataList) {
                _friendsDataList = new List<UserInfoSimple>();
            }
            _friendsDataList.Clear();
            for (int i = 0; i < obj.FriendsDataList.Count; i++){
                _friendsDataList.Add(obj.FriendsDataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_AdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdvProgressData (Msg_AdvProgressData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdvProgressData () { 
            _friendsDataList = new List<UserInfoSimple>();
        }
        #endregion
    }
}