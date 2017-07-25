/********************************************************************
** Filename : Room
** Author : Dong
** Date : 2017/6/20 星期二 下午 4:56:18
** Summary : Room
***********************************************************************/

using System;
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
        Created,
        OpenBattle,
    }

    public enum EBattleState
    {
        None,
        Open,
        Wait,
        Start,
        Fight,
        Close
    }

    public class Room
    {
        protected ERoomState _eRoomState;
        protected EBattleType _eBattleType;
        protected long _id;
        protected long _projectId;
        protected RoomUser _hostUser;
        protected List<RoomUser> _users = new List<RoomUser>();
        private Action _successCallback;
        private Action _failCallback;

        public long Id
        {
            get { return _id; }
        }

        public long ProjectId
        {
            get { return _projectId; }
        }

        public ERoomState ERoomState
        {
            get { return _eRoomState; }
        }

        public List<RoomUser> Users
        {
            get { return _users; }
        }

        public EBattleType EBattleType
        {
            get { return _eBattleType; }
        }

        public RoomUser HostUser
        {
            get { return _hostUser; }
        }

        public void Clear()
        {
            _eRoomState = ERoomState.None;
            _eBattleType = EBattleType.EBT_None;
            _projectId = 0;
            _id = 0;
            _hostUser = null;
            _users.Clear();
            _failCallback = null;
            _successCallback = null;
        }

        public bool TryGetUser(long userGuid, out RoomUser user)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                var cur = _users[i];
                if (cur != null && cur.Guid == userGuid)
                {
                    user = cur;
                    return true;
                }
            }
            user = null;
            return false;
        }

        public RoomUser GetLocalUser()
        {
            RoomUser roomUser;
            if (TryGetUser(LocalUser.Instance.UserGuid, out roomUser))
            {
                return roomUser;
            }
            return null;
        }

        public void Create(EBattleType eBattleType, long projectId,  Action successCallback, Action failCallback)
        {
            if (_eRoomState != ERoomState.None)
            {
                LogHelper.Warning("Room.Create RoomState: {0}", _eRoomState);
                return;
            }
            _successCallback = successCallback;
            _failCallback = failCallback;
            _eRoomState = ERoomState.RequestCreate;
            RoomManager.Instance.SendRequestCreateRoom(eBattleType, projectId);
        }

        public bool OnCreateSuccess(RoomUser user,long roomGuid, long projectGuid, EBattleType eBattleType)
        {
            if (_eRoomState != ERoomState.RequestCreate)
            {
                LogHelper.Warning("Room.OnCreateSuccess RoomState: {0}", _eRoomState);
                return false;
            }
            _hostUser = user;
            _id = roomGuid;
            _projectId = projectGuid;
            _eBattleType = eBattleType;
            _eRoomState = ERoomState.Created;
            if (_successCallback != null)
            {
                _successCallback.Invoke();
            }
            _successCallback = null;
            _failCallback = null;
            return true;
        }

        public bool OnCreateFailed()
        {
            if (_eRoomState != ERoomState.RequestCreate)
            {
                LogHelper.Warning("Room.OnCreateFailed RoomState: {0}", _eRoomState);
                return false;
            }
            _eRoomState = ERoomState.None;
            if (_failCallback != null)
            {
                _failCallback.Invoke();
            }
            _successCallback = null;
            _failCallback = null;
            return true;
        }

        public void Join(long roomId, Action successCallback, Action failCallback)
        {
            
            if (_eRoomState != ERoomState.None)
            {
                LogHelper.Warning("Room.Join RoomState: {0}", _eRoomState);
                return;
            }
            _successCallback = successCallback;
            _failCallback = failCallback;
            _eRoomState = ERoomState.RequestJoin;
            RoomManager.Instance.SendRequestJoinRoom(roomId);
        }
        
        public bool OnJoinSuccess()
        {
            if (_eRoomState != ERoomState.RequestJoin)
            {
                LogHelper.Warning("Room.OnJoinSuccess RoomState: {0}", _eRoomState);
                return false;
            }
            _eRoomState = ERoomState.Created;
            if (_successCallback != null)
            {
                _successCallback.Invoke();
            }
            _successCallback = null;
            _failCallback = null;
            return true;
        }

        public bool OnJoinFailed()
        {
            if (_eRoomState != ERoomState.RequestJoin)
            {
                LogHelper.Warning("Room.OnJoinFailed RoomState: {0}", _eRoomState);
                return false;
            }
            _eRoomState = ERoomState.None;
            if (_failCallback != null)
            {
                _failCallback.Invoke();
            }
            _successCallback = null;
            _failCallback = null;
            return true;
        }

        public void OnRoomInfo(Msg_RC_RoomInfo ret)
        {
            _users.Clear();
            ret.Users.ForEach(msgUser =>
            {
                var user = new RoomUser();
                user.Init(msgUser.UserGuid, msgUser.UserName, msgUser.Ready == 1);
                _users.Add(user);
            });
            SetRoomInfo(ret.HostUserGuid, ret.RoomGuid, ret.ProjectGuid, ret.EBattleType);
            Messenger.Broadcast(EMessengerType.OnRoomInfoChanged);
        }

        public void SetRoomInfo(long hostUserGuid, long roomGuid, long projectId, EBattleType eBattleType)
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (_users[i].Guid == hostUserGuid)
                {
                    _hostUser = _users[i];
                    break;
                }
            }
            _id = roomGuid;
            _projectId = projectId;
            _eBattleType = eBattleType;
        }

        public bool AddUser(RoomUser user)
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
            Messenger<long>.Broadcast(EMessengerType.OnRoomPlayerEnter, user.Guid);
            return true;
        }

        public void SetUserReady(bool flag)
        {
            RoomUser roomUser;
            if (TryGetUser(LocalUser.Instance.UserGuid, out roomUser))
            {
                roomUser.Ready = flag;
                RoomManager.Instance.SendRoomReadyInfo(flag);
            }
        }

        public void OnUserReady(long userGuid, bool flag)
        {
            RoomUser user;
            if (!TryGetUser(userGuid, out user))
            {
                return;
            }
            user.Ready = flag;
            Messenger.Broadcast(EMessengerType.OnRoomPlayerReadyChanged);
        }

        public bool SelfExit(Action successCallback, Action failedCallback)
        {
            if (_eRoomState != ERoomState.Created)
            {
                LogHelper.Warning("Room.SelfExit RoomState: {0}", _eRoomState);
                return false;
            }
            _successCallback = successCallback;
            _failCallback = failedCallback;
            RoomManager.Instance.SendRequestExitRoom(_id);
            return true;
        }
        
        public bool OnSelfExit(bool success)
        {
            if (_eRoomState != ERoomState.Created)
            {
                LogHelper.Warning("Room.OnSelfExit RoomState: {0}", _eRoomState);
                return false;
            }
            if (success)
            {
                if (_successCallback != null)
                {
                    _successCallback.Invoke();
                }
            }
            else
            {
                if (_failCallback != null)
                {
                    _failCallback.Invoke();
                }
            }
            Clear();
            _successCallback = null;
            _failCallback = null;
            return true;
        }
        

        public void OnUserExit(long exitGuid, long hostGuid)
        {
            for (int i = _users.Count - 1; i >= 0; i--)
            {
                var user = _users[i];
                if (user != null)
                {
                    if (user.Guid == exitGuid)
                    {
                        _users.Remove(user);
//                        PoolFactory<RoomUser>.Free(user);
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
            Messenger<long>.Broadcast(EMessengerType.OnRoomPlayerExit, exitGuid);
        }

        public void OnWarnningHost()
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
            Messenger.Broadcast(EMessengerType.OnRoomWarnningHost);
        }

        public void OnOpenBattle()
        {
            //开启战场！
            _eRoomState = ERoomState.OpenBattle;
            Messenger.Broadcast(EMessengerType.OnOpenBattle);
        }

        public bool CanStart()
        {
            if (_users.Count != 2)
            {
                return false;
            }
            foreach (var roomUser in _users)
            {
                if (!roomUser.Ready && roomUser.Guid != LocalUser.Instance.UserGuid)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
