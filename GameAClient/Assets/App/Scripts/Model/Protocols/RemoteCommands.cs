// protocol command msgs

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public class RemoteCommands {
        public static bool IsRequstingLogin {
            get { return _isRequstingLogin; }
        }
        private static bool _isRequstingLogin = false;
        /// <summary>
		/// 登录
		/// </summary>
		/// <param name="account">账号</param>
		/// <param name="password">密码</param>
		/// <param name="accountType">账号类型</param>
		/// <param name="snsUserInfo"></param>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void Login (
            string account,
            string password,
            EAccountIdentifyType accountType,
            Msg_SNSUserInfo snsUserInfo,
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_Login> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLogin) {
                return;
            }
            _isRequstingLogin = true;
            Msg_CS_CMD_Login msg = new Msg_CS_CMD_Login();
            // 登录
            msg.Account = account;
            msg.Password = password;
            msg.AccountType = accountType;
            msg.SnsUserInfo = snsUserInfo;
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Login>(
                SoyHttpApiPath.Login, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLogin = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Login", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLogin = false;
                },
                form
            );
        }

        public static bool IsRequstingLoginAsGuest {
            get { return _isRequstingLoginAsGuest; }
        }
        private static bool _isRequstingLoginAsGuest = false;
        /// <summary>
		/// 游客登录
		/// </summary>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void LoginAsGuest (
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_LoginAsGuest> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLoginAsGuest) {
                return;
            }
            _isRequstingLoginAsGuest = true;
            Msg_CS_CMD_LoginAsGuest msg = new Msg_CS_CMD_LoginAsGuest();
            // 游客登录
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_LoginAsGuest>(
                SoyHttpApiPath.LoginAsGuest, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLoginAsGuest = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "LoginAsGuest", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLoginAsGuest = false;
                },
                form
            );
        }

        public static bool IsRequstingLoginByToken {
            get { return _isRequstingLoginByToken; }
        }
        private static bool _isRequstingLoginByToken = false;
        /// <summary>
		/// 设备登录包
		/// </summary>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void LoginByToken (
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_LoginByToken> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLoginByToken) {
                return;
            }
            _isRequstingLoginByToken = true;
            Msg_CS_CMD_LoginByToken msg = new Msg_CS_CMD_LoginByToken();
            // 设备登录包
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_LoginByToken>(
                SoyHttpApiPath.LoginByToken, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLoginByToken = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "LoginByToken", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLoginByToken = false;
                },
                form
            );
        }

        public static bool IsRequstingForgetPassword {
            get { return _isRequstingForgetPassword; }
        }
        private static bool _isRequstingForgetPassword = false;
        /// <summary>
		/// 忘记密码
		/// </summary>
		/// <param name="account">账号</param>
		/// <param name="password">密码</param>
		/// <param name="accountType">账号类型</param>
		/// <param name="verificationCode">验证码</param>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void ForgetPassword (
            string account,
            string password,
            EAccountIdentifyType accountType,
            string verificationCode,
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_ForgetPassword> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingForgetPassword) {
                return;
            }
            _isRequstingForgetPassword = true;
            Msg_CS_CMD_ForgetPassword msg = new Msg_CS_CMD_ForgetPassword();
            // 忘记密码
            msg.Account = account;
            msg.Password = password;
            msg.AccountType = accountType;
            msg.VerificationCode = verificationCode;
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ForgetPassword>(
                SoyHttpApiPath.ForgetPassword, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingForgetPassword = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ForgetPassword", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingForgetPassword = false;
                },
                form
            );
        }

        public static bool IsRequstingChangePassword {
            get { return _isRequstingChangePassword; }
        }
        private static bool _isRequstingChangePassword = false;
        /// <summary>
		/// 修改密码
		/// </summary>
		/// <param name="oldPassword">账号</param>
		/// <param name="newPassword">密码</param>
		/// <param name="verificationCode">验证码</param>
        public static void ChangePassword (
            string oldPassword,
            string newPassword,
            string verificationCode,
            Action<Msg_SC_CMD_ChangePassword> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingChangePassword) {
                return;
            }
            _isRequstingChangePassword = true;
            Msg_CS_CMD_ChangePassword msg = new Msg_CS_CMD_ChangePassword();
            // 修改密码
            msg.OldPassword = oldPassword;
            msg.NewPassword = newPassword;
            msg.VerificationCode = verificationCode;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangePassword>(
                SoyHttpApiPath.ChangePassword, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingChangePassword = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ChangePassword", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingChangePassword = false;
                },
                form
            );
        }

        public static bool IsRequstingRegister {
            get { return _isRequstingRegister; }
        }
        private static bool _isRequstingRegister = false;
        /// <summary>
		/// 注册
		/// </summary>
		/// <param name="account">账号</param>
		/// <param name="password">密码</param>
		/// <param name="accountType">账号类型</param>
		/// <param name="verificationCode">验证码</param>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void Register (
            string account,
            string password,
            EAccountIdentifyType accountType,
            string verificationCode,
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_Register> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingRegister) {
                return;
            }
            _isRequstingRegister = true;
            Msg_CS_CMD_Register msg = new Msg_CS_CMD_Register();
            // 注册
            msg.Account = account;
            msg.Password = password;
            msg.AccountType = accountType;
            msg.VerificationCode = verificationCode;
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Register>(
                SoyHttpApiPath.Register, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingRegister = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Register", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingRegister = false;
                },
                form
            );
        }

        public static bool IsRequstingAccountBind {
            get { return _isRequstingAccountBind; }
        }
        private static bool _isRequstingAccountBind = false;
        /// <summary>
		/// 绑定账号
		/// </summary>
		/// <param name="account">账号</param>
		/// <param name="password">密码</param>
		/// <param name="accountType">账号类型</param>
		/// <param name="verificationCode">验证码</param>
        public static void AccountBind (
            string account,
            string password,
            EAccountIdentifyType accountType,
            string verificationCode,
            Action<Msg_SC_CMD_AccountBind> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingAccountBind) {
                return;
            }
            _isRequstingAccountBind = true;
            Msg_CS_CMD_AccountBind msg = new Msg_CS_CMD_AccountBind();
            // 绑定账号
            msg.Account = account;
            msg.Password = password;
            msg.AccountType = accountType;
            msg.VerificationCode = verificationCode;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_AccountBind>(
                SoyHttpApiPath.AccountBind, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingAccountBind = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "AccountBind", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingAccountBind = false;
                },
                form
            );
        }

        public static bool IsRequstingGetVerificationCode {
            get { return _isRequstingGetVerificationCode; }
        }
        private static bool _isRequstingGetVerificationCode = false;
        /// <summary>
		/// 请求验证码
		/// </summary>
		/// <param name="account">账号</param>
		/// <param name="accountType">账号类型</param>
		/// <param name="verifyCodeType">验证码类型</param>
        public static void GetVerificationCode (
            string account,
            EAccountIdentifyType accountType,
            EVerifyCodeType verifyCodeType,
            Action<Msg_SC_CMD_GetVerificationCode> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetVerificationCode) {
                return;
            }
            _isRequstingGetVerificationCode = true;
            Msg_CS_CMD_GetVerificationCode msg = new Msg_CS_CMD_GetVerificationCode();
            // 请求验证码
            msg.Account = account;
            msg.AccountType = accountType;
            msg.VerifyCodeType = verifyCodeType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetVerificationCode>(
                SoyHttpApiPath.GetVerificationCode, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetVerificationCode = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetVerificationCode", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetVerificationCode = false;
                },
                form
            );
        }

        public static bool IsRequstingLogout {
            get { return _isRequstingLogout; }
        }
        private static bool _isRequstingLogout = false;
        /// <summary>
		/// 退出登录
		/// </summary>
		/// <param name="flag">占位</param>
        public static void Logout (
            int flag,
            Action<Msg_SC_CMD_Logout> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLogout) {
                return;
            }
            _isRequstingLogout = true;
            Msg_CS_CMD_Logout msg = new Msg_CS_CMD_Logout();
            // 退出登录
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Logout>(
                SoyHttpApiPath.Logout, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLogout = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Logout", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLogout = false;
                },
                form
            );
        }

        public static bool IsRequstingLoginByQQGame {
            get { return _isRequstingLoginByQQGame; }
        }
        private static bool _isRequstingLoginByQQGame = false;
        /// <summary>
		/// QQGame登录
		/// </summary>
		/// <param name="openId">OpenId</param>
		/// <param name="openKey">OpenKey</param>
		/// <param name="appVersion">应用版本号</param>
		/// <param name="resVersion">资源版本号</param>
		/// <param name="devicePlatform">设备类型</param>
        public static void LoginByQQGame (
            string openId,
            string openKey,
            string appVersion,
            string resVersion,
            EPhoneType devicePlatform,
            Action<Msg_SC_CMD_LoginByQQGame> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingLoginByQQGame) {
                return;
            }
            _isRequstingLoginByQQGame = true;
            Msg_CS_CMD_LoginByQQGame msg = new Msg_CS_CMD_LoginByQQGame();
            // QQGame登录
            msg.OpenId = openId;
            msg.OpenKey = openKey;
            msg.AppVersion = appVersion;
            msg.ResVersion = resVersion;
            msg.DevicePlatform = devicePlatform;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_LoginByQQGame>(
                SoyHttpApiPath.LoginByQQGame, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingLoginByQQGame = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "LoginByQQGame", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingLoginByQQGame = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateUserInfo {
            get { return _isRequstingUpdateUserInfo; }
        }
        private static bool _isRequstingUpdateUserInfo = false;
        /// <summary>
		/// 用户详细信息
		/// </summary>
		/// <param name="data">用户信息</param>
        public static void UpdateUserInfo (
            Msg_SC_DAT_UserInfoDetail data,
            Action<Msg_SC_CMD_UpdateUserInfo> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateUserInfo) {
                return;
            }
            _isRequstingUpdateUserInfo = true;
            Msg_CS_CMD_UpdateUserInfo msg = new Msg_CS_CMD_UpdateUserInfo();
            // 用户详细信息
            msg.Data = data;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateUserInfo>(
                SoyHttpApiPath.UpdateUserInfo, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateUserInfo = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateUserInfo", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateUserInfo = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateFollowState {
            get { return _isRequstingUpdateFollowState; }
        }
        private static bool _isRequstingUpdateFollowState = false;
        /// <summary>
		/// 更新关注状态
		/// </summary>
		/// <param name="userId">用户id</param>
		/// <param name="followFlag"></param>
        public static void UpdateFollowState (
            long userId,
            bool followFlag,
            Action<Msg_SC_CMD_UpdateFollowState> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateFollowState) {
                return;
            }
            _isRequstingUpdateFollowState = true;
            Msg_CS_CMD_UpdateFollowState msg = new Msg_CS_CMD_UpdateFollowState();
            // 更新关注状态
            msg.UserId = userId;
            msg.FollowFlag = followFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateFollowState>(
                SoyHttpApiPath.UpdateFollowState, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateFollowState = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateFollowState", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateFollowState = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateBlockState {
            get { return _isRequstingUpdateBlockState; }
        }
        private static bool _isRequstingUpdateBlockState = false;
        /// <summary>
		/// 更新关注状态
		/// </summary>
		/// <param name="userId">用户id</param>
		/// <param name="blockFlag"></param>
        public static void UpdateBlockState (
            long userId,
            bool blockFlag,
            Action<Msg_SC_CMD_UpdateBlockState> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateBlockState) {
                return;
            }
            _isRequstingUpdateBlockState = true;
            Msg_CS_CMD_UpdateBlockState msg = new Msg_CS_CMD_UpdateBlockState();
            // 更新关注状态
            msg.UserId = userId;
            msg.BlockFlag = blockFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateBlockState>(
                SoyHttpApiPath.UpdateBlockState, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateBlockState = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateBlockState", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateBlockState = false;
                },
                form
            );
        }

        public static bool IsRequstingSearchUser {
            get { return _isRequstingSearchUser; }
        }
        private static bool _isRequstingSearchUser = false;
        /// <summary>
		/// 搜索好友
		/// </summary>
		/// <param name="userNickName">用户名</param>
        public static void SearchUser (
            string userNickName,
            Action<Msg_SC_CMD_SearchUser> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSearchUser) {
                return;
            }
            _isRequstingSearchUser = true;
            Msg_CS_CMD_SearchUser msg = new Msg_CS_CMD_SearchUser();
            // 搜索好友
            msg.UserNickName = userNickName;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SearchUser>(
                SoyHttpApiPath.SearchUser, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSearchUser = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SearchUser", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSearchUser = false;
                },
                form
            );
        }

        public static bool IsRequstingSearchUserByShortId {
            get { return _isRequstingSearchUserByShortId; }
        }
        private static bool _isRequstingSearchUserByShortId = false;
        /// <summary>
		/// 搜索玩家ID
		/// </summary>
		/// <param name="shortId">短id</param>
        public static void SearchUserByShortId (
            long shortId,
            Action<Msg_SC_CMD_SearchUserByShortId> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSearchUserByShortId) {
                return;
            }
            _isRequstingSearchUserByShortId = true;
            Msg_CS_CMD_SearchUserByShortId msg = new Msg_CS_CMD_SearchUserByShortId();
            // 搜索玩家ID
            msg.ShortId = shortId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SearchUserByShortId>(
                SoyHttpApiPath.SearchUserByShortId, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSearchUserByShortId = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SearchUserByShortId", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSearchUserByShortId = false;
                },
                form
            );
        }

        public static bool IsRequstingPublishWorldProject {
            get { return _isRequstingPublishWorldProject; }
        }
        private static bool _isRequstingPublishWorldProject = false;
        /// <summary>
		/// 发布关卡
		/// </summary>
		/// <param name="personalProjectId"></param>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="recordUploadParam">录像上传参数</param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
		/// <param name="uploadParam">上传参数</param>
        public static void PublishWorldProject (
            long personalProjectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            Msg_RecordUploadParam recordUploadParam,
            int timeLimit,
            int winCondition,
            Msg_ProjectUploadParam uploadParam,
            Action<Msg_SC_CMD_PublishWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPublishWorldProject) {
                return;
            }
            _isRequstingPublishWorldProject = true;
            Msg_CS_CMD_PublishWorldProject msg = new Msg_CS_CMD_PublishWorldProject();
            // 发布关卡
            msg.PersonalProjectId = personalProjectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.RecordUploadParam = recordUploadParam;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            msg.UploadParam = uploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishWorldProject>(
                SoyHttpApiPath.PublishWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPublishWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPublishWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingUnpublishWorldProject {
            get { return _isRequstingUnpublishWorldProject; }
        }
        private static bool _isRequstingUnpublishWorldProject = false;
        /// <summary>
		/// 取消发布
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void UnpublishWorldProject (
            List<long> projectId,
            Action<Msg_SC_CMD_UnpublishWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnpublishWorldProject) {
                return;
            }
            _isRequstingUnpublishWorldProject = true;
            Msg_CS_CMD_UnpublishWorldProject msg = new Msg_CS_CMD_UnpublishWorldProject();
            // 取消发布
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnpublishWorldProject>(
                SoyHttpApiPath.UnpublishWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnpublishWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnpublishWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnpublishWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayWorldProject {
            get { return _isRequstingPlayWorldProject; }
        }
        private static bool _isRequstingPlayWorldProject = false;
        /// <summary>
		/// Msg_CS_CMD_PlayWorldProject
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void PlayWorldProject (
            long projectId,
            Action<Msg_SC_CMD_PlayWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayWorldProject) {
                return;
            }
            _isRequstingPlayWorldProject = true;
            Msg_CS_CMD_PlayWorldProject msg = new Msg_CS_CMD_PlayWorldProject();
            // Msg_CS_CMD_PlayWorldProject
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayWorldProject>(
                SoyHttpApiPath.PlayWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingMatchShadowBattle {
            get { return _isRequstingMatchShadowBattle; }
        }
        private static bool _isRequstingMatchShadowBattle = false;
        /// <summary>
		/// 匹配乱入对决
		/// </summary>
		/// <param name="id">BattleId</param>
        public static void MatchShadowBattle (
            long id,
            Action<Msg_SC_CMD_MatchShadowBattle> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingMatchShadowBattle) {
                return;
            }
            _isRequstingMatchShadowBattle = true;
            Msg_CS_CMD_MatchShadowBattle msg = new Msg_CS_CMD_MatchShadowBattle();
            // 匹配乱入对决
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_MatchShadowBattle>(
                SoyHttpApiPath.MatchShadowBattle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingMatchShadowBattle = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "MatchShadowBattle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingMatchShadowBattle = false;
                },
                form
            );
        }

        public static bool IsRequstingRequestHelpShadowBattle {
            get { return _isRequstingRequestHelpShadowBattle; }
        }
        private static bool _isRequstingRequestHelpShadowBattle = false;
        /// <summary>
		/// 乱入对决请求帮战
		/// </summary>
		/// <param name="shadowBattleId">Id</param>
		/// <param name="targetUserId"></param>
        public static void RequestHelpShadowBattle (
            long shadowBattleId,
            long targetUserId,
            Action<Msg_SC_CMD_RequestHelpShadowBattle> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingRequestHelpShadowBattle) {
                return;
            }
            _isRequstingRequestHelpShadowBattle = true;
            Msg_CS_CMD_RequestHelpShadowBattle msg = new Msg_CS_CMD_RequestHelpShadowBattle();
            // 乱入对决请求帮战
            msg.ShadowBattleId = shadowBattleId;
            msg.TargetUserId = targetUserId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_RequestHelpShadowBattle>(
                SoyHttpApiPath.RequestHelpShadowBattle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingRequestHelpShadowBattle = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "RequestHelpShadowBattle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingRequestHelpShadowBattle = false;
                },
                form
            );
        }

        public static bool IsRequstingGiveUpShadowBattle {
            get { return _isRequstingGiveUpShadowBattle; }
        }
        private static bool _isRequstingGiveUpShadowBattle = false;
        /// <summary>
		/// 放弃乱入对决
		/// </summary>
		/// <param name="id">Id</param>
        public static void GiveUpShadowBattle (
            long id,
            Action<Msg_SC_CMD_GiveUpShadowBattle> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGiveUpShadowBattle) {
                return;
            }
            _isRequstingGiveUpShadowBattle = true;
            Msg_CS_CMD_GiveUpShadowBattle msg = new Msg_CS_CMD_GiveUpShadowBattle();
            // 放弃乱入对决
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GiveUpShadowBattle>(
                SoyHttpApiPath.GiveUpShadowBattle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGiveUpShadowBattle = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GiveUpShadowBattle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGiveUpShadowBattle = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitWorldProjectResult {
            get { return _isRequstingCommitWorldProjectResult; }
        }
        private static bool _isRequstingCommitWorldProjectResult = false;
        /// <summary>
		/// 提交过关世界关卡数据
		/// </summary>
		/// <param name="token">关卡Id</param>
		/// <param name="recordUploadParam">录像上传参数</param>
        public static void CommitWorldProjectResult (
            long token,
            Msg_RecordUploadParam recordUploadParam,
            Action<Msg_SC_CMD_CommitWorldProjectResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitWorldProjectResult) {
                return;
            }
            _isRequstingCommitWorldProjectResult = true;
            Msg_CS_CMD_CommitWorldProjectResult msg = new Msg_CS_CMD_CommitWorldProjectResult();
            // 提交过关世界关卡数据
            msg.Token = token;
            msg.RecordUploadParam = recordUploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitWorldProjectResult>(
                SoyHttpApiPath.CommitWorldProjectResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitWorldProjectResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitWorldProjectResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitWorldProjectResult = false;
                },
                form
            );
        }

        public static bool IsRequstingReplyProjectComment {
            get { return _isRequstingReplyProjectComment; }
        }
        private static bool _isRequstingReplyProjectComment = false;
        /// <summary>
		/// 回复关卡评论
		/// </summary>
		/// <param name="commentId"></param>
		/// <param name="content">内容</param>
		/// <param name="replyOther">是否回复他人</param>
		/// <param name="targetReplyId">回复的回复ID</param>
        public static void ReplyProjectComment (
            long commentId,
            string content,
            bool replyOther,
            long targetReplyId,
            Action<Msg_SC_CMD_ReplyProjectComment> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReplyProjectComment) {
                return;
            }
            _isRequstingReplyProjectComment = true;
            Msg_CS_CMD_ReplyProjectComment msg = new Msg_CS_CMD_ReplyProjectComment();
            // 回复关卡评论
            msg.CommentId = commentId;
            msg.Content = content;
            msg.ReplyOther = replyOther;
            msg.TargetReplyId = targetReplyId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReplyProjectComment>(
                SoyHttpApiPath.ReplyProjectComment, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReplyProjectComment = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReplyProjectComment", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReplyProjectComment = false;
                },
                form
            );
        }

        public static bool IsRequstingPostWorldProjectComment {
            get { return _isRequstingPostWorldProjectComment; }
        }
        private static bool _isRequstingPostWorldProjectComment = false;
        /// <summary>
		/// 提交世界关卡评论
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="targetUserId"></param>
		/// <param name="comment"></param>
        public static void PostWorldProjectComment (
            long projectId,
            long targetUserId,
            string comment,
            Action<Msg_SC_CMD_PostWorldProjectComment> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPostWorldProjectComment) {
                return;
            }
            _isRequstingPostWorldProjectComment = true;
            Msg_CS_CMD_PostWorldProjectComment msg = new Msg_CS_CMD_PostWorldProjectComment();
            // 提交世界关卡评论
            msg.ProjectId = projectId;
            msg.TargetUserId = targetUserId;
            msg.Comment = comment;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PostWorldProjectComment>(
                SoyHttpApiPath.PostWorldProjectComment, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPostWorldProjectComment = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PostWorldProjectComment", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPostWorldProjectComment = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateWorldProjectCommentLike {
            get { return _isRequstingUpdateWorldProjectCommentLike; }
        }
        private static bool _isRequstingUpdateWorldProjectCommentLike = false;
        /// <summary>
		/// 修改评论赞
		/// </summary>
		/// <param name="commentId">关卡Id</param>
		/// <param name="likeFlag"></param>
        public static void UpdateWorldProjectCommentLike (
            long commentId,
            bool likeFlag,
            Action<Msg_SC_CMD_UpdateWorldProjectCommentLike> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateWorldProjectCommentLike) {
                return;
            }
            _isRequstingUpdateWorldProjectCommentLike = true;
            Msg_CS_CMD_UpdateWorldProjectCommentLike msg = new Msg_CS_CMD_UpdateWorldProjectCommentLike();
            // 修改评论赞
            msg.CommentId = commentId;
            msg.LikeFlag = likeFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateWorldProjectCommentLike>(
                SoyHttpApiPath.UpdateWorldProjectCommentLike, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateWorldProjectCommentLike = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateWorldProjectCommentLike", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateWorldProjectCommentLike = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateWorldProjectLike {
            get { return _isRequstingUpdateWorldProjectLike; }
        }
        private static bool _isRequstingUpdateWorldProjectLike = false;
        /// <summary>
		/// 修改关卡顶踩
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="likeState"></param>
        public static void UpdateWorldProjectLike (
            long projectId,
            EProjectLikeState likeState,
            Action<Msg_SC_CMD_UpdateWorldProjectLike> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateWorldProjectLike) {
                return;
            }
            _isRequstingUpdateWorldProjectLike = true;
            Msg_CS_CMD_UpdateWorldProjectLike msg = new Msg_CS_CMD_UpdateWorldProjectLike();
            // 修改关卡顶踩
            msg.ProjectId = projectId;
            msg.LikeState = likeState;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateWorldProjectLike>(
                SoyHttpApiPath.UpdateWorldProjectLike, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateWorldProjectLike = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateWorldProjectLike", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateWorldProjectLike = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateWorldProjectFavorite {
            get { return _isRequstingUpdateWorldProjectFavorite; }
        }
        private static bool _isRequstingUpdateWorldProjectFavorite = false;
        /// <summary>
		/// 修改关卡收藏状态
		/// </summary>
		/// <param name="projectMainId">关卡Id</param>
		/// <param name="favoriteFlag"></param>
        public static void UpdateWorldProjectFavorite (
            long projectMainId,
            bool favoriteFlag,
            Action<Msg_SC_CMD_UpdateWorldProjectFavorite> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateWorldProjectFavorite) {
                return;
            }
            _isRequstingUpdateWorldProjectFavorite = true;
            Msg_CS_CMD_UpdateWorldProjectFavorite msg = new Msg_CS_CMD_UpdateWorldProjectFavorite();
            // 修改关卡收藏状态
            msg.ProjectMainId = projectMainId;
            msg.FavoriteFlag = favoriteFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateWorldProjectFavorite>(
                SoyHttpApiPath.UpdateWorldProjectFavorite, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateWorldProjectFavorite = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateWorldProjectFavorite", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateWorldProjectFavorite = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteWorldProjectFavorite {
            get { return _isRequstingDeleteWorldProjectFavorite; }
        }
        private static bool _isRequstingDeleteWorldProjectFavorite = false;
        /// <summary>
		/// 删除收藏关卡
		/// </summary>
		/// <param name="projecMaintId">关卡Id</param>
        public static void DeleteWorldProjectFavorite (
            List<long> projecMaintId,
            Action<Msg_SC_CMD_DeleteWorldProjectFavorite> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteWorldProjectFavorite) {
                return;
            }
            _isRequstingDeleteWorldProjectFavorite = true;
            Msg_CS_CMD_DeleteWorldProjectFavorite msg = new Msg_CS_CMD_DeleteWorldProjectFavorite();
            // 删除收藏关卡
            msg.ProjecMaintId.AddRange(projecMaintId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteWorldProjectFavorite>(
                SoyHttpApiPath.DeleteWorldProjectFavorite, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteWorldProjectFavorite = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteWorldProjectFavorite", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteWorldProjectFavorite = false;
                },
                form
            );
        }

        public static bool IsRequstingDownloadProject {
            get { return _isRequstingDownloadProject; }
        }
        private static bool _isRequstingDownloadProject = false;
        /// <summary>
		/// 下载关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void DownloadProject (
            long projectId,
            Action<Msg_SC_CMD_DownloadProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDownloadProject) {
                return;
            }
            _isRequstingDownloadProject = true;
            Msg_CS_CMD_DownloadProject msg = new Msg_CS_CMD_DownloadProject();
            // 下载关卡
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DownloadProject>(
                SoyHttpApiPath.DownloadProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDownloadProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DownloadProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDownloadProject = false;
                },
                form
            );
        }

        public static bool IsRequstingSearchWorldProject {
            get { return _isRequstingSearchWorldProject; }
        }
        private static bool _isRequstingSearchWorldProject = false;
        /// <summary>
		/// 搜索关卡
		/// </summary>
		/// <param name="shortId">短id</param>
        public static void SearchWorldProject (
            long shortId,
            Action<Msg_SC_CMD_SearchWorldProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSearchWorldProject) {
                return;
            }
            _isRequstingSearchWorldProject = true;
            Msg_CS_CMD_SearchWorldProject msg = new Msg_CS_CMD_SearchWorldProject();
            // 搜索关卡
            msg.ShortId = shortId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SearchWorldProject>(
                SoyHttpApiPath.SearchWorldProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSearchWorldProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SearchWorldProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSearchWorldProject = false;
                },
                form
            );
        }

        public static bool IsRequstingGetProjectByMainId {
            get { return _isRequstingGetProjectByMainId; }
        }
        private static bool _isRequstingGetProjectByMainId = false;
        /// <summary>
		/// 通过主id和版本号获取关卡
		/// </summary>
		/// <param name="mainId">关卡Id</param>
		/// <param name="version">特定版本号</param>
        public static void GetProjectByMainId (
            long mainId,
            int version,
            Action<Msg_SC_CMD_GetProjectByMainId> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetProjectByMainId) {
                return;
            }
            _isRequstingGetProjectByMainId = true;
            Msg_CS_CMD_GetProjectByMainId msg = new Msg_CS_CMD_GetProjectByMainId();
            // 通过主id和版本号获取关卡
            msg.MainId = mainId;
            msg.Version = version;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetProjectByMainId>(
                SoyHttpApiPath.GetProjectByMainId, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetProjectByMainId = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetProjectByMainId", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetProjectByMainId = false;
                },
                form
            );
        }

        public static bool IsRequstingCreateProject {
            get { return _isRequstingCreateProject; }
        }
        private static bool _isRequstingCreateProject = false;
        /// <summary>
		/// 创建关卡
		/// </summary>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag"></param>
		/// <param name="recordUploadParam"></param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
		/// <param name="uploadParam">上传参数</param>
		/// <param name="isMulti">是否多人关卡</param>
		/// <param name="netData">联机信息</param>
        public static void CreateProject (
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            Msg_RecordUploadParam recordUploadParam,
            int timeLimit,
            int winCondition,
            Msg_ProjectUploadParam uploadParam,
            bool isMulti,
            Msg_NetBattleData netData,
            Action<Msg_SC_CMD_CreateProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCreateProject) {
                return;
            }
            _isRequstingCreateProject = true;
            Msg_CS_CMD_CreateProject msg = new Msg_CS_CMD_CreateProject();
            // 创建关卡
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUploadParam = recordUploadParam;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            msg.UploadParam = uploadParam;
            msg.IsMulti = isMulti;
            msg.NetData = netData;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CreateProject>(
                SoyHttpApiPath.CreateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCreateProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CreateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCreateProject = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateProject {
            get { return _isRequstingUpdateProject; }
        }
        private static bool _isRequstingUpdateProject = false;
        /// <summary>
		/// 更新关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="name"></param>
		/// <param name="summary"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag"></param>
		/// <param name="recordUploadParam"></param>
		/// <param name="timeLimit">时间限制</param>
		/// <param name="winCondition">胜利条件</param>
		/// <param name="uploadParam">上传参数</param>
		/// <param name="netData">联机信息</param>
        public static void UpdateProject (
            long projectId,
            string name,
            string summary,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            Msg_RecordUploadParam recordUploadParam,
            int timeLimit,
            int winCondition,
            Msg_ProjectUploadParam uploadParam,
            Msg_NetBattleData netData,
            Action<Msg_SC_CMD_UpdateProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateProject) {
                return;
            }
            _isRequstingUpdateProject = true;
            Msg_CS_CMD_UpdateProject msg = new Msg_CS_CMD_UpdateProject();
            // 更新关卡
            msg.ProjectId = projectId;
            msg.Name = name;
            msg.Summary = summary;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUploadParam = recordUploadParam;
            msg.TimeLimit = timeLimit;
            msg.WinCondition = winCondition;
            msg.UploadParam = uploadParam;
            msg.NetData = netData;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateProject>(
                SoyHttpApiPath.UpdateProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateProject = false;
                },
                form
            );
        }

        public static bool IsRequstingEditProject {
            get { return _isRequstingEditProject; }
        }
        private static bool _isRequstingEditProject = false;
        /// <summary>
		/// 继续编辑关卡
		/// </summary>
		/// <param name="projectMainId">关卡Id</param>
        public static void EditProject (
            long projectMainId,
            Action<Msg_SC_CMD_EditProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingEditProject) {
                return;
            }
            _isRequstingEditProject = true;
            Msg_CS_CMD_EditProject msg = new Msg_CS_CMD_EditProject();
            // 继续编辑关卡
            msg.ProjectMainId = projectMainId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_EditProject>(
                SoyHttpApiPath.EditProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingEditProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "EditProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingEditProject = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteProject {
            get { return _isRequstingDeleteProject; }
        }
        private static bool _isRequstingDeleteProject = false;
        /// <summary>
		/// 删除关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void DeleteProject (
            List<long> projectId,
            Action<Msg_SC_CMD_DeleteProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteProject) {
                return;
            }
            _isRequstingDeleteProject = true;
            Msg_CS_CMD_DeleteProject msg = new Msg_CS_CMD_DeleteProject();
            // 删除关卡
            msg.ProjectId.AddRange(projectId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteProject>(
                SoyHttpApiPath.DeleteProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteProject = false;
                },
                form
            );
        }

        public static bool IsRequstingCreateUnitPreinstall {
            get { return _isRequstingCreateUnitPreinstall; }
        }
        private static bool _isRequstingCreateUnitPreinstall = false;
        /// <summary>
		/// 创建预设
		/// </summary>
		/// <param name="preinstall">预设数据</param>
        public static void CreateUnitPreinstall (
            Msg_Preinstall preinstall,
            Action<Msg_SC_CMD_CreateUnitPreinstall> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCreateUnitPreinstall) {
                return;
            }
            _isRequstingCreateUnitPreinstall = true;
            Msg_CS_CMD_CreateUnitPreinstall msg = new Msg_CS_CMD_CreateUnitPreinstall();
            // 创建预设
            msg.Preinstall = preinstall;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CreateUnitPreinstall>(
                SoyHttpApiPath.CreateUnitPreinstall, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCreateUnitPreinstall = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CreateUnitPreinstall", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCreateUnitPreinstall = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateUnitPreinstall {
            get { return _isRequstingUpdateUnitPreinstall; }
        }
        private static bool _isRequstingUpdateUnitPreinstall = false;
        /// <summary>
		/// 更新预设
		/// </summary>
		/// <param name="preinstallId">预设Id</param>
		/// <param name="preinstall">预设数据</param>
        public static void UpdateUnitPreinstall (
            long preinstallId,
            Msg_Preinstall preinstall,
            Action<Msg_SC_CMD_UpdateUnitPreinstall> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateUnitPreinstall) {
                return;
            }
            _isRequstingUpdateUnitPreinstall = true;
            Msg_CS_CMD_UpdateUnitPreinstall msg = new Msg_CS_CMD_UpdateUnitPreinstall();
            // 更新预设
            msg.PreinstallId = preinstallId;
            msg.Preinstall = preinstall;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateUnitPreinstall>(
                SoyHttpApiPath.UpdateUnitPreinstall, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateUnitPreinstall = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateUnitPreinstall", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateUnitPreinstall = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteUnitPreinstall {
            get { return _isRequstingDeleteUnitPreinstall; }
        }
        private static bool _isRequstingDeleteUnitPreinstall = false;
        /// <summary>
		/// 删除预设
		/// </summary>
		/// <param name="preinstallId">预设Id</param>
        public static void DeleteUnitPreinstall (
            List<long> preinstallId,
            Action<Msg_SC_CMD_DeleteUnitPreinstall> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteUnitPreinstall) {
                return;
            }
            _isRequstingDeleteUnitPreinstall = true;
            Msg_CS_CMD_DeleteUnitPreinstall msg = new Msg_CS_CMD_DeleteUnitPreinstall();
            // 删除预设
            msg.PreinstallId.AddRange(preinstallId);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteUnitPreinstall>(
                SoyHttpApiPath.DeleteUnitPreinstall, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteUnitPreinstall = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteUnitPreinstall", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteUnitPreinstall = false;
                },
                form
            );
        }

        public static bool IsRequstingUseProps {
            get { return _isRequstingUseProps; }
        }
        private static bool _isRequstingUseProps = false;
        /// <summary>
		/// 使用道具
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="itemTypes"></param>
        public static void UseProps (
            long token,
            List<int> itemTypes,
            Action<Msg_SC_CMD_UseProps> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUseProps) {
                return;
            }
            _isRequstingUseProps = true;
            Msg_CS_CMD_UseProps msg = new Msg_CS_CMD_UseProps();
            // 使用道具
            msg.Token = token;
            msg.ItemTypes.AddRange(itemTypes);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UseProps>(
                SoyHttpApiPath.UseProps, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUseProps = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UseProps", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUseProps = false;
                },
                form
            );
        }

        public static bool IsRequstingUnlockHomePart {
            get { return _isRequstingUnlockHomePart; }
        }
        private static bool _isRequstingUnlockHomePart = false;
        /// <summary>
		/// 解锁装饰
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="id"></param>
        public static void UnlockHomePart (
            EHomePart type,
            long id,
            Action<Msg_SC_CMD_UnlockHomePart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnlockHomePart) {
                return;
            }
            _isRequstingUnlockHomePart = true;
            Msg_CS_CMD_UnlockHomePart msg = new Msg_CS_CMD_UnlockHomePart();
            // 解锁装饰
            msg.Type = type;
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockHomePart>(
                SoyHttpApiPath.UnlockHomePart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnlockHomePart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockHomePart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnlockHomePart = false;
                },
                form
            );
        }

        public static bool IsRequstingChangeAvatarPart {
            get { return _isRequstingChangeAvatarPart; }
        }
        private static bool _isRequstingChangeAvatarPart = false;
        /// <summary>
		/// 角色换装
		/// </summary>
		/// <param name="type">部位</param>
		/// <param name="newId"></param>
        public static void ChangeAvatarPart (
            EAvatarPart type,
            long newId,
            Action<Msg_SC_CMD_ChangeAvatarPart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingChangeAvatarPart) {
                return;
            }
            _isRequstingChangeAvatarPart = true;
            Msg_CS_CMD_ChangeAvatarPart msg = new Msg_CS_CMD_ChangeAvatarPart();
            // 角色换装
            msg.Type = type;
            msg.NewId = newId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangeAvatarPart>(
                SoyHttpApiPath.ChangeAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingChangeAvatarPart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ChangeAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingChangeAvatarPart = false;
                },
                form
            );
        }

        public static bool IsRequstingBuyAvatarPart {
            get { return _isRequstingBuyAvatarPart; }
        }
        private static bool _isRequstingBuyAvatarPart = false;
        /// <summary>
		/// 购买时装
		/// </summary>
		/// <param name="buyList">购买的数据列表</param>
		/// <param name="putOn">购买成功是否立即穿上</param>
        public static void BuyAvatarPart (
            List<Msg_BuyAvatarPartItem> buyList,
            bool putOn,
            Action<Msg_SC_CMD_BuyAvatarPart> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingBuyAvatarPart) {
                return;
            }
            _isRequstingBuyAvatarPart = true;
            Msg_CS_CMD_BuyAvatarPart msg = new Msg_CS_CMD_BuyAvatarPart();
            // 购买时装
            msg.BuyList.AddRange(buyList);
            msg.PutOn = putOn;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_BuyAvatarPart>(
                SoyHttpApiPath.BuyAvatarPart, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingBuyAvatarPart = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "BuyAvatarPart", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingBuyAvatarPart = false;
                },
                form
            );
        }

        public static bool IsRequstingRaffle {
            get { return _isRequstingRaffle; }
        }
        private static bool _isRequstingRaffle = false;
        /// <summary>
		/// 转盘抽奖
		/// </summary>
		/// <param name="id">抽奖券Id</param>
        public static void Raffle (
            long id,
            Action<Msg_SC_CMD_Raffle> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingRaffle) {
                return;
            }
            _isRequstingRaffle = true;
            Msg_CS_CMD_Raffle msg = new Msg_CS_CMD_Raffle();
            // 转盘抽奖
            msg.Id = id;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Raffle>(
                SoyHttpApiPath.Raffle, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingRaffle = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Raffle", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingRaffle = false;
                },
                form
            );
        }

        public static bool IsRequstingChangePictureFull {
            get { return _isRequstingChangePictureFull; }
        }
        private static bool _isRequstingChangePictureFull = false;
        /// <summary>
		/// 装备拼图
		/// </summary>
		/// <param name="slot">槽位</param>
		/// <param name="putOnId">装备上的拼图Id</param>
		/// <param name="putOffId">卸下的拼图Id</param>
        public static void ChangePictureFull (
            int slot,
            long putOnId,
            long putOffId,
            Action<Msg_SC_CMD_ChangePictureFull> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingChangePictureFull) {
                return;
            }
            _isRequstingChangePictureFull = true;
            Msg_CS_CMD_ChangePictureFull msg = new Msg_CS_CMD_ChangePictureFull();
            // 装备拼图
            msg.Slot = slot;
            msg.PutOnId = putOnId;
            msg.PutOffId = putOffId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ChangePictureFull>(
                SoyHttpApiPath.ChangePictureFull, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingChangePictureFull = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ChangePictureFull", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingChangePictureFull = false;
                },
                form
            );
        }

        public static bool IsRequstingCompoundPictureFull {
            get { return _isRequstingCompoundPictureFull; }
        }
        private static bool _isRequstingCompoundPictureFull = false;
        /// <summary>
		/// 合成拼图
		/// </summary>
		/// <param name="pictureId">拼图Id</param>
        public static void CompoundPictureFull (
            long pictureId,
            Action<Msg_SC_CMD_CompoundPictureFull> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCompoundPictureFull) {
                return;
            }
            _isRequstingCompoundPictureFull = true;
            Msg_CS_CMD_CompoundPictureFull msg = new Msg_CS_CMD_CompoundPictureFull();
            // 合成拼图
            msg.PictureId = pictureId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CompoundPictureFull>(
                SoyHttpApiPath.CompoundPictureFull, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCompoundPictureFull = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CompoundPictureFull", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCompoundPictureFull = false;
                },
                form
            );
        }

        public static bool IsRequstingUpgradePictureFull {
            get { return _isRequstingUpgradePictureFull; }
        }
        private static bool _isRequstingUpgradePictureFull = false;
        /// <summary>
		/// 升级拼图
		/// </summary>
		/// <param name="pictureId">拼图Id</param>
		/// <param name="targetLevel">目标等级</param>
        public static void UpgradePictureFull (
            long pictureId,
            int targetLevel,
            Action<Msg_SC_CMD_UpgradePictureFull> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpgradePictureFull) {
                return;
            }
            _isRequstingUpgradePictureFull = true;
            Msg_CS_CMD_UpgradePictureFull msg = new Msg_CS_CMD_UpgradePictureFull();
            // 升级拼图
            msg.PictureId = pictureId;
            msg.TargetLevel = targetLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpgradePictureFull>(
                SoyHttpApiPath.UpgradePictureFull, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpgradePictureFull = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpgradePictureFull", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpgradePictureFull = false;
                },
                form
            );
        }

        public static bool IsRequstingCompoundWeapon {
            get { return _isRequstingCompoundWeapon; }
        }
        private static bool _isRequstingCompoundWeapon = false;
        /// <summary>
		/// 合成武器
		/// </summary>
		/// <param name="weaponId">武器Id</param>
		/// <param name="universalCount">使用万能碎片数量</param>
        public static void CompoundWeapon (
            long weaponId,
            int universalCount,
            Action<Msg_SC_CMD_CompoundWeapon> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCompoundWeapon) {
                return;
            }
            _isRequstingCompoundWeapon = true;
            Msg_CS_CMD_CompoundWeapon msg = new Msg_CS_CMD_CompoundWeapon();
            // 合成武器
            msg.WeaponId = weaponId;
            msg.UniversalCount = universalCount;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CompoundWeapon>(
                SoyHttpApiPath.CompoundWeapon, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCompoundWeapon = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CompoundWeapon", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCompoundWeapon = false;
                },
                form
            );
        }

        public static bool IsRequstingUpgradeWeapon {
            get { return _isRequstingUpgradeWeapon; }
        }
        private static bool _isRequstingUpgradeWeapon = false;
        /// <summary>
		/// 升级武器
		/// </summary>
		/// <param name="weaponId">武器Id</param>
		/// <param name="universalCount">使用万能碎片数量</param>
		/// <param name="targetLevel">目标等级</param>
        public static void UpgradeWeapon (
            long weaponId,
            int universalCount,
            int targetLevel,
            Action<Msg_SC_CMD_UpgradeWeapon> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpgradeWeapon) {
                return;
            }
            _isRequstingUpgradeWeapon = true;
            Msg_CS_CMD_UpgradeWeapon msg = new Msg_CS_CMD_UpgradeWeapon();
            // 升级武器
            msg.WeaponId = weaponId;
            msg.UniversalCount = universalCount;
            msg.TargetLevel = targetLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpgradeWeapon>(
                SoyHttpApiPath.UpgradeWeapon, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpgradeWeapon = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpgradeWeapon", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpgradeWeapon = false;
                },
                form
            );
        }

        public static bool IsRequstingUpgradeTrainProperty {
            get { return _isRequstingUpgradeTrainProperty; }
        }
        private static bool _isRequstingUpgradeTrainProperty = false;
        /// <summary>
		/// 升级训练属性
		/// </summary>
		/// <param name="property">属性ID</param>
		/// <param name="targetLevel">目标等级</param>
        public static void UpgradeTrainProperty (
            ETrainPropertyType property,
            int targetLevel,
            Action<Msg_SC_CMD_UpgradeTrainProperty> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpgradeTrainProperty) {
                return;
            }
            _isRequstingUpgradeTrainProperty = true;
            Msg_CS_CMD_UpgradeTrainProperty msg = new Msg_CS_CMD_UpgradeTrainProperty();
            // 升级训练属性
            msg.Property = property;
            msg.TargetLevel = targetLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpgradeTrainProperty>(
                SoyHttpApiPath.UpgradeTrainProperty, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpgradeTrainProperty = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpgradeTrainProperty", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpgradeTrainProperty = false;
                },
                form
            );
        }

        public static bool IsRequstingCompleteUpgradeTrainProperty {
            get { return _isRequstingCompleteUpgradeTrainProperty; }
        }
        private static bool _isRequstingCompleteUpgradeTrainProperty = false;
        /// <summary>
		/// 升级训练阶层
		/// </summary>
		/// <param name="property">属性ID</param>
		/// <param name="targetLevel">目标等级</param>
		/// <param name="remainTime">剩余时间</param>
        public static void CompleteUpgradeTrainProperty (
            ETrainPropertyType property,
            int targetLevel,
            long remainTime,
            Action<Msg_SC_CMD_CompleteUpgradeTrainProperty> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCompleteUpgradeTrainProperty) {
                return;
            }
            _isRequstingCompleteUpgradeTrainProperty = true;
            Msg_CS_CMD_CompleteUpgradeTrainProperty msg = new Msg_CS_CMD_CompleteUpgradeTrainProperty();
            // 升级训练阶层
            msg.Property = property;
            msg.TargetLevel = targetLevel;
            msg.RemainTime = remainTime;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CompleteUpgradeTrainProperty>(
                SoyHttpApiPath.CompleteUpgradeTrainProperty, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCompleteUpgradeTrainProperty = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CompleteUpgradeTrainProperty", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCompleteUpgradeTrainProperty = false;
                },
                form
            );
        }

        public static bool IsRequstingUpgradeTrainGrade {
            get { return _isRequstingUpgradeTrainGrade; }
        }
        private static bool _isRequstingUpgradeTrainGrade = false;
        /// <summary>
		/// 升级训练阶层
		/// </summary>
		/// <param name="targetGrade">目标阶层</param>
        public static void UpgradeTrainGrade (
            int targetGrade,
            Action<Msg_SC_CMD_UpgradeTrainGrade> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpgradeTrainGrade) {
                return;
            }
            _isRequstingUpgradeTrainGrade = true;
            Msg_CS_CMD_UpgradeTrainGrade msg = new Msg_CS_CMD_UpgradeTrainGrade();
            // 升级训练阶层
            msg.TargetGrade = targetGrade;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpgradeTrainGrade>(
                SoyHttpApiPath.UpgradeTrainGrade, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpgradeTrainGrade = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpgradeTrainGrade", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpgradeTrainGrade = false;
                },
                form
            );
        }

        public static bool IsRequstingReceiveQQGameReward {
            get { return _isRequstingReceiveQQGameReward; }
        }
        private static bool _isRequstingReceiveQQGameReward = false;
        /// <summary>
		/// QQ蓝钻大厅特权奖励领取
		/// </summary>
		/// <param name="type">特权类型</param>
		/// <param name="subType">特权子类型</param>
		/// <param name="inx">下标</param>
		/// <param name="blueVipType">蓝钻类型</param>
        public static void ReceiveQQGameReward (
            EQQGamePrivilegeType type,
            EQQGamePrivilegeSubType subType,
            int inx,
            EQQGameBlueVipType blueVipType,
            Action<Msg_SC_CMD_ReceiveQQGameReward> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReceiveQQGameReward) {
                return;
            }
            _isRequstingReceiveQQGameReward = true;
            Msg_CS_CMD_ReceiveQQGameReward msg = new Msg_CS_CMD_ReceiveQQGameReward();
            // QQ蓝钻大厅特权奖励领取
            msg.Type = type;
            msg.SubType = subType;
            msg.Inx = inx;
            msg.BlueVipType = blueVipType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReceiveQQGameReward>(
                SoyHttpApiPath.ReceiveQQGameReward, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReceiveQQGameReward = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReceiveQQGameReward", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReceiveQQGameReward = false;
                },
                form
            );
        }

        public static bool IsRequstingPublishUserMessage {
            get { return _isRequstingPublishUserMessage; }
        }
        private static bool _isRequstingPublishUserMessage = false;
        /// <summary>
		/// 发布留言
		/// </summary>
		/// <param name="userId">用户ID</param>
		/// <param name="content">内容</param>
        public static void PublishUserMessage (
            long userId,
            string content,
            Action<Msg_SC_CMD_PublishUserMessage> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPublishUserMessage) {
                return;
            }
            _isRequstingPublishUserMessage = true;
            Msg_CS_CMD_PublishUserMessage msg = new Msg_CS_CMD_PublishUserMessage();
            // 发布留言
            msg.UserId = userId;
            msg.Content = content;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishUserMessage>(
                SoyHttpApiPath.PublishUserMessage, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPublishUserMessage = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishUserMessage", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPublishUserMessage = false;
                },
                form
            );
        }

        public static bool IsRequstingReplyUserMessage {
            get { return _isRequstingReplyUserMessage; }
        }
        private static bool _isRequstingReplyUserMessage = false;
        /// <summary>
		/// 回复留言
		/// </summary>
		/// <param name="userMessageId">留言ID</param>
		/// <param name="content">内容</param>
		/// <param name="targetReplyId">回复的回复ID</param>
        public static void ReplyUserMessage (
            long userMessageId,
            string content,
            long targetReplyId,
            Action<Msg_SC_CMD_ReplyUserMessage> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReplyUserMessage) {
                return;
            }
            _isRequstingReplyUserMessage = true;
            Msg_CS_CMD_ReplyUserMessage msg = new Msg_CS_CMD_ReplyUserMessage();
            // 回复留言
            msg.UserMessageId = userMessageId;
            msg.Content = content;
            msg.TargetReplyId = targetReplyId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReplyUserMessage>(
                SoyHttpApiPath.ReplyUserMessage, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReplyUserMessage = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReplyUserMessage", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReplyUserMessage = false;
                },
                form
            );
        }

        public static bool IsRequstingUpdateUserMessageLike {
            get { return _isRequstingUpdateUserMessageLike; }
        }
        private static bool _isRequstingUpdateUserMessageLike = false;
        /// <summary>
		/// 回复留言
		/// </summary>
		/// <param name="userMessageId">留言ID</param>
		/// <param name="likeFlag">赞</param>
        public static void UpdateUserMessageLike (
            long userMessageId,
            bool likeFlag,
            Action<Msg_SC_CMD_UpdateUserMessageLike> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUpdateUserMessageLike) {
                return;
            }
            _isRequstingUpdateUserMessageLike = true;
            Msg_CS_CMD_UpdateUserMessageLike msg = new Msg_CS_CMD_UpdateUserMessageLike();
            // 回复留言
            msg.UserMessageId = userMessageId;
            msg.LikeFlag = likeFlag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UpdateUserMessageLike>(
                SoyHttpApiPath.UpdateUserMessageLike, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUpdateUserMessageLike = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UpdateUserMessageLike", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUpdateUserMessageLike = false;
                },
                form
            );
        }

        public static bool IsRequstingBuyEnergy {
            get { return _isRequstingBuyEnergy; }
        }
        private static bool _isRequstingBuyEnergy = false;
        /// <summary>
		/// 购买体力
		/// </summary>
		/// <param name="energy">购买数量</param>
        public static void BuyEnergy (
            int energy,
            Action<Msg_SC_CMD_BuyEnergy> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingBuyEnergy) {
                return;
            }
            _isRequstingBuyEnergy = true;
            Msg_CS_CMD_BuyEnergy msg = new Msg_CS_CMD_BuyEnergy();
            // 购买体力
            msg.Energy = energy;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_BuyEnergy>(
                SoyHttpApiPath.BuyEnergy, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingBuyEnergy = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "BuyEnergy", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingBuyEnergy = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayAdventureLevel {
            get { return _isRequstingPlayAdventureLevel; }
        }
        private static bool _isRequstingPlayAdventureLevel = false;
        /// <summary>
		/// 进入冒险关卡
		/// </summary>
		/// <param name="section">章节id</param>
		/// <param name="projectType">关卡类型</param>
		/// <param name="level">关卡id</param>
        public static void PlayAdventureLevel (
            int section,
            EAdventureProjectType projectType,
            int level,
            Action<Msg_SC_CMD_PlayAdventureLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayAdventureLevel) {
                return;
            }
            _isRequstingPlayAdventureLevel = true;
            Msg_CS_CMD_PlayAdventureLevel msg = new Msg_CS_CMD_PlayAdventureLevel();
            // 进入冒险关卡
            msg.Section = section;
            msg.ProjectType = projectType;
            msg.Level = level;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayAdventureLevel>(
                SoyHttpApiPath.PlayAdventureLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayAdventureLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayAdventureLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayAdventureLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingUnlockAdventureSection {
            get { return _isRequstingUnlockAdventureSection; }
        }
        private static bool _isRequstingUnlockAdventureSection = false;
        /// <summary>
		/// 解锁章节
		/// </summary>
		/// <param name="section">章节</param>
        public static void UnlockAdventureSection (
            int section,
            Action<Msg_SC_CMD_UnlockAdventureSection> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingUnlockAdventureSection) {
                return;
            }
            _isRequstingUnlockAdventureSection = true;
            Msg_CS_CMD_UnlockAdventureSection msg = new Msg_CS_CMD_UnlockAdventureSection();
            // 解锁章节
            msg.Section = section;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_UnlockAdventureSection>(
                SoyHttpApiPath.UnlockAdventureSection, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingUnlockAdventureSection = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "UnlockAdventureSection", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingUnlockAdventureSection = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitAdventureLevelResult {
            get { return _isRequstingCommitAdventureLevelResult; }
        }
        private static bool _isRequstingCommitAdventureLevelResult = false;
        /// <summary>
		/// 提交冒险模式数据
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="recordUploadParam">录像上传参数</param>
        public static void CommitAdventureLevelResult (
            long token,
            Msg_RecordUploadParam recordUploadParam,
            Action<Msg_SC_CMD_CommitAdventureLevelResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitAdventureLevelResult) {
                return;
            }
            _isRequstingCommitAdventureLevelResult = true;
            Msg_CS_CMD_CommitAdventureLevelResult msg = new Msg_CS_CMD_CommitAdventureLevelResult();
            // 提交冒险模式数据
            msg.Token = token;
            msg.RecordUploadParam = recordUploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitAdventureLevelResult>(
                SoyHttpApiPath.CommitAdventureLevelResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitAdventureLevelResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitAdventureLevelResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitAdventureLevelResult = false;
                },
                form
            );
        }

        public static bool IsRequstingReform {
            get { return _isRequstingReform; }
        }
        private static bool _isRequstingReform = false;
        /// <summary>
		/// 改造
		/// </summary>
		/// <param name="flag">占位</param>
        public static void Reform (
            int flag,
            Action<Msg_SC_CMD_Reform> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReform) {
                return;
            }
            _isRequstingReform = true;
            Msg_CS_CMD_Reform msg = new Msg_CS_CMD_Reform();
            // 改造
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Reform>(
                SoyHttpApiPath.Reform, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReform = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "Reform", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReform = false;
                },
                form
            );
        }

        public static bool IsRequstingReselectReformLevel {
            get { return _isRequstingReselectReformLevel; }
        }
        private static bool _isRequstingReselectReformLevel = false;
        /// <summary>
		/// 随机改造关卡
		/// </summary>
		/// <param name="curReformSection">当前改造关卡所属章节</param>
		/// <param name="curReformLevel">当前改造关卡所属关卡</param>
        public static void ReselectReformLevel (
            int curReformSection,
            int curReformLevel,
            Action<Msg_SC_CMD_ReselectReformLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReselectReformLevel) {
                return;
            }
            _isRequstingReselectReformLevel = true;
            Msg_CS_CMD_ReselectReformLevel msg = new Msg_CS_CMD_ReselectReformLevel();
            // 随机改造关卡
            msg.CurReformSection = curReformSection;
            msg.CurReformLevel = curReformLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReselectReformLevel>(
                SoyHttpApiPath.ReselectReformLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReselectReformLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReselectReformLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReselectReformLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingGetReformReward {
            get { return _isRequstingGetReformReward; }
        }
        private static bool _isRequstingGetReformReward = false;
        /// <summary>
		/// 领取改造奖励
		/// </summary>
		/// <param name="rewardLevel">改造奖励级别</param>
        public static void GetReformReward (
            int rewardLevel,
            Action<Msg_SC_CMD_GetReformReward> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetReformReward) {
                return;
            }
            _isRequstingGetReformReward = true;
            Msg_CS_CMD_GetReformReward msg = new Msg_CS_CMD_GetReformReward();
            // 领取改造奖励
            msg.RewardLevel = rewardLevel;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetReformReward>(
                SoyHttpApiPath.GetReformReward, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetReformReward = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetReformReward", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetReformReward = false;
                },
                form
            );
        }

        public static bool IsRequstingSaveReformProject {
            get { return _isRequstingSaveReformProject; }
        }
        private static bool _isRequstingSaveReformProject = false;
        /// <summary>
		/// 上传改造关卡
		/// </summary>
		/// <param name="projectId">关卡Id</param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="passFlag">是否已经通过</param>
		/// <param name="recordUploadParam">录像上传参数</param>
		/// <param name="uploadParam">上传参数</param>
        public static void SaveReformProject (
            long projectId,
            int programVersion,
            int resourceVersion,
            bool passFlag,
            Msg_RecordUploadParam recordUploadParam,
            Msg_ProjectUploadParam uploadParam,
            Action<Msg_SC_CMD_SaveReformProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSaveReformProject) {
                return;
            }
            _isRequstingSaveReformProject = true;
            Msg_CS_CMD_SaveReformProject msg = new Msg_CS_CMD_SaveReformProject();
            // 上传改造关卡
            msg.ProjectId = projectId;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.PassFlag = passFlag;
            msg.RecordUploadParam = recordUploadParam;
            msg.UploadParam = uploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SaveReformProject>(
                SoyHttpApiPath.SaveReformProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSaveReformProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SaveReformProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSaveReformProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPublishReformProject {
            get { return _isRequstingPublishReformProject; }
        }
        private static bool _isRequstingPublishReformProject = false;
        /// <summary>
		/// 发布改造关卡
		/// </summary>
		/// <param name="personalProjectId"></param>
		/// <param name="programVersion"></param>
		/// <param name="resourceVersion"></param>
		/// <param name="recordUploadParam">录像上传参数</param>
		/// <param name="uploadParam">上传参数</param>
        public static void PublishReformProject (
            long personalProjectId,
            int programVersion,
            int resourceVersion,
            Msg_RecordUploadParam recordUploadParam,
            Msg_ProjectUploadParam uploadParam,
            Action<Msg_SC_CMD_PublishReformProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPublishReformProject) {
                return;
            }
            _isRequstingPublishReformProject = true;
            Msg_CS_CMD_PublishReformProject msg = new Msg_CS_CMD_PublishReformProject();
            // 发布改造关卡
            msg.PersonalProjectId = personalProjectId;
            msg.ProgramVersion = programVersion;
            msg.ResourceVersion = resourceVersion;
            msg.RecordUploadParam = recordUploadParam;
            msg.UploadParam = uploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PublishReformProject>(
                SoyHttpApiPath.PublishReformProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPublishReformProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PublishReformProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPublishReformProject = false;
                },
                form
            );
        }

        public static bool IsRequstingGetMatchChallengeProject {
            get { return _isRequstingGetMatchChallengeProject; }
        }
        private static bool _isRequstingGetMatchChallengeProject = false;
        /// <summary>
		/// 获取挑战关卡
		/// </summary>
		/// <param name="flag">占位符</param>
        public static void GetMatchChallengeProject (
            int flag,
            Action<Msg_SC_CMD_GetMatchChallengeProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingGetMatchChallengeProject) {
                return;
            }
            _isRequstingGetMatchChallengeProject = true;
            Msg_CS_CMD_GetMatchChallengeProject msg = new Msg_CS_CMD_GetMatchChallengeProject();
            // 获取挑战关卡
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_GetMatchChallengeProject>(
                SoyHttpApiPath.GetMatchChallengeProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingGetMatchChallengeProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "GetMatchChallengeProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingGetMatchChallengeProject = false;
                },
                form
            );
        }

        public static bool IsRequstingSelectMatchChallengeProject {
            get { return _isRequstingSelectMatchChallengeProject; }
        }
        private static bool _isRequstingSelectMatchChallengeProject = false;
        /// <summary>
		/// 选取挑战关卡
		/// </summary>
		/// <param name="challengeType">选择类型</param>
		/// <param name="change">花钱切换</param>
        public static void SelectMatchChallengeProject (
            EChallengeProjectType challengeType,
            bool change,
            Action<Msg_SC_CMD_SelectMatchChallengeProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSelectMatchChallengeProject) {
                return;
            }
            _isRequstingSelectMatchChallengeProject = true;
            Msg_CS_CMD_SelectMatchChallengeProject msg = new Msg_CS_CMD_SelectMatchChallengeProject();
            // 选取挑战关卡
            msg.ChallengeType = challengeType;
            msg.Change = change;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SelectMatchChallengeProject>(
                SoyHttpApiPath.SelectMatchChallengeProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSelectMatchChallengeProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SelectMatchChallengeProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSelectMatchChallengeProject = false;
                },
                form
            );
        }

        public static bool IsRequstingPlayMatchChallengeLevel {
            get { return _isRequstingPlayMatchChallengeLevel; }
        }
        private static bool _isRequstingPlayMatchChallengeLevel = false;
        /// <summary>
		/// 开始挑战
		/// </summary>
		/// <param name="projectId">关卡Id</param>
        public static void PlayMatchChallengeLevel (
            long projectId,
            Action<Msg_SC_CMD_PlayMatchChallengeLevel> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingPlayMatchChallengeLevel) {
                return;
            }
            _isRequstingPlayMatchChallengeLevel = true;
            Msg_CS_CMD_PlayMatchChallengeLevel msg = new Msg_CS_CMD_PlayMatchChallengeLevel();
            // 开始挑战
            msg.ProjectId = projectId;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_PlayMatchChallengeLevel>(
                SoyHttpApiPath.PlayMatchChallengeLevel, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingPlayMatchChallengeLevel = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "PlayMatchChallengeLevel", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingPlayMatchChallengeLevel = false;
                },
                form
            );
        }

        public static bool IsRequstingCommitMatchChallengeLevelResult {
            get { return _isRequstingCommitMatchChallengeLevelResult; }
        }
        private static bool _isRequstingCommitMatchChallengeLevelResult = false;
        /// <summary>
		/// 提交匹配挑战关卡数据
		/// </summary>
		/// <param name="token">令牌</param>
		/// <param name="recordUploadParam">录像上传参数</param>
        public static void CommitMatchChallengeLevelResult (
            long token,
            Msg_RecordUploadParam recordUploadParam,
            Action<Msg_SC_CMD_CommitMatchChallengeLevelResult> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingCommitMatchChallengeLevelResult) {
                return;
            }
            _isRequstingCommitMatchChallengeLevelResult = true;
            Msg_CS_CMD_CommitMatchChallengeLevelResult msg = new Msg_CS_CMD_CommitMatchChallengeLevelResult();
            // 提交匹配挑战关卡数据
            msg.Token = token;
            msg.RecordUploadParam = recordUploadParam;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_CommitMatchChallengeLevelResult>(
                SoyHttpApiPath.CommitMatchChallengeLevelResult, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingCommitMatchChallengeLevelResult = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "CommitMatchChallengeLevelResult", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingCommitMatchChallengeLevelResult = false;
                },
                form
            );
        }

        public static bool IsRequstingMatchSkipChallenge {
            get { return _isRequstingMatchSkipChallenge; }
        }
        private static bool _isRequstingMatchSkipChallenge = false;
        /// <summary>
		/// 跳过本次挑战
		/// </summary>
		/// <param name="flag">占位</param>
        public static void MatchSkipChallenge (
            int flag,
            Action<Msg_SC_CMD_MatchSkipChallenge> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingMatchSkipChallenge) {
                return;
            }
            _isRequstingMatchSkipChallenge = true;
            Msg_CS_CMD_MatchSkipChallenge msg = new Msg_CS_CMD_MatchSkipChallenge();
            // 跳过本次挑战
            msg.Flag = flag;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_MatchSkipChallenge>(
                SoyHttpApiPath.MatchSkipChallenge, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingMatchSkipChallenge = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "MatchSkipChallenge", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingMatchSkipChallenge = false;
                },
                form
            );
        }

        public static bool IsRequstingSendMail {
            get { return _isRequstingSendMail; }
        }
        private static bool _isRequstingSendMail = false;
        /// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="targetUserId">用户</param>
		/// <param name="title"></param>
		/// <param name="content"></param>
        public static void SendMail (
            long targetUserId,
            string title,
            string content,
            Action<Msg_SC_CMD_SendMail> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingSendMail) {
                return;
            }
            _isRequstingSendMail = true;
            Msg_CS_CMD_SendMail msg = new Msg_CS_CMD_SendMail();
            // 发送邮件
            msg.TargetUserId = targetUserId;
            msg.Title = title;
            msg.Content = content;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_SendMail>(
                SoyHttpApiPath.SendMail, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingSendMail = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "SendMail", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingSendMail = false;
                },
                form
            );
        }

        public static bool IsRequstingMarkMailRead {
            get { return _isRequstingMarkMailRead; }
        }
        private static bool _isRequstingMarkMailRead = false;
        /// <summary>
		/// 标记已读
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="idList"></param>
		/// <param name="mailType">邮件类型</param>
        public static void MarkMailRead (
            EMarkMailReadTargetType targetType,
            List<long> idList,
            EMailType mailType,
            Action<Msg_SC_CMD_MarkMailRead> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingMarkMailRead) {
                return;
            }
            _isRequstingMarkMailRead = true;
            Msg_CS_CMD_MarkMailRead msg = new Msg_CS_CMD_MarkMailRead();
            // 标记已读
            msg.TargetType = targetType;
            msg.IdList.AddRange(idList);
            msg.MailType = mailType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_MarkMailRead>(
                SoyHttpApiPath.MarkMailRead, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingMarkMailRead = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "MarkMailRead", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingMarkMailRead = false;
                },
                form
            );
        }

        public static bool IsRequstingReceiptMailAttach {
            get { return _isRequstingReceiptMailAttach; }
        }
        private static bool _isRequstingReceiptMailAttach = false;
        /// <summary>
		/// 领取附件
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="idList"></param>
		/// <param name="mailType">邮件类型</param>
        public static void ReceiptMailAttach (
            EReceiptMailAttachTargetType targetType,
            List<long> idList,
            EMailType mailType,
            Action<Msg_SC_CMD_ReceiptMailAttach> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingReceiptMailAttach) {
                return;
            }
            _isRequstingReceiptMailAttach = true;
            Msg_CS_CMD_ReceiptMailAttach msg = new Msg_CS_CMD_ReceiptMailAttach();
            // 领取附件
            msg.TargetType = targetType;
            msg.IdList.AddRange(idList);
            msg.MailType = mailType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ReceiptMailAttach>(
                SoyHttpApiPath.ReceiptMailAttach, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingReceiptMailAttach = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ReceiptMailAttach", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingReceiptMailAttach = false;
                },
                form
            );
        }

        public static bool IsRequstingDeleteMail {
            get { return _isRequstingDeleteMail; }
        }
        private static bool _isRequstingDeleteMail = false;
        /// <summary>
		/// 领取附件
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="idList"></param>
		/// <param name="mailType">邮件类型</param>
        public static void DeleteMail (
            EDeleteMailTargetType targetType,
            List<long> idList,
            EMailType mailType,
            Action<Msg_SC_CMD_DeleteMail> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingDeleteMail) {
                return;
            }
            _isRequstingDeleteMail = true;
            Msg_CS_CMD_DeleteMail msg = new Msg_CS_CMD_DeleteMail();
            // 领取附件
            msg.TargetType = targetType;
            msg.IdList.AddRange(idList);
            msg.MailType = mailType;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_DeleteMail>(
                SoyHttpApiPath.DeleteMail, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingDeleteMail = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "DeleteMail", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingDeleteMail = false;
                },
                form
            );
        }

        public static bool IsRequstingShareProject {
            get { return _isRequstingShareProject; }
        }
        private static bool _isRequstingShareProject = false;
        /// <summary>
		/// 分享关卡
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="userIdList"></param>
        public static void ShareProject (
            long projectId,
            List<long> userIdList,
            Action<Msg_SC_CMD_ShareProject> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingShareProject) {
                return;
            }
            _isRequstingShareProject = true;
            Msg_CS_CMD_ShareProject msg = new Msg_CS_CMD_ShareProject();
            // 分享关卡
            msg.ProjectId = projectId;
            msg.UserIdList.AddRange(userIdList);
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ShareProject>(
                SoyHttpApiPath.ShareProject, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingShareProject = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ShareProject", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingShareProject = false;
                },
                form
            );
        }

        public static bool IsRequstingExecuteCommand {
            get { return _isRequstingExecuteCommand; }
        }
        private static bool _isRequstingExecuteCommand = false;
        /// <summary>
		/// 执行GM指令
		/// </summary>
		/// <param name="userId">用户</param>
		/// <param name="command"></param>
        public static void ExecuteCommand (
            long userId,
            string command,
            Action<Msg_SC_CMD_ExecuteCommand> successCallback, Action<ENetResultCode> failedCallback,
            UnityEngine.WWWForm form = null) {

            if (_isRequstingExecuteCommand) {
                return;
            }
            _isRequstingExecuteCommand = true;
            Msg_CS_CMD_ExecuteCommand msg = new Msg_CS_CMD_ExecuteCommand();
            // 执行GM指令
            msg.UserId = userId;
            msg.Command = command;
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_ExecuteCommand>(
                SoyHttpApiPath.ExecuteCommand, msg, ret => {
                    if (successCallback != null) {
                        successCallback.Invoke(ret);
                    }
                    _isRequstingExecuteCommand = false;
                }, (failedCode, failedMsg) => {
                    LogHelper.Error("Remote command error, msg: {0}, code: {1}, info: {2}", "ExecuteCommand", failedCode, failedMsg);
                    if (failedCallback != null) {
                        failedCallback.Invoke(failedCode);
                    }
                    _isRequstingExecuteCommand = false;
                },
                form
            );
        }

    }
}