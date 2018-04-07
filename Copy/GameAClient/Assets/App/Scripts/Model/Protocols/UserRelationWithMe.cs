// 用户与我的关系 | 用户与我的关系
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserRelationWithMe : SyncronisticData<Msg_SC_DAT_UserRelationWithMe> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户ID
        /// </summary>
        private long _userId;
        /// <summary>
        /// 关注我
        /// </summary>
        private bool _followMe;
        /// <summary>
        /// 被我关注
        /// </summary>
        private bool _followedByMe;
        /// <summary>
        /// 好友
        /// </summary>
        private bool _isFriend;
        /// <summary>
        /// 被我屏蔽
        /// </summary>
        private bool _blockedByMe;
        /// <summary>
        /// 亲密值
        /// </summary>
        private int _friendliness;

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
        /// 关注我
        /// </summary>
        public bool FollowMe { 
            get { return _followMe; }
            set { if (_followMe != value) {
                _followMe = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 被我关注
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
        /// <summary>
        /// 被我屏蔽
        /// </summary>
        public bool BlockedByMe { 
            get { return _blockedByMe; }
            set { if (_blockedByMe != value) {
                _blockedByMe = value;
                SetDirty();
            }}
        }
        /// <summary>
        /// 亲密值
        /// </summary>
        public int Friendliness { 
            get { return _friendliness; }
            set { if (_friendliness != value) {
                _friendliness = value;
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
            if (_isRequesting) {
                if (_cs_userId != userId) {
                    if (null != failedCallback) failedCallback.Invoke (ENetResultCode.NR_None);
                    return;
                }
                OnRequest (successCallback, failedCallback);
            } else {
                _cs_userId = userId;
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
        }

        public bool OnSync (Msg_SC_DAT_UserRelationWithMe msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _followMe = msg.FollowMe;           
            _followedByMe = msg.FollowedByMe;           
            _isFriend = msg.IsFriend;           
            _blockedByMe = msg.BlockedByMe;           
            _friendliness = msg.Friendliness;           
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserRelationWithMe msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _followMe = msg.FollowMe;           
            _followedByMe = msg.FollowedByMe;           
            _isFriend = msg.IsFriend;           
            _blockedByMe = msg.BlockedByMe;           
            _friendliness = msg.Friendliness;           
            return true;
        } 

        public bool DeepCopy (UserRelationWithMe obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            _followMe = obj.FollowMe;           
            _followedByMe = obj.FollowedByMe;           
            _isFriend = obj.IsFriend;           
            _blockedByMe = obj.BlockedByMe;           
            _friendliness = obj.Friendliness;           
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
            OnCreate();
        }
        #endregion
    }
}