/********************************************************************
** Filename : UIViewItem
** Author : Dong
** Date : 2015/7/29 星期三 下午 3:30:33
** Summary : UIViewItem
***********************************************************************/

using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewItem : UIViewBase
    {
        public Button Actor;
        public Button Earth;
        public Button Mechanism;
        public Button Monster;
        public Button Collection;
        public Button Decoration;
        public ScrollRect ScrollRect;
        public HorizontalLayoutGroup LayoutGroup;

	    public GameObject EditorTabbarGo;
    }
}