  /********************************************************************
  ** Filename : UICtrlChangePhoneNum.cs
  ** Author : quan
  ** Date : 2016/6/30 18:41
  ** Summary : UICtrlChangePhoneNum.cs
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
    public class UICtrlChangePhoneNum : UISocialContentCtrlBase<UIVIewChangePhoneNum>, IUIWithTitle
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
            _cachedView.SmsCodeBtn.onClick.AddListener(OnGetSmsCodeBtnClick);
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
            _cachedView.PhoneNumInput.text = string.Empty;
            _cachedView.SmsCodeInput.text = string.Empty;
        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
        }

        #endregion

        #region 事件处理
        private void OnGetSmsCodeBtnClick()
        {
//            string phoneNum = _cachedView.PhoneNumInput.text;
//            bool phoneResult = CheckTools.CheckPhoneNum(phoneNum);
//            LoginLogicUtil.ShowPhoneNumCheckTip(phoneResult);
//            if(!phoneResult)
//            {
//                return;
//            }
//            User user = LocalUser.Instance.User;
//            if(user == null || user.PhoneNum == phoneNum)
//            {
//                CommonTools.ShowPopupDialog("输入的手机号和当前账号绑定相同");
//                return;
//            }
//            Msg_CA_RequestBindPhoneNumSmsCode msg = new Msg_CA_RequestBindPhoneNumSmsCode();
//            msg.PhoneNum = _cachedView.PhoneNumInput.text;
//            _cachedView.SmsCodeBtn.enabled = false;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_BindPhoneNumRet>(SoyHttpApiPath.GetBindPhoneNumSmsCode, msg, ret=>{
//                if(ret.ResultCode == EBindPhoneNumResult.BPNR_Success)
//                {
//                    CommonTools.ShowPopupDialog("发送成功");
//                    return;
//                }
//                if(ret.ResultCode == EBindPhoneNumResult.BPNR_PhoneHasBeenBinding)
//                {
//                    CommonTools.ShowPopupDialog("该手机号已绑定其他匠游账号");
//                }
//                else if(ret.ResultCode == EBindPhoneNumResult.BPNR_SendCodeError)
//                {
//                    CommonTools.ShowPopupDialog("验证码发送失败");
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("发送失败，请稍后重试");
//                }
//            }, (failCode, failMsg)=>{
//            });
//            CoroutineProxy.Instance.StartCoroutine(ProcessSmsCodeCountDown());
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

        private void OnConfirmBtnClick()
        {
//            string phoneNum = _cachedView.PhoneNumInput.text;
//            bool phoneResult = CheckTools.CheckPhoneNum(phoneNum);
//            LoginLogicUtil.ShowPhoneNumCheckTip(phoneResult);
//            if(!phoneResult)
//            {
//                return;
//            }
//            User user = LocalUser.Instance.User;
//            if(user == null || user.PhoneNum == phoneNum)
//            {
//                CommonTools.ShowPopupDialog("输入的手机号和当前账号绑定相同");
//                return;
//            }
//            string smsCode = _cachedView.SmsCodeInput.text;
//            bool smsCodeResult = CheckTools.CheckVerificationCode(smsCode);
//            LoginLogicUtil.ShowVerificationCodeCheckTip(smsCodeResult);
//            if(!smsCodeResult)
//            {
//                return;
//            }
//            Msg_CA_BindPhoneNum msg = new Msg_CA_BindPhoneNum();
//            msg.PhoneNum = phoneNum;
//            msg.SmsCode = smsCode;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_BindPhoneNumRet>(SoyHttpApiPath.BindPhoneNum, msg, ret=>{
//                if(ret.ResultCode == EBindPhoneNumResult.BPNR_Success)
//                {
//                    if(LocalUser.Instance.User == user)
//                    {
//                        user.PhoneNum = phoneNum;
//                    }
//                    if(_isOpen)
//                    {
//                        _uiStack.OpenPrevious();
//                    }
//                    return;
//                }
//                if(ret.ResultCode == EBindPhoneNumResult.BPNR_VerificationCodeError)
//                {
//                    CommonTools.ShowPopupDialog("验证码输入有误");
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("绑定失败");
//                }
//            }, (failCode, failMsg)=>{
//                
//            });
        }

        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "绑定手机";
        }

        #endregion
    }
}

