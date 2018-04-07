using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewInvitedByFriend : UIViewResManagedBase
    {
        public RectTransform PanelRtf;
        public RectTransform MaskRtf;
        public GameObject TeamPannel;
        public GameObject RoomPannel;
        public Texture DefaultProjectTexture;
        public RawImage Cover;
        public Text InviteProjcetTxt;
        public Button CloseBtn;
        public Button OKBtn;
        public Button CancelBtn;
        public Toggle RefuseTog;
        public Text AdvLvTxt;
        public Text CreateLvTxt;
        public Texture DefaultUserIconTexture;
        public RawImage UserIcon;
        public GameObject BlueVipDock;
        public Image BlueImg;
        public Image SuperBlueImg;
        public Image BlueYearVipImg;
        public Text UserName;
        public UITabGroup TabGroup;
        public GameObject TogDock;
        public GameObject TogSelectedDock;
    }
}