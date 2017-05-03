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

        // published
        public RawImage CoverImg;
        public Text StateTxt;

        public Texture DefaultProjectCoverTex;
    }
}
