// 获取等级数据 | 获取等级数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserLevel : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 工匠等级
        /// </summary>
        private int _creatorLevel;
        /// <summary>
        /// 工匠经验
        /// </summary>
        private long _creatorExp;
        /// <summary>
        /// 冒险家等级
        /// </summary>
        private int _playerLevel;
        /// <summary>
        /// 冒险家经验
        /// </summary>
        private long _playerExp;
        /// <summary>
        /// 金币
        /// </summary>
        private long _goldCoin;
        /// <summary>
        /// 钻石
        /// </summary>
        private long _diamond;
        /// <summary>
        /// 亲密币
        /// </summary>
        private int _friendlinessCoin;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 工匠等级
        /// </summary>
        public int CreatorLevel { 
            get { return _creatorLevel; }
            set { if (_creatorLevel != value) {
                _creatorLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 工匠经验
        /// </summary>
        public long CreatorExp { 
            get { return _creatorExp; }
            set { if (_creatorExp != value) {
                _creatorExp = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 冒险家等级
        /// </summary>
        public int PlayerLevel { 
            get { return _playerLevel; }
            set { if (_playerLevel != value) {
                _playerLevel = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 冒险家经验
        /// </summary>
        public long PlayerExp { 
            get { return _playerExp; }
            set { if (_playerExp != value) {
                _playerExp = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 金币
        /// </summary>
        public long GoldCoin { 
            get { return _goldCoin; }
            set { if (_goldCoin != value) {
                _goldCoin = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 钻石
        /// </summary>
        public long Diamond { 
            get { return _diamond; }
            set { if (_diamond != value) {
                _diamond = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 亲密币
        /// </summary>
        public int FriendlinessCoin { 
            get { return _friendlinessCoin; }
            set { if (_friendlinessCoin != value) {
                _friendlinessCoin = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
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
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
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
            _friendlinessCoin = msg.FriendlinessCoin;           
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
            OnCreate();
        }
        #endregion
    }
}