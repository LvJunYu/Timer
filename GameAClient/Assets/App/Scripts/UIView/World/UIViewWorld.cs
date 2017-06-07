using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorld : UIViewBase
    {
        public Texture DefaultCoverTexture;
        
        public Button ReturnBtn;
        public Dropdown DropDown;
        public Button ShowDetailBtn;
        public Button HideDetailBtn;

        public RectTransform ListPanel;
        public GridDataScroller GridScroller;

        public RectTransform InfoPanel;
        public Text TitleText;
        public Text SubTitleText;
        public RawImage UserIcon;
        public Text UserNickNameText;
        public Text UserLevelText;
        public RawImage Cover;
        public Text PlayCountText;
        public Text LikeCountText;
        public Text CompleteRateText;
        public Text Desc;
        public Button FavoriteBtn;
        public Button UnfavoriteBtn;

        public GameObject DetailInfo;
        public UITagGroup TagGroup;
        public Button RecentRecordTab;
        public Button RankRecordTab;
        public Button CommentListTab;

        public GameObject RecentRecordDock;
        public GridDataScroller RecentRecordGridScroller;
        public GameObject RankRecordDock;
        public GridDataScroller RankRecordGridScroller;
        public GameObject CommentListDock;
        public TableDataScroller CommentListTableScroller;

        public InputField CommentInput;
        public Button PostCommentBtn;
    }
}
