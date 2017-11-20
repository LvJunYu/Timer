using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewDialog : UMViewResManagedBase
    {
        public Button CloseBtn;
        public Text Title;
        public Text Content;
        public GameObject SeperatorDock;
        public GameObject ButtonListDock;
        public Button[] ButtonAry;
        public Text[] ButtonTextAry;
        public Image[] ButtonBgAry;
        public Sprite[] BgSprite;
        public Image FullScreenMask;
    }
}

