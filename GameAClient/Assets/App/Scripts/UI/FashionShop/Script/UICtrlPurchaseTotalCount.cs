
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
	[UIAutoSetup(EUIAutoSetupType.Create)]
	public class UICtrlPurchaseTotalCount : UISocialCtrlBase<UIViewPurchaseTotalCount>
	{
		#region 常量与字段

		#endregion

		protected override void OnOpen (object parameter)
		{
			base.OnOpen (parameter);


		}

		protected override void OnClose()
		{
			base.OnClose();
		}
		protected override void InitEventListener()
		{
			base.InitEventListener ();
		}

		protected override void OnViewCreated()
		{
			base.OnViewCreated ();
			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtnClick);
//			_cachedView.GoShoppingViwe.onClick.AddListener (GoShoppingViwe);
		}
		private void OnCloseBtnClick () {
			SocialGUIManager.Instance.OpenPopupUI<UICtrlPurchaseTotalCount> ();//???/
		}

		/// <summary>
		/// 去往购买确认界面
		/// </summary>
		public void GoShoppingViwe()
		{
			
		}



	}
}
