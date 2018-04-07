using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewPublishProject : UIViewResManagedBase
    {
        public GameObject StandaloneObj;
        public GameObject MultiObj;
        public RectTransform PanelRtf;
        public Text TitleTxt;
        public Text DescText;
        public Button OKBtn;
        public Button CancelBtn;
        public Button CloseBtn;
        public InputField TitleField;
        public InputField DescField;
        public Text PassCondition;
        public Text TimeLimit;
        public RawImage Cover;
        public Texture DefaultCover;
        public Text OKBtnTxt;
        /// 联机
        public Text NetBattleTimeLimit;
        public Text NetBattleMinPlayerCount;
        public Text TimeOverCondition;
        public Text WinScoreCondition;
        public Text ArriveScore;
        public Text CollectGemScore;
        public Text KillMonsterScore;
        public Text KillPlayerScore;
    }
}