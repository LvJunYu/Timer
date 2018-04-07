using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMViewPersonalInfoMessage : UMViewResManagedBase
    {
        public Texture DefaultIconTexture;
        public Button HeadBtn;
        public RawImage UserIcon;
        public Text CreateTime;
        public Text Content;
        public Text PraiseCountTxt;
        public Button PraiseBtn;
        public Text ReplayCountTxt;
        public Button ReplayBtn;
        public GameObject ReplayDock;
        public RectTransform ReplayRtf;
        public Button SendBtn;
        public InputField InputField;
        public GameObject PublishDock;
        public RectTransform FirstReplyRtf;
        public Button MoreBtn;
        public Text MoreTxt;
        public Button FoldBtn;
        public Button DeleteBtn;
        public GameObject VersionLineDock;
        public GameObject Line;
    }
}
