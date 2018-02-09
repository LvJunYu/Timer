using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlMultiBattle : UICtrlAnimationBase<UIViewMultiBattle>
    {
        private bool _hasRequested;
        private bool _pushGoldEnergyStyle;
        private List<Project> _dataList;
        private List<UMCtrlOfficialProject> _umList;
        private USCtrlChat _chat;
        private OfficialProjectList _data;
        private List<Msg_MC_RoomUserInfo> _userList;
        private USCtrlMultiTeam[] _usCtrlMultiTeams;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnSelectedOfficalProjectListChanged, OnSelectedOfficalProjectListChanged);
            RegisterEvent(EMessengerType.OnInTeam, OnInTeam);
            RegisterEvent(EMessengerType.OnLeaveTeam, OnLeaveTeam);
            RegisterEvent(EMessengerType.OnTeamUserChanged, RefreshTeamPannel);
            RegisterEvent<int>(EMessengerType.OnTeamUserChanged, OnTeamUserCountChanged);
        }

        private void OnTeamUserCountChanged(int count)
        {
            if (_dataList == null || _umList == null)
            {
                return;
            }
            if (LocalUser.Instance.MultiBattleData.IsMyTeam)
            {
                for (int i = 0; i < _dataList.Count; i++)
                {
                    if (_dataList[i].NetData.PlayerCount < count && _umList[i].Selected)
                    {
                        _umList[i].OnSelectBtn();
                    }
                }
            }
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp, new Vector3(0, 100, 0), 0.17f);
            SetPart(_cachedView.PannelRtf.transform, EAnimationType.MoveFromDown);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtn);
            _cachedView.QuickStartBtn.onClick.AddListener(OnQuickStartBtn);
            _cachedView.QuitTeamBtn.onClick.AddListener(OnQuitTeamBtn);
            _cachedView.InviteButton.onClick.AddListener(OnInviteButton);
            _cachedView.RefuseInviteTog.onValueChanged.AddListener(value =>
                LocalUser.Instance.MultiBattleData.RefuseTeamInvite = value);
            _chat = new USCtrlChat();
            _chat.ResScenary = ResScenary;
            _chat.Scene = USCtrlChat.EScene.Team;
            _chat.Init(_cachedView.RoomChat);
            var list = _cachedView.PlayerDock.GetComponentsInChildren<USViewMultiTeam>(true);
            _usCtrlMultiTeams = new USCtrlMultiTeam[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlMultiTeams[i] = new USCtrlMultiTeam();
                _usCtrlMultiTeams[i].Init(list[i]);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }

            _chat.Open();
            _cachedView.RefuseInviteTog.isOn = LocalUser.Instance.MultiBattleData.RefuseTeamInvite;
            if (!_hasRequested)
            {
                RequestData();
            }
            else
            {
                CompleteRequestData();
            }
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
        }

        private void RequestData()
        {
            _data = AppData.Instance.OfficialProjectList;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在读取数据");
            _data.RequestOfficalMulti(() =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                _dataList = _data.MultiProjects;
                if (_dataList != null)
                {
                    _umList = new List<UMCtrlOfficialProject>(_dataList.Count);
                    for (int i = 0; i < _dataList.Count; i++)
                    {
                        var um = new UMCtrlOfficialProject();
                        um.Init(_cachedView.GridRtf, ResScenary);
                        um.Set(_dataList[i]);
                        _umList.Add(um);
                        LocalUser.Instance.MultiBattleData.OnProjectSelectedChanged(_dataList[i].ProjectId, true);
                    }

                    _hasRequested = true;
                    CompleteRequestData();
                }
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                SocialGUIManager.Instance.CloseUI<UICtrlMultiBattle>();
                SocialGUIManager.ShowPopupDialog("请求关卡数据失败");
            });
        }

        private void CompleteRequestData()
        {
            var teamInfo = LocalUser.Instance.MultiBattleData.TeamInfo;
            _cachedView.TeamPannel.SetActive(teamInfo != null);
            if (teamInfo == null)
            {
                RoomManager.Instance.SendCreateTeam();
            }
            else
            {
                RefreshTeamPannel();
            }
        }

        private void RefreshTeamPannel()
        {
            if (!_isOpen || LocalUser.Instance.MultiBattleData.TeamInfo == null)
            {
                return;
            }
            _userList = LocalUser.Instance.MultiBattleData.TeamInfo.UserList;
            _cachedView.QuitTeamBtn.SetActiveEx(_userList.Count > 1);
            _cachedView.InviteButton.SetActiveEx(_userList.Count < TeamManager.MaxTeamCount);
            for (int i = 0; i < _usCtrlMultiTeams.Length; i++)
            {
                if (i < _userList.Count)
                {
                    _usCtrlMultiTeams[i].Set(_userList[i]);
                }
                else
                {
                    _usCtrlMultiTeams[i].Set(null);
                }
            }
            _cachedView.QuickStartBtn.SetActiveEx(LocalUser.Instance.MultiBattleData.IsMyTeam);
        }

        private void OnLeaveTeam()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlMultiBattle>();
        }

        private void OnQuitTeamBtn()
        {
            RoomManager.Instance.SendExitTeam();
        }

        private void OnInviteButton()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInviteFriend>().InviteType = EInviteType.Team;
        }

        private void OnQuickStartBtn()
        {
            if (LocalUser.Instance.MultiBattleData.SelectedOfficalProjectList.Count == 0)
            {
                SocialGUIManager.ShowPopupDialog("至少选择一个关卡才能开始游戏");
                return;
            }

            RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Offical);
        }

        private void OnReturnBtn()
        {
            RoomManager.Instance.SendExitTeam();
            SocialGUIManager.Instance.CloseUI<UICtrlMultiBattle>();
        }

        private void OnSelectedOfficalProjectListChanged()
        {
            if (_dataList == null || _umList == null)
            {
                return;
            }

            var list = LocalUser.Instance.MultiBattleData.SelectedOfficalProjectList;
            for (int i = 0; i < _dataList.Count; i++)
            {
                _umList[i].SetSelected(list.Contains(_dataList[i].ProjectId));
            }
        }

        private void OnInTeam()
        {
            if (!_isOpen)
            {
                return;
            }

            _cachedView.TeamPannel.SetActive(true);
            RefreshTeamPannel();
        }
    }
}