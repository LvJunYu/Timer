using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewWorkShop : UIViewBase
    {
        public Texture DefaultCoverTexture;
        
        public Button ReturnBtn;
        public Button ChangeModeBtn;

        public Button NewProjectBtn;
        public Button PublishBtn;
        public Button DeleteBtn;

        public GridDataScroller GridScroller;

        // personal project info detail
        public RawImage Cover;
        public Button EditBtn;
        public Text Title;
        public Text Desc;
    }
}
