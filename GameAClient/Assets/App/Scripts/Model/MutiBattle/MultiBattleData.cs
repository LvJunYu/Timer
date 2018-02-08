using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class MultiBattleData
    {
        private const string RefuseInviteKey = "RefuseTeamInvite";
        private const int RefuseMins = 10;
        private const int MaxInviteCache = 5;
        private float _refuseTeamTime;
        private float _refuseRoomTime;
        private Msg_MC_TeamInfo _teamInfo;
        private List<long> _selectedOfficalProjectList = new List<long>();
        private bool _isMyTeam;
        private Queue<Msg_MC_TeamInvite> _teamInviteStack = new Queue<Msg_MC_TeamInvite>(MaxInviteCache);
        private Queue<Msg_MC_RoomInvite> _roomInviteStack = new Queue<Msg_MC_RoomInvite>(MaxInviteCache);

        public Msg_MC_TeamInfo TeamInfo
        {
            get { return _teamInfo; }
        }

        public List<long> SelectedOfficalProjectList
        {
            get { return _selectedOfficalProjectList; }
        }

        public bool IsMyTeam
        {
            get { return _isMyTeam; }
        }

        public bool RefuseTeamInvite
        {
            get { return PlayerPrefs.HasKey(RefuseInviteKey) && PlayerPrefs.GetInt(RefuseInviteKey) == 1; }
            set { PlayerPrefs.SetInt(RefuseInviteKey, value ? 1 : 0); }
        }

        public bool RefuseTeamInviteInMins
        {
            get { return Time.time - _refuseTeamTime < RefuseMins; }
            set
            {
                if (value)
                {
                    _refuseTeamTime = Time.time;
                }
                else
                {
                    _refuseTeamTime = -RefuseMins;
                }
            }
        }

        public bool RefuseRoomInviteInMins
        {
            get { return Time.time - _refuseRoomTime < RefuseMins; }
            set
            {
                if (value)
                {
                    _refuseRoomTime = Time.time;
                }
                else
                {
                    _refuseRoomTime = -RefuseMins;
                }
            }
        }

        public Queue<Msg_MC_TeamInvite> TeamInviteStack
        {
            get { return _teamInviteStack; }
        }

        public Queue<Msg_MC_RoomInvite> RoomInviteStack
        {
            get { return _roomInviteStack; }
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
            Messenger.Broadcast(EMessengerType.OnInTeam);
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

        public void OnRoomInvite(Msg_MC_RoomInvite msg)
        {
            if (RefuseRoomInviteInMins)
            {
                return;
            }

            while (_roomInviteStack.Count >= MaxInviteCache)
            {
                _roomInviteStack.Dequeue();
            }
            _roomInviteStack.Enqueue(msg);
            Messenger.Broadcast(EMessengerType.OnRoomInviteChanged);
            Messenger<Msg_MC_RoomInvite>.Broadcast(EMessengerType.OnRoomInviteChanged, msg);
        }

        public void OnTeamInvite(Msg_MC_TeamInvite msg)
        {
            if (RefuseTeamInvite || RefuseTeamInviteInMins)
            {
                return;
            }

            while (_teamInviteStack.Count >= MaxInviteCache)
            {
                _teamInviteStack.Dequeue();
            }

            _teamInviteStack.Enqueue(msg);
            Messenger.Broadcast(EMessengerType.OnTeamInviteChanged);
            Messenger<Msg_MC_TeamInvite>.Broadcast(EMessengerType.OnTeamInviteChanged, msg);
        }

        public void OnJoinTeam(Msg_MC_JoinTeam msg)
        {
            if (msg.ResultCode == EMCJoinTeamCode.MCJT_Success)
            {
                _teamInfo = msg.TeamInfo;
                _isMyTeam = false;
                Messenger.Broadcast(EMessengerType.OnInTeam);
                SocialGUIManager.Instance.OpenUI<UICtrlMultiBattle>();
            }
            else if (msg.ResultCode == EMCJoinTeamCode.MCJT_Full)
            {
                SocialGUIManager.ShowPopupDialog("队伍已满");
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("要求已失效");
            }
        }

        public void OnEnterTeam(Msg_MC_EnterTeam msg)
        {
            if (_teamInfo.UserList.Contains(msg.User))
            {
                LogHelper.Error("OnEnterTeam fail, user has in this team");
                return;
            }

            _teamInfo.UserList.Add(msg.User);
            Messenger.Broadcast(EMessengerType.OnTeamUserChanged);
        }

        public void SyncUserStatusChange(Msg_MC_SyncUserStatusChange msg)
        {
            if (_teamInfo == null)
            {
                return;
            }

            bool hasChanged = false;
            for (int i = 0; i < msg.UserList.Count; i++)
            {
                var index = _teamInfo.UserList.FindIndex(u => u.UserGuid == msg.UserList[i].UserGuid);
                if (index >= 0)
                {
                    _teamInfo.UserList[index] = msg.UserList[i];
                    hasChanged = true;
                }
            }

            if (hasChanged)
            {
                Messenger.Broadcast(EMessengerType.OnTeamUserChanged);
            }
        }
    }
}