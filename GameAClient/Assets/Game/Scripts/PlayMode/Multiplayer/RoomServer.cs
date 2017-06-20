/********************************************************************
** Filename : RoomServer
** Author : Dong
** Date : 2017/6/19 星期一 下午 5:31:43
** Summary : RoomServer
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    public class RoomServer : NetServer
    {
        public static RoomServer Instance = new RoomServer();

        public RoomServer()
        {
        }

        public RoomServer(ProtoSerializer serializer, IHandler<object, NetLink> handler) : base(serializer, handler)
        {
            _handler = new RoomHandler();
            _serializer = new ServerProtoSerializer(typeof(ECSMsgType), ProtoSerializer.ProtoNameSpace,
                typeof(ServerProtoSerializer).Assembly);
        }

        public bool Init()
        {
            _port = 8080;
            _ip = "127.0.0.1";
            if (!StartUp())
            {
                LogHelper.Error("RS StartUp port[{1}] for Client Failed",  _port);
                return false;
            }
            LogHelper.Info("RS StartUp port[{1}] for Client Success.", _port);
            return true;
        }
    }

    public class RoomHandler : Handler<object, NetLink>
    {
        protected override void InitHandler()
        {
        }
    }
}
