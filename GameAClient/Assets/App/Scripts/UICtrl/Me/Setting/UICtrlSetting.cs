 /********************************************************************
 ** Filename : UICtrlSetting.cs
 ** Author : quan
 ** Date : 16/4/30 下午6:42
 ** Summary : UICtrlSetting.cs
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
    public class UICtrlSetting : UISocialContentCtrlBase<UIViewSetting>, IUIWithTitle
    {
        #region 常量与字段
        private const string AccountSettingIconSpriteName = "face_s";
        private const string ClearCacheIconSpriteName = "face_s";
        private ListMenuView _listMenuView;
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
            ListMenuViewCreator creator = new ListMenuViewCreator(_cachedView.MenuListDock);
            creator.AddSimpleItem("账号和密码", ()=>{
                if(!AppLogicUtil.CheckAndRequiredLogin())
                {
                    return;
                }
                SocialGUIManager.Instance.OpenUI<UICtrlAccountModify>();
            });
            creator.AddSimpleItem("清除缓存", ()=>{
                SocialApp.Instance.ClearCache();
                Application.Quit();
            });

            _listMenuView = creator.GetView();
            _cachedView.LogoutButton.onClick.AddListener(OnLogoutClick);
            _cachedView.RecommendConsoleBtn.onClick.AddListener(()=>{
//                SocialGUIManager.Instance.OpenUI<UICtrlRecommendConsole>();
            });
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(SoyEngine.EMessengerType.OnAccountLoginStateChanged, OnAccountLoginStateChanged);
        }

        protected override void OnDestroy()
        {
            _cachedView.LogoutButton.onClick.RemoveListener(OnLogoutClick);
        }

        private void UpdateView()
        {
            if(LocalUser.Instance.Account.HasLogin)
            {
                _cachedView.LogoutButton.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.LogoutButton.gameObject.SetActive(false);
            }

//            _cachedView.AdminDock.SetActive(LocalUser.Instance.UserLegacy != null && LocalUser.Instance.UserLegacy.AccountRoleType == EAccountRoleType.AcRT_Admin);
        }

        protected override void OnOpen(object parameter)
        {
            UpdateView();
            base.OnOpen(parameter);
        }

        #endregion

        #region 事件处理
        private void OnAccountLoginStateChanged()
        {
            if(_isViewCreated && _isOpen)
            {
                UpdateView();
            }
        }


        private void OnLogoutClick()
        {
            NetworkManager.AppHttpClient.SendWithCb<Msg_SC_CMD_Logout>(SoyHttpApiPath.Logout, new Msg_CS_CMD_Logout(), ret=>{

            }, (intCode, str)=>{

            });
            LocalUser.Instance.Account.Logout();
            _uiStack.OpenPrevious();
        }


        #endregion 事件处理

        #region 接口
        public object GetTitle()
        {
            return "设置";
        }

        #endregion
    }
}
