/********世豪*************/
using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;

namespace GameA
{
	public class UMViewShoppingCard : UMViewBase
	{
		/// <summary>
		/// 购买按钮
		/// </summary>
				public Button BuyBtn;
//				public GameObject BuyBtnVive;
//				public Button BuyBtnViveBtn;
//				public GameObject BuyBtnViveBtnVive;
		/// <summary>
		/// 使用按钮
		/// </summary>
				public Button USBtn;

		/// <summary>
		/// 穿戴按钮
		/// </summary>
				public Button CDBtn;
//				public GameObject CDBtnVive;
		/// <summary>
		/// Texture点击按钮试穿card
		/// </summary>
				public Button DressBtn;





		/*四个界面显示文字*/
		/// <summary>
		/// 右上角倒计时
		/// </summary>
				public Text DjsText;
		/// <summary>
		/// 时装简介名字
		/// </summary>
				public Text JiText;
		/// <summary>
		/// 价格
		/// </summary>
				public Text price;
		/// <summary>
		/// 限时打折券
		/// </summary>
				public Text Limit;


	}
}
