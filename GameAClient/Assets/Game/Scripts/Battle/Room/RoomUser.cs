/********************************************************************
** Filename : RoomUser
** Author : Dong
** Date : 2017/6/21 星期三 下午 7:52:03
** Summary : RoomUser
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA.Game
{
    [Poolable(MinPoolSize = 100, PreferedPoolSize = 1000, MaxPoolSize = 10000)]
    public class RoomUser : IPoolableObject
    {
        protected long _guid;
        protected string _name;
        protected bool _ready;

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

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _guid = 0;
            _name = null;
            _ready = false;
        }

        public void OnDestroyObject()
        {
        }

        public void Init(long guid, string name, bool ready)
        {
            _guid = guid;
            _name = name;
            _ready = ready;
        }

        public override string ToString()
        {
            return string.Format("Guid: {0}, Name: {1}, Ready: {2}", _guid, _name, _ready);
        }
    }
}
