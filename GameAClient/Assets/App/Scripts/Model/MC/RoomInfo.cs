using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class RoomInfo
    {
        private long _roomId;
        private long _projectId;
        private int _maxUserCount;
        private long _createTime;
        private long _hostUserId;
        private Project _project; //可能为空
        private List<RoomUser> _users;
        private bool _requestFinish;
        private RoomUser[] _roomUserArray = new RoomUser[TeamManager.MaxTeamCount];

        public long RoomId
        {
            get { return _roomId; }
        }

        public long ProjectId
        {
            get { return _projectId; }
        }

        public int MaxUserCount
        {
            get { return _maxUserCount; }
        }

        public long CreateTime
        {
            get { return _createTime; }
        }

        public Project Project
        {
            get { return _project; }
        }

        public bool RequestFinish
        {
            get { return _requestFinish; }
        }

        public object UserCount
        {
            get { return _users.Count; }
        }

        public List<RoomUser> Users
        {
            get { return _users; }
        }

        public RoomUser[] RoomUserArray
        {
            get { return _roomUserArray; }
        }

        public long HostUserId
        {
            get { return _hostUserId; }
        }

        public RoomInfo(Msg_MC_RoomInfo msg)
        {
            if (null == msg) return;
            _roomId = msg.RoomGuid;
            _projectId = msg.ProjectGuid;
            _maxUserCount = msg.MaxUserCount;
            _createTime = msg.CreateTime;
            _users = new List<RoomUser>(msg.Users.Count);
            for (int i = 0; i < msg.Users.Count; i++)
            {
                _users.Add(new RoomUser(msg.Users[i]));
            }

            ProjectManager.Instance.GetDataOnAsync(_projectId, value =>
            {
                _project = value;
                _requestFinish = true;
                Messenger<long>.Broadcast(EMessengerType.OnRoomProjectInfoFinish, _roomId);
            });
        }

        public RoomInfo(Msg_RC_RoomInfo msg)
        {
            if (null == msg) return;
            _roomId = msg.RoomId;
            _projectId = msg.ProjectId;
            _maxUserCount = msg.MaxUserCount;
            _hostUserId = msg.HostUserId;
            _users = new List<RoomUser>(msg.Users.Count);
            for (int i = 0; i < msg.Users.Count; i++)
            {
                _users.Add(new RoomUser(msg.Users[i]));
            }

            SortRoomUsers();
            ProjectManager.Instance.GetDataOnAsync(_projectId, value =>
            {
                _project = value;
                _requestFinish = true;
                Messenger<long>.Broadcast(EMessengerType.OnRoomProjectInfoFinish, _roomId);
            });
        }

        private void SortRoomUsers()
        {
            for (int i = 0; i < _roomUserArray.Length; i++)
            {
                _roomUserArray[i] = null;
            }

            for (int i = 0; i < _users.Count; i++)
            {
                int index = _users[i].Inx;
                if (index >= _roomUserArray.Length)
                {
                    LogHelper.Error("index >= _roomUsers.Length");
                    continue;
                }

                if (_roomUserArray[index] == null)
                {
                    _roomUserArray[index] = _users[i];
                }
                else
                {
                    LogHelper.Error("index {0} of _roomUserArray is not null", index);
                }
            }
        }

        public void OnUserLeave(long userId)
        {
            RoomUser user = _users.Find(p => p.Guid == userId);
            if (user != null)
            {
                if (user.Player != null)
                {
                    PlayMode.Instance.DeleteUnit(user.Player);
                }

                _users.Remove(user);
                SortRoomUsers();
            }
        }

        public void OnUserExit(Msg_RC_UserExit msg)
        {
            _hostUserId = msg.HostUserGuid;
            OnUserLeave(msg.UserGuid);
        }

        public void OnRoomChangePos(Msg_RC_ChangePos msg)
        {
            RoomUser user = _users.Find(p => p.Guid == msg.UserGuid);
            if (user != null)
            {
                int oriInx = user.Inx;
                int targetInx = msg.PosInx;
                if (_roomUserArray[targetInx] != null)
                {
                    LogHelper.Error("OnRoomChangePos, but targetPos != null");
                }

                user.Inx = targetInx;
                _roomUserArray[targetInx] = _roomUserArray[oriInx];
                _roomUserArray[oriInx] = null;
                if (user.Player != null)
                {
                    var dic = TeamManager.Instance.GetPlayerUnitExtraDic();
                    UnitExtraDynamic unitExtra;
                    if (dic.TryGetValue(targetInx, out unitExtra))
                    {
                        user.Player.SetUnitExtra(unitExtra);
                    }

                    user.Player.Reset();
                    user.Player.OnPlay();
                }
            }
        }

        public void OnRoomPlayerReadyChanged(Msg_RC_UserReadyInfo msg)
        {
            RoomUser user = _users.Find(p => p.Guid == msg.UserGuid);
            if (user != null)
            {
                user.Ready = msg.ReadyFlag;
            }

            Messenger<bool>.Broadcast(EMessengerType.OnRoomPlayerAllReadyChanged, user.Ready && CheckAllReady());
        }

        private bool CheckAllReady()
        {
            for (int i = 0; i < _users.Count; i++)
            {
                if (!_users[i].Ready && _users[i].Guid != _hostUserId)
                {
                    return false;
                }
            }

            return true;
        }

        public void OnRoomUserEnter(Msg_RC_RoomUserInfo msg)
        {
            RoomUser user = new RoomUser(msg);
            _users.Add(user);
            SortRoomUsers();
            if (PlayMode.Instance.HasCreatedPlayer)
            {
                user.Player = PlayMode.Instance.AddPlayer(msg.inx, false);
            }
        }
    }
}