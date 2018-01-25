/********************************************************************
** Filename : RoomClient
** Author : Dong
** Date : 2017/6/20 星期二 上午 11:03:36
** Summary : RoomClient
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    /// <summary>
    /// 服务器需要取得并且分配好三个角色的出生位置
    /// </summary>
    public class RoomClient : JoyTCPClient
    {
        public RoomClient()
        {
            _handler = RoomClientHandler.Intance;
            _serializer = new ClientProtoSerializer(typeof(ECRMsgType), ProtoSerializer.ProtoNameSpace,
                new GeneratedClientSerializer());
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            LogHelper.Debug("RoomClient OnConnected");
            RoomManager.Instance.SendPlayerLoginRS();
//            new GameObject().AddComponent<TestRoom>();
        }

        protected override void OnDisconnected(int code = 0)
        {
            base.OnDisconnected(code);
            LogHelper.Debug("RoomClient OnDisConnected");
            Loom.QueueOnMainThread(RoomClientHandler.Intance.OnDisconnect);
        }
    }

    public class RoomClientHandler : Handler<object, object>
    {
        public static RoomClientHandler Intance = new RoomClientHandler();
        
        private GameModeNetPlay _modeNetPlay;
        private List<Action> _roomActionList = new List<Action>();

        protected override void InitHandler()
        {
            RegisterHandler<Msg_RC_LoginRet>(Msg_RC_LoginRet);
            RegisterHandler<Msg_RC_FrameDataArray>(Msg_RC_FrameDataArray);
            RegisterHandler<Msg_RC_RoomClose>(Msg_RC_RoomClose);
            
            RegisterHandler<Msg_RC_RoomUserEnter>(Msg_RC_RoomUserEnter);
            RegisterHandler<Msg_RC_UserReadyInfo>(Msg_RC_UserReadyInfo);
            RegisterHandler<Msg_RC_ChangePos>(Msg_RC_ChangePos);
            RegisterHandler<Msg_RC_UserExit>(Msg_RC_UserExit);
            RegisterHandler<Msg_RC_Kick>(Msg_RC_Kick);
            RegisterHandler<Msg_RC_WarnningHost>(Msg_RC_WarnningHost);
            RegisterHandler<Msg_RC_RoomOpen>(Msg_RC_RoomOpen);
            RegisterHandler<Msg_RC_RoomChat>(Msg_RC_RoomChat);
        }

        private void Msg_RC_RoomChat(Msg_RC_RoomChat msg, object netlink)
        {
        }

        private void Msg_RC_RoomOpen(Msg_RC_RoomOpen msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnRoomOpen();
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_RoomOpen(msg, netlink);
                });
            }
        }

        private void Msg_RC_WarnningHost(Msg_RC_WarnningHost msg, object netlink)
        {
//            RoomManager.Instance.OnWarnningHost();
        }

        private void Msg_RC_Kick(Msg_RC_Kick msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnUserKick(msg);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_Kick(msg, netlink);
                });
            }
        }

        private void Msg_RC_UserExit(Msg_RC_UserExit msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnUserExit(msg);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_UserExit(msg, netlink);
                });
            }
        }

        private void Msg_RC_ChangePos(Msg_RC_ChangePos msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnRoomChangePos(msg);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_ChangePos(msg, netlink);
                });
            }
        }

        private void Msg_RC_UserReadyInfo(Msg_RC_UserReadyInfo msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnRoomPlayerReadyChanged(msg);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_UserReadyInfo(msg, netlink);
                });
            }
        }

        private void Msg_RC_RoomUserEnter(Msg_RC_RoomUserEnter msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnRoomUserEnter(msg.UserInfo);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_RoomUserEnter(msg, netlink);
                });
            }
        }

        private void Msg_RC_FrameDataArray(Msg_RC_FrameDataArray msg, object netlink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnInputDatas(msg);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_FrameDataArray(msg, netlink);
                });
            }
        }

        private void Msg_RC_RoomClose(Msg_RC_RoomClose msg, object netLink)
        {
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnRoomClose(msg.ResultCode);
            }
            else
            {
                _roomActionList.Add(() =>
                {
                    Msg_RC_RoomClose(msg, netLink);
                });
            }
        }
        
        private void Msg_RC_LoginRet(Msg_RC_LoginRet msg, object netLink)
        {
            _modeNetPlay = null;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(RoomManager.Instance);
            if (msg.ResultCode == ERLoginCode.ELC_Success)
            {
                LogHelper.Debug("Login RS Success");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在启动游戏");
                ProjectManager.Instance.GetDataOnAsync(msg.RoomInfo.ProjectId, project =>
                {
                    project.PrepareRes(() =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            GameManager.Instance.RequestPlayMultiBattle(project);
                            SocialApp.Instance.ChangeToGame();
                            _modeNetPlay = GM2DGame.Instance.GameMode as GameModeNetPlay;
                            _modeNetPlay.OnRoomInfo(msg.RoomInfo);
                            for (int i = 0; i < _roomActionList.Count; i++)
                            {
                                _roomActionList[i].Invoke();
                            }
                            _roomActionList.Clear();
                        },
                        () =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                            SocialGUIManager.ShowPopupDialog("进入关卡失败");
                        });
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("进入关卡失败");
                });
            }
            else
            {
                LogHelper.Debug("Login RS Failed");
                SocialGUIManager.ShowPopupDialog("联机失败");
            }
        }


        public void OnDisconnect()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().TryCloseLoading(RoomManager.Instance);
            if (_modeNetPlay != null)
            {
                _modeNetPlay.OnDisconnected();
            }
            else
            {
                _roomActionList.Add(OnDisconnect);
            }
        }
    }
}