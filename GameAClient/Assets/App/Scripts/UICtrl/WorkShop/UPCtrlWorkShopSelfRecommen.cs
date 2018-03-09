using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopSelfRecommen : UPCtrlWorkShopProjectBase
    {
        private List<Table_WorkShopNumberOfSlot> _allSoltList = new List<Table_WorkShopNumberOfSlot>();
        private int _maxSoltNum;
        private bool _isRequesting;
        private UserSelfRecommendProjectList _data = LocalUser.Instance.UserSelfRecommendProjectData;
        private List<UserSelfRecommendProject> _dataList = LocalUser.Instance.UserSelfRecommendProjectList;

        protected List<CardDataRendererWrapper<UserSelfRecommendProject>> _contentList =
            new List<CardDataRendererWrapper<UserSelfRecommendProject>>();

        protected Dictionary<long, CardDataRendererWrapper<UserSelfRecommendProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<UserSelfRecommendProject>>();

        private readonly List<Msg_SortSelfRecommendProjectItem> sortItemList =
            new List<Msg_SortSelfRecommendProjectItem>();

        protected override void OnViewCreated()
        {
            foreach (var soltData in TableManager.Instance.Table_WorkShopNumberOfSlotDic)
            {
                _allSoltList.Add(soltData.Value);
            }

            _allSoltList.Sort((a, b) => { return a.Num - b.Num; });
            _maxSoltNum = _allSoltList[0].Num;
            base.OnViewCreated();
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void RequestData(bool append = false)
        {
            if (_hasRequested && !append || _isRequesting) return;
            _isRequesting = true;
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }

            _data.Request(LocalUser.Instance.UserGuid, startInx, _pageSize, () =>
                {
                    AppendProjectList(_dataList, _data);
                    if (_isOpen)
                    {
                        RefreshView();
                    }

                    _hasRequested = true;
                    _isRequesting = false;
                }, code =>
                {
                    SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！");
                    _isRequesting = false;
                }
            );
        }

        public override void RefreshView()
        {
            _cachedView.EmptyObj.SetActiveEx(_dataList == null || _dataList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            if (_dataList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }

            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                if (!_dict.ContainsKey(_projectList[i].ProjectId))
                {
                    CardDataRendererWrapper<UserSelfRecommendProject> w =
                        new CardDataRendererWrapper<UserSelfRecommendProject>(_dataList[i], OnItemClick);
                    _contentList.Add(w);
                    if (_dict.ContainsKey(_dataList[i].SlotInx))
                    {
                    }
                    else
                    {
                        _dict.Add(_dataList[i].SlotInx, w);
                    }
                }
            }

            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
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
            }
        }

//点击
        private void OnItemClick(CardDataRendererWrapper<UserSelfRecommendProject> obj)
        {
        }

//获取item
        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlSoltProject();
            item.Init(parent, _resScenary);
            item.SetCurUI(UMCtrlProject.ECurUI.Editing);
            return item;
        }

        public override void Close()
        {
            _isOpen = false;
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
        }

        private void AppendProjectList(List<UserSelfRecommendProject> projectList, UserSelfRecommendProjectList data)
        {
            List<UserSelfRecommendProject> removeList = new List<UserSelfRecommendProject>();
            for (int i = 0; i < data.ProjectList.Count; i++)
            {
                if (data.ProjectList[i].SlotInx >= LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount)
                {
                    data.ProjectList.RemoveAt(i);
                    removeList.Add(data.ProjectList[i]);
                }
            }

            for (int i = 0; i < data.ProjectList.Count; i++)
            {
                if (projectList.Count > data.ProjectList[i].SlotInx)
                {
                    projectList[data.ProjectList[i].SlotInx] = data.ProjectList[i];
                }
                else
                {
                    int addnum = data.ProjectList[i].SlotInx - projectList.Count;
                    for (int j = 0; j < addnum; j++)
                    {
                        UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                        userSelfRecommendProject.SlotInx = projectList.Count + j;
                        userSelfRecommendProject.ProjectData = new Project();
                        projectList.Add(new UserSelfRecommendProject());
                    }

                    projectList.Add(data.ProjectList[i]);
                }
            }

            if (projectList.Count < LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount)
            {
                int addnum = LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount - projectList.Count;
                for (int j = 0; j < addnum; j++)
                {
                    UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                    userSelfRecommendProject.SlotInx = projectList.Count + j;
                    userSelfRecommendProject.ProjectData = new Project();
                    projectList.Add(userSelfRecommendProject);
                }
            }

            if (projectList.Count < _maxSoltNum)
            {
                int addnum = _maxSoltNum - projectList.Count;
                for (int j = 0; j < addnum; j++)
                {
                    UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                    userSelfRecommendProject.SlotInx = projectList.Count + j;
                    userSelfRecommendProject.ProjectData = null;
                    projectList.Add(userSelfRecommendProject);
                }
            }

            for (int i = 0; i < removeList.Count; i++)
            {
                RemoteCommands.RemoveSelfRecommendProject(sortItemList,
                    removeList[i].ProjectData.ProjectId,
                    removeList[i].SlotInx,
                    ret => { removeList.RemoveAt(i); },
                    code => { SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！"); });
            }
        }
    }
}