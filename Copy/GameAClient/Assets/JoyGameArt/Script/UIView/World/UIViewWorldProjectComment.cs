using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorldProjectComment : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public Button OKBtn;
        public Button CancelBtn;
        public Toggle GoodTog;
        public Toggle BadTog;
        public InputField CommentInput;
    }
}