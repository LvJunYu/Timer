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
        /// <summary>
        /// 选择游戏按钮
        /// </summary>
		public Button WorkshopButton;
        /// <summary>
        /// 世界切换按钮
        /// </summary>
		public Button WorldButton;
        /// <summary>
        ///单人游戏按钮
        /// </summary>
		public Button SingleModeButton;
        /// <summary>
        /// 人物按钮，时装商店
        /// </summary>
		public Button AvatarBtn;
        /// <summary>
        /// 人物动画
        /// </summary>
		public Spine.Unity.SkeletonAnimation PlayerAvatarAnimation;
        /// <summary>
        /// 人物摄像机
        /// </summary>
		public Camera AvatarRenderCamera;
        /// <summary>
        /// 人物
        /// </summary>
		public RawImage AvatarImage;
        /// <summary>
        /// 换装按钮
        /// </summary>
		public Button TestChangeAvatarBtn;
        /// <summary>
        /// 清空用户数据按钮
        /// </summary>
		public Button DebugClearUserDataBtn;


		// user info
        /// <summary>
        /// 用户名字
        /// </summary>
		public Text NickName;
        /// <summary>
        /// 头像
        /// </summary>
		public RawImage UserHeadAvatar;
        /// <summary>
        /// 默认图片
        /// </summary>
		public Texture DefaultUserHeadTexture;
        /// <summary>
        /// 男性别的小图标
        /// </summary>
		public Image MaleIcon;
        /// <summary>
        /// 女性别的小图标
        /// </summary>
		public Image FemaleIcon;
        /// <summary>
        /// 冒险经验，等级
        /// </summary>
		public Text AdventureLevel;
        /// <summary>
        /// 匠人经验，等级
        /// </summary>
		public Text CreatorLevel;
        /// <summary>
        /// 点券，充值的，钻石
        /// </summary>
		public Text DiamondCount;
        /// <summary>
        /// 游戏中的金钱
        /// </summary>
		public Text MoneyCount;
        /// <summary>
        /// 内购游戏得到的金币
        /// </summary>
		public Button MoneyBtn;
        /// <summary>
        /// 内购钻石
        /// </summary>
		public Button DiamondBtn;

    }
}
