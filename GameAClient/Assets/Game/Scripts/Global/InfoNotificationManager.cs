using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    /// <summary>
    /// 信息通知中心
    /// </summary>
    public class InfoNotificationManager : IDisposable
    {
        private static InfoNotificationManager _instance;

        public static InfoNotificationManager Instance
        {
            get { return _instance ?? (_instance = new InfoNotificationManager()); }
        }

        private static int RequestInterval = 300;
        private float _lastRequestTime;
        private bool _inGame;
        private bool _hasInited;
        private NotificationPushStatistic _notificationPushStatistic = new NotificationPushStatistic();
        private NotificationPushData _notificationPushData = new NotificationPushData();
        private UICtrlInfoNotificationRaw _uiCtrlInfoNotificationRaw;

        public static int MaskAll
        {
            get
            {
                return 1 << (int) ENotificationDataType.NDT_Follower |
                       1 << (int) ENotificationDataType.NDT_ProjectComment |
                       1 << (int) ENotificationDataType.NDT_ProjectCommentReply |
                       1 << (int) ENotificationDataType.NDT_ProjectDownload |
                       1 << (int) ENotificationDataType.NDT_ProjectFavorite |
                       1 << (int) ENotificationDataType.NDT_UserMessageBoard |
                       1 << (int) ENotificationDataType.NDT_UserMessageBoardReply;
            }
        }

        public void RequestData()
        {
            _lastRequestTime = Time.time;
            _notificationPushStatistic.Request(() =>
            {
                //查看推送消息
                var pushData = _notificationPushStatistic.PushStatisticList;
                int mask = 0;
                for (int i = 0; i < pushData.Count; i++)
                {
                    if (CheckInfoValid(pushData[i]))
                    {
                        mask |= 1 << (int) pushData[i].Type;
                    }
                }

                if (mask > 0)
                {
                    RequestPushData(mask);
                }

                //查看通知消息
                var infoNotificaitonNew = false;
                var notificationData = _notificationPushStatistic.NotificationStatisticList;
                for (int i = 0; i < notificationData.Count; i++)
                {
                    if (CheckInfoValid(notificationData[i]))
                    {
                        infoNotificaitonNew = true;
                        break;
                    }
                }

                if (_uiCtrlInfoNotificationRaw == null)
                {
                    _uiCtrlInfoNotificationRaw = SocialGUIManager.Instance.GetUI<UICtrlInfoNotificationRaw>();
                }

                _uiCtrlInfoNotificationRaw.OnInfoNotificationHasNew(infoNotificaitonNew);
            });
        }

        private void RequestPushData(int mask)
        {
            _notificationPushData.Request(mask, () =>
            {
                var pushDatas = _notificationPushData.DataList;
                if (pushDatas.Count > 0)
                {
                    Messenger<List<NotificationPushDataItem>>.Broadcast(EMessengerType.OnInfoNotificationChanged,
                        pushDatas);
                }
            }, code => { LogHelper.Error("NotificationPushData Request fail, code = {0}", code); });
        }

        private bool CheckInfoValid(NotificationPushStatisticItem info)
        {
            return info.Type != ENotificationDataType.NDT_None && info.Count > 0;
        }

        public void Init()
        {
            if (GlobalVar.Instance.Env != EEnvironment.Production)
            {
                RequestInterval = 10;
            }
            Messenger.AddListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            Messenger.AddListener(EMessengerType.OnChangeToGameMode, OnChangeToGameMode);
            _hasInited = true;
        }

        public void Update()
        {
            if (!_hasInited || _inGame)
            {
                return;
            }

            if (Time.time - _lastRequestTime > RequestInterval)
            {
                RequestData();
            }
        }

        public void Dispose()
        {
            Messenger.RemoveListener(EMessengerType.OnChangeToAppMode, OnChangeToAppMode);
            Messenger.RemoveListener(EMessengerType.OnChangeToGameMode, OnChangeToGameMode);
            _instance = null;
        }

        public void MarkReadBatch(UICtrlInfoNotification.EMenu menu, Action successAction = null,
            Action failAction = null)
        {
            RemoteCommands.MarkNotificationHasReadBatch(GetMask(menu), msg =>
            {
                if (msg.ResultCode == (int) ENotificationOperationResultCode.NORC_Success)
                {
                    if (successAction != null)
                    {
                        successAction.Invoke();
                    }
                }
                else
                {
                    LogHelper.Error("MarkNotificationHasReadBatch fail, ResultCode = {0}", msg.ResultCode);
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                }
            }, code =>
            {
                LogHelper.Error("MarkNotificationHasReadBatch fail, code = {0}", code);
                if (failAction != null)
                {
                    failAction.Invoke();
                }
            });
        }

        private const string FollowerRawStr = "<color=#E37B17>{0}</color> 关注了你";
        private const string MessageBoardRawStr = "<color=#E37B17>{0}</color> 给你留言了";
        private const string MessageBoardReplyRawStr = "<color=#E37B17>{0}</color> 回复了你的留言";
        private const string ProjectCommentRawStr = "<color=#E37B17>{0}</color> 评论了你的关卡";
        private const string ProjectCommentReplyRawStr = "<color=#E37B17>{0}</color> 回复了你的关卡评论";
        private const string ProjectFavoriteRawStr = "<color=#E37B17>{0}</color> 收藏了你的关卡";
        private const string ProjectDownloadRawStr = "<color=#E37B17>{0}</color> 下载了你的关卡";

        public static string GetPushInfoFormat(ENotificationDataType pushDataType)
        {
            switch (pushDataType)
            {
                case ENotificationDataType.NDT_Follower:
                    return FollowerRawStr;
                case ENotificationDataType.NDT_UserMessageBoard:
                    return MessageBoardRawStr;
                case ENotificationDataType.NDT_UserMessageBoardReply:
                    return MessageBoardReplyRawStr;
                case ENotificationDataType.NDT_ProjectComment:
                    return ProjectCommentRawStr;
                case ENotificationDataType.NDT_ProjectCommentReply:
                    return ProjectCommentReplyRawStr;
                case ENotificationDataType.NDT_ProjectFavorite:
                    return ProjectFavoriteRawStr;
                case ENotificationDataType.NDT_ProjectDownload:
                    return ProjectDownloadRawStr;
                default:
                    LogHelper.Error("GetPushInfoFormat fail, dataType = {0}", pushDataType);
                    return string.Empty;
            }
        }

        public static long GetMask(UICtrlInfoNotification.EMenu menu)
        {
            if (menu == UICtrlInfoNotification.EMenu.Basic)
            {
                return 1 << (int) ENotificationDataType.NDT_Follower |
                       1 << (int) ENotificationDataType.NDT_UserMessageBoard |
                       1 << (int) ENotificationDataType.NDT_UserMessageBoardReply;
            }

            if (menu == UICtrlInfoNotification.EMenu.MyProject)
            {
                return 1 << (int) ENotificationDataType.NDT_ProjectDownload |
                       1 << (int) ENotificationDataType.NDT_ProjectFavorite |
                       1 << (int) ENotificationDataType.NDT_ProjectComment |
                       1 << (int) ENotificationDataType.NDT_ProjectCommentReply;
            }

            LogHelper.Error("GetMask fail, menu = {0}", menu);
            return 0;
        }

        public static bool IsStatisticsType(ENotificationDataType dataType)
        {
            return dataType == ENotificationDataType.NDT_Follower ||
                   dataType == ENotificationDataType.NDT_ProjectDownload ||
                   dataType == ENotificationDataType.NDT_ProjectFavorite ||
                   dataType == ENotificationDataType.NDT_ProjectComment ||
                   dataType == ENotificationDataType.NDT_UserMessageBoard;
        }

        private const string FollowFormat = "{0}个人关注了你";
        private const string UserMessageBoardFormat = "你有{0}条新留言";
        private const string UserMessageBoardReplyFormat = "<color=#F4A251>{0}</color>的回复：";
        private const string ProjectCommentFormat = "{0}个人评论了你的<color=#5E96B7>{1}</color>关卡";
        private const string ProjectCommentReplyFormat = "<color=#F4A251>{0}</color>在关卡<color=#5E96B7>{1}</color>的回复：";
        private const string ProjectFavoriteFormat = "{0}个人收藏了你的<color=#5E96B7>{1}</color>关卡";
        private const string ProjectDownloadFormat = "{0}个人下载了你的<color=#5E96B7>{1}</color>关卡";

        public static string GetContentTxt(NotificationDataItem data)
        {
            switch (data.Type)
            {
                case ENotificationDataType.NDT_Follower:
                    return string.Format(FollowFormat, data.Count);
                case ENotificationDataType.NDT_UserMessageBoard:
                    return string.Format(UserMessageBoardFormat, data.Count);
                case ENotificationDataType.NDT_UserMessageBoardReply:
                    return string.Format(UserMessageBoardReplyFormat, data.Sender.NickName);
                case ENotificationDataType.NDT_ProjectComment:
                    return string.Format(ProjectCommentFormat, data.Count, data.ProjectData.Name);
                case ENotificationDataType.NDT_ProjectCommentReply:
                    return string.Format(ProjectCommentReplyFormat, data.Sender.NickName, data.ProjectData.Name);
                case ENotificationDataType.NDT_ProjectFavorite:
                    return string.Format(ProjectFavoriteFormat, data.Count, data.ProjectData.Name);
                case ENotificationDataType.NDT_ProjectDownload:
                    return string.Format(ProjectDownloadFormat, data.Count, data.ProjectData.Name);
            }

            LogHelper.Error("GetContentTxt fail");
            return string.Empty;
        }

        private const string CheckStr = "查看";
        private const string CheckProjectStr = "查看关卡";
        private const string MessageBoardStr = "留言板";
        private const string FansStr = "粉丝列表";

        public static string GetBtnTxt(NotificationDataItem data)
        {
            switch (data.Type)
            {
                case ENotificationDataType.NDT_Follower:
                    return FansStr;
                case ENotificationDataType.NDT_UserMessageBoard:
                    return MessageBoardStr;
                case ENotificationDataType.NDT_UserMessageBoardReply:
                case ENotificationDataType.NDT_ProjectComment:
                case ENotificationDataType.NDT_ProjectCommentReply:
                    return CheckStr;
                case ENotificationDataType.NDT_ProjectFavorite:
                case ENotificationDataType.NDT_ProjectDownload:
                    return CheckProjectStr;
            }

            LogHelper.Error("GetBtnTxt fail");
            return string.Empty;
        }

        public static string GetDetailTxt(NotificationDataItem data)
        {
            switch (data.Type)
            {
                case ENotificationDataType.NDT_UserMessageBoardReply:
                    return data.UserMessageReply.Content;
                case ENotificationDataType.NDT_ProjectCommentReply:
                    return data.ProjectCommentReply.Content;
            }

            LogHelper.Error("GetDetailTxt fail");
            return string.Empty;
        }

        public static void OnBtnClick(NotificationDataItem data)
        {
            switch (data.Type)
            {
                case ENotificationDataType.NDT_Follower:
                    SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>(UICtrlSocialRelationship.EMenu.Fans);
                    break;
                case ENotificationDataType.NDT_UserMessageBoard:
                    SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(LocalUser.Instance.User)
                        .OpenMenu(UICtrlPersonalInformation.EMenu.MessageBoard);
                    break;
                case ENotificationDataType.NDT_UserMessageBoardReply:
                    SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(data.UserMessageReply.UserInfoDetail)
                        .OpenMenu(UICtrlPersonalInformation.EMenu.MessageBoard);
                    break;
                case ENotificationDataType.NDT_ProjectComment:
                case ENotificationDataType.NDT_ProjectCommentReply:
                    SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(data.ProjectData)
                        .OpenMenu(UICtrlProjectDetail.EMenu.Comment);
                    break;
                case ENotificationDataType.NDT_ProjectFavorite:
                    SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(data.ProjectData);
                    break;
                case ENotificationDataType.NDT_ProjectDownload:
                    SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(data.ProjectData);
                    break;
            }
        }

        public static void OnPushNotificationBtnClick(NotificationPushDataItem data)
        {
            switch (data.Type)
            {
                case ENotificationDataType.NDT_Follower:
                    SocialGUIManager.Instance.OpenUI<UICtrlSocialRelationship>(UICtrlSocialRelationship.EMenu.Fans);
                    break;
                case ENotificationDataType.NDT_UserMessageBoard:
                    SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(LocalUser.Instance.User)
                        .OpenMenu(UICtrlPersonalInformation.EMenu.MessageBoard);
                    break;
                case ENotificationDataType.NDT_UserMessageBoardReply:
                    UserManager.Instance.GetDataOnAsync(data.ContentId,
                        user =>
                        {
                            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(user)
                                .OpenMenu(UICtrlPersonalInformation.EMenu.MessageBoard);
                        });
                    break;
                case ENotificationDataType.NDT_ProjectComment:
                case ENotificationDataType.NDT_ProjectCommentReply:
                    ProjectManager.Instance.GetDataOnAsync(data.ContentId,
                        p =>
                        {
                            SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(p)
                                .OpenMenu(UICtrlProjectDetail.EMenu.Comment);
                        });
                    break;
                case ENotificationDataType.NDT_ProjectFavorite:
                case ENotificationDataType.NDT_ProjectDownload:
                    ProjectManager.Instance.GetDataOnAsync(data.ContentId,
                        p => { SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(p); });
                    break;
            }
        }

        private void OnChangeToGameMode()
        {
            _inGame = true;
        }

        private void OnChangeToAppMode()
        {
            _inGame = false;
        }
    }
}