using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlProjectDetail : UICtrlAnimationBase<UIViewProjectDetail>
    {
        public static string EmptyStr = "-";
        public Project Project;
        private EMenu _curMenu = EMenu.None;
        private UPCtrlProjectDetailBase _curMenuCtrl;
        private UPCtrlProjectDetailBase[] _menuCtrlArray;
        private bool _isRequestDownload;
        private bool _isRequestFavorite;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.BgBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.FavoriteTog.onValueChanged.AddListener(OnFavoriteTogValueChanged);
            _cachedView.DownloadBtn.onClick.AddListener(OnDownloadBtn);
            _cachedView.ShareBtn.onClick.AddListener(OnShareBtn);
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtnClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.GoodTog.onValueChanged.AddListener(OnGoodTogValueChanged);
            _cachedView.BadTog.onValueChanged.AddListener(OnBadTogValueChanged);

            _menuCtrlArray = new UPCtrlProjectDetailBase[(int) EMenu.Max];
            var upCtrlProjectInfo = new UPCtrlProjectRecentRecord();
            upCtrlProjectInfo.Set(ResScenary);
            upCtrlProjectInfo.SetMenu(EMenu.Recent);
            upCtrlProjectInfo.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Recent] = upCtrlProjectInfo;

            var upCtrlProjectRecordRank = new UPCtrlProjectRecordRank();
            upCtrlProjectRecordRank.Set(ResScenary);
            upCtrlProjectRecordRank.SetMenu(EMenu.Rank);
            upCtrlProjectRecordRank.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Rank] = upCtrlProjectRecordRank;

            var upCtrlProjectComment = new UPCtrlProjectComment();
            upCtrlProjectComment.Set(ResScenary);
            upCtrlProjectComment.SetMenu(EMenu.Comment);
            upCtrlProjectComment.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Comment] = upCtrlProjectComment;

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

        private void OnBadTogValueChanged(bool arg0)
        {
        }

        private void OnGoodTogValueChanged(bool arg0)
        {
        }

        private void OnHeadBtn()
        {
            if (Project != null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(Project.UserInfoDetail);
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Clear();
            Project = parameter as Project;
            if (Project != null)
            {
                Project.Request(Project.ProjectId, null, null);
            }
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.Recent, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
            RefreshView();
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }

            base.OnClose();
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

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI2;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnProjectDataChanged, OnProjectDataChanged);
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToApp);
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
        }

        private void SetNull()
        {
            DictionaryTools.SetContentText(_cachedView.TitleText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.AdvLevelText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.CreateLevelText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.PlayCountText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.CompleteRateText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.FavoriteCount, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.DownloadCount, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.ShareCount, EmptyStr);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }

        private void RefreshView()
        {
            if (Project == null)
            {
                SetNull();
                return;
            }
            DictionaryTools.SetContentText(_cachedView.TitleText, Project.Name);
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, Project.UserInfo.NickName);
            DictionaryTools.SetContentText(_cachedView.AdvLevelText,
                GameATools.GetLevelString(Project.UserInfo.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.CreateLevelText,
                GameATools.GetLevelString(Project.UserInfo.LevelData.CreatorLevel));
            DictionaryTools.SetContentText(_cachedView.PlayCountText,
                Project.ExtendReady ? Project.PlayCount.ToString() : EmptyStr);
            DictionaryTools.SetContentText(_cachedView.CompleteRateText,
                Project.ExtendReady ? GameATools.GetCompleteRateString(Project.CompleteRate) : EmptyStr);
            RefreshBtns();
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, Project.UserInfo.HeadImgUrl,
                _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, Project.IconPath,
                _cachedView.DefaultCoverTexture);
        }

        private void RefreshBtns()
        {
            bool myself = Project.UserInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid;
            bool hasFollowed = Project.UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;
            _cachedView.FollowBtn.SetActiveEx(!myself && !hasFollowed);
            _cachedView.FollowedObj.SetActiveEx(!myself && hasFollowed);
            _cachedView.FavoriteTog.isOn = Project.ProjectUserData != null && Project.ProjectUserData.Favorite;
            DictionaryTools.SetContentText(_cachedView.FavoriteCount, Project.FavoriteCount.ToString());
            DictionaryTools.SetContentText(_cachedView.DownloadCount, Project.ExtendData.DownloadCount.ToString());
            DictionaryTools.SetContentText(_cachedView.ShareCount, Project.ExtendData.ShareCount.ToString());
        }

        private void OnFollowBtn()
        {
            if (Project == null) return;
            LocalUser.Instance.RelationUserList.RequestFollowUser(Project.UserInfoDetail);
        }

        private void OnFavoriteTogValueChanged(bool value)
        {
            if (Project == null || _isRequestFavorite)
            {
                return;
            }
            _isRequestFavorite = true;
            Project.UpdateFavorite(value, () =>
            {
                _isRequestFavorite = false;
                RefreshBtns();
            }, code => { _isRequestFavorite = false; });
        }

        private void OnDownloadBtn()
        {
            if (Project == null)
            {
                return;
            }
            if (_isRequestDownload)
            {
                return;
            }
            _isRequestDownload = true;
            Project.DownloadProject(() =>
            {
                _isRequestDownload = false;
                RefreshBtns();
                SocialGUIManager.ShowPopupDialog("关卡下载成功，请到工坊查看");
            }, code => { _isRequestDownload = false; });
        }

        private void OnShareBtn()
        {
        }

        private void OnPlayBtnClick()
        {
            if (Project == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入关卡");
            //TODO 测试，请求排行榜第一的录像作为影子数据
            if (Project.ProjectRecordRankList.AllList.Count > 0)
            {
                Record record = Project.ProjectRecordRankList.AllList[0].Record;
                Project.RequestPlayShadowBattle(record, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlayShadowBattle(Project, record);
                    SocialApp.Instance.ChangeToGame();
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                });
            }
            else
            {
                Project.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlay(Project);
                    SocialApp.Instance.ChangeToGame();
                }, error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                });
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

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }

        private void OnProjectDataChanged(long projectId)
        {
            if (!_isOpen || Project == null || Project.ProjectId != projectId)
            {
                return;
            }
            RefreshView();
            if (_curMenuCtrl != null)
            {
                ((IOnChangeHandler<long>) _curMenuCtrl).OnChangeHandler(projectId);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
        }

        private void OnChangeToApp()
        {
            if (!_isOpen)
            {
                return;
            }
            if (!_isViewCreated)
            {
                return;
            }
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != null)
                {
                    _menuCtrlArray[i].OnChangeToApp();
                }
            }
        }

        private void OnRelationShipChanged(UserInfoDetail userInfoDetail)
        {
            if (userInfoDetail == Project.UserInfoDetail)
            {
                RefreshBtns();
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                _menuCtrlArray[i].Clear();
            }
        }

        public enum EMenu
        {
            None = -1,
            Recent,
            Rank,
            Comment,
            Max
        }
    }
}