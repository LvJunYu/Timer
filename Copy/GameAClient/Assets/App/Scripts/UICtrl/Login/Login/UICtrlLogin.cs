/********************************************************************
** Filename : UICtrlLogin
** Author : Dong
** Date : 2015/4/27 18:01:01
** Summary : UICtrlLogin
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIAutoSetup]
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
            BadWordManger.Instance.InputFeidAddListen(_cachedView.Phone);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.Pwd);
        }

        #endregion

        #region  private 

        #endregion

        #region uievent 

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
                    LoginLogicUtil.ShowLoginError(ret);
                    LogHelper.Error("登录失败, Code: " + ret);
                });
        }

        private void GuestLoginIn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "努力登录中");
            LocalUser.Instance.Account.GuestLoginIn(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialApp.Instance.LoginSucceed();
                },
                (ret) =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LoginLogicUtil.ShowLoginError(ELoginCode.LoginC_Error);
                });
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

        #endregion
    }
}