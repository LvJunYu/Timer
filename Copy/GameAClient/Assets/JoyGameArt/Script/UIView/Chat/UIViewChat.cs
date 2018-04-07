using UnityEngine;
using SoyEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewChat : UIViewResManagedBase
    {
        public Button CloseBtn;
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public PushButton VoiceBtn;
        public Button FaceBtn;
        public Button SendTexBtn;
        public GridDataScroller FriendGridDataScroller;
        public RectTransform FriendItemRtf;
        public ScrollRect[] ScrollRect;
        public InputField InptField;
        public UITabGroup TabGroup;
        public TableDataScroller[] TableDataScrollers;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
    }
}