// 冒险模式用户数据 | 冒险模式用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        // 体力数据
        private UserEnergy _userEnergyData;
        // 冒险进度
        private AdventureUserProgress _adventureUserProgress;
        // 用户章节数据列表
        private List<AdventureUserSection> _sectionList;

        // cs fields----------------------------------
        // 用户
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 体力数据
        public UserEnergy UserEnergyData { 
            get { return _userEnergyData; }
            set { if (_userEnergyData != value) {
                _userEnergyData = value;
                SetDirty();
            }}
        }
        // 冒险进度
        public AdventureUserProgress AdventureUserProgress { 
            get { return _adventureUserProgress; }
            set { if (_adventureUserProgress != value) {
                _adventureUserProgress = value;
                SetDirty();
            }}
        }
        // 用户章节数据列表
        public List<AdventureUserSection> SectionList { 
            get { return _sectionList; }
            set { if (_sectionList != value) {
                _sectionList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _userEnergyData && _userEnergyData.IsDirty) {
                    return true;
                }
                if (null != _adventureUserProgress && _adventureUserProgress.IsDirty) {
                    return true;
                }
                if (null != _sectionList) {
                    for (int i = 0; i < _sectionList.Count; i++) {
                        if (null != _sectionList[i] && _sectionList[i].IsDirty) {
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
		/// 冒险模式用户数据
		/// </summary>
		/// <param name="userId">用户.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_AdventureUserData msg = new Msg_CS_DAT_AdventureUserData();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserData>(
                SoyHttpApiPath.AdventureUserData, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_AdventureUserData msg)
        {
            if (null == msg) return false;
            if (null == _userEnergyData) {
                _userEnergyData = new UserEnergy(msg.UserEnergyData);
            } else {
                _userEnergyData.OnSyncFromParent(msg.UserEnergyData);
            }
            if (null == _adventureUserProgress) {
                _adventureUserProgress = new AdventureUserProgress(msg.AdventureUserProgress);
            } else {
                _adventureUserProgress.OnSyncFromParent(msg.AdventureUserProgress);
            }
            _sectionList = new List<AdventureUserSection>();
            for (int i = 0; i < msg.SectionList.Count; i++) {
                _sectionList.Add(new AdventureUserSection(msg.SectionList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AdventureUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserData (Msg_SC_DAT_AdventureUserData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserData () { 
            _userEnergyData = new UserEnergy();
            _adventureUserProgress = new AdventureUserProgress();
            _sectionList = new List<AdventureUserSection>();
        }
        #endregion
    }
}