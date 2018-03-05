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
    }

    public class UMCtrlSettlePlayersData : UMCtrlBase<UMViewSettlePalyerDataItem>, IUMPoolable
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
            IsShow = true;
        }

        public void Hide()
        {
            IsShow = false;
        }

        public void SetItemData(SettlePlayerData data)
        {
            _playerData = data;
            _cachedView.PlayerNameText.text = _playerData.Name;
            _cachedView.KillNumText.text = _playerData.killNum.ToString();
            _cachedView.KilledNumText.text = _playerData.KilledNum.ToString();
            _cachedView.ScoreText.text = _playerData.Score.ToString();
        }

        public bool Init(RectTransform rectTransform, EResScenary resScenary, Vector3 localpos = new Vector3())
        {
            throw new NotImplementedException();
        }

        public void SetParent(RectTransform rectTransform)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}