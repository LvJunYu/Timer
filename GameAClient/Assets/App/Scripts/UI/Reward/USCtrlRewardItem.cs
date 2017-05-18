﻿/********************************************************************
** Filename : UICtrlModifyMatchMain
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    public class USCtrlRewardItem : USCtrlBase<USViewRewardItem>
    {
        #region 常量与字段
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewRewardItem view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
//            _cachedView.SelectBtn.onClick.AddListener (OnSelectBtn);
        }

        public void SetItem (Sprite sprite, string name) {
            _cachedView.Icon.sprite = sprite;
            _cachedView.Name.text = name;
        }

        #endregion

    }
}