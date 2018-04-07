using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewProjectDetailShare : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public GameObject EmptyPannel;
        public GameObject SharePannel;
        public Button CloseBtn;
        public Button OKBtn;
        public Button CancelBtn;
        public Button AddFriendsBtn;
        public GridDataScroller GridDataScroller;
        public Toggle AllSelectTog;
    }
}