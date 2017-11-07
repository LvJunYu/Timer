using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public partial class BlueVipData
    {
        private static string _blueVipSpriteName = "icon_blue_{0}";
        private static string _superBlueVipSpriteName = "icon_luxury_{0}";

//        protected override void OnSyncPartial()
//        {
//            base.OnSyncPartial();
//            _isBlueYearVip = true;
//            _blueVipLevel = 3;
//            _isBlueVip = true;
//        }

        public Sprite BlueVipSprite
        {
            get { return JoyResManager.Instance.GetSprite(string.Format(_blueVipSpriteName, _blueVipLevel)); }
        }

        public Sprite SuperBlueVipSprite
        {
            get { return JoyResManager.Instance.GetSprite(string.Format(_superBlueVipSpriteName, _blueVipLevel)); }
        }

        public void RefreshBlueVipView(GameObject blueVipDock, Image blueImg, Image superBlueImg, Image lueYearVipImg)
        {
            blueVipDock.SetActive(_isBlueVip || _isSuperBlueVip || _isBlueYearVip);
            blueImg.SetActiveEx(!_isSuperBlueVip && _isBlueVip);
            superBlueImg.SetActiveEx(_isSuperBlueVip);
            lueYearVipImg.SetActiveEx(_isBlueYearVip);
            if (!_isSuperBlueVip && _isBlueVip)
            {
                blueImg.sprite = LocalUser.Instance.User.UserInfoSimple.BlueVipData.BlueVipSprite;
            }
            else if (_isSuperBlueVip)
            {
                superBlueImg.sprite = LocalUser.Instance.User.UserInfoSimple.BlueVipData.SuperBlueVipSprite;
            }
        }
    }
}