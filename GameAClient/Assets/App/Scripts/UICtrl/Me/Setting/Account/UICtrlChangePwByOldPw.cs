  /********************************************************************
  ** Filename : UICtrlChangePwByOldPw.cs
  ** Author : quan
  ** Date : 2016/6/30 18:42
  ** Summary : UICtrlChangePwByOldPw.cs
  ***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlChangePwByOldPw : UISocialContentCtrlBase<UIViewChangePwByOldPw>, IUIWithTitle
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
            _cachedView.OldPwInput.text = string.Empty;
            _cachedView.NewPwInput.text = string.Empty;
        }


        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
        }

        #endregion

        #region 事件处理

        private void OnConfirmBtnClick()
        {
//            string oldPw = _cachedView.OldPwInput.text;
//            CheckTools.ECheckPasswordResult oldPwResult = CheckTools.CheckPassword(oldPw);
//
//            if(oldPwResult != CheckTools.ECheckPasswordResult.Success)
//            {
//                if (oldPwResult == CheckTools.ECheckPasswordResult.TooShort)
//                {
//                    CommonTools.ShowPopupDialog("旧密码过短，密码长度8-16个字符", null);
//                }
//                else if (oldPwResult == CheckTools.ECheckPasswordResult.TooLong)
//                {
//                    CommonTools.ShowPopupDialog("旧密码过长，密码长度8-16个字符", null);
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("旧密码格式有误", null);
//                }
//                return;
//            }
//            string newPw = _cachedView.NewPwInput.text;
//            CheckTools.ECheckPasswordResult newPwResult = CheckTools.CheckPassword(newPw);
//            if(newPwResult != CheckTools.ECheckPasswordResult.Success)
//            {
//                if (newPwResult == CheckTools.ECheckPasswordResult.TooShort)
//                {
//                    CommonTools.ShowPopupDialog("新密码过短，密码长度8-16个字符", null);
//                }
//                else if (newPwResult == CheckTools.ECheckPasswordResult.TooLong)
//                {
//                    CommonTools.ShowPopupDialog("新密码过长，密码长度8-16个字符", null);
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("新密码格式有误", null);
//                }
//                return;
//            }
//            if(newPw == oldPw)
//            {
//                CommonTools.ShowPopupDialog("新密码和旧密码相同");
//                return;
//            }
//            Msg_CA_ChangePassword msg = new Msg_CA_ChangePassword();
//            msg.OldPassword = oldPw;
//            msg.NewPassword = newPw;
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
//                else if(resultCode == EChangePasswordResult.CPR_OldPasswordError)
//                {
//                    CommonTools.ShowPopupDialog("旧密码错误");
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("密码修改失败");
//                }
//            }, (failCode, failMsg)=>{
//            });
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
