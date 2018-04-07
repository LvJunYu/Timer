using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlBindingPhone : UICtrlGenericBase<UIViewBindingPhone>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUIEvent();
            InitInputFieldCheck();
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

        private void OnBindingPhone()
        {
            var phone = _cachedView.Phone.text;
            string verificationCode = _cachedView.SmsCode.text;
            var pwd = _cachedView.Pwd.text;
            bool phoneCheckResult = CheckTools.CheckPhoneNum(phone);
            LoginLogicUtil.ShowPhoneNumCheckTip(phoneCheckResult);
            if (!phoneCheckResult)
            {
                return;
            }
            bool verificationCheckResult = CheckTools.CheckVerificationCode(verificationCode);
            LoginLogicUtil.ShowVerificationCodeCheckTip(verificationCheckResult);
            if (!verificationCheckResult)
            {
                return;
            }
            CheckTools.ECheckPasswordResult newpwdCheckResult = CheckTools.CheckPassword(pwd);
            LoginLogicUtil.ShowPasswordCheckTip(newpwdCheckResult);
            if (newpwdCheckResult != CheckTools.ECheckPasswordResult.Success)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "账号绑定中");

            LocalUser.Instance.Account.BindingPhone(phone, pwd, EAccountIdentifyType.AIT_Phone, verificationCode,
                ret =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    Close();
                    SocialApp.Instance.LoginSucceed();
                },
                code =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LoginLogicUtil.ShowAccountBindError(code);
                });
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
            RemoteCommands.GetVerificationCode(phoneNum, EAccountIdentifyType.AIT_Phone, EVerifyCodeType.VCT_Bind,
                ret => { }, null
            );
            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());
        }

        private void InitUIEvent()
        {
            _cachedView.GetSMSCode.onClick.AddListener(OnGetSMSCodeButtonClick);
            _cachedView.ConfirmBtn.onClick.AddListener(OnConfirmBtnClick);
            _cachedView.CloseBtn.onClick.AddListener(Close);
        }

        private void InitInputFieldCheck()
        {
            BadWordManger.Instance.InputFeidAddListen(_cachedView.Phone);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.Pwd);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.SmsCode);
        }

        #region uievent 

        private void OnGetSMSCodeButtonClick()
        {
            OnSmsCode();
        }

        private void OnConfirmBtnClick()
        {
            OnBindingPhone();
        }

        #endregion
    }
}