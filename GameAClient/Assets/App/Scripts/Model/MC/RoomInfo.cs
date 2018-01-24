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

        public void SortRoomUsers()
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
                    LogHelper.Error("index >= _roomUsers.Length");
                }
            } 
        }
    }
}