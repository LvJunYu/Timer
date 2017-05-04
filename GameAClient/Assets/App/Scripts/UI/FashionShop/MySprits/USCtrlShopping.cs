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
	public class USCtrlShopping : USCtrlBase <USViewShopping>
	{
		private List<UMCtrlShoppingCard> _cardList = new List<UMCtrlShoppingCard>();
		private Project _content;

		/// <summary>
		/// 掉这个方法的时候就是添加到list中，然后创建新的shopping
		/// </summary>
		/// <param name="item">Item.</param>
		public void AddShopping(UMCtrlShoppingCard item)
		{
			_cardList.Add (item);

			Console.WriteLine (_cardList.Count);

		}

		private EShoppingPage _shoppingPage;
		public EShoppingPage ShoppingPage {
			get {
				return this._shoppingPage;
			}
			set {
				_shoppingPage = value;
			}
		}
		protected override void OnViewCreated()
		{
			
			base.OnViewCreated ();

//			_cachedView.
//			_cachedView.TagGroup.AddButton(_cachedView.SYbutton, OnSYbuttonClick);
//			_cachedView.TagGroup.AddButton(_cachedView.KZbutton, OnKZbuttonClick);
//			_cachedView.TagGroup.AddButton(_cachedView.XZbutton, OnXZbuttonClick);
//			_cachedView.TagGroup.AddButton(_cachedView.ZSbutton, OnZSbuttonClick);
//			_cachedView.TagGroup.AddButton(_cachedView.BuyBtn, OnBuybuttonClick);

		}
		/*这我读去到了数据*/
		/*销毁掉所有的UI然后再创建出来*/
		public void Set(List <Person> dataList)
		{
			for (int i= _cardList.Count; i>=0;i-- )
			{
				_cardList [i].Destroy ();
			}
			_cardList.Clear ();
			for (int i = 0; i < dataList.Count; i++) {
				_cardList [i].Destroy ();
				var UM = new UMCtrlShoppingCard ();

				UM.Init (_cachedView.SYDock);
				UM.Set (dataList [i]);
				_cardList.Add (UM);
			}


		}
				 


		public void Open()
		{
			_cachedView.gameObject.SetActive (true);
		}

		public void Close()
		{
			_cachedView.gameObject.SetActive (false);
		}



		public enum EShoppingPage
		{
			SYUIViewShopping,
			KZUIViewShopping,
			ZSUIViewShopping,
			XZUIViewShopping,
			MZUIViewShopping,

		}
		public void USUIShopping(EShoppingPage USUI){
			switch (USUI)
			{
			case  EShoppingPage.SYUIViewShopping:
				break;
			case EShoppingPage.KZUIViewShopping:
				break;
			case EShoppingPage.MZUIViewShopping:
				break;
			case EShoppingPage.ZSUIViewShopping:
				break;
			case EShoppingPage.XZUIViewShopping:
				break;

			}


		}
		/*这是我的数据,读表读出来一部分，然后自己写一部分，形成一个聚合代码*/
		public class Person
		{
			public  int id;
			private  string Name;
			public  string  sex;


			/**perferbuff*/



			public string name
			{
				get { return Name;}
			}
		}
		 

	}
}
