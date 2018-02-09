using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class MultiBattleData
    {
        private const string RefuseInviteKey = "RefuseTeamInvite";
        private const int RefuseSecond = 600;
        private const int MaxCacheSecond = 60;
        private const int MaxInviteCache = 5;
        private float _refuseTeamTime = -RefuseSecond;
        private float _refuseRoomTime = -RefuseSecond;
        private Msg_MC_TeamInfo _teamInfo;
        private List<long> _selectedOfficalProjectList = new List<long>();
        private bool _isMyTeam;
        private Queue<LocalTeamInvite> _teamInviteStack = new Queue<LocalTeamInvite>(MaxInviteCache);
        private Queue<LocalRoomInvite> _roomInviteStack = new Queue<LocalRoomInvite>(MaxInviteCache);
        private RoomChatPreinstallList _chatPreinstallList = new RoomChatPreinstallList();

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
            get { return Time.time - _refuseTeamTime < RefuseSecond; }
            set
            {
                if (value)
                {
                    _refuseTeamTime = Time.time;
                }
                else
                {
                    _refuseTeamTime = -RefuseSecond;
                }
            }
        }

        public bool RefuseRoomInviteInMins
        {
            get { return Time.time - _refuseRoomTime < RefuseSecond; }
            set
            {
                if (value)
                {
                    _refuseRoomTime = Time.time;
                }
                else
                {
                    _refuseRoomTime = -RefuseSecond;
                }
            }
        }

        public List<LocalTeamInvite> TeamInviteList
        {
            get
            {
                while (_teamInviteStack.Count > 0 && Time.time - _teamInviteStack.Peek().CreateTime > MaxCacheSecond)
                {
                    _teamInviteStack.Dequeue();
                }

                var list = new List<LocalTeamInvite>();
                while (_teamInviteStack.Count > 0)
                {
                    var invite = _teamInviteStack.Dequeue();
                    //去掉同一个人的邀请
                    int index = list.FindIndex(p => p.Msg.Inviter.UserGuid == invite.Msg.Inviter.UserGuid);
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                    }
                    list.Add(invite);
                }
                list.Reverse();
                return list;
            }
        }

        public List<LocalRoomInvite> RoomInviteStack
        {
            get
            {
                while (_roomInviteStack.Count > 0 && Time.time - _roomInviteStack.Peek().CreateTime > MaxCacheSecond)
                {
                    _roomInviteStack.Dequeue();
                }
                var list = new List<LocalRoomInvite>();
                while (_roomInviteStack.Count > 0)
                {
                    var invite = _roomInviteStack.Dequeue();
                    //去掉同一个人的邀请
                    int index = list.FindIndex(p => p.Msg.Inviter.UserGuid == invite.Msg.Inviter.UserGuid);
                    if (index >= 0)
                    {
                        list.RemoveAt(index);
                    }
                    list.Add(invite);
                }
                list.Reverse();
                return list;
            }
        }

        public RoomChatPreinstallList ChatPreinstallList
        {
            get { return _chatPreinstallList; }
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
                if (LocalUser.Instance.UserGuid == msg.UserId)
                {
                    if (msg.Reason == EMCExitTeamReason.MCETR_Kicked)
                    {
                        if (SocialGUIManager.Instance.CurrentMode != SocialGUIManager.EMode.Game)
                        {
                            SocialGUIManager.ShowPopupDialog("您被踢出队伍", null,
                                new KeyValuePair<string, Action>("确定", OnLeaveTeam));
                        }
                        else
                        {
                            OnLeaveTeam();
                        }
                    }
                    else if (msg.Reason == EMCExitTeamReason.MCETR_Disconnect)
                    {
                        SocialGUIManager.ShowPopupDialog("已经失去连接", null,
                            new KeyValuePair<string, Action>("确定", OnLeaveTeam));
                    }
                    else
                    {
                        OnLeaveTeam();
                    }
                }
                else if (msg.UserId == _teamInfo.UserList[0].UserGuid)
                {
                    SocialGUIManager.ShowPopupDialog("队长已解散队伍", null,
                        new KeyValuePair<string, Action>("确定", OnLeaveTeam));
                }
                else
                {
                    _teamInfo.UserList.Remove(user);
                    Messenger.Broadcast(EMessengerType.OnTeamUserChanged);
                }
            }
        }

        private void OnLeaveTeam()
        {
            _teamInfo = null;
            Messenger.Broadcast(EMessengerType.OnLeaveTeam);
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

            _roomInviteStack.Enqueue(new LocalRoomInvite(msg));
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

            _teamInviteStack.Enqueue(new LocalTeamInvite(msg));
            Messenger.Broadcast(EMessengerType.OnTeamInviteChanged);
            Messenger<Msg_MC_TeamInvite>.Broadcast(EMessengerType.OnTeamInviteChanged, msg);
        }

        private void JoinTeam(Msg_MC_JoinTeam msg)
        {
            _teamInfo = msg.TeamInfo;
            _isMyTeam = false;
            Messenger.Broadcast(EMessengerType.OnInTeam);
            SocialGUIManager.Instance.OpenUI<UICtrlMultiBattle>();
        }

        public void OnJoinTeam(Msg_MC_JoinTeam msg)
        {
            if (msg.ResultCode == EMCJoinTeamCode.MCJT_Success)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlInvitedByFriend>();
                if (SocialGUIManager.Instance.CurrentMode == SocialGUIManager.EMode.Game)
                {
                    GM2DGame.Instance.QuitGame(() => JoinTeam(msg), null);
                }
                else
                {
                    JoinTeam(msg);
                }
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

    public class LocalTeamInvite
    {
        public Msg_MC_TeamInvite Msg;
        public float CreateTime;

        public LocalTeamInvite(Msg_MC_TeamInvite msg)
        {
            Msg = msg;
            CreateTime = Time.time;
        }
    }

    public class LocalRoomInvite
    {
        public Msg_MC_RoomInvite Msg;
        public float CreateTime;

        public LocalRoomInvite(Msg_MC_RoomInvite msg)
        {
            Msg = msg;
            CreateTime = Time.time;
        }
    }
}