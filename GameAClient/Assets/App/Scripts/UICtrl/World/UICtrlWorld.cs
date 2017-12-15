using System;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlWorld : UICtrlAnimationBase<UIViewWorld>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlWorldPanelBase _curMenuCtrl;
        private UPCtrlWorldPanelBase[] _menuCtrlArray;
        private bool _pushGoldEnergyStyle;
        private UMCtrlProject _sarchUmCtrlProject;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReturnBtn.onClick.AddListener(OnReturnBtnClick);
            _cachedView.SearchBtn.onClick.AddListener(OnSearchBtn);
            _menuCtrlArray = new UPCtrlWorldPanelBase[(int) EMenu.Max];

            var upCtrlWorldRecommendProject = new UPCtrlWorldRecommendProject();
            upCtrlWorldRecommendProject.Set(ResScenary);
            upCtrlWorldRecommendProject.SetMenu(EMenu.Recommend);
            upCtrlWorldRecommendProject.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Recommend] = upCtrlWorldRecommendProject;

            var upCtrlWorldNewest = new UPCtrlWorldNewest();
            upCtrlWorldNewest.Set(ResScenary);
            upCtrlWorldNewest.SetMenu(EMenu.NewestProject);
            upCtrlWorldNewest.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.NewestProject] = upCtrlWorldNewest;

            var upCtrlWorldMulti = new UPCtrlWorldMulti();
            upCtrlWorldMulti.Set(ResScenary);
            upCtrlWorldMulti.SetMenu(EMenu.Multi);
            upCtrlWorldMulti.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Multi] = upCtrlWorldMulti;

            var upCtrlWorldUserFavorite = new UPCtrlWorldUserFavorite();
            upCtrlWorldUserFavorite.Set(ResScenary);
            upCtrlWorldUserFavorite.SetMenu(EMenu.UserFavorite);
            upCtrlWorldUserFavorite.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.UserFavorite] = upCtrlWorldUserFavorite;

            var uPCtrlWorldRanklistPanel = new UPCtrlWorldRanklistPanel();
            uPCtrlWorldRanklistPanel.Set(ResScenary);
            uPCtrlWorldRanklistPanel.SetMenu(EMenu.RankList);
            uPCtrlWorldRanklistPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.RankList] = uPCtrlWorldRanklistPanel;

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
            BadWordManger.Instance.InputFeidAddListen(_cachedView.SearchInputField);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.SearchRoomInputField);
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
            RegisterEvent<long>(EMessengerType.OnRoomProjectInfoFinish, OnRoomProjectInfoFinish);
            RegisterEvent(EMessengerType.OnRoomListChanged, OnRoomListChanged);
            RegisterEvent<Msg_MC_QueryRoom>(EMessengerType.OnQueryRoomRet, OnQueryRoomRet);
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Clear();
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

        private void Clear()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != null)
                {
                    _menuCtrlArray[i].Clear();
                }
            }
            _cachedView.SearchInputField.text = String.Empty;
        }

        private void OnChangeToAppMode()
        {
            if (_curMenu == EMenu.Multi && _curMenuCtrl != null)
            {
                _curMenuCtrl.RequestData();
            }
        }

        private void OnRoomListChanged()
        {
            if (_isOpen && _curMenu == EMenu.Multi)
            {
                ((UPCtrlWorldMulti)_curMenuCtrl).OnRoomListChanged();
            }
        }

        private void OnQueryRoomRet(Msg_MC_QueryRoom msg)
        {
            if (_isOpen && _curMenu == EMenu.Multi)
            {
                ((UPCtrlWorldMulti)_curMenuCtrl).OnQueryRoomRet(msg);
            }
        }

        private void OnSearchBtn()
        {
            if (!(_curMenuCtrl is UPCtrlWorldProjectBase)) return;
            if (string.IsNullOrEmpty(_cachedView.SearchInputField.text))
            {
                ChangeMenu(_curMenu);
            }
            else
            {
                long projectId;
                if (long.TryParse(_cachedView.SearchInputField.text, out projectId))
                {
                    if (projectId == 0)
                    {
                        SocialGUIManager.ShowPopupDialog("关卡0不存在。");
                        return;
                    }
                    RemoteCommands.SearchWorldProject(projectId,
                        msg =>
                        {
                            if (msg.ResultCode == (int) ESearchWorldProjectCode.SWPC_Success)
                            {
                                ShowSearchedProject(new Project(msg.Data));
                            }
                            else if (msg.ResultCode == (int) ESearchWorldProjectCode.SWPC_NotExsit)
                            {
                                SocialGUIManager.ShowPopupDialog(string.Format("关卡{0}不存在。", projectId));
                            }
                        }, code => SocialGUIManager.ShowPopupDialog("搜索关卡失败。"));
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("请输入正确的关卡号！");
                }
            }
        }

        private void ShowSearchedProject(Project project)
        {
            _cachedView.Pannels[(int) _curMenu].SetActive(false);
            _cachedView.SearchPannelRtf.SetActiveEx(true);
            if (_sarchUmCtrlProject == null)
            {
                _sarchUmCtrlProject = new UMCtrlProject();
                _sarchUmCtrlProject.SetCurUI(UMCtrlProject.ECurUI.Search);
                _sarchUmCtrlProject.Init(_cachedView.SearchPannelRtf, ResScenary);
            }
            CardDataRendererWrapper<Project> w =
                new CardDataRendererWrapper<Project>(project, OnItemClick);
            _sarchUmCtrlProject.Set(w);
        }

        private void OnItemClick(CardDataRendererWrapper<Project> item)
        {
            if (null != item && null != item.Content)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(item.Content);
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
            _cachedView.SearchPannelRtf.SetActiveEx(false);
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
            SocialGUIManager.Instance.CloseUI<UICtrlWorld>();
        }

        private void OnProjectDataChanged(long projectId)
        {
            if (!_isOpen)
            {
                return;
            }
            if (_curMenu != EMenu.Multi && _curMenuCtrl != null)
            {
                ((IOnChangeHandler<long>) _curMenuCtrl).OnChangeHandler(projectId);
            }
        }

        private void OnRoomProjectInfoFinish(long roomId)
        {
            if (!_isOpen)
            {
                return;
            }
            if (_curMenu == EMenu.Multi && _curMenuCtrl != null)
            {
                ((IOnChangeHandler<long>) _curMenuCtrl).OnChangeHandler(roomId);
            }
        }

        public enum EMenu
        {
            None = -1,
            Recommend,
            NewestProject,
            Multi,
            UserFavorite,
            RankList,
            Max
        }
    }
}