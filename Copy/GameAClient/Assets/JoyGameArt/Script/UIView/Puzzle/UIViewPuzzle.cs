using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewPuzzle : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
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
