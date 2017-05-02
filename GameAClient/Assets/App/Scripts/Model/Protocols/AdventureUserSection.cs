// 获取冒险章节用户数据 | 获取冒险章节用户数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class AdventureUserSection : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 
        private int _section;
        // 
        private int _treasureMapBuyCount;
        // 
        private List<AdventureUserLevelDataDetail> _normalLevelUserDataList;
        // 
        private List<AdventureUserLevelDataDetail> _bonusLevelUserDataList;

        // cs fields----------------------------------
        // 用户
        private long _cs_userId;
        // 章节
        private int _cs_section;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 
        public int Section { 
            get { return _section; }
            set { if (_section != value) {
                _section = value;
                SetDirty();
            }}
        }
        // 
        public int TreasureMapBuyCount { 
            get { return _treasureMapBuyCount; }
            set { if (_treasureMapBuyCount != value) {
                _treasureMapBuyCount = value;
                SetDirty();
            }}
        }
        // 
        public List<AdventureUserLevelDataDetail> NormalLevelUserDataList { 
            get { return _normalLevelUserDataList; }
            set { if (_normalLevelUserDataList != value) {
                _normalLevelUserDataList = value;
                SetDirty();
            }}
        }
        // 
        public List<AdventureUserLevelDataDetail> BonusLevelUserDataList { 
            get { return _bonusLevelUserDataList; }
            set { if (_bonusLevelUserDataList != value) {
                _bonusLevelUserDataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }
        // 章节
        public int CS_Section { 
            get { return _cs_section; }
            set { _cs_section = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _normalLevelUserDataList) {
                    for (int i = 0; i < _normalLevelUserDataList.Count; i++) {
                        if (null != _normalLevelUserDataList[i] && _normalLevelUserDataList[i].IsDirty) {
                            return true;
                        }
                    }
                }
                if (null != _bonusLevelUserDataList) {
                    for (int i = 0; i < _bonusLevelUserDataList.Count; i++) {
                        if (null != _bonusLevelUserDataList[i] && _bonusLevelUserDataList[i].IsDirty) {
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
		/// 获取冒险章节用户数据
		/// </summary>
		/// <param name="userId">用户.</param>
		/// <param name="section">章节.</param>
        public void Request (
            long userId,
            int section,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_AdventureUserSection msg = new Msg_CS_DAT_AdventureUserSection();
            msg.UserId = userId;
            msg.Section = section;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_AdventureUserSection>(
                SoyHttpApiPath.AdventureUserSection, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_AdventureUserSection msg)
        {
            if (null == msg) return false;
            _section = msg.Section;           
            _treasureMapBuyCount = msg.TreasureMapBuyCount;           
            _normalLevelUserDataList = new List<AdventureUserLevelDataDetail>();
            for (int i = 0; i < msg.NormalLevelUserDataList.Count; i++) {
                _normalLevelUserDataList.Add(new AdventureUserLevelDataDetail(msg.NormalLevelUserDataList[i]));
            }
            _bonusLevelUserDataList = new List<AdventureUserLevelDataDetail>();
            for (int i = 0; i < msg.BonusLevelUserDataList.Count; i++) {
                _bonusLevelUserDataList.Add(new AdventureUserLevelDataDetail(msg.BonusLevelUserDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_AdventureUserSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserSection (Msg_SC_DAT_AdventureUserSection msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public AdventureUserSection () { 
            _normalLevelUserDataList = new List<AdventureUserLevelDataDetail>();
            _bonusLevelUserDataList = new List<AdventureUserLevelDataDetail>();
        }
        #endregion
    }
}