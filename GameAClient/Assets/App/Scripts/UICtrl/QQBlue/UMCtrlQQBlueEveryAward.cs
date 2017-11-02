using GameA.Game;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlQQBlueEveryAward : UMCtrlBase<UMViewQQBlueAward> 
    {
        
        private static string _blueVipSpriteName = "icon_blue_{0}";
        private Sprite _sprite;

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }
        public void Unload()
        {
            throw new System.NotImplementedException();
        }

        public void SetAward(int level ,int coinsNum,int diamondNum)
        {
            DictionaryTools.SetContentText(_cachedView.AwardCoinsNum, coinsNum.ToString());
            DictionaryTools.SetContentText(_cachedView.AwarDiamondDiNum, diamondNum.ToString());
            _cachedView.BlueImage.sprite = JoyResManager.Instance.GetSprite(string.Format(_blueVipSpriteName, level));
            if (LocalUser.Instance.User.UserInfoSimple.BlueVipData.BlueVipLevel == level)
            {
                _cachedView.CanGetText.gameObject.SetActive(true);
            }
            else
            {
                _cachedView.CanGetText.gameObject.SetActive(false);
            }
        }
    }
}