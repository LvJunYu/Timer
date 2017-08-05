using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorkShop : UIViewBase
    {
        public Texture DefaultCoverTexture;
        
        public Button ReturnBtn;
        public Button PublishedBtn;
        public Button WorkingOnBtn;

        public Text ChangeModeBtnText;
        public GameObject Private;
        // private project info detail
        public RawImage Cover;
        public Button EditBtn;
        public Text Title;
        public Text SubTitle;

        public GameObject HummerIcon;
        public GameObject PlayIcon;


        public InputField TitleInput;
        public InputField SubTitleInput;

        public Button EditTitleBtn;
        public Button EditSubTitleBtn;
        public Button ConfirmTitleBtn;
        public Button ConfirmSubTitleBtn;
        public Text Desc;
        public InputField DescInput;
        public Button EditDescBtn;
        public Button ConfirmDescBtn;
        public Button PublishBtn;
        public Button DeleteBtn;
        // private project list
        public GridDataScroller PrivateProjectsGridScroller;
        public Button NewProjectBtn;

        public GameObject Public;
        public Text MakerLvl;
        public Image MakerExpFillImg;
        public Text MakerExpText;
        public Text PublishedProjectCnt;
        public Text ServedPlayerCnt;
        public Text LikedCnt;
        public Text MostPopulerProjectName;
        public GridDataScroller PublicProjectsGridScroller;

        public GameObject[] ObjectsShowWhenEmpty;
        public GameObject Data;

    }
}
