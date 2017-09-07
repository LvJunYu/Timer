using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SoyEngine
{
	public class USCtrlBase<T> where T:USViewBase
	{
		protected T _cachedView;
		private bool _hasInited;

		public bool HasInited
		{
			get { return _hasInited; }
		}

		public virtual void Init(T view)
		{
			_cachedView = view;
			OnViewCreated ();
			_hasInited = true;
		}
		/// <summary>
		/// 创建方法
		/// </summary>
		protected virtual void OnViewCreated()
		{			
		}
	}
}

