using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlOfficalWorkShopAddProject : UPCtrlWorkShopOfficialProjectBase
    {
        private GmMasterProjectList _data = OfficalData.Instance.GmProjectList;
        private int _type;
        private HashSet<long> _IdSet = new HashSet<long>();

        protected override void OnViewCreated()
        {
            _cachedView.AddSelfRecommendProjectScroller.Set(OnItemRefresh, GetItemRenderer);
            _cachedView.AddCancelBtn.onClick.AddListener(Close);
            _cachedView.AddConfirmBtn.onClick.AddListener(OnConfirmBtn);
        }

        public override void Open()
        {
            _isOpen = true;
            _cachedView.AddSelfRecommendProjectPanel.SetActiveEx(true);
            RequestData();
        }

        public void OpenMenu(int type, HashSet<long> idset)
        {
            _type = type;
            _IdSet = idset;
            Open();
        }

        public override void Close()
        {
            _cachedView.AddSelfRecommendProjectPanel.SetActiveEx(false);
            _isOpen = false;
        }

        public override void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = _data.AllList.Count;
            }

            _data.Request(startInx, _pageSize, _type, () =>
            {
                _projectList.Clear();
                for (int i = 0; i < _data.AllList.Count; i++)
                {
                    if (!_IdSet.Contains(_data.AllList[i].ProjectId))
                    {
                        _projectList.Add(_data.AllList[i]);
                    }
                }
                if (_isOpen)
                {
                    RefreshView();
                }
            }, null);
        }

        public override void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(false);
            _contentList.Clear();
            _dict.Clear();
            if (_projectList == null)
            {
                _cachedView.AddSelfRecommendProjectScroller.SetEmpty();
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

            _cachedView.AddSelfRecommendProjectScroller.SetItemCount(_contentList.Count);
        }

        protected override void OnItemClick(CardDataRendererWrapper<Project> item)
        {
//            if (item != null && item.Content != null)
//            {
//                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
//            }
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectAddOfficalProject();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
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
                item.Index = inx;
                if (!_data.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        public new void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<Project> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }

        public new void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }

        public new void Clear()
        {
            _contentList.Clear();
            _dict.Clear();
            _projectList = null;
        }

        private void OnConfirmBtn()
        {
            Close();
            _mainCtrl._curMenuCtrl.OnConfirmAddBtn();
        }
    }
}