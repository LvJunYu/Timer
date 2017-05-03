  /********************************************************************
  ** Filename : UMViewProjectCardDetail.cs
  ** Author : quan
  ** Date : 2016/7/28 14:44
  ** Summary : UMViewProjectCardDetail.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
using UnityEngine;

namespace GameA
{
    public class UMViewProjectCardDetail : UMViewCardBase
    {
        public Texture DefaultTexture;
        public Text CreateTime;
        public Text AuthorName;
        public RawImage UserIcon;
        public UIRateStarArray RateStar;
        public Text RateCount;
        public Text Summary;
        public UIProjectCompleteRate ProjectCompleteRate;



        public RectTransform RecentCompleteUserDock;
        public Text RecentCompleteUserTip;

        public GameObject CommentTip;
        public VerticalLayoutGroup CommentDock;
		/// <summary>
		/// ?？??
		/// </summary>
        public Button CommentBtn;
        public Text CommentCount;
    }
}
