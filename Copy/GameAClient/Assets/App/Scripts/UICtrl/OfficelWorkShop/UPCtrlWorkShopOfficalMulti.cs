using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopOfficalMulti : UPCtrlWorkShopOfficialProjectBase
    {
        private int MaxMultiCout = 50;
        private GmNetProjectPrepareList _data;
        private List<USCtrlTimePick> _timepickList = new List<USCtrlTimePick>();
        private GmNetProjectPublishTime PublishTime = new GmNetProjectPublishTime();

        protected new readonly List<CardDataRendererWrapper<GmPrepareProject>>
            _contentList = new List<CardDataRendererWrapper<GmPrepareProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>>();

        protected new List<GmPrepareProject> _projectList;


        private List<int> _PublisheTime = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.MultiPublishBtn.onClick.AddListener(OnPublishTimeBtn);
            _cachedView.MultiUploadBtn.onClick.AddListener(OnStoryUpLoadBtn);
            _cachedView.MultiEditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.MultiRemoveBtn.onClick.AddListener(OnRemoveBtn);
            _cachedView.MultiCancelRemoveBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.MultiAddProjectBtn.onClick.AddListener(OnAddBtn);
            for (int i = 0; i < _cachedView.MultiTimePick.Length; i++)
            {
                USCtrlTimePick timePick = new USCtrlTimePick();
                timePick.Init(_cachedView.MultiTimePick[i]);
                timePick.SetType((TimeType) i,
                    RefreshTimePick);
                _timepickList.Add(timePick);
            }
        }

        public override void Open()
        {
            base.Open();
            RefreshBtn(true);
            RemoveProjectList.Clear();
            RequestTime();
        }

        public override void RequestData(bool append = false)
        {
            base.RequestData(append);
            _data = OfficalData.Instance.GmNetProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _data.AllList.Count;
            }

            _data.Request(startInx, MaxMultiCout, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { LogHelper.Error("GmNetProjectPrepareList Request fail, code = {0}", code); });
        }

        private void RequestTime()
        {
            _cachedView.MultiReleaseTimeObj.SetActiveEx(false);
            _cachedView.MultiReleaseTimeText.SetActiveEx(false);
            PublishTime.Request(0, () =>
            {
                DateTime dateTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(PublishTime.PublishTime);
                if (LocalUser.Instance.User.RoleType == (int) EAccountRoleType.AcRT_Admin)
                {
                    _cachedView.MultiReleaseTimeObj.SetActiveEx(true);
                    _cachedView.MultiReleaseTimeText.SetActiveEx(false);
                }
                else
                {
                    _cachedView.MultiReleaseTimeObj.SetActiveEx(false);
                    _cachedView.MultiReleaseTimeText.SetActiveEx(true);
                    _cachedView.MultiReleaseTimeText.text = dateTime.ToString();
                }

                if (dateTime < DateTime.Now)
                {
                    dateTime = DateTime.Now.AddHours(2);
                }

                _PublisheTime = OfficalData.SettimesItems(dateTime);
                for (int i = 0; i < _timepickList.Count; i++)
                {
                    _timepickList[i].SetOrgTime(_PublisheTime);
                }
            }, code => { });
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
                UMCtrlProjectGMWrokShop um = item as UMCtrlProjectGMWrokShop;
                if (um != null) um.EnableDrag(false);
                if (!_data.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectGMWrokShop();
            item.Init(parent, _resScenary);
            return item;
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
                if (!_dict.ContainsKey(_projectList[i].Id))
                {
                    CardDataRendererWrapper<GmPrepareProject> w =
                        new CardDataRendererWrapper<GmPrepareProject>(_projectList[i], OnItemClick);
                    _contentList.Add(w);
                    if (_dict.ContainsKey(_projectList[i].Id))
                    {
                    }
                    else
                    {
                        _dict.Add(_projectList[i].Id, w);
                    }
                }
            }

            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
        }

        protected void OnItemClick(CardDataRendererWrapper<GmPrepareProject> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }

            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content.ProjectData);
//            SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().RefreshGMProject(item.Content.Status);
        }

        private void RefreshTimePick()
        {
            for (int i = 0; i < _timepickList.Count; i++)
            {
                _timepickList[i].RefreshTime();
            }
        }

        private void OnEditBtn()
        {
            RefreshBtn(false);
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendEditBtn);
        }

        private void OnRemoveBtn()
        {
            if (_data.AllList.Count > MaxMultiCout)
            {
                LogHelper.Error("多人关卡的数目超过了预定的最大数量50");
            }
            else
            {
                List<Msg_GmUpdateNetProjectPrepare> _updateList = new List<Msg_GmUpdateNetProjectPrepare>();
                for (int i = 0; i < RemoveProjectList.Count; i++)
                {
                    Msg_GmUpdateNetProjectPrepare item = new Msg_GmUpdateNetProjectPrepare();
                    item.SlotInx = RemoveProjectList[i];
                    item.ProjectId = 0;
                    _updateList.Add(item);
                }

                RemoteCommands.GmUpdateNetProjectPrepare(_updateList,
                    ret =>
                    {
                        SocialGUIManager.ShowPopupDialog("待选成功！");
                        RequestData();
                    },
                    code => { SocialGUIManager.ShowPopupDialog("待选失败"); });
                RefreshBtn(true);
                Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
            }

            RemoveProjectList.Clear();
        }

        private void OnCancelBtn()
        {
            RefreshBtn(true);
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
            RemoveProjectList.Clear();
        }

        private void RefreshBtn(bool isEdit)
        {
            _cachedView.MultiEditBtn.SetActiveEx(isEdit);
            _cachedView.MultiRemoveBtn.SetActiveEx(!isEdit);
            _cachedView.MultiCancelRemoveBtn.SetActiveEx(!isEdit);
        }

        private void OnPublishTimeBtn()
        {
            RemoteCommands.GmPublishNetProject(
                DateTimeUtil.DateTimeToUnixTimestampMillis(OfficalData.GetTimeByItems(_PublisheTime)), ret => { },
                code => { });
        }

        private void OnStoryUpLoadBtn()
        {
            RemoteCommands.GmUploadNetProject(
                0, ret => { },
                code => { });
        }

        private void OnAddBtn()
        {
            HashSet<long> idSet = new HashSet<long>();
            for (int i = 0; i < _projectList.Count; i++)
            {
                idSet.Add(_projectList[i].ProjectData.ProjectId);
            }

            _mainCtrl.OpenAddSelfRecommendPanel(Project.ProjectTypeAllMulti, idSet);
        }

        public override void OnConfirmAddBtn()
        {
            List<Msg_GmUpdateNetProjectPrepare> _updateList = new List<Msg_GmUpdateNetProjectPrepare>();
            for (int i = 0; i < _data.AllList.Count; i++)
            {
                Msg_GmUpdateNetProjectPrepare item = new Msg_GmUpdateNetProjectPrepare();
                item.SlotInx = i;
                item.ProjectId = _data.AllList[i].ProjectData.ProjectId;
                _updateList.Add(item);
            }

            for (int i = 0; i < AddProjectList.Count; i++)
            {
                Msg_GmUpdateNetProjectPrepare item = new Msg_GmUpdateNetProjectPrepare();
                item.SlotInx = _updateList.Count + i;
                item.ProjectId = AddProjectList[i].ProjectId;
                _updateList.Add(item);
            }

            RemoteCommands.GmUpdateNetProjectPrepare(_updateList,
                ret =>
                {
                    if (ret.ResultCode == (int) ENetResultCode.NR_Success)
                    {
                        RequestData();
                        SocialGUIManager.ShowPopupDialog("添加成功！");
                    }
                },
                code => { SocialGUIManager.ShowPopupDialog("添加失败"); });
            RefreshBtn(true);
            _updateList.Clear();
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
        }
    }
}