using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoyEngine
{

	public class USViewBase : UIBehaviour
	{
		#region 变量

		protected RectTransform _trans;

		#endregion

		#region 属性

		public RectTransform Trans
		{
			get
			{
				return _trans;
			}
		}

		#endregion

		#region 方法

		protected override void Awake()
		{
			_trans = GetComponent<RectTransform>();
		}

		#endregion
	}
}

