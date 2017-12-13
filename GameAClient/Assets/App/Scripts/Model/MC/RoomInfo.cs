using System.Collections.Generic;
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
        private Project _project;
        private List<Msg_MC_RoomUserInfo> _users;
        private bool _requestFinish;

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

        public RoomInfo(Msg_MC_RoomInfo msg)
        {
            if (null == msg) return;
            _roomId = msg.RoomGuid;
            _projectId = msg.ProjectGuid;
            _maxUserCount = msg.MaxUserCount;
            _createTime = msg.CreateTime;
            _users = msg.Users;
            _project = new Project();
            _project.Request(_projectId, () =>
            {
                _requestFinish = true;
                Messenger<long>.Broadcast(EMessengerType.OnRoomProjectInfoFinish, _roomId);
            }, null);
        }
    }
}