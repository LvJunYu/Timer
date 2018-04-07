using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public abstract class UPCtrlWorkShopOfficialProjectBase : UPCtrlBase<UICtrlOfficalWorkShop, UIViewOfficalWorkShop>,
        IOnChangeHandler<long>
    {
        protected bool _hasRequested;
        protected const int _pageSize = 21;
        protected List<Project> _projectList = new List<Project>();
        protected EResScenary _resScenary;
        protected UICtrlOfficalWorkShop.EOfficalMenu _menu;
        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        protected List<int> RemoveProjectList = new List<int>();
        protected List<Project> AddProjectList = new List<Project>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
            base.Close();
        }

        public virtual void RequestData(bool append = false)
        {
        }

        public virtual void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            if (_projectList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }

            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _projectList.Count);
            for (int i = 0; i < _projectList.Count; i++)
            {
                if (!_dict.ContainsKey(_projectList[i].ProjectId))
                {
                    CardDataRendererWrapper<Project> w =
                        new CardDataRendererWrapper<Project>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    if (_dict.ContainsKey(_projectList[i].ProjectId))
                    {
                    }
                    else
                    {
                        _dict.Add(_projectList[i].ProjectId, w);
                    }
                }
            }

            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        protected virtual void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (item != null && item.Content != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
            }
        }

        protected abstract IDataItemRenderer GetItemRenderer(RectTransform parent);

        protected virtual void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
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

        public void SetMenu(UICtrlOfficalWorkShop.EOfficalMenu menu)
        {
            _menu = menu;
        }

        public void Clear()
        {
            _contentList.Clear();
            _dict.Clear();
            _projectList = null;
        }

        public void OnSelectBtn(int index)
        {
            RemoveProjectList.Add(index);
        }

        public void OnUnSelecttBtn(int index)
        {
            RemoveProjectList.Remove(index);
        }

        public void OnAddSelectBtn(Project project)
        {
            AddProjectList.Add(project);
        }

        public void OnAddUnSelecttBtn(Project project)
        {
            AddProjectList.Remove(project);
        }

        public virtual void OnConfirmAddBtn()
        {
        }
    }
}