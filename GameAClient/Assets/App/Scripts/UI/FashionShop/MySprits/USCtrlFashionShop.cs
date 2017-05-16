/********世豪*************/
using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class USCtrlFashionShop : USCtrlBase <USViewFashionShop>
	{
        private UMCtrlFashionShopCard _curSelectedPrivateCard;
        private EShoppingPage _shoppingPage;
        public EShoppingPage ShoppingPage
        {
            get
            {
                return this._shoppingPage;
            }
            set
            {
                _shoppingPage = value;
            }
        }

        public void Open()
        {
            _cachedView.gameObject.SetActive(true);
        }

        public void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }

	    /// <summary>
        /// 枚举分页
        /// </summary>
        public enum EShoppingPage
        {
            FashionPage1,
            FashionPage2,
            FashionPage3,
            FashionPage4,
            FashionPage5,
        }
        /// <summary>
        /// 根据字符串返回枚举
        /// </summary>
        /// <param name="USUI"></param>
	    //public void USUIShopping(EShoppingPage USUI)
	    //{
	    //    switch (USUI)
	    //    {
	    //        case EShoppingPage.FashionPage1:
	    //            break;
	    //        case EShoppingPage.FashionPage2:
	    //            break;
	    //        case EShoppingPage.FashionPage3:
	    //            break;
	    //        case EShoppingPage.FashionPage4:
	    //            break;
     //           case EShoppingPage.FashionPage5:
     //               break;
     //               //case EShoppingPage.FashionPage5:
     //               // break;
     //       }
	    //}


        //		/*这是我的数据,读表读出来一部分，然后自己写一部分，形成一个聚合代码*/
        //		public class Person
        //		{
        //			public  int id;
        //			private  string Name;
        //			public  string  sex;


        //			/**perferbuff*/



        //			public string name
        //			{
        //				get { return Name;}
        //			}
        //		}

                 private List<UMCtrlFashionShopCard> _cardList = new List<UMCtrlFashionShopCard>();
        //		private Project _content;

        //		/// <summary>
        //		/// 掉这个方法的时候就是添加到list中，然后创建新的shopping
        //		/// </summary>
        //		/// <param name="item">Item.</param>
        //		//public void AddShopping(UMCtrlShoppingCard item)
        //		//{
        //		//	_cardList.Add (item);

        //		//	Console.WriteLine (_cardList.Count);

        //		//}
        //		protected override void OnViewCreated()
        //		{

        //			base.OnViewCreated ();

        ////			_cachedView.
        ////			_cachedView.TagGroup.AddButton(_cachedView.SYbutton, OnSYbuttonClick);
        ////			_cachedView.TagGroup.AddButton(_cachedView.KZbutton, OnKZbuttonClick);
        ////			_cachedView.TagGroup.AddButton(_cachedView.XZbutton, OnXZbuttonClick);
        ////			_cachedView.TagGroup.AddButton(_cachedView.ZSbutton, OnZSbuttonClick);
        ////			_cachedView.TagGroup.AddButton(_cachedView.BuyBtn, OnBuybuttonClick);

        //		}
        /*这我读去到了数据*/
        /*销毁掉所有的UI然后再创建出来*/
        public void Set(List<ShopItem> pageList)
        {
            //for (int i = pageList.Count; i >= 0; i--)
            //{
            //    _cardList[i].Destroy();
            //}
            _cardList.Clear();
            for (int i = 0; i < pageList.Count; i++)
            {
                var UM = new UMCtrlFashionShopCard();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(pageList[i]);
                
                _cardList.Add(UM);
            }


        }

    }
}
