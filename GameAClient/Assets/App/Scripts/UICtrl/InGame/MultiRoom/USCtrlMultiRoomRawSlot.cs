using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlMultiRoomRawSlot : USCtrlBase<USViewMultiRoomRawSlot>
    {
        private const string BgImgFormat = "img_board_{0}";
        private RoomUser _user;
        private UnitExtraDynamic _unitExtra;
        private long _hostUserId;

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

        public void SetState(EState eState)
        {
            bool isMyself = _user != null && _user.Guid == LocalUser.Instance.UserGuid;
            _cachedView.RoomHostFlag.SetActive(eState == EState.Host);
            _cachedView.PreparedFlag.SetActive(eState == EState.Prepared);
            _cachedView.BgImage.SetActiveEx(eState != EState.Disable);
            _cachedView.PlayerImg.SetActiveEx(_user != null);
            _cachedView.BgSelectedImg.SetActiveEx(isMyself);
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

        public void SetUnitExtra(UnitExtraDynamic playerUnitExtra)
        {
            _unitExtra = playerUnitExtra;
            if (_unitExtra != null)
            {
                _cachedView.BgImage.sprite = GetBgSprite(_unitExtra.TeamId);
            }
        }
        
        public enum EState
        {
            Host,
            Prepared,
            UnPrepared,
            Waiting,
            Disable,
        }
    }
}