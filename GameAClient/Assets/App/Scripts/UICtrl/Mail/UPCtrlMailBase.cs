using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlMailBase : UPCtrlBase<UICtrlMail, UIViewMail>
    {
        protected EResScenary _resScenary;
        protected UICtrlMail.EMenu _menu;
        protected List<Mail> _dataList = new List<Mail>();
        protected int _startIndex = 0;
        protected int _maxCount = 50;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetTalkItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RefreshData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _dataList = null;
        }

        public void RefreshData()
        {
            LocalUser.Instance.Mail.Request(_startIndex, _maxCount, () =>
                {
                    _dataList = LocalUser.Instance.Mail.DataList;
                    RefreshView();
                },
                code=>{SocialGUIManager.ShowPopupDialog("请求邮箱数据失败。");});
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_dataList.Count);
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
        }

        protected virtual IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlMail();
            item.Init(parent, _resScenary);
            return item;
        }

        public void SetResScenary(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlMail.EMenu menu)
        {
            _menu = menu;
        }
    }
}