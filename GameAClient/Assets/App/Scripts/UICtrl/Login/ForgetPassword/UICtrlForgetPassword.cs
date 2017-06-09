  /********************************************************************
  ** Filename : UICtrlForgetPassword.cs
  ** Author : quan
  ** Date : 2016/6/24 16:54
  ** Summary : UICtrlForgetPassword.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using cn.sharesdk.unity3d;
using SoyEngine.Proto;
using System.Text;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlForgetPassword : UISocialContentCtrlBase<UIViewForgetPassword>, IUIWithTitle
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUIEvent();
            LoginLogicUtil.Init();
        }

        protected override void OnOpen(object parameter)
        {
            //可能是Text的bug
            int charLimit = _cachedView.Phone.characterLimit;
            _cachedView.Phone.characterLimit = charLimit + 1;
            _cachedView.Phone.text = LoginLogicUtil.PhoneNum;
            _cachedView.Phone.characterLimit = charLimit;

            _cachedView.Pwd.text = "";
            _cachedView.SmsCode.text = "";
            LoginLogicUtil.OnSuccess = OnLoginSuccess;
            LoginLogicUtil.OnFailed = OnLoginFailed;
            base.OnOpen(parameter);
        }

        private IEnumerator ProcessSmsCodeCountDown()
        {
            Text text =  _cachedView.GetSMSCodeBtnLabel;
            for(int countDown = 60; countDown>0; countDown--)
            {
                text.text = "" + countDown +"秒后可重发";
                yield return new WaitForSeconds(1f);
            }
            text.text = "获取验证码";
            _cachedView.GetSMSCode.enabled = true;
        }

        private void OnResetPwd()
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
            bool verificationCheckResult = CheckTools.CheckVerificationCode(verificationCode);
            LoginLogicUtil.ShowVerificationCodeCheckTip(verificationCheckResult);
            if(!verificationCheckResult)
            {
                return;
            }
            CheckTools.ECheckPasswordResult pwdCheckResult = CheckTools.CheckPassword(pwd);
            LoginLogicUtil.ShowPasswordCheckTip(pwdCheckResult);
            if (pwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "密码重置中");
//            LoginLogicUtil.RequestSmsLogin(phone, verificationCode, pwd, EVerifyCodeType.VCT_ChangePassword);
            LoginLogicUtil.PhoneNum = _cachedView.Phone.text;
        }


        #endregion

        #region  private 


        private void OnSmsCode()
        {
//            string phoneNum = _cachedView.Phone.text;
//            bool phoneCheckResult = CheckTools.CheckPhoneNum(phoneNum);
//            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
//            if (!phoneCheckResult)
//            {
//                return;
//            }
//            Msg_CA_RequestSMSCode req = new Msg_CA_RequestSMSCode();
//            req.PhoneNum = phoneNum;
//            req.VerifyCodeType = EVerifyCodeType.VCT_ChangePassword;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_CommonResult>(SoyHttpApiPath.GetSmsCode, req, ret => {
//                ECommonResultCode c = (ECommonResultCode)ret.Code;
//                LogHelper.Info("Send VerificationCode Success, Code: {0}, Msg: {1}", c, ret.Msg);
//            }, (code, msg) => {
//                ENetResultCode c = (ENetResultCode)code;
//                LogHelper.Info("Send VerificationCode Error, Code: {0}, Msg: {1}", c, msg);
//            });
//            _cachedView.SmsCode.text = "";
//            _cachedView.GetSMSCode.enabled = false;
//            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());
        }

        private void InitUIEvent()
        {
            _cachedView.GetSMSCode.onClick.AddListener(OnGetSMSCodeButtonClick);
            _cachedView.DoResetPassword.onClick.AddListener(OnDoResetPasswordClick);
        }

        private void OnLoginSuccess()
        {
            if(_uiStack != null)
            {
                _uiStack.Close();
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            CommonTools.ShowPopupDialog("密码重置成功");
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

        private void OnDoResetPasswordClick()
        {
            OnResetPwd();
        }

        #endregion

        public object GetTitle()
        {
            return "忘记密码";
        }

    }
}
