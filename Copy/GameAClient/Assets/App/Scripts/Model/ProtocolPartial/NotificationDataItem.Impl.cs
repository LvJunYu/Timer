using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class NotificationDataItem
    {
        public void MarkRead(Action succussAction = null, Action failAction = null)
        {
            RemoteCommands.MarkNotificationHasRead(_type, _id, msg =>
            {
                if (msg.ResultCode == (int) ENotificationOperationResultCode.NORC_Success)
                {
                    if (succussAction != null)
                    {
                        succussAction.Invoke();
                    }
                }
                else
                {
                    LogHelper.Error("MarkNotificationHasRead fail, ResultCode = {0}", msg.ResultCode);
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                }
            }, code =>
            {
                LogHelper.Error("MarkNotificationHasRead fail, code = {0}", code);
                if (failAction != null)
                {
                    failAction.Invoke();
                }
            });
        }
    }
}