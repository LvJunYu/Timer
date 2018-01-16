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
        public static string EmptyStr = "-";
        public const string _countFormat = "({0})";
        private const string _maxShow = "(999+)";
        public Project Project;
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
        public bool IsMulti;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.FavoriteBtn.onClick.AddListener(OnFavoriteTogValueChanged);
            _cachedView.DownloadBtn.onClick.AddListener(OnDownloadBtn);
            _cachedView.ShareBtn.onClick.AddListener(OnShareBtn);
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

            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var index = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i],
                    b => ClickMenu(index, b));
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
            Project = parameter as Project;
            if (Project == null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                return;
            }
            if (!Project.IsValid)
            {
                SocialGUIManager.ShowPopupDialog("关卡已被原作者删除");
                if (Project.UserFavorite)
                {
                    Project.UpdateFavorite(false);
                }
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                return;
            }
            IsMulti = Project.IsMulti;
            Project.Request(Project.ProjectId, null, null);
            RefreshView();
            if (Project.ProjectId != _lastProjectId)
            {
                if (IsMulti)
                {
                    _curMenu = EMenu.Room;
                }
                else
                {
                    _curMenu = EMenu.Recent;
                }
                _lastProjectId = Project.ProjectId;
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
            if (IsMulti)
            {
                return true;
            }
            if (Project.ProjectUserData.PlayCount == 0)
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
            if (Project == null)
            {
                SetNull();
                return;
            }
            _cachedView.MenuButtonAry[(int) EMenu.Room].SetActiveEx(IsMulti);
            _cachedView.MenuButtonAry[(int) EMenu.Recent].SetActiveEx(!IsMulti);
            _cachedView.MenuButtonAry[(int) EMenu.Rank].SetActiveEx(!IsMulti);
            _cachedView.StandalonePannel.SetActive(!IsMulti);
            _cachedView.MultiPannel.SetActive(IsMulti);
            _cachedView.CreateBtn.SetActiveEx(IsMulti);
            if (IsMulti)
            {
                RefreshRoomInfo();
            }
            else
            {
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
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                    _cachedView.DefaultCoverTexture);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, Project.IconPath,
                    _cachedView.DefaultCoverTexture);
                user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                    _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
            }
            RefreshBtns();
            RefreshCommentCount(Project.TotalCommentCount);
        }

        private void RefreshRoomInfo()
        {
            if (Project == null) return;
            var netData = Project.NetData;
            if (netData == null) return;
            _cachedView.TitleTxt.text = Project.Name;
            _cachedView.DescTxt.text = Project.Summary;
            _cachedView.PlayerCount.text = netData.PlayerCount.ToString();
            _cachedView.LifeCount.text = netData.GetLifeCount();
            _cachedView.ReviveTime.text = netData.GetReviveTime();
            _cachedView.ReviveProtectTime.text = netData.GetReviveProtectTime();
            _cachedView.TimeLimit.text = netData.GetTimeLimit();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.WinScore.ToString();
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
            _cachedView.WinScoreCondition.SetActiveEx(netData.ScoreWinCondition);
//            _cachedView.ArriveScore.SetActiveEx(Game.PlayMode.Instance.SceneState.FinalCount > 0);
//            _cachedView.CollectGemScore.SetActiveEx(Game.PlayMode.Instance.SceneState.TotalGem > 0);
//            _cachedView.KillMonsterScore.SetActiveEx(Game.PlayMode.Instance.SceneState.MonsterCount > 0);
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
                string.Format(_countFormat, Project.LikeCount + Project.UnlikeCount));
        }

        private void OnCreateBtn()
        {
            if (IsMulti)
            {
                RoomManager.Instance.SendRequestCreateRoom(Project.ProjectId);
            }
        }

        private void RefreshCommentCount(int count)
        {
            _cachedView.CommentCount.SetActiveEx(count > 0);
            _cachedView.CommentSelectedCount.SetActiveEx(count > 0);
            if (count > 0)
            {
                _cachedView.CommentCount.text = count < 1000 ? string.Format(_countFormat, count) : _maxShow;
                _cachedView.CommentSelectedCount.text = count < 1000 ? string.Format(_countFormat, count) : _maxShow;
            }
        }

        private void OnBadTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (Project == null) return;
            if (Project.ProjectUserData == null)
            {
                Project.Request();
                return;
            }
            if (!CheckPlayed("玩过才能评分哦~~现在进入关卡吗？"))
            {
                _onlyChangeView = true;
                _cachedView.BadTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }
            if (value && Project.ProjectUserData.LikeState != EProjectLikeState.PLS_Unlike)
            {
                Project.UpdateLike(EProjectLikeState.PLS_Unlike,
                    () => { Project.Request(); });
            }
            else if (!value && Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Unlike &&
                     !_cachedView.GoodTog.isOn)
            {
                Project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { Project.Request(); });
            }
        }

        private void OnGoodTogValueChanged(bool value)
        {
            if (_onlyChangeView) return;
            if (Project == null) return;
            if (Project.ProjectUserData == null)
            {
                Project.Request(Project.ProjectId, null, null);
                return;
            }
            if (!CheckPlayed("玩过才能评分哦~~现在进入关卡吗？"))
            {
                _onlyChangeView = true;
                _cachedView.GoodTog.isOn = !value;
                _onlyChangeView = false;
                return;
            }
            if (value && Project.ProjectUserData.LikeState != EProjectLikeState.PLS_Like)
            {
                Project.UpdateLike(EProjectLikeState.PLS_Like,
                    () => { Project.Request(); });
            }
            else if (!value && Project.ProjectUserData.LikeState == EProjectLikeState.PLS_Like &&
                     !_cachedView.BadTog.isOn)
            {
                Project.UpdateLike(EProjectLikeState.PLS_AllRight,
                    () => { Project.Request(); });
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
            if (Project == null || Project.ProjectUserData == null) return;
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
            Project.SendComment(_cachedView.CommentInput.text, flag =>
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
                    Project.Request();
                }
            });
        }

        private void OnHeadBtn()
        {
            if (Project != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(Project.UserInfoDetail);
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
                SocialGUIManager.ShowPopupDialog("关卡下载成功，请到工坊查看\n    (下载的地图不能发布)");
            }, code => { _isRequestDownload = false; });
        }

        private void OnShareBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetailShare>(Project);
        }

        private void OnPlayBtnClick()
        {
            if (Project == null || Project.IsMulti)
            {
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "请求进入关卡");
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

        private void OnProjectDataChanged(long projectId)
        {
            if (_isOpen && Project != null && Project.ProjectId == projectId)
            {
                if (!Project.IsValid)
                {
                    SocialGUIManager.ShowPopupDialog("关卡已被原作者删除");
                    if (Project.UserFavorite)
                    {
                        Project.UpdateFavorite(false);
                    }
                    SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
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
            if (_curMenu == EMenu.Comment)
            {
                ((UPCtrlProjectComment) _curMenuCtrl).OnReplyProjectComment(commentId, reply);
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
//            Project = null;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
        }

        private void OnChangeToApp()
        {
            if (_isOpen && Project != null)
            {
                Project.Request();
                if (_curMenuCtrl != null)
                {
                    _curMenuCtrl.OnChangeToApp();
                }
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

        public enum EMenu
        {
            None = -1,
            Room,
            Recent,
            Comment,
            Rank,
            Max
        }
    }
}