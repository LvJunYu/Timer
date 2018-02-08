using DG.Tweening;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInvitedRaw : UICtrlResManagedBase<UIViewInvitedRaw>
    {
        private const int MaxWaitTime = 10;
        private const string InviteFormat1 = "{0}邀请您";
        private const string InviteFormat2 = "加入关卡【{0}】";
        private Sequence _openSequenceTeam;
        private Sequence _closeSequenceTeam;
        private Sequence _openSequenceRoom;
        private Sequence _closeSequenceRoom;
        private bool _showTeamInvite;
        private bool _showRoomInvite;
        private float _teamTimer;
        private float _roomTimer;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpDialog;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<Msg_MC_TeamInvite>(EMessengerType.OnTeamInviteChanged, PushTeamInvite);
            RegisterEvent<Msg_MC_RoomInvite>(EMessengerType.OnRoomInviteChanged, PushRoomInvite);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RoomBtn.onClick.AddListener(OnRoomBtn);
            _cachedView.TeamBtn.onClick.AddListener(OnTeamBtn);
        }

        private void OnRoomBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInvitedByFriend>(EInviteType.Room);
            _roomTimer = 0;
        }

        private void OnTeamBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlInvitedByFriend>(EInviteType.Team);
            _teamTimer = 0;
        }

        private void PushRoomInvite(Msg_MC_RoomInvite msg)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlInvitedRaw>();
            }

            _cachedView.RoomInviteTxt1.text =
                string.Format(InviteFormat1, GameATools.GetRawStr(msg.Inviter.NickName, 8));
            ProjectManager.Instance.GetDataOnAsync(msg.ProjectId,p =>
            {
                _cachedView.RoomInviteTxt2.text = string.Format(InviteFormat2, GameATools.GetRawStr(p.Name, 6));
            });
            OpenAnimation(EInviteType.Room);
            _showRoomInvite = true;
            _roomTimer = MaxWaitTime;
        }

        private void PushTeamInvite(Msg_MC_TeamInvite msg)
        {
            if (!_isOpen)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlInvitedRaw>();
            }

            _cachedView.TeamInviteTxt.text =
                string.Format(InviteFormat1, GameATools.GetRawStr(msg.Inviter.NickName, 8));
            OpenAnimation(EInviteType.Team);
            _showTeamInvite = true;
            _teamTimer = MaxWaitTime;
        }

        private void OpenAnimation(EInviteType inviteType)
        {
            if (inviteType == EInviteType.Team)
            {
                if (null == _openSequenceTeam)
                {
                    CreateSequences();
                }

                if (_closeSequenceTeam.IsPlaying())
                {
                    _closeSequenceTeam.Complete(true);
                }

                _openSequenceTeam.Restart();
            }
            else if (inviteType == EInviteType.Room)
            {
                if (null == _openSequenceRoom)
                {
                    CreateSequences();
                }

                if (_closeSequenceRoom.IsPlaying())
                {
                    _closeSequenceRoom.Complete(true);
                }

                _openSequenceRoom.Restart();
            }
        }

        private void CloseAnimation(EInviteType inviteType)
        {
            if (inviteType == EInviteType.Team)
            {
                if (null == _closeSequenceTeam)
                {
                    CreateSequences();
                }

                if (_openSequenceTeam.IsPlaying())
                {
                    _openSequenceTeam.Complete(true);
                }

                _closeSequenceTeam.PlayForward();
            }
            else if (inviteType == EInviteType.Room)
            {
                if (null == _closeSequenceRoom)
                {
                    CreateSequences();
                }

                if (_openSequenceRoom.IsPlaying())
                {
                    _openSequenceRoom.Complete(true);
                }

                _closeSequenceRoom.PlayForward();
            }
        }

        private void CreateSequences()
        {
            _openSequenceTeam = DOTween.Sequence();
            _closeSequenceTeam = DOTween.Sequence();
            _openSequenceTeam.Append(
                _cachedView.TeamInviteRtf.DOBlendableMoveBy(Vector3.right * 200, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequenceTeam.Append(_cachedView.TeamInviteRtf.DOBlendableMoveBy(Vector3.right * 200, 0.3f)
                .SetEase(Ease.InOutQuad)).SetAutoKill(false).Pause();

            _openSequenceRoom = DOTween.Sequence();
            _closeSequenceRoom = DOTween.Sequence();
            _openSequenceRoom.Append(
                _cachedView.RoomInviteRtf.DOBlendableMoveBy(Vector3.right * 200, 0.3f).From()
                    .SetEase(Ease.OutQuad)).SetAutoKill(false).Pause();
            _closeSequenceRoom.Append(_cachedView.RoomInviteRtf.DOBlendableMoveBy(Vector3.right * 200, 0.3f)
                .SetEase(Ease.InOutQuad)).SetAutoKill(false).Pause();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_showTeamInvite)
            {
                if (_teamTimer > 0)
                {
                    _teamTimer -= Time.deltaTime;
                    _cachedView.TeamInviteTimerTxt.text = Mathf.CeilToInt(_teamTimer).ToString();
                }
                else
                {
                    CloseAnimation(EInviteType.Team);
                    _showTeamInvite = false;
                }
            }

            if (_showRoomInvite)
            {
                if (_roomTimer > 0)
                {
                    _roomTimer -= Time.deltaTime;
                    _cachedView.RoomInviteTimerTxt.text = Mathf.CeilToInt(_roomTimer).ToString();
                }
                else
                {
                    CloseAnimation(EInviteType.Room);
                    _showRoomInvite = false;
                }
            }
        }
    }
}