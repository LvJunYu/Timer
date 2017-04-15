  /********************************************************************
  ** Filename : UMCtrlSimpleListMenuItem.cs
  ** Author : quan
  ** Date : 2015/6/2 0:50
  ** Summary : UMCtrlSimpleListMenuItem.cs
  ***********************************************************************/



using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public class UMCtrlSimpleListMenuItem: UMCtrlBase<UMViewSimpleListMenuItem>, IListMenuViewItem
	{
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void SetClickCallback(UnityEngine.Events.UnityAction callback)
        {
            _cachedView.Btn.onClick.AddListener(callback);
        }

        public void SetContent(string content)
        {
            if (_isViewCreated)
            {
                DictionaryTools.SetContentText(_cachedView.Content, content);
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.Btn.onClick = null;
        }

        public void Create(RectTransform parent)
        {
            base.Init(parent,Vector3.zero);
        }

        public RectTransform GetTransform()
        {
            return _view.Trans;            
        }
        #endregion
	}
}
