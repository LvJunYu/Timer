using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInfoProjectBase : UPCtrlPersonalInfoBase, IOnChangeHandler<long>
    {
        protected const int PageSize = 10;
        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        protected List<Project> _pojectList;

        protected bool _unload;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _unload = false;
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _unload = true;
            _cachedView.GridDataScrollers[(int) _menu - 1].RefreshCurrent();
            base.Close();
        }

        protected void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
        }

        protected virtual void OnItemRefresh(IDataItemRenderer item, int inx)
        {
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoProject();
            item.Init(parent, _resScenary);
            return item;
        }

        protected virtual void RequestData(bool append = false)
        {
        }

        public override void RefreshView()
        {
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _pojectList.Count);
            _dict.Clear();
            for (int i = 0; i < _pojectList.Count; i++)
            {
                Project p = _pojectList[i];
                CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(p, OnItemClick);
                _contentList.Add(w);
                _dict.Add(p.ProjectId, w);
            }
            _cachedView.GridDataScrollers[(int) _menu - 1].SetItemCount(_contentList.Count);
        }

        public void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }

        public override void OnDestroy()
        {
            _contentList = null;
            _dict = null;
            base.OnDestroy();
        }
    }
}