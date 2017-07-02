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
using NewResourceSolution;

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

        public void SetItem (RewardItem item) {
            _cachedView.Icon.sprite = item.GetSprite ();
            _cachedView.Name.text = string.Format ("{0} X {1}", item.GetName (), item.Count);
        }
        public void SetItem (string title, string icon) {
            _cachedView.Icon.sprite = ResourcesManager.Instance.GetSprite (icon);
            _cachedView.Name.text = title;
        }
        #endregion

    }
}