/********************************************************************
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
		private UsingAvatarPart _usingAvatarData;
		private ValidAvatarPart _validAvatarData;
//        private User _user;

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
        #endregion

        #region 方法
        private LocalUser()
        {
            Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLogout, OnLogout);
			_usingAvatarData = new UsingAvatarPart ();
			_validAvatarData = new ValidAvatarPart ();
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