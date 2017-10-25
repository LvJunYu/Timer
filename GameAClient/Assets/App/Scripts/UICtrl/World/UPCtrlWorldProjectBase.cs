using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldProjectBase : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        protected const int _pageSize = 12;
        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();
        protected List<Project> _projectList;
        protected EResScenary _resScenary;
        protected bool _isRequesting;
        protected bool _unload;
        protected UICtrlWorld.EMenu _menu;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _unload = false;
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _unload = true;
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        protected void RefreshView()
        {
            if (_projectList == null)
            {
                _contentList.Clear();
                _dict.Clear();
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            _dict.Clear();
            for (int i = 0; i < _projectList.Count; i++)
            {
                CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(_projectList[i], OnItemClick);
                _contentList.Add(w);
                _dict.Add(_projectList[i].ProjectId, w);
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        protected void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProject();
            item.Init(parent, _resScenary);
            return item;
        }

        protected virtual void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if(inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_contentList[inx]);
            }
        }

        protected virtual void RequestData(bool append = false)
        {
        }

        public void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public void SetMenu(UICtrlWorld.EMenu menu)
        {
            _menu = menu;
        }
    }
}