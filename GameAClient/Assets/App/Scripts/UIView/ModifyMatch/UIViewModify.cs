using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UIViewModify : UIViewBase
    {
		public Button CloseBtn;

        public Button PublishBtn;
        public Button EditBtn;
        public Button RepickBtn;

        public Button RandomPickBtn;

        // published
        //public RawImage CoverImg;
        //public Text StateTxt;
        //public Text ProjectLocTxt;
        public USViewModifyCard CurrentModifyCard;

        //public Texture DefaultProjectCoverTex;

        public Image InputBlock;
    }
}
