using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
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
        public int KillMonsterNum;
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
        private Vector2 _moveOutPos = new Vector2(1000, 0);
        private Vector2 _moveInPos = new Vector2(-44.0f, 0f);
        private Tween _moveTween;

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

        public void MoveContent(int index)
        {
            _cachedView.ContentTrans.anchoredPosition = _moveOutPos;
            _moveTween = _cachedView.ContentTrans.DOAnchorPos(_moveInPos, 0.5f);
            _moveTween.SetDelay(0.5f * index);
            _moveTween.Play();
        }

        public void Hide()
        {
            _cachedView.Trans.SetActiveEx(false);
            IsShow = false;
        }

        public void SetItemData(SettlePlayerData data, bool isCoorepation)
        {
            if (isCoorepation)
            {
                _cachedView.KilledNumText.SetActiveEx(false);
                _cachedView.KillNumText.SetActiveEx(false);
                _cachedView.KillMonsterNum.SetActiveEx(true);
            }
            else
            {
                _cachedView.KilledNumText.SetActiveEx(true);
                _cachedView.KillNumText.SetActiveEx(true);
                _cachedView.KillMonsterNum.SetActiveEx(false);
            }

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
            _cachedView.AddFriendBtn.SetActiveEx(_playerData.PlayerId != _playerData.MainPlayID);
            _cachedView.UnLikeBtn.SetActiveEx(false);
            _cachedView.KillMonsterNum.text = _playerData.KillMonsterNum.ToString();
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

        public new bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3())
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