using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    /// <summary>
    /// 信息通知中心
    /// </summary>
    public class InfoNotificationManager : IDisposable
    {
        private static InfoNotificationManager _instance;
        private Dictionary<int, bool> _checkNew;

        public static InfoNotificationManager Instance
        {
            get { return _instance ?? (_instance = new InfoNotificationManager()); }
        }

        public InfoNotificationManager()
        {
            _checkNew = new Dictionary<int, bool>((int) EInfoNotificationType.Max);
            for (int i = 0; i < (int) EInfoNotificationType.Max; i++)
            {
                _checkNew.Add(i, false);
            }
        }

        public bool CheckNew(EInfoNotificationType eInfoNotificationType)
        {
            return _checkNew[(int) eInfoNotificationType];
        }

        public void Mark(EInfoNotificationType eInfoNotificationType, bool value)
        {
            if (_checkNew[(int) eInfoNotificationType] != value)
            {
                _checkNew[(int) eInfoNotificationType] = value;
                Messenger<EInfoNotificationType, bool>.Broadcast(EMessengerType.OnInfoNotificationChanged,
                    eInfoNotificationType, value);
            }
        }

        public void Dispose()
        {
            _instance = null;
        }
    }

    public enum EInfoNotificationType
    {
        NewFans, //新粉丝
        NewMessage, //新留言
        NewMail, //新邮件
        PublishedProjectScoreChanged, //关卡被评分
        PublishedProjectComment, //关卡被评论
        PublishedProjectPlayed, //关卡被挑战
        PublishedProjectDownload, //关卡被下载
        PublishedProjectShared, //关卡被分享
        PublishedProjectCollect, //关卡被收藏
        Max
    }
}