﻿/********************************************************************
** Filename : UICtrlSignup
** Author : Dong
** Date : 2015/4/27 22:20:13
** Summary : UICtrlSignup
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using cn.sharesdk.unity3d;
using SoyEngine.Proto;
using System.Text;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlSignup : UICtrlGenericBase<UIViewSignup>
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.FrontUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUIEvent();
            LoginLogicUtil.Init();
        }

        protected override void OnOpen(object parameter)
        {
            //可能是Text的bug
            //Debug.Log("______cachedView.Phone.characterLimit__________" + _cachedView.Phone.characterLimit);
            int charLimit = _cachedView.Phone.characterLimit;
            _cachedView.Phone.characterLimit = charLimit + 1;
            _cachedView.Phone.text = LoginLogicUtil.PhoneNum;
            _cachedView.Phone.characterLimit = charLimit;

            _cachedView.Pwd.text = "";
            _cachedView.SmsCode.text = "";
            LoginLogicUtil.OnSuccess = OnLoginSuccess;
            LoginLogicUtil.OnFailed = OnLoginFailed;
//            LoginLogicUtil.OnSnsInfoLogin = LoginLogicUtil.RequestLogin;
            LoginLogicUtil.OnSnsInfoCancel = OnSnsInfoCancel;
            base.OnOpen(parameter);
        }

        private IEnumerator ProcessSmsCodeCountDown()
        {
            _cachedView.SmsCode.text = "";
            _cachedView.GetSMSCode.enabled = false;
            Text text =  _cachedView.GetSMSCodeBtnLabel;
            for(int countDown = 60; countDown>0; countDown--)
            {
                text.text = "" + countDown +"秒后可重发";
                yield return new WaitForSeconds(1f);
            }
            text.text = "获取验证码";
            _cachedView.GetSMSCode.enabled = true;
        }

        /// <summary>
        /// 注册功能待完成
        /// </summary>
        private void OnSignup()
        {
            var phone = _cachedView.Phone.text;
            var verificationCode = _cachedView.SmsCode.text;
            var pwd = _cachedView.Pwd.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phone);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if(!phoneCheckResult)
            {
                return;
            }
            bool verificationCheckResult = CheckTools.CheckVerificationCode (verificationCode);
            LoginLogicUtil.ShowVerificationCodeCheckTip(verificationCheckResult);
            //if(!verificationCheckResult)
            //{
            //    return;
            //}
            CheckTools.ECheckPasswordResult pwdCheckResult = CheckTools.CheckPassword(pwd);
            LoginLogicUtil.ShowPasswordCheckTip(pwdCheckResult);
            if (pwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "努力注册中");
            LocalUser.Instance.Account.SignUp(phone, pwd, EAccountIdentifyType.AIT_Phone, verificationCode,
                ret=> {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.GetUI<UICtrlSignup>().Close();
                    SocialApp.Instance.LoginSucceed();
                },
                (ret) =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Error("登录失败, Code: " + ret);
                });
//            LoginLogicUtil.RequestSmsLogin(phone, verificationCode, pwd, EVerifyCodeType.VCT_Register);
            LoginLogicUtil.PhoneNum = _cachedView.Phone.text;
        }

        #endregion

        #region  
        /// <summary>
        /// 验证码功能待完成
        /// </summary>
        private void OnSmsCode()
        {
            string phoneNum = _cachedView.Phone.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phoneNum);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if (!phoneCheckResult)
            {
                return;
            }
            RemoteCommands.GetVerificationCode(phoneNum,EAccountIdentifyType.AIT_Phone,EVerifyCodeType.VCT_Register,
                (ret) => { },null

                );
            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());
            //            string phoneNum = _cachedView.Phone.text;
            //            bool phoneCheckResult = CheckTools.CheckPhoneNum(phoneNum);
            //            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            //            if (!phoneCheckResult)
            //            {
            //                return;
            //            }
            //            Msg_CA_RequestSMSCode req = new Msg_CA_RequestSMSCode();
            //            req.PhoneNum = phoneNum;
            //            req.VerifyCodeType = EVerifyCodeType.VCT_Register;
            //
            //            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_CommonResult>(SoyHttpApiPath.GetSmsCode, req, ret => {
            //                ECommonResultCode c = (ECommonResultCode)ret.Code;
            //                LogHelper.Info("Send VerificationCode Success, Code: {0}, Msg: {1}", c, ret.Msg);
            //            }, (code, msg) => {
            //                ENetResultCode c = (ENetResultCode)code;
            //                LogHelper.Info("Send VerificationCode Error, Code: {0}, Msg: {1}", c, msg);
            //            });
            //            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());
        }

        private void InitUIEvent()
        {
            _cachedView.GetSMSCode.onClick.AddListener(OnGetSMSCodeButtonClick);
            _cachedView.DoSignup.onClick.AddListener(OnDoSignupButtonClick);
            _cachedView.ReturnLogin.onClick.AddListener(ReturnLogin);
            //_cachedView.QQ.onClick.AddListener(OnQQ);
            //_cachedView.Weibo.onClick.AddListener(OnWeibo);
            //_cachedView.WeChat.onClick.AddListener(OnWeChat);
        }

        private void ReturnLogin()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlLogin>();
            Close();
        }

        private void OnLoginSuccess()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            CommonTools.ShowPopupDialog("注册成功");
            Close();
        }

        private void OnLoginFailed()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        }

        #endregion

        #region uievent 

        private void OnGetSMSCodeButtonClick()
        {
            OnSmsCode();
        }

        private void OnDoSignupButtonClick()
        {
            OnSignup();
        }

        private void OnQQ()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            LoginLogicUtil.OnQQ();
        }
        private void OnWeibo()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            LoginLogicUtil.OnWeibo();
        }

        private void OnWeChat()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请稍等");
            LoginLogicUtil.OnWeChat();
        }

        private void OnSnsInfoCancel()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            LoginLogicUtil.ShowSnsLoginCancel();
        }
        #endregion

        public object GetTitle()
        {
            return "注册";
        }
    }
}
