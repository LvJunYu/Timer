using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewProjectDetail : UIViewBase
    {
        public Texture DefaultCoverTexture;
        
        public Button CloseBtn;
        public Button BgBtn;
        public Button PlayBtn;
        public Text TitleText;
        public Text SubTitleText;
        public RawImage UserIcon;
        public Text UserNickNameText;
        public Text UserLevelText;
        public Text CreateTimeText;
        public RawImage Cover;
        public Text PlayCountText;
        public Text LikeCountText;
        public Text CompleteRateText;

        public UITabGroup TabGroup;
        public Button DetailTab;
        public Button DetailTab2;
        public Button RecordRankTab;
        public Button RecordRankTab2;
        public Button CommentListTab;
        public Button CommentListTab2;
        
        public GameObject DetailDock;
        public Text Desc;
        public RectTransform RecentPlayUserDock;
        public Button FavoriteBtn;
        public Button UnfavoriteBtn;
        public Text FavoriteCount;

        public GameObject RecordRankDock;
        public GridDataScroller RecordRankGridScroller;
        
        public GameObject CommentListDock;
        public TableDataScroller CommentListTableScroller;
        public InputField CommentInput;
        public Button PostCommentBtn;

        public Toggle FollowToggle;
        public Text   FollowText;
        public Toggle BlockToggle;
        public Text   BlockText;
    }
}
