using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorld : UIViewResManagedBase
    {
        public Button ReturnBtn;
        public RectTransform TitleRtf;
        public RectTransform PannelRtf;
        public RectTransform BGRtf;
        public GameObject TopDock;
        public GameObject EmptyObj;
        public UITabGroup TabGroup;
        public UITabGroup NewestTabGroup;
        public UITabGroup RankTimeTapGroup;
        public UITabGroup RankTypeTapGroup;
        public InputField SearchInputField;
        public Button SearchBtn;
        public InputField SearchRoomInputField;
        public Button SearchRoomBtn;
        public GameObject MultiDetailPannel;
        public RectTransform SearchPannelRtf;
        public Text LevelTex;
        public Text CountTex;
        public GameObject[] NewestPannels;
        public GridDataScroller[] NewestGridDataScrollers;
        public GameObject[] Pannels;
        public GridDataScroller[] GridDataScrollers;
        public ScrollRectEx1[] ScrollRectEx1s;
        public Button[] NewestButtonAry;
        public Button[] NewestSelectedButtonAry;
        public Button[] MenuButtonAry;
        public Button[] MenuSelectedButtonAry;
        public Button[] RankTimeBtnAry;
        public Button[] RankTimeBtnSelectAry;
        public Button[] RankTypeBtnAry;
        public Button[] RankTypeBtnSelectAry;
        public Text DescTxt;
        public Text PlayerCount;
        public Text LifeCount;
        public Text ReviveTime;
        public Text ReviveProtectTime;
        public Text TimeLimit;
        public Text TimeOverCondition;
        public Text WinScoreCondition;
        public Text ArriveScore;
        public Text CollectGemScore;
        public Text KillMonsterScore;
        public Text KillPlayerScore;
        public Button JoinRoomBtn;
        public Button QuickJoinBtn;
        public Toggle[] ProjectTypeTogs;

        //GM
        public Button EditBtn;
        public Button RemoveBtn;
        public Button CancelRemoveBtn;
        public USViewTimePicker[] TimePick;
        public GameObject ReleaseTimeObj;
        public Button ConfirmEffectBtn;
        public Button OpenCandiateBtn;
        public Text ReleaseTimeText;
        public Button ImmediatelyConfirmBtn;

        //推荐区
        public Button MoveOutCandidateBtn;
        public Button CancelBtn;
        public Button RecommendBtn;
        public GameObject CandidatePanel;
        public GridDataScroller CandidateGridData;
        public ScrollRectEx1 CandidateRect;
    }
}