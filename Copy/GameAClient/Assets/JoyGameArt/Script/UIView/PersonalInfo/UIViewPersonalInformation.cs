using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPersonalInformation : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public GameObject BtnsObj;
        public GameObject EmptyObj;
        public Button CloseBtn;
        public Button FollowBtn;
        public Text FollowBtnTxt;
        public Button ChatBtn;
        public Button BlockBtn;
        public Text BlockBtnTxt;
        public Button EditBtn;
        public Button SaveEditBtn;
        public InputField NameInputField;
        public InputField DescInputField;
        public Toggle MaleToggle;
        public Toggle FamaleToggle;
        public GameObject NormalObj;
        public GameObject EditObj;
        public GameObject BlueVipDock;
        public Image BlueImg;
        public Image SuperBlueImg;
        public Image BlueYearVipImg;
        public RawImage AvatarRawImage;
        public RawImage HeadImg;
        public Texture HeadDefaltTexture;
        public Button HeadBtn;
        public GameObject MaleObj;
        public GameObject FamaleObj;
        public Text IdTxt;
        public Text Name;
        public Text Desc;
        public Text FollowNum;
        public Text FansNum;
        public Text AdvLv;
        public Text AdvExp;
        public Text CreateLv;
        public Text CreateExp;
        public Image AdvExpBar;
        public Image CreateExpBar;
        public Text TotalPlayCount;
        public Text TotalSuccessCount;
        public Text AdvPraisedCount;
        public Text TotalScoreCount;
        public Text TotalPublishCount;
        public Text TotalPlayedCount;
        public Text CreatePraisedCount;
        public Text TotalCommentCount;
        public Text MessageNum;
        public Text MessageSelectedNum;
        public InputField InputField;
        public Button SendBtn;
        public UITabGroup TabGroup;
        public TableDataScroller MessageTableDataScroller;
        public GridDataScroller[] GridDataScrollers;
        public GameObject[] Pannels;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
    }
}