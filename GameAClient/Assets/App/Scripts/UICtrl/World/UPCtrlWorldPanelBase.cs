using SoyEngine;

namespace GameA
{
    public abstract class UPCtrlWorldPanelBase : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        protected EResScenary _resScenary;
        protected UICtrlWorld.EMenu _menu;
        protected bool _unload;

        public override void Open()
        {
            base.Open();
            _unload = false;
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            bool canSearch = _menu != UICtrlWorld.EMenu.RankList && _menu != UICtrlWorld.EMenu.Multi;
            _cachedView.SearchBtn.SetActiveEx(canSearch);
            _cachedView.SearchInputField.SetActiveEx(canSearch);
        }

        public override void Close()
        {
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public abstract void RequestData(bool append = false);

        public virtual void RefreshView()
        {
        }

        protected abstract void OnItemRefresh(IDataItemRenderer item, int inx);

        public virtual void OnChangeHandler(long val){}

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlWorld.EMenu menu)
        {
            _menu = menu;
        }

        public virtual void Clear()
        {
        }
    }
}