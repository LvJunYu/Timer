using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewAdvLvlDetail : UIViewBase
    {
        public Button InfoBtn1;
        public Button InfoBtn2;
        public Button RecordBtn1;
        public Button RecordBtn2;
        public Button RankBtn1;
        public Button RankBtn2;
        public RawImage Cover1;
        public RawImage Cover2;
        public RawImage Cover3;
        public Text FirstName;
        public Text FirstScore;



        public Texture DefaultCover;

        public Button PlayBtn;

        public Button CloseBtn;

        public USViewAdvLvlDetailInfo InfoPanel;
        public USViewAdvLvlDetailRecord RecordPanel;
        public USViewAdvLvlDetailRank RankPanel;
    }
}
