// 冒险模式好友最高关数据 | 冒险模式好友最高关数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class FriendMaxLevelData : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 最好关卡好友数据列表
        /// </summary>
        private List<MaxLevelFriendsData> _maxLevelFriendsDataList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 最好关卡好友数据列表
        /// </summary>
        public List<MaxLevelFriendsData> MaxLevelFriendsDataList { 
            get { return _maxLevelFriendsDataList; }
            set { if (_maxLevelFriendsDataList != value) {
                _maxLevelFriendsDataList = value;
                SetDirty();
            }}
        }
        
        // cs properties----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        public long CS_UserId { 
            get { return _cs_userId; }
            set { _cs_userId = value; }
        }

        public override bool IsDirty {
            get {
                if (null != _maxLevelFriendsDataList) {
                    for (int i = 0; i < _maxLevelFriendsDataList.Count; i++) {
                        if (null != _maxLevelFriendsDataList[i] && _maxLevelFriendsDataList[i].IsDirty) {
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
		/// 冒险模式好友最高关数据
		/// </summary>
		/// <param name="userId">用户.</param>
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

                Msg_CS_DAT_FriendMaxLevelData msg = new Msg_CS_DAT_FriendMaxLevelData();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_FriendMaxLevelData>(
                    SoyHttpApiPath.FriendMaxLevelData, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_FriendMaxLevelData msg)
        {
            if (null == msg) return false;
            _maxLevelFriendsDataList = new List<MaxLevelFriendsData>();
            for (int i = 0; i < msg.MaxLevelFriendsDataList.Count; i++) {
                _maxLevelFriendsDataList.Add(new MaxLevelFriendsData(msg.MaxLevelFriendsDataList[i]));
            }
            OnSyncPartial();
            return true;
        }

        public bool DeepCopy (FriendMaxLevelData obj)
        {
            if (null == obj) return false;
            if (null ==  obj.MaxLevelFriendsDataList) return false;
            if (null ==  _maxLevelFriendsDataList) {
                _maxLevelFriendsDataList = new List<MaxLevelFriendsData>();
            }
            _maxLevelFriendsDataList.Clear();
            for (int i = 0; i < obj.MaxLevelFriendsDataList.Count; i++){
                _maxLevelFriendsDataList.Add(obj.MaxLevelFriendsDataList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_FriendMaxLevelData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendMaxLevelData (Msg_SC_DAT_FriendMaxLevelData msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public FriendMaxLevelData () { 
            _maxLevelFriendsDataList = new List<MaxLevelFriendsData>();
            OnCreate();
        }
        #endregion
    }
}