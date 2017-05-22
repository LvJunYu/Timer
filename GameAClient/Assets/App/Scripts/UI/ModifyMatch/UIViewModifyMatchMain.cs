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

        // match
        public Button MatchBtn;
        //public Image MatchCDImage;
        public Text MatchPoint;

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

        public Button ClaimBtn;

        public Texture DefaultProjectCoverTex;
    }
}
