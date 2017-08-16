using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlChangePassword : UICtrlGenericBase<UIViewChangePassword>
    {

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
            int charLimit = _cachedView.Phone.characterLimit;
            _cachedView.Phone.characterLimit = charLimit + 1;
            _cachedView.Phone.text = LoginLogicUtil.PhoneNum;
            _cachedView.Phone.characterLimit = charLimit;
            _cachedView.NewPwd.text = "";
            _cachedView.OldPwd.text = "";
            _cachedView.SmsCode.text = "";
            LoginLogicUtil.OnSuccess = OnLoginSuccess;
            LoginLogicUtil.OnFailed = OnLoginFailed;
            base.OnOpen(parameter);
        }

        private IEnumerator ProcessSmsCodeCountDown()
        {
            Text text = _cachedView.GetSMSCodeBtnLabel;
            for (int countDown = 60; countDown > 0; countDown--)
            {
                text.text = "" + countDown + "秒后可重发";
                yield return new WaitForSeconds(1f);
            }
            text.text = "获取验证码";
            _cachedView.GetSMSCode.enabled = true;
        }

        private void OnResetPwd()
        {
            var phone = _cachedView.Phone.text;
            string verificationCode = null;
            var Newpwd = _cachedView.NewPwd.text;
            var Oldpwd = _cachedView.OldPwd.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phone);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if (!phoneCheckResult)
            {
                return;
            }
            //bool verificationCheckResult = CheckTools.CheckVerificationCode(verificationCode);
            //LoginLogicUtil.ShowVerificationCodeCheckTip(verificationCheckResult);
            //if(!verificationCheckResult)
            //{
            //    return;
            //}
            CheckTools.ECheckPasswordResult NewpwdCheckResult = CheckTools.CheckPassword(Newpwd);
            LoginLogicUtil.ShowPasswordCheckTip(NewpwdCheckResult);
            if (NewpwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            CheckTools.ECheckPasswordResult OldpwdCheckResult = CheckTools.CheckPassword(Oldpwd);
            LoginLogicUtil.ShowPasswordCheckTip(OldpwdCheckResult);
            if (OldpwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "密码修改中");

            LocalUser.Instance.Account.ChangePassword(Oldpwd, Newpwd, verificationCode,
                (ret) => {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Close();
                    SocialApp.Instance.LoginSucceed();
                },
                (ret) =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Error("修改密码失败, Code: " + ret);
                });
            //            LoginLogicUtil.RequestSmsLogin(phone, verificationCode, pwd, EVerifyCodeType.VCT_ChangePassword);
            LoginLogicUtil.PhoneNum = _cachedView.Phone.text;
        }

        private void OnSmsCode()
        {
            string phoneNum = _cachedView.Phone.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phoneNum);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if (!phoneCheckResult)
            {
                return;
            }
            RemoteCommands.GetVerificationCode(phoneNum, EAccountIdentifyType.AIT_Phone, EVerifyCodeType.VCT_ForgetPassword,
                (ret) => { }, null
                );
            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());

        }

        private void InitUIEvent()
        {
            _cachedView.GetSMSCode.onClick.AddListener(OnGetSMSCodeButtonClick);
            _cachedView.DoResetPassword.onClick.AddListener(OnDoResetPasswordClick);
            _cachedView.ForgetPassword.onClick.AddListener(ForgetPWD);
            _cachedView.CloseBtn.onClick.AddListener(Close);
        }

        private void OnLoginSuccess()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            CommonTools.ShowPopupDialog("修改重置成功");
        }

        private void OnLoginFailed()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        }
 
        #region uievent 
        private void OnGetSMSCodeButtonClick()
        {
            OnSmsCode();
        }

        private void OnDoResetPasswordClick()
        {
            OnResetPwd();
        }

        private void ForgetPWD()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlForgetPassword>();
            Close();
        }

        #endregion

        public object GetTitle()
        {
            return "忘记密码";
        }
    }
}
