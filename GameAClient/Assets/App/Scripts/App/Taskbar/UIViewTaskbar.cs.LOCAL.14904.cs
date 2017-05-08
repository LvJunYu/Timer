/********************************************************************
** Filename : UIViewTaskbar
** Author : Dong
** Date : 2015/4/30 16:08:41
** Summary : UIViewTaskbar
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UIViewTaskbar : UIViewBase
    {
//        public UITaskButton Soy;
//        public UITaskButton News;
//        public UITaskButton Create;
//        public UITaskButton Daily;
//        public UITaskButton Me;
//        public RectTransform ScaleRoot;

		public Button WorkshopButton;
		public Button WorldButton;
		public Button SingleModeButton;
        public Button LotteryButton;
        public Button AvatarBtn;
		public Spine.Unity.SkeletonAnimation PlayerAvatarAnimation;
		public Camera AvatarRenderCamera;
		public RawImage AvatarImage;

		public Button TestChangeAvatarBtn;
		public Button DebugClearUserDataBtn;


		// user info
		public Text NickName;
		public RawImage UserHeadAvatar;
		public Texture DefaultUserHeadTexture;
		public Image MaleIcon;
		public Image FemaleIcon;
		public Text AdventureLevel;
		public Text CreatorLevel;

		public Text DiamondCount;
		public Text MoneyCount;
		public Button MoneyBtn;
		public Button DiamondBtn;

    }
}
