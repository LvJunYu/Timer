using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewProjectDetail : UIViewResManagedBase
    {
        public Texture DefaultCoverTexture;
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public Button CloseBtn;
        public Button PlayBtn;
        public Text ProjectId;
        public Text TitleText;
        public Text Desc;
        public GameObject BlueVipDock;
        public Image BlueImg;
        public Image SuperBlueImg;
        public Image BlueYearVipImg;
        public RawImage UserIcon;
        public Button HeadBtn;
        public Text UserNickNameText;
        public Text AdvLevelText;
        public Text CreateLevelText;
        public Button FollowBtn;
        public Text FollowBtnTxt;
        public Text ProjectCreateDate;
        public RawImage Cover;
        public Text PlayCountText;
        public Text LikeCountTxt;
        public Text ScoreTxt;
        public Button FavoriteBtn;
        public Text FavoriteTxt;
        public Button DownloadBtn;
        public Button ShareBtn;
        public Button DeleteBtn;
        public Button EditBtn;
        public Toggle GoodTog;
        public Toggle BadTog;
        public InputField CommentInput;
        public Button PostCommentBtn;
        public Text CommentCount;
        public Text CommentSelectedCount;
        public UITabGroup TabGroup;
        public GridDataScroller RoomGridDataScroller;
        public TableDataScroller RecentGridDataScroller;
        public TableDataScroller CommentTableScroller;
        public GridDataScroller RankGridDataScroller;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;

        public Text PlayerCount;
        public Text LifeCount;
        public Text ReviveTime;
        public Text ReviveProtectTime;
        public Text TimeLimit;
        public Text TimeOverCondition;
        public Text WinScoreCondition;
        public Text ArriveScore;
        public Text CollectGemScore;
        public Text KillMonsterScore;
        public Text KillPlayerScore;
        public Button CreateBtn;
        public GameObject DownDock;

        public Text RpgTileText;
        public Text RpgDescText;
        public GameObject UserObj;
        public GameObject TileObj;
        public RectTransform CommentRect;

        //GM
        public GameObject GMPanel;
        public Button RecommendWaitingBtn;
        public Button RecommendPrepareBtn;
        public Button RemoveWaitingBtn;
        public Button UpdataProjectBtn;
        public Button RemoveProjectBtn;
    }
}