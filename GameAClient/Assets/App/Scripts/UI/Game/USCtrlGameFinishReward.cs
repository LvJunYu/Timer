/********************************************************************
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
    public class USCtrlGameFinishReward : USCtrlBase<USViewGameFinishReward>
    {
        #region 常量与字段
        #endregion

        #region 属性


        #endregion

        #region 方法
        public override void Init (USViewGameFinishReward view)
        {
            base.Init (view);
        }

        protected override void OnViewCreated ()
        {
            base.OnViewCreated ();
//            _cachedView.SelectBtn.onClick.AddListener (OnSelectBtn);
        }

        public void Set (Sprite sprite, string s) {
            _cachedView.Icon.sprite = sprite;
            _cachedView.Num.text = s;
            _cachedView.gameObject.SetActive (true);
        }

        public void Hide () {
            _cachedView.gameObject.SetActive (false);
        }
        #endregion

    }
}