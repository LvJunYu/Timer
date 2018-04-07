using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewShadowBattle : UIViewResManagedBase
    {
        public Text NickName;
        public RawImage UserHead;
        public Texture DefaultHeadTexture;
        public GameObject BlueVipDock;
        public Image BlueImg;
        public Image SuperBlueImg;
        public Image BlueYearVipImg;
        public Image MaleIcon;
        public Image FemaleIcon;
        public Text AdvLevel;
        public Text CreatorLevel;
        public Text Score;
        public Button CancelBtn;
        public Button PlayBtn;
        public USViewGameFinishReward[] Rewards;
        public Text CountDownTxt;
    }
}