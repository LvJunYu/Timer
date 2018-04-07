using System.Collections;
using System;
using UnityEngine;
using SoyEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图详情中的拼图
    /// </summary>
    public class UMViewPuzzleDetailItem : UMViewResManagedBase
    {
        public Image Puzzle_Active;
        public List<RectTransform> RectTFs;
        public Color DisableColor;
        public Image Puzzle_Board;
    }

}
