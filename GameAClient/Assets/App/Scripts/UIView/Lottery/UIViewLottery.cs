using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;

namespace GameA
{
	public class UIViewLottery : UIViewBase
    {
		public Button CloseBtn;

        public Button RaffleTicketlvl1Btn;

        public Button RaffleTicketlvl2Btn;

        public Button RaffleTicketlvl3Btn;

        public Button RaffleTicketlvl4Btn;

        public Button RaffleTicketlvl5Btn;

        public Button RaffleBtn;//modify it when use cat 

        public Transform RoolPanel;

        public UICtrlLottery[] Items;

	    public Text NumberOfTicketlvl1;

        public Text NumberOfTicketlvl2;

        public Text NumberOfTicketlvl3;

        public Text NumberOfTicketlvl4;

        public Text NumberOfTicketlvl5;

        public Text SelectedTicketType;

        public Text RewardExhibition;

	    public Image[] BrightLamp;

	    public Image Reward1;
	    public Image Reward2;
	    public Image Reward3;
	    public Image Reward4;
	    public Image Reward5;
	    public Image Reward6;
	    public Image Reward7;
	    public Image Reward8;

        public Text RewardType1;
        public Text RewardType2;
        public Text RewardType3;
        public Text RewardType4;
        public Text RewardType5;
        public Text RewardType6;
        public Text RewardType7;
        public Text RewardType8;
	    public SkeletonGraphic SpineCat;
	    public Button CatBtn;
    }
}
