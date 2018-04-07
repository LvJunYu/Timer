using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewOfficalWorkShop : UIViewResManagedBase
    {
        public Button ReturnBtn;
        public RectTransform TitleRtf;
        public RectTransform PannelRtf;
        public RectTransform BGRtf;
        public GameObject EmptyObj;
        public UITabGroup TabGroup;
        public GameObject[] Pannels;
        public GridDataScroller[] GridDataScrollers;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;

        public GameObject AddSelfRecommendProjectPanel;
        public GridDataScroller AddSelfRecommendProjectScroller;
        public Button AddConfirmBtn;
        public Button AddCancelBtn;


        //剧情
        public Button StoryCancelRemoveBtn;
        public USViewTimePicker[] StoryTimePick;
        public GameObject StoryReleaseTimeObj;
        public Button StoryPublishBtn;
        public Text StoryReleaseTimeText;
        public Button StoryUploadBtn;
        public Button StoryRemoveBtn;
        public Button StoryEditBtn;
        public Button StoryAddProjectBtn;


        //多人
        public Button MultiCancelRemoveBtn;
        public USViewTimePicker[] MultiTimePick;
        public GameObject MultiReleaseTimeObj;
        public Button MultiPublishBtn;
        public Text MultiReleaseTimeText;
        public Button MultiUploadBtn;
        public Button MultiRemoveBtn;
        public Button MultiEditBtn;
        public Button MultiAddProjectBtn;


        //冒险
        public Button AdventureCancelRemoveBtn;
        public USViewTimePicker[] AdventureTimePick;
        public GameObject AdventureReleaseTimeObj;
        public Button AdventurePublishBtn;
        public Text AdventureReleaseTimeText;
        public Button AdventureUploadBtn;
        public Button AdventureRemoveBtn;
        public Button AdventureEditBtn;
        public Button AddSectionBtn;
        public RectTransform BtnIndexGroupContent;
    }
}