/********************************************************************
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
    public class UMViewDialog : UMViewResManagedBase
    {
        public Button CloseBtn;
        public Text Title;
        public Text Content;
        public GameObject SeperatorDock;
        public GameObject ButtonListDock;
        public Button[] ButtonAry;
        public Text[] ButtonTextAry;
        public Image[] ButtonBgAry;
        public Sprite[] BgSprite;
        public Image FullScreenMask;
    }
}

