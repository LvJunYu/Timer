using SoyEngine;
using UnityEngine;
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
        public Button RestoreFashionBtn;
        public Button PurchaseAllFittingFashionBtn;
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
	    public RawImage Avatar;
        public Image SeletctedPage1Image;
        public Image SeletctedPage2Image;
        public Image SeletctedPage3Image;
        public Image SeletctedPage4Image;

        public RectTransform Head;
        public RectTransform UpBody;
        public RectTransform Lowerpart;
    }
}
