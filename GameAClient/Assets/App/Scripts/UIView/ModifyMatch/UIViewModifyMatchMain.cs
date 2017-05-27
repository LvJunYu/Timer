using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewModifyMatchMain : UIViewBase
    {
		public Button CloseBtn;

        // modify
		public Button ModifyBtn;
        //public Image ModifyCDImage;
        public Text ModifyCDText;
        public Text ModifyChanceReady;
        public GameObject [] ModifyLightSmall;
        public GameObject [] ModifyLightBig;

        // match
        public Button MatchBtn;
        //public Image MatchCDImage;
        public Text matchCDText;
        public Text MatchPoint;
        public GameObject [] MatchLightSmall;
        public GameObject [] MatchLightBig;

        // info
        public Text MakerLevel;
        public Text CanModifyNum;
        public Text CanDeleteNum;
        public Text CanAddNum;

        // published
        public RawImage PublishedProjectSnapShoot;
        public Text PassingRate;
        public Text ValidTime;
        public Text ChallengeUserCnt;
        public Image ChallengeUserCntBar;
        public GameObject ChallengeMark1;
        public GameObject ChallengeMark2;
        public GameObject ChallengeMark3;


        public Button ClaimBtn;

        public Texture DefaultProjectCoverTex;
    }
}
