/********************************************************************
** Filename : RoomClient
** Author : Dong
** Date : 2017/6/20 星期二 上午 11:03:36
** Summary : RoomClient
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA.Game
{
    /// <summary>
    /// 服务器需要取得并且分配好三个角色的出生位置
    /// </summary>
    public class RoomClient : NetClient
    {
        public RoomClient()
        {
            _handler = new RoomClientHandler();
            _serializer = new ClientProtoSerializer(typeof(ECRMsgType), ProtoSerializer.ProtoNameSpace,
                new GeneratedClientSerializer());
        }

        protected override void OnConnected()
        {
            LogHelper.Debug("RoomClient OnConnected");
            RoomManager.Instance.SendPlayerLoginRS();
            new GameObject().AddComponent<TestRoom>();
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
        private GameModeNetPlay _modeNetPlay;

        protected override void InitHandler()
        {
            RegisterHandler<Msg_RC_LoginRet>(Msg_RC_LoginRet);
            RegisterHandler<Msg_RC_CreateRoomRet>(Msg_RC_CreateRoomRet);
            RegisterHandler<Msg_RC_JoinRoomRet>(Msg_RC_JoinRoomRet);
            RegisterHandler<Msg_RC_RoomInfo>(Msg_RC_RoomInfo);
            RegisterHandler<Msg_RC_RoomUserEnter>(Msg_RC_RoomUserEnter);
            RegisterHandler<Msg_RC_UserExitRet>(Msg_RC_UserExitRet);
            RegisterHandler<Msg_RC_UserExit>(Msg_RC_UserExit);
            RegisterHandler<Msg_RC_UserReadyInfo>(Msg_RC_UserReadyInfo);
            RegisterHandler<Msg_RC_WarnningHost>(Msg_RC_WarnningHost);
            RegisterHandler<Msg_RC_RoomOpen>(Msg_RC_RoomOpen);

            RegisterHandler<Msg_RC_UserEnterBattle>(Msg_RC_UserEnterBattle);
            RegisterHandler<Msg_RC_BattleStart>(Msg_RC_BattleStart);
            RegisterHandler<Msg_RC_InputDatas>(Msg_RC_InputDatas);
            RegisterHandler<Msg_RC_UserExitBattle>(Msg_RC_UserExitBattle);
            RegisterHandler<Msg_RC_BattleClose>(Msg_RC_BattleClose);
        }

        private void Msg_RC_BattleClose(Msg_RC_BattleClose msg, NetLink netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnBattleClose(msg);
            }
        }

        private void Msg_RC_UserExitBattle(Msg_RC_UserExitBattle msg, NetLink netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnUserExitBattle(msg);
            }
        }

        private void Msg_RC_InputDatas(Msg_RC_InputDatas msg, NetLink netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnInputDatas(msg);
            }
        }

        private void Msg_RC_BattleStart(Msg_RC_BattleStart msg, NetLink netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnBattleStart(msg);
            }
        }

        private void Msg_RC_UserEnterBattle(Msg_RC_UserEnterBattle msg, NetLink netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnUserEnterBattle(msg);
            }
        }

        private void Msg_RC_RoomOpen(Msg_RC_RoomOpen msg, NetLink netLink)
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
                        SocialGUIManager.Instance.ChangeToGameMode();
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

        private void Msg_RC_WarnningHost(Msg_RC_WarnningHost msg, NetLink netLink)
        {
            RoomManager.Instance.OnWarnningHost();
        }

        private void Msg_RC_UserReadyInfo(Msg_RC_UserReadyInfo msg, NetLink netLink)
        {
            RoomManager.Instance.OnUserReadyInfo(msg);
        }

        private void Msg_RC_UserExit(Msg_RC_UserExit msg, NetLink netLink)
        {
            RoomManager.Instance.OnUserExit(msg);
        }

        private void Msg_RC_UserExitRet(Msg_RC_UserExitRet msg, NetLink netLink)
        {
            RoomManager.Instance.OnSelfExit(msg);
        }

        private void Msg_RC_RoomUserEnter(Msg_RC_RoomUserEnter msg, NetLink netLink)
        {
            RoomManager.Instance.OnNewUserJoinRoom(msg);
        }

        private void Msg_RC_RoomInfo(Msg_RC_RoomInfo msg, NetLink netLink)
        {
            RoomManager.Instance.OnRoomInfo(msg);
        }

        private void Msg_RC_JoinRoomRet(Msg_RC_JoinRoomRet msg, NetLink netLink)
        {
            RoomManager.Instance.OnJoinRoomRet(msg);
        }

        private void Msg_RC_CreateRoomRet(Msg_RC_CreateRoomRet msg, NetLink netLink)
        {
            RoomManager.Instance.OnCreateRoomRet(msg);
        }

        private void Msg_RC_LoginRet(Msg_RC_LoginRet msg, NetLink netLink)
        {
            if (msg.ResultCode == ERLoginCode.ELC_Success)
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