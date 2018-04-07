using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewAdvLvlDetail : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public RawImage Cover1;
        public Text FirstName;
        public Text FirstScore;
        public Text Title;
        public Texture DefaultCover;
        public Button PlayBtn;
        public Button CloseBtn;
        public USViewAdvLvlDetailInfo InfoPanel;
//        public USViewAdvLvlDetailRecord RecordPanel;
        public USViewAdvLvlDetailRank RankPanel;
    }
}