using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldProjectBase : UPCtrlWorldPanelBase
    {
        protected const int _pageSize = 21;
        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        protected List<Project> _projectList;
        protected bool _isRequesting;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        protected override void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
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
            var item = new UMCtrlProject();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_contentList[inx]);
            }
        }

        protected override void RequestData(bool append = false)
        {
        }

        public override void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
//            RequestData();
        }

        public override void Clear()
        {
            _unload = true;
            _contentList.Clear();
            _dict.Clear();
            _projectList = null;
            _cachedView.GridDataScrollers[(int)_menu].ContentPosition = Vector2.zero;
        }
    }
}