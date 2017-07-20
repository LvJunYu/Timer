﻿/********************************************************************
** Filename : UICtrlLogin
** Author : Dong
** Date : 2015/4/27 18:01:01
** Summary : UICtrlLogin
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using cn.sharesdk.unity3d;
using SoyEngine.Proto;
using System.Text;
using NewResourceSolution;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlLogin : UICtrlGenericBase<UIViewLogin>
    {
        #region 常量与字段

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            LoginLogicUtil.Init();
            _cachedView.Guest.onClick.AddListener(GuestLoginIn);
            _cachedView.Signup.onClick.AddListener(SignUp);
            _cachedView.Login.onClick.AddListener(OnLogin);
            _cachedView.ForgetPasswordBtnRes.onClick.AddListener(ForgetPWD);
            _cachedView.ChangePasswordBtnRes.onClick.AddListener(ChangePWD);
        }

        protected override void OnOpen(object parameter)
        {
//            LoginLogicUtil.OnSuccess = OnLoginSuccess;
//            LoginLogicUtil.OnFailed = OnLoginFailed;
//            LoginLogicUtil.OnSnsInfoLogin = LoginLogicUtil.RequestLogin;
//            LoginLogicUtil.OnSnsInfoCancel = OnSnsInfoCancel;



            base.OnOpen(parameter);
        }

        private void OnLogin()
        {
            var phone = _cachedView.Phone.text;
            var pwd = _cachedView.Pwd.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phone);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if (!phoneCheckResult)
            {
                return;
            }
            CheckTools.ECheckPasswordResult pwdCheckResult = CheckTools.CheckPassword(pwd);
            LoginLogicUtil.ShowPasswordCheckTip(pwdCheckResult);
            if (pwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            Msg_SNSUserInfo msg_SNSUserInfo = new Msg_SNSUserInfo();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "努力登录中");
            LocalUser.Instance.Account.OnLogin(phone, pwd, EAccountIdentifyType.AIT_Phone, null,
                (ret) =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialApp.Instance.LoginSucceed();
                },
                (ret) =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Error("登录失败, Code: " + ret);
                });
            //            Msg_CA_Login msg = new Msg_CA_Login();
            //            msg.ClientVersion = 0;
            //            msg.DeviceId = SystemInfo.deviceUniqueIdentifier;
            //            msg.PhoneNum = phone;
            //            msg.Password = pwd;
            //            msg.LoginType = Msg_CA_Login.ELoginType.LT_Phone;
            //            LoginLogicUtil.RequestLogin(msg);
            //            LoginLogicUtil.PhoneNum = _cachedView.Phone.text;
        }

        #endregion

        #region  private 

        //		private void InitUIEvent()
        //		{
        //			_cachedView.Login.onClick.AddListener(OnLoginButtonClick);
        //			_cachedView.Signup.onClick.AddListener(OnSignupButtonClick);
        //			_cachedView.QQ.onClick.AddListener(OnQQ);
        //			_cachedView.Weibo.onClick.AddListener(OnWeibo);
        //			_cachedView.WeChat.onClick.AddListener(OnWeChat);
        //		}

        //        private void OnLoginSuccess()
        //        {
        //            if(_uiStack != null)
        //            {
        //                _uiStack.Close();
        //            }
        //
        //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        //            CommonTools.ShowPopupDialog("登录成功");
        //        }
        //
        //        private void OnLoginFailed()
        //        {
        //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        //        }

        #endregion

        #region uievent 


        //	    private void OnLoginButtonClick()
        //	    {
        //		    OnLogin();
        //	    }
        //
        //	    private void OnSignupButtonClick()
        //        {
        //            if(!string.IsNullOrEmpty(_cachedView.Phone.text))
        //            {
        //                LoginLogicUtil.PhoneNum = _cachedView.Phone.text;
        //            }
        //            _uiStack.OpenUI<UICtrlSignup>();
        //		}

        private void GuestLoginIn()
        {
            LocalUser.Instance.Account.GuestLoginIn(SocialApp.Instance.LoginSucceed, null);
        }

        private void ForgetPWD()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlForgetPassword>();
        }

        private void ChangePWD()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlChangePassword>();
        }


        private void SignUp()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlSignup>();
        }

        //
            //        private void OnQQ()
            //        {
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            //            LoginLogicUtil.OnQQ();
            //        }
            //        private void OnWeibo()
            //        {
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            //            LoginLogicUtil.OnWeibo();
            //        }
            //
            //        private void OnWeChat()
            //        {
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            //            LoginLogicUtil.OnWeChat();
            //        }
            //
            //        private void OnSnsInfoCancel()
            //        {
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            //            LoginLogicUtil.ShowSnsLoginCancel();
            //        }

            #endregion
        
    }
}