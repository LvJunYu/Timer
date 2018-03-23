﻿using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInvitedByFriend : UICtrlAnimationBase<UIViewInvitedByFriend>
    {
        private const string InviteRoomFormat = "邀请您加入关卡<color=#4CA2D4>【{0}】</color>";
        private EInviteType _inviteType;
        private List<LocalTeamInvite> _teamInviteList;
        private List<LocalRoomInvite> _roomInviteList;
        private bool _onlyChangeView;
        private Text[] _tagTxts;
        private Text[] _tagSelectedTxts;
        private Button[] _tagBtns;
        private Button[] _tagSelectedBtns;
        private int _curIndex;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.RefuseTog.onValueChanged.AddListener(OnRefuseTogValueChanged);
            _tagTxts = _cachedView.TogDock.GetComponentsInChildren<Text>();
            _tagSelectedTxts = _cachedView.TogSelectedDock.GetComponentsInChildren<Text>(true);
            _tagBtns = _cachedView.TogDock.GetComponentsInChildren<Button>();
            _tagSelectedBtns = _cachedView.TogSelectedDock.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _tagBtns.Length; i++)
            {
                var inx = i;
                _cachedView.TabGroup.AddButton(_tagBtns[i], _tagSelectedBtns[i], b => ClickInvite(inx, b));
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _inviteType = (EInviteType) parameter;
            _onlyChangeView = true;
            _cachedView.RefuseTog.isOn = false;
            _onlyChangeView = false;
            RefreshView();
        }

        protected override void OnClose()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultProjectTexture);
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.AppGameUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnTeamInviteChanged, OnTeamInviteChanged);
            RegisterEvent(EMessengerType.OnRoomInviteChanged, OnRoomInviteChanged);
        }

        private void RefreshView()
        {
            _cachedView.TeamPannel.SetActive(_inviteType == EInviteType.Team);
            _cachedView.RoomPannel.SetActive(_inviteType == EInviteType.Room);
            if (_inviteType == EInviteType.Team)
            {
                _teamInviteList = LocalUser.Instance.MultiBattleData.TeamInviteList;
                for (int i = 0; i < _tagBtns.Length; i++)
                {
                    _tagBtns[i].SetActiveEx(i < _teamInviteList.Count);
                }
                for (int i = 0; i < _tagTxts.Length; i++)
                {
                    if (i < _teamInviteList.Count)
                    {
                        _tagSelectedTxts[i].text = _tagTxts[i].text = GameATools.GetRawStr(_teamInviteList[i].Msg.Inviter.NickName, 6);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (_inviteType == EInviteType.Room)
            {
                _roomInviteList = LocalUser.Instance.MultiBattleData.RoomInviteStack;
                for (int i = 0; i < _tagBtns.Length; i++)
                {
                    _tagBtns[i].SetActiveEx(i < _roomInviteList.Count);
                }

                for (int i = 0; i < _tagTxts.Length; i++)
                {
                    if (i < _roomInviteList.Count)
                    {
                        _tagSelectedTxts[i].text = _tagTxts[i].text = GameATools.GetRawStr(_roomInviteList[i].Msg.Inviter.NickName, 6);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            for (int i = 0; i < _tagSelectedBtns.Length; i++)
            {
                _tagSelectedBtns[i].SetActiveEx(i == 0);
            }
            ClickInvite(0, true);
        }

        private void ClickInvite(int inx, bool b)
        {
            if (!b) return;
            _curIndex = inx;
            Msg_MC_RoomUserInfo _inviter = null;
            if (_inviteType == EInviteType.Team)
            {
                _inviter = _teamInviteList[inx].Msg.Inviter;
            }
            else if (_inviteType == EInviteType.Room)
            {
                var invite = _roomInviteList[inx];
                _inviter = invite.Msg.Inviter;
                ProjectManager.Instance.GetDataOnAsync(invite.Msg.ProjectId, p =>
                {
                    _cachedView.InviteProjcetTxt.text = string.Format(InviteRoomFormat, p.Name);
                    ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath,
                        _cachedView.DefaultProjectTexture);
                });
            }

            if (_inviter != null)
            {
                _cachedView.UserName.text = _inviter.NickName;
                UserManager.Instance.GetDataOnAsync(_inviter.UserGuid, user =>
                {
                    _cachedView.AdvLvTxt.text = GameATools.GetLevelString(user.UserInfoSimple.LevelData.PlayerLevel);
                    _cachedView.CreateLvTxt.text =
                        GameATools.GetLevelString(user.UserInfoSimple.LevelData.CreatorLevel);
                    ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.UserInfoSimple.HeadImgUrl,
                        _cachedView.DefaultUserIconTexture);
                    user.UserInfoSimple.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg,
                        _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
                });
            }
        }

        private void OnTeamInviteChanged()
        {
            if (_isOpen && _inviteType == EInviteType.Team)
            {
                RefreshView();
            }
        }

        private void OnRoomInviteChanged()
        {
            if (_isOpen && _inviteType == EInviteType.Room)
            {
                RefreshView();
            }
        }

        private void OnRefuseTogValueChanged(bool value)
        {
            if (_onlyChangeView)
            {
                return;
            }

            if (_inviteType == EInviteType.Room)
            {
                LocalUser.Instance.MultiBattleData.RefuseRoomInviteInMins = value;
            }
            else if (_inviteType == EInviteType.Team)
            {
                LocalUser.Instance.MultiBattleData.RefuseTeamInviteInMins = value;
            }
        }

        private void OnOKBtn()
        {
            if (_inviteType == EInviteType.Team)
            {
                RoomManager.Instance.SendJoinTeam(_teamInviteList[_curIndex].Msg.TeamHostId);
            }
            else if (_inviteType == EInviteType.Room)
            {
                var roomId = _roomInviteList[_curIndex].Msg.RoomId;
                if (GM2DGame.Instance != null)
                {
                    var gameMode = GM2DGame.Instance.GameMode;
                    if (gameMode is GameModeNetPlay && ((GameModeNetPlay)gameMode).RoomInfo.RoomId == roomId)
                    {
                        SocialGUIManager.Instance.CloseUI<UICtrlInvitedByFriend>();
                        SocialGUIManager.ShowPopupDialog("已加入关卡");
                        return;
                    }
                }
                RoomManager.Instance.SendRequestJoinRoom(roomId);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInvitedByFriend>();
        }
    }
}