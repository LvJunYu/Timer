using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;
using GameA;
using GameA.Game;
using SoyEngine.Proto;

namespace GameA
{
    public class SettlePlayerData
    {
        public string Name;
        public int killNum;
        public int KilledNum;
        public int Score;
        public int TeamScore;
        public int TeamId;
        public bool IsWin;
        public long MainPlayID;
        public long PlayerId;
        public bool IsMvp;
    }

    public class UMCtlSettlePalyerDataItem : UMCtrlBase<UMViewSettlePalyerDataItem>, IUMPoolable
    {
        #region 变量

        private SettlePlayerData _playerData;
        private UserInfoDetail _playDetail;

        #endregion

        #region 属性

        #endregion

        #region 方法

        public void IintItem(int unitID, bool isUnlock)
        {
        }

        #endregion

        public bool IsShow { get; private set; }

        public void Show()
        {
            _cachedView.Trans.SetActiveEx(true);
            IsShow = true;
        }

        public void Hide()
        {
            _cachedView.Trans.SetActiveEx(false);
            IsShow = false;
        }

        public void SetItemData(SettlePlayerData data)
        {
            _cachedView.AddFriendBtn.onClick.RemoveAllListeners();
            _cachedView.LikeBtn.onClick.RemoveAllListeners();
            _cachedView.UnLikeBtn.onClick.RemoveAllListeners();
            _cachedView.AddFriendBtn.onClick.AddListener(OnAddFriendBtn);
            _cachedView.LikeBtn.onClick.AddListener(OnLikeBtn);
            _cachedView.UnLikeBtn.onClick.AddListener(OnUnLikeBtn);
            _playDetail = null;
            _playerData = data;
            _cachedView.PlayerNameText.text = _playerData.Name;
            _cachedView.KillNumText.text = _playerData.killNum.ToString();
            _cachedView.KilledNumText.text = _playerData.KilledNum.ToString();
            _cachedView.ScoreText.text = _playerData.Score.ToString();
            _cachedView.TeamScoreText.text = _playerData.TeamScore.ToString();
            string name = TeamManager.GetTeamColorName(_playerData.TeamId);
            string spriteName = String.Format("img_ball_{0}", name);
            _cachedView.TeamImage.sprite =
                JoyResManager.Instance.GetSprite(spriteName);
            _cachedView.MainPlayImage.SetActiveEx(_playerData.PlayerId == _playerData.MainPlayID);
            _cachedView.MvpImage.SetActiveEx(_playerData.IsMvp);
            _cachedView.WinLine.SetActiveEx(_playerData.IsWin);
            _cachedView.FailLine.SetActiveEx(!_playerData.IsWin);
            _cachedView.HadFollowText.text = String.Format("已关注{0}", _playerData.Name);
            _cachedView.AddFriendBtn.SetActiveEx(_playerData.PlayerId == _playerData.MainPlayID);
            _cachedView.UnLikeBtn.SetActiveEx(false);
            UserManager.Instance.GetDataOnAsync(_playerData.PlayerId, detail =>
            {
                _playDetail = detail;
                if (_playDetail.UserInfoSimple.RelationWithMe.FollowedByMe)
                {
                    _cachedView.HadFollowText.SetActiveEx(true);
                }
                else
                {
                    _cachedView.HadFollowText.SetActiveEx(false);
                }
            }, () => { });
        }

        public bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3())
        {
            return base.Init(rectTransform, resScenary);
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.parent = rectTransform;
        }

        public void OnAddFriendBtn()
        {
            if (_playDetail != null)
            {
                if (_playDetail.UserInfoSimple.RelationWithMe.FollowedByMe)
                {
                    LocalUser.Instance.RelationUserList.RequestRemoveFollowUser(_playDetail,
                        () => { _cachedView.HadFollowText.SetActiveEx(false); });
                }
                else
                {
                    LocalUser.Instance.RelationUserList.RequestFollowUser(_playDetail,
                        () => { _cachedView.HadFollowText.SetActiveEx(true); });
                }
            }
        }

        public void OnLikeBtn()
        {
            _cachedView.UnLikeBtn.SetActiveEx(true);
            if (_playerData != null)
            {
                SocialGUIManager.Instance.GetUI<UICtrlSettlePlayersData>().AddLikePlayer(_playerData.PlayerId);
            }
        }

        public void OnUnLikeBtn()
        {
            _cachedView.UnLikeBtn.SetActiveEx(false);
            if (_playerData != null)
            {
                SocialGUIManager.Instance.GetUI<UICtrlSettlePlayersData>().RemoveLikePlayer(_playerData.PlayerId);
            }
        }
    }
}