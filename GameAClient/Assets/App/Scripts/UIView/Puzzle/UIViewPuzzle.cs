using System.Collections;
using System;
using UnityEngine;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewPuzzle : UIViewBase
    {
        public Button CloseBtn;
        public RectTransform PuzzleLocsGrid;
        public RectTransform PuzzleItemGrid;
        public ToggleGroup ToggleGroup;
        public Toggle Qulity;
        public Toggle Level;
        public Toggle Func;
        public GridDataScroller PuzzleItemGridDataScroller;
        public ScrollRect PuzzleItemSrollRect;
    }
}
