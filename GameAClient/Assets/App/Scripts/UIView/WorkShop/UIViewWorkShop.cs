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
        public Button ChangeModeBtn;
        public Text ChangeModeBtnText;



        public GameObject Private;
        // private project info detail
        public RawImage Cover;
        public Button EditBtn;
        public Text Title;
        public InputField TitleInput;
        public Button EditTitleBtn;
        public Button ConfirmTitleBtn;
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

    }
}
