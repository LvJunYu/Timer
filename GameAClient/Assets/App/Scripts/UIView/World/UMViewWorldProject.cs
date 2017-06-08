  /********************************************************************
  ** Filename : UMViewWorldProject.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMViewWorldProject.cs
  ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewWorldProject : UMViewBase
    {
        public Button Button;
        public Texture DefaultCoverTexture;
        public RawImage Cover;
        public Text Title;
        public Text SubTitle;
        public Text PlayCount;
        public Text LikeCount;
        public Text CompleteRate;
        public Text CommentCount;
        public Text PublishTime;
        public Image SeletedMark;
    }
}
