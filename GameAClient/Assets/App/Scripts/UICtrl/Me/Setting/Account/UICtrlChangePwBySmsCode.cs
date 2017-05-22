  /********************************************************************
  ** Filename : UICtrlChangePwBySmsCode.cs
  ** Author : quan
  ** Date : 2016/6/30 19:14
  ** Summary : UICtrlChangePwBySmsCode.cs
  ***********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlChangePwBySmsCode : UISocialContentCtrlBase<UIViewChangePwBySmsCode>, IUIWithTitle
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SmsCodeBtn.onClick.AddListener(OnSmsBtnClick);
            _cachedView.ConfirmBtn.onClick.AddListener(OnConfirmBtnClick);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        protected override void OnDestroy()
        {
        }

        private void UpdateView()
        {
//            _cachedView.PhoneNumInput.text = LocalUser.Instance.UserLegacy.PhoneNum;
//            _cachedView.SmsCodeInput.text = string.Empty;
        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
        }

        #endregion

        #region 事件处理
        private void OnSmsBtnClick()
        {
//            Msg_CA_RequestSMSCode req = new Msg_CA_RequestSMSCode();
//            req.PhoneNum = _cachedView.PhoneNumInput.text;
//            req.VerifyCodeType = EVerifyCodeType.VCT_ChangePassword;
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

        private void OnConfirmBtnClick()
        {
//            var smsCode = _cachedView.SmsCodeInput.text;
//            bool smsCodeResult = CheckTools.CheckVerificationCode(smsCode);
//            LoginLogicUtil.ShowVerificationCodeCheckTip(smsCodeResult);
//            if(!smsCodeResult)
//            {
//                return;
//            }
//            var pwd = _cachedView.PwdInput.text;
//            CheckTools.ECheckPasswordResult pwdCheckResult = CheckTools.CheckPassword(pwd);
//            LoginLogicUtil.ShowPasswordCheckTip(pwdCheckResult);
//            if (pwdCheckResult != CheckTools.ECheckPasswordResult.Success)
//            {
//                return;
//            }
//            Msg_CA_ChangePassword msg = new Msg_CA_ChangePassword();
//            msg.NewPassword = pwd;
//            msg.VerificationCode = smsCode;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_CommonResult>(SoyHttpApiPath.ChangePassword, msg, ret=>{
//                EChangePasswordResult resultCode = (EChangePasswordResult)ret.Code;
//                if(resultCode == EChangePasswordResult.CPR_Success)
//                {
//                    LocalUser.Instance.Account.OnTokenChange(ret.Msg);
//                    CommonTools.ShowPopupDialog("密码修改成功");
//                    if(_isOpen)
//                    {
//                        _uiStack.OpenPrevious();
//                    }
//                    return;
//                }
//                if(resultCode == EChangePasswordResult.CPR_NewPasswordError)
//                {
//                    CommonTools.ShowPopupDialog("新密码格式有误", null);
//                }
//                else if(resultCode == EChangePasswordResult.CPR_VerificationCodeError)
//                {
//                    CommonTools.ShowPopupDialog("验证码错误");
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("密码修改失败");
//                }
//            }, (failCode, failMsg)=>{
//                
//            });
        }

        private IEnumerator ProcessSmsCodeCountDown()
        {
            _cachedView.SmsCodeInput.text = "";
            _cachedView.SmsCodeBtn.enabled = false;
            Text text =  _cachedView.SmsCodeText;
            for(int countDown = 60; countDown>0; countDown--)
            {
                text.text = "" + countDown +"秒后可重发";
                yield return new WaitForSeconds(1f);
            }
            text.text = "获取验证码";
            _cachedView.SmsCodeBtn.enabled = true;
        }
        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "修改密码";
        }

        #endregion
    }
}
