using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public partial class BlueVipData
    {
        private static string _blueVipSpriteName = "icon_blue_{0}";
        private static string _superBlueVipSpriteName = "icon_luxury_{0}";

        protected override void OnSyncPartial(Msg_SC_DAT_BlueVipData msg)
        {
            base.OnSyncPartial(msg);
//            msg.IsBlueVip = Random.Range(0, 2) == 1;
//            msg.IsSuperBlueVip = Random.Range(0, 2) == 1;
//            msg.IsBlueYearVip = Random.Range(0, 2) == 1;
//            msg.BlueVipLevel = Random.Range(1, 8);
        }

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
                blueImg.sprite = BlueVipSprite;
            }
            else if (_isSuperBlueVip)
            {
                superBlueImg.sprite = SuperBlueVipSprite;
            }
        }
    }
}