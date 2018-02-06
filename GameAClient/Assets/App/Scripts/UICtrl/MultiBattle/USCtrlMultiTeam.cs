using SoyEngine;

namespace GameA
{
    public class USCtrlMultiTeam : USCtrlBase<USViewMultiTeam>
    {
        private UserInfoDetail _userInfo;
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
        }
        
        private void OnDeleteBtn()
        {
        }

        public void SetSelected(bool value)
        {
            _cachedView.BgSelectedImg.SetActiveEx(value);
        }
    }
}