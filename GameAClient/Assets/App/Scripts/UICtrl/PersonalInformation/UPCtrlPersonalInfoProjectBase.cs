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

        protected List<Project> _projectList;

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
            _dict.Clear();
            if (_projectList == null)
            {
                _cachedView.EmptyObj.SetActive(true);
                _cachedView.GridDataScrollers[(int) _menu - 1].SetEmpty();
                return;
            }
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            for (int i = 0; i < _projectList.Count; i++)
            {
                if (!_dict.ContainsKey(_projectList[i].ProjectId))
                {
                    CardDataRendererWrapper<Project> w = new CardDataRendererWrapper<Project>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    _dict.Add(_projectList[i].ProjectId, w);
                }
            }
            _cachedView.EmptyObj.SetActive(_contentList.Count == 0);
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

        public override void Clear()
        {
            base.Clear();
            _unload = true;
            _cachedView.GridDataScrollers[(int) _menu - 1].RefreshCurrent();
            _cachedView.GridDataScrollers[(int) _menu - 1].ContentPosition = Vector2.zero;
            _projectList = null;
            _contentList.Clear();
            _dict.Clear();
        }
    }
}