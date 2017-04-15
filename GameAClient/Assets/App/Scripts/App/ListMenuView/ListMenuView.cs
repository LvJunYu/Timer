  /********************************************************************
  ** Filename : ListMenuView.cs
  ** Author : quan
  ** Date : 
  ** Summary : ListMenuView.cs
  ***********************************************************************/



using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
	public class ListMenuView
	{
        #region 常量与字段
        private List<IListMenuViewItem> _list;
        private RectTransform _root;
        #endregion

        #region 属性
        public List<IListMenuViewItem> List
        {
            get
            {
                return this._list;
            }
        }

        public RectTransform Root
        {
            get
            {
                return this._root;
            }
        }
        #endregion

        #region 方法
        public ListMenuView(List<IListMenuViewItem> list, RectTransform root)
        {
            _list = list;
            _root = root;
        }
        #endregion
	}
}
