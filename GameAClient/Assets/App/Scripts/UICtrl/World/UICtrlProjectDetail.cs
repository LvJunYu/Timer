using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlProjectDetail : UICtrlAnimationBase<UIViewProjectDetail>
    {
        public static string EmptyStr = "-";
        public static string CountFormat = "({0})";
        public Project Project;
        private bool _isRequestDownload;
        private bool _isRequestFavorite;
        private bool _onlyChangeView;
        private bool _collected;
        private float _srollRectHeight;
        private float _contentHeight;
        private UPCtrlProjectComment _upCtrlProjectComment;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.FavoriteBtn.onClick.AddListener(OnFavoriteTogValueChanged);
            _cachedView.DownloadBtn.onClick.AddListener(OnDownloadBtn);
            _cachedView.ShareBtn.onClick.AddListener(OnShareBtn);
            _cachedView.RecordsBtn.onClick.AddListener(OnRecordsBtn);
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtnClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.GoodTog.onValueChanged.AddListener(OnGoodTogValueChanged);
            _cachedView.BadTog.onValueChanged.AddListener(OnBadTogValueChanged);

            _upCtrlProjectComment = new UPCtrlProjectComment();
            _upCtrlProjectComment.Set(ResScenary);
            _upCtrlProjectComment.Init(this, _cachedView);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Project = parameter as Project;
            _upCtrlProjectComment.Open();
            _srollRectHeight = _cachedView.ScrollRect.rectTransform().rect.height;
            _contentHeight = _cachedView.ScrollRect.content.rect.height;
            _cachedView.ScrollRect.content.anchoredPosition = Vector2.zero;
            if (Project != null)
            {
                Project.Request(Project.ProjectId, null, null);
            }
            RefreshView();
        }

        protected override void OnClose()
        {
            _upCtrlProjectComment.Close();
            Clear();
            base.OnClose();
        }

//        protected override void SetPartAnimations()
//        {
//            base.SetPartAnimations();
//            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
//            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
//        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Overlay;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnProjectDataChanged, OnProjectDataChanged);
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToApp);
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            bool down = _cachedView.ScrollRect.content.anchoredPosition.y >= _contentHeight - _srollRectHeight;
            _cachedView.MouseScrollWheelTool.ScorllWheelDownOff = down && _upCtrlProjectComment.HasComment;
            _cachedView.CommentTableScroller.ScorllWheelDownOff = !down;
            bool up = _cachedView.CommentTableScroller.ContentPosition.y <= 0;
            _cachedView.CommentTableScroller.ScorllWheelUpOff = up && _upCtrlProjectComment.HasComment;
            _cachedView.MouseScrollWheelTool.ScorllWheelUpOff = !up;
        }

        private void SetNull()
        {
            DictionaryTools.SetContentText(_cachedView.TitleText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.ProjectId, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.Desc, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.AdvLevelText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.CreateLevelText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.PlayCountText, EmptyStr);
            DictionaryTools.SetContentText(_cachedView.ProjectCreateDate, EmptyStr);
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
            UserInfoSimple user = Project.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.ProjectId, Project.ShortId.ToString());
            DictionaryTools.SetContentText(_cachedView.TitleText, Project.Name);
            DictionaryTools.SetContentText(_cachedView.Desc, Project.Summary);
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, user.NickName);
            DictionaryTools.SetContentText(_cachedView.AdvLevelText,
                GameATools.GetLevelString(user.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.CreateLevelText,
                GameATools.GetLevelString(user.LevelData.CreatorLevel));
            DictionaryTools.SetContentText(_cachedView.PlayCountText,
                Project.ExtendReady ? Project.PlayCount.ToString() : EmptyStr);
            DictionaryTools.SetContentText(_cachedView.ProjectCreateDate,
                GameATools.FormatServerDateString(Project.CreateTime));
            RefreshBtns();
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, Project.IconPath,
                _cachedView.DefaultCoverTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
        }

        private void RefreshBtns()
        {
            if (Project == null) return;
            bool myself = Project.UserInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid;
            bool hasFollowed = Project.UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;
            _cachedView.FollowBtn.SetActiveEx(!myself);
            DictionaryTools.SetContentText(_cachedView.FollowBtnTxt,
                hasFollowed ? RelationCommonString.FollowedStr : RelationCommonString.FollowStr);
            _collected = Project.ProjectUserData != null && Project.ProjectUserData.Favorite;
            _cachedView.FavoriteTxt.text = _collected ? "已收藏" : "收藏";
            _onlyChangeView = true;
            _cachedView.GoodTog.isOn = Project.ProjectUserData != null &&
                                       Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like;
            _cachedView.BadTog.isOn = Project.ProjectUserData != null &&
                                      Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike;
            _onlyChangeView = false;
            DictionaryTools.SetContentText(_cachedView.ScoreTxt, Project.ScoreFormat);
            DictionaryTools.SetContentText(_cachedView.LikeCountTxt,
                string.Format(CountFormat, Project.LikeCount + Project.UnlikeCount));
        }

        private void OnBadTogValueChanged(bool value)
        {
            if (_onlyChangeView)
            {
                return;
            }
            if (Project == null) return;
            if (Project.ProjectUserData == null)
            {
                Project.Request(Project.ProjectId, null, null);
                return;
            }
            if (Project.ProjectUserData.PlayCount == 0)
            {
                SocialGUIManager.ShowPopupDialog("玩过才能评分哦~~");
                _onlyChangeView = true;
                _cachedView.BadTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }
            if (value && Project.ProjectUserData.LikeState != EProjectLikeState.PLS_Unlike)
            {
                Project.UpdateLike(EProjectLikeState.PLS_Unlike,
                    () => { Project.Request(Project.ProjectId, null, null); });
            }
            else if (!value && Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike &&
                     !_cachedView.GoodTog.isOn)
            {
                Project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { Project.Request(Project.ProjectId, null, null); });
            }
        }

        private void OnGoodTogValueChanged(bool value)
        {
            if (_onlyChangeView)
            {
                return;
            }
            if (Project == null) return;
            if (Project.ProjectUserData == null)
            {
                Project.Request(Project.ProjectId, null, null);
                return;
            }
            if (Project.ProjectUserData.PlayCount == 0)
            {
                SocialGUIManager.ShowPopupDialog("玩过才能评分哦~~");
                _onlyChangeView = true;
                _cachedView.GoodTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }
            if (value && Project.ProjectUserData.LikeState != EProjectLikeState.PLS_Like)
            {
                Project.UpdateLike(EProjectLikeState.PLS_Like,
                    () => { Project.Request(Project.ProjectId, null, null); });
            }
            else if (!value && Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like &&
                     !_cachedView.BadTog.isOn)
            {
                Project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { Project.Request(Project.ProjectId, null, null); });
            }
        }

        private void OnHeadBtn()
        {
            if (Project != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(Project.UserInfoDetail);
//                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
            }
        }

        private void OnFollowBtn()
        {
            if (Project == null) return;
            if (Project.UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe)
            {
                LocalUser.Instance.RelationUserList.RequestRemoveFollowUser(Project.UserInfoDetail);
            }
            else
            {
                LocalUser.Instance.RelationUserList.RequestFollowUser(Project.UserInfoDetail);
            }
        }

        private void OnFavoriteTogValueChanged()
        {
            if (Project == null || _isRequestFavorite)
            {
                return;
            }
            _isRequestFavorite = true;
            Project.UpdateFavorite(!_collected, () =>
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
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetailShare>(Project);
        }

        private void OnRecordsBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetailRecords>(Project);
        }

        private void OnPlayBtnClick()
        {
            if (Project == null)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入关卡");
            //TODO 测试，请求排行榜第一的录像作为影子数据
//            if (Project.ProjectRecordRankList.AllList.Count > 0)
//            {
//                Record record = Project.ProjectRecordRankList.AllList[0].Record;
//                Project.RequestPlayShadowBattle(record, () =>
//                {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    GameManager.Instance.RequestPlayShadowBattle(Project, record);
//                    SocialApp.Instance.ChangeToGame();
//                }, () =>
//                {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
//                });
//            }
//            else
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

        private void OnProjectDataChanged(long projectId)
        {
            if (_isOpen && Project != null && Project.ProjectId == projectId)
            {
                RefreshView();
                _upCtrlProjectComment.OnChangeHandler(projectId);
            }
        }

        private void Clear()
        {
            _onlyChangeView = true;
            _cachedView.GoodTog.isOn = false;
            _cachedView.BadTog.isOn = false;
            _onlyChangeView = false;
            Project = null;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
        }

        private void OnChangeToApp()
        {
            if (_isOpen && Project != null)
            {
                Project.Request(Project.ProjectId, null, null);
                _upCtrlProjectComment.OnChangeToApp();
            }
        }

        private void OnRelationShipChanged(UserInfoDetail userInfoDetail)
        {
            if (Project == null) return;
            if (userInfoDetail == Project.UserInfoDetail)
            {
                RefreshBtns();
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