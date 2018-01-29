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

        protected UMCtrlProject.ECurUI _eCurUi;
        protected List<Project> _projectList;
        protected bool _isRequesting;
        
        public int Mask {
            get
            {
                int mask = 0;
                for (int i = 0; i < _cachedView.ProjectTypeTogs.Length; i++)
                {
                    if (_cachedView.ProjectTypeTogs[i].isOn)
                    {
                        mask |= 1 << i;
                    }
                }

                if (mask == 0)
                {
                    mask = Project.ProjectTypeAllMask;
                }
                return mask;
            }
        }
        
        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _eCurUi = GetUMCurUI(_menu);
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void RefreshView()
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
            item.SetCurUI(_eCurUi);
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

        public override void RequestData(bool append = false)
        {
        }

        public virtual void OnProjectTypesChanged()
        {
            RequestData();
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
            base.Clear();
            _unload = true;
            _contentList.Clear();
            _dict.Clear();
            _projectList = null;
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
        }

        private UMCtrlProject.ECurUI GetUMCurUI(UICtrlWorld.EMenu menu)
        {
            switch (menu)
            {
                case UICtrlWorld.EMenu.Recommend:
                    return UMCtrlProject.ECurUI.Recommend;
//                case UICtrlWorld.EMenu.MaxScore:
//                    return UMCtrlProject.ECurUI.MaxScore;
                case UICtrlWorld.EMenu.NewestProject:
                    return UMCtrlProject.ECurUI.AllNewestProject;
//                case UICtrlWorld.EMenu.Follows:
//                    return UMCtrlProject.ECurUI.Follows;
                case UICtrlWorld.EMenu.UserFavorite:
                    return UMCtrlProject.ECurUI.UserFavorite;
//                case UICtrlWorld.EMenu.UserPlayHistory:
//                    return UMCtrlProject.ECurUI.UserPlayHistory;
                case UICtrlWorld.EMenu.RankList:
                    return UMCtrlProject.ECurUI.RankList;
                default:
                    return UMCtrlProject.ECurUI.None;
            }
        }
    }
}