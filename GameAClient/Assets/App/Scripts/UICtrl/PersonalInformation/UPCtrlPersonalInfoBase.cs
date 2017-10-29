using SoyEngine;

namespace GameA
{
    public abstract class UPCtrlPersonalInfoBase : UPCtrlBase<UICtrlPersonalInformation, UIViewPersonalInformation>
    {
        protected EResScenary _resScenary;
        protected UICtrlPersonalInformation.EMenu _menu;

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
        }

        public override void Close()
        {
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public abstract void RefreshView();
        
        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlPersonalInformation.EMenu menu)
        {
            _menu = menu;
        }
    }
}