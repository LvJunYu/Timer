  /********************************************************************
  ** Filename : UIViewPublishedPorjects.cs
  ** Author : quan
  ** Date : 2016/6/10 16:22
  ** Summary : UIViewPublishedPorjects.cs
  ***********************************************************************/
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPublishedPorjects: UIViewBase
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

