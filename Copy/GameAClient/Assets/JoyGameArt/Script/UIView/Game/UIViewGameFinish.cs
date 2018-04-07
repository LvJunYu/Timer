/********************************************************************
** Filename : UIViewGameFinish  
** Author : ake
** Date : 4/29/2016 8:06:30 PM
** Summary : UIViewGameFinish  
***********************************************************************/


using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewGameFinish : UIViewResManagedBase
    {
        public Text Score;
        public Text ScoreOutLine;
        public Text PlusExp;
        public Text level;
        public Image UpGrade;
        public Image ExpBar;
        public RawImage FriendHeadImg;
        public Texture DefaultHeadImg;
        public GameObject Win;
        public GameObject Lose;
        public GameObject RewardObj;
        public GameObject ExpBarObj;
        public GameObject PlayRecordObj;
        public GameObject ShadowBattleFailObj;
        public GameObject ShadowBattleWinObj;
        public Transform ShineRotateRoot;
        public Text FriendHelpTxt;
        public Button PlayRecordBtn;
        public Button FriendHelpBtn;
        public Button ReturnBtn;
        public Button RetryBtn;
        public Button NextBtn;
        public Button ContinueEditBtn;
        public Button GiveUpBtn;
        public Button RetryShadowBattleBtn;
        public Animation Animation;
        public USViewGameFinishReward[] Rewards;
        public GameObject MultiWinObj;
        public GameObject MultiLoseObj;
        public RectTransform MultiShineRotateRoot;
        public Button MultiConfirmBtn;
        public GameObject SiglePanel;
        public GameObject ScoreTextGroup;
        public Text LastTimeText;
        public Text KillMonterNumText;
        public Text GetTeethText;
        public GameObject WorkShopScoreTextGroup;
        public Text WorkShopLastTimeText;
        public Text WorkShopKillMonterNumText;
        public Text WorkShopGetTeethText;
        public RectTransform BtnsGroup;
        public GameObject[] WinImageGroup;
        public GameObject[] FailImageGroup;
        public GameObject WinHelmetImage;
        public GameObject FailHelmetImage;
    }
}