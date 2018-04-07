using System.Collections;
using System;
using UnityEngine;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 拼图装备栏位
    /// </summary>
    public class UMViewPuzzleEquipLoc : UMViewResManagedBase
    {
        public Image ActiveImage;
        public GameObject LockObj;
        public Text UnlockLvTxt;
        public Image PuzzleImg;
        public Button EquipBtn;
        public GameObject BG;
        public Image BoardImg;
    }
}
