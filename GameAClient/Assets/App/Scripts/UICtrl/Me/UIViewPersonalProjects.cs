/********************************************************************
** Filename : UIViewMyCreatedProjects
** Author : Dong
** Date : 2015/4/30 16:34:49
** Summary : UIViewMyCreatedProjects
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPersonalProjects : UIViewBase
    {
        public RectTransform BottomMenuDock;
        public Button DeleteButton;
        public Text DeleteButtonText;
        public Button CancelButton;

        public GridDataScroller GridScroller;
        public UIRefreshController RefreshController;
        public GameObject Content;
        public GameObject EmptyTip;
        public GameObject ErrorTip;

        public Button EditButtonRightResource;
        public Button CancelButtonRightResource;
    }
}
