using SoyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameA
{
    public abstract class UICtrlInGameAnimationBase<T> : UICtrlAnimationBase<T> where T : UIViewBase
    {
        protected override void InitEventListener ()
        {
            base.InitEventListener ();
            RegisterEvent (EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        private void OnChangeToAppMode ()
        {
            SocialGUIManager.Instance.CloseUI (this.GetType ());
        }
    }
}