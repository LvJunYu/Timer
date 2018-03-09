using System;
using UnityEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine.UI;
using SoyEngine;
using GameA;
using GameA.Game;

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
    }

    public class UMCtlSettlePalyerDataItem : UMCtrlBase<UMViewSettlePalyerDataItem>, IUMPoolable
    {
        #region 变量

        private SettlePlayerData _playerData;

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
        }

        public bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3())
        {
            return base.Init(rectTransform, resScenary);
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.parent = rectTransform;
        }
    }
}