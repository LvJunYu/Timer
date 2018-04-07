using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorkShop : UIViewResManagedBase
    {
        public Button ReturnBtn;
        public RectTransform TitleRtf;
        public RectTransform PannelRtf;
        public RectTransform BGRtf;
        public Text SlotsNum;
        public GameObject EmptyObj;
        public UITabGroup TabGroup;
        public GameObject[] Pannels;
        public GridDataScroller[] GridDataScrollers;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;

        public Button SelfRecommendEditBtn;
        public Button CancelBtn;
        public Button RemoveBtn;
        public GameObject TipObj;

        public GameObject AddSelfRecommendProjectPanel;
        public GridDataScroller AddSelfRecommendProjectScroller;
        public Button AddConfirmBtn;
        public Button AddCancelBtn;
    }
}