/********************************************************************
** Filename : UICtrlWorld.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UICtrlWorld.cs
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlWorld : UICtrlGenericBase<UIViewWorld>
    {
        #region 常量与字段

        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
        }

        #region private
        private void InitUI()
        {
            List<string> menuList = new List<string>();
            menuList.Add("最新关卡");
            menuList.Add("最近玩过的关卡");
            menuList.Add("收藏的关卡");
            _cachedView.MenuDropDown.AddOptions(menuList);
            _cachedView.MenuDropDown.onValueChanged.AddListener(OnMenuChanged);
        }

        private void OnMenuChanged(int selectInx)
        {
            
        }
        #endregion private

        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        #endregion 接口

        #endregion

        private enum EState
        {
            NewestProject,
            UserPlayHistory,
            UserFavorite,
        }
    }
}
