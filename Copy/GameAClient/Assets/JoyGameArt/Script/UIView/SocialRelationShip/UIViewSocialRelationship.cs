using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSocialRelationship : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public Button SearchBtn;
        public InputField SeachInputField;
        public UITabGroup TabGroup;
        public GameObject EmptyObj;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
        public GridDataScroller[] GridDataScrollers;
    }
}