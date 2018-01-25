using System;
using System.Collections.Generic;
using DG.Tweening;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlMultiRoom : UICtrlInGameBase<UIViewMultiRoom>
    {
        private const string EmptyStr = "无";
        private RoomInfo _roomInfo;
        private Project _project;
        private bool _openState;
        private USCtrlMultiRoomSlot[] _usCtrlMultiRoomSlots;
        private USCtrlMultiRoomRawSlot[] _usCtrlMultiRoomRawSlots;
        private Sequence _openSequence;
        private Sequence _closeSequence;
        private List<UnitExtraDynamic> _roomPlayerUnitExtras = new List<UnitExtraDynamic>();
        private RoomUser _myRoomUser;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
//            RegisterEvent(EMessengerType.OnRoomInfoChanged, OnRoomInfoChanged);
            RegisterEvent<long>(EMessengerType.OnUserExit, OnUserExit);
            RegisterEvent<long>(EMessengerType.OnUserKick, OnUserKick);
            RegisterEvent<Msg_RC_ChangePos>(EMessengerType.OnRoomChangePos, OnRoomChangePos);
            RegisterEvent<Msg_RC_UserReadyInfo>(EMessengerType.OnRoomPlayerReadyChanged, OnRoomPlayerReadyChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OpenBtn.onClick.AddListener(OnOpenBtn);
            _cachedView.CloseButton.onClick.AddListener(OnCloseButton);
            _cachedView.WorldRecruitBtn.onClick.AddListener(OnWorldRecruitBtn);
            _cachedView.InviteFriendBtn.onClick.AddListener(OnInviteFriendBtn);
            _cachedView.PrepareBtn.onClick.AddListener(OnPrepareBtn);
            _cachedView.RawPrepareBtn.onClick.AddListener(OnPrepareBtn);
            _cachedView.StartBtn.onClick.AddListener(OnStartBtn);
            _cachedView.RawStartBtn.onClick.AddListener(OnStartBtn);
            var list = _cachedView.OpenPannel.GetComponentsInChildren<USViewMultiRoomSlot>();
            _usCtrlMultiRoomSlots = new USCtrlMultiRoomSlot[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlMultiRoomSlots[i] = new USCtrlMultiRoomSlot();
                _usCtrlMultiRoomSlots[i].Init(list[i]);
                _usCtrlMultiRoomSlots[i].SetIndex(i);
            }

            var rawList = _cachedView.ClosePannel.GetComponentsInChildren<USViewMultiRoomRawSlot>();
            _usCtrlMultiRoomRawSlots = new USCtrlMultiRoomRawSlot[rawList.Length];
            for (int i = 0; i < rawList.Length; i++)
            {
                _usCtrlMultiRoomRawSlots[i] = new USCtrlMultiRoomRawSlot();
                _usCtrlMultiRoomRawSlots[i].Init(rawList[i]);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _roomInfo = parameter as RoomInfo;
            _project = GM2DGame.Instance.GameMode.Project;
            if (_roomInfo == null || _project == null)
            {
                LogHelper.Error("Open UICtrlMultiRoom fail");
                SocialGUIManager.Instance.CloseUI<UICtrlMultiRoom>();
                return;
            }

            _myRoomUser = _roomInfo.Users.Find(p => p.Guid == LocalUser.Instance.UserGuid);
            if (_myRoomUser == null)
            {
                LogHelper.Error("Open UICtrlMultiRoom fail, _myRoomUser == null");
                SocialGUIManager.Instance.CloseUI<UICtrlMultiRoom>();
                return;
            }

            if (!GetRoomPlayerUnitExtras())
            {
                SocialGUIManager.Instance.CloseUI<UICtrlMultiRoom>();
                return;
            }

            _openState = true;
            RefrshView();
        }

        private bool GetRoomPlayerUnitExtras()
        {
            var dic = TeamManager.Instance.GetPlayerUnitExtraDic();
            _roomPlayerUnitExtras.Clear();
            for (int i = 0; i < TeamManager.MaxTeamCount; i++)
            {
                UnitExtraDynamic unitExtra;
                if (dic.TryGetValue(i, out unitExtra))
                {
                    _roomPlayerUnitExtras.Add(unitExtra);
                }
            }

            if (_project.NetData == null || _project.NetData.PlayerCount != _roomPlayerUnitExtras.Count)
            {
                LogHelper.Error(
                    "_project.NetData == null || _project.NetData.PlayerCount != roomPlayerUnitExtras.Count");
                return false;
            }

            for (int i = 0; i < _usCtrlMultiRoomSlots.Length; i++)
            {
                if (i < _roomPlayerUnitExtras.Count)
                {
                    _usCtrlMultiRoomSlots[i].SetUnitExtra(_roomPlayerUnitExtras[i]);
                }
                else
                {
                    _usCtrlMultiRoomSlots[i].SetUnitExtra(null);
                }
            }

            for (int i = 0; i < _usCtrlMultiRoomRawSlots.Length; i++)
            {
                if (i < _roomPlayerUnitExtras.Count)
                {
                    _usCtrlMultiRoomRawSlots[i].SetUnitExtra(_roomPlayerUnitExtras[i]);
                }
                else
                {
                    _usCtrlMultiRoomRawSlots[i].SetUnitExtra(null);
                }
            }

            return true;
        }

        private void RefrshView()
        {
            _cachedView.OpenPannel.SetActiveEx(_openState);
            _cachedView.MaskRtf.SetActiveEx(_openState);
            _cachedView.ClosePannel.SetActiveEx(!_openState);
            if (_openState)
            {
                RefreshOpenPannel();
            }
            else
            {
                RefreshClosePannel();
            }

            RefreshBtns();
        }

        private void RefreshClosePannel()
        {
            RefreshPlayerInfo();
        }

        private void RefreshOpenPannel()
        {
            _cachedView.TitleTxt.text = _project.Name;
            _cachedView.RoomIdTxt.text = _roomInfo.RoomId.ToString();
            RefreshWinConditionView();
            RefreshPlayerInfo();
        }

        private void RefreshPlayerInfo()
        {
            var userArray = _roomInfo.RoomUserArray;
            if (_openState)
            {
                for (int i = 0; i < _usCtrlMultiRoomSlots.Length; i++)
                {
                    if (i < userArray.Length)
                    {
                        _usCtrlMultiRoomSlots[i].Set(userArray[i]);
                    }
                    else
                    {
                        _usCtrlMultiRoomRawSlots[i].Set(null);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _usCtrlMultiRoomRawSlots.Length; i++)
                {
                    if (i < userArray.Length)
                    {
                        _usCtrlMultiRoomRawSlots[i].Set(userArray[i]);
                    }
                }
            }
        }

        private void RefreshWinConditionView()
        {
            var netData = _project.NetData;
            if (netData == null) return;
            _cachedView.TimeLimit.text = netData.GetTimeLimit();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.ScoreWinCondition ? netData.WinScore.ToString() : EmptyStr;
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
        }

        private void CreateSequences()
        {
            _openSequence = DOTween.Sequence();
            _closeSequence = DOTween.Sequence();
            _openSequence.Append(
                _cachedView.OpenPannel.DOBlendableMoveBy(Vector3.left * 800, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequence.Append(_cachedView.OpenPannel.DOBlendableMoveBy(Vector3.left * 800, 0.3f)
                .SetEase(Ease.InOutQuad)).OnComplete(OnCloseAnimationComplete).SetAutoKill(false).Pause();

            Image img = _cachedView.MaskRtf.GetComponent<Image>();
            if (img != null)
            {
                _openSequence.Join(img.DOFade(0, 0.3f).From().SetEase(Ease.OutQuad));
                _closeSequence.Join(img.DOFade(0, 0.3f).SetEase(Ease.Linear));
            }
        }

        private void OnCloseAnimationComplete()
        {
            _closeSequence.Rewind();
            SetState(false);
        }

        private void OnRoomChangePos(Msg_RC_ChangePos msg)
        {
            if (!_isOpen)
            {
                return;
            }

            _roomInfo.OnRoomChangePos(msg);
            RefreshPlayerInfo();
        }

        private void OnRoomPlayerReadyChanged(Msg_RC_UserReadyInfo msg)
        {
            if (!_isOpen)
            {
                return;
            }

            _roomInfo.OnRoomPlayerReadyChanged(msg);
            RefreshPlayerInfo();
            RefreshBtns();
        }

        private void RefreshBtns()
        {
            bool isHost = _roomInfo.HostUserId == LocalUser.Instance.UserGuid;
            _cachedView.PrepareBtnTxt.text = _myRoomUser.Ready ? "取消准备" : "准  备";
            _cachedView.RawPrepareBtnTxt.text = _myRoomUser.Ready ? "取消" : "准备";
            _cachedView.PrepareBtn.SetActiveEx(!isHost);
            _cachedView.RawPrepareBtn.SetActiveEx(!isHost);
            _cachedView.StartBtn.SetActiveEx(isHost);
            _cachedView.RawStartBtn.SetActiveEx(isHost);
        }

        private void OnUserKick(long userId)
        {
            if (!_isOpen)
            {
                return;
            }

            if (userId == LocalUser.Instance.UserGuid)
            {
                SocialGUIManager.ShowPopupDialog("您已被房主提出游戏", null, new KeyValuePair<string, Action>("确定", () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
                    GM2DGame.Instance.QuitGame(
                        () => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                        code => { SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this); },
                        true
                    );
                }));
            }
            else
            {
                _roomInfo.OnUserExit(userId);
                RefreshPlayerInfo();
            }
        }

        private void OnUserExit(long userId)
        {
            if (!_isOpen)
            {
                return;
            }

            _roomInfo.OnUserExit(userId);
            RefreshPlayerInfo();
        }

        private void OnCloseButton()
        {
            if (null == _closeSequence)
            {
                CreateSequences();
            }

            if (_openSequence.IsPlaying())
            {
                _openSequence.Complete(true);
            }

            _closeSequence.PlayForward();
        }

        private void OnOpenBtn()
        {
            if (null == _openSequence)
            {
                CreateSequences();
            }

            if (_closeSequence.IsPlaying())
            {
                _closeSequence.Complete(true);
            }

            SetState(true);
            _openSequence.Restart();
        }

        private void SetState(bool value)
        {
            _openState = value;
            RefrshView();
        }

        private void OnStartBtn()
        {
        }

        private void OnPrepareBtn()
        {
            RoomManager.Instance.SendRoomPrepare(!_myRoomUser.Ready);
        }

        private void OnInviteFriendBtn()
        {
        }

        private void OnWorldRecruitBtn()
        {
        }
    }
}