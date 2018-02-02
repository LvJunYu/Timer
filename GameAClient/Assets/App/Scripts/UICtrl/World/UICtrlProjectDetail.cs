using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlProjectDetail : UICtrlAnimationBase<UIViewProjectDetail>, ICheckOverlay
    {
        private static string EmptyStr = "-";
        private const string CountFormat = "({0})";
        private const string MaxShow = "(999+)";
        private Project _project;
        private bool _isMyself;
        private bool _isMulti;

        private bool _isRequestDownload;
        private bool _isRequestFavorite;
        private bool _onlyChangeView;
        private bool _collected;
        private float _srollRectHeight;
        private float _contentHeight;
        private EMenu _curMenu = EMenu.None;
        private UPCtrlProjectDetailBase _curMenuCtrl;
        private UPCtrlProjectDetailBase[] _menuCtrlArray;
        private long _lastProjectId;
        private bool _isPostComment;

        public bool IsMyself
        {
            get { return _isMyself; }
        }

        public Project Project
        {
            get { return _project; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.FavoriteBtn.onClick.AddListener(OnFavoriteTogValueChanged);
            _cachedView.DownloadBtn.onClick.AddListener(OnDownloadBtn);
            _cachedView.ShareBtn.onClick.AddListener(OnShareBtn);
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
            _cachedView.EditBtn.onClick.AddListener(OnEditBtn);
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtnClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.GoodTog.onValueChanged.AddListener(OnGoodTogValueChanged);
            _cachedView.BadTog.onValueChanged.AddListener(OnBadTogValueChanged);
            _cachedView.PostCommentBtn.onClick.AddListener(OnPostCommentBtn);
            _cachedView.CommentInput.onEndEdit.AddListener(OnCommentInputEndEdit);
            _cachedView.CreateBtn.onClick.AddListener(OnCreateBtn);
            _menuCtrlArray = new UPCtrlProjectDetailBase[(int) EMenu.Max];

            var upCtrlProjectRoomList = new UPCtrlProjectRoomList();
            upCtrlProjectRoomList.SetResScenary(ResScenary);
            upCtrlProjectRoomList.SetMenu(EMenu.Room);
            upCtrlProjectRoomList.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Room] = upCtrlProjectRoomList;

            var upCtrlProjectRecentRecord = new UPCtrlProjectRecentRecord();
            upCtrlProjectRecentRecord.SetResScenary(ResScenary);
            upCtrlProjectRecentRecord.SetMenu(EMenu.Recent);
            upCtrlProjectRecentRecord.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Recent] = upCtrlProjectRecentRecord;

            var upCtrlProjectComment = new UPCtrlProjectComment();
            upCtrlProjectComment.SetResScenary(ResScenary);
            upCtrlProjectComment.SetMenu(EMenu.Comment);
            upCtrlProjectComment.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Comment] = upCtrlProjectComment;

            var upCtrlProjectRankRecord = new UPCtrlProjectRankRecord();
            upCtrlProjectRankRecord.SetResScenary(ResScenary);
            upCtrlProjectRankRecord.SetMenu(EMenu.Rank);
            upCtrlProjectRankRecord.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Rank] = upCtrlProjectRankRecord;

            var upCtrlProjectMultiDetail = new UPCtrlProjectMultiDetail();
            upCtrlProjectMultiDetail.SetResScenary(ResScenary);
            upCtrlProjectMultiDetail.SetMenu(EMenu.MultiDetail);
            upCtrlProjectMultiDetail.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MultiDetail] = upCtrlProjectMultiDetail;
            
            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var index = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i], b =>
                {
                    if (b)
                    {
                        ChangeMenu((EMenu) index);
                    }
                });
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }

            BadWordManger.Instance.InputFeidAddListen(_cachedView.CommentInput);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _project = parameter as Project;
            if (_project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                return;
            }

            if (!CheckProjectValid())
            {
                return;
            }

            _isMyself = _project.UserInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid;
            _isMulti = _project.IsMulti;
            _project.Request(_project.ProjectId, null, null);
            RefreshView();
            if (_project.ProjectId != _lastProjectId)
            {
                if (_isMulti)
                {
                    _curMenu = EMenu.Room;
                }
                else
                {
                    _curMenu = EMenu.Recent;
                }

                _lastProjectId = _project.ProjectId;
            }

            _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }

            Clear();
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.FrontUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnProjectDataChanged, OnProjectDataChanged);
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToApp);
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
            RegisterEvent(EMessengerType.OnPublishDockActiveChanged, OnMessageBoardElementSizeChanged);
            RegisterEvent<long, ProjectCommentReply>(EMessengerType.OnReplyProjectComment, OnReplyProjectComment);
            RegisterEvent<ProjectComment>(EMessengerType.OnDeleteProjectComment, OnDeleteProjectComment);
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

//        public override void OnUpdate()
//        {
//            base.OnUpdate();
//            if (_cachedView.ScrollRect.content.anchoredPosition.y >= _contentHeight - _srollRectHeight - 0.2f)
//            {
//                _cachedView.MouseScrollWheelTool.ScorllWheelDownOff = _upCtrlProjectComment.HasComment;
//                _pannelDownTimer += Time.deltaTime;
//                if (_pannelDownTimer > _scrollSwitchDelay)
//                {
//                    _pannelDown = true;
//                }
//            }
//            else
//            {
//                _cachedView.MouseScrollWheelTool.ScorllWheelDownOff = false;
//                _pannelDownTimer = 0;
//                _pannelDown = false;
//            }
//            _cachedView.CommentTableScroller.ScorllWheelDownOff = !_pannelDown;
//
//            if (_cachedView.CommentTableScroller.ContentPosition.y <= 0.2f)
//            {
//                _cachedView.CommentTableScroller.ScorllWheelUpOff = true;
//                _commentUpTimer += Time.deltaTime;
//                if (_commentUpTimer > _scrollSwitchDelay)
//                {
//                    _commentUp = true;
//                }
//            }
//            else
//            {
//                _cachedView.CommentTableScroller.ScorllWheelUpOff = false;
//                _commentUpTimer = 0;
//                _commentUp = false;
//            }
//            _cachedView.MouseScrollWheelTool.ScorllWheelUpOff = !_commentUp && _cachedView.CommentTableScroller.MouseIn;
//            _cachedView.MouseScrollWheelTool.ScorllWheelUpOff |=
//                _cachedView.RecentGridDataScroller.MouseIn && _upCtrlProjectRecentRecord.HasComment;
//            _cachedView.MouseScrollWheelTool.ScorllWheelDownOff |=
//                _cachedView.RecentGridDataScroller.MouseIn && _upCtrlProjectRecentRecord.HasComment;
//        }

        public bool CheckPlayed(string content)
        {
            if (_isMulti)
            {
                return true;
            }

            if (_project.ProjectUserData.PlayCount == 0)
            {
                SocialGUIManager.ShowPopupDialog(content, null,
                    new KeyValuePair<string, Action>("取消", null),
                    new KeyValuePair<string, Action>("进入", () => { OnPlayBtnClick(); }));
                return false;
            }

            return true;
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
            if (_project == null)
            {
                SetNull();
                return;
            }

            _cachedView.MenuButtonAry[(int) EMenu.Room].SetActiveEx(_isMulti);
            _cachedView.MenuButtonAry[(int) EMenu.Recent].SetActiveEx(!_isMulti);
            _cachedView.MenuButtonAry[(int) EMenu.Rank].SetActiveEx(!_isMulti);
            _cachedView.MenuButtonAry[(int) EMenu.MultiDetail].SetActiveEx(_isMulti);
            _cachedView.CreateBtn.SetActiveEx(_isMulti);
            _cachedView.EditBtn.SetActiveEx(_isMyself);
            _cachedView.DeleteBtn.SetActiveEx(_isMyself);
            _cachedView.FavoriteBtn.SetActiveEx(!_isMyself);
            _cachedView.DownloadBtn.SetActiveEx(!_isMyself);
            UserInfoSimple user = _project.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.ProjectId, _project.ShortId.ToString());
            DictionaryTools.SetContentText(_cachedView.TitleText, _project.Name);
            DictionaryTools.SetContentText(_cachedView.Desc, _project.ShowSummary);
            DictionaryTools.SetContentText(_cachedView.UserNickNameText, user.NickName);
            DictionaryTools.SetContentText(_cachedView.AdvLevelText,
                GameATools.GetLevelString(user.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.CreateLevelText,
                GameATools.GetLevelString(user.LevelData.CreatorLevel));
            DictionaryTools.SetContentText(_cachedView.PlayCountText,
                _project.ExtendReady ? _project.PlayCount.ToString() : EmptyStr);
            DictionaryTools.SetContentText(_cachedView.ProjectCreateDate,
                GameATools.FormatServerDateString(_project.CreateTime));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultCoverTexture);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _project.IconPath,
                _cachedView.DefaultCoverTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
            RefreshBtns();
            RefreshCommentCount(_project.TotalCommentCount);
        }

        private void RefreshBtns()
        {
            if (_project == null) return;
            bool hasFollowed = _project.UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;
            _cachedView.FollowBtn.SetActiveEx(!_isMyself);
            DictionaryTools.SetContentText(_cachedView.FollowBtnTxt,
                hasFollowed ? RelationCommonString.FollowedStr : RelationCommonString.FollowStr);
            _collected = _project.ProjectUserData != null && _project.ProjectUserData.Favorite;
            _cachedView.FavoriteTxt.text = _collected ? RelationCommonString.CollectedStr : RelationCommonString.CollectStr;
            _onlyChangeView = true;
            _cachedView.GoodTog.isOn = _project.ProjectUserData != null &&
                                       _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like;
            _cachedView.BadTog.isOn = _project.ProjectUserData != null &&
                                      _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike;
            _onlyChangeView = false;
            DictionaryTools.SetContentText(_cachedView.ScoreTxt, _project.ScoreFormat);
            DictionaryTools.SetContentText(_cachedView.LikeCountTxt,
                string.Format(CountFormat, _project.LikeCount + _project.UnlikeCount));
        }

        private void OnCreateBtn()
        {
            if (_isMulti)
            {
                RoomManager.Instance.SendRequestCreateRoom(_project.ProjectId);
            }
        }

        private void RefreshCommentCount(int count)
        {
            _cachedView.CommentCount.SetActiveEx(count > 0);
            _cachedView.CommentSelectedCount.SetActiveEx(count > 0);
            if (count > 0)
            {
                _cachedView.CommentCount.text = count < 1000 ? string.Format(CountFormat, count) : MaxShow;
                _cachedView.CommentSelectedCount.text = count < 1000 ? string.Format(CountFormat, count) : MaxShow;
            }
        }

        private void OnBadTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (_project == null) return;
            if (_project.ProjectUserData == null)
            {
                _project.Request();
                return;
            }

            if (!CheckPlayed("玩过才能评分哦~~现在进入关卡吗？"))
            {
                _onlyChangeView = true;
                _cachedView.BadTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }

            if (value && _project.ProjectUserData.LikeState != EProjectLikeState.PLS_Unlike)
            {
                _project.UpdateLike(EProjectLikeState.PLS_Unlike,
                    () => { _project.Request(); });
            }
            else if (!value && _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike &&
                     !_cachedView.GoodTog.isOn)
            {
                _project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { _project.Request(); });
            }
        }

        private void OnGoodTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (_project == null) return;
            if (_project.ProjectUserData == null)
            {
                _project.Request(_project.ProjectId, null, null);
                return;
            }

            if (!CheckPlayed("玩过才能评分哦~~现在进入关卡吗？"))
            {
                _onlyChangeView = true;
                _cachedView.GoodTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }

            if (value && _project.ProjectUserData.LikeState != EProjectLikeState.PLS_Like)
            {
                _project.UpdateLike(EProjectLikeState.PLS_Like,
                    () => { _project.Request(); });
            }
            else if (!value && _project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like &&
                     !_cachedView.BadTog.isOn)
            {
                _project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { _project.Request(); });
            }
        }

        private void OnCommentInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnPostCommentBtn();
            }
        }

        private void OnPostCommentBtn()
        {
            if (_project == null || _project.ProjectUserData == null) return;
            if (!CheckPlayed("玩过才能评论哦~~现在进入关卡吗？"))
            {
                return;
            }

            if (_isPostComment)
            {
                return;
            }

            if (string.IsNullOrEmpty(_cachedView.CommentInput.text))
            {
                return;
            }

            _isPostComment = true;
            _project.SendComment(_cachedView.CommentInput.text, flag =>
            {
                _isPostComment = false;
                if (flag)
                {
                    _cachedView.CommentInput.text = string.Empty;
                    _cachedView.CommentTableScroller.ContentPosition = Vector2.zero;
                    if (_curMenu != EMenu.Comment)
                    {
                        _curMenu = EMenu.Comment;
                        _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
                    }

                    _project.Request();
                }
            });
        }

        private void OnHeadBtn()
        {
            if (_project != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_project.UserInfoDetail);
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

        private void OnEditBtn()
        {
            Project.Edit();
        }

        private void OnDeleteBtn()
        {
            if (null == Project) return;
            CommonTools.ShowPopupDialog(
                string.Format("删除之后将无法恢复，确定要删除《{0}》吗？", Project.Name), "删除提示",
                new KeyValuePair<string, Action>("取消", () => { }),
                new KeyValuePair<string, Action>("确定", () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在删除");
                    Project.UnPublish(() =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                    }, () =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        CommonTools.ShowPopupDialog("删除关卡失败");
                    });
                }));
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
            }, code =>
            {
                _isRequestFavorite = false;
                SocialGUIManager.ShowPopupDialog("收藏关卡失败");
            });
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
                SocialGUIManager.ShowPopupDialog("关卡下载成功，请到工坊查看\n    (下载的地图不能发布)");
            }, code => { _isRequestDownload = false; });
        }

        private void OnShareBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetailShare>(Project);
        }

        private void OnPlayBtnClick()
        {
            if (_project == null)
            {
                return;
            }
            if (_isMulti)
            {
                RoomManager.Instance.SendRequestQuickPlay(EQuickPlayType.EQPT_Specific, _project.ProjectId);
            }
            else
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入关卡");
                _project.RequestPlay(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    GameManager.Instance.RequestPlay(_project);
                    SocialApp.Instance.ChangeToGame();
                }, error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                }); 
            }
        }

        private bool CheckProjectValid()
        {
            if (!_project.IsValid)
            {
                SocialGUIManager.ShowPopupDialog("关卡已被原作者删除");
                Messenger.Broadcast(EMessengerType.OnProjectNotValid);
                if (_project.UserFavorite)
                {
                    _project.UpdateFavorite(false);
                }

                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                return false;
            }

            return true;
        }

        private void OnProjectDataChanged(long projectId)
        {
            if (_isOpen && _project != null && _project.ProjectId == projectId)
            {
                if (!CheckProjectValid())
                {
                    return;
                }

                RefreshView();
                if (_curMenuCtrl != null)
                {
                    _curMenuCtrl.OnChangeHandler(projectId);
                }
            }
        }

        private void OnMessageBoardElementSizeChanged()
        {
            if (_isOpen && _curMenu == EMenu.Comment)
            {
                _cachedView.CommentTableScroller.RefreshAllSizes();
            }
        }

        private void OnReplyProjectComment(long commentId, ProjectCommentReply reply)
        {
            if (_isOpen && _curMenu == EMenu.Comment)
            {
                ((UPCtrlProjectComment) _curMenuCtrl).OnReplyProjectComment(commentId, reply);
            }
        }

        private void OnDeleteProjectComment(ProjectComment comment)
        {
            if (_isOpen && _curMenu == EMenu.Comment)
            {
                ((UPCtrlProjectComment) _curMenuCtrl).OnDeleteUserMessage(comment);
            }
        }

        private void Clear()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                _menuCtrlArray[i].Clear();
            }

            _onlyChangeView = true;
            _cachedView.GoodTog.isOn = false;
            _cachedView.BadTog.isOn = false;
            _onlyChangeView = false;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
        }

        private void OnChangeToApp()
        {
            if (_isOpen && _project != null)
            {
                _project.Request();
                if (_curMenuCtrl != null)
                {
                    _curMenuCtrl.OnChangeToApp();
                }
            }
        }

        private void OnRelationShipChanged(UserInfoDetail userInfoDetail)
        {
            if (_project == null) return;
            if (userInfoDetail == _project.UserInfoDetail)
            {
                RefreshBtns();
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
            _cachedView.DownDock.SetActive(_curMenu != EMenu.MultiDetail);
        }

        public enum EMenu
        {
            None = -1,
            Room,
            Recent,
            Comment,
            Rank,
            MultiDetail,
            Max
        }
    }
}