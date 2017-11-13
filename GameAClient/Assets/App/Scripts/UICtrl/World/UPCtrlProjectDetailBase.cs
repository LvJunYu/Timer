using SoyEngine;
using UnityEngine;

namespace GameA
{
    public abstract class UPCtrlProjectDetailBase : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>,
        IOnChangeHandler<long>
    {
        protected EResScenary _resScenary;
        protected UICtrlProjectDetail.EMenu _menu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            if (_menu < UICtrlProjectDetail.EMenu.Comment)
//            {
//                _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
//            }
        }

        public override void Open()
        {
            base.Open();
//            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            base.Close();
//            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
        }

        protected abstract void RequestData(bool append = false);

        protected abstract void RefreshView();

        protected abstract IDataItemRenderer GetItemRenderer(RectTransform parent);

        protected abstract void OnItemRefresh(IDataItemRenderer item, int inx);

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

        public void Set(EResScenary resScenary)
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