// 时装打折券 | 时装打折券
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserAvatarPartDiscountCoupon : SyncronisticData<Msg_SC_DAT_UserAvatarPartDiscountCoupon> {
        #region 字段
        // sc fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _userId;
        /// <summary>
        /// 
        /// </summary>
        private List<AvatarPartDiscountCouponGroup> _groupList;

        // cs fields----------------------------------
        /// <summary>
        /// 用户
        /// </summary>
        private long _cs_userId;
        #endregion

        #region 属性
        // sc properties----------------------------------
        /// <summary>
        /// 用户
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
        public List<AvatarPartDiscountCouponGroup> GroupList { 
            get { return _groupList; }
            set { if (_groupList != value) {
                _groupList = value;
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
                if (null != _groupList) {
                    for (int i = 0; i < _groupList.Count; i++) {
                        if (null != _groupList[i] && _groupList[i].IsDirty) {
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
		/// 时装打折券
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

                Msg_CS_DAT_UserAvatarPartDiscountCoupon msg = new Msg_CS_DAT_UserAvatarPartDiscountCoupon();
                msg.UserId = userId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserAvatarPartDiscountCoupon>(
                    SoyHttpApiPath.UserAvatarPartDiscountCoupon, msg, ret => {
                        if (OnSync(ret)) {
                            OnSyncSucceed(); 
                        }
                    }, (failedCode, failedMsg) => {
                        OnSyncFailed(failedCode, failedMsg);
                });            
            }            
        }

        public bool OnSync (Msg_SC_DAT_UserAvatarPartDiscountCoupon msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            _groupList = new List<AvatarPartDiscountCouponGroup>();
            for (int i = 0; i < msg.GroupList.Count; i++) {
                _groupList.Add(new AvatarPartDiscountCouponGroup(msg.GroupList[i]));
            }
            OnSyncPartial(msg);
            return true;
        }
        
        public bool CopyMsgData (Msg_SC_DAT_UserAvatarPartDiscountCoupon msg)
        {
            if (null == msg) return false;
            _userId = msg.UserId;           
            if (null ==  _groupList) {
                _groupList = new List<AvatarPartDiscountCouponGroup>();
            }
            _groupList.Clear();
            for (int i = 0; i < msg.GroupList.Count; i++) {
                _groupList.Add(new AvatarPartDiscountCouponGroup(msg.GroupList[i]));
            }
            return true;
        } 

        public bool DeepCopy (UserAvatarPartDiscountCoupon obj)
        {
            if (null == obj) return false;
            _userId = obj.UserId;           
            if (null ==  obj.GroupList) return false;
            if (null ==  _groupList) {
                _groupList = new List<AvatarPartDiscountCouponGroup>();
            }
            _groupList.Clear();
            for (int i = 0; i < obj.GroupList.Count; i++){
                _groupList.Add(obj.GroupList[i]);
            }
            return true;
        }

        public void OnSyncFromParent (Msg_SC_DAT_UserAvatarPartDiscountCoupon msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserAvatarPartDiscountCoupon (Msg_SC_DAT_UserAvatarPartDiscountCoupon msg) {
            if (OnSync(msg)) {
                OnSyncSucceed();
            }
        }

        public UserAvatarPartDiscountCoupon () { 
            _groupList = new List<AvatarPartDiscountCouponGroup>();
            OnCreate();
        }
        #endregion
    }
}