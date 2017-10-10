// 成就数据 | 获取成就数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class Achievement : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _userId;
        /// <summary>
        /// 统计数据
        /// </summary>
        private List<AchievementStatisticItem> _statisticList;
        /// <summary>
        /// 成就数据
        /// </summary>
        private List<AchievementItem> _achievementList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
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
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户Id
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _statisticList) {
                    for (int i = 0; i < _statisticList.Count; i++) {
                        if (null != _statisticList[i] && _statisticList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                if (null != _achievementList) {
                    for (int i = 0; i < _achievementList.Count; i++) {
                        if (null != _achievementList[i] && _achievementList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取成就数据
		/// </summary>
		/// <param name="userId">用户Id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
                OnRequest (successCallback, failedCallback);

                Msg_CS_DAT_Achievement msg = new Msg_CS_DAT_Achievement();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Achievement>(
                    SoyHttpApiPath.Achievement, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_Achievement msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
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

        public bool DeepCopy (Achievement obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
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

        public void OnSyncFromParent (Msg_SC_DAT_Achievement msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Achievement (Msg_SC_DAT_Achievement msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public Achievement () { 
            _statisticList = new List<AchievementStatisticItem>();
            _achievementList = new List<AchievementItem>();
            OnCreate();
        }
        #endregion
    }
}