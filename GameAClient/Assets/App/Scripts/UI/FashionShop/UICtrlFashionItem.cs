using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SoyEngine.Proto;
using GameA.Game;
/*商店的按钮，时装*/
namespace GameA
{
	public class UICtrlFashionItem : MonoBehaviour {

		#region 常量与字段
		public Button BuyBtn;
		public Button UseBtn;

//		public Image ItemIcon;
		/// <summary>
		/// 买的哪个东西图
		/// </summary>
		public RawImage ItemIcon;
		/// <summary>
		/// 试装
		/// </summary>
		public Button OpenDressUI;
		public Text ItemName;
		/// <summary>
		/// 价钱
		/// </summary>
		public Text Price;
		/// <summary>
		/// 右上角倒计时
		/// </summary>
		public Text DJS;
		/// <summary>
		/// 试用券
		/// </summary>
		public Text Trial;
		/// <summary>
		/// 穿戴按钮
		/// </summary>
		public Button DressBtn;

		private EAvatarPart _type;
		private int _id;
		#endregion

		#region 属性

		#endregion

		#region 方法
		//void Start () {
		//	BuyBtn.onClick.AddListener (OnBuyBtn);
		//	UseBtn.onClick.AddListener (OnUseBtn);
		//	DressBtn.onClick.AddListener (OnDrseeBtn);
		//}

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
        /// <summary>
        /// 买
        /// </summary>
		private void OnBuyBtn () {
		}
        /// <summary>
        /// 使用
        /// </summary>
		private void OnUseBtn () {
		}
		/// <summary>
		/// 穿
		/// </summary>
		private void OnDrseeBtn(){
			
		}
		#endregion
	}
}