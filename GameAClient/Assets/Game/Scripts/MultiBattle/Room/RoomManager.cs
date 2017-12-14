/********************************************************************
** Filename : RoomManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:23:13
** Summary : RoomManager
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using SoyEngine.MasterServer;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class RoomManager
    {
        private static RoomManager _instance;
        private bool _run;
        private RoomClient _roomClient = new RoomClient();
        private MSClient _msClient = new MSClient();
        private Room _room = new Room();
        private Msg_CM_CreateRoom _msgCreateRoom = new Msg_CM_CreateRoom();
        private List<RoomInfo> _roomList = new List<RoomInfo>();
        public bool IsEnd;

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

        public static MSClient MsClient
        {
            get { return Instance._msClient; }
        }

        public List<RoomInfo> RoomList
        {
            get { return _roomList; }
        }

        public bool Init()
        {
            _run = false;
            var msAddress = SocialApp.Instance.MasterServerAddress;
            if (string.IsNullOrEmpty(msAddress))
            {
                msAddress = "127.0.0.1";
            }
            ConnectMS(msAddress, 3001);
            _run = true;

            Messenger.AddListener(EMessengerType.OnApplicationQuit, OnApplicationQuit);
            return true;
        }

        private void OnApplicationQuit()
        {
            if (_roomClient != null)
            {
                _roomClient.Disconnect();
            }
            if (_msClient != null)
            {
                _msClient.Disconnect();
            }
        }

        public void ConnectRS(string ip, ushort port)
        {
            LogHelper.Debug("StartConnectRS: {0}, {1}", ip, port);
            _roomClient.Connect(ip, port, null, e =>
            {
                Loom.QueueOnMainThread(() =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("联机服务失败，请稍后再试");
                });
            }, 10000);
        }

        public void ConnectMS(string ip, ushort port)
        {
            LogHelper.Debug("StartConnectMS: {0}, {1}", ip, port);
            _msClient.ConnectWithRetry(ip, port);
        }

        public void Update()
        {
            if (!_run)
            {
                return;
            }
            if (_roomClient != null)
            {
                _roomClient.Update();
            }
            if (_msClient != null)
            {
                _msClient.Update();
            }
        }

        private void SendToRSServer(object msg)
        {
            if (_roomClient != null && _roomClient.IsConnected())
            {
                _roomClient.Write(msg);
            }
        }

        private void SendToMSServer(object msg)
        {
            LogHelper.Debug("MSClient IsConnected: {0}", _msClient.IsConnected());
            if (_msClient != null && _msClient.IsConnected())
            {
                _msClient.Write(msg);
            }
        }

        #region Room Send

        /// <summary>
        /// 请求登陆服务器
        /// </summary>
        public void SendPlayerLoginMS()
        {
            var login = new Msg_CM_Login();
            login.ClientVersion = GlobalVar.Instance.AppVersion;
            login.Token = LocalUser.Instance.Account.Token;
            login.NickName = LocalUser.Instance.User.UserInfoSimple.NickName;
            SendToMSServer(login);
        }

        /// <summary>
        /// 请求登陆服务器
        /// </summary>
        public void SendPlayerLoginRS()
        {
            var login = new Msg_CR_Login();
            login.ClientVersion = GlobalVar.Instance.AppVersion;
            login.Token = LocalUser.Instance.Account.Token;
//            login.UserId = LocalUser.Instance.UserGuid;
            SendToRSServer(login);
        }

        public void SendRequestCreateRoom(long projectGuid)
        {
            if (!_msClient.IsConnected())
            {
                SocialGUIManager.ShowPopupDialog("当前联机服务不可用，请稍后再试");
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在创建房间");
            _msgCreateRoom.ProjectId = projectGuid;
            _msgCreateRoom.MaxUserCount = 6;
            SendToMSServer(_msgCreateRoom);
        }

        public void SendRequestJoinRoom(long roomId)
        {
            if (!_msClient.IsConnected())
            {
                SocialGUIManager.ShowPopupDialog("当前联机服务不可用，请稍后再试");
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在加入房间");
            var msg = new Msg_CM_JoinRoom();
            msg.RoomGuid = roomId;
            SendToMSServer(msg);
        }

        public void SendRoomReadyInfo(bool flag)
        {
            var msg = new Msg_CM_UserReadyInfo();
            msg.Flag = flag ? 1 : 0;
            SendToRSServer(msg);
        }

        public void SendRequestExitRoom(long roomGuid)
        {
            var msg = new Msg_CM_UserExit();
            msg.Flag = 1;
            SendToRSServer(msg);
        }

        #endregion

        #region Room Receive

        public void OnCreateRoomRet(Msg_MC_CreateRoomRet msg)
        {
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(this);
                SocialGUIManager.ShowPopupDialog("房间创建失败");
//                _room.OnCreateFailed();
                return;
            }
//            var user = new RoomUser();
//            user.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserName, false);
//            _room.OnCreateSuccess(user, msg.RoomGuid, _msgCreateRoom.ProjectGuid, _msgCreateRoom.EBattleType);
            ConnectRS(msg.RSAddress, (ushort) msg.RSPort);
            LogHelper.Debug("CreateRoom Success {0}", msg.RoomGuid);
        }

        internal void OnJoinRoomRet(Msg_MC_JoinRoomRet msg)
        {
            if (msg.ResultCode != ERoomCode.ERC_Success)
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(this);
                SocialGUIManager.ShowPopupDialog("加入房间失败");
//                _room.OnJoinFailed();
                return;
            }
//            _room.OnJoinSuccess();
            ConnectRS(msg.RSAddress, (ushort) msg.RSPort);
        }

        internal void OnRoomInfo(Msg_MC_RoomInfo msg)
        {
            _room.OnRoomInfo(msg);
        }

        internal void OnNewUserJoinRoom(Msg_MC_RoomUserEnter msg)
        {
            Msg_MC_RoomUserInfo msgUser = msg.UserInfo;
            var user = new RoomUser();
            user.Init(msgUser.UserGuid, msgUser.NickName, msgUser.Ready == 1);
            _room.AddUser(user);
        }

        internal void OnUserExit(Msg_MC_UserExit msg)
        {
            _room.OnUserExit(msg.UserGuid, msg.HostUserGuid);
        }

        internal void OnSelfExit(Msg_MC_UserExitRet msg)
        {
            _room.OnSelfExit(msg.Flag == 1);
        }

        internal void OnUserReadyInfo(Msg_MC_UserReadyInfo msg)
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

        public void RequestRoomList(bool append, long projectId = 0)
        {
            var data = new Msg_CM_QueryRoomList();
            data.ProjectId = projectId;
            if (append)
            {
                if (_roomList.Count > 0)
                {
                    data.MinRoomId = _roomList[_roomList.Count - 1].RoomId;
                }
            }
            else
            {
                _roomList.Clear();
            }
            data.MaxCount = UPCtrlWorldMulti.PageSize;
            MsClient.Write(data);
        }
    }
}