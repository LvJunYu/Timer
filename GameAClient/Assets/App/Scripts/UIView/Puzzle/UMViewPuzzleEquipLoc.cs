﻿using System.Collections;
using System;
using UnityEngine;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 拼图装备栏位
    /// </summary>
    public class UMViewPuzzleEquipLoc : UMViewBase
    {
        public Image ActiveImage;
        public GameObject LockObj;
        public Text UnlockLvTxt;
        public GameObject PuzzleItem;
        public Button EquipBtn;
    }
}
