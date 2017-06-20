/********************************************************************
** Filename : EMsgType
** Author : Dong
** Date : 2015/7/5 星期日 下午 10:27:35
** Summary : EMsgType
***********************************************************************/

using System;

namespace SoyEngine
{
    public enum ECSMsgType
    {
        None,
        Msg_CS_Ping,
        Msg_SC_Pong,
        Msg_CR_Login,
        Msg_RC_LoginRet,
        Msg_CR_Logout,
        Max,
    }
}