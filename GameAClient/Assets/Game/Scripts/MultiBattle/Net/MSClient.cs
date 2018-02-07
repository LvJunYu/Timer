using GameA;
using GameA.Game;
using SoyEngine.Proto;
using UnityEngine;

namespace SoyEngine.MasterServer
{
    public class MSClient : JoyTCPClient
    {
        private float HeartBeatIntervalSecond = 50;
        private GameTimer _heartBeatGameTimer = new GameTimer();
        private int _reconnectAttempts;

        public MSClient()
        {
            _handler = new MSHandler();
            _serializer = new ClientProtoSerializer(typeof(ECMMsgType), ProtoSerializer.ProtoNameSpace,
                new GeneratedClientSerializer());
            StartHeartBeatCheck();
        }

        public override void Write(object obj)
        {
            _heartBeatGameTimer.Reset();
            base.Write(obj);
        }

        public void ConnectWithRetry(string ip, ushort port)
        {
            _ip = ip;
            _port = port;
            Reconnect();
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            RoomManager.Instance.SendPlayerLoginMS();
            _reconnectAttempts = 0;
        }

        protected override void OnDisconnected(int code = 0)
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(RoomManager.Instance);
            base.OnDisconnected(code);
            LogHelper.Debug("MSClient OnDisConnected");
            Loom.QueueOnMainThread(Reconnect);
        }

        private void TryReconnect()
        {
            if (_reconnectAttempts < 5)
            {
                _reconnectAttempts++;
            }

            //重连的间隔时间会越来越长  
            int timeout = 2 << _reconnectAttempts;
            LogHelper.Info("链接关闭，{0}秒后重新连接", timeout);
            Loom.QueueOnMainThread(Reconnect, timeout * 1000);
        }

        private void Reconnect()
        {
            Connect(_ip, _port, null, exception => TryReconnect(), 5000);
        }

        private void StartHeartBeatCheck()
        {
            LogHelper.Debug("StartHeartBeatCheck TimerInterval: {0}", _heartBeatGameTimer.GetIntervalSeconds());
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunWaitForSeconds(
                Mathf.Max(0, HeartBeatIntervalSecond - (float) _heartBeatGameTimer.GetIntervalSeconds()) + 1,
                () =>
                {
                    if (IsConnected())
                    {
                        if (_heartBeatGameTimer.PassedSeconds(HeartBeatIntervalSecond))
                        {
                            Msg_CM_ImAlive msgCmImAlive = new Msg_CM_ImAlive();
                            msgCmImAlive.Flag = 1;
                            Write(msgCmImAlive);
                        }
                    }
                    else
                    {
                        _heartBeatGameTimer.Reset();
                    }

                    StartHeartBeatCheck();
                }));
        }
    }

    public class MSHandler : Handler<object, object>
    {
//        private GameModeNetPlay _modeNetPlay;

        protected override void InitHandler()
        {
            RegisterHandler<Msg_MC_LoginRet>(Msg_MC_LoginRet);
            RegisterHandler<Msg_MC_CreateRoomRet>(Msg_MC_CreateRoomRet);
            RegisterHandler<Msg_MC_JoinRoomRet>(Msg_MC_JoinRoomRet);
            RegisterHandler<Msg_MC_RoomInfo>(Msg_MC_RoomInfo);
            RegisterHandler<Msg_MC_RoomUserEnter>(Msg_MC_RoomUserEnter);
            RegisterHandler<Msg_MC_UserExitRet>(Msg_MC_UserExitRet);
            RegisterHandler<Msg_MC_UserExit>(Msg_MC_UserExit);
            RegisterHandler<Msg_MC_UserReadyInfo>(Msg_MC_UserReadyInfo);
            RegisterHandler<Msg_MC_WarnningHost>(Msg_MC_WarnningHost);
            RegisterHandler<Msg_MC_RoomOpen>(Msg_MC_RoomOpen);
            RegisterHandler<Msg_MC_QueryRoomList>(Msg_MC_QueryRoomListRet);
            RegisterHandler<Msg_MC_QueryRoom>(Msg_MC_QueryRoomRet);
            RegisterHandler<Msg_MC_Chat>(Msg_MC_Chat);
            RegisterHandler<Msg_MC_SelectProject>(Msg_MC_SelectProject);
            RegisterHandler<Msg_MC_UnselectProject>(Msg_MC_UnselectProject);
            RegisterHandler<Msg_MC_CreateTeam>(Msg_MC_CreateTeam);
            RegisterHandler<Msg_MC_ExitTeam>(Msg_MC_ExitTeam);
        }

        private void Msg_MC_ExitTeam(Msg_MC_ExitTeam msg, object netlink)
        {
            LocalUser.Instance.MutiBattleData.OnExitTeam(msg);
        }

        private void Msg_MC_CreateTeam(Msg_MC_CreateTeam msg, object netlink)
        {
            if (msg.ResultCode == EMCCreateTeamCode.MCCT_Success)
            {
                LocalUser.Instance.MutiBattleData.OnCreateTeam(msg);
            }
            else
            {
                LogHelper.Error("CreateTeam fail, ResultCode = {0}", msg.ResultCode);
                SocialGUIManager.ShowPopupDialog("创建队伍失败");
            }
        }

        private void Msg_MC_UnselectProject(Msg_MC_UnselectProject msg, object netlink)
        {
            LocalUser.Instance.MutiBattleData.OnProjectSelectedChanged(msg.ProjectIdList, false);
        }

        private void Msg_MC_SelectProject(Msg_MC_SelectProject msg, object netlink)
        {
            LocalUser.Instance.MutiBattleData.OnProjectSelectedChanged(msg.ProjectIdList, true);
        }

        private void Msg_MC_Chat(Msg_MC_Chat msg, object netlink)
        {
            AppData.Instance.ChatData.OnMCChat(msg);
        }

        private void Msg_MC_QueryRoomListRet(Msg_MC_QueryRoomList msg, object netlink)
        {
            RoomManager.Instance.OnQueryRoomListRet(msg);
        }

        private void Msg_MC_LoginRet(Msg_MC_LoginRet msg, object netlink)
        {
            LogHelper.Info("Msg_MC_LoginRet: {0}", msg.ResultCode);
        }

        private void Msg_MC_RoomOpen(Msg_MC_RoomOpen msg, object obj)
        {
//            RoomManager.Instance.OnOpenBattle();
//            //这里进行房间和战场的交接
//            EBattleType battleType = RoomManager.Instance.Room.EBattleType;
//
//            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Format("请求进入关卡"));
//            var project = new Project();
//            project.Request(RoomManager.Instance.Room.ProjectId,
//                () => project.RequestPlay(() =>
//                    {
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                        switch (battleType)
//                        {
//                            case EBattleType.EBT_PVE:
//                                GameManager.Instance.RequestPlayMultiCooperation(project);
//                                break;
//                            case EBattleType.EBT_PVP:
//                                GameManager.Instance.RequestPlayMultiBattle(project);
//                                break;
//                        }
//                        SocialApp.Instance.ChangeToGame();
//                        _modeNetPlay = GM2DGame.Instance.GameMode as GameModeNetPlay;
//                    },
//                    error =>
//                    {
//                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                        SocialGUIManager.ShowPopupDialog("进入关卡失败");
//                    }),
//                error =>
//                {
//                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
//                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
//                });
        }

        private void Msg_MC_WarnningHost(Msg_MC_WarnningHost msg, object obj)
        {
            RoomManager.Instance.OnWarnningHost();
        }

        private void Msg_MC_UserReadyInfo(Msg_MC_UserReadyInfo msg, object obj)
        {
            RoomManager.Instance.OnUserReadyInfo(msg);
        }

        private void Msg_MC_UserExit(Msg_MC_UserExit msg, object obj)
        {
            RoomManager.Instance.OnUserExit(msg);
        }

        private void Msg_MC_UserExitRet(Msg_MC_UserExitRet msg, object obj)
        {
            RoomManager.Instance.OnSelfExit(msg);
        }

        private void Msg_MC_RoomUserEnter(Msg_MC_RoomUserEnter msg, object obj)
        {
            RoomManager.Instance.OnNewUserJoinRoom(msg);
        }

        private void Msg_MC_RoomInfo(Msg_MC_RoomInfo msg, object obj)
        {
            RoomManager.Instance.OnRoomInfo(msg);
        }

        private void Msg_MC_JoinRoomRet(Msg_MC_JoinRoomRet msg, object obj)
        {
            RoomManager.Instance.OnJoinRoomRet(msg);
        }

        private void Msg_MC_CreateRoomRet(Msg_MC_CreateRoomRet msg, object obj)
        {
            RoomManager.Instance.OnCreateRoomRet(msg);
        }

        private void Msg_MC_QueryRoomRet(Msg_MC_QueryRoom msg, object obj)
        {
            RoomManager.Instance.OnQueryRoomRet(msg);
        }
    }
}