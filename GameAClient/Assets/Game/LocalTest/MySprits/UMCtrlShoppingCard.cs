/********世豪*************/
using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;



namespace GameA
{
//	[UIAutoSetup(EUIAutoSetupType.Create)]
	public class UMCtrlShoppingCard :UMCtrlBase<UMViewShoppingCard>
	{
		private Project _content;
		/*应该每一个界面都会被他的父类管理，这个list 也应该在usbase中，这样才能实现购买界面被管理*/

		protected override void OnViewCreated()
		{
			
			base.OnViewCreated();
			_cachedView.BuyBtn.onClick.AddListener (BuyViwe);
			_cachedView.CDBtn.onClick.AddListener (CDEvent);
			_cachedView.USBtn.onClick.AddListener (USEvent);
			_cachedView.DressBtn.onClick.AddListener (Dress);

		}
		public void Set(){
			
		}
		/*这是我取到了数据以后我要读出这个数据*/
		public void Set(USCtrlShopping.Person data)
		{

//			DictionaryTools.SetStaticText (_cachedView.DjsText, "剩余时间");

		}

		private void OnCardClick()
		{
		}

		/// <summary>
		/// 买的按钮的事件，这里写点下按钮以后弹出的界面
		/// 
		/// 还需要将该界面的数据取到，赋值给显示界面
		/// </summary>
		private void BuyViwe()
		{
			SocialGUIManager.Instance.OpenPopupUI<UICtrlGoShopping> ();
		}
		/// <summary>
		/// 穿戴的事件，服务器发送数据在这个接收
		/// </summary>
		private void CDEvent()
		{
			
		}
		/// <summary>
		/// 使用按钮的点击事件
		/// </summary>
		private void USEvent()
		{
			
		}
		/// <summary>
		/// 试穿按钮的事件，
		/// </summary>
		private void Dress()
		{
			
		}


		public enum EType 
		{
			SY,
			KZ,
			XZ,
			ZS,
			MZ
		}







	}

}

