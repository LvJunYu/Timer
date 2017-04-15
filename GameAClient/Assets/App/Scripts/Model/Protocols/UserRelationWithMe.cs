// 用户与我的关系 | 用户与我的关系
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserRelationWithMe : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        // 用户ID
        private long _userId;
        // 
        private bool _followMe;
        // 
        private bool _followedByMe;
        // 好友
        private bool _isFriend;

        // cs fields----------------------------------
        // 用户id
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        // 用户ID
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        // 
        public bool FollowMe { 
            get { return _followMe; }
            set { if (_followMe != value) {
                _followMe = value;
                SetDirty();
            }}
        }
        // 
        public bool FollowedByMe { 
            get { return _followedByMe; }
            set { if (_followedByMe != value) {
                _followedByMe = value;
                SetDirty();
            }}
        }
        // 好友
        public bool IsFriend { 
            get { return _isFriend; }
            set { if (_isFriend != value) {
                _isFriend = value;
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
		/// 用户与我的关系
		/// </summary>
		/// <param name="userId">用户id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserRelationWithMe msg = new Msg_CS_DAT_UserRelationWithMe();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserRelationWithMe>(
                SoyHttpApiPath.UserRelationWithMe, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_UserRelationWithMe msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _followMe = msg.FollowMe;           
            _followedByMe = msg.FollowedByMe;           
            _isFriend = msg.IsFriend;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserRelationWithMe msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRelationWithMe (Msg_SC_DAT_UserRelationWithMe msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRelationWithMe () { 
        }
        #endregion
    }
}