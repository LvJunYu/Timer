using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSingleMode : UIViewResManagedBase
    {
        public ScrollRectEx2 ChapterScrollRect;
        public RectTransform TitleRtf;
        public RectTransform PanelRtf;
        public RectTransform BGRtf;
        public RectTransform LeftBtnRtf;
        public RectTransform RightBtnRtf;
        public Button ReturnBtn;
        public Button MatchBtn;
        public Button EncBtn;
        public Button NextBtn;
        public Button PrevBtn;

        public RectTransform NextBtnRtf;
        public RectTransform PreBtnRtf;

        public Text ChapterTitle;
        public Text StarNumber;

        public USViewChapter[] Chapters;
        public Image InputBlock;

        public CanvasGroup[] ChapterBg;
    }
}