/********************************************************************
** Filename : Account
** Author : quansiwei
** Date : 16/3/24 上午10:17:07
** Summary : Account
***********************************************************************/

using System;
using UnityEngine;
using SoyEngine.Proto;
using GameA;
using NewResourceSolution;

namespace SoyEngine
{
    public class Account
    {
        private static string AccountTokenKey = "AccountToken" + Application.platform;
        private static string AccountGuidKey = "AccountGuid" + Application.platform;
        private long _userGuid;
        private static string _token;
        private static string _deviceId;
        private static EPhoneType _devicePlatform;
        private static readonly Account _instance = new Account();
        public static Account Instance
        {
            get { return _instance; }
        }

        public long UserGuid
        {
            get { return _userGuid; }
        }

        public EPhoneType DevicePlatform
        {
            get { return _devicePlatform; }
        }

        public string Token
        {
            get { return _token; }
        }

        public bool HasLogin
        {
            get { return _userGuid != 0; }
        }


        private Account()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _devicePlatform = EPhoneType.PhT_iPhone;

            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                _devicePlatform = EPhoneType.PhT_Android;

            }
            else
            {
                _devicePlatform = EPhoneType.PhT_None;

            }
        }

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

        public void ChangePassword(
            string oldPassword,
            string newPassword,
            string verificationCode,
            Action<Msg_SC_CMD_ChangePassword> successCallback, Action<EChangePasswordCode> failedCallback
            )
        {
            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.ChangePassword(oldPassword, newPassword,verificationCode, ret =>
                {
                    if (ret.ResultCode == (int)EChangePasswordCode.CPC_Success)
                    {
                        _userGuid = ret.UserId;
                        OnTokenChange(ret.Token);
                        if (null != successCallback)
                        {
                            successCallback.Invoke(ret);
                        }
                    }
                    else
                    {
                        failedCallback.Invoke((EChangePasswordCode)ret.ResultCode);
                    }
                }, (errorCode) => {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(EChangePasswordCode.CPC_None);
                    }
                });

        }

        public  void ResetPassword(
            string account,
            string password,
            EAccountIdentifyType accountType,
            string verificationCode,
            Action<Msg_SC_CMD_ForgetPassword> successCallback, Action<EForgetPasswordCode> failedCallback)
        {

            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.ForgetPassword(account, password, accountType, verificationCode, GlobalVar.Instance.AppVersion,
                Res, _devicePlatform, ret =>
                {
                    if (ret.ResultCode == (int)EForgetPasswordCode.FPC_Success)
                    {
                        _userGuid = ret.UserId;
                        OnTokenChange(ret.Token);
                        if (null != successCallback)
                        {
                            successCallback.Invoke(ret);
                        }
                    }
                    else
                    {
                        failedCallback.Invoke((EForgetPasswordCode)ret.ResultCode);
                    }
                }, (errorCode) => {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(EForgetPasswordCode.FPC_None);
                    }
                });
        }

        public void SignUp(
            string account,
            string password,
            EAccountIdentifyType accountType,
            string verificationCode,
            Action<Msg_SC_CMD_Register> successCallback, Action<ERegisterCode> failedCallback)
        {
            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.Register(account, password, accountType, verificationCode, GlobalVar.Instance.AppVersion,
                Res, _devicePlatform, ret =>
                {
                    if (ret.ResultCode == (int)ERegisterCode.RegisterC_Success)
                    {
                        _userGuid = ret.UserId;
                        OnTokenChange(ret.Token);
                        if (null != successCallback)
                        {
                            successCallback.Invoke(ret);
                        }
                    }
                    //else if (ret.ResultCode == (int)ERegisterCode.)
                    //{
                    //    _userGuid = ret.UserId;
                    //    OnTokenChange(ret.Token.ToString());
                    //    if (null != successCallback)
                    //    {
                    //        successCallback.Invoke();
                    //    }
                    //}
                    else
                    {
                        failedCallback.Invoke((ERegisterCode)ret.ResultCode);
                    }
                }, (errorCode) => {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(ERegisterCode.RegisterC_None);
                    }
                });
        }


        public void LoginByToken(Action successCallback, Action<ELoginByTokenCode> failedCallback)
        {
          
            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.LoginByToken(GlobalVar.Instance.AppVersion, Res, _devicePlatform, ret =>
            {
                if (ret.ResultCode == (int)ELoginByTokenCode.LBTC_Success)
                {
                    _userGuid = ret.UserId; 
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                    }
                }
                else if (ret.ResultCode == (int)ELoginByTokenCode.LBTC_SuccessNewToken)
                {
                    _userGuid = ret.UserId;
                    OnTokenChange(ret.NewToken.ToString());
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                    }
                }
                else
                {
                    failedCallback.Invoke((ELoginByTokenCode)ret.ResultCode);
                }
            }, (errorCode) => {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ELoginByTokenCode.LBTC_None);
                }
            });
        }

        public void GuestLoginIn(Action successCallback, Action<ELoginByTokenCode> failedCallback)
        {
            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.LoginAsGuest(RuntimeConfig.Instance.Version, Res, LocalUser.Instance.Account.DevicePlatform, ret =>
            {
                if (ret.ResultCode == (int)ELoginByTokenCode.LBTC_Success)
                {
                    _userGuid = ret.UserId;
                    // Debug.Log("____pre_______"+)
                    if (ret.ResultCode == (int)ELoginAsGuestCode.LAGC_Success)
                    {
                        OnTokenChange(ret.Token.ToString());
                    }
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                    }
                }
                else
                {
                    failedCallback.Invoke((ELoginByTokenCode)ret.ResultCode);
                }
            }, (errorCode) => {
                if (null != failedCallback)
                {
                    failedCallback.Invoke(ELoginByTokenCode.LBTC_None);
                }
            });
            //(ret) => { SocialApp.Instance.LoginSucceed(); },
            // (ret) => { }
            //);
        }


        public void OnLogin(
            string account,
            string password,
            EAccountIdentifyType accountType,
            Msg_SNSUserInfo snsUserInfo,
            Action<Msg_SC_CMD_Login> successCallback, Action<ELoginCode> failedCallback,
            UnityEngine.WWWForm form = null)
        {
            MessengerAsync.Broadcast(EMessengerType.OnAccountLogin);
            MessengerAsync.Broadcast(EMessengerType.OnAccountLoginStateChanged);
            //Msg_SNSUserInfo msg_SNSUserInfo = new Msg_SNSUserInfo();

            string Res = AppData.Instance.AppResVersion.ToString();
            RemoteCommands.Login(account, password, accountType, snsUserInfo, GlobalVar.Instance.AppVersion,
                Res, _devicePlatform, ret =>
                {
                    if (ret.ResultCode == (int)ELoginCode.LoginC_Success)
                    {
                        _userGuid = ret.UserId;
                        OnTokenChange(ret.Token.ToString());
                        if (null != successCallback)
                        {
                            successCallback.Invoke(ret);
                        }
                    }
                    else
                    {
                        failedCallback.Invoke((ELoginCode)ret.ResultCode);
                    }
                }, (errorCode) => {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(ELoginCode.LoginC_None);
                    }
                });
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