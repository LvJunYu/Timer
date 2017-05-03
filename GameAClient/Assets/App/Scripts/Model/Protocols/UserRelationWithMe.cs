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
        /// <summary>
        /// 用户ID
        /// </summary>
        private long _userId;
        /// <summary>
        /// 
        /// </summary>
        private bool _followMe;
        /// <summary>
        /// 
        /// </summary>
        private bool _followedByMe;
        /// <summary>
        /// 好友
        /// </summary>
        private bool _isFriend;

        // cs fields----------------------------------
        /// <summary>
        /// 用户id
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { 
            get { return _userId; }
            set { if (_userId != value) {
                _userId = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool FollowMe { 
            get { return _followMe; }
            set { if (_followMe != value) {
                _followMe = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 
        /// </summary>
        public bool FollowedByMe { 
            get { return _followedByMe; }
            set { if (_followedByMe != value) {
                _followedByMe = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 好友
        /// </summary>
        public bool IsFriend { 
            get { return _isFriend; }
            set { if (_isFriend != value) {
                _isFriend = value;
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