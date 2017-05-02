// 获取等级数据 | 获取等级数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserLevel : SyncronisticData 
    {
        #region 字段
        // sc fields----------------------------------
        // 工匠等级
        private int _creatorLevel;
        // 工匠经验
        private long _creatorExp;
        // 冒险家等级
        private int _playerLevel;
        // 冒险家经验
        private long _playerExp;
        // 金币
        private long _goldCoin;
        // 钻石
        private long _diamond;

        // cs fields----------------------------------
        // 用户id
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 工匠等级
        public int CreatorLevel { 
            get { return _creatorLevel; }
            set { if (_creatorLevel != value) {
                _creatorLevel = value;
                SetDirty();
            }}
        }
        // 工匠经验
        public long CreatorExp { 
            get { return _creatorExp; }
            set { if (_creatorExp != value) {
                _creatorExp = value;
                SetDirty();
            }}
        }
        // 冒险家等级
        public int PlayerLevel { 
            get { return _playerLevel; }
            set { if (_playerLevel != value) {
                _playerLevel = value;
                SetDirty();
            }}
        }
        // 冒险家经验
        public long PlayerExp { 
            get { return _playerExp; }
            set { if (_playerExp != value) {
                _playerExp = value;
                SetDirty();
            }}
        }
        // 金币
        public long GoldCoin { 
            get { return _goldCoin; }
            set { if (_goldCoin != value) {
                _goldCoin = value;
                SetDirty();
            }}
        }
        // 钻石
        public long Diamond { 
            get { return _diamond; }
            set { if (_diamond != value) {
                _diamond = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        // 用户id
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                return base.IsDirty;
            }
        }
        #endregion

        #region 方法
        /// <summary>
		/// 获取等级数据
		/// </summary>
		/// <param name="userId">用户id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserLevel msg = new Msg_CS_DAT_UserLevel();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserLevel>(
                SoyHttpApiPath.UserLevel, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_UserLevel msg)
        {
            if (null == msg) return false;
            _creatorLevel = msg.CreatorLevel;           
            _creatorExp = msg.CreatorExp;           
            _playerLevel = msg.PlayerLevel;           
            _playerExp = msg.PlayerExp;           
            _goldCoin = msg.GoldCoin;           
            _diamond = msg.Diamond;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserLevel msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserLevel (Msg_SC_DAT_UserLevel msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserLevel () { 
        }
        #endregion
    }
}