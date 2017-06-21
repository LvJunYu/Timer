/********************************************************************
** Filename : RoomManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:23:13
** Summary : RoomManager
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class RoomManager
    {
        public static RoomManager _instance;
        private bool _run;

        private RoomClient _roomClient = new RoomClient();

        private Room _room;
        private Msg_CR_CreateRoom _msgCreateRoom = new Msg_CR_CreateRoom();

        public static RoomManager Instance
        {
            get { return _instance ?? (_instance = new RoomManager()); }
        }

        public Room Room
        {
            get { return _room; }
        }

        public static RoomClient RoomClient
        {
            get { return Instance._roomClient; }
        }

        public bool Init()
        {
            _run = false;
            _room = PoolFactory<Room>.Get();
            ConnectRS("localhost", 6000);
            _run = true;
            return true;
        }

        public void ConnectRS(string ip, ushort port)
        {
            LogHelper.Debug("StartConnectRS: {0}, {1}", ip, port);
            _roomClient.Connect(ip, port);
        }

        public void Update()
        {
            if (!_run)
            {
                return;
            }
            _roomClient.Update();
        }

        private void SendToServer(object msg)
        {
            if (_roomClient != null && _roomClient.IsConnnected())
            {
                _roomClient.Send(msg);
            }
        }

        #region Send

        /// <summary>
        /// 请求登陆服务器
        /// </summary>
        public void SendPlayerLoginRS()
        {
            var login = new Msg_CR_Login();
            login.ClientVersion = GlobalVar.Instance.AppVersion;
            login.UserId = LocalUser.Instance.UserGuid;
            SendToServer(login);
            LogHelper.Debug("SendPlayerLoginRS:{0}", login.ToString());
        }

        public void SendRequestCreateRoom(EBattleType eBattleType, long projectGuid)
        {
            if (_room == null)
            {
                return;
            }
            if (_room.ERoomState == ERoomState.RequestCreate)
            {
                LogHelper.Warning("SendRequestCreateRoom Repeated");
                return;
            }
            _msgCreateRoom.EBattleType = eBattleType;
            _msgCreateRoom.ProjectGuid = projectGuid;
            SendToServer(_msgCreateRoom);
            _room.ERoomState = ERoomState.RequestCreate;
        }

        public void SendRequestJoinRoom(long roomGuid)
        {
            if (_room == null)
            {
                return;
            }
            if (_room.ERoomState == ERoomState.RequestJoin)
            {
                LogHelper.Warning("SendRequestJoinRoom Repeated");
                return;
            }
            _room.Guid = roomGuid;
            var msg = new Msg_CR_JoinRoom();
            msg.RoomGuid = roomGuid;
            SendToServer(msg);
            _room.ERoomState = ERoomState.RequestJoin;
        }

        public void SendRoomReadyInfo(bool flag)
        {
            if (_room == null)
            {
                return;
            }
            var msg = new Msg_CR_RoomReadyInfo();
            msg.RoomGuid = _room.Guid;
            msg.Flag = flag ? 1 : 0;
            SendToServer(msg);
        }

        public void SendRequestExitRoom(long roomGuid)
        {
            if (_room == null)
            {
                return;
            }
            var msg = new Msg_CR_RoomExit();
            msg.RoomGuid = _room.Guid;
            SendToServer(msg);
        }

        #endregion

        #region Receive

        public void OnCreateRoomRet(Msg_RC_CreateRoomRet msg)
        {
            if (_room == null)
            {
                return;
            }
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                _room.ERoomState = ERoomState.RequstFailed;
                return;
            }
            var user = PoolFactory<RoomUser>.Get();
            user.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserName, false);
            _room.OnCreate(user, msg.RoomGuid, _msgCreateRoom.ProjectGuid, _msgCreateRoom.EBattleType);
            LogHelper.Debug("CreateRoom Success {0}", msg.RoomGuid);
        }

        internal void OnJoinRoomRet(Msg_RC_JoinRoomRet msg)
        {
            if (_room == null)
            {
                return;
            }
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                _room.ERoomState = ERoomState.RequstFailed;
            }
        }

        internal void OnRoomInfo(Msg_RC_RoomInfo msg)
        {
            if (_room == null)
            {
                return;
            }
            for (int i = 0; i < msg.Users.Count; i++)
            {
                OnNewUserJoinRoom(msg.Users[i]);
            }
            _room.OnRoomInfo(msg.HostUserGuid, msg.RoomGuid, msg.ProjectGuid, msg.EBattleType);
        }

        internal void OnNewUserJoinRoom(Msg_RC_RoomUserInfo msg)
        {
            if (_room == null)
            {
                return;
            }
            var user = PoolFactory<RoomUser>.Get();
            user.Init(msg.UserGuid, msg.UserName, msg.Ready == 1);
            _room.AddUser(user);
        }

        internal void OnUserExit(Msg_RC_UserExitRoom msg)
        {
            if (_room == null)
            {
                return;
            } 
            _room.UserExit(msg.UserGuid, msg.HostUserGuid);
        }

        internal void OnWarnningHost()
        {
            if (_room == null)
            {
                return;
            }
            _room.WarnningHost();
        }

        internal void OnRoomOpen()
        {
            if (_room == null)
            {
                return;
            }
            _room.Open();
        }

        internal void OnUserReadyInfo(Msg_RC_UserReadyInfo msg)
        {
            if (_room == null)
            {
                return;
            }
            _room.UserReady(msg.UserGuid, msg.Flag == 1);
        }

        #endregion
    }
}
