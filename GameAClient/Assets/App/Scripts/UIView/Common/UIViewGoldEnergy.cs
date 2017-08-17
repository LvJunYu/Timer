using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGoldEnergy : UIViewBase {

        public Text GoldNumber;
        public Text DiamondNumber;

        public Button EnergyPlusBtn;
        public Button GoldPlusBtn;
        public Button DiamondPlusBtn;

        public Image EnergyBar;
        public Text EnergyNumber;

        public GameObject Energy;
        public GameObject Gold;
        public GameObject Diamond;
    }
}