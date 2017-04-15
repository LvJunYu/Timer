/********************************************************************
** Filename : UICtrlTaskbar
** Author : Dong
** Date : 2015/4/30 16:09:07
** Summary : UICtrlTaskbar
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameA
{
    public class ListMenuViewCreator
    {
        #region 常量与字段
        private List<IListMenuViewItem> _list = new List<IListMenuViewItem>();
        private RectTransform _root;
        private LayoutElement _layoutElement;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public ListMenuViewCreator(RectTransform parent, int padding = 0)
        {
            GameObject go = new GameObject("ListMenuView");
            go.transform.parent = parent;
            go.layer = parent.gameObject.layer;
            _root = go.AddComponent<RectTransform>();
            _root.localScale = Vector3.one;
            _root.localRotation = Quaternion.identity;
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.localPosition = Vector3.zero;
            _root.sizeDelta = Vector3.zero;
            _layoutElement = go.AddComponent<LayoutElement>();
            _layoutElement.minHeight = _layoutElement.preferredHeight = 0;
        }

        /// <summary>
        /// 通用添加条目方法
        /// </summary>
        /// <param name="item">Item.</param>
        public void AddMenuItem(IListMenuViewItem item)
        {
            _list.Add(item);
            item.Create(_root);
            RectTransform tran = item.GetTransform();
            tran.sizeDelta = new Vector2(0, tran.sizeDelta.y);
            tran.anchoredPosition = new Vector2(0, -_layoutElement.preferredHeight);
            _layoutElement.minHeight = _layoutElement.preferredHeight = _layoutElement.minHeight + tran.sizeDelta.y;
        }

        public void AddSpace(int height)
        {
            _layoutElement.minHeight = _layoutElement.preferredHeight = _layoutElement.minHeight + height;
        }

        /// <summary>
        /// 快速添加简单菜单项
        /// </summary>
        /// <param name="labelText">Label text.</param>
        /// <param name="callback">Callback.</param>
        public void AddSimpleItem(string labelText, UnityEngine.Events.UnityAction callback)
        {
            UMCtrlSimpleListMenuItem item = new UMCtrlSimpleListMenuItem();
            AddMenuItem(item);
            item.SetContent(labelText);
            item.SetClickCallback(callback);
        }

        public ListMenuView GetView()
        {
            ListMenuView lv = new ListMenuView(_list, _root);
            return lv;
        }
        #endregion
    }
}
