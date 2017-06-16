using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USViewModifyCard : USViewBase
    {
        public Text Title;

        public RawImage Cover;

        public Texture DefaultProjectCoverTex;

        public GameObject Empty;
        public GameObject Normal;
        public GameObject CanPublish;
        public GameObject NotPass;
        public GameObject NotModify;
    }
}
