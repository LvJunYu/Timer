using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlQQAward : UMCtrlBase<UMViewQQAward>
    {
        private string _diamondSpriteName = "icon_diam_1";
        private string _coinSpriteName = "icon_gold";
        private Sprite _awardSprite;
        private string _spriteName;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public void SetAward(UICtrlQQHall.Award award)
        {
            switch (award.Type)
            {
                case AwardEnum.Diamond:
                    _spriteName = _diamondSpriteName;
                    break;
                case AwardEnum.Coin:
                    _spriteName = _coinSpriteName;
                    break;
            }
            if (JoyResManager.Instance.TryGetSprite(_spriteName, out _awardSprite) )
            {
                _cachedView.AwardImage.sprite = _awardSprite;
            }
            DictionaryTools.SetContentText(_cachedView.AwardNum, award.Num.ToString());
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public enum AwardEnum
        {
            Diamond,
            Coin
        }
    }
}