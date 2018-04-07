using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewInfoNotification : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public UITabGroup TabGroup;
        public GameObject EmptyObj;
        public RectTransform ReplayPannel;
        public Text ReplyTxt;
        public InputField ReplyInputField;
        public Button MaskBtn;
        public Button CancelBtn;
        public Button SendBtn;
        public Button ClearBtn;
        public TableDataScroller[] TableDataScrollers;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
        public GameObject[] RedAry;
    }
}