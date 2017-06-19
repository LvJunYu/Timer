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
    }

    public class RoomHandler : Handler<object, NetLink>
    {
        protected override void InitHandler()
        {
        }
    }
}
