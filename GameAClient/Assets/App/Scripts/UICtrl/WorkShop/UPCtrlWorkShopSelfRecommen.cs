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
        private List<UserSelfRecommendProject> _removeList = new List<UserSelfRecommendProject>();
        private List<Table_WorkShopNumberOfSlot> _allSoltList = new List<Table_WorkShopNumberOfSlot>();
        private int _maxSoltNum;
        private bool _isRequesting;
        private UserSelfRecommendProjectList _data = LocalUser.Instance.UserSelfRecommendProjectData;
        private List<UserSelfRecommendProject> _dataList = LocalUser.Instance.UserSelfRecommendProjectList;

        protected new List<CardDataRendererWrapper<UserSelfRecommendProject>> _contentList =
            new List<CardDataRendererWrapper<UserSelfRecommendProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<UserSelfRecommendProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<UserSelfRecommendProject>>();

        private int _startInx;

        protected override void OnViewCreated()
        {
            foreach (var soltData in TableManager.Instance.Table_WorkShopNumberOfSlotDic)
            {
                _allSoltList.Add(soltData.Value);
            }

            _allSoltList.Sort((a, b) => { return b.Num - a.Num; });
            _maxSoltNum = _allSoltList[0].Num;
            base.OnViewCreated();
            _cachedView.SelfRecommendEditBtn.onClick.AddListener(ShowRemoveBtn);
            _cachedView.CancelBtn.onClick.AddListener(ShowEditBtn);
            _cachedView.RemoveBtn.onClick.AddListener(OnRemoveBtn);
        }

        public override void Open()
        {
            _isOpen = true;
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            _cachedView.EmptyObj.SetActiveEx(false);
            RequestData();
            ShowEditBtn();
        }

        public override void RequestData(bool append = false)
        {
            if (!_mainCtrl.SelfRecommendDirty && _hasRequested && !append || _isRequesting) return;
            _isRequesting = true;
            if (append)
            {
                _startInx = _dataList.Count;
            }

            RequestNewData();
        }

        private void RequestNewData()
        {
            ParallelTaskHelper<ENetResultCode> helper = new ParallelTaskHelper<ENetResultCode>(() =>
            {
                AppendProjectList(_dataList, _data);
                if (_isOpen)
                {
                    RefreshView();
                }

                _mainCtrl.SelfRecommendDirty = false;
                _hasRequested = true;
                _isRequesting = false;
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！");
                _isRequesting = false;
            });
            helper.AddTask(LoadDataList);
            helper.AddTask(LoadUserData);
        }

        private void LoadDataList(Action successcallback, Action<ENetResultCode> failcallback)
        {
            _data.Request(LocalUser.Instance.UserGuid, _startInx, _pageSize, successcallback, failcallback);
        }

        private void LoadUserData(Action successcallback, Action<ENetResultCode> failcallback)
        {
            LocalUser.Instance.UserSelfRecommendProjectStatistic.Request(LocalUser.Instance.UserGuid, successcallback,
                failcallback);
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
            for (int i = 0;
                i < _dataList.Count;
                i++)
            {
                if (!_dict.ContainsKey(_dataList[i].SlotInx))
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
                item.Index = inx;
            }
        }


//点击
        private void OnItemClick(CardDataRendererWrapper<UserSelfRecommendProject> obj)
        {
        }

//获取item
        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectSelfCommend();
            item.Init(parent, _resScenary);
            item.SetScrollRect(_cachedView.GridDataScrollers[(int) _menu]);
            return item;
        }

        public override void Close()
        {
            AddMsgOprate();
            if (_mainCtrl.SortItemList.Count > 0)
            {
                RemoteCommands.SortSelfRecommendProject(_mainCtrl.SortItemList, ret => { RequestNewData(); },
                    code => { });
            }

            _isOpen = false;
            _cachedView.Pannels[(int) _menu].SetActiveEx(false);
        }

        private void AppendProjectList(List<UserSelfRecommendProject> projectList, UserSelfRecommendProjectList data)
        {
            List<UserSelfRecommendProject> removeErroList = new List<UserSelfRecommendProject>();
            for (int i = 0; i < data.ProjectList.Count; i++)
            {
                if (data.ProjectList[i].SlotInx >= LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount)
                {
                    removeErroList.Add(data.ProjectList[i]);
                }
            }

            projectList.Clear();
            for (int i = 0; i < data.ProjectList.Count; i++)
            {
                if (projectList.Count > data.ProjectList[i].SlotInx)
                {
                    UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                    userSelfRecommendProject.SlotInx = data.ProjectList[i].SlotInx;
                    Project p = new Project();
                    p.DeepCopy(data.ProjectList[i].ProjectData);
                    userSelfRecommendProject.ProjectData = p;
                    projectList[data.ProjectList[i].SlotInx] = userSelfRecommendProject;
                }
                else
                {
                    int addnum = data.ProjectList[i].SlotInx - projectList.Count;
                    int oldnum = projectList.Count;
                    for (int j = 0; j < addnum; j++)
                    {
                        UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                        userSelfRecommendProject.SlotInx = oldnum + j;
                        userSelfRecommendProject.ProjectData = null;
                        projectList.Add(userSelfRecommendProject);
                    }

                    UserSelfRecommendProject userSelfRecommendProjectdeep = new UserSelfRecommendProject();
                    userSelfRecommendProjectdeep.SlotInx = data.ProjectList[i].SlotInx;
                    Project p = new Project();
                    p.DeepCopy(data.ProjectList[i].ProjectData);
                    userSelfRecommendProjectdeep.ProjectData = p;
                    projectList.Add(userSelfRecommendProjectdeep);
                }
            }

            if (projectList.Count < LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount)
            {
                int addnum = LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount - projectList.Count;
                int oldnum = projectList.Count;
                for (int j = 0; j < addnum; j++)
                {
                    UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                    userSelfRecommendProject.SlotInx = oldnum + j;
                    userSelfRecommendProject.ProjectData = null;
                    projectList.Add(userSelfRecommendProject);
                }
            }

            if (projectList.Count < _maxSoltNum)
            {
                int addnum = _maxSoltNum - projectList.Count;
                int oldnum = projectList.Count;
                for (int j = 0; j < addnum; j++)
                {
                    UserSelfRecommendProject userSelfRecommendProject = new UserSelfRecommendProject();
                    userSelfRecommendProject.SlotInx = oldnum + j;
                    userSelfRecommendProject.ProjectData = null;
                    projectList.Add(userSelfRecommendProject);
                }
            }

            List<Msg_SelfRecommendProjectOperateItem> _list = new List<Msg_SelfRecommendProjectOperateItem>();
            for (int i = 0; i < removeErroList.Count; i++)
            {
                Msg_SelfRecommendProjectOperateItem selfRecommendProject = new Msg_SelfRecommendProjectOperateItem();
                selfRecommendProject.ProjectMainId = removeErroList[i].ProjectData.MainId;
                selfRecommendProject.SlotInx = removeErroList[i].SlotInx;
                _list.Add(selfRecommendProject);
            }

            if (removeErroList.Count > 0)
            {
                RemoteCommands.RemoveSelfRecommendProject(_mainCtrl.SortItemList, _list,
                    ret =>
                    {
                        _removeList.Clear();
                        _mainCtrl.SelfRecommendDirty = true;
                        RequestData();
                    },
                    code => { SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！"); });
            }
        }

        private void ShowEditBtn()
        {
            _cachedView.CancelBtn.SetActiveEx(false);
            _cachedView.RemoveBtn.SetActiveEx(false);
            _cachedView.SelfRecommendEditBtn.SetActiveEx(true);
            _cachedView.TipObj.SetActive(true);
        }

        private void ShowRemoveBtn()
        {
            _cachedView.CancelBtn.SetActiveEx(true);
            _cachedView.RemoveBtn.SetActiveEx(true);
            _cachedView.SelfRecommendEditBtn.SetActiveEx(false);
            _cachedView.TipObj.SetActive(false);
            _removeList.Clear();
            Messenger.Broadcast(EMessengerType.OnWorkShopSelfRecommendEditBtn);
        }

        private void OnRemoveBtn()
        {
            AddMsgOprate();
            ShowEditBtn();
            List<Msg_SelfRecommendProjectOperateItem> _list = new List<Msg_SelfRecommendProjectOperateItem>();
            for (int i = 0; i < _removeList.Count; i++)
            {
                Msg_SelfRecommendProjectOperateItem selfRecommendProject = new Msg_SelfRecommendProjectOperateItem();
                selfRecommendProject.ProjectMainId = _removeList[i].ProjectData.MainId;
                selfRecommendProject.SlotInx = _removeList[i].SlotInx;
                _list.Add(selfRecommendProject);
            }

            if (_removeList.Count > 0)
            {
                RemoteCommands.RemoveSelfRecommendProject(_mainCtrl.SortItemList, _list,
                    ret =>
                    {
                        _removeList.Clear();
                        _mainCtrl.SelfRecommendDirty = true;
                        RequestData();
                    },
                    code => { SocialGUIManager.ShowPopupDialog("服务器繁忙，请稍后再试！"); });
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        //选中关卡后的响应
        public bool OnSelectProject(UserSelfRecommendProject project)
        {
            bool canadd = false;
            if (_isOpen)
            {
                _removeList.Add(project);
                canadd = true;
            }

            return canadd;
        }

        public bool OnUnSelectProject(UserSelfRecommendProject project)
        {
            bool canremove = false;
            if (_isOpen)
            {
                if (_removeList.Remove(project))
                {
                    canremove = true;
                }
            }

            return canremove;
        }

        public void OnUmProjectDragEnd(int oldIndex, int newIndex)
        {
            AddMsgOprate();
            _mainCtrl.SelfRecommendDirty = true;
            if (oldIndex < newIndex)
            {
                UserSelfRecommendProject project = new UserSelfRecommendProject();
                project.SlotInx = _dataList[oldIndex].SlotInx;
                project.ProjectData = _dataList[oldIndex].ProjectData;
                for (int i = oldIndex; i < newIndex; i++)
                {
                    _dataList[i].ProjectData = _dataList[i + 1].ProjectData;
                }

                _dataList[newIndex].ProjectData = project.ProjectData;
            }

            if (oldIndex > newIndex)
            {
                UserSelfRecommendProject project = new UserSelfRecommendProject();
                project.SlotInx = _dataList[oldIndex].SlotInx;
                project.ProjectData = _dataList[oldIndex].ProjectData;
                for (int i = oldIndex; i > newIndex; i--)
                {
                    _dataList[i].ProjectData = _dataList[i - 1].ProjectData;
                }

                _dataList[newIndex].ProjectData = project.ProjectData;
            }

            _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
            RefreshView();
            AddMsgOprate();
        }

        public void AddMsgOprate()
        {
            _mainCtrl.SortItemList.Clear();
            List<UserSelfRecommendProject> tempList = new List<UserSelfRecommendProject>();
            AppendProjectList(tempList, _data);

            for (int i = 0; i < LocalUser.Instance.UserSelfRecommendProjectStatistic.TotalCount; i++)
            {
                Msg_SortSelfRecommendProjectItem msgItem = compareSelfProject(_dataList[i], tempList[i]);
                if (msgItem.SlotInx == 0)
                {
                    msgItem.SlotInx = i;
                    _mainCtrl.SortItemList.Add(msgItem);
                }
            }
        }

        private Msg_SortSelfRecommendProjectItem compareSelfProject(UserSelfRecommendProject newp,
            UserSelfRecommendProject oldp)
        {
            Msg_SortSelfRecommendProjectItem msgItem = new Msg_SortSelfRecommendProjectItem();
            msgItem.SlotInx = -1;
            if (oldp.ProjectData == null)
            {
                if (newp.ProjectData != null)
                {
                    msgItem.NewProjectMainId = newp.ProjectData.MainId;
                    msgItem.OldProjectMainId = 0;
                    msgItem.SlotInx = 0;
                }
            }
            else
            {
                if (newp.ProjectData == null)
                {
                    msgItem.NewProjectMainId = 0;
                    msgItem.OldProjectMainId = oldp.ProjectData.MainId;
                    msgItem.SlotInx = 0;
                }
                else
                {
                    if (newp.ProjectData.MainId != oldp.ProjectData.MainId)
                    {
                        msgItem.NewProjectMainId = newp.ProjectData.MainId;
                        msgItem.OldProjectMainId = oldp.ProjectData.MainId;
                        msgItem.SlotInx = 0;
                    }
                }
            }

            return msgItem;
        }
    }
}