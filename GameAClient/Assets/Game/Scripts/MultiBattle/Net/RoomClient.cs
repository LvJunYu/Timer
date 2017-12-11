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