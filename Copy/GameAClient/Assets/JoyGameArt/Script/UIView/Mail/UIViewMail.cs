using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewMail : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public UITabGroup TabGroup;
        public GameObject EmptyObj;
        public GridDataScroller[] GridDataScrollers;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
    }
}