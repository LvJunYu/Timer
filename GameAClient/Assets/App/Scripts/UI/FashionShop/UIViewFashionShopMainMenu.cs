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
		public USViewFashionShop USViewShop;
	    public USViewFashionShop FashionPage1;
        public USViewFashionShop FashionPage2;
		public USViewFashionShop FashionPage3;
		public USViewFashionShop FashionPage4;
        public USViewFashionShop FashionPage5;

	    public Text UsingHead;
	    public Text UsingUpper;
	    public Text UsingLower;
	    public Text UsingAppendage;



	    //public UICtrlFashionItem[] Items;
    }
}
