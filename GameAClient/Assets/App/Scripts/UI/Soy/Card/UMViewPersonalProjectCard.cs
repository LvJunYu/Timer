  /********************************************************************
  ** Filename : UMViewPersonalProjectCard.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMViewPersonalProjectCard.cs
  ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewPersonalProjectCard : UMViewCardBase
    {
        public Texture DefaultCoverTexture;
        public Text CreateTime;
        public Text ProjectCategoryText;
        public Image SelectableMask;
        public Image UnsetectMark;
        public Image SeletedMark;
        public GameObject InfoDock;
        public GameObject EmptyDock;
    }
}
