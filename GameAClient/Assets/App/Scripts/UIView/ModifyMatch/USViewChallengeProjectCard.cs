using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USViewChallengeProjectCard : USViewBase
    {
        public Button SelectBtn;
        public Text Title;
        public Text AddNum;
        public Text ModifyNum;
        public Text DelNum;
        public Text PassingRate;
        public RawImage Cover;

        public Texture DefaultProjectCoverTex;

        public GameObject Empty;
        public GameObject Normal;
        public GameObject Root;
    }
}
