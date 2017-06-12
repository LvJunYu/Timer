﻿using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorld : UIViewBase
    {
        public Texture DefaultCoverTexture;
        
        public Button ReturnBtn;
        public Dropdown MenuDropDown;
        public Button TopBtn;
        public Button ShowDetailBtn;
        public Button HideDetailBtn;

        public RectTransform ListPanel;
        public GridDataScroller GridScroller;

        public RectTransform InfoPanel;
        public Button PlayBtn;
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

        public RectTransform DetailPanel;
        public UITabGroup TabGroup;
        public Button RecentRecordTab;
        public Button RecentRecordTab2;
        public Button RecordRankTab;
        public Button RecordRankTab2;
        public Button CommentListTab;
        public Button CommentListTab2;

        public GameObject RecentRecordDock;
        public GridDataScroller RecentRecordGridScroller;
        public GameObject RecordRankDock;
        public GridDataScroller RecordRankGridScroller;
        public GameObject CommentListDock;
        public TableDataScroller CommentListTableScroller;

        public InputField CommentInput;
        public Button PostCommentBtn;
    }
}
