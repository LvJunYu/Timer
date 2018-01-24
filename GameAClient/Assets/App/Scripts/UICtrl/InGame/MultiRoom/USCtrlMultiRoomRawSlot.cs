using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlMultiRoomRawSlot : USCtrlBase<USViewMultiRoomRawSlot>
    {
        private const string BgImgFormat = "img_board_{0}";
//        private const string SelectedImgFormat = "img_board_{0}_s";
        private RoomUser _user;

        public void Set(RoomUser roomUser)
        {
            _user = roomUser;
            RefreshView();
        }

        private void RefreshView()
        {
            if (_user == null)
            {
                SetState(EState.Waiting);
            }
            else
            {
                //todo
            }
        }

        public void SetState(EState eState)
        {
            _cachedView.RoomOwnerFlag.SetActive(eState == EState.Owner);

            _cachedView.BgImage.SetActiveEx(eState != EState.Waiting && eState != EState.Disable);
            if (_user != null)
            {
                var dic = TeamManager.Instance.GetPlayerUnitExtraDic();
                UnitExtraDynamic unitExtra;
                if (dic.TryGetValue(_user.Inx, out unitExtra))
                {
                    _cachedView.BgImage.sprite = GetBgSprite(unitExtra.TeamId);
                }
            }
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

        public enum EState
        {
            Owner,
            Prepared,
            UnPrepared,
            Waiting,
            Disable,
        }
    }
}