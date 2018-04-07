using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewMultiRoom : UIViewResManagedBase
    {
        public RectTransform OpenPannel;
        public RectTransform ClosePannel;
        public Image MaskImage;
        public Button OpenBtn;
        public Button CloseButton;
        
        public Text TitleTxt;
        public Text RoomIdTxt;
        public Text TimeLimit;
        public Text TimeOverCondition;
        public Text WinScoreCondition;
        public Text ArriveScore;
        public Text CollectGemScore;
        public Text KillMonsterScore;
        public Text KillPlayerScore;
        public Button WorldRecruitBtn;
        public Button InviteFriendBtn;
        public Button PrepareBtn;
        public Text PrepareBtnTxt;
        public Button StartBtn;
        public Button RawStartBtn;
        public Button RawPrepareBtn;
        public Text RawPrepareBtnTxt;
        public Text StartBtnTxt;
        public Text RawStartBtnTxt;

        public USViewChat RoomChat;
    }
}