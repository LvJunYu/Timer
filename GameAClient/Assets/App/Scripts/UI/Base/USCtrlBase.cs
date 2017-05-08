using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SoyEngine
{
	public class USCtrlBase<T> where T:USViewBase
	{
		protected T _cachedView;

		public virtual void Init(T view)
		{
			_cachedView = view;
			OnViewCreated ();
		}
		/// <summary>
		/// 创建方法
		/// </summary>
		protected virtual void OnViewCreated()
		{
			
		}
	}
}

