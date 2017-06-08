  /********************************************************************
  ** Filename : UMViewWorldProjectComment.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMViewWorldProjectComment.cs
  ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewWorldProjectComment : UMViewBase
    {
        public Texture DefaultCoverTexture;
        public RawImage Cover;
        public Text Title;
        public Text CreateTime;
        public Text ProjectCategoryText;
        public Image SelectableMask;
        public Image UnsetectMark;
        public Image SeletedMark;
        public GameObject InfoDock;
        public GameObject EmptyDock;
        public Button CardBtn;
    }
}
