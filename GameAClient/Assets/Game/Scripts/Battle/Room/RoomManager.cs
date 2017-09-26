/********************************************************************
** Filename : RoomManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:23:13
** Summary : RoomManager
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class RoomManager
    {
        private static RoomManager _instance;
        private bool _run;
        private RoomClient _roomClient = new RoomClient();
        private Room _room = new Room();
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
            string address = SocialApp.Instance.RoomServerAddress;
            if (string.IsNullOrEmpty(address))
            {
                address = "localhost";
            }
            ConnectRS(address, 6000);
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

        #region Room Send

        /// <summary>
        /// 请求登陆服务器
        /// </summary>
        public void SendPlayerLoginRS()
        {
            var login = new Msg_CR_Login();
            login.ClientVersion = GlobalVar.Instance.AppVersion;
            login.UserId = LocalUser.Instance.UserGuid;
            SendToServer(login);
        }

        public void SendRequestCreateRoom(EBattleType eBattleType, long projectGuid)
        {
            _msgCreateRoom.EBattleType = eBattleType;
            _msgCreateRoom.ProjectGuid = projectGuid;
            SendToServer(_msgCreateRoom);
        }

        public void SendRequestJoinRoom(long roomId)
        {
            var msg = new Msg_CR_JoinRoom();
            msg.RoomGuid = roomId;
            SendToServer(msg);
        }

        public void SendRoomReadyInfo(bool flag)
        {
            var msg = new Msg_CR_UserReadyInfo();
            msg.Flag = flag ? 1 : 0;
            SendToServer(msg);
        }

        public void SendRequestExitRoom(long roomGuid)
        {
            var msg = new Msg_CR_UserExit();
            msg.Flag = 1;
            SendToServer(msg);
        }

        #endregion

        #region Room Receive

        public void OnCreateRoomRet(Msg_RC_CreateRoomRet msg)
        {
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                _room.OnCreateFailed();
                return;
            }
            var user = new RoomUser();
            user.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserName, false);
            _room.OnCreateSuccess(user, msg.RoomGuid, _msgCreateRoom.ProjectGuid, _msgCreateRoom.EBattleType);
            LogHelper.Debug("CreateRoom Success {0}", msg.RoomGuid);
        }

        internal void OnJoinRoomRet(Msg_RC_JoinRoomRet msg)
        {
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                _room.OnJoinFailed();
                return;
            }
            _room.OnJoinSuccess();
        }

        internal void OnRoomInfo(Msg_RC_RoomInfo msg)
        {
            _room.OnRoomInfo(msg);
        }

        internal void OnNewUserJoinRoom(Msg_RC_RoomUserEnter msg)
        {
            Msg_RC_RoomUserInfo msgUser = msg.UserInfo;
            var user = new RoomUser();
            user.Init(msgUser.UserGuid, msgUser.UserName, msgUser.Ready == 1);
            _room.AddUser(user);
        }

        internal void OnUserExit(Msg_RC_UserExit msg)
        {
            _room.OnUserExit(msg.UserGuid, msg.HostUserGuid);
        }

        internal void OnSelfExit(Msg_RC_UserExitRet msg)
        {
            _room.OnSelfExit(msg.Flag == 1);
        }

        internal void OnUserReadyInfo(Msg_RC_UserReadyInfo msg)
        {
            _room.OnUserReady(msg.UserGuid, msg.Flag == 1);
        }

        internal void OnWarnningHost()
        {
            _room.OnWarnningHost();
        }

        internal void OnOpenBattle()
        {
            _room.OnOpenBattle();
        }

        #endregion
    }
}
