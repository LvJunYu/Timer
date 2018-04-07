using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UPCtrlWorldOfficalSelectGM : UPCtrlWorldProjectBase
    {
        private WorldOfficialRecommendPrepareProjectList _data;
        private List<USCtrlTimePick> _timepickList = new List<USCtrlTimePick>();
        private GmOfficialRecommendProjectPublishTime PublishTime = new GmOfficialRecommendProjectPublishTime();

        protected new readonly List<CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>
            _contentList = new List<CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>>();

        protected new List<WorldOfficialRecommendPrepareProject> _projectList;
        public List<long> RemoveProjectList = new List<long>();

        private List<Msg_SortOfficialRecommendProjectPrepareItem> _sortList =
            new List<Msg_SortOfficialRecommendProjectPrepareItem>();

        private List<int> _PublisheTime = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            for (int i = 0; i < _cachedView.TimePick.Length; i++)
            {
                USCtrlTimePick timePick = new USCtrlTimePick();
                timePick.Init(_cachedView.TimePick[i]);
                timePick.SetType((TimeType) i,
                    RefreshTimePick);
                _timepickList.Add(timePick);
            }

            _cachedView.EditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.RemoveBtn.onClick.AddListener(OnRemoveBtn);
            _cachedView.CancelRemoveBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.ConfirmEffectBtn.onClick.AddListener(OnPublishTimeBtn);
            _cachedView.OpenCandiateBtn.onClick.AddListener(OpenCandiatePannel);
            _cachedView.ImmediatelyConfirmBtn.onClick.AddListener(OnImmediatelyConfirm);
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
            _data = AppData.Instance.WorldData.WorldOfficialPrepareProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _isRequesting = true;
            _data.Request(startInx, _pageSize, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }

                _isRequesting = false;
            }, code =>
            {
                _isRequesting = false;
                LogHelper.Error("WorldNewestProjectList Request fail, code = {0}", code);
            });
        }

        private void RequestTime()
        {
            _cachedView.ReleaseTimeObj.SetActiveEx(false);
            _cachedView.ReleaseTimeText.SetActiveEx(false);
            PublishTime.Request(0, () =>
            {
                DateTime dateTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(PublishTime.PublishTime);
                if (LocalUser.Instance.User.RoleType == (int) EAccountRoleType.AcRT_Admin)
                {
                    _cachedView.ReleaseTimeObj.SetActiveEx(true);
                    _cachedView.ReleaseTimeText.SetActiveEx(false);
                }
                else
                {
                    _cachedView.ReleaseTimeObj.SetActiveEx(false);
                    _cachedView.ReleaseTimeText.SetActiveEx(true);
                    _cachedView.ReleaseTimeText.text = dateTime.ToString();
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
                item.Index = inx;
                UMCtrlProjectGMRecommend setItem = item as UMCtrlProjectGMRecommend;
                if (setItem != null) setItem.SetScrollRect(_cachedView.GridDataScrollers[(int) _menu]);
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
            var item = new UMCtrlProjectGMRecommend();
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
                    CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> w =
                        new CardDataRendererWrapper<WorldOfficialRecommendPrepareProject>(_projectList[i], OnItemClick);
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

        protected void OnItemClick(CardDataRendererWrapper<WorldOfficialRecommendPrepareProject> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }

            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content.ProjectData);
            SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().RefreshGMProject(item.Content.Status);
        }

        protected override void OnEndDragEvent(PointerEventData arg0)
        {
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
            RemoteCommands.GmUpdateProjectOfficialRecommendStatus(RemoveProjectList,
                EOfficialRecommendProjectStatus.EORPS_Candidate,
                ret =>
                {
                    SocialGUIManager.ShowPopupDialog("待选成功！");
                    RequestData();
                    RefreshBtn(true);
                },
                code => { SocialGUIManager.ShowPopupDialog("待选失败"); });


            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
        }

        private void OnCancelBtn()
        {
            RefreshBtn(true);
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
        }

        private void RefreshBtn(bool isEdit)
        {
            _cachedView.EditBtn.SetActiveEx(isEdit);
            _cachedView.CancelRemoveBtn.SetActiveEx(!isEdit);
            _cachedView.RemoveBtn.SetActiveEx(!isEdit);
        }

        private void OnPublishTimeBtn()
        {
            RemoteCommands.GmPublishOfficialRecommendProject(
                DateTimeUtil.DateTimeToUnixTimestampMillis(OfficalData.GetTimeByItems(_PublisheTime)),
                ret =>
                {
                    RequestTime();
                    SocialGUIManager.ShowPopupDialog("确定发布时间成功");
                },
                code => { SocialGUIManager.ShowPopupDialog("确定发布时间失败"); });
        }

        public void OnUmProjectDragEnd(int oldIndex, int newIndex)
        {
            _sortList.Clear();
            if (oldIndex < newIndex)
            {
                for (int i = oldIndex; i < newIndex; i++)
                {
                    Msg_SortOfficialRecommendProjectPrepareItem
                        sortItem = new Msg_SortOfficialRecommendProjectPrepareItem();
                    sortItem.SlotInx = i;
                    sortItem.OldProjectMainId = _projectList[i].ProjectData.MainId;
                    sortItem.NewProjectMainId = _projectList[i + 1].ProjectData.MainId;
                    _sortList.Add(sortItem);
                }

                {
                    Msg_SortOfficialRecommendProjectPrepareItem
                        sortItem = new Msg_SortOfficialRecommendProjectPrepareItem();
                    sortItem.SlotInx = newIndex;
                    sortItem.OldProjectMainId = _projectList[newIndex].ProjectData.MainId;
                    sortItem.NewProjectMainId = _projectList[oldIndex].ProjectData.MainId;
                    _sortList.Add(sortItem);
                }
            }

            if (oldIndex > newIndex)
            {
                for (int i = newIndex + 1; i <= oldIndex; i++)
                {
                    Msg_SortOfficialRecommendProjectPrepareItem
                        sortItem = new Msg_SortOfficialRecommendProjectPrepareItem();
                    sortItem.SlotInx = i;
                    sortItem.OldProjectMainId = _projectList[i].ProjectData.MainId;
                    sortItem.NewProjectMainId = _projectList[i - 1].ProjectData.MainId;
                    _sortList.Add(sortItem);
                }

                {
                    Msg_SortOfficialRecommendProjectPrepareItem
                        sortItem = new Msg_SortOfficialRecommendProjectPrepareItem();
                    sortItem.SlotInx = newIndex;
                    sortItem.OldProjectMainId = _projectList[newIndex].ProjectData.MainId;
                    sortItem.NewProjectMainId = _projectList[oldIndex].ProjectData.MainId;
                    _sortList.Add(sortItem);
                }
            }

            RemoteCommands.SortOfficialRecommendProjectPrepare(_sortList, ret => { RequestData(); }, code => { });
            _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
            RefreshView();
        }

        private void OpenCandiatePannel()
        {
            _mainCtrl.UpCtrlWorldCandidate.Open();
        }

        private void OnImmediatelyConfirm()
        {
            RemoteCommands.GmPublishOfficialRecommendProject(
                0,
                ret =>
                {
                    RequestTime();
                    SocialGUIManager.ShowPopupDialog("确定发布时间成功");
                },
                code => { SocialGUIManager.ShowPopupDialog("确定发布时间失败"); });
        }
    }
}