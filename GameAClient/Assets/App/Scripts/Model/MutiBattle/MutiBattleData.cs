using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class MutiBattleData
    {
        private Msg_MC_TeamInfo _teamInfo;
        private List<long> _selectedOfficalProjectList = new List<long>();
        private bool _isMyTeam;

        public Msg_MC_TeamInfo TeamInfo
        {
            get { return _teamInfo; }
            set { _teamInfo = value; }
        }

        public List<long> SelectedOfficalProjectList
        {
            get { return _selectedOfficalProjectList; }
        }

        public bool IsMyTeam
        {
            get { return _isMyTeam; }
        }

        public void OnProjectSelectedChanged(List<long> list, bool value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                OnProjectSelectedChanged(list[i], value);
            }
            Messenger.Broadcast(EMessengerType.OnSelectedOfficalProjectListChanged);
        }

        public void OnProjectSelectedChanged(long id, bool value)
        {
            if (value)
            {
                if (!_selectedOfficalProjectList.Contains(id))
                {
                    _selectedOfficalProjectList.Add(id);
                }
            }
            else
            {
                if (_selectedOfficalProjectList.Contains(id))
                {
                    _selectedOfficalProjectList.Remove(id);
                }
            }
        }

        public void OnCreateTeam(Msg_MC_CreateTeam msg)
        {
            _teamInfo = msg.TeamInfo;
            _isMyTeam = true;
            Messenger.Broadcast(EMessengerType.OnCreateTeam);
        }

        public void OnExitTeam(Msg_MC_ExitTeam msg)
        {
            if (_teamInfo == null)
            {
                return;
            }

            var user = _teamInfo.UserList.Find(u => u.UserGuid == msg.UserId);
            if (user != null)
            {
                _teamInfo.UserList.Remove(user);
                if (msg.UserId == LocalUser.Instance.UserGuid)
                {
                    _teamInfo = null;
                }
                Messenger<Msg_MC_ExitTeam>.Broadcast(EMessengerType.OnTeamerExit, msg);
            }
        }
    }
}