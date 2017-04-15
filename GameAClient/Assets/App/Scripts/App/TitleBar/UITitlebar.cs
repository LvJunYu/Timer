  /********************************************************************
  ** Filename : UITitlebar.cs
  ** Author : quan
  ** Date : 2016/6/21 20:11
  ** Summary : UITitlebar.cs
  ***********************************************************************/
using System;
using UnityEngine;
using UnityEngine.UI;
using SoyEngine;

namespace GameA
{
    public class UITitlebar: MonoBehaviour
    {
        public CanvasGroup CanvasGroup;

        public RectTransform MainTitleDock;
        /// <summary>
        /// 标题
        /// </summary>
        public Text MainTitleText;
        /// <summary>
        /// 图片标题
        /// </summary>
        public Image MainTitleImage;
        /// <summary>
        /// 标签标题
        /// </summary>
        public UITagGroup MainTagGroup;
        /// <summary>
        /// 标签按钮数组
        /// </summary>
        public Button[] TagArray;
        /// <summary>
        /// 默认返回按钮
        /// </summary>
        public Button DefaultReturnButton;

        public Text DefaultReturnButtonText;

        public Button DefaultCloseButton;

        public Text DefaultCloseButtonText;

        public Button LeftCustomButton;

        public Text LeftCustomButtonText;

        public Image LeftCustomButtonImage;

        public Button RightCustomButton;

        public Text RightCustomButtonText;

        public Image RightCustomButtonImage;
    }
}

