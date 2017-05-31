/********************************************************************
** Filename : Project
** Author : Dong
** Date : 2015/10/19 星期一 下午 7:18:18
** Summary : Project
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;

namespace GameA
{
    [Poolable(MinPoolSize = ConstDefine.MaxLRUProjectCount / 10, PreferedPoolSize = ConstDefine.MaxLRUProjectCount / 5,
        MaxPoolSize = ConstDefine.MaxLRUProjectCount)]
	public partial class Project : SyncronisticData {
        #region 变量
        private bool _syncIgnoreMe = false;

        private long _guid;
        private User _user;
        private int _resourceVersion;
        private int _downloadPrice;

        private bool _extendReady = false;
        private int _likeCount;
        private int _favoriteCount;
        private int _downloadCount;
        private int _shareCount;
        private float _totalRate;
        private int _totalRateCount;
        private int _totalCommentCount;
        private long _totalClickCount;
        private int _completeCount;
        private int _failCount;
        private byte[] _deadPos;
        private long _commitToken;

//        private List<ProjectComment> _projectCommentList;
        private GameTimer _projectCommentListRequestTimer;

        private bool _userLike;
        private bool _userFavorite;
        private int _userCompleteCount;
        private long _userLastPlayTime;

        private List<PlayedProjectUser> _recentPlayedUserList;

        private List<RecordRankHolder> _projectPlayRecordList;
        private GameTimer _projectPlayRecordListRequestTimer;

        private List<Record> _projectRecentRecordList;
        private GameTimer _projectRecentRecordListRequestTimer;

        private GameTimer _projectInfoRequestTimer;

        /// <summary>
        /// 下载资源成功回调
        /// </summary>
        private Action _downloadResSucceedCB;
        /// <summary>
        /// 下载资源失败回调
        /// </summary>
        private Action _downloadResFailedCB;
        /// <summary>
        /// 是否正在下载资源
        /// </summary>
        private bool _isdownloadingRes;

        /// <summary>
        /// 冒险模式的第几章，如果不是冒险模式关卡，则为0
        /// </summary>
        private int _sectionId;
        /// <summary>
        /// 冒险模式的第几关，如果不是冒险模式关卡，则为0
        /// </summary>
        private int _levelId;
        #endregion 变量

        #region 属性

        public float PassRate {
            get {
                if (null != ExtendData && ExtendData.PlayCount > 0) {
                    return (float)ExtendData.CompleteCount / ExtendData.PlayCount;
                } else {
                    return 0f;
                }
            }
        }

        public User UserLegacy
        {
            get
            {
                return this._user;
            }
            set
            {
                _user = value;
            }
        }

        public int DownloadPrice
        {
            get { return _downloadPrice; }
            set { _downloadPrice = value; }
        }

        public int LikeCount
        {
            get
            {
                return _likeCount;
            }
        }

        public int FavoriteCount
        {
            get
            {
                return _favoriteCount;
            }
        }

        public int DownloadCount
        {
            get
            {
                return _downloadCount;
            }
        }

        public int ShareCount
        {
            get
            {
                return _shareCount;
            }
        }

        public float TotalRate
        {
            get
            {
                return this._totalRate;
            }
            set
            {
                _totalRate = value;
            }
        }

        public int TotalRateCount
        {
            get
            {
                return this._totalRateCount;
            }
            set
            {
                _totalRateCount = value;
            }
        }

        public int TotalCommentCount
        {
            get
            {
                return this._totalCommentCount;
            }
            set
            {
                _totalCommentCount = value;
            }
        }

        public long TotalClickCount
        {
            get
            {
                return this._totalClickCount;
            }
            set
            {
                _totalClickCount = value;
            }
        }

        public int CompleteCount
        {
            get
            {
                return _completeCount;
            }

            set
            {
                _completeCount = value;
            }
        }

        public int FailCount
        {
            get
            {
                return _failCount;
            }

            set
            {
                _failCount = value;
            }
        }

        public float CompleteRate
        {
            get
            {
                if (_completeCount == 0)
                {
                    return 0;
                }
                else
                {
                    return 1f * _completeCount / (_completeCount + _failCount);
                }
            }
        }

        public byte[] DeadPos
        {
            get
            {
                return _deadPos;
            }

            set
            {
                _deadPos = value;
            }
        }

        public bool ExtendReady
        {
            get { return _extendReady; }
            set { _extendReady = value; }
        }

        public List<PlayedProjectUser> RecentPlayedUserList
        {
            get
            {
                return _recentPlayedUserList;
            }

            set
            {
                _recentPlayedUserList = value;
            }
        }

        public bool UserLike
        {
            get
            {
                return _userLike;
            }
        }

        public bool UserFavorite
        {
            get
            {
                return this._userFavorite;
            }
            set
            {
                _userFavorite = value;
            }
        }

        public int UserCompleteCount
        {
            get { return _userCompleteCount; }
        }

        public long UserLastPlayTime
        {
            get { return _userLastPlayTime; }
        }

        public GameTimer ProjectCommentListRequestTimer
        {
            get
            {
                if (_projectCommentListRequestTimer == null)
                {
                    _projectCommentListRequestTimer = new GameTimer();
                    _projectCommentListRequestTimer.Zero();
                }
                return _projectCommentListRequestTimer;
            }
        }

        public GameTimer ProjectIntoRequestTimer
        {
            get
            {
                if (_projectInfoRequestTimer == null)
                {
                    _projectInfoRequestTimer = new GameTimer();
                    _projectInfoRequestTimer.Zero();
                }
                return _projectInfoRequestTimer;
            }
        }

        public List<RecordRankHolder> ProjectPlayRecordList
        {
            get
            {
                return _projectPlayRecordList;
            }
        }

        public GameTimer ProjectPlayRecordListRequestTimer
        {
            get
            {
                if (_projectPlayRecordListRequestTimer == null)
                {
                    _projectPlayRecordListRequestTimer = new GameTimer();
                    _projectPlayRecordListRequestTimer.Zero();
                }
                return _projectPlayRecordListRequestTimer;
            }
        }

        public List<Record> ProjectRecentRecordList
        {
            get
            {
                return _projectRecentRecordList;
            }
        }

        public GameTimer ProjectRecentRecordListRequestTimer
        {
            get
            {
                if (_projectRecentRecordListRequestTimer == null)
                {
                    _projectRecentRecordListRequestTimer = new GameTimer();
                    _projectRecentRecordListRequestTimer.Zero();
                }
                return _projectRecentRecordListRequestTimer;
            }
        }

        public int SectionId {
            get {
                return _sectionId;
            }
            set {
                _sectionId = value;
            }
        }

        public int LevelId {
            get {
                return _levelId;
            }
            set {
                _levelId = value;
            }
        }
        #endregion 属性

        #region 方法

        public void BeginEdit()
        {
            GameManager.Instance.RequestEdit(this);
        }

		public void BeginPlay(bool startGame, Action successCallback, Action<int> failedCallback) { }
//        public void BeginPlay(bool startGame, Action successCallback, Action<EPlayProjectRetCode> failedCallback)
//        {
//            if (GameManager.Instance.GameMode != EGameMode.Normal)
//            {
//                if (startGame)
//                {
//                    GameManager.Instance.RequestPlay(this);
//                }
//                if (successCallback != null)
//                {
//                    successCallback.Invoke();
//                }
//            }
//            else
//            {
//                Msg_CA_PlayProject msg = new Msg_CA_PlayProject();
//                msg.ProjectGuid = _guid;
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_PlayProjectRet>(SoyHttpApiPath.ClickPlayProject, msg, ret =>
//                {
//                    if (ret.ResultCode == EPlayProjectRetCode.PPRC_Success)
//                    {
//                        _commitToken = ret.Token;
//                        _deadPos = ret.DeadPos;
//                        if (startGame)
//                        {
//                            GameManager.Instance.RequestPlay(this);
//                        }
//                        _userLastPlayTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
//                        Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
//                        if (successCallback != null)
//                        {
//                            successCallback.Invoke();
//                        }
//                    }
//                    else
//                    {
//                        if (failedCallback != null)
//                        {
//                            failedCallback.Invoke(ret.ResultCode);
//                        }
//                    }
//                }, (errCode, errMsg) =>
//                {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke(EPlayProjectRetCode.PPRC_Error);
//                    }
//                });
//            }
//        }

        public void BeginPlayRecord(Record record)
        {
            GameManager.Instance.RequestPlayRecord(this, record);
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
            int downloadPrice,
            bool passFlag,
            float recordUsedTime, 
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
                name = string.Format("我的匠游大作");
            }
            Name = name;
            Summary = summary;
            WinCondition = winCondition;

            WWWForm form = new WWWForm();
            form.AddBinaryData("levelFile", dataBytes);
            form.AddBinaryData("iconFile", iconBytes);
            if (recordBytes != null)
            {
                form.AddBinaryData("recordFile", recordBytes);
            }
            String oldIconPath = _iconPath;
            if (LocalDataState == ELocalDataState.LDS_UnCreated) {
                RemoteCommands.CreateProject (
                    Name,
                    Summary,
                    ProgramVersion,
                    ResourcesVersion,
                    passFlag,
                    recordUsedTime,
                    timeLimit,
                    winCondition,
                    msg => {
                        if (msg.ResultCode == (int)EProjectOperateResult.POR_Success) {
//                            LocalCacheManager.Instance.Save(dataBytes, LocalCacheManager.EType.File, ResPath);
                            ImageResourceManager.Instance.SaveOrUpdateImageData(IconPath, iconBytes);
                            LocalUser.Instance.User.GetSavedPrjectRequestTimer().Zero();
                            if (successCallback != null)
                            {
                                successCallback.Invoke();
                            }
                        } else {
                            if (failedCallback != null)
                            {
                                failedCallback.Invoke(EProjectOperateResult.POR_Error);
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
                    recordUsedTime,
                    timeLimit,
                    winCondition,
                    msg => {
                        OnSyncFromParent(msg.ProjectData);
                        if (null != iconBytes && msg.ProjectData.IconPath != oldIconPath) {
                            ImageResourceManager.Instance.DeleteImageCache(oldIconPath);
                            ImageResourceManager.Instance.SaveOrUpdateImageData(msg.ProjectData.IconPath, iconBytes);
                        }
                        LocalUser.Instance.User.GetSavedPrjectRequestTimer().Zero();
                        if (successCallback != null)
                        {
                            successCallback.Invoke();
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
            if (_isdownloadingRes) {
                _downloadResSucceedCB -= successCallback;
                _downloadResSucceedCB += successCallback;
                _downloadResFailedCB -= failedCallback;
                _downloadResFailedCB += failedCallback;
                return;
            }
            _isdownloadingRes = true;
            SFile file = SFile.GetFileWithUrl(SoyPath.Instance.GetFileUrl(targetRes));
            Debug.Log ("____________________download map file: " + targetRes);
            file.DownloadAsync((f) =>
                {
                    _isdownloadingRes = false;
                    LocalCacheManager.Instance.Save(f.FileBytes, LocalCacheManager.EType.File, targetRes);
                    if (_downloadResSucceedCB != null)
                    {
                        _downloadResSucceedCB.Invoke();
                    }
                }, sFile =>
                {
                    _isdownloadingRes = false;
                    Debug.Log("__________________________" + SoyPath.Instance.GetFileUrl(targetRes));
                    if (_downloadResFailedCB != null)
                    {
                        _downloadResFailedCB.Invoke();
                    }
                });
        }

        public void UpdateLike(bool likeFlag, Action<bool> callback = null)
        {
            if (_userLike == likeFlag)
            {
                if (callback != null)
                {
                    callback.Invoke(false);
                }
                return;
            }
//            Msg_CS_CMD_UpdateProjectLike msg = new Msg_CS_CMD_UpdateProjectLike();
//            msg.ProjectGuid = _guid;
//            msg.LikeFlag = likeFlag;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateProjectRet>(SoyHttpApiPath.LikeProject, msg, ret =>
//                {
//                    Project p;
//                    if (ret.Result != (int)EProjectOperateResult.POR_Success)
//                    {
//                        if (ret.ProjectData != null)
//                        {
//                            p = ProjectManager.Instance.OnSyncProject(ret.ProjectData);
//                            if (p != this)
//                            {
//                                OnSyncProject(ret.ProjectData);
//                            }
//                        }
//                        if (callback != null)
//                        {
//                            callback.Invoke(false);
//                        }
//                        return;
//                    }
//                    p = ProjectManager.Instance.OnSyncProject(ret.ProjectData);
//                    if (p != this)
//                    {
//                        OnSyncProject(ret.ProjectData);
//                    }
//                    if (callback != null)
//                    {
//                        callback.Invoke(true);
//                    }
//                }, (code, msgStr) =>
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                });
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

        public void UpdateFavorite(bool favorite, Action<bool> callback = null)
        {
//            if (_userFavorite == favorite)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            Msg_CS_CMD_UpdateProjectFavorite msg = new Msg_CS_CMD_UpdateProjectFavorite();
//            msg.ProjectGuid = _guid;
//            msg.Favorite = favorite;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateProjectRet>(SoyHttpApiPath.UpdateProjectFavorite, msg, ret =>
//                {
//                    Project p;
//                    if (ret.Result != (int)EProjectOperateResult.POR_Success)
//                    {
//                        if (ret.ProjectData != null)
//                        {
//                            p = ProjectManager.Instance.OnSyncProject(ret.ProjectData);
//                            if (p != this)
//                            {
//                                OnSyncProject(ret.ProjectData);
//                            }
//                        }
//                        if (callback != null)
//                        {
//                            callback.Invoke(false);
//                        }
//                        return;
//                    }
//                    p = ProjectManager.Instance.OnSyncProject(ret.ProjectData);
//                    if (p != this)
//                    {
//                        OnSyncProject(ret.ProjectData);
//                    }
//                    if (callback != null)
//                    {
//                        callback.Invoke(true);
//                    }
//                }, (code, msgStr) =>
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                });
        }

//        public void SendComment(string comment, Action<bool> callback, User replyUser = null)
//        {
//            if (!_isValid)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            if (string.IsNullOrEmpty(comment) || comment.Length > SoyConstDefine.MaxProjectCommentLength)
//            {
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//                return;
//            }
//            Msg_CA_PostProjectComment msg = new Msg_CA_PostProjectComment();
//            msg.ProjectGuid = _guid;
//            msg.Comment = comment;
//            if (replyUser != null)
//            {
//                msg.TargetUserGuid = replyUser.UserGuid;
//            }
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_PostProjectComment>(SoyHttpApiPath.CommentProject, msg, ret =>
//            {
//                if (ret.ResultCode == ECommonResultCode.CRC_Error)
//                {
//                    if (callback != null)
//                    {
//                        callback.Invoke(false);
//                    }
//                    return;
//                }
//                _projectCommentListRequestTimer.Zero();
//                if (_projectCommentList == null)
//                {
//                    _projectCommentList = new List<ProjectComment>();
//                    LogHelper.Warning("SendComment warning, commentList not init");
//                }
//                _projectCommentList.Add(new ProjectComment(ret.ProjectComment));
//                _projectCommentList.Sort((pc1, pc2) => -pc1.CreateTime.CompareTo(pc2.CreateTime));
//                Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
//                if (callback != null)
//                {
//                    callback.Invoke(true);
//                }
//            }, (code, msgStr) =>
//            {
//
//                if (callback != null)
//                {
//                    callback.Invoke(false);
//                }
//            });
//        }

		protected override void OnSyncPartial ()
		{
			base.OnSyncPartial ();
			if (_syncIgnoreMe)
            {
                return;
            }
			if (_extendData != null)
            {
				OnSyncProjectExtendData(_extendData);
            }
			if (_projectUserData != null)
            {
				OnSyncProjectUserData(_projectUserData);
            }

            // 
//            _bytesData = null;
		}

		public void OnSyncProjectExtendData(ProjectExtend msg)
        {
            _extendReady = true;
            _isValid = msg.IsValid;
            _totalCommentCount = msg.CommentCount;
            _totalClickCount = msg.PlayCount;
            _completeCount = msg.CompleteCount;
            _failCount = msg.FailCount;
            _likeCount = msg.LikeCount;
            _favoriteCount = msg.FavoriteCount;
            _downloadCount = msg.DownloadCount;
            _shareCount = msg.ShareCount;
//            if (msg.CommentList != null)
//            {
//                OnSyncProjectCommentList(msg.CommentList);
//            }
        }

//        public void OnSyncRecentPlayedUserDataList(Msg_AC_ProjectRecentPlayedUserList msg)
//        {
//            if (_recentPlayedUserList == null)
//            {
//                _recentPlayedUserList = new List<PlayedProjectUser>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnProjectRecentPlayedChanged);
//                }
//            }
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _recentPlayedUserList.Clear();
//            }
//            msg.DataList.ForEach(msgItem =>
//                                 _recentPlayedUserList.Add(new PlayedProjectUser(msgItem)));
//            _recentPlayedUserList.Sort((p1, p2) =>
//                                       -p1.LastPlayTime.CompareTo(p2.LastPlayTime));
//            MessengerAsync.Broadcast(EMessengerType.OnProjectRecentPlayedChanged);
//        }

//        public void OnSyncProjectCommentList(Msg_AC_ProjectCommentList msg)
//        {
//            if (_projectCommentList == null)
//            {
//                _projectCommentList = new List<ProjectComment>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnProjectCommentChanged);
//                }
//            }
//            _totalCommentCount = msg.TotalCount;
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _projectCommentList.Clear();
//            }
//            msg.DataList.ForEach(msgItem =>
//                _projectCommentList.Add(new ProjectComment(msgItem)));
//            _projectCommentList.Sort((p1, p2) =>
//                                     -p1.CreateTime.CompareTo(p2.CreateTime));
//            MessengerAsync.Broadcast(EMessengerType.OnProjectCommentChanged);
//        }

		public void OnSyncProjectUserData(ProjectUserData msg)
        {
//            _userRate = (byte)msg.Rate;
            _userFavorite = msg.Favorite;
            _userLastPlayTime = msg.LastPlayTime;
            _userCompleteCount = msg.CompleteCount;
//            _userLike = msg.LikeFlag;
        }

        public void ClearProjectUserData()
        {
            _userLike = false;
//            _userRate = 0;
            _userFavorite = false;
            _userLastPlayTime = 0;
            _userCompleteCount = 0;
        }

//        public void CommitPlayResult(bool success, float usedTime, byte[] playRecord, byte[] deadPos, Action<int, bool> successCallback, Action<EPlayProjectResultRetCode> failedCallback)
//        {
//            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
//            {
//                WWWForm wwwForm = null;
//                Msg_CA_PlayProjectResult msg = new Msg_CA_PlayProjectResult();
//                msg.ProjectId = _guid;
//                msg.Token = _commitToken;
//                msg.Success = success;
//                msg.DeadPos = deadPos;
//                msg.UsedTime = usedTime;
//                wwwForm = new WWWForm();
//                wwwForm.AddBinaryData("recordFile", playRecord);
//                NetworkManager.AppHttpClient.SendWithCb<Msg_AC_PlayProjectResultRet>(SoyHttpApiPath.CommitPlayProjectResult, msg, ret =>
//                {
//                    if (ret.ResultCode == EPlayProjectResultRetCode.PPRRC_Success)
//                    {
//                        Messenger<Msg_AC_Reward>.Broadcast(EMessengerType.OnReceiveReward, ret.Reward);
//                        if (successCallback != null)
//                        {
//                            successCallback.Invoke(ret.Rank, ret.NewRecord);
//                        }
//                    }
//                    else
//                    {
//                        if (failedCallback != null)
//                        {
//                            failedCallback.Invoke(ret.ResultCode);
//                        }
//                    }
//                }, (failCode, failMsg) =>
//                {
//                    SoyHttpClient.ShowErrorTip(failCode);
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke(EPlayProjectResultRetCode.PPRRC_None);
//                    }
//                }, wwwForm);
//            }));
//        }


        public void DownloadProject(Action successCallback, Action<EProjectOperateResult> failedCallback)
        {
//            Msg_CA_RequestDownloadProject msg = new Msg_CA_RequestDownloadProject();
//            msg.ProjectId = _guid;
//            User user = LocalUser.Instance.User;
//            NetworkManager.AppHttpClient.SendWithCb<Msg_AC_OperateProjectRet>(SoyHttpApiPath.DownloadProject, msg, (ret) =>
//            {
//                if (ret.Result == (int)EProjectOperateResult.POR_Success)
//                {
//                    _downloadCount = _downloadCount + 1;
//                    Project p = new Project();
//                    p._projectStatus = EProjectStatus.PS_Private;
//                    user.OnProjectCreated(ret.ProjectData, p);
//                    user.GetSavedPrjectRequestTimer().Zero();
//                    if (_user.UserGuid != user.UserGuid)
//                    {
//                        user.OnGoldCoinChange(-_downloadPrice);
//                    }
//                    user.UserInfoRequestGameTimer.Zero();
//                    if (successCallback != null)
//                    {
//                        successCallback.Invoke();
//                    }
//                }
//                else
//                {
//                    if (failedCallback != null)
//                    {
//                        failedCallback.Invoke((EProjectOperateResult)ret.Result);
//                    }
//                }
//            }, (errCode, errMsg) =>
//            {
//                SoyHttpClient.ShowErrorTip(errCode);
//                if (failedCallback != null)
//                {
//                    failedCallback.Invoke(EProjectOperateResult.POR_None);
//                }
//            });
        }


//        public void OnSyncProjectPlayRecordList(Msg_AC_ProjectPlayRecordList msg)
//        {
//            if (_projectPlayRecordList == null)
//            {
//                _projectPlayRecordList = new List<RecordRankHolder>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    MessengerAsync.Broadcast(EMessengerType.OnProjectPlayRecordChanged);
//                }
//            }
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _projectPlayRecordList.Clear();
//            }
//            int rank = 0;
//            msg.DataList.ForEach(msgItem =>
//                                 _projectPlayRecordList.Add(new RecordRankHolder(msgItem, this, rank++)));
//            MessengerAsync.Broadcast(EMessengerType.OnProjectPlayRecordChanged);
//        }

//        public void OnSyncProjectRecentRecordList(Msg_AC_ProjectPlayRecordList msg)
//        {
//            if (_projectRecentRecordList == null)
//            {
//                _projectRecentRecordList = new List<Record>();
//                if (msg.Result == ECachedDataState.CDS_None
//                   || msg.Result == ECachedDataState.CDS_Uptodate)
//                {
//                    //MessengerAsync.Broadcast(EMessengerType.OnProjectPlayRecordChanged);
//                }
//            }
//            if (msg.Result == ECachedDataState.CDS_None
//                || msg.Result == ECachedDataState.CDS_Uptodate)
//            {
//                return;
//            }
//            if (msg.Result == ECachedDataState.CDS_Recreate)
//            {
//                _projectRecentRecordList.Clear();
//            }
//            msg.DataList.ForEach(msgItem =>
//                                 _projectRecentRecordList.Add(new Record(msgItem, this, false)));
//            //MessengerAsync.Broadcast(EMessengerType.OnProjectPlayRecordChanged);
//        }


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
            p.DownloadPrice = 0;
            p._passFlag = false;
            return p;
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
        public class CompleteProjectUser
        {
            private User _user;
            private long _completeTime;
            public User User
            {
                get
                {
                    return _user;
                }

                set
                {
                    _user = value;
                }
            }

            public long CompleteTime
            {
                get
                {
                    return _completeTime;
                }

                set
                {
                    _completeTime = value;
                }
            }

//            public CompleteProjectUser(Msg_CompleteProjectUserData msg)
//            {
//                Set(msg);
//            }
//
//            public void Set(Msg_CompleteProjectUserData msg)
//            {
//                _user = UserManager.Instance.OnSyncUserData(msg.UserData);
//                _completeTime = msg.CompleteTime;
//            }
        }

        public class PlayedProjectUser
        {
            private User _user;
            private long _lastPlayTime;
            public User User
            {
                get
                {
                    return _user;
                }

                set
                {
                    _user = value;
                }
            }

            public long LastPlayTime
            {
                get
                {
                    return _lastPlayTime;
                }

                set
                {
                    _lastPlayTime = value;
                }
            }

//            public PlayedProjectUser(Msg_ProjectRecentPlayedUserData msg)
//            {
//                Set(msg);
//            }

//            public void Set(Msg_ProjectRecentPlayedUserData msg)
//            {
//                _user = UserManager.Instance.OnSyncUserData(msg.UserData);
//                _lastPlayTime = msg.LastPlayTime;
//            }
        }

        #endregion
    }
}