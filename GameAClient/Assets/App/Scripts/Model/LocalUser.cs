﻿/********************************************************************
** Filename : LocalUser
** Author : Dong
** Date : 2015/4/7 16:00:53
** Summary : LocalUser
***********************************************************************/

using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class LocalUser : DataBase
    {
        #region 常量与字段

        public readonly static LocalUser Instance = new LocalUser();

        private readonly Account _account = Account.Instance;
		private UserInfoDetail _user;
		private UsingAvatarPart _usingAvatarData =new UsingAvatarPart();
		private ValidAvatarPart _validAvatarData =new ValidAvatarPart();
        // 抽奖相关数据
        private UserRaffleTicket _userRaffleTicket =new UserRaffleTicket();
        // 匹配挑战相关数据
        private MatchUserData _matchUserData = new MatchUserData ();

        private PersonalProjectList _personalProjectList = new PersonalProjectList ();

        #endregion
        #region 属性

        public long UserGuid
        {
            get { return _account.UserGuid; }
        }

        public Account Account
        {
            get { return _account; }
        }

        public User UserLegacy
        {
            get { return null; }
        }

		public UserInfoDetail User {
			get { return _user; }
		}
        public UsingAvatarPart UsingAvatarData {
            get {
                return this._usingAvatarData;
            }
        }

        public ValidAvatarPart ValidAvatarData {
            get {
                return this._validAvatarData;
            }
        }
        /// <summary>
        /// 抽奖相关数据
        /// </summary>
        /// <value>The raffle ticket.</value>
        public UserRaffleTicket RaffleTicket
        {
            get { return this._userRaffleTicket; }
        }
        /// <summary>
        /// 匹配挑战相关数据
        /// </summary>
        /// <value>The using avatar data.</value>
        public MatchUserData MatchUserData {
            get {
                return this._matchUserData;
            }
        }

        public PersonalProjectList PersonalProjectList {
            get {
                return this._personalProjectList;
            }
        }
        #endregion

        #region 方法
        private LocalUser()
        {
            Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLogout, OnLogout);
        }

        public bool Init()
        {
            _account.ReadCache();
            if (_account.HasLogin)
            {
                if (!UserManager.Instance.TryGetData(_account.UserGuid, out _user))
                {
                    LogHelper.Error("UserInfo missing");
                    _account.Logout();
                }
            }
            return true;
        }

        public void LoadUserData(Action successCallback, Action<ENetResultCode> failedCallback)
        {
//            Msg_CS_DAT_UserInfoDetail msg = new Msg_CS_DAT_UserInfoDetail();
//            msg.UserId = UserGuid;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_UserInfoDetail>(SoyHttpApiPath.UserInfoDetail, msg, ret =>
//            {
//                _user = UserManager.Instance.OnSyncUserData(ret);
//                if (successCallback != null)
//                {
//                    successCallback.Invoke();
//                }
//            }, (errorCode, errorMsg) => {
//                if (failedCallback != null)
//                {
//                    failedCallback.Invoke(errorCode);
//                }
//            });
			if (_user == null) {
				_user = new UserInfoDetail ();
			}
			_user.Request (
				UserGuid,
				successCallback,
				failedCallback
			);
        }

        private void OnLogout()
        {
            if (_user != null)
            {
                _user.OnLogout();
                _user = null;
            }
        }
        #endregion
    }
}