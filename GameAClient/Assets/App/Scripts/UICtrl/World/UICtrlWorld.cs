/********************************************************************
** Filename : UICtrlWorld.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UICtrlWorld.cs
***********************************************************************/

using System;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlWorld : UICtrlGenericBase<UIViewWorld>
    {
        #region 常量与字段
        private EMenu _curMenu = EMenu.None;
        private UPCtrlBase<UICtrlWorld, UIViewWorld> _curMenuCtrl;
        private UPCtrlBase<UICtrlWorld, UIViewWorld>[] _menuCtrlArray;
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region private
        private void InitUI()
        {
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);

            _menuCtrlArray = new UPCtrlBase<UICtrlWorld, UIViewWorld>[(int) EMenu.Max];
            _menuCtrlArray[(int) EMenu.Recommend] = new UPCtrlWorldRecommendProject();
            _menuCtrlArray[(int) EMenu.UserPlayHistory] = new UPCtrlWorldUserPlayHistory();
            _menuCtrlArray[(int) EMenu.UserFavorite] = new UPCtrlWorldUserFavorite();
            Array.ForEach(_menuCtrlArray, c=>
            {
                if (null == c)
                {
                    return;
                }
                c.Init(this, _cachedView);
            });
            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i],
                    b => ClickMenu(inx, b));
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }
        }

        private void ChangeMenu(EMenu menu)
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _curMenu = menu;
            var inx = (int) _curMenu;
            if (inx < _menuCtrlArray.Length)
            {
                _curMenuCtrl = _menuCtrlArray[inx];
            }
            if(_curMenuCtrl != null)
            {
                _curMenuCtrl.Open();
            }
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

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnProjectDataChanged, OnProjectDataChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int)EMenu.Recommend, true);
            }
        }

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu)selectInx);
            }
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorld>();
        }

        #endregion 接口
        private void OnProjectDataChanged(long projectId)
        {
            if (!_isOpen)
            {
                return;
            }
            if (_curMenuCtrl != null)
            {
                ((IOnChangeHandler<long>)_curMenuCtrl).OnChangeHandler(projectId);
            }
        }
        #endregion

        private enum EMenu
        {
            None = -1,
            Recommend,
            UserPlayHistory,
            UserFavorite,
            RankList,
            Max
        }
    }
}
