/********************************************************************
** Filename : PlayerBase
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:34:10
** Summary : PlayerBase
***********************************************************************/

using System;
using System.Collections;

namespace GameA.Game
{
    public class PlayerBase : ActorBase
    {
        protected long _playerGuid;

        public long PlayerGuid
        {
            get { return _playerGuid; }
        }

        public void ExitBattle()
        {
            if (RoomManager.RoomClient != null && RoomManager.RoomClient.IsConnnected())
            {
                RoomManager.RoomClient.Disconnect();
            }
            //RoomManager.Instance.Room.Exit(this);
        }
    }
}
