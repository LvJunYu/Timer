using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UIViewProjectDetailInfo : UIViewBase
    {
        public Button CloseBtn;
        public Button PlayBtn;

        public Text ProjectTitle;
        public Text AuthorName;
        public Text ProjectDesc;
        public RawImage AuthorImg;
        public RawImage Cover;

        public Text ServedPlayerCnt;
        public Text LikedPlayerCnt;
        public Text PassRate;

        public Texture DefaultHeadImg;
        public Texture DefaultCover;


        public Button AddTagBtn;
    }
}
