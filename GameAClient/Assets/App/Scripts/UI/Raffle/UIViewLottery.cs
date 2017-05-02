using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

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

        public Transform mRoolPanel;



        public UICtrlLottery[] Items;

	    public Text NumberOfTicketlvl1;

        public Text NumberOfTicketlvl2;

        public Text NumberOfTicketlvl3;

        public Text NumberOfTicketlvl4;

        public Text NumberOfTicketlvl5;

        public Text SelectedTicketType;

        public Text RewardExhibition;




    }
}
