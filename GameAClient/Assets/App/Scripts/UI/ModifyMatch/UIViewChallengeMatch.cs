using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewChallengeMatch : UIViewBase
    {
		public Button CloseBtn;

        public Button RandomPickBtn;
        public Button ChallengeBtn;
        public Button SelectBtn;


        public USViewChallengeProjectCard ChallengeProjectEasy;
        public USViewChallengeProjectCard ChallengeProjectNormal;
        public USViewChallengeProjectCard ChallengeProjectHard;
        public USViewChallengeProjectCard ChallengeProjectUnkown;
    }
}
