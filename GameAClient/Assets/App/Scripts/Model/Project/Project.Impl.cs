/********************************************************************
** Filename : Project
** Author : Dong
** Date : 2015/10/19 星期一 下午 7:18:18
** Summary : Project
***********************************************************************/

using System;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    [Poolable(MinPoolSize = ConstDefine.MaxLRUProjectCount / 10, PreferedPoolSize = ConstDefine.MaxLRUProjectCount / 5,
        MaxPoolSize = ConstDefine.MaxLRUProjectCount)]
	public partial class Project
    {
        #region 变量
        private long _guid;
        private int _downloadPrice;

        private bool _extendReady;
        private byte[] _deadPos;
        private long _commitToken;

        private WorldProjectRecentPlayedUserList _recentPlayedUserList;

        private GameTimer _projectInfoRequestTimer;

        private AdventureLevelRankList _adventureLevelRankList;
        #endregion 变量

        #region 属性

        public AdventureLevelRankList AdventureLevelRankList
        {
            get { return _adventureLevelRankList ?? (_adventureLevelRankList = new AdventureLevelRankList()); }
        }

        public int LikeCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.LikeCount;
                }
                return 0;
            }
        }

        public int FavoriteCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.FavoriteCount;
                }
                return 0;
            }
        }

        public int TotalCommentCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.CommentCount;
                }
                return 0;
            }
        }

        public long PlayCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.PlayCount;
                }
                return 0;
            }
        }

        public int CompleteCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.CompleteCount;
                }
                return 0;
            }
        }

        public int FailCount
        {
            get
            {
                if (_extendReady)
                {
                    return _extendData.FailCount;
                }
                return 0;
            }
        }

        public float CompleteRate
        {
            get
            {
                if (CompleteCount == 0)
                {
                    return 0;
                }
                else
                {
                    return 1f * CompleteCount / (CompleteCount + FailCount);
                }
            }
        }

        public byte[] DeadPos
        {
            get { return _deadPos; }

            set { _deadPos = value; }
        }

        public bool ExtendReady
        {
            get { return _extendReady; }
            set { _extendReady = value; }
        }

        public EProjectLikeState UserLike
        {
            get
            {
                if (_projectUserData != null)
                {
                    return _projectUserData.LikeState;
                }
                return EProjectLikeState.PLS_None;
            }
        }

        public bool UserFavorite
        {
            get
            {
                if (_projectUserData != null)
                {
                    return _projectUserData.Favorite;
                }
                return false;
            }
        }

        public int UserCompleteCount
        {
            get
            {
                if (_projectUserData != null)
                {
                    return _projectUserData.CompleteCount;
                }
                return 0;
            }
        }

        public long UserLastPlayTime
        {
            get
            {
                if (_projectUserData != null)
                {
                    return _projectUserData.LastPlayTime;
                }
                return 0;
            }
        }

        public int SectionId { get; set; }

        public int LevelId { get; set; }

        public EAdventureProjectType AdventureProjectType { get; set; }

        public WorldProjectRecentPlayedUserList RecentPlayedUserList
        {
            get
            {
                if (_recentPlayedUserList == null)
                {
                    _recentPlayedUserList = new WorldProjectRecentPlayedUserList();
                    _recentPlayedUserList.CS_ProjectId = _projectId;
                }
                return _recentPlayedUserList;
            }
        }

        #endregion 属性

        #region 方法

        public void RequestPlay(Action successCallback, Action<ENetResultCode> failedCallback)
        {
            RemoteCommands.PlayWorldProject(_projectId, ret => {
                if (ret.ResultCode == (int)EPlayWorldProjectCode.PWPC_Success)
                {
                    _commitToken = ret.Token;
                    _deadPos = ret.DeadPos;
                    if (_projectUserData != null)
                    {
                        _projectUserData.LastPlayTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                    }

                    PrepareRes (() => {
                        if (successCallback != null)
                        {
                            successCallback.Invoke();
                        }
                    }, () => {
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke(ENetResultCode.NR_None);
                        }
                    });
                }
                else
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                }
            }, code => {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(code);
                }
            });
        }

        public byte[] GetData()
        {
            string targetRes = ResPath;
            // 这个方法应该在prepare之后执行，所以这里不做异常检查了
            if (_projectStatus == EProjectStatus.PS_Reform && string.IsNullOrEmpty (ResPath)) {
                targetRes = AppData.Instance.AdventureData.ProjectList.SectionList [TargetSection - 1].NormalProjectList [TargetLevel - 1].ResPath;
            }
            if (_projectStatus == EProjectStatus.PS_Reform && null != _bytesData) {
                return _bytesData;
            }
            return LocalCacheManager.Instance.Load (LocalCacheManager.EType.File, targetRes);
        }

        public void Save(
            string name, 
            string summary,
            byte[] dataBytes,
            byte[] iconBytes,
            bool passFlag,
            bool success,
            int usedTime,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            byte[] recordBytes,
            int timeLimit,
            int winCondition,
            Action successCallback,
            Action<EProjectOperateResult> failedCallback)
        {
            if (ProjectStatus == EProjectStatus.PS_Public)
            {
                LogHelper.Error("Save Project is readonly");
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                name = "我的大作";
            }
            Name = name;
            Summary = summary;
            WinCondition = winCondition;

            WWWForm form = new WWWForm();
            if (dataBytes != null)
            {
                form.AddBinaryData("levelFile", dataBytes);
            }
            if (iconBytes != null)
            {
                form.AddBinaryData("iconFile", iconBytes);
            }
            if (recordBytes != null)
            {
                form.AddBinaryData("recordFile", recordBytes);
            }
            String oldIconPath = _iconPath;
            // 如果是在工坊界面修改关卡的信息，就不用更新附加参数
            if (null != dataBytes)
            {
                RefreshProjectUploadParam();
            }
            Msg_RecordUploadParam recordUploadParam = new Msg_RecordUploadParam()
            {
                Success = success,
                UsedTime = usedTime,
                Score = score,
                ScoreItemCount = scoreItemCount,
                KillMonsterCount = killMonsterCount,
                LeftTime = leftTime,
                LeftLife = leftLife,
                DeadPos = null
            };
            if (LocalDataState == ELocalDataState.LDS_UnCreated) {
                RemoteCommands.CreateProject (
                    Name,
                    Summary,
                    ProgramVersion,
                    ResourcesVersion,
                    passFlag,
                    recordUploadParam,
                    timeLimit,
                    winCondition,
                    GetMsgProjectUploadParam(),
                    msg => {
                        if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
//                            LocalCacheManager.Instance.Save(dataBytes, LocalCacheManager.EType.File, ResPath);
                            OnSyncFromParent(msg.ProjectData);
                            ImageResourceManager.Instance.SaveOrUpdateImageData(IconPath, iconBytes);
                            Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectCreated, this);
                            if (successCallback != null)
                            {
                                successCallback.Invoke();
                            }
                        } else {
                            LogHelper.Error("level create error, code: {0}", msg.ResultCode);
                            if (failedCallback != null)
                            {
                                failedCallback.Invoke((EProjectOperateResult) msg.ResultCode);
                            }
                        }
                    },
                    code => {
                        LogHelper.Error("level create error, code: {0}", code);
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke(EProjectOperateResult.POR_Error);
                        }
                    },
                    form
                );
            } else {
                RemoteCommands.UpdateProject (
                    ProjectId,
                    Name,
                    Summary,
                    ProgramVersion,
                    ResourcesVersion,
                    passFlag,
                    recordUploadParam,
                    timeLimit,
                    winCondition,
                    // 如果是在工坊界面修改关卡的信息，就不必传附加参数
                    null == dataBytes ? null : GetMsgProjectUploadParam(),
                    msg => {
                        if (msg.ResultCode == (int)EProjectOperateResult.POR_Success)
                        {
                            OnSyncFromParent(msg.ProjectData);
                            if (null != iconBytes && msg.ProjectData.IconPath != oldIconPath) {
                                ImageResourceManager.Instance.DeleteImageCache(oldIconPath);
                                ImageResourceManager.Instance.SaveOrUpdateImageData(msg.ProjectData.IconPath, iconBytes);
                            }
                            Messenger<Project>.Broadcast(EMessengerType.OnWorkShopProjectDataChanged, this);
                            if (successCallback != null)
                            {
                                successCallback.Invoke();
                            }
                        }
                        else
                        {
                            LogHelper.Error("level upload error, code: {0}", msg.ResultCode);
                            if (failedCallback != null)
                            {
                                failedCallback.Invoke((EProjectOperateResult) msg.ResultCode);
                            }
                        }
                    },
                    code => {
                        LogHelper.Error("level upload error, code: {0}", code);
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke(EProjectOperateResult.POR_Error);
                        }
                    },
                    form
                );
            }
        }

        public void Delete()
        {
            DeleteResCache();
            LocalCacheManager.Instance.DeleteObject(ECacheDataType.CDT_ProjectData, ProjectId);
        }

        public void DeleteResCache()
        {
            LocalCacheManager.Instance.Delete(LocalCacheManager.EType.File, ResPath);
            ImageResourceManager.Instance.DeleteImageCache(IconPath);
        }


        public void Publish(Action onSuccess, Action<EProjectOperateResult> onError)
        {
            var user = LocalUser.Instance.User;
            RemoteCommands.PublishWorldProject(
                _projectId,
                _name,
                _summary,
                _programVersion,
                _resourcesVersion,
                null,
                _timeLimit,
                _winCondition,
                null,
                ret =>
                {
                    if (ret.ResultCode == (int)EProjectOperateResult.POR_Success)
                    {
                        user.GetPublishedPrjectRequestTimer().Zero();
                        user.GetSavedPrjectRequestTimer().Zero();
                        //                    Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
                        if (onSuccess != null)
                        {
                            onSuccess.Invoke();
                        }
                    }
                else
                {
                    LogHelper.Error("level upload error, code: {0}", ret.ResultCode);
                    if (onError != null)
                    {
                        onError.Invoke((EProjectOperateResult)ret.ResultCode);
                    }
                }
                },
                code =>
                {
                    SoyHttpClient.ShowErrorTip(code);
                    if (onError != null)
                    {
                        onError.Invoke(EProjectOperateResult.POR_None);
                    }
                });
        }


        public void PrepareRes(Action successCallback, Action failedCallback = null)
        {
            string targetRes = ResPath;
            // 改造关卡特殊处理
            if (_projectStatus == EProjectStatus.PS_Reform) {
                if (_bytesData != null) {
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                    return;
                }
                if (string.IsNullOrEmpty (ResPath)) {
                    if ((TargetSection - 1) < AppData.Instance.AdventureData.ProjectList.SectionList.Count &&
                        (TargetLevel - 1) < AppData.Instance.AdventureData.ProjectList.SectionList [TargetSection - 1].NormalProjectList.Count &&
                        !string.IsNullOrEmpty (AppData.Instance.AdventureData.ProjectList.SectionList [TargetSection - 1].NormalProjectList [TargetLevel - 1].ResPath)) {
                        targetRes = AppData.Instance.AdventureData.ProjectList.SectionList [TargetSection - 1].NormalProjectList [TargetLevel - 1].ResPath;
                    } else {
                        if (string.IsNullOrEmpty (ResPath)) {
                            if (null != failedCallback) {
                                failedCallback.Invoke ();
                            }
                            return;
                        }
                    }
                }
                // else 正常往后执行，检查自己的respath
            } else {
                if (string.IsNullOrEmpty (ResPath)) {
                    if (null != failedCallback) {
                        failedCallback.Invoke ();
                    }
                    return;
                }
            }
            byte[] data = LocalCacheManager.Instance.Load(LocalCacheManager.EType.File, targetRes);
            if (data != null)
            {
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
                return;
            }
            SFile file = SFile.GetFileWithUrl(SoyPath.Instance.GetFileUrl(targetRes));
            //Debug.Log ("____________________download map file: " + targetRes + " success cb: " + _downloadResSucceedCB + " / successCallback: " + successCallback);
            file.DownloadAsync((f) =>
                {
                    LocalCacheManager.Instance.Save(f.FileBytes, LocalCacheManager.EType.File, targetRes);
                    //Debug.Log ("__________________________ call download success cb : " + _downloadResSucceedCB);
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }, sFile =>
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke();
                    }
                });
        }

        public void UpdateLike(EProjectLikeState likeState, Action successCallback = null, Action<ENetResultCode> failedCallback = null)
        {
            if (_projectUserData.LikeState == likeState)
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
                return;
            }
            RemoteCommands.UpdateWorldProjectLike(_projectId, likeState, ret =>
                {
                    if (ret.ResultCode != (int)EUpdateWorldProjectLikeCode.UWPLC_Success)
                    {
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke(ENetResultCode.NR_None);
                        }
                        return;
                    }
                    if (_projectUserData != null) {
                        _projectUserData.LikeState = likeState;
                    }
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }, code =>
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                });
        }

        public void AddShareCount(Action<bool> callback = null)
        {
//            Msg_CA_OnShareSuccess msg = new Msg_CA_OnShareSuccess();
//            msg.RefType = EAppContentItemType.ACIT_Project;
//            msg.RefId = _guid;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OnShareSuccessRet>(SoyHttpApiPath.OnShareSuccess, msg, ret =>
//            {
//                if (ret.OnShareSuccessRetCode != (int)EOnShareSuccessRetCode.OSSRC_Success)
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                else
//                {
//                    _shareCount = _shareCount + 1;
//                    if (callback != null)
//                    {
//                        callback.Invoke(true);
//                    }
//                }
//            }, (code, msgStr) =>
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
        }

        public void UpdateFavorite(bool favorite, Action successCallback = null, Action<ENetResultCode> failedCallback = null)
        {
            if (UserFavorite == favorite)
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
                return;
            }
            RemoteCommands.UpdateWorldProjectFavorite(_projectId, favorite, ret => {
                if (ret.ResultCode != (int)EUpdateWorldProjectFavoriteCode.UWPFC_Success)
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                    return;
                }
                if (_projectUserData != null) {
                    _projectUserData.Favorite = favorite;
                }
                if (_extendData != null)
                {
                    _extendData.FavoriteCount += favorite ? 1 : -1;
                }
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, code =>
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(code);
                }
            });
        }

		protected override void OnSyncPartial ()
		{
			base.OnSyncPartial ();
			if (_extendData != null)
            {
				OnSyncProjectExtendData(_extendData);
            }
            FireProjectDataChangeEvent();
		}

		public void OnSyncProjectExtendData(ProjectExtend msg)
        {
            _extendReady = true;
            _isValid = msg.IsValid;
        }

        public void CommitPlayResult(
            bool success,
            int usedTime,
            int score,
            int scoreItemCount,
            int killMonsterCount,
            int leftTime,
            int leftLife,
            byte [] recordBytes,
            byte [] deadPos, 
            Action successCallback, Action<ENetResultCode> failedCallback)
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => {
                if (_commitToken == 0)
                {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                    return;
                }
                WWWForm wwwForm = new WWWForm();
                if (recordBytes != null)
                {
                    wwwForm.AddBinaryData("recordFile", recordBytes);
                }
                Msg_RecordUploadParam recordUploadParam = new Msg_RecordUploadParam()
                {
                    Success = success,
                    UsedTime = usedTime,
                    Score = score,
                    ScoreItemCount = scoreItemCount,
                    KillMonsterCount = killMonsterCount,
                    LeftTime = leftTime,
                    LeftLife = leftLife,
                    DeadPos = deadPos
                };
                RemoteCommands.CommitWorldProjectResult(
                    _commitToken,
                    recordUploadParam,
                    ret => {
                    if (ret.ResultCode == (int)ECommitWorldProjectResultCode.CWPRC_Success)
                    {
                        if (successCallback != null)
                        {
                            successCallback.Invoke();
                        }
                    }
                    else
                    {
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke(ENetResultCode.NR_None);
                        }
                    }
                }, code => {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke(ENetResultCode.NR_None);
                    }
                }, wwwForm);
            }));
        }


        public void DownloadProject(Action successCallback, Action<EProjectOperateResult> failedCallback)
        {
            RemoteCommands.DownloadProject(_projectId, (ret) =>
            {
                if (ret.ResultCode == (int)EProjectOperateResult.POR_Success)
                {
                    _extendData.DownloadCount ++;
                    Project p = new Project();
                    p._projectStatus = EProjectStatus.PS_Private;
                    p.OnSync(ret.ProjectData);
                    LocalUser.Instance.PersonalProjectList.LocalAdd(p);
                    if (successCallback != null)
                    {
                        successCallback.Invoke();
                    }
                }
                else
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke((EProjectOperateResult)ret.ResultCode);
                    }
                }
            }, errCode =>
            {
                SoyHttpClient.ShowErrorTip(errCode);
                if (failedCallback != null)
                {
                    failedCallback.Invoke(EProjectOperateResult.POR_None);
                }
            });
        }

        public static Project CreateWorkShopProject()
        {
            Project p = new Project();
            p.ProjectId = LocalCacheManager.Instance.GetLocalGuid();
//            p.UserLegacy = LocalUser.Instance.UserLegacy;
            p.LocalDataState = ELocalDataState.LDS_UnCreated;
            p.Name = "";
            p.Summary = "";
            p.ResPath = "" + LocalCacheManager.Instance.GetLocalGuid() + ".soy";
            p.IconPath = "" + LocalCacheManager.Instance.GetLocalGuid() + ".jpg";
            p.ProjectStatus = EProjectStatus.PS_Private;
            p.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
            p._passFlag = false;
            return p;
        }

        public void FireProjectDataChangeEvent()
        {
            Messenger<long>.Broadcast(EMessengerType.OnProjectDataChanged, _projectId);
        }
        #endregion 方法

        #region 枚举

        public enum EResState
        {
            None,
            Undownload,
            Downloading,
            Ready,
        }

        public enum ELoadResult
        {
            None,
            Success,
            Timeout,
            NotExsit,
            Error,
        }
        #endregion 枚举

        #region 内部类
        #endregion
    }
}