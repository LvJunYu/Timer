using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorkShopEdit : UIViewResManagedBase
    {
        public GameObject DownLoadObj;
        public GameObject EdittingObj;
        public Texture DefaultCoverTexture;
        public RectTransform PannelRtf;
        public Button ReturnBtn;
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
        public Button OKBtn;
        public Button DeleteBtn;
        public Text OKBtnTxt;
        public Text UITitleTxt;

        public RawImage DownLoadCover;
        public Button DownloadEditBtn;
        public Text DownLoadTitle;
        public Text ProjectId;
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
    }
}