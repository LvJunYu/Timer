using SoyEngine;

namespace GameA
{
    public abstract class UPCtrlProjectDetailBase : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>,
        IOnChangeHandler<long>
    {
        protected EResScenary _resScenary;
        protected UICtrlProjectDetail.EMenu _menu;

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
        }

        public override void Close()
        {
            base.Close();
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
        }

        protected abstract void RequestData(bool append = false);

        protected abstract void RefreshView();

        public void OnChangeToApp()
        {
            RequestData();
        }

        public void OnChangeHandler(long val)
        {
            if (_isOpen)
            {
                RefreshView();
            }
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlProjectDetail.EMenu menu)
        {
            _menu = menu;
        }

        public abstract void Clear();
    }
}