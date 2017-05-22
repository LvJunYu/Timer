  /********************************************************************
  ** Filename : UIViewFollowUserList.cs
  ** Author : quan
  ** Date : 2016/6/10 10:02
  ** Summary : UIViewFollowUserList.cs
  ***********************************************************************/
using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewFollowedUserList : UIViewBase
    {
        public GridDataScroller GridScroller;
        public UIRefreshController RefreshController;
        public GameObject Content;
        public GameObject EmptyTip;
        public GameObject ErrorTip;
    }
}

