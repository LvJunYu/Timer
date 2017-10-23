//  | 成就推送数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AchievementPushData : SyncronisticData {
        #region 字段
        /// <summary>
        /// 统计数据
        /// </summary>
        private List<AchievementStatisticItem> _statisticList;
        /// <summary>
        /// 成就数据
        /// </summary>
        private List<AchievementItem> _achievementList;
        #endregion

        #region 属性
        /// <summary>
        /// 统计数据
        /// </summary>
        public List<AchievementStatisticItem> StatisticList { 
            get { return _statisticList; }
            set { if (_statisticList != value) {
                _statisticList = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 成就数据
        /// </summary>
        public List<AchievementItem> AchievementList { 
            get { return _achievementList; }
            set { if (_achievementList != value) {
                _achievementList = value;
                SetDirty();
            }}
        }
        #endregion

        #region 方法
        public bool OnSync (Msg_AchievementPushData msg)
        {
            if (null == msg) return false;
            _statisticList = new List<AchievementStatisticItem>();
            for (int i = 0; i < msg.StatisticList.Count; i++) {
                _statisticList.Add(new AchievementStatisticItem(msg.StatisticList[i]));
            }
            _achievementList = new List<AchievementItem>();
            for (int i = 0; i < msg.AchievementList.Count; i++) {
                _achievementList.Add(new AchievementItem(msg.AchievementList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (AchievementPushData obj)
        {
            if (null == obj) return false;
            if (null ==  obj.StatisticList) return false;
            if (null ==  _statisticList) {
                _statisticList = new List<AchievementStatisticItem>();
            }
            _statisticList.Clear();
            for (int i = 0; i < obj.StatisticList.Count; i++){
                _statisticList.Add(obj.StatisticList[i]);
            }
            if (null ==  obj.AchievementList) return false;
            if (null ==  _achievementList) {
                _achievementList = new List<AchievementItem>();
            }
            _achievementList.Clear();
            for (int i = 0; i < obj.AchievementList.Count; i++){
                _achievementList.Add(obj.AchievementList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_AchievementPushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementPushData (Msg_AchievementPushData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AchievementPushData () { 
            _statisticList = new List<AchievementStatisticItem>();
            _achievementList = new List<AchievementItem>();
        }
        #endregion
    }
}