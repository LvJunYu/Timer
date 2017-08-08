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
    public class UIViewPuzzleDetail : UIViewBase
    {
        public Button CloseBtn;
        public Button ActiveBtn;
        public Button EquipBtn;
        public RectTransform PuzzleItemPos;
        public RectTransform PuzzleFragmentGrid;
        public Text LvTxt;
        public Text NameTxt;
        public Text DescTxt;
    }
}
