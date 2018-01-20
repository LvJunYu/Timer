using System;
using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class USCtrlMultiRoomSlot : USCtrlBase<USViewMultiRoomSlot>
    {
        private const string WaitingStr = "等待中...";
        private const string BgImgFormat = "img_room_{0}";
        private const string SelectedImgFormat = "img_room_{0}_s";
        private UserInfoDetail _userInfoDetail;
        private int _index;

        public void SetState(EState eState)
        {
            _cachedView.RoomOwnerFlag.SetActive(eState == EState.Owner);
            _cachedView.WaitingObj.SetActive(eState == EState.Waiting);
            _cachedView.DiableObj.SetActive(eState == EState.Disable);
            _cachedView.TitleTxt.text = GetTitle(eState);
            
            _cachedView.BgImage.SetActiveEx(eState != EState.Waiting && eState!= EState.Disable);
            var dic = TeamManager.Instance.PlayerUnitExtraDic;
            UnitExtraDynamic unitExtra;
            if (dic.TryGetValue(_index, out unitExtra))
            {
                _cachedView.BgImage.sprite = GetBgSprite(unitExtra.TeamId);
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

        private string GetTitle(EState eState)
        {
            switch (eState)
            {
                case EState.Owner:
                case EState.Prepared:
                    return _userInfoDetail.UserInfoSimple.NickName;
                case EState.Waiting:
                    return WaitingStr;
            }

            return string.Empty;
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