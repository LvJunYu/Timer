using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWeapon : UIViewBase
    {
        public Image EffctImage;
        public Image UnlockSystemTitle;
        public Image UnlockAbilityTitle;
        public Image RewardLight;
        public Image UnlockLight;
        public Image AbilityLight;
        public Text Tip;
        public Button BGBtn;

        public USViewRewardItem[] ItemList;
    }
}
