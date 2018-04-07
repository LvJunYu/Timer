using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorkShopOfficalAdventure : UPCtrlWorkShopOfficialProjectBase
    {
        public const int AdventureNormalCout = 9;
        public const int AdventureBounsCout = 3;
        private int MaxRpgCout = 50;
        private GmAdventureProjectPrepareList _data;
        private GmAdventureSection _curSection;
        private List<USCtrlTimePick> _timepickList = new List<USCtrlTimePick>();
        private GmAdventureProjectPublishTime PublishTime = new GmAdventureProjectPublishTime();

        protected new readonly List<CardDataRendererWrapper<GmPrepareProject>>
            _contentList = new List<CardDataRendererWrapper<GmPrepareProject>>();

        protected new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>> _dict =
            new Dictionary<long, CardDataRendererWrapper<GmPrepareProject>>();

        private List<GmAdventureSection> _adventureSectionsList;
        private List<int> _PublisheTime = new List<int>();
        protected new List<GmPrepareProject> _projectList;
        private List<UMCtrlSetionBtn> _setionIndexBtnGroup = new List<UMCtrlSetionBtn>();
        private int _curSectionIndex;


        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.AdventurePublishBtn.onClick.AddListener(OnPublishTimeBtn);
            _cachedView.AdventureUploadBtn.onClick.AddListener(OnStoryUpLoadBtn);
            _cachedView.AdventureEditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.AdventureRemoveBtn.onClick.AddListener(OnRemoveBtn);
            _cachedView.AdventureCancelRemoveBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.AddSectionBtn.onClick.AddListener(AddSection);
            for (int i = 0; i < _cachedView.AdventureTimePick.Length; i++)
            {
                USCtrlTimePick timePick = new USCtrlTimePick();
                timePick.Init(_cachedView.AdventureTimePick[i]);
                timePick.SetType((TimeType) i,
                    RefreshTimePick);
                _timepickList.Add(timePick);
            }
        }

        public override void Open()
        {
            _isOpen = true;
            _cachedView.Pannels[(int) _menu].SetActiveEx(true);
            RefreshBtn(true);
            RemoveProjectList.Clear();
            RequestData();
            RequestTime();
        }

        public override void RequestData(bool append = false)
        {
            _data = OfficalData.Instance.GmAdventureProjectPrepareList;
            int startInx = 0;
            if (append)
            {
                startInx = _data.AllList.Count;
            }

            _data.Request(startInx, MaxRpgCout, () =>
            {
                _adventureSectionsList = _data.SectionList;
                if (_data.AllList.Count <= 0)
                {
                    AddSection();
                }
                else
                {
                    _curSection = _data.SectionList[Mathf.Clamp(_curSectionIndex, 0, _data.SectionList.Count - 1)];
                    if (_isOpen)
                    {
                        RefreshSetionIndex();
                        SelectIndexBtn(1);
                    }
                }
            }, code => { LogHelper.Error("WorldNewestProjectList Request fail, code = {0}", code); });
        }

        private void RequestTime()
        {
            _cachedView.AdventureReleaseTimeObj.SetActiveEx(false);
            _cachedView.AdventureReleaseTimeText.SetActiveEx(false);
            PublishTime.Request(0, () =>
            {
                DateTime dateTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(PublishTime.PublishTime);
                if (LocalUser.Instance.User.RoleType == (int) EAccountRoleType.AcRT_Admin)
                {
                    _cachedView.AdventureReleaseTimeObj.SetActiveEx(true);
                    _cachedView.AdventureReleaseTimeText.SetActiveEx(false);
                }
                else
                {
                    _cachedView.AdventureReleaseTimeObj.SetActiveEx(false);
                    _cachedView.AdventureReleaseTimeText.SetActiveEx(true);
                    _cachedView.AdventureReleaseTimeText.text = dateTime.ToString();
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
                item.Index = inx;
                UMCtrlProjectGMWrokShop um = item as UMCtrlProjectGMWrokShop;
                if (um != null) um.EnableDrag(true);
                if (um != null) um.SetScrollRect(_cachedView.GridDataScrollers[(int) _menu]);
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
            _projectList = GetGmPrepareProjects(_curSection);
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
            if (_curSection.NormalProjectList.Count + _curSection.BonusProjectList.Count >
                AdventureNormalCout + AdventureBounsCout)
            {
                LogHelper.Error("冒险关卡的数目超过了预定的最大数量12");
            }
            else
            {
                List<Msg_GmUpdateAdventureProjectPrepare> _updateList = new List<Msg_GmUpdateAdventureProjectPrepare>();
                for (int i = 0; i < RemoveProjectList.Count; i++)
                {
                    Msg_GmUpdateAdventureProjectPrepare item = new Msg_GmUpdateAdventureProjectPrepare();
                    item.Section = _curSection.Section;
                    if (RemoveProjectList[i] >= AdventureNormalCout)
                    {
                        item.ProjectType = EAdventureProjectType.APT_Bonus;
                        item.Level = RemoveProjectList[i] + 1 - AdventureNormalCout;
                    }
                    else
                    {
                        item.ProjectType = EAdventureProjectType.APT_Normal;
                        item.Level = RemoveProjectList[i] + 1;
                    }

                    item.ProjectId = 0;
                    _updateList.Add(item);
                }

                if (_updateList.Count > 0)
                {
                    RemoteCommands.GmUpdateAdventureProjectPrepare(_updateList,
                        ret =>
                        {
                            RequestData();
                            SocialGUIManager.ShowPopupDialog("删除成功！");
                        },
                        code => { SocialGUIManager.ShowPopupDialog("删除失败"); });
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
            _cachedView.AdventureEditBtn.SetActiveEx(isEdit);
            _cachedView.AdventureRemoveBtn.SetActiveEx(!isEdit);
            _cachedView.AdventureCancelRemoveBtn.SetActiveEx(!isEdit);
        }

        private void OnPublishTimeBtn()
        {
            RemoteCommands.GmPublishAdventureProject(
                DateTimeUtil.DateTimeToUnixTimestampMillis(OfficalData.GetTimeByItems(_PublisheTime)), ret =>
                {
                    if (ret.ResultCode == (int) ENetResultCode.NR_Success)
                    {
                        SocialGUIManager.ShowPopupDialog("发布时间成功！");
                    }
                },
                code => { SocialGUIManager.ShowPopupDialog("发布时间失败！"); });
        }

        private void OnStoryUpLoadBtn()
        {
            RemoteCommands.GmUploadAdventureProject(
                0, ret => { },
                code => { });
        }

        private List<GmPrepareProject> GetGmPrepareProjects(GmAdventureSection section)
        {
            List<GmPrepareProject> temppojectList =
                new List<GmPrepareProject>();
            for (int i = 0; i < AdventureNormalCout + AdventureBounsCout; i++)
            {
                GmPrepareProject item = new GmPrepareProject();
                item.Id = i + 1;
                item.ProjectData = new Project();
                temppojectList.Add(item);
            }

            for (int i = 0; i < section.NormalProjectList.Count; i++)
            {
                if (section.NormalProjectList[i] != null)
                {
                    temppojectList[section.NormalProjectList[i].Level - 1] =
                        SwitchProject(section.NormalProjectList[i]);
                }
            }

            for (int i = 0; i < section.BonusProjectList.Count; i++)
            {
                if (section.BonusProjectList[i] != null)
                {
                    temppojectList[section.BonusProjectList[i].Level - 1 + AdventureNormalCout] =
                        SwitchProject(section.NormalProjectList[i]);
                }
            }

            return temppojectList;
        }

        public override void OnConfirmAddBtn()
        {
            if (AddProjectList.Count <= 0)
            {
                return;
            }

            List<Msg_GmUpdateAdventureProjectPrepare> _updateList =
                new List<Msg_GmUpdateAdventureProjectPrepare>();
            int index = 0;
            for (int i = 0; i < _projectList.Count; i++)
            {
                if (_projectList[i].ProjectData.ProjectId == 0)
                {
                    Msg_GmUpdateAdventureProjectPrepare item = new Msg_GmUpdateAdventureProjectPrepare();
                    item.Section = _curSection.Section;
                    item.Level = i + 1;
                    item.ProjectId = AddProjectList[index].ProjectId;
                    if (i >= AdventureNormalCout)
                    {
                        item.ProjectType = EAdventureProjectType.APT_Bonus;
                    }
                    else
                    {
                        item.ProjectType = EAdventureProjectType.APT_Normal;
                    }

                    _updateList.Add(item);
                    ++index;
                    if (index >= AddProjectList.Count)
                    {
                        break;
                    }
                }
            }

            if (_updateList.Count > 0)
            {
                RemoteCommands.GmUpdateAdventureProjectPrepare(_updateList,
                    ret =>
                    {
                        SocialGUIManager.ShowPopupDialog("添加成功！");
                        RequestData();
                    },
                    code => { SocialGUIManager.ShowPopupDialog("添加失败"); });
            }


            RefreshBtn(true);
            Messenger.Broadcast(EMessengerType.OnWorldOfficalRecommendCancelBtn);
        }

        private void AddSection()
        {
            _curSection = new GmAdventureSection();
            _curSectionIndex = _adventureSectionsList.Count + 1;
            _curSection.Section = _curSectionIndex;
            RefreshView();
        }

        private void RefreshSetionIndex()
        {
            for (int i = 0; i < _setionIndexBtnGroup.Count; i++)
            {
                UMPoolManager.Instance.Free(_setionIndexBtnGroup[i]);
            }

            for (int i = 0; i < _adventureSectionsList.Count; i++)
            {
                UMCtrlSetionBtn indexBtn =
                    UMPoolManager.Instance.Get<UMCtrlSetionBtn>(_cachedView.BtnIndexGroupContent, _resScenary);
                indexBtn.SetSectionIndex(_adventureSectionsList[i].Section, SelectIndexBtn);
                _setionIndexBtnGroup.Add(indexBtn);
            }
        }

        private void SelectIndexBtn(int index)
        {
            _curSectionIndex = index;
            _setionIndexBtnGroup[_curSectionIndex - 1].OnNoSelect();
            _curSection = _adventureSectionsList[Mathf.Clamp(_curSectionIndex, 0, _adventureSectionsList.Count - 1)];
            RefreshView();
        }

        public void OnDrangEnd(int benginIndex, int newindex)
        {
            List<Msg_GmUpdateAdventureProjectPrepare> _updateList =
                new List<Msg_GmUpdateAdventureProjectPrepare>();
            if (benginIndex < newindex)
            {
                for (int i = benginIndex; i < newindex; i++)
                {
                    Msg_GmUpdateAdventureProjectPrepare
                        sortItem = new Msg_GmUpdateAdventureProjectPrepare();
                    sortItem.Section = _curSection.Section;
                    if (i >= AdventureNormalCout)
                    {
                        sortItem.ProjectType = EAdventureProjectType.APT_Bonus;
                        sortItem.Level = i - AdventureNormalCout + 1;
                    }
                    else
                    {
                        sortItem.Level = i + 1;
                        sortItem.ProjectType = EAdventureProjectType.APT_Normal;
                    }

                    sortItem.ProjectId = _projectList[i + 1].ProjectData.ProjectId;
                    _updateList.Add(sortItem);
                }

                {
                    Msg_GmUpdateAdventureProjectPrepare
                        sortItem = new Msg_GmUpdateAdventureProjectPrepare();
                    sortItem.Section = _curSection.Section;
                    if (newindex >= AdventureNormalCout)
                    {
                        sortItem.ProjectType = EAdventureProjectType.APT_Bonus;
                        sortItem.Level = newindex - AdventureNormalCout + 1;
                    }
                    else
                    {
                        sortItem.Level = newindex + 1;
                        sortItem.ProjectType = EAdventureProjectType.APT_Normal;
                    }

                    sortItem.ProjectId = _projectList[benginIndex].ProjectData.ProjectId;
                    _updateList.Add(sortItem);
                }
            }

            if (benginIndex > newindex)
            {
                for (int i = newindex + 1; i <= benginIndex; i++)
                {
                    Msg_GmUpdateAdventureProjectPrepare
                        sortItem = new Msg_GmUpdateAdventureProjectPrepare();
                    sortItem.Section = _curSection.Section;

                    if (i >= AdventureNormalCout)
                    {
                        sortItem.ProjectType = EAdventureProjectType.APT_Bonus;
                        sortItem.Level = i - AdventureNormalCout + 1;
                    }
                    else
                    {
                        sortItem.Level = i + 1;
                        sortItem.ProjectType = EAdventureProjectType.APT_Normal;
                    }

                    sortItem.ProjectId = _projectList[i - 1].ProjectData.ProjectId;
                    _updateList.Add(sortItem);
                }

                {
                    Msg_GmUpdateAdventureProjectPrepare
                        sortItem = new Msg_GmUpdateAdventureProjectPrepare();
                    sortItem.Section = _curSection.Section;
                    if (newindex >= AdventureNormalCout)
                    {
                        sortItem.ProjectType = EAdventureProjectType.APT_Bonus;
                        sortItem.Level = newindex - AdventureNormalCout + 1;
                    }
                    else
                    {
                        sortItem.Level = newindex;
                        sortItem.ProjectType = EAdventureProjectType.APT_Normal;
                    }

                    sortItem.ProjectId = _projectList[benginIndex].ProjectData.ProjectId;
                    _updateList.Add(sortItem);
                }
            }

            if (_updateList.Count > 0)
            {
                RemoteCommands.GmUpdateAdventureProjectPrepare(_updateList, ret => { RequestData(); }, code => { });
            }
        }

        public void OnAddProjectBtn()
        {
            HashSet<long> idSet = new HashSet<long>();
            for (int i = 0; i < _projectList.Count; i++)
            {
                if (_projectList[i].ProjectData.ProjectId != 0)
                {
                    idSet.Add(_projectList[i].ProjectData.ProjectId);
                }
            }

            _mainCtrl.OpenAddSelfRecommendPanel(Project.ProjectTypeSingle, idSet);
        }

        private GmPrepareProject SwitchProject(GmAdventurePrepareProject adventurePrepareProject)
        {
            GmPrepareProject p = new GmPrepareProject();
            p.Id = adventurePrepareProject.Level;
            p.ProjectData = adventurePrepareProject.ProjectData;
            p.LastUpdateUser = adventurePrepareProject.LastUpdateUser;
            p.LastUpdateTime = adventurePrepareProject.LastUpdateTime;
            return p;
        }
    }
}