// 社交关系统计 | 社交关系统计
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserRelationStatistic : SyncronisticData {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户ID
        /// </summary>
        private long _userId;
        /// <summary>
        /// 关注数
        /// </summary>
        private int _followCount;
        /// <summary>
        /// 粉丝数
        /// </summary>
        private int _followerCount;
        /// <summary>
        /// 好友数
        /// </summary>
        private int _friendCount;

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
        /// 关注数
        /// </summary>
        public int FollowCount { 
            get { return _followCount; }
            set { if (_followCount != value) {
                _followCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 粉丝数
        /// </summary>
        public int FollowerCount { 
            get { return _followerCount; }
            set { if (_followerCount != value) {
                _followerCount = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 好友数
        /// </summary>
        public int FriendCount { 
            get { return _friendCount; }
            set { if (_friendCount != value) {
                _friendCount = value;
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
		/// 社交关系统计
		/// </summary>
		/// <param name="userId">用户id.</param>
        public void Request (
            long userId,
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            OnRequest (successCallback, failedCallback);

            Msg_CS_DAT_UserRelationStatistic msg = new Msg_CS_DAT_UserRelationStatistic();
            msg.UserId = userId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserRelationStatistic>(
                SoyHttpApiPath.UserRelationStatistic, msg, ret => {
                    if (OnSync(ret)) {
                        OnSyncSucceed(); 
                    }
                }, (failedCode, failedMsg) => {
                    OnSyncFailed(failedCode, failedMsg);
            });
        }

        public bool OnSync (Msg_SC_DAT_UserRelationStatistic msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _followCount = msg.FollowCount;           
            _followerCount = msg.FollowerCount;           
            _friendCount = msg.FriendCount;           
            OnSyncPartial();
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserRelationStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRelationStatistic (Msg_SC_DAT_UserRelationStatistic msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserRelationStatistic () { 
        }
        #endregion
    }
}