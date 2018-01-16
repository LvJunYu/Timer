/********************************************************************
  ** Filename : EMessengerType.cs
  ** Author : quan
  ** Date : 3/29/2017 11:53 AM
  ** Summary : EMessengerType.cs
  ***********************************************************************/

namespace GameA
{
    public static partial class EMessengerType
    {
        public static readonly int OnAppDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnApplicationQuit = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnNewestProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserFavoriteProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserWorldProjectPlayHistoryListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorldBestProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorldFollowedUserProjectListChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnProjectDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectMyFavoriteChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectCommentChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectRecentCompleteChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectRecentPlayedChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectRecentRecordChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnProjectRecordRankChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserPublishedProjectChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserFavoriteChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserProjectPlayHistoryChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserFollowChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserFollowerChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserMatrixDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserInfoChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorkShopProjectCreated = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorkShopProjectDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorkShopProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorkShopDownloadListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnWorkShopProjectPublished = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnRequestStartGame = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnGameStartComplete = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnGameStop = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnGameStopComplete = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnPrepareNextProjectResFailed = SoyEngine.EMessengerType.NextId++;
        public static readonly int LoadEmptyScene = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnChangeToGameMode = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnChangeToAppMode = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnNotificationChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReceiveRemoteNotification = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnDailyMissionListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserMissionDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReceiveReward = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnMeNewMessageStateChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnEscapeClick = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnSendRecordCommentSuccess = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRecordCommentChanged = SoyEngine.EMessengerType.NextId++;

        /// <summary>
        /// 弹窗
        /// </summary>
        public static readonly int ShowDialog = SoyEngine.EMessengerType.NextId++;

        /// <summary>
        ///     朋友圈有新数据
        /// </summary>
        public static readonly int NewsFeedChanged = SoyEngine.EMessengerType.NextId++;

        /// <summary>
        ///     好友数据变化
        /// </summary>
        public static readonly int FriendsDataChanged = SoyEngine.EMessengerType.NextId++;

        /// <summary>
        ///     聊天内容变化
        /// </summary>
        public static readonly int ChatContentChanged = SoyEngine.EMessengerType.NextId++;

        /// <summary>
        ///     聊天会话变化
        /// </summary>
        public static readonly int ChatSessionChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int CheckAppVersionComplete = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnGamePlaySpeedChanged = SoyEngine.EMessengerType.NextId++;

        //public static readonly int OnAOISubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnAOIUnsubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnDynamicSubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnDynamicUnsubscribe = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRecordFullScreenStateChanged = SoyEngine.EMessengerType.NextId++;

        // 匹配改造相关
        public static readonly int OnReformProjectPublished = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnChallengeProjectSelected = SoyEngine.EMessengerType.NextId++;

        // 金钱、体力变化
        public static readonly int OnGoldChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnDiamondChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnEnergyChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnResourcesCheckFinish = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnResourcesCheckStart = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnResourcesUpdateProgressUpdate = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnVersionUpdateStateChange = SoyEngine.EMessengerType.NextId++;

        //PVP
        public static readonly int OnRoomInfoChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnRoomPlayerReadyChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRoomPlayerEnter = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRoomPlayerExit = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRoomWarnningHost = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnOpenBattle = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRoomListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnQueryRoomRet = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnJoinRoomFail = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnRoomProjectInfoFinish = SoyEngine.EMessengerType.NextId++;

        //拼图
        public static readonly int OnPuzzleCompound = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnPuzzleEquip = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnPuzzleFragChanged = SoyEngine.EMessengerType.NextId++;

        //武器升级后广播消息
        public static readonly int OnWeaponDataChange = SoyEngine.EMessengerType.NextId++;

        //设置变化
        public static readonly int OnGameSettingChanged = SoyEngine.EMessengerType.NextId++;

        //训练
        public static readonly int OnUpgradeTrainProperty = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnUpgradeTrainGrade = SoyEngine.EMessengerType.NextId++;

        //成就
        public static readonly int OnAchieve = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnAddAchievementCount = SoyEngine.EMessengerType.NextId++;

        //荣誉通报
        public static readonly int OnHonorReport = SoyEngine.EMessengerType.NextId++;

        //修改按键
        public static readonly int OnGetInputKeyCode = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnInputKeyCodeChanged = SoyEngine.EMessengerType.NextId++;

        //聊天
        public static readonly int OnSendText = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnSendVoice = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReceiveText = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReceiveVoice = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReceiveStatus = SoyEngine.EMessengerType.NextId++;

        //邮箱
        public static readonly int OnMailListChanged = SoyEngine.EMessengerType.NextId++;

        //好友
        public static readonly int OnRelationShipChanged = SoyEngine.EMessengerType.NextId++;

        //QQ
        public static readonly int OnQQRewardGetChangee = SoyEngine.EMessengerType.NextId++;

        //留言板
        public static readonly int OnPublishDockActiveChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnReplyUserMessage = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnReplyProjectComment = SoyEngine.EMessengerType.NextId++;

        //乱入
        public static readonly int OnShadowBattleStart = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnShadowBattleFriendHelp = SoyEngine.EMessengerType.NextId++;

        //新消息通知
        public static readonly int OnInfoNotificationChanged = SoyEngine.EMessengerType.NextId++;

        //绳子
        public static readonly int OnPlayerClimbRope = SoyEngine.EMessengerType.NextId++;
    }
}