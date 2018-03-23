using System;
using SoyEngine;

namespace GameA
{
    public partial class NotificationPushStatistic
    {
        public void Request(Action sucuessAction, Action failAction = null)
        {
            var userId = LocalUser.Instance.UserGuid;
            if (userId == 0)
            {
                return;
            }

            Request(userId, sucuessAction, code =>
            {
                LogHelper.Error("NotificationPushStatistic Request fail, code = {0}", code);
                if (failAction != null)
                {
                    failAction.Invoke();
                }
            });
        }
    }
}