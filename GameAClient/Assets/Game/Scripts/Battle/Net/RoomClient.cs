﻿/********************************************************************
** Filename : RoomClient
** Author : Dong
** Date : 2017/6/20 星期二 上午 11:03:36
** Summary : RoomClient
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class RoomClient : NetClient
    {
        public RoomClient()
        {
            _handler = new RoomClientHandler();
            _serializer = new ClientProtoSerializer(typeof(ECRMsgType), ProtoSerializer.ProtoNameSpace, new GeneratedClientSerializer());
        }

        protected override void OnConnected()
        {
            LogHelper.Debug("RoomClient OnConnected");
            RoomManager.Instance.SendPlayerLoginRS();
        }

        protected override void OnClose()
        {
            LogHelper.Debug("RoomClient OnClose");
        }

        protected override void OnDisConnected()
        {
            LogHelper.Debug("RoomClient OnDisConnected");
        }
    }

    public class RoomClientHandler : Handler<object, NetLink>
    {
        protected override void InitHandler()
        {
            RegisterHandler<Msg_RC_LoginRet>(Msg_RC_LoginRet);
            RegisterHandler<Msg_RC_CreateRoomRet>(Msg_RC_CreateRoomRet);
            RegisterHandler<Msg_RC_JoinRoomRet>(Msg_RC_JoinRoomRet);
            RegisterHandler<Msg_RC_RoomInfo>(Msg_RC_RoomInfo);
            RegisterHandler<Msg_RC_RoomUserInfo>(Msg_RC_RoomUserInfo);
            RegisterHandler<Msg_RC_UserExitRoom>(Msg_RC_UserExitRoom);
            RegisterHandler<Msg_RC_UserReadyInfo>(Msg_RC_UserReadyInfo);
            RegisterHandler<Msg_RC_WarnningHost>(Msg_RC_WarnningHost);
            RegisterHandler<Msg_RC_RoomOpen>(Msg_RC_RoomOpen);
        }

        private void Msg_RC_RoomOpen(Msg_RC_RoomOpen msg, NetLink netLink)
        {
            RoomManager.Instance.OnRoomOpen();
        }

        private void Msg_RC_WarnningHost(Msg_RC_WarnningHost msg, NetLink netLink)
        {
            RoomManager.Instance.OnWarnningHost();
        }

        private void Msg_RC_UserReadyInfo(Msg_RC_UserReadyInfo msg, NetLink netLink)
        {
            RoomManager.Instance.OnUserReadyInfo(msg);
        }

        private void Msg_RC_UserExitRoom(Msg_RC_UserExitRoom msg, NetLink netLink)
        {
            RoomManager.Instance.OnUserExit(msg);
        }

        private void Msg_RC_RoomUserInfo(Msg_RC_RoomUserInfo msg, NetLink netLink)
        {
            RoomManager.Instance.OnNewUserJoinRoom(msg);
        }

        private void Msg_RC_RoomInfo(Msg_RC_RoomInfo msg, NetLink netLink)
        {
            RoomManager.Instance.OnRoomInfo(msg);
        }

        private void Msg_RC_JoinRoomRet(Msg_RC_JoinRoomRet msg, NetLink netLink)
        {
            LogHelper.Debug("Msg_RC_JoinRoomRet : {0}", msg);
            RoomManager.Instance.OnJoinRoomRet(msg);
        }

        private void Msg_RC_CreateRoomRet(Msg_RC_CreateRoomRet msg, NetLink netLink)
        {
            LogHelper.Debug("Msg_RC_CreateRoomRet : {0}", msg);
            RoomManager.Instance.OnCreateRoomRet(msg);
        }

        private void Msg_RC_LoginRet(Msg_RC_LoginRet msg, NetLink netLink)
        {
            if (msg.ResultCode == ELoginCode.ELC_Success)
            {
                LogHelper.Debug("Login RS Success");
            }
            else
            {
                LogHelper.Debug("Login RS Failed");
            }
        }
    }
}
