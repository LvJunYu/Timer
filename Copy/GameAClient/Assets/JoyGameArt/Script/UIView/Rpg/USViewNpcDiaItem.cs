using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USViewNpcDiaItem : USViewBase
    {
        public Text DiaText;
        public Button UpBtn;
        public Button DownBtn;
        public Button DelteBtn;
        public Image IconImage;
        public Text IndexText;
        public GameObject DisableObj;
        public GameObject EnableObj;
        public DragHelper DragHelper;
        public Image SelectImage;
        public CtrlDrag CtrlDrag;
        public Button EditDiaBtn;
        public OnPoinHover EditDiaBtnMask;
        public OnPoinHover EditBtnPointHover;
    }
}