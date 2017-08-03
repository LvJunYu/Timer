﻿/********************************************************************
** Filename : UMViewItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 6:48:19
** Summary : UMViewItem
***********************************************************************/

using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewItem : UMViewBase
    {
        public EventTriggerListener EventTrigger;
        public Image SpriteIcon;
        public Transform ShadowTrans;
        public GameObject Unlimited;
        public Text Number;
        public GameObject Arrow;
    }
}