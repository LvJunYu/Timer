/********************************************************************
** Filename : UICtrlProjectDetail
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlProjectDetail
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlProjectDetail : UISocialContentCtrlBase<UIViewProjectDetail>, IUIWithTitle
    {
        #region 常量与字段
        private const string SummaryTemplate = "简介：{0}";
        private const long RequestInterval = 1 * GameTimer.Minute2Ticks;
        private bool _isRequest = false;
        private ProjectParams _content;
        private bool _commentRequest = false;
        private bool _collapseSumFlag = false;
        #endregion

        #region 属性

        #endregion

        #region 方法
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PlayBtn.onClick.AddListener(OnPlayClick);
            _cachedView.AuthorBtn.onClick.AddListener(OnAuthorClick);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtnClick);
            _cachedView.UnfollowBtn.onClick.AddListener(OnFollowBtnClick);
            _cachedView.FavoriteBtn.onClick.AddListener(OnFavoriteBtnClick);
            _cachedView.DownloadBtn.onClick.AddListener(OnDownloadBtnClick);
            _cachedView.ShareBtn.onClick.AddListener(OnShareBtnClick);
            _cachedView.LikeBtn.onClick.AddListener(OnLikeBtnClick);
            _cachedView.CollapseSumBtn.onClick.AddListener(OnCollapseSumBtnClick);



            _cachedView.CommentInput.SendBtn.onClick.AddListener(OnPostCommentBtnClick);




            _cachedView.TagGroup.AddButton(_cachedView.SummaryBtn, OnSummaryBtnClick);
            _cachedView.TagGroup.AddButton(_cachedView.RecentBtn, OnRecentBtnClick);
            _cachedView.TagGroup.AddButton(_cachedView.RankBtn, OnRankBtnClick);
            _cachedView.TagGroup.AddButton(_cachedView.CommentBtn, OnCommentBtnClick);



            _cachedView.CommentInput.Input.characterLimit = SoyConstDefine.MaxProjectCommentLength;
//            _cachedView.CommentList.ItemClickCallback = OnCommentItemClick;

//            _cachedView.AddMixRecommend.onClick.AddListener(()=>{
//                MatrixProjectTools.AdminRecommendContent(this, ERecommendCategory.RC_Mix, EAppContentItemType.ACIT_Project, _content.Project.Guid);
//            });
//            _cachedView.AddOwnRecommend.onClick.AddListener(()=>{
//                MatrixProjectTools.AdminRecommendContent(this, (ERecommendCategory)(int)_content.Project.ProjectCategory, EAppContentItemType.ACIT_Project, _content.Project.Guid);
//            });
        }

        private void OnAuthorClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlUserInfo>(_content.Project.UserLegacy);
        }

        private void OnPlayClick()
        {
            Play(_content);
        }


        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            ProjectParams param = parameter as ProjectParams;
            if(param == null)
            {
                LogHelper.Error("UICtrlProjectDetail ProjectData Not Ready");
                return;
            }
            const int tagHeight = 64;
            _cachedView.CommentListLayoutElement.preferredHeight = _cachedView.CommentListLayoutElement.minHeight = _cachedView.Trans.GetHeight() - tagHeight;
            _cachedView.CommentContentLayoutElement.minHeight = _cachedView.CommentListLayoutElement.preferredHeight - 0.01f;
            _cachedView.ProjectPlayListLayoutElement.preferredHeight = _cachedView.ProjectPlayListLayoutElement.minHeight = _cachedView.Trans.GetHeight() - tagHeight;
            _cachedView.ProjectPlayListContentLayoutElement.minHeight = _cachedView.ProjectPlayListLayoutElement.preferredHeight - 0.01f;
            _cachedView.ProjectRecentListLayoutElement.preferredHeight = _cachedView.ProjectRecentListLayoutElement.minHeight = _cachedView.Trans.GetHeight() - tagHeight;
            _cachedView.ProjectRecentListContentLayoutElement.minHeight = _cachedView.ProjectRecentListContentLayoutElement.preferredHeight - 0.01f;
            if(_content == null ||  _content.Project != param.Project || _content.Type != param.Type)
            {
                _content = param;
//                _cachedView.CommentList.SetProject(_content.Project);
                _cachedView.TagGroup.SelectIndex(0, true);
                CollapseSum(true);
                ScrollToTop();
            }
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
//            _cachedView.CommentList.OnClose();
            base.Close();
        }

        private void RequestData()
        {
            if(_isRequest)
            {
                return;
            }
            Project project = _content.Project;
            GameTimer timer = project.ProjectIntoRequestTimer;
            if(timer.GetInterval() > RequestInterval)
            {
                _isRequest = true;
                Msg_CS_DAT_Project msg = new Msg_CS_DAT_Project();
                msg.ProjectId = _content.Project.ProjectId;
                NetworkManager.AppHttpClient.SendWithCb<Msg_SC_DAT_Project>(SoyHttpApiPath.Project, msg, ret=>{
                    _isRequest = false;
                    ProjectManager.Instance.OnSyncProject(ret);
                    project.ProjectIntoRequestTimer.Reset();
                    if(project == _content.Project)
                    {
                        RefreshStatisticAndUserInfo();
                        RefreshAuthorView();
                    }
                }, (code, msgStr)=>{
                    _isRequest = false;
                });

//                Msg_CA_RequestProjectRecentPlayedUserList msg2 = new Msg_CA_RequestProjectRecentPlayedUserList();
//                msg2.ProjectGuid = _content.Project.Guid;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_ProjectRecentPlayedUserList>(SoyHttpApiPath.GetRecentPlayedProjectUserList, msg2, ret=>{
//                    project.OnSyncRecentPlayedUserDataList(ret);
//                    if(project == _content.Project)
//                    {
//                        RefreshRecentPlayedUser();
//                    }
//                });
            }
        }

        public void ChangeToCommentTab()
        {
            _cachedView.TagGroup.SelectIndex(3);
        }

        private void RefreshView()
        {
//            _cachedView.TagGroup.SelectIndex(_cachedView.TagGroup.CurInx, true);
//            Project pj = _content.Project;
//            if(_content.Project.ProjectStatus == EProjectStatus.PS_Public && !_content.Project.IsValid)
//            {
//                DictionaryTools.SetContentText(_cachedView.ProjectName, "[已删除]");
//            }
//            else
//            {
//                DictionaryTools.SetContentText(_cachedView.ProjectName, pj.Name);
//            }
//            if(string.IsNullOrEmpty(pj.Summary))
//            {
//                _cachedView.CollapseSumBtn.gameObject.SetActive(false);
//            }
//            else
//            {
//                _cachedView.CollapseSumBtn.gameObject.SetActive(true);
//                DictionaryTools.SetContentText(_cachedView.Summary, string.Format(SummaryTemplate, pj.Summary));
//            }
//
//            ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectCover, pj.IconPath, _cachedView.DefaultTexture);
//            _cachedView.CreateTime.text =  DateTimeUtil.UnixTimestampMillisToLocalDateTime(_content.Project.CreateTime).ToString("MM-dd HH:mm");
//            if(_content.Project.DownloadPrice < 0)
//            {
//                DictionaryTools.SetContentText(_cachedView.DownloadBtnText, "不可下载");
//                _cachedView.DownloadPriceImage.gameObject.SetActive(false);
//            }
//            else if(_content.Project.DownloadPrice == 0)
//            {
//                DictionaryTools.SetContentText(_cachedView.DownloadBtnText, "免费下载");
//                _cachedView.DownloadPriceImage.gameObject.SetActive(false);
//            }
//            else
//            {
//                DictionaryTools.SetContentText(_cachedView.DownloadBtnText, ""+_content.Project.DownloadPrice);
//                _cachedView.DownloadPriceImage.gameObject.SetActive(true);
//            }
//            RefreshAuthorView();
//            RefreshStatisticAndUserInfo();
//            RefreshRecentPlayedUser();
//
//            _cachedView.AdminDock.SetActive(LocalUser.Instance.UserLegacy!=null && LocalUser.Instance.UserLegacy.AccountRoleType == EAccountRoleType.AcRT_Admin);
        }

        private void RefreshStatisticAndUserInfo()
        {
            _cachedView.RateStarAry.SetRate(_content.Project.TotalRate);
            DictionaryTools.SetContentText(_cachedView.RateCount, "(" + _content.Project.TotalRateCount +")");
            _cachedView.ProjectCompleteRate.Set(_content.Project.ExtendReady, _content.Project.CompleteCount, _content.Project.FailCount);
            DictionaryTools.SetContentText(_cachedView.CommentBtnText, "评论("+_content.Project.TotalCommentCount+")");

            if(_content.Project.UserFavorite)
            {
                _cachedView.FavoriteBtnImage.sprite = _cachedView.FavoriteSprite;
                DictionaryTools.SetContentText(_cachedView.FavoriteBtnText, "已收藏");
            }
            else
            {
                _cachedView.FavoriteBtnImage.sprite = _cachedView.NotFavoriteSprite;
                DictionaryTools.SetContentText(_cachedView.FavoriteBtnText, "收藏");
            }

            if(_content.Project.UserLike == EProjectLikeState.PLS_Like)
            {
                _cachedView.LikeBtnImage.sprite = _cachedView.LikeSprite;
            }
            else
            {
                _cachedView.LikeBtnImage.sprite = _cachedView.DislikeSprite;
            }

            DictionaryTools.SetContentText(_cachedView.FavoriteCountText, ClientTools.FormatNumberString(_content.Project.FavoriteCount));
            DictionaryTools.SetContentText(_cachedView.DownloadCountText, ClientTools.FormatNumberString(_content.Project.DownloadCount));
            DictionaryTools.SetContentText(_cachedView.ShareCountText, ClientTools.FormatNumberString(_content.Project.ShareCount));
            DictionaryTools.SetContentText(_cachedView.LikeCountText, ClientTools.FormatNumberString(_content.Project.LikeCount));
        }

        private void RefreshAuthorView()
        {
            User user = _content.Project.UserLegacy;
            if(user.Sex == ESex.S_None)
            {
                _cachedView.SexImg.gameObject.SetActive(false);
            }
            else
            {
                _cachedView.SexImg.gameObject.SetActive(true);
                _cachedView.SexImg.sprite = AppResourceManager.Instance.GetSprite(SpriteNameDefine.GetSexIcon(user.Sex));
            }

            if(LocalUser.Instance.UserGuid == user.UserId)
            {
                _cachedView.FollowBtn.gameObject.SetActive(false);
                _cachedView.UnfollowBtn.gameObject.SetActive(false);
            }
            else
            {
                if(user.FollowedByMe)
                {
                    _cachedView.FollowBtn.gameObject.SetActive(false);
                    _cachedView.UnfollowBtn.gameObject.SetActive(true);
                }
                else
                {
                    _cachedView.FollowBtn.gameObject.SetActive(true);
                    _cachedView.UnfollowBtn.gameObject.SetActive(false);
                }
            }
            DictionaryTools.SetContentText(_cachedView.AuthorName, _content.Project.UserLegacy.NickName);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, _content.Project.UserLegacy.HeadImgUrl, _cachedView.DefaultTexture);
        }

        private void RefreshRecentPlayedUser()
        {
//            if(_content.Project.RecentPlayedUserList == null || _content.Project.RecentPlayedUserList.Count == 0)
//            {
//                _cachedView.RecentPlayedProjectUserList.Hide();
//                _cachedView.RecentPlayedProjectUserTip.SetActive(true);
//            }
//            else
//            {
//                _cachedView.RecentPlayedProjectUserList.Show();
//                _cachedView.RecentPlayedProjectUserList.Set(_content.Project.RecentPlayedUserList);
//                _cachedView.RecentPlayedProjectUserTip.SetActive(false);
//            }
        }

        private void OnChangeToAppMode()
        {
            if(_isViewCreated && _isOpen)
            {
                _content.Project.ProjectIntoRequestTimer.Zero();
                RequestData();
                RefreshView();
            }
        }

        private void OnSummaryBtnClick(bool open)
        {
            _cachedView.SummaryDock.gameObject.SetActive(open);
        }

        private void OnRecentBtnClick(bool open)
        {
            _cachedView.RecentDock.SetActive(open);
            if(open)
            {
            }
        }

        private void OnRankBtnClick(bool open)
        {
            _cachedView.RankDock.SetActive(open);
            if(open)
            {
            }
        }

        private void OnCommentBtnClick(bool open)
        {
            _cachedView.CommentDock.SetActive(open);
            _cachedView.CommentInput.gameObject.SetActive(open && _content.Project.IsValid);
            _cachedView.CommentInput.ClearAll();
            if(open)
            {
//                _cachedView.CommentList.Refresh();
            }
        }

        private bool _isFollowStateRequest = false;
        private void OnFollowBtnClick()
        {
//            if(!AppLogicUtil.CheckAndRequiredLogin())
//            {
//                return;
//            }
//            if(_isFollowStateRequest)
//            {
//                return;
//            }
//            User user = _content.Project.UserLegacy;
//            _isFollowStateRequest = true;
//            user.UpdateFollowState(!_content.Project.UserLegacy.FollowedByMe, flag=>{
//                _isFollowStateRequest = false;
//                if(flag)
//                {
//                    if(user.UserId == _content.Project.UserLegacy.UserId)
//                    {
//                        RefreshAuthorView();
//                    }
//                    if(LocalUser.Instance.UserLegacy != null)
//                    {
//                        LocalUser.Instance.UserLegacy.FollowedListRequestTimer.Zero();
//                        LocalUser.Instance.UserLegacy.UserInfoRequestGameTimer.Zero();
//                    }
//                    user.FollowerListRequestTimer.Zero();
//                    user.UserInfoRequestGameTimer.Zero();
//                }
//            });
        }

        private bool _isFavoriteRequest = false;
        private void OnFavoriteBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }

            if(!_content.Project.UserFavorite && !CheckValidAndTip())
            {
                return;
            }

            if(_isFavoriteRequest)
            {
                return;
            }

            Project project = _content.Project;
            _isFavoriteRequest = true;
            _content.Project.UpdateFavorite(!_content.Project.UserFavorite, flag=>{
                _isFavoriteRequest = false;
                if(flag)
                {
                    if(_isOpen && project.ProjectId == _content.Project.ProjectId)
                    {
                        RefreshStatisticAndUserInfo();
                    }
                }
            });
        }

        private void OnDownloadBtnClick()
        {
//            if(!AppLogicUtil.CheckAndRequiredLogin())
//            {
//                return;
//            }
//            string tipString;
//            if(LocalUser.Instance.UserLegacy.UserId == _content.Project.UserLegacy.UserId)
//            {
//                tipString = "花费0金币下载这个作品";
//            }
//            else
//            {
//                if(_content.Project.DownloadPrice < 0)
//                {
//                    return;
//                }
//                tipString = string.Format("花费{0}金币下载这个作品", _content.Project.DownloadPrice);
//            }
//            CommonTools.ShowPopupDialog(tipString,null,
//                new System.Collections.Generic.KeyValuePair<string, Action>("确定", ()=>{
//                    DownloadProject();
//                }), 
//                new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
//
//                }));
        }
        private void DownloadProject()
        {
//            int price = 0;
//            if(LocalUser.Instance.UserLegacy != null && LocalUser.Instance.UserLegacy.UserId == _content.Project.UserLegacy.UserId)
//            {
//            }
//            else if(_content.Project.DownloadPrice < 0)
//            {
//                return;
//            }
//            else if(_content.Project.DownloadPrice == 0)
//            {
//            }
//            else
//            {
//                price = _content.Project.DownloadPrice;
//            }
//            User user = LocalUser.Instance.UserLegacy;
//            Project project = _content.Project;
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "");
//            MatrixProjectTools.PreparePersonalProjectData(()=>{
////                var userMatrixData = AppData.Instance.UserMatrixData.GetData(project.MatrixGuid);
//                int localCount = LocalUser.Instance.UserLegacy.GetSavedProjectCount();
////                if(userMatrixData.PersonalProjectWorkshopSize <= localCount)
////                {
////                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
////                    ShowDownloadFailedTip(EProjectOperateResult.POR_WorkshopSizeNotEnough);
////                    return;
////                }
//                project.DownloadProject(()=>{
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    CommonTools.ShowPopupDialog("作品下载成功，请到工坊查看");
//                }, failedCode=>{
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    ShowDownloadFailedTip(failedCode);
//                });
//            }, ()=>{
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                CommonTools.ShowPopupDialog("用户数据获取失败，下载失败");
//            });
        }

        private void ShowDownloadFailedTip(EProjectOperateResult code)
        {
            if(code == EProjectOperateResult.POR_WorkshopSizeNotEnough)
            {
                CommonTools.ShowPopupDialog("工坊已满，请升级匠人经验或者前去工坊整理");
            }
            else if(code == EProjectOperateResult.POR_Error)
            {
                CommonTools.ShowPopupDialog("发生错误，作品下载失败");
            }
        }

        private void OnShareBtnClick()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlShareMenu>(_content.Project);
        }

        private void OnPostCommentBtnClick()
        {
            if(!AppLogicUtil.CheckAndRequiredLogin())
            {
                return;
            }

            if(string.IsNullOrEmpty(_cachedView.CommentInput.Input.text))
            {
                return;
            }
            if(!CheckValidAndTip())
            {
                return;
            }
            if(_commentRequest)
            {
                return;
            }
            _commentRequest = true;
            Project project = _content.Project;
//            _content.Project.SendComment(_cachedView.CommentInput.Input.text, flag=>{
//                _commentRequest = false;
//                if(flag)
//                {
//                    if(project == _content.Project)
//                    {
//                        _cachedView.CommentList.Refresh();
//                        _cachedView.CommentInput.ClearInput();
//                        _content.Project.ProjectIntoRequestTimer.Zero();
//                        _content.Project.TotalCommentCount++;
//                        RefreshStatisticAndUserInfo();
//                    }
//                }
//                else
//                {
//                    CommonTools.ShowPopupDialog("网络错误，评论失败");
//                }
//            }, _cachedView.CommentInput.TargetUser);
        }

        private bool _isLikeRequest = false;
        private void OnLikeBtnClick()
        {
//            if(!AppLogicUtil.CheckAndRequiredLogin())
//            {
//                return;
//            }
//
//            if(!_content.Project.UserLike && !CheckValidAndTip())
//            {
//                return;
//            }
//
//            if(_isLikeRequest)
//            {
//                return;
//            }
//
//            Project project = _content.Project;
//            _isLikeRequest = true;
//            _content.Project.UpdateLike(!_content.Project.UserLike, flag=>{
//                _isLikeRequest = false;
//                if(flag)
//                {
//                    if(_isOpen && _content != null && _content.Project != null && project.ProjectId == _content.Project.ProjectId)
//                    {
//                        RefreshStatisticAndUserInfo();
//                    }
//                }
//            });
        }

        private bool CheckValidAndTip()
        {
            if(!_content.Project.IsValid)
            {
                CommonTools.ShowPopupDialog("作品已被删除，无法进行操作");
                return false;
            }
            return true;
        }

//        private void OnCommentItemClick(UMCtrlProjectComment item)
//        {
//            if(item.Content.UserInfo.UserGuid == LocalUser.Instance.UserGuid)
//            {
//                return;
//            }
//            _cachedView.CommentInput.SetTargetUser(item.Content.UserInfo);
//        }

        private void OnCollapseSumBtnClick()
        {
            CollapseSum(!_collapseSumFlag);
        }

        private void CollapseSum(bool flag)
        {
            _collapseSumFlag = flag;
            _cachedView.CollapsibleSumDock.SetActive(!flag);
            DictionaryTools.SetContentText(_cachedView.CollapseSumText, flag ? "+展开简介":"-收起简介");
        }

        public object GetTitle()
        {
            return "作品详情";
        }

        #endregion

        public static void Play(ProjectParams content)
        {
            if(!MatrixProjectTools.CheckProjectValidAndShowTip(content.Project))
            {
                return;
            }
            EMatrixProjectResState resState = EMatrixProjectResState.None;
            if(!MatrixProjectTools.CheckProjectStateForRun(content.Project, out resState))
            {
                MatrixProjectTools.ShowMatrixProjectResCheckTip(resState);
                return;
            }
            float needDownloadSize = LocalResourceManager.Instance.GetNeedDownloadSizeMB("GameMaker2D");
            if(Application.internetReachability != NetworkReachability.NotReachable
                && !Util.IsFloatEqual(needDownloadSize, 0))
            {
                CommonTools.ShowPopupDialog(string.Format("本次进入游戏需要更新 {0:N2}MB 资源，可能会产生费用，是否继续？", Mathf.Max(needDownloadSize, 0.01f)),
                    null,
                    new System.Collections.Generic.KeyValuePair<string, Action>("继续", ()=>{
                        InternalBeginPlay(content);
                    }),
                    new System.Collections.Generic.KeyValuePair<string, Action>("取消", ()=>{
                        LogHelper.Debug("Cancel BeginPlay");
                    })
                );
            }
            else
            {
                MatrixProjectTools.TryTipLoginBeforePlayProject(()=>InternalBeginPlay(content));
            }
        }

        private static void InternalBeginPlay(ProjectParams content)
        {
            if(content.Type == EProjectParamType.ProjectList)
            {
                InternalBeginPlayList(content);
            }
            else
            {
                InternalBeginPlaySingle(content);
            }
        }

        private static void InternalBeginPlaySingle(ProjectParams content)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>(), "作品加载中");
            content.Project.PrepareRes(()=>{
                content.Project.BeginPlay(true, ()=>{
                    MatrixProjectTools.OnProjectBeginPlaySuccess();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
                    SocialGUIManager.Instance.ChangeToGameMode();
                }, code=>{
//                    if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
//                    {
//                        CommonTools.ShowPopupDialog("作品已被删除，启动失败");
//                    }
//                    else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
//                    {
//                        CommonTools.ShowPopupDialog("启动过于频繁，启动失败");
//                    }
//                    else
//                    {
//                        if(Application.internetReachability == NetworkReachability.NotReachable)
//                        {
//                            CommonTools.ShowPopupDialog("启动失败，请检查网络环境");
//                        }
//                        else
//                        {
//                            CommonTools.ShowPopupDialog("启动失败，未知错误");
//                        }
//                    } 
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
                });
            }, ()=>{
                LogHelper.Error("Project OnPlayClick, Project GetRes Error");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
                CommonTools.ShowPopupDialog("作品加载失败，请检查网络");
            });
        }

        private static void InternalBeginPlayList(ProjectParams content)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>(), "作品加载中");
            ProjectLoadList pll = new ProjectLoadList();
            pll.Set(content.ProjectList, ()=>OnProjectResLoadSuccess(content), OnProjectResLoadFailed);
            pll.StartLoad();
        }

        private static void OnProjectResLoadSuccess(ProjectParams content)
        {
            content.Project.BeginPlay(false, ()=>{
                MatrixProjectTools.OnProjectBeginPlaySuccess();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
                GameManager.Instance.RequestPlay(content.ProjectList, content.Inx);
                SocialGUIManager.Instance.ChangeToGameMode();
            }, code=>{
//                if(code == EPlayProjectRetCode.PPRC_ProjectHasBeenDeleted)
//                {
//                    CommonTools.ShowPopupDialog("作品已被删除，启动失败");
//                }
//                else if(code == EPlayProjectRetCode.PPRC_FrequencyTooHigh)
//                {
//                    CommonTools.ShowPopupDialog("启动过于频繁，启动失败");
//                }
//                else
//                {
//                    if(Application.internetReachability == NetworkReachability.NotReachable)
//                    {
//                        CommonTools.ShowPopupDialog("启动失败，请检查网络环境");
//                    }
//                    else
//                    {
//                        CommonTools.ShowPopupDialog("启动失败，未知错误");
//                    }
//                } 
//                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
            });
        }

        private static void OnProjectResLoadFailed()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>());
            CommonTools.ShowPopupDialog("关卡资源加载失败，请检查网络环境");
        }
    }

    public class ProjectParams
    {
        public EProjectParamType Type;
        public Project Project;
        public List<Project> ProjectList;
        public int Inx;
    }

    public enum EProjectParamType
    {
        None,
        Project,
        ProjectList,
    }
}
