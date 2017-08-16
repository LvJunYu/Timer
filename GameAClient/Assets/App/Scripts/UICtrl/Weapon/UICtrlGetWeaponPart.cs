using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlGetWeaponPart : UICtrlGenericBase<UIViewGetWeaponPart>
    {

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void OnOpen (object parameter)
        {
            base.OnOpen(parameter);
            _cachedView.BtnClose.onClick.AddListener(OnBGBtn);
            _cachedView.BtnConfirm.onClick.AddListener(OnBGBtn);
        }
        
        protected override void OnClose() {
            
            base.OnClose ();
          
        }
        
        protected override void InitEventListener() {
            base.InitEventListener ();
        }
        
        protected override void OnViewCreated() {
            base.OnViewCreated ();
           

        
        }

        public override void OnUpdate ()
        {
            base.OnUpdate ();
         
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.InGameTip;
        }

        private void OnBGBtn () {
                SocialGUIManager.Instance.CloseUI<UICtrlGetWeaponPart> ();
        }


    
        #endregion
    }
}
