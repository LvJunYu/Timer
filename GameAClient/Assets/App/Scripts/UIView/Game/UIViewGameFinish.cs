/********************************************************************
** Filename : UIViewGameFinish  
** Author : ake
** Date : 4/29/2016 8:06:30 PM
** Summary : UIViewGameFinish  
***********************************************************************/


using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewGameFinish:UIViewBase
	{
        //public UIRectTransformStatus Status;

        //      public Button PlayAgain;
        //      public Button PlayAgain2;
        //      public Button ExitGame;
        //      public Button ExitGame2;
        //      public Button Public;
        //      public Button ReturnToEditor;
        //      public Button ReturnToEditor2;
        //public Button NextLevel;

        //      public Text PlayAgainText;
        //public Text PlayAgain2Text;
        //      public Text ExitGameText;
        //public Text ExitGame2Text;
        //public Text PublicText;
        //      public Text ReturnToEditorText;
        //public Text ReturnToEditor2Text;
        //public Text NextLevelText;

        //public Button ButtonMark;
        //public Text ButtonMarkText;
        ////public UMViewGameRatingBarItem[] MarkStarArray;
        //public Text GameTimeText;
        //public Image NewRecordTip;
        //public Image RankTitle;
        //public Text RankText;
        //public Button SkipBtn;
        public Text Score;
        public Text ScoreOutLine;

        public GameObject Win;
        public GameObject Lose;
        public GameObject RewardObj;

        public Button ReturnBtn;
        public Button RetryBtn;
        public Button NextBtn;
        public Button ContinueEditBtn;

        public USViewGameFinishReward [] Rewards;
	}
}