  /********************************************************************
  ** Filename : UMViewWorldRecentRecord.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMViewWorldRecentRecord.cs
  ***********************************************************************/


using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewWorldRecentRecord : UMViewBase
    {
        public Texture DefaultUserIconTexture;
        public Button Button;
        public RawImage UserIcon;
        public Text UserName;
        public Text UserLevel;
        public Text CreateTime;
        public Text UsedTime;
        public Text Score;
        public Image SeletedMark;
    }
}
