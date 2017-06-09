using System;
using UnityEngine;
using UnityEngine.UI;

namespace SoyEngine
{
    public class UPCtrlBase<C, V> where C:UICtrlBase where V:UIViewBase
	{
		protected V _cachedView;
        protected C _mainCtrl;
        protected bool _isOpen = false;

        public virtual void Init(C ctrl, V view)
		{
			_cachedView = view;
            _mainCtrl = ctrl;
			OnViewCreated ();
		}
		/// <summary>
		/// 创建方法
		/// </summary>
		protected virtual void OnViewCreated()
		{
		}

        public virtual void Open() { _isOpen = true; }
        public virtual void Close() { _isOpen = false; }
	}
}

