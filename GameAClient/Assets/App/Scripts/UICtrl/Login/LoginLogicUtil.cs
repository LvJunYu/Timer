  /********************************************************************
  ** Filename : LoginLogicUtil.cs
  ** Author : quan
  ** Date : 2016/6/24 10:57
  ** Summary : LoginLogicUtil.cs
  ***********************************************************************/

using System;
using System.Collections;
using System.Text;
using cn.sharesdk.unity3d;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public static class LoginLogicUtil
    {
        private const string PhoneNumKey = "PhoneNum";
        private static bool _hasInited;
        private static string _phoneNum;
        public static Action OnSuccess;
        public static Action OnFailed;
//        public static Action<Msg_CA_Login> OnSnsInfoLogin;
        public static Action OnSnsInfoCancel;
        public static string PhoneNum
        {
            get
            {
                return _phoneNum;
            }
            set
            {
                _phoneNum = value;
                PlayerPrefs.SetString(PhoneNumKey, value);
                PlayerPrefs.Save();
            }
        }

        public static void Init()
        {
            if(_hasInited)
            {
                return;
            }
            ShareSDK.Instance.authHandler = OnAuthResultHandler;
            ShareSDK.Instance.showUserHandler = OnGetUserInfoResultHandler;
            Messenger<ENetResultCode, string>.AddListener(SoyEngine.EMessengerType.OnAppHttpClientReceiveError, OnAppHttpClientReceiveError);
            _phoneNum = PlayerPrefs.GetString(PhoneNumKey, string.Empty).Trim();
            _hasInited = true;
        }

        private static void OnAppHttpClientReceiveError(ENetResultCode code, string msgStr)
        {
            if(code == ENetResultCode.NR_TokenExpired)
            {
                CommonTools.ShowPopupDialog("登录信息已经过期，请重新登录");
                LocalUser.Instance.Account.Logout();
//                SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
            }
        }

        public static void OnQQ()
        {
            if(!Application.isMobilePlatform)
            {
                if(OnSnsInfoCancel != null)
                {
                    OnSnsInfoCancel.Invoke();
                }
                return;
            }
            ShareSDK.Instance.Authorize(PlatformType.QQ);
        }

        public static void OnWeibo()
        {
            if(!Application.isMobilePlatform)
            {
                if(OnSnsInfoCancel != null)
                {
                    OnSnsInfoCancel.Invoke();
                }
                return;
            }
            ShareSDK.Instance.Authorize(PlatformType.SinaWeibo);
        }

        public static void OnWeChat()
        {
            if(!Application.isMobilePlatform)
            {
                if(OnSnsInfoCancel != null)
                {
                    OnSnsInfoCancel.Invoke();
                }
                return;
            }
            ShareSDK.Instance.Authorize(PlatformType.WeChat);
        }


        public static void ShowPhoneNumCheckTip(bool checkResult)
        {
            if(checkResult)
            {

            }
            else
            {
                CommonTools.ShowPopupDialog("手机号码输入有误");
            }
        }

        public static void ShowPasswordCheckTip(CheckTools.ECheckPasswordResult checkResult)
        {
            if(checkResult == CheckTools.ECheckPasswordResult.Success)
            {
            }
            else if (checkResult == CheckTools.ECheckPasswordResult.TooShort)
            {
                CommonTools.ShowPopupDialog("密码过短，密码长度8-16个字符");
            }
            else if (checkResult == CheckTools.ECheckPasswordResult.TooLong)
            {
                CommonTools.ShowPopupDialog("密码过长，密码长度8-16个字符");
            }
            else
            {
                CommonTools.ShowPopupDialog("密码格式有误");
            }
        }

        public static void ShowVerificationCodeCheckTip(bool checkResult)
        {
            if(checkResult)
            {

            }
            else
            {
                CommonTools.ShowPopupDialog("验证码错误");
            }
        }

        public static void ShowLoginError(ELoginCode code)
        {
            if (code == ELoginCode.LoginC_None)
            {
            }
            else if (code == ELoginCode.LoginC_ErrorTooMany)
            {
                CommonTools.ShowPopupDialog("登录失败次数过多，请稍后重试");
            }
            else if (code == ELoginCode.LoginC_PasswordError)
            {
                CommonTools.ShowPopupDialog("密码错误");
            }
            else if (code == ELoginCode.LoginC_UserNotExsit)
            {
                CommonTools.ShowPopupDialog("用户不存在");
            }
            else
            {
                CommonTools.ShowPopupDialog("登录失败");
            }
        }

        public static void ShowRegisterError(ERegisterCode code)
        {
            if (code == ERegisterCode.RegisterC_None)
            {
            }
            else if (code == ERegisterCode.RegisterC_IdentifyDuplication)
            {
                CommonTools.ShowPopupDialog("该账号已经存在");
            }
            else if (code == ERegisterCode.RegisterC_VerificationCodeError)
            {
                CommonTools.ShowPopupDialog("验证码错误");
            }
            else
            {
                CommonTools.ShowPopupDialog("注册失败");
            }
        }

        public static void ShowForgetPasswordError(EForgetPasswordCode code)
        {
            if (code == EForgetPasswordCode.FPC_None)
            {
            }
            else if (code == EForgetPasswordCode.FPC_AccountNotExsit)
            {
                CommonTools.ShowPopupDialog("账号不存在");
            }
            else if (code == EForgetPasswordCode.FPC_VerificationCodeError)
            {
                CommonTools.ShowPopupDialog("验证码错误");
            }
            else
            {
                CommonTools.ShowPopupDialog("找回密码失败");
            }
        }


        private static void LogHashTable(Hashtable table, string title = null)
        {
            StringBuilder sb = new StringBuilder(1024);
            if(!string.IsNullOrEmpty(title))
            {
                sb.AppendLine(title);
            }
            if(table == null)
            {
                sb.AppendLine("null");
                LogHelper.Info(sb.ToString());
                return;
            }
            foreach(DictionaryEntry en in table)
            {
                sb.AppendLine(en.Key+": "+en.Value);
            }
            LogHelper.Info(sb.ToString());
        }

        private static void OnSnsLoginCancel()
        {
            if(OnSnsInfoCancel != null)
            {
                OnSnsInfoCancel.Invoke();
            }
        }

        public static void ShowSnsLoginCancel()
        {
            CommonTools.ShowPopupDialog("授权失败或用户取消授权，登录失败");
        }

//        private static void OnWechatUserInfo(Hashtable userInfoTable, Hashtable authInfoTable)
//        {
//            Msg_CA_Login login = new Msg_CA_Login();
////            login.ClientVersion = 0;
////            login.DeviceId = SystemInfo.deviceUniqueIdentifier;
//            login.LoginType = Msg_CA_Login.ELoginType.LT_Wechat;
//            Msg_CA_Login.SNSUserInfo snsUserInfo = new Msg_CA_Login.SNSUserInfo();
//            snsUserInfo.AccessToken = ObjectToString(authInfoTable["token"]);
//            snsUserInfo.AdditionalId = ObjectToString(userInfoTable["openid"]);
//            snsUserInfo.BirthDay = 0;
//            snsUserInfo.City = ObjectToString(userInfoTable["city"]);
//            snsUserInfo.Country = ObjectToString(userInfoTable["country"]);
//            snsUserInfo.HeadImgUrl = ObjectToString(userInfoTable["headimgurl"]);
//            snsUserInfo.Pid = ObjectToString(userInfoTable["unionid"]);
//            snsUserInfo.Province = ObjectToString(userInfoTable["province"]);
//            string gender = ObjectToString(userInfoTable["sex"]);
//            if(gender == "1")
//            {
//                snsUserInfo.Sex = ESex.S_Male;
//            }
//            else if(gender == "2")
//            {
//                snsUserInfo.Sex = ESex.S_Female;
//            }
//            else
//            {
//                snsUserInfo.Sex = ESex.S_None;
//            }
//            snsUserInfo.UserNickName = ObjectToString(userInfoTable["nickname"]);
//            login.SnsUserInfo = snsUserInfo;
//            if(OnSnsInfoLogin != null)
//            {
//                OnSnsInfoLogin.Invoke(login);
//            }
//        }

//        private static void OnWeiboUserInfo(Hashtable userInfoTable, Hashtable authInfoTable)
//        {
//            Msg_CA_Login login = new Msg_CA_Login();
////            login.ClientVersion = 0;
////            login.DeviceId = SystemInfo.deviceUniqueIdentifier;
//            login.LoginType = Msg_CA_Login.ELoginType.LT_Weibo;
//            Msg_CA_Login.SNSUserInfo snsUserInfo = new Msg_CA_Login.SNSUserInfo();
//            snsUserInfo.AccessToken = ObjectToString(authInfoTable["token"]);
//            snsUserInfo.BirthDay = 0;
//            snsUserInfo.HeadImgUrl = ObjectToString(userInfoTable["avatar_hd"]);
//            snsUserInfo.Pid = ObjectToString(userInfoTable["id"]);
//            snsUserInfo.Sex = ESex.S_None;
//            string gender = ObjectToString(userInfoTable["gender"]);
//            if(gender == "m")
//            {
//                snsUserInfo.Sex = ESex.S_Male;
//            }
//            else if(gender == "f")
//            {
//                snsUserInfo.Sex = ESex.S_Female;
//            }
//            else
//            {
//                snsUserInfo.Sex = ESex.S_None;
//            }
//            snsUserInfo.UserNickName = ObjectToString(userInfoTable["name"]);
//            if(CheckTools.CheckNickName(snsUserInfo.UserNickName) != CheckTools.ECheckNickNameResult.Success)
//            {
//                snsUserInfo.UserNickName = null;
//            }
//            login.SnsUserInfo = snsUserInfo;
//            if(OnSnsInfoLogin != null)
//            {
//                OnSnsInfoLogin.Invoke(login);
//            }
//        }
//
//        private static void OnQQUserInfo(Hashtable userInfoTable, Hashtable authInfoTable)
//        {
//            Msg_CA_Login login = new Msg_CA_Login();
////            login.ClientVersion = 0;
////            login.DeviceId = SystemInfo.deviceUniqueIdentifier;
//            login.LoginType = Msg_CA_Login.ELoginType.LT_QQ;
//            Msg_CA_Login.SNSUserInfo snsUserInfo = new Msg_CA_Login.SNSUserInfo();
//            if(Application.platform == RuntimePlatform.Android)
//            {
//                snsUserInfo.AccessToken = ObjectToString(authInfoTable["token"]);
//                snsUserInfo.HeadImgUrl = ObjectToString(authInfoTable["userIcon"]);
//                snsUserInfo.Pid = ObjectToString(authInfoTable["userID"]);
//                snsUserInfo.City = ObjectToString(userInfoTable["city"]);
//                snsUserInfo.Province = ObjectToString(userInfoTable["province"]);
//                string gender = ObjectToString(authInfoTable["userGender"]);
//                if(gender == "m")
//                {
//                    snsUserInfo.Sex = ESex.S_Male;
//                }
//                else if(gender == "f")
//                {
//                    snsUserInfo.Sex = ESex.S_Female;
//                }
//                else
//                {
//                    snsUserInfo.Sex = ESex.S_None;
//                }
//                snsUserInfo.UserNickName = ObjectToString(authInfoTable["userName"]);
//            }
//            else if(Application.platform == RuntimePlatform.IPhonePlayer)
//            {
//                snsUserInfo.AccessToken = ObjectToString(authInfoTable["token"]);
//                snsUserInfo.HeadImgUrl = ObjectToString(userInfoTable["figureurl_qq_1"]);
//                snsUserInfo.Pid = ObjectToString(authInfoTable["uid"]);
//                snsUserInfo.City = ObjectToString(userInfoTable["city"]);
//                snsUserInfo.Province = ObjectToString(userInfoTable["province"]);
//                string gender = ObjectToString(userInfoTable["gender"]);
//                if(gender == "男")
//                {
//                    snsUserInfo.Sex = ESex.S_Male;
//                }
//                else if(gender == "女")
//                {
//                    snsUserInfo.Sex = ESex.S_Female;
//                }
//                else
//                {
//                    snsUserInfo.Sex = ESex.S_None;
//                }
//                snsUserInfo.UserNickName = ObjectToString(userInfoTable["nickname"]);
//            }
//            login.SnsUserInfo = snsUserInfo;
//            if(OnSnsInfoLogin != null)
//            {
//                OnSnsInfoLogin.Invoke(login);
//            }
//        }

//        private static string ObjectToString(object obj)
//        {
//            if(obj == null)
//            {
//                return null;
//            }
//            return obj.ToString().Trim();
//        }


        private static void OnAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            LogHashTable(result, "AuthResult, "+type.ToString()+", " + state.ToString());
            if (state == ResponseState.Success)
            {
                LogHelper.Info ("authorize success !" + "Platform :" + type);
                ShareSDK.Instance.GetUserInfo(type);
            }
            else if (state == ResponseState.Fail)
            {
                #if UNITY_ANDROID
                LogHelper.Info ("authorize fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
                #elif UNITY_IPHONE
                LogHelper.Info ("authorize fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
                #endif
                OnSnsLoginCancel();
            }
            else if (state == ResponseState.Cancel) 
            {
                LogHelper.Info ("authorize cancel !");
                OnSnsLoginCancel();
            }
        }

        private static void OnGetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
        {
            LogHashTable(result, "GetUserInfoResult, "+type.ToString()+", " + state.ToString());
            if (state == ResponseState.Success)
            {
                LogHelper.Info ("Get userInfo success !Platform :" + type );
                Hashtable table = ShareSDK.Instance.GetAuthInfo(type);
                LogHashTable(table, "AuthInfo, "+type.ToString());
                if(type == PlatformType.WeChat)
                {
//                    OnWechatUserInfo(result, table);
                }
                else if(type == PlatformType.SinaWeibo)
                {
//                    OnWeiboUserInfo(result, table);
                }
                else if (type == PlatformType.QQ)
                {
//                    OnQQUserInfo(result, table);
                }
            }
            else if (state == ResponseState.Fail)
            {
                #if UNITY_ANDROID
                LogHelper.Info ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
                #elif UNITY_IPHONE
                LogHelper.Info ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
                #endif
                OnSnsLoginCancel();
            }
            else if (state == ResponseState.Cancel) 
            {
                LogHelper.Info ("cancel !");
                OnSnsLoginCancel();
            }
        }
    }
}

