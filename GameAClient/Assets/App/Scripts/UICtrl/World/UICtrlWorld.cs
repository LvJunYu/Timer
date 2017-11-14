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

            var upCtrlNewestProject = new UPCtrlWorldNewestProject();
            upCtrlNewestProject.Set(ResScenary);
            upCtrlNewestProject.SetMenu(EMenu.NewestProject);
            upCtrlNewestProject.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.NewestProject] = upCtrlNewestProject;

            var upCtrlWorldUserFavorite = new UPCtrlWorldUserFavorite();
            upCtrlWorldUserFavorite.Set(ResScenary);
            upCtrlWorldUserFavorite.SetMenu(EMenu.UserFavorite);
            upCtrlWorldUserFavorite.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.UserFavorite] = upCtrlWorldUserFavorite;

            var upCtrlWorldUserPlayHistory = new UPCtrlWorldUserPlayHistory();
            upCtrlWorldUserPlayHistory.Set(ResScenary);
            upCtrlWorldUserPlayHistory.SetMenu(EMenu.UserPlayHistory);
            upCtrlWorldUserPlayHistory.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.UserPlayHistory] = upCtrlWorldUserPlayHistory;

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
        }

        private void OnSearchBtn()
        {
            if (!(_curMenuCtrl is UPCtrlWorldProjectBase)) return;
            if (string.IsNullOrEmpty(_cachedView.SearchInputField.text))
            {
                ((UPCtrlWorldProjectBase) _curMenuCtrl).RequestData();
            }
            else
            {
                long projectId;
                if (long.TryParse(_cachedView.SearchInputField.text, out projectId))
                {
                    RemoteCommands.SearchWorldProject(projectId,
                        msg =>
                        {
                            if (msg.ResultCode == (int) ESearchWorldProjectCode.SWPC_Success)
                            {
                                ((UPCtrlWorldProjectBase) _curMenuCtrl).ShowSearchedProject(new Project(msg.Data));
                            }
                            else if (msg.ResultCode == (int) ESearchWorldProjectCode.SWPC_NotExsit)
                            {
                                SocialGUIManager.ShowPopupDialog(string.Format("关卡{0}不存在。", projectId));
                            }
                        }, code => SocialGUIManager.ShowPopupDialog("搜索关卡失败。"));
                }
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
            SocialGUIManager.Instance.CloseUI<UICtrlWorld>();
        }

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

        public enum EMenu
        {
            None = -1,
            Recommend,
            MaxScore,
            NewestProject,
            Follows,
            UserFavorite,
            UserPlayHistory,
            RankList,
            Max
        }
    }
}