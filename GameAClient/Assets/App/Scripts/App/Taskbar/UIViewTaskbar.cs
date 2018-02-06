/********************************************************************
** Filename : UIViewTaskbar
** Author : Dong
** Date : 2015/4/30 16:08:41
** Summary : UIViewTaskbar
***********************************************************************/

using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewTaskbar : UIViewResManagedBase
    {
//        public UITaskButton Soy;
//        public UITaskButton News;
//        public UITaskButton Create;
//        public UITaskButton Daily;
//        public UITaskButton Me;
//        public RectTransform ScaleRoot;
        //public Button SignUpBtn;
        public Button Account;

        public Button ServiceBtn;
        public Button ForumBtn;
        public Button RechargeBtn;

        public Button PersonalInformation;

        /// <summary>
        /// 选择游戏按钮
        /// </summary>
        public Button WorkshopButton;

        public GameObject Workshop;
        public GameObject WorkshopDisable;

        /// <summary>
        /// 世界切换按钮
        /// </summary>
        public Button WorldButton;

        public GameObject World;
        public GameObject WorldDisable;

        /// <summary>
        /// 对战按钮
        /// </summary>
        public Button BattleButton;

        public GameObject Battle;
        public GameObject BattleDisable;

        /// <summary>
        ///单人游戏按钮
        /// </summary>
        public Button SingleModeButton;

        public GameObject SingleMode;

        public Button StoryGameButton;
        public GameObject StoryGameObj;

//		public GameObject SingleModeDisable;
        /// <summary>
        /// 单人模式父物体，做动画用
        /// </summary>
        public Transform SingleModeParent;

        /// <summary>
        /// 人物按钮，时装商店
        /// </summary>
        public Button AvatarBtn;

        public Text AvatarText;

        //public GameObject AvatarDisable;
        /// <summary>
        /// 人物动画
        /// </summary>
        public Button LotteryBtn;

        public GameObject Lottery;
//        public GameObject LotteryDisable;

        public Button MailBoxBtn;

        public GameObject MailBox;
//        public GameObject MailBoxDisable;

        public Button FriendsBtn;

        public GameObject Friends;
//        public GameObject FriendsDisable;

        public Button PuzzleBtn;

        public GameObject Puzzle;
//        public GameObject PuzzleDisable;

        public Button TrainBtn;

        public GameObject Train;
//	    public GameObject TrainDisable;

        public Button AchievementBtn;

        public GameObject Achievement;
//	    public GameObject AchievementDisable;

        public Button QQHallBtn;
        public GameObject QQHall;
        public Image QqHallImage;

        public Button QQBlueBtn;
        public GameObject QQBlue;
        public Image QQBlueLight;
        public Image QqOpenImage;

        public Button AnnouncementBtn;
        public GameObject Announcement;
        public Button ChatBtn;

        /// <summary>
        /// 人物动画
        /// </summary>
        //public Spine.Unity.SkeletonAnimation PlayerAvatarAnimation;
        /// <summary>
        /// 人物摄像机
        /// </summary>
        //public Camera AvatarRenderCamera;
        /// <summary>
        /// 人物
        /// </summary>
        //public RawImage AvatarImage;
        /// <summary>
        /// 换装按钮
        /// </summary>
        public SkeletonGraphic SpineCat;

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

        public Image AdventureExperience;

        public Image CreatorExperience;

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

        //蓝钻
        public GameObject BlueVipDock;

        public Image BlueImg;
        public Image SuperBlueImg;
        public Image BlueYearVipImg;

        public Button UnlockAll;

        /// <summary>
        /// 武器按钮
        /// </summary>
        public Button Weapon;

        public GameObject WeaponObject;

        /// <summary>
        /// 武器按钮
        /// </summary>
        public Button HandBook;

        public GameObject HandBookObject;

        public USViewChat HomeChat;
    }
}