using UnityEngine;
using System.Collections;
using SoyEngine;
using UnityEngine.UI;
/*主UI*/
namespace GameA
{
	public class UIViewFashionShopMainMenu : UIViewBase
    {
		/// <summary>
		/// 时装关闭按钮
		/// </summary>
		public Button CloseBtn;
		/// <summary>
		/// 菜单按钮
		/// </summary>
		public UITagGroup TagGroup;
		/// <summary>
		/// 独立的US界面
		/// </summary>
		public USViewShopping USViewShopping;
	    public USViewShopping FashionPage1;
        public USViewShopping XZViewShopping;
		public USViewShopping ZSViewShopping;
		public USViewShopping MZViewShopping;
		public USViewShopping KZViewShopping;




		//public UICtrlFashionItem[] Items;
    }
}
