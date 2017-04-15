  /********************************************************************
  ** Filename : UIViewUserInfo.cs
  ** Author : quan
  ** Date : 2016/6/11 19:18
  ** Summary : UIViewUserInfo.cs
  ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewUserInfo : UIViewBase
    {
        public Button FollowBtn;
        public Button UnfollowBtn;
        public RawImage UserHeadImg;
        public Image SexImg;
        public Text NickName;
        public Text Profile;
        public UIUserLevel PlayerLevel;
        public UIUserLevel CreatorLevel;
        public Button FollowListBtn;
        public Text FollowListText;
        public Button FollowerListBtn;
        public Text FollowerListText;

        public UITagGroup TagGroup;

        public Button PublishedProjectBtn;
//        public UISoyPublishedProjectList SoyPublishedProjects;

        public Button ProjectPlayHistoryBtn;

        public Texture DefaultUserHeadTexture;
    }
}
