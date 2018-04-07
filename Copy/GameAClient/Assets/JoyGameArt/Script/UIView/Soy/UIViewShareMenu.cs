/********************************************************************
  ** Filename : UIViewShareMenu.cs
  ** Author : quan
  ** Date : 11/15/2016 1:38 PM
  ** Summary : UIViewShareMenu.cs
  ***********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewShareMenu : UIViewResManagedBase
    {
        public RectTransform ContentDock;
        public CanvasGroup CanvasGroup;
        public Button BackgroundBtn;
        public Image BackgroundImage;
        public Button CancelBtn;

        public Button ShareToWechatFriendsBtn;
        public Button ShareToWechatMomentsBtn;
        public Button ShareToQQFriendsBtn;
        public Button ShareToQZoneBtn;
    }
}