/********世豪*************/
using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class USViewFashionShop : USViewBase
	{
		/// <summary>
		/// 第一页
		/// </summary>
		public Button Page1Btn;
		public GameObject Page1Obj;

        /// <summary>
        /// 第二页
        /// </summary>
        public Button Page2Btn;
		public GameObject Page2Obj;
  
        /// <summary>
        /// 第三页
        /// </summary>
        public Button Page3Btn;
        public GameObject Page3Obj;

        /// <summary>
        /// 第四页
        /// </summary>
        public Button Page4Btn;
        public GameObject Page4Obj;

        /// <summary>
        /// 第五页
        /// </summary>
        //public Button Page5Btn;
        //public GameObject Page5Obj;


        /// <summary>
        /// 购买按钮
        /// </summary>
        //		public Button BuyBtn;
        //		public GameObject BuyBtnVive;
        //		public Button BuyBtnViveBtn;
        //		public GameObject BuyBtnViveBtnVive;
        /// <summary>
        /// 使用按钮
        /// </summary>
        //		public Button USBtn;

        /// <summary>
        /// 穿戴按钮
        /// </summary>
        //		public Button CDBtn;
        //		public GameObject CDBtnVive;
        /// <summary>
        /// Texture点击按钮试穿
        /// </summary>
        //		public Button DressBtn;

        /*四个界面显示文字*/
        /// <summary>
        /// 右上角倒计时
        /// </summary>
        //		public Text DjsText;
        /// <summary>
        /// 时装简介名字
        /// </summary>
        //		public Text JiText;
        /// <summary>
        /// 价格
        /// </summary>
        //		public Text price;
        /// <summary>
        /// 限时打折券
        /// </summary>
        //		public Text Limit;


        public UITagGroup TagGroup;
        //		public UICommentInput CommentInput;
        public Transform Dock;


        /// <summary>
        /// 卡片的父物体的位置（1）
        /// 剩下的还有4个界面
        /// </summary>
        //public RectTransform SYDock;

    }
}