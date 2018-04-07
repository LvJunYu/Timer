using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewSetProjectSize : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public Button OKBtn;
        public Button CloseBtn;
        public Toggle Max;
        public Toggle[] ProjectTypeTogs;
        public GameObject[] SelectedMarkHorizontal;
        public GameObject[] SelectedMarkVertical;
        public Button[] Btns;
        public GameObject[] Rects;
    }
}