/********************************************************************
** Filename : UICtrlWorld.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UICtrlWorld.cs
***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlWorld : UICtrlAnimationBase<UIViewWorld>
    {
        #region 常量与字段

        private EMenu _curMenu = EMenu.None;
        private UPCtrlBase<UICtrlWorld, UIViewWorld> _curMenuCtrl;
        private UPCtrlBase<UICtrlWorld, UIViewWorld>[] _menuCtrlArray;
        private bool _pushGoldEnergyStyle;
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region private

        private void InitUI()
        {
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);

            _menuCtrlArray = new UPCtrlBase<UICtrlWorld, UIViewWorld>[(int) EMenu.Max];
            var upCtrlWorldRecommendProject = new UPCtrlWorldRecommendProject();
            upCtrlWorldRecommendProject.Set(ResScenary);
            upCtrlWorldRecommendProject.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Recommend] = upCtrlWorldRecommendProject;
            var upCtrlWorldUserPlayHistory = new UPCtrlWorldUserPlayHistory();
            upCtrlWorldUserPlayHistory.Set(ResScenary);
            upCtrlWorldUserPlayHistory.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.UserPlayHistory] = upCtrlWorldUserPlayHistory;
            var upCtrlWorldUserFavorite = new UPCtrlWorldUserFavorite();
            upCtrlWorldUserFavorite.Set(ResScenary);
            upCtrlWorldUserFavorite.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.UserFavorite] = upCtrlWorldUserFavorite;
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
            if (_curMenuCtrl != null)
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

        protected override void OnDestroy()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != null)
                {
                    _menuCtrlArray[i].OnDestroy();
                }
            }
            _curMenuCtrl = null;
            base.OnDestroy();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
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
                _cachedView.TabGroup.SelectIndex((int) EMenu.Recommend, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            base.OnClose();
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _firstDelayFrames = 7;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp,new Vector3(0,100,0),0.1f);
            SetPart(_cachedView.TabGroup.transform, EAnimationType.MoveFromLeft,new Vector3(-200,0,0));
            SetPart(_cachedView.ContentPanelDockRtf, EAnimationType.MoveFromRight);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
        }

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
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
                ((IOnChangeHandler<long>) _curMenuCtrl).OnChangeHandler(projectId);
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