using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlMultiRoomSlot : USCtrlBase<USViewMultiRoomSlot>
    {
        private static readonly Color MyColor = new Color(229 / (float) 255, 124 / (float) 255, 23 / (float) 255, 1);
        private static readonly Color NormalColor = new Color(116 / (float) 255, 83 / (float) 255, 53 / (float) 255, 1);
        private const string WaitingStr = "等待中...";
        private const string BgImgFormat = "img_room_{0}";
        private int _index;
        private RoomUser _user;
        private UnitExtraDynamic _unitExtra;
        private long _hostUserId;
        private EState _curState;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
            _cachedView.ChangePosBtn.onClick.AddListener(OnChangePosBtn);
        }

        private void OnChangePosBtn()
        {
            if (_curState == EState.Waiting)
            {
                RoomManager.Instance.SendChangePos(_index);
            }
        }

        private void OnDeleteBtn()
        {
            RoomManager.Instance.SendDeletePlayer(_user.Guid);
        }

        public void SetState(EState eState)
        {
            _curState = eState;
            bool isMyself = _user != null && _user.Guid == LocalUser.Instance.UserGuid;
            _cachedView.RoomHosterFlag.SetActive(eState == EState.Host);
            _cachedView.BgImage.SetActiveEx(eState != EState.Disable);
            _cachedView.BgSelectedImg.SetActiveEx(isMyself);
            _cachedView.PlayerImg.SetActiveEx(_user != null);
            _cachedView.DiableObj.SetActive(eState == EState.Disable);
            _cachedView.PreparedFlag.SetActive(eState == EState.Prepared);
            _cachedView.DeleteBtn.SetActiveEx(_user != null && LocalUser.Instance.UserGuid == _hostUserId &&
                                              eState != EState.Host);
            _cachedView.TitleTxt.text = GetTitle(eState);
            _cachedView.TitleTxt.color = isMyself ? MyColor : NormalColor;
        }

        private Sprite GetBgSprite(int teamId)
        {
            if (teamId < 1 || teamId > TeamManager.MaxTeamCount)
            {
                LogHelper.Error("teamId is out of range");
                return null;
            }
            return JoyResManager.Instance.GetSprite(string.Format(BgImgFormat, TeamManager.GetTeamColorName(teamId)));
        }

        private string GetTitle(EState eState)
        {
            switch (eState)
            {
                case EState.Host:
                case EState.Prepared:
                case EState.UnPrepared:
                    return _user.Name;
                case EState.Waiting:
                    return WaitingStr;
            }

            return string.Empty;
        }

        public enum EState
        {
            Host,
            Prepared,
            UnPrepared,
            Waiting,
            Disable,
        }

        public void Set(RoomUser roomUser)
        {
            _user = roomUser;
            RefreshView();
        }

        private void RefreshView()
        {
            var gameMode = GM2DGame.Instance.GameMode as GameModeNetPlay;
            if (gameMode != null)
            {
                _hostUserId = gameMode.RoomInfo.HostUserId;
            }

            if (_user == null)
            {
                if (_unitExtra == null)
                {
                    SetState(EState.Disable);
                }
                else
                {
                    SetState(EState.Waiting);
                }
            }
            else
            {
                if (_user.Guid == _hostUserId)
                {
                    SetState(EState.Host);
                }
                else if (_user.Ready)
                {
                    SetState(EState.Prepared);
                }
                else
                {
                    SetState(EState.UnPrepared);
                }
            }
        }

        public void SetUnitExtra(UnitExtraDynamic playerUnitExtra)
        {
            _unitExtra = playerUnitExtra;
            if (_unitExtra != null)
            {
                _cachedView.BgImage.sprite = GetBgSprite(_unitExtra.TeamId);
            }
        }

        public void SetIndex(int index)
        {
            _index = index;
        }
    }
}