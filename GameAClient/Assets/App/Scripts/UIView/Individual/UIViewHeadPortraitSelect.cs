using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewHeadPortraitSelect : UIViewBase
    {
        public USViewBoostItem [] BoostItems;
        public Text Tip;
        public Button OKBtn;
        public Button CloseBtn;
        public UITagGroup TagGroup;
        public RectTransform Dock;

        public USViewHeadPortraitSelect USViewShop;
    }
}