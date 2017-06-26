  /********************************************************************
  ** Filename : EMessengerType.cs
  ** Author : quan
  ** Date : 3/29/2017 11:53 AM
  ** Summary : EMessengerType.cs
  ***********************************************************************/
using System;

namespace GameA
{
    public static partial class EMessengerType
    {
        public static readonly int OnAppDataChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnNewestProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserFavoriteProjectListChanged = SoyEngine.EMessengerType.NextId++;
        public static readonly int OnUserWorldProjectPlayHistoryListChanged = SoyEngine.EMessengerType.NextId++;

        public static readonly int OnProjectDataChanged = SoyEngine.EMessengerType.NextId++;
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

        public static readonly int CheckAppVersionComplete = SoyEngine.EMessengerType.NextId ++;

        public static int OnGamePlaySpeedChanged = SoyEngine.EMessengerType.NextId++;

        //public static readonly int OnAOISubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnAOIUnsubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnDynamicSubscribe = SoyEngine.EMessengerType.NextId++;
        //public static readonly int OnDynamicUnsubscribe = SoyEngine.EMessengerType.NextId++;
        public static int OnRecordFullScreenStateChanged = SoyEngine.EMessengerType.NextId++;

        // 匹配改造相关
        public static int OnReformProjectPublished = SoyEngine.EMessengerType.NextId++;
        public static int OnChallengeProjectSelected = SoyEngine.EMessengerType.NextId++;

        // 金钱、体力变化
        public static int OnGoldChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnDiamondChanged = SoyEngine.EMessengerType.NextId++;
        public static int OnEnergyChanged = SoyEngine.EMessengerType.NextId++;


        public static int OnResourcesCheckFinish = SoyEngine.EMessengerType.NextId++;
        public static int OnResourcesCheckStart = SoyEngine.EMessengerType.NextId++;
    }
}

