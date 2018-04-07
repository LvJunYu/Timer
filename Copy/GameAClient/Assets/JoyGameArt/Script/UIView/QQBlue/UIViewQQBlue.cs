using Kent.Boogaart.KBCsv;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewQQBlue : UIViewResManagedBase
    {
        [Header("Total")] public Button CloseButton;
        public Button[] DredgeBtnGroup;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
        public GameObject[] AwardPanel;
        public UITabGroup TabGroup;
        [Header("NewPlayAward")] 
        public Button ColltionButtonNewPlayer;

        public Button ColltionNoBlueNewPlayer;
        public RectTransform NewPlayerAwardContent;
        [Header("EveryDayAward")] 
        public Button ColltionEveryDayPlayer;
        public Button ColltionNoBlueEveryDay;
        [Header("EveryDayAward/Luxury")]
        public Text LuxuryExtraCoinsText;
        public Text LuxuryExtraDiamondText;
        public Text LuxuryCoinsText;
        public Text LuxuryDiamondText;
        public GameObject LuxuryGetText;
        public GameObject LuxuryNoGetBtn;
        [Header("EveryDayAward/Year")]
        public Text YearExtraCoinsText;
        public Text YearExtraDiamondText;
        public Text YearCoinsText;
        public Text YearDiamondText;
        public GameObject YearGetText;
        public GameObject YearNoGetBtn;
        [Header("EveryDayAward/LeftPanel")]
        public RectTransform BlueEveryDayAwardContent;
      


        [Header("GrowAward")]
        public GridDataScroller GrowAwardDataScroller;

        [Header("Introduce")]
        public Button IntroduceBlueBtn;
    }
}