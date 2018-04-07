// 角色正在使用时装数据 | 角色正在使用时装数据
using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
	public partial class UsingAvatarPart{
		#region 字段
		/// <summary>
		/// 正在使用的头部
		/// </summary>
		private AvatarPartItem _head;
		/// <summary>
		/// 正在使用的上身
		/// </summary>
		private AvatarPartItem _upper;
		/// <summary>
		/// 正在使用的下身
		/// </summary>
		private AvatarPartItem _lower;
		/// <summary>
		/// 正在使用的附件
		/// </summary>
		private AvatarPartItem _appendage;
		#endregion

		#region 属性
		/// <summary>
		/// 正在使用的头部
		/// </summary>
		public AvatarPartItem Head {
			get {
				return this._head;
			}
		}
		/// <summary>
		/// 正在使用的上身
		/// </summary>
		public AvatarPartItem Upper {
			get {
				return this._upper;
			}
		}
		/// <summary>
		/// 正在使用的下身
		/// </summary>
		public AvatarPartItem Lower {
			get {
				return this._lower;
			}
		}
		/// <summary>
		/// 正在使用的附件
		/// </summary>
		public AvatarPartItem Appendage {
			get {
				return this._appendage;
			}
		}
		#endregion

		#region 方法
		protected override void OnSyncPartial ()
		{
			base.OnSyncPartial ();
			_head = null;
			_upper = null;
			_lower = null;
			_appendage = null;
			if (ItemDataList != null) {
				for (int i = 0; i < ItemDataList.Count; i++) {				
					if (ItemDataList [i].Type == (int)EAvatarPart.AP_Head) {
						if (_head == null) {
							_head = ItemDataList [i];
						} else {
							LogHelper.Error ("Using avatar part [head] dumplicate ({0} and {1})", _head.Id, ItemDataList [i].Id);
						}
					} else if (ItemDataList [i].Type == (int)EAvatarPart.AP_Upper) {
						if (_upper == null) {
							_upper = ItemDataList [i];
						} else {
							LogHelper.Error ("Using avatar part [upper] dumplicate ({0} and {1})", _upper.Id, ItemDataList [i].Id);
						}
					} else if (ItemDataList [i].Type == (int)EAvatarPart.AP_Lower) {
						if (_lower == null) {
							_lower = ItemDataList [i];
						} else {
							LogHelper.Error ("Using avatar part [lower] dumplicate ({0} and {1})", _lower.Id, ItemDataList [i].Id);
						}
					} else if (ItemDataList [i].Type == (int)EAvatarPart.AP_Appendage) {
						if (_appendage == null) {
							_appendage = ItemDataList [i];
						} else {
							LogHelper.Error ("Using avatar part [appendage] dumplicate ({0} and {1})", _appendage.Id, ItemDataList [i].Id);
						}
					}
				}
			}
		}
		#endregion
	}
}