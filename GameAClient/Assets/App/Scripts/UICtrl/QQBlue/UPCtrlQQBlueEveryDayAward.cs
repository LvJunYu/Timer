using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQBlueEveryDayAward : UPCtrlQQBlueBase
    {
        private int BlueLevelMax = 7;
        private List<int> _coinsNum = new List<int>() {10, 20, 30, 40, 50, 60, 70};
        private List<int> _diamondNum = new List<int>() {10, 20, 30, 40, 50, 60, 70};
        private int _luxuryCoinsNum = 1500;
        private int _luxuryDiamondNum = 18;
        private int _yearCoinsNum = 200;
        private int _yearDiamondNum = 100;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            Init();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            _isOpen = true;
           
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }

        private void Init()
        {
            for (int i = 1; i <= BlueLevelMax; i++)
            {
                var levelAward = new UMCtrlQQBlueEveryAward();
                levelAward.Init(_cachedView.BlueEveryDayAwardContent, EResScenary.UIHome, Vector3.zero);
                levelAward.SetAward(i, _coinsNum[i - 1], _diamondNum[i - 1]);
            }
            _cachedView.LuxuryCoinsText.text = String.Format("x{0}", _luxuryCoinsNum);
            _cachedView.LuxuryExtraCoinsText.text = String.Format("额外{0}金币", _luxuryCoinsNum);
            _cachedView.LuxuryDiamondText.text = String.Format("x{0}", _luxuryDiamondNum);
            _cachedView.LuxuryExtraDiamondText.text = String.Format("额外{0}钻石", _luxuryDiamondNum);

            _cachedView.YearCoinsText.text = String.Format("x{0}", _yearCoinsNum);
            _cachedView.YearExtraCoinsText.text = String.Format("额外{0}金币", _yearCoinsNum);
            _cachedView.YearDiamondText.text = String.Format("x{0}", _yearDiamondNum);
            _cachedView.YearExtraDiamondText.text = String.Format("额外{0}钻石", _yearDiamondNum);

            _cachedView.LuxuryGetText.SetActiveEx(LocalUser.Instance.User.UserInfoSimple.BlueVipData.IsSuperBlueVip);
            _cachedView.LuxuryNoGetBtn.SetActiveEx(!LocalUser.Instance.User.UserInfoSimple.BlueVipData.IsSuperBlueVip);
            
            _cachedView.YearGetText.SetActiveEx(LocalUser.Instance.User.UserInfoSimple.BlueVipData.IsBlueYearVip);
            _cachedView.YearNoGetBtn.SetActiveEx(!LocalUser.Instance.User.UserInfoSimple.BlueVipData.IsBlueYearVip);
            
        }
    }
}