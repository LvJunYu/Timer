using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class RoomChatPreinstallList
    {
        public void RequestList(Action successAction, Action failAction = null)
        {
            Request(LocalUser.Instance.UserGuid, successAction, code =>
            {
                LogHelper.Error("RoomChatPreinstallList Request fail, code = {0}", code);
                if (failAction != null)
                {
                    failAction.Invoke();
                }
            });
        }
        
        public void CreateRoomChatPreinstall(string str, Action<RoomChatPreinstall> sucessAction, Action failAction = null)
        {
            RemoteCommands.CreateRoomChatPreinstall(str, msg =>
            {
                if (msg.ResultCode == (int) ERoomChatPreinstallOperateResult.RCPOR_Success)
                {
                    if (sucessAction != null)
                    {
                        sucessAction.Invoke(new RoomChatPreinstall(msg.Data));
                    }
                }
                else
                {
                    if (failAction != null)
                    {
                        failAction.Invoke();
                    }
                }
            }, code =>
            {
                LogHelper.Error("CreateRoomChatPreinstall fail, code = {0}", code);
                if (failAction != null)
                {
                    failAction.Invoke();
                }
            });
        }
    }
}