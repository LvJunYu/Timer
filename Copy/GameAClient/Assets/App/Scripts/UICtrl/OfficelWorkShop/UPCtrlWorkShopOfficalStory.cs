using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UPCtrlWorkShopOfficalStory : UPCtrlWorkShopOfficialProjectBase
    {
        private int MaxRpgCout = 50;
        private GmRpgProjectPrepareList _data;
        private List<USCtrlTimePick> _timepickList = new List<USCtrlTimePick>();
        private GmRpgProjectPublishTime PublishTime = new GmRpgProjectPublishTime();

        protected new readonly List<CardDataRendererWrapper<GmPrepareProject>>
            _contentList = new List<CardDataRendererWrapper<GmPrepareProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>>();

        protected new List<GmPrepareProject> _projectList;


        private List<int> _PublisheTime = new List<int>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.StoryPublishBtn.onClick.AddListener(OnPublishTimeBtn);
            _cachedView.StoryUploadBtn.onClick.AddListener(OnStoryUpLoadBtn);
            _cachedView.StoryEditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.StoryRemoveBtn.onClick.AddListener(OnRemoveBtn);
            _cachedView.StoryCancelRemoveBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.StoryAddProjectBtn.onClick.AddListener(OnAddBtn);
            for (int i = 0; i < _cachedView.StoryTimePick.Length; i++)
            {
                USCtrlTimePick timePick = new USCtrlTimePick();
                timePick.Init(_cachedView.StoryTimePick[i]);
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
            _data = OfficalData.Instance.GmStoryProjectList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }

            _data.Request(startInx, MaxRpgCout, () =>
            {
                _projectList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { LogHelper.Error("WorldNewestProjectList Request fail, code = {0}", code); });
        }

        private void RequestTime()
        {
            _cachedView.StoryReleaseTimeObj.SetActiveEx(false);
            _cachedView.StoryReleaseTimeText.SetActiveEx(false);
            PublishTime.Request(0, () =>
            {
                DateTime dateTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(PublishTime.PublishTime);
                if (LocalUser.Instance.User.RoleType == (int) EAccountRoleType.AcRT_Admin)
                {
                    _cachedView.StoryReleaseTimeObj.SetActiveEx(true);
                    _cachedView.StoryReleaseTimeText.SetActiveEx(false);
                }
                else
                {
                    _cachedView.StoryReleaseTimeObj.SetActiveEx(false);
                    _cachedView.StoryReleaseTimeText.SetActiveEx(true);
                    _cachedView.StoryReleaseTimeText.text = dateTime.ToString();
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
            if (_data.AllList.Count > MaxRpgCout)
            {
                LogHelper.Error("剧情关卡的数目超过了预定的最大数量50");
            }
            else
            {
                List<Msg_GmUpdateRpgProjectPrepare> _updateList = new List<Msg_GmUpdateRpgProjectPrepare>();
                for (int i = 0; i < RemoveProjectList.Count; i++)
                {
                    Msg_GmUpdateRpgProjectPrepare item = new Msg_GmUpdateRpgProjectPrepare();
                    item.SlotInx = RemoveProjectList[i];
                    item.ProjectId = 0;
                    _updateList.Add(item);
                }

                if (_updateList.Count > 0)
                {
                    RemoteCommands.GmUpdateRpgProjectPrepare(_updateList,
                        ret =>
                        {
                            SocialGUIManager.ShowPopupDialog("移除成功！");
                            RequestData();
                        },
                        code => { SocialGUIManager.ShowPopupDialog("移除失败"); });
                }

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
            _cachedView.StoryEditBtn.SetActiveEx(isEdit);
            _cachedView.StoryRemoveBtn.SetActiveEx(!isEdit);
            _cachedView.StoryCancelRemoveBtn.SetActiveEx(!isEdit);
        }

        private void OnPublishTimeBtn()
        {
            RemoteCommands.GmPublishRpgProject(
                DateTimeUtil.DateTimeToUnixTimestampMillis(OfficalData.GetTimeByItems(_PublisheTime)), ret => { },
                code => { });
        }

        private void OnStoryUpLoadBtn()
        {
            RemoteCommands.GmUploadRpgProject(
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

            _mainCtrl.OpenAddSelfRecommendPanel(Project.ProjectTypeSingle, idSet);
        }

        public override void OnConfirmAddBtn()
        {
            List<Msg_GmUpdateRpgProjectPrepare> _updateList = new List<Msg_GmUpdateRpgProjectPrepare>();
            for (int i = 0; i < AddProjectList.Count; i++)
            {
                Msg_GmUpdateRpgProjectPrepare item = new Msg_GmUpdateRpgProjectPrepare();
                item.SlotInx = _data.AllList.Count + i;
                item.ProjectId = AddProjectList[i].ProjectId;
                _updateList.Add(item);
            }

            RemoteCommands.GmUpdateRpgProjectPrepare(_updateList,
                ret =>
                {
                    SocialGUIManager.ShowPopupDialog("添加成功！");
                    RequestData();
                },
                code => { SocialGUIManager.ShowPopupDialog("添加失败"); });
            RefreshBtn(true);
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
        }
    }
}