using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopAddSelfRecommend : UPCtrlBase<UICtrlWorkShop, UIViewWorkShop>,
        IOnChangeHandler<long>
    {
        protected bool _hasRequested;
        protected const int _pageSize = 21;
        protected List<Project> _projectList;
        protected EResScenary _resScenary;
        protected UICtrlWorkShop.EMenu _menu;
        protected List<CardDataRendererWrapper<Project>> _contentList = new List<CardDataRendererWrapper<Project>>();

        protected Dictionary<long, CardDataRendererWrapper<Project>> _dict =
            new Dictionary<long, CardDataRendererWrapper<Project>>();

        private UserPublishedWorldProjectList _data = LocalUser.Instance.UserPublishedWorldProjectList;
        private List<UMCtrlProjectAddSelfCommend> _umSelfCommendsList = new List<UMCtrlProjectAddSelfCommend>();
        private List<Project> _addUserSelfRecommendProjects = new List<Project>();
        private int _lastSoltNum;
        private List<int> _lastSoltList = new List<int>();
        private List<long> _haveAddProject = new List<long>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AddSelfRecommendProjectScroller.Set(OnItemRefresh, GetItemRenderer);
            _cachedView.AddCancelBtn.onClick.AddListener(Close);
            _cachedView.AddConfirmBtn.onClick.AddListener(OnConfirmBtn);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.AddSelfRecommendProjectPanel.SetActiveEx(true);
            RequestData();
            RefreshView();
            _lastSoltList.Clear();
            _lastSoltNum = 0;
            _addUserSelfRecommendProjects.Clear();
            for (int i = 0; i < LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount; i++)
            {
                if (LocalUser.Instance.UserSelfRecommendProjectList[i].ProjectData == null)
                {
                    _lastSoltList.Add(LocalUser.Instance.UserSelfRecommendProjectList[i].SlotInx);
                    _lastSoltNum++;
                }
                else
                {
                    _haveAddProject.Add(LocalUser.Instance.UserSelfRecommendProjectList[i].ProjectData.MainId);
                }
            }
        }

        public override void Close()
        {
            _cachedView.AddSelfRecommendProjectPanel.SetActiveEx(false);
            base.Close();
        }

        public virtual void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = LocalUser.Instance.UserPublishedWorldProjectList.AllList.Count;
            }

            _data.Requset(startInx, _pageSize, () =>
            {
                _projectList = _data.AllList.FindAll(p => { return !_haveAddProject.Contains(p.MainId); });
                if (_isOpen)
                {
                    RefreshView();
                }
            }, null);
        }

        public virtual void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_projectList == null || _projectList.Count == 0);
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

        protected virtual void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (item != null && item.Content != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
            }
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectAddSelfCommend();
            item.Init(parent, _resScenary);
            _umSelfCommendsList.Add(item);
            return item;
        }

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

        public void UpdateUmLastTime()
        {
            if (!_isOpen)
            {
                return;
            }

            for (int i = 0; i < _umSelfCommendsList.Count; i++)
            {
                _umSelfCommendsList[i].UpdateLastTime();
            }
        }

        public void Clear()
        {
            _contentList.Clear();
            _dict.Clear();
            _projectList = null;
        }

        public bool AddProject(Project project)
        {
            bool canadd = false;

            if (_addUserSelfRecommendProjects.Count < _lastSoltNum)
            {
                _addUserSelfRecommendProjects.Add(project);
                canadd = true;
            }

            return canadd;
        }

        public bool RemoveProject(Project project)
        {
            bool remove = _addUserSelfRecommendProjects.Remove(project);
            return remove;
        }

        private void OnConfirmBtn()
        {
            _mainCtrl.GetuUpCtrlWorkShopSelfRecommen().AddMsgOprate();
            List<Msg_SelfRecommendProjectOperateItem> _list = new List<Msg_SelfRecommendProjectOperateItem>();
            _addUserSelfRecommendProjects.Sort((a, b) => { return (int) (a.LastDirtyTime - b.LastDirtyTime); });

            for (int i = 0; i < _addUserSelfRecommendProjects.Count; i++)
            {
                Msg_SelfRecommendProjectOperateItem selfRecommendProject = new Msg_SelfRecommendProjectOperateItem();
                selfRecommendProject.ProjectMainId = _addUserSelfRecommendProjects[i].MainId;
                selfRecommendProject.SlotInx = _lastSoltList[i];
                _list.Add(selfRecommendProject);
            }

            RemoteCommands.AddSelfRecommendProject(_mainCtrl.SortItemList,
                _list,
                ret =>
                {
                    _mainCtrl.SortItemList.Clear();
                    _mainCtrl.SelfRecommendDirty = true;
                    _mainCtrl.RefreshSelfRecommend();
                    SocialGUIManager.ShowPopupDialog("添加成功！");
                },
                code => { SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！"); });


            Close();
        }
    }
}