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
		protected bool _isOpen = false;

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

		public virtual void OnDestroy()
		{
			
		}
		
		public virtual void Open() { _isOpen = true; }
		public virtual void Close() { _isOpen = false; }
	}
}

