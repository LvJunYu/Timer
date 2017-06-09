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
        private static readonly Vector2 LeftHidePos = new Vector2(-1000, -66);
        private static readonly Vector2 LeftPos = new Vector2(-300, -66);
        private static readonly Vector2 RightPos = new Vector2(300, -66);
        private static readonly Vector2 RightHidePos = new Vector2(1000, -66);

        private EMenu _curMenu = EMenu.None;
        private UPCtrlWorldProjectInfo _projectInfoPanel;
        private UPCtrlBase<UICtrlWorld, UIViewWorld> _curMenuCtrl;
        private UPCtrlBase<UICtrlWorld, UIViewWorld>[] _menuCtrlArray;
        #endregion

        #region 属性

        #endregion

        #region 方法

        public void SetProject(Project project)
        {
            _projectInfoPanel.SetData(project);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            HideDetail();
            ChangeMenu(EMenu.NewestProject);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        #region private
        private void InitUI()
        {
            _cachedView.ShowDetailBtn.onClick.AddListener(ShowDetail);
            _cachedView.HideDetailBtn.onClick.AddListener(HideDetail);
            _cachedView.TopBtn.onClick.AddListener(ClickTop);
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);

            List<string> menuList = new List<string>();
            menuList.Add("最新关卡");
            menuList.Add("最近玩过的关卡");
            menuList.Add("收藏的关卡");
            _cachedView.MenuDropDown.ClearOptions();
            _cachedView.MenuDropDown.AddOptions(menuList);
            _cachedView.MenuDropDown.onValueChanged.AddListener(OnMenuChanged);

            _menuCtrlArray = new UPCtrlBase<UICtrlWorld, UIViewWorld>[]{
                new UPCtrlWorldNewestProject(),
                new UPCtrlWorldUserPlayHistory(),
                new UPCtrlWorldUserFavorite(),
            };
            Array.ForEach(_menuCtrlArray, c=>c.Init(this, _cachedView));
            _projectInfoPanel = new UPCtrlWorldProjectInfo();
            _projectInfoPanel.Init(this, _cachedView);
        }


        private void ChangeMenu(EMenu menu)
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _curMenu = menu;
            int inx = (int)_curMenu;
            if (inx < _menuCtrlArray.Length)
            {
                _curMenuCtrl = _menuCtrlArray[inx];
            }
            if(_curMenuCtrl != null)
            {
                _curMenuCtrl.Open();
            }
        }

        private void ShowDetail()
        {
            _cachedView.ListPanel.anchoredPosition = LeftHidePos;
            _cachedView.InfoPanel.anchoredPosition = LeftPos;
            _cachedView.DetailPanel.anchoredPosition = RightPos;
            _cachedView.ShowDetailBtn.gameObject.SetActive(false);
            _cachedView.HideDetailBtn.gameObject.SetActive(true);
        }

        private void HideDetail()
        {
            _cachedView.ListPanel.anchoredPosition = LeftPos;
            _cachedView.InfoPanel.anchoredPosition = RightPos;
            _cachedView.DetailPanel.anchoredPosition = RightHidePos;
            _cachedView.ShowDetailBtn.gameObject.SetActive(true);
            _cachedView.HideDetailBtn.gameObject.SetActive(false);
        }

        private void ClickTop()
        {
            _cachedView.GridScroller.ContentPosition = Vector2.zero;
        }
        #endregion private

        #region 接口

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitUI();
        }
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }


        private void OnMenuChanged(int selectInx)
        {
            ChangeMenu((EMenu)selectInx);
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorld>();
        }

        #endregion 接口

        #endregion

        private enum EMenu
        {
            NewestProject,
            UserPlayHistory,
            UserFavorite,
            None,
        }
    }
}
