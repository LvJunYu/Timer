using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewProjectDetail : UIViewBase
    {
        public Texture DefaultTexture;
        public Text ProjectName;
        public RawImage ProjectCover;
        public UIProjectCompleteRate ProjectCompleteRate;
        public Button AuthorBtn;
        public Text AuthorName;
        public Image SexImg;
        public RawImage UserIcon;
        public Text Summary;
        public Text CreateTime;
        public Button FollowBtn;
        public Button UnfollowBtn;
        public UITagGroup TagGroup;
        public Button SummaryBtn;
        public GameObject SummaryDock;
        public Button RecentBtn;
        public GameObject RecentDock;
        public Button RankBtn;
        public GameObject RankDock;
        public Button CommentBtn;
        public Text CommentBtnText;
        public GameObject CommentDock;
        public UICommentInput CommentInput;

        public GameObject RecentPlayedProjectUserTip;
        public UIRecentPlayedProjectUserList RecentPlayedProjectUserList;

        public LayoutElement ProjectRecentListLayoutElement;
        public LayoutElement ProjectRecentListContentLayoutElement;

        public LayoutElement ProjectPlayListLayoutElement;
        public LayoutElement ProjectPlayListContentLayoutElement;

//        public UICommentList CommentList;
        public LayoutElement CommentListLayoutElement;
        public LayoutElement CommentContentLayoutElement;

        public Button PlayBtn;

        public UIRateStarArray RateStarAry;
        public Text RateCount;

        public Image ProjectCategoryImage;
        public Sprite[] ProjectCategorySpriteAry;

        public Button CollapseSumBtn;
        public Text CollapseSumText;
        public GameObject CollapsibleSumDock;

        public Button FavoriteBtn;
        public Text FavoriteBtnText;
        public Text FavoriteCountText;
        public Image FavoriteBtnImage;
        public Sprite FavoriteSprite;
        public Sprite NotFavoriteSprite;

        public Button DownloadBtn;
        public Text DownloadBtnText;
        public Text DownloadCountText;
        public Image DownloadPriceImage;

        public Button ShareBtn;
        public Text ShareCountText;

        public Button LikeBtn;
        public Text LikeCountText;
        public Image LikeBtnImage;
        public Sprite LikeSprite;
        public Sprite DislikeSprite;

        public GameObject AdminDock;
        public Button AddMixRecommend;
        public Button AddOwnRecommend;
    }

}