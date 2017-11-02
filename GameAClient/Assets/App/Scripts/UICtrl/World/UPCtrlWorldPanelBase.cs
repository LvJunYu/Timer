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
            _cachedView.SearchBtn.SetActiveEx(_menu != UICtrlWorld.EMenu.RankList);
            _cachedView.SearchInputField.SetActiveEx(_menu != UICtrlWorld.EMenu.RankList);
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        protected abstract void RequestData(bool append = false);

        protected virtual void RefreshView()
        {
        }

        protected abstract void OnItemRefresh(IDataItemRenderer item, int inx);

        public abstract void OnChangeHandler(long val);

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