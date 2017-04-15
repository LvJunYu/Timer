﻿/********************************************************************
** Filename : UMViewDialog.cs
** Author : quan
** Date : 15/7/5 下午3:11
** Summary : UMViewDialog.cs
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewDialog : UMViewBase
    {
        public Image ContentBg;
        public Text Title;
        public Text Content;
        public Button[] ButtonAry;
        public Text[] ButtonTextAry;
        public Image[] ButtonBgAry;
        public Sprite[] BgSprite;
        public Image FullScreenMask;
    }
}

