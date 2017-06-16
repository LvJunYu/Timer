using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewChallengeMatch : UIViewBase
    {
		public Button CloseBtn;

        public Text SelectTip;
        public Button RandomPickBtn;
        public Button ChallengeBtn;
        public Button AbandomBtn;

        public Text ChallengeEnergyCost;

        public USViewChallengeProjectCard ChallengeProjectEasy;
        public USViewChallengeProjectCard ChallengeProjectNormal;
        public USViewChallengeProjectCard ChallengeProjectHard;
        public USViewChallengeProjectCard ChallengeProjectUnkown;

        public Image InputBlock;
    }
}
