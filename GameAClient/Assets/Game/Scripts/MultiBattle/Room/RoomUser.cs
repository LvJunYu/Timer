﻿/********************************************************************
** Filename : RoomUser
** Author : Dong
** Date : 2017/6/21 星期三 下午 7:52:03
** Summary : RoomUser
***********************************************************************/

using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 100, PreferedPoolSize = 1000, MaxPoolSize = 10000)]
    public class RoomUser : IPoolableObject
    {
        protected long _guid;
        protected string _name;
        protected bool _ready;
        protected int _inx;

        public string Name
        {
            get { return _name; }
        }

        public long Guid
        {
            get { return _guid; }
        }

        public bool Ready
        {
            get { return _ready; }
            set { _ready = value; }
        }

        public int Inx
        {
            get { return _inx; }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _guid = 0;
            _name = null;
            _ready = false;
            _inx = 0;
        }

        public void OnDestroyObject()
        {
        }

        public RoomUser()
        {
        }
        
        public RoomUser(Msg_MC_RoomUserInfo roomUserInfo)
        {
            _guid = roomUserInfo.UserGuid;
            _name = roomUserInfo.NickName;
            _ready = roomUserInfo.Ready == 1;
            _inx = roomUserInfo.inx;
        }

        public RoomUser(Msg_RC_RoomUserInfo roomUserInfo)
        {
            _guid = roomUserInfo.UserGuid;
            _name = roomUserInfo.NickName;
            _ready = roomUserInfo.Ready == 1;
            _inx = roomUserInfo.inx;
        }
        
        public void Init(long guid, string name, bool ready, int index = 0)
        {
            _guid = guid;
            _name = name;
            _ready = ready;
            _inx = index;
        }

    }
}
