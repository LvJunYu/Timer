using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlWorkShop : UICtrlAnimationBase<UIViewWorkShop>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlWorkShopProjectBase _curMenuCtrl;
        private UPCtrlWorkShopProjectBase[] _menuCtrlArray;
        private bool _pushGoldEnergyStyle;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlWorkShopProjectBase[(int) EMenu.Max];

            var upCtrlWorkShopProjectEditing = new UPCtrlWorkShopProjectEditing();
            upCtrlWorkShopProjectEditing.Set(ResScenary);
            upCtrlWorkShopProjectEditing.SetMenu(EMenu.EditingProjects);
            upCtrlWorkShopProjectEditing.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.EditingProjects] = upCtrlWorkShopProjectEditing;

            var upCtrlWorkShopProjectPublished = new UPCtrlWorkShopProjectPublished();
            upCtrlWorkShopProjectPublished.Set(ResScenary);
            upCtrlWorkShopProjectPublished.SetMenu(EMenu.PublishedProjects);
            upCtrlWorkShopProjectPublished.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.PublishedProjects] = upCtrlWorkShopProjectPublished;

            var upCtrlWorkShopProjectDownload = new UPCtrlWorkShopProjectDownload();
            upCtrlWorkShopProjectDownload.Set(ResScenary);
            upCtrlWorkShopProjectDownload.SetMenu(EMenu.Downoad);
            upCtrlWorkShopProjectDownload.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Downoad] = upCtrlWorkShopProjectDownload;

            var upCtrlWorkShopSelfRecommen = new UPCtrlWorkShopSelfRecommen();
            upCtrlWorkShopSelfRecommen.Set(ResScenary);
            upCtrlWorkShopSelfRecommen.SetMenu(EMenu.SelfRecommen);
            upCtrlWorkShopSelfRecommen.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.SelfRecommen] = upCtrlWorkShopSelfRecommen;

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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
//            Clear();
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.EditingProjects, true);
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
            RegisterEvent(EMessengerType.OnWorkShopProjectListChanged, OnEditingProjectsChanged);
            RegisterEvent(EMessengerType.OnWorkShopDownloadListChanged, OnWorkShopDownloadListChanged);
            RegisterEvent(EMessengerType.OnUserPublishedProjectChanged, OnPublishedProjectsChanged);
            RegisterEvent<Project>(EMessengerType.OnWorkShopProjectDataChanged, OnEditingProjectDataChanged);
            RegisterEvent<long>(EMessengerType.OnWorkShopProjectPublished, OnWorkShopProjectPublished);
            RegisterEvent(EMessengerType.OnProjectNotValid, OnProjectNotValid);
        }

        private void OnProjectNotValid()
        {
            if (_isOpen && _curMenuCtrl is UPCtrlWorkShopProjectPublished)
            {
                ((UPCtrlWorkShopProjectPublished) _curMenuCtrl).RequestData();
            }
        }

        private void OnWorkShopProjectPublished(long projectId)
        {
            if (_isOpen && _curMenu == EMenu.EditingProjects && _curMenuCtrl is UPCtrlWorkShopProjectEditing)
            {
                ((UPCtrlWorkShopProjectEditing) _curMenuCtrl).OnWorkShopProjectPublished(projectId);
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

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.TitleRtf, EAnimationType.MoveFromUp, new Vector3(0, 100, 0), 0.17f);
            SetPart(_cachedView.TabGroup.transform, EAnimationType.MoveFromLeft, new Vector3(-200, 0, 0));
            SetPart(_cachedView.PannelRtf, EAnimationType.MoveFromRight);
            SetPart(_cachedView.BGRtf, EAnimationType.Fade);
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

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorkShop>();
        }

        private void OnEditingProjectsChanged()
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EMenu.EditingProjects && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopProjectEditing) _curMenuCtrl).RefreshView();
            }
        }

        private void OnWorkShopDownloadListChanged()
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EMenu.Downoad && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopProjectDownload) _curMenuCtrl).RefreshView();
            }
        }

        private void OnEditingProjectDataChanged(Project project)
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EMenu.EditingProjects && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopProjectEditing) _curMenuCtrl).OnChangeHandler(project.ProjectId);
            }
        }

        private void OnPublishedProjectsChanged()
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EMenu.PublishedProjects && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopProjectPublished) _curMenuCtrl).RefreshView();
            }
        }

        public enum EMenu
        {
            None = -1,
            EditingProjects,
            PublishedProjects,
            Downoad,
            SelfRecommen,
            Max
        }
    }
}