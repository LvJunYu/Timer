/********************************************************************
** Filename : Room
** Author : Dong
** Date : 2017/6/20 星期二 下午 4:56:18
** Summary : Room
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public enum ERoomState
    {
        None,
        RequestCreate,
        RequestJoin,
        RequstFailed,
        Created,
        Open,
        Fight,
        Close
    }

    [Poolable(MinPoolSize = 100, PreferedPoolSize = 1000, MaxPoolSize = 10000)]
    public class Room : IPoolableObject
    {
        protected ERoomState _eRoomState;
        protected EBattleType _eBattleType;
        protected long _guid;
        protected long _projectGuid;

        protected RoomUser _hostUser;

        protected List<RoomUser> _users = new List<RoomUser>();

        public long Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public long ProjectGuid
        {
            get { return _projectGuid; }
        }

        public ERoomState ERoomState
        {
            get { return _eRoomState; }
            set { _eRoomState = value; }
        }

        public void OnGet()
        {
        }

        public void OnFree()
        {
            _eRoomState = ERoomState.None;
            _eBattleType = EBattleType.EBT_None;
            _projectGuid = 0;
            _guid = 0;
            _hostUser = null;
            _users.Clear();
        }

        public void OnDestroyObject()
        {
        }

        public bool OnCreate(RoomUser user,long roomGuid, long projectGuid, EBattleType eBattleType)
        {
            _hostUser = user;
            _guid = roomGuid;
            _projectGuid = projectGuid;
            _eBattleType = eBattleType;
            _eRoomState = ERoomState.Created;
            return true;
        }

        internal void OnRoomInfo(long hostUserGuid, long roomGuid, long projectGuid, EBattleType eBattleType)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Guid == hostUserGuid)
                {
                    _hostUser = _users[i];
                    break;
                }
            }
            _guid = roomGuid;
            _projectGuid = projectGuid;
            _eBattleType = eBattleType;
            _eRoomState = ERoomState.Created;
        }

        internal bool AddUser(RoomUser user)
        {
            if (_users.Count >= ConstDefineGM2D.MaxUserCount)
            {
                LogHelper.Error("AddUser Failed, ERC_Full, {0}", user);
                return false;
            }
            if (_users.Contains(user))
            {
                LogHelper.Error("AddUser Failed, ERC_Repeat, {0}", user);
                return false;
            }
            _users.Add(user);
            return true;
        }

        internal void UserReady(long userGuid, bool flag)
        {
             for (int i = _users.Count - 1; i >= 0; i--)
            {
                var user = _users[i];
                if (user != null && user.Guid == userGuid)
                {
                    user.Ready = flag;
                }
            }
        }

        internal void UserExit(long exitGuid, long hostGuid)
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                var user = _users[i];
                if (user != null)
                {
                    if (user.Guid == exitGuid)
                    {
                        _users.Remove(user);
                        PoolFactory<RoomUser>.Free(user);
                    }
                    else if (user.Guid == hostGuid)
                    {
                        if (_hostUser != user)
                        {
                            LogHelper.Debug("HostUser Changed : {0}", user);
                        }
                        _hostUser = user;
                    }
                }
            }
        }

        internal void Open()
        {
            //进入战场！
            _eRoomState = ERoomState.Open;
        }

        internal void WarnningHost()
        {
            if (_hostUser == null)
            {
                return;
            }
            if (_hostUser.Guid != LocalUser.Instance.UserGuid)
            {
                return;
            }
            //警告自己
            LogHelper.Debug("WarnningHost");
        }
    }
}
