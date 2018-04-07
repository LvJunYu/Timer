using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlOfficalWorkShop : UICtrlAnimationBase<UIViewOfficalWorkShop>
    {
        private EOfficalMenu _curMenu = EOfficalMenu.None;
        public UPCtrlWorkShopOfficialProjectBase _curMenuCtrl;
        private UPCtrlWorkShopOfficialProjectBase[] _menuCtrlArray;
        private UPCtrlOfficalWorkShopAddProject _uPCtrlWorkShopAddProject;

        private UPCtrlWorkShopOfficalAdventure _upCtrlWorkShopOfficalAdventure;

        public readonly List<Msg_SortSelfRecommendProjectItem> SortItemList =
            new List<Msg_SortSelfRecommendProjectItem>();

        public bool SelfRecommendDirty = false;

        private bool _pushGoldEnergyStyle;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlWorkShopOfficialProjectBase[(int) EOfficalMenu.Max];

            var upCtrlWorkShopProjectEditing = new UPCtrlWorkShopOfficialProjectEditing();
            upCtrlWorkShopProjectEditing.Set(ResScenary);
            upCtrlWorkShopProjectEditing.SetMenu(EOfficalMenu.EditingProjects);
            upCtrlWorkShopProjectEditing.Init(this, _cachedView);
            _menuCtrlArray[(int) EOfficalMenu.EditingProjects] = upCtrlWorkShopProjectEditing;

            var upCtrlWorkShopProjectPublished = new UPCtrlWorkShopOfficialProjectPublished();
            upCtrlWorkShopProjectPublished.Set(ResScenary);
            upCtrlWorkShopProjectPublished.SetMenu(EOfficalMenu.OfficialPublishedProjects);
            upCtrlWorkShopProjectPublished.Init(this, _cachedView);
            _menuCtrlArray[(int) EOfficalMenu.OfficialPublishedProjects] = upCtrlWorkShopProjectPublished;

            var upCtrlOfficialWorkShopStory = new UPCtrlWorkShopOfficalStory();
            upCtrlOfficialWorkShopStory.Set(ResScenary);
            upCtrlOfficialWorkShopStory.SetMenu(EOfficalMenu.OfficialStory);
            upCtrlOfficialWorkShopStory.Init(this, _cachedView);
            _menuCtrlArray[(int) EOfficalMenu.OfficialStory] = upCtrlOfficialWorkShopStory;

            var upCtrlOfficialWorkShopMulti = new UPCtrlWorkShopOfficalMulti();
            upCtrlOfficialWorkShopMulti.Set(ResScenary);
            upCtrlOfficialWorkShopMulti.SetMenu(EOfficalMenu.OfficialMulti);
            upCtrlOfficialWorkShopMulti.Init(this, _cachedView);
            _menuCtrlArray[(int) EOfficalMenu.OfficialMulti] = upCtrlOfficialWorkShopMulti;

            _upCtrlWorkShopOfficalAdventure = new UPCtrlWorkShopOfficalAdventure();
            _upCtrlWorkShopOfficalAdventure.Set(ResScenary);
            _upCtrlWorkShopOfficalAdventure.SetMenu(EOfficalMenu.OfficialAdventure);
            _upCtrlWorkShopOfficalAdventure.Init(this, _cachedView);
            _menuCtrlArray[(int) EOfficalMenu.OfficialAdventure] = _upCtrlWorkShopOfficalAdventure;

            _uPCtrlWorkShopAddProject = new UPCtrlOfficalWorkShopAddProject();
            _uPCtrlWorkShopAddProject.Set(ResScenary);
            _uPCtrlWorkShopAddProject.Init(this, _cachedView);

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
            if (_curMenu == EOfficalMenu.None)
            {
                _curMenu = EOfficalMenu.EditingProjects;
            }

            _cachedView.TabGroup.SelectIndex((int) _curMenu, true);

            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }

            _uPCtrlWorkShopAddProject.Close();
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

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
        }

        private void OnPublishedProjectChanged(long projectId)
        {
            SelfRecommendDirty = true;
            if (_isOpen && _curMenu == EOfficalMenu.OfficialPublishedProjects)
            {
                _curMenuCtrl.RequestData();
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

        private void ChangeMenu(EOfficalMenu menu)
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
                ChangeMenu((EOfficalMenu) selectInx);
            }
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlOfficalWorkShop>();
        }

        private void OnEditingProjectsChanged()
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EOfficalMenu.EditingProjects && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopOfficialProjectEditing) _curMenuCtrl).RefreshView();
            }
        }

//        private void OnWorkShopDownloadListChanged()
//        {
//            if (!_isOpen)
//            {
//                return;
//            }
//
//            if (_curMenu == EOfficalMenu.OfficialAdventure && _curMenuCtrl != null)
//            {
//                ((UPCtrlWorkShopProjectDownload) _curMenuCtrl).RefreshView();
//            }
//        }

        private void OnEditingProjectDataChanged(Project project)
        {
            if (!_isOpen)
            {
                return;
            }

            if (_curMenu == EOfficalMenu.EditingProjects && _curMenuCtrl != null)
            {
                ((UPCtrlWorkShopOfficialProjectEditing) _curMenuCtrl).OnChangeHandler(project.ProjectId);
            }
        }

//        private void OnPublishedProjectsChanged(long userId)
//        {
//            if (!_isOpen || userId != LocalUser.Instance.UserGuid)
//            {
//                return;
//            }
//
//            if (_curMenu == EOfficalMenu.OfficialPublishedProjects && _curMenuCtrl != null)
//            {
//                ((UPCtrlWorkShopProjectPublished) _curMenuCtrl).RefreshView();
//            }
//        }

        public void OnSelectBtn(int index)
        {
            _curMenuCtrl.OnSelectBtn(index);
        }

        public void OnUnSelectBtn(int index)
        {
            _curMenuCtrl.OnUnSelecttBtn(index);
        }

        public void OnAddSelectBtn(Project project)
        {
            _curMenuCtrl.OnAddSelectBtn(project);
        }

        public void OnAddUnSelectBtn(Project project)
        {
            _curMenuCtrl.OnAddUnSelecttBtn(project);
        }

        public enum EOfficalMenu
        {
            None = -1,
            EditingProjects,
            OfficialPublishedProjects,
            OfficialAdventure,
            OfficialMulti,
            OfficialStory,
            Max
        }

        public void OpenAddSelfRecommendPanel(int type, HashSet<long> idset)
        {
            _uPCtrlWorkShopAddProject.OpenMenu(type, idset);
        }

        public void OpenMenu(EOfficalMenu menu)
        {
            if (_curMenu != menu)
            {
                _curMenu = menu;
            }

            _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
        }

        public void OnDragEnd(int benginIndex, int newindex)
        {
            if (_curMenu != EOfficalMenu.OfficialAdventure)
            {
                return;
            }

            UPCtrlWorkShopOfficalAdventure up = _curMenuCtrl as UPCtrlWorkShopOfficalAdventure;
            up.OnDrangEnd(benginIndex, newindex);
        }

        public void AddOfficialAdventure()
        {
            _upCtrlWorkShopOfficalAdventure.OnAddProjectBtn();
        }
    }
}