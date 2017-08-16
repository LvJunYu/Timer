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

        public USViewHeadPortraitSelect USViewShop;
        public Button HeadPortrait1;
        public Button HeadPortrait2;
        public Button HeadPortrait3;
        public Button HeadPortrait4;
        public Button HeadPortrait5;
        public Button HeadPortrait6;

        public Image SeletctedHead1Image;
        public Image SeletctedHead2Image;
        public Image SeletctedHead3Image;
        public Image SeletctedHead4Image;
        public Image SeletctedHead5Image;
        public Image SeletctedHead6Image;
    }
}