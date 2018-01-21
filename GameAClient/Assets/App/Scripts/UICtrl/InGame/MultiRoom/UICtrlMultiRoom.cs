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

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnRoomInfoChanged, OnRoomInfoChanged);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OpenBtn.onClick.AddListener(OnOpenBtn);
            _cachedView.CloseButton.onClick.AddListener(OnCloseButton);
            _cachedView.WorldRecruitBtn.onClick.AddListener(OnWorldRecruitBtn);
            _cachedView.InviteFriendBtn.onClick.AddListener(OnInviteFriendBtn);
            _cachedView.PrepareBtn.onClick.AddListener(OnPrepareBtn);
            _cachedView.StartBtn.onClick.AddListener(OnStartBtn);
            _cachedView.RawStartBtn.onClick.AddListener(OnRawStartBtn);
            var list = _cachedView.OpenPannel.GetComponentsInChildren<USViewMultiRoomSlot>();
            _usCtrlMultiRoomSlots = new USCtrlMultiRoomSlot[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                _usCtrlMultiRoomSlots[i] = new USCtrlMultiRoomSlot();
                _usCtrlMultiRoomSlots[i].Init(list[i]);
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
//            _roomInfo = new RoomInfo(new Msg_MC_RoomInfo()); //todo 临时
            _project = GM2DGame.Instance.GameMode.Project;
            if (_roomInfo == null || _project == null)
            {
                LogHelper.Error("Open UICtrlMultiRoom fail");
                SocialGUIManager.Instance.CloseUI<UICtrlMultiRoom>();
                return;
            }

            _openState = false;
            RefrshView();
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
            var users = PlayerManager.Instance.RoomUsers;
            if (_openState)
            {
                for (int i = 0; i < _usCtrlMultiRoomSlots.Length; i++)
                {
                    if (i < users.Length)
                    {
                        _usCtrlMultiRoomSlots[i].Set(users[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _usCtrlMultiRoomRawSlots.Length; i++)
                {
                    if (i < users.Length)
                    {
                        _usCtrlMultiRoomRawSlots[i].Set(users[i]);
                    }
                }
            }
        }

        private void OnRoomInfoChanged()
        {
            if (_isOpen)
            {
                RefreshPlayerInfo();
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

        private void OnRawStartBtn()
        {
        }

        private void OnStartBtn()
        {
        }

        private void OnPrepareBtn()
        {
        }

        private void OnInviteFriendBtn()
        {
        }

        private void OnWorldRecruitBtn()
        {
        }
    }
}