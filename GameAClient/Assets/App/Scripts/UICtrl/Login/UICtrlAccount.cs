using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlAccount : UICtrlGenericBase<UIViewAccount>
    {
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.LoginOut.onClick.AddListener(LoginOut);
        }

        private void LoginOut()
        {
            
            SocialGUIManager.Instance.OpenPopupUI<UICtrlLogin>();
            RemoteCommands.Logout(1,
                (ret) =>
                {
                    LocalUser.Instance.Account.Logout();
                }, null
                );
        }
    }
}