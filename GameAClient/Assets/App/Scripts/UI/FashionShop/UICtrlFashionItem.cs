using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameA.Game;

namespace GameA
{
	public class UICtrlFashionItem : MonoBehaviour {

		#region 常量与字段
		public Button BuyBtn;
		public Button UseBtn;

		public Image ItemIcon;
		public Text ItemName;
		public Text Price;

		#endregion

		#region 属性

		#endregion

		#region 方法
		void Start () {
			BuyBtn.onClick.AddListener (OnBuyBtn);
			UseBtn.onClick.AddListener (OnUseBtn);
		}

//		private void RefreshInfo (Table_FashionShop) {
//			
//		}

		private void OnBuyBtn () {
		}

		private void OnUseBtn () {
		}
		#endregion
	}
}