using SoyEngine;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlUpdateResource : UICtrlGenericBase<UIViewUpdateResource>
    {
        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainFrame;
        }

        protected override void InitEventListener()
        { 
            base.InitEventListener();
            RegisterEvent (EMessengerType.OnResourcesCheckStart, OnResourcesCheckStart);
            RegisterEvent (EMessengerType.OnResourcesCheckFinish, OnResourcesCheckFinish);
            RegisterEvent<float>(EMessengerType.OnResourceUpdateProgressChange, OnResourceUpdateProgressChange);
        }

        private void OnResourcesCheckStart ()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlUpdateResource> ();
        }
        private void OnResourcesCheckFinish ()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlUpdateResource> ();
        }

        private void OnResourceUpdateProgressChange (float progress)
        {
            _cachedView.ProgressText.text = string.Format ("{0:D} %", progress * 100);
        }

    }
}