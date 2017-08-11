using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlAccountModify : UISocialContentCtrlBase<UIViewAccountModify>, IUIWithTitle
    {
        #region 常量与字段
        private User _user;
        private SnsBinding _snsBinding;
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
            _cachedView.PhoneNumBtn.onClick.AddListener(()=>{
                SocialGUIManager.Instance.OpenUI<UICtrlChangePhoneNum>();
            });
            _cachedView.ChangePwByOldPwBtn.onClick.AddListener(()=>{
                if(string.IsNullOrEmpty(_user.PhoneNum))
                {
                    CommonTools.ShowPopupDialog("请先绑定手机");
                    return;
                }
                SocialGUIManager.Instance.OpenUI<UICtrlChangePwByOldPw>();
            });
            _cachedView.ChangePwBySmsCodeBtn.onClick.AddListener(()=>{
                if(string.IsNullOrEmpty(_user.PhoneNum))
                {
                    CommonTools.ShowPopupDialog("请先绑定手机");
                    return;
                }
                SocialGUIManager.Instance.OpenUI<UICtrlChangePwBySmsCode>();
            });
            _cachedView.WechatToggle.onValueChanged.AddListener(OnToggleWechat);
            _cachedView.QQToggle.onValueChanged.AddListener(OnToggleQQ);
            _cachedView.WeiboToggle.onValueChanged.AddListener(OnToggleWeibo);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountLoginStateChanged);
        }

        protected override void OnDestroy()
        {
        }

        private void UpdateView()
        {
            if(string.IsNullOrEmpty(_user.PhoneNum))
            {
                _cachedView.PhoneNumText.text = "未绑定";
            }
            else
            {
                _cachedView.PhoneNumText.text = _user.PhoneNum;
            }
            string unbindingStr = "未绑定";
            _cachedView.WechatToggle.isOn = _snsBinding.BindingWechat;
            _cachedView.WechatNameText.text = _snsBinding.BindingWechat ? _snsBinding.WechatNickName : unbindingStr;
            _cachedView.QQToggle.isOn = _snsBinding.BindingQQ;
            _cachedView.QQNameText.text = _snsBinding.BindingQQ ? _snsBinding.QQNickName : unbindingStr;
            _cachedView.WeiboToggle.isOn = _snsBinding.BindingWeibo;
            _cachedView.WeiboNameText.text = _snsBinding.BindingWeibo ? _snsBinding.WeiboNickName : unbindingStr;
        }

        protected override void OnOpen(object parameter)
        {
//            if(LocalUser.Instance.UserLegacy == null)
//            {
//                LogHelper.Error("UICtrlAccountModify user not login");
//                return;
//            }
//            else
//            {
//                _user = LocalUser.Instance.UserLegacy;
//                _snsBinding = _user.SnsBinding;
//                if(_snsBinding == null)
//                {
//                    _snsBinding = new SnsBinding();
//                    _snsBinding.Clear();
//                }
//                UpdateView();
//            }
            base.OnOpen(parameter);
        }

        #endregion

        #region 事件处理
        private void OnAccountLoginStateChanged()
        {
            if(_isViewCreated && _isOpen)
            {
                _uiStack.OpenPrevious();
            }
        }

        private void OnToggleWechat(bool toggle)
        {
            _cachedView.WechatToggle.targetGraphic.enabled = !toggle;
            if(toggle == _snsBinding.BindingWechat)
            {
                return;
            }
            if(toggle)
            {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "处理中");
//                LoginLogicUtil.OnSnsInfoLogin = msg=>OnAuthSuccess(_cachedView.WechatToggle, ESNSPlatform.SP_WeChat, msg);
//                LoginLogicUtil.OnSnsInfoCancel = ()=>OnAuthCancel(_cachedView.WechatToggle);
//                LoginLogicUtil.OnWeChat();
            }
            else
            {
                TryUnbind(ESNSPlatform.SP_WeChat, _cachedView.WechatToggle);
            }
        }

        private void OnToggleQQ(bool toggle)
        {
            _cachedView.QQToggle.targetGraphic.enabled = !toggle;
            if(toggle == _snsBinding.BindingQQ)
            {
                return;
            }
            if(toggle)
            {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "处理中");
//                LoginLogicUtil.OnSnsInfoLogin = msg=>OnAuthSuccess(_cachedView.QQToggle, ESNSPlatform.SP_QQ, msg);
//                LoginLogicUtil.OnSnsInfoCancel = ()=>OnAuthCancel(_cachedView.QQToggle);
//                LoginLogicUtil.OnQQ();
            }
            else
            {
                TryUnbind(ESNSPlatform.SP_QQ, _cachedView.QQToggle);
            }
        }

        private void OnToggleWeibo(bool toggle)
        {
            _cachedView.WeiboToggle.targetGraphic.enabled = !toggle;
            if(toggle == _snsBinding.BindingWeibo)
            {
                return;
            }
            if(toggle)
            {
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "处理中");
//                LoginLogicUtil.OnSnsInfoLogin = msg=>OnAuthSuccess(_cachedView.WeiboToggle, ESNSPlatform.SP_Weibo, msg);
//                LoginLogicUtil.OnSnsInfoCancel = ()=>OnAuthCancel(_cachedView.WeiboToggle);
//                LoginLogicUtil.OnWeibo();
            }
            else
            {
                TryUnbind(ESNSPlatform.SP_Weibo, _cachedView.WeiboToggle);
            }
        }

//        private void OnAuthSuccess(Toggle toggle, ESNSPlatform platform, Msg_CA_Login msgLogin)
//        {
//            Msg_CA_BindSnsAccount msg = new Msg_CA_BindSnsAccount();
//            Msg_CA_Login.SNSUserInfo snsInfo = msgLogin.SnsUserInfo;
//            msg.Pid = snsInfo.Pid;
//            msg.AccessToken = snsInfo.AccessToken;
//            msg.AdditionalStr = snsInfo.AdditionalId;
//            msg.NickName = snsInfo.UserNickName;
//            msg.PlatformType = platform;
//            User user = _user;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_BindSnsAccountRet>(SoyHttpApiPath.BindSnsAccount, msg, ret=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                if(ret.ResultCode == EBindSnsResult.BSR_Success)
//                {
//                    if(LocalUser.Instance.User == user)
//                    {
////                        _user.OnSyncUserSnsBinding(ret.SnsBinding);
//                        if(_isOpen)
//                        {
//                            UpdateView();
//                        }
//                        CommonTools.ShowPopupDialog("绑定成功");
//                    }
//                    return;
//                }
//
//                if(ret.ResultCode == EBindSnsResult.BSR_HasBinded)
//                {
//                    CommonTools.ShowPopupDialog("该社交账号已被其他匠游账号绑定，请尝试直接用该社交账号登录");
//                }
//                toggle.isOn = false;
//            }, (failCode, failMsg)=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                CommonTools.ShowPopupDialog("绑定失败");
//                toggle.isOn = false;
//            });
//        }

        private void OnAuthCancel(Toggle toggle)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            CommonTools.ShowPopupDialog("授权失败或用户取消授权，绑定失败");
            toggle.isOn = false;
        }

        private void TryUnbind(ESNSPlatform platform, Toggle toggle)
        {
//            if(string.IsNullOrEmpty(_user.PhoneNum))
//            {
//                CommonTools.ShowPopupDialog("请先绑定手机号，再解绑社交账号，否则该账号将无法登录");
//                toggle.isOn = true;
//                return;
//            }
//            Msg_CA_UnbindSnsAccount msg = new Msg_CA_UnbindSnsAccount();
//            msg.PlatformType = platform;
//            User user = _user;
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "处理中");
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_BindSnsAccountRet>(SoyHttpApiPath.UnbindSnsAccount, msg, ret=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                if(ret.ResultCode == EBindSnsResult.BSR_Success)
//                {
//                    if(LocalUser.Instance.User == user)
//                    {
////                        user.OnSyncUserSnsBinding(ret.SnsBinding);
//                        CommonTools.ShowPopupDialog("解绑成功");
//                    }
//                    return;
//                }
//                CommonTools.ShowPopupDialog("解绑失败");
//                toggle.isOn = true;
//            }, (failCode, failMsg)=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                CommonTools.ShowPopupDialog("解绑失败");
//                toggle.isOn = true;
//            });
        }
        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "账号和密码";
        }

        #endregion
    }
}
