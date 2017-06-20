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
        Msg_CL_Login,
        Msg_LC_LoginRet,
        Msg_CL_Register,
        Msg_LC_RegisterResult,

        Msg_AC_CommonResult = 100,
        Msg_CA_RequestAppData,
        Msg_AC_AppData,
        Msg_CA_Login,
        Msg_AC_LoginRet,
        Msg_CA_LoginByToken,
        Msg_AC_LoginByTokenRet,
        Msg_CA_Logout,
        Msg_CA_RequestSMSCode,
        Msg_CA_Register,
        Msg_AC_RegisterRet,
        Msg_CA_ChangePassword,
        Msg_CA_UpdateUserInfo,
        Msg_AC_UpdateUserInfoRet,
        Msg_CA_RequestUserInfo,
        Msg_AC_UserInfoList,

        Msg_CA_RequestAllMatrixData,
        Msg_AC_MatrixData,
        Msg_AC_AllMatrixs,
        Msg_CA_RequestMySavedProjectList,
        Msg_AC_ProjectList,
        Msg_AC_ProjectData,
        Msg_CA_RequestNewestProject,
        Msg_CA_RequestPublishedProject,
        Msg_AC_RecommendProjects,
        Msg_CA_CreateProject,
        Msg_CA_PublishProject,
        Msg_AC_OperateProjectRet,
        Msg_CA_UpdateProject,
        Msg_CA_DeleteMySavedProject,
        Msg_CA_UnpublishProject,

        Msg_CA_AddFriend,
        Msg_CA_DeleteFriend,
        Msg_AC_AddFriendRet,
        Msg_AC_DeleteFriendRet,
        Msg_CA_AcceptToBeFriend,
        Msg_AC_RelationDatasRet,
        Msg_CA_Chat,
        Msg_AC_ChatAck,
        Msg_AC_ChatRet,
        Msg_CA_ChatRetAck,

        Max,
    }
}