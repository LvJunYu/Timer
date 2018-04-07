using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewPuzzleDetail : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public Button ActiveBtn;
        public Button EquipBtn;
        public GameObject Unable_Active;
        public GameObject Unable_Equip;
        public Text ActiveTxt;
        public RectTransform PuzzleItemPos;
        public HorizontalLayoutGroup PuzzleFragmentGrid;
        public Text LvTxt;
        public Text NameTxt;
        public Text DescTxt;
        public Text CostNumTxt;
        public ScrollRect FragsScrollRect;
    }
}
