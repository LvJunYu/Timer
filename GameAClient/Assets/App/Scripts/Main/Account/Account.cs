﻿/********************************************************************
** Filename : Account
** Author : quansiwei
** Date : 16/3/24 上午10:17:07
** Summary : Account
***********************************************************************/

using System;
using UnityEngine;
using SoyEngine.Proto;

namespace SoyEngine
{
    public class Account
    {
        private static string AccountTokenKey = "AccountToken";
        private static string AccountGuidKey = "AccountGuid";
        private long _userGuid;
        private string _apiPath;
        private static string _token;
        private static string _deviceId;
        private static readonly Account _instance = new Account();
        public static Account Instance
        {
            get { return _instance; }
        }

        public long UserGuid
        {
            get { return _userGuid; }
        }

        public string Token
        {
            get { return _token; }
        }

        public bool HasLogin
        {
            get { return _userGuid != 0; }
        }

        public string ApiPath
        {
            get
            {
                return _apiPath;
            }

            set
            {
                _apiPath = value;
            }
        }

        private Account() {}

        public void ReadCache()
        {
            _token = PlayerPrefs.GetString(AccountTokenKey, "");
            string str = PlayerPrefs.GetString(AccountGuidKey, "0");
            long.TryParse(str, out _userGuid);
            if (_userGuid == 0)
            {
                _token = "";
                Save();
            }
        }


        public void Logout()
        {
            _userGuid = 0;
            _token = "";
            Save();
            MessengerAsync.Broadcast(EMessengerType.OnAccountLoginStateChanged);
            MessengerAsync.Broadcast(EMessengerType.OnAccountLogout);
        }


        public void LoginByToken(Action successCallback, Action<ELoginByTokenCode> failedCallback)
        {
            Msg_CS_CMD_LoginByToken msg = new Msg_CS_CMD_LoginByToken();
            msg.AppVersion = GlobalVar.Instance.AppVersion;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                msg.DevicePlatform = EPhoneType.PhT_iPhone;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                msg.DevicePlatform = EPhoneType.PhT_Android;
            }
            else
            {
                msg.DevicePlatform = EPhoneType.PhT_None;
            }

            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_LoginByToken>(_apiPath, msg, ret =>
            {
                if (ret.ResultCode == (int)ELoginByTokenCode.LBTC_Success)
                {
                    _userGuid = ret.UserId;
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                    }
                }
                else
                {
                    failedCallback.Invoke((ELoginByTokenCode)ret.ResultCode);
                }
            }, (errorCode, errorMsg) => {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ELoginByTokenCode.LBTC_None);
                }
            });
        }


        public void OnLogin(long userGuid, string token)
        {
            _userGuid = userGuid;
            _token = token;
            Save();
            MessengerAsync.Broadcast(EMessengerType.OnAccountLogin);
            MessengerAsync.Broadcast(EMessengerType.OnAccountLoginStateChanged);
        }

        public void OnTokenChange(string token)
        {
            _token = token;
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetString(AccountTokenKey, _token);
            PlayerPrefs.SetString(AccountGuidKey, _userGuid.ToString());
            PlayerPrefs.Save();
        }

        public static void AppHttpClientAccountInspector(WWWForm form)
        {
            if (Instance.HasLogin)
            {
                form.AddField("token", _token);
            }
            if (_deviceId == null)
            {
                _deviceId = SystemInfo.deviceUniqueIdentifier;
            }
            form.AddField("deviceId", _deviceId);
        }
    }
}