using System.Collections;
using System;
using UnityEngine;
using SoyEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public class UMViewPuzzleFragmentItem : UMViewBase
    {
        public List<RectTransform> Rects;
        public List<Image> Images;
        public List<Image> Image_Disables;
        public List<Text> HaveNumTxts;
        public List<Text> OrderTxts;
        public Color disableColor;
    }
}
