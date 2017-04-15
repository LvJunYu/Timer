 /********************************************************************
 ** Filename : IListViewItem.cs
 ** Author : quansiwei
 ** Date : 2015/5/12 11:58
 ** Summary : ListView的接口
 ***********************************************************************/



using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    public interface IListMenuViewItem
    {
        #region 常量与字段
        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 创建条目
        /// </summary>
        /// <param name="parent">Parent.</param>
        void Create(RectTransform parent);

        RectTransform GetTransform();
        #endregion
    }
}
