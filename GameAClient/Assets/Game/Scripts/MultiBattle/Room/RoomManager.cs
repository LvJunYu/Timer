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
        private string _masterServerAddress;
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

        public string MasterServerAddress
        {
            get { return _masterServerAddress; }
            set { _masterServerAddress = value; }
        }

        public List<RoomInfo> RoomList
        {
            get { return _roomList; }
        }

        public bool Init()
        {
            _run = false;
            ConnectMS(_masterServerAddress, 3001);
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

        public void SendToRSServer(object msg)
        {
            if (_roomClient != null && _roomClient.IsConnected())
            {
                _roomClient.Write(msg);
            }
        }

        public void SendToMSServer(object msg)
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
            ProjectManager.Instance.GetDataOnAsync(projectGuid, p =>
            {
                _msgCreateRoom.MaxUserCount = p.NetData.PlayerCount;
                SendToMSServer(_msgCreateRoom);
            });
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

        public void SendRequestQuickPlay(long projectId = 1)
        {
            if (!_msClient.IsConnected())
            {
                SocialGUIManager.ShowPopupDialog("当前联机服务不可用，请稍后再试");
                return;
            }
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在加入");
            var msg = new Msg_CM_QuickPlay();
            msg.ProjectId = projectId;
            if (projectId == 1)
            {
                SendToMSServer(msg);
            }
            else
            {
                ProjectManager.Instance.GetDataOnAsync(projectId, p =>
                {
                    msg.MaxUserCount = p.NetData.PlayerCount;
                    SendToMSServer(msg);
                });
            }
        }
        
        public void SendQueryRoom(long roomId)
        {
            if (!_msClient.IsConnected())
            {
                SocialGUIManager.ShowPopupDialog("当前联机服务不可用，请稍后再试");
                return;
            }
            var msg = new Msg_CM_QueryRoom();
            msg.RoomId = roomId;
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
        
        public void SendQueryRoomList(bool append, long projectId = 0)
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
            SendToMSServer(data);
        }

        public void SendDeletePlayer(long userId)
        {
            var data = new Msg_CR_Kick();
            data.UserGuid = userId;
            SendToRSServer(data);
        }
        
        public void SendExitRoom()
        {
            var data = new Msg_CR_UserExit();
            data.Flag = 1;
            SendToRSServer(data);
        }

        public void SendChangePos(int index)
        {
            var data = new Msg_CR_ChangePos();
            data.PosInx = index;
            SendToRSServer(data);
        }

        public void SendRoomPrepare(bool value)
        {
            var data = new Msg_CR_UserReadyInfo();
            data.ReadyFlag = value;
            SendToRSServer(data);
        }

        public void SendRoomOpen()
        {
            var data = new Msg_CR_RoomOpen();
            data.Flag = 1;
            SendToRSServer(data);
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
                if (msg.ResultCode == ERoomCode.ERC_Full)
                {
                    SocialGUIManager.ShowPopupDialog("房间人数已满");
                }
                else if (msg.ResultCode == ERoomCode.ERC_NotExist)
                {
                    SocialGUIManager.ShowPopupDialog("房间已失效");
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("加入房间失败");
                }
                Messenger.Broadcast(EMessengerType.OnJoinRoomFail);
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
//            _room.OnWarnningHost();
        }

        internal void OnOpenBattle()
        {
            _room.OnOpenBattle();
        }

        public void OnQueryRoomListRet(Msg_MC_QueryRoomList msg)
        {
            var list = msg.Data;
            for (int i = 0; i < list.Count; i++)
            {
                RoomList.Add(new RoomInfo(list[i]));
            }
            IsEnd = list.Count < UPCtrlWorldMulti.PageSize;
            Messenger.Broadcast(EMessengerType.OnRoomListChanged);
        }

        public void OnQueryRoomRet(Msg_MC_QueryRoom msg)
        {
            if (msg.ResultCode == ERoomCode.ERC_Success)
            {
                RoomList.Clear();
                RoomList.Add(new RoomInfo(msg.Data));
                Messenger<Msg_MC_QueryRoom>.Broadcast(EMessengerType.OnQueryRoomRet, msg);
            }
            else if (msg.ResultCode == ERoomCode.ERC_NotExist)
            {
                SocialGUIManager.ShowPopupDialog(string.Format("没有房间ID为{0}的房间", msg.RoomId));
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("查找失败");
            }
        }
        #endregion
    }
}