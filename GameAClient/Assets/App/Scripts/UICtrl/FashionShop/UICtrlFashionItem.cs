using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SoyEngine.Proto;
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
		private EAvatarPart _type;
		private int _id;
		#endregion

		#region 属性

		#endregion

		#region 方法
		void Start () {
			//BuyBtn.onClick.AddListener (OnBuyBtn);
			//UseBtn.onClick.AddListener (OnUseBtn);
		}

		private void RefreshInfo (Table_FashionShop tableShop) {

			_type = (EAvatarPart)tableShop.Type;
			_id = tableShop.ItemIdx;
			switch (_type) {
			case EAvatarPart.AP_Head:
				var head = TableManager.Instance.GetHeadParts (_id);
				ItemName.text = head.Name;
				Price.text = string.Format ("{0}G", head.PriceGoldWeek);
				break;
			case EAvatarPart.AP_Upper:
				break;
			case EAvatarPart.AP_Lower:
				break;
			case EAvatarPart.AP_Appendage:
				break;
			}


		}

		private void OnBuyBtn () {
		}

		private void OnUseBtn () {
		}
		#endregion
	}
}