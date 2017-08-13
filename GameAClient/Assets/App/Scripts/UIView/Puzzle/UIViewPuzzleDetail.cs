﻿using System.Collections;
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
        public GameObject Unable_Active;
        public GameObject Unable_Equip;
        public Text ActiveTxt;
        public RectTransform PuzzleItemPos;
        public HorizontalLayoutGroup PuzzleFragmentGrid;
        public Text LvTxt;
        public Text NameTxt;
        public Text DescTxt;
        public Text CostNumTxt;
        public Transform HalfFragImages;
        public Transform QuarterFragImages;
        public Transform SixthFragImages;
        public Transform NinthFragImages;
    }
}
