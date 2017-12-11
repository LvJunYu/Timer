using GameA;
using GameA.Game;
using SoyEngine.Proto;

namespace SoyEngine.MasterServer
{
    public class MSClient : JoyTCPClient
    {
        public MSClient()
        {
            _handler = new MSHandler();
            _serializer = new ClientProtoSerializer(typeof(ECMMsgType), ProtoSerializer.ProtoNameSpace,
                new GeneratedClientSerializer());
            
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            RoomManager.Instance.SendPlayerLoginMS();
        }

        protected override void OnDisconnected(int code = 0)
        {
        }
    }

    public class MSHandler : Handler<object, object>
    {
        private GameModeNetPlay _modeNetPlay;
        
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
        }

        private void Msg_MC_LoginRet(Msg_MC_LoginRet msg, object netlink)
        {
            LogHelper.Info("Msg_MC_LoginRet: {0}", msg.ResultCode);
        }

        private void Msg_MC_RoomOpen(Msg_MC_RoomOpen msg, object obj)
        {
            RoomManager.Instance.OnOpenBattle();
            //这里进行房间和战场的交接
            EBattleType battleType = RoomManager.Instance.Room.EBattleType;

            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Format("请求进入关卡"));
            var project = new Project();
            project.Request(RoomManager.Instance.Room.ProjectId,
                () => project.RequestPlay(() =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        switch (battleType)
                        {
                            case EBattleType.EBT_PVE:
                                GameManager.Instance.RequestPlayMultiCooperation(project);
                                break;
                            case EBattleType.EBT_PVP:
                                GameManager.Instance.RequestPlayMultiBattle(project);
                                break;
                        }
                        SocialApp.Instance.ChangeToGame();
                        _modeNetPlay = GM2DGame.Instance.GameMode as GameModeNetPlay;
                    },
                    error =>
                    {
                        SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                        SocialGUIManager.ShowPopupDialog("进入关卡失败");
                    }),
                error =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                });
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
    }
}