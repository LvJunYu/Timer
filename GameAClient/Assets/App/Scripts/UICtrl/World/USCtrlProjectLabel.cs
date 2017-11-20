using SoyEngine;

namespace GameA
{
    public class USCtrlProjectLabel : USCtrlBase<USViewProjectLabel>
    {
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Btn.onClick.AddListener(OnBtn);
        }

        private void OnBtn()
        {
            SocialGUIManager.ShowPopupDialog("暂未开启标签功能。");
        }
    }
}