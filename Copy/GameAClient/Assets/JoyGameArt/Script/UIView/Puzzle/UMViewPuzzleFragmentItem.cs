using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public class UMViewPuzzleFragmentItem : UMViewResManagedBase
    {
        public List<RectTransform> Rects;
        public List<Image> Images;
        public List<Image> Image_Disables;
        public List<Text> HaveNumTxts;
        public List<Text> OrderTxts;
        public List<Image> MaskImgs;
        public List<Image> OutlineImgs;
        public Color disableColor;
    }
}
