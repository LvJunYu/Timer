using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class ChatData
    {
        private const int ChatListHighWaterLine = 60;
        private const int ChatListLowWaterLine = 30;
        private const long RoomChatCDTimeTick = 1 * GameTimer.Second2Ticks;
        private const long TeamChatCDTimeTick = 1 * GameTimer.Second2Ticks;
        private const long WorldChatCDTimeTick = 15 * GameTimer.Second2Ticks;
        private const string FrequentlyWarning = "您的发言太快，请{0}秒后再发送";

        public event Action<EChatType, Item> OnChatListAppend;
        public event Action<EChatType> OnChatListCutHead;


        private List<Item> _allChatList = new List<Item>();
        private List<Item> _worldChatList = new List<Item>();
        private List<Item> _worldInviteChatList = new List<Item>();
        private List<Item> _systemChatList = new List<Item>();
        private List<Item> _roomChatList = new List<Item>();

        private GameTimer _roomCDTimer = new GameTimer();
        private GameTimer _teamCDTimer = new GameTimer();
        private GameTimer _worldCDTimer = new GameTimer();

        public List<Item> AllChatList
        {
            get { return _allChatList; }
        }

        public List<Item> WorldChatList
        {
            get { return _worldChatList; }
        }

        public List<Item> WorldInviteChatList
        {
            get { return _worldInviteChatList; }
        }

        public List<Item> SystemChatList
        {
            get { return _systemChatList; }
        }

        public List<Item> RoomChatList
        {
            get { return _roomChatList; }
        }

        public ChatData()
        {
            _roomCDTimer.Zero();
            _worldCDTimer.Zero();
        }

        public bool SendRoomChat(string data)
        {
            if (!_roomCDTimer.PassedTicks(RoomChatCDTimeTick))
            {
                return false;
            }

            Msg_CR_RoomChat msg = new Msg_CR_RoomChat();
            msg.Data = data;
            RoomManager.Instance.SendToRSServer(msg);
            return true;
        }
        
        public bool SendTeamChat(string data)
        {
            if (!_teamCDTimer.PassedTicks(TeamChatCDTimeTick))
            {
                return false;
            }

            Msg_CM_Chat msg = new Msg_CM_Chat();
            msg.ChatType = ECMChatType.CMCT_TeamChat;
            msg.Data = data;
            RoomManager.Instance.SendToMSServer(msg);
            return true;
        }

        public bool SendWorldChat(string data)
        {
            if (!_worldCDTimer.PassedTicks(WorldChatCDTimeTick))
            {
                AddLocalSystemChat(string.Format(FrequentlyWarning, GetWorldLeftSecond()));
                return false;
            }

            Msg_CM_Chat msg = new Msg_CM_Chat();
            msg.ChatType = ECMChatType.CMCT_WorldChat;
            msg.Data = data;
            RoomManager.Instance.SendToMSServer(msg);
            return true;
        }

        private int GetWorldLeftSecond()
        {
            return (int) (WorldChatCDTimeTick / GameTimer.Second2Ticks - _worldCDTimer.GetIntervalSeconds());
        }

        public void AddLocalSystemChat(string data)
        {
            var msg = new Msg_MC_Chat();
            msg.Data = data;
            msg.ChatType = ECMChatType.CMCT_System;
            var item = new Item(msg);
            AddChatItem(item);
        }

        public bool SendWorldInvite(long roomId)
        {
            if (!_worldCDTimer.PassedTicks(WorldChatCDTimeTick))
            {
                AddLocalSystemChat(string.Format(FrequentlyWarning, GetWorldLeftSecond()));
                return false;
            }

            Msg_CM_Chat msg = new Msg_CM_Chat();
            msg.ChatType = ECMChatType.CMCT_WorldInvite;
            msg.Param = roomId;
            RoomManager.Instance.SendToMSServer(msg);
            return true;
        }

        public void ClearRoomChat()
        {
            _roomChatList.Clear();
            FireCutHeadEvent(EChatType.Room);
        }

        public void OnRCChat(Msg_RC_RoomChat msgChat)
        {
            var item = new Item(msgChat);
            AddChatItem(item);
        }

        public void OnMCChat(Msg_MC_Chat msgChat)
        {
            var item = new Item(msgChat);
            AddChatItem(item);
        }

        private void AddChatItem(Item item)
        {
            AddChatItem(EChatType.All, item);
            AddChatItem(item.ChatType, item);
        }

        private void AddChatItem(EChatType chatType, Item item)
        {
            var list = GetList(chatType);
            list.Add(item);
            FireAppendEvent(chatType, item);
            if (list.Count >= ChatListHighWaterLine)
            {
                list.RemoveRange(0, ChatListHighWaterLine - ChatListLowWaterLine);
                FireCutHeadEvent(chatType);
            }
        }

        public List<Item> GetList(EChatType chatType)
        {
            switch (chatType)
            {
                case EChatType.All:
                    return _allChatList;
                case EChatType.World:
                    return _worldChatList;
                case EChatType.WorldInvite:
                    return _worldInviteChatList;
                case EChatType.System:
                    return _systemChatList;
                case EChatType.Room:
                    return _roomChatList;
                default:
                    throw new ArgumentOutOfRangeException("chatType", chatType, null);
            }
        }

        private void FireAppendEvent(EChatType chatType, Item item)
        {
            if (OnChatListAppend != null)
            {
                OnChatListAppend.Invoke(chatType, item);
            }
        }

        private void FireCutHeadEvent(EChatType chatType)
        {
            if (OnChatListCutHead != null)
            {
                OnChatListCutHead.Invoke(chatType);
            }
        }

        public class Item
        {
            private ChatUser _chatUser;
            private EChatType _chatType;
            private DateTime _time;
            private string _content;
            private long _param;

            public ChatUser ChatUser
            {
                get { return _chatUser; }
            }

            public EChatType ChatType
            {
                get { return _chatType; }
            }

            public DateTime Time
            {
                get { return _time; }
            }

            public string Content
            {
                get { return _content; }
            }

            public long Param
            {
                get { return _param; }
            }

            public Item(Msg_RC_RoomChat msg)
            {
                var roomUser = PlayerManager.Instance.RoomInfo.GetRoomUserByGuid(msg.PlayerId);
                if (roomUser != null)
                {
                    _chatUser = new ChatUser(roomUser.Guid, roomUser.Name);
                }
                else
                {
                    _chatUser = new ChatUser(msg.PlayerId, "" + msg.PlayerId);
                }

                _chatType = EChatType.Room;
                _content = msg.Data;
                _time = DateTime.Now;
            }

            public Item(Msg_MC_Chat msg)
            {
                switch (msg.ChatType)
                {
                    case ECMChatType.CMCT_None:
                        LogHelper.Warning("ChatItem type error");
                        break;
                    case ECMChatType.CMCT_WorldChat:
                        _chatType = EChatType.World;
                        break;
                    case ECMChatType.CMCT_WorldInvite:
                        _chatType = EChatType.WorldInvite;
                        break;
                    case ECMChatType.CMCT_System:
                        _chatType = EChatType.System;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _content = msg.Data;
                _param = msg.Param;
                _time = DateTime.Now;
                if (msg.UserInfo != null)
                {
                    _chatUser = new ChatUser(msg.UserInfo.UserGuid, msg.UserInfo.NickName);
                }
            }
        }

        public class ChatUser
        {
            private long _userGuid;
            private string _userNickName;

            public long UserGuid
            {
                get { return _userGuid; }
            }

            public string UserNickName
            {
                get { return _userNickName; }
            }

            public ChatUser(long userGuid, string userNickName)
            {
                _userGuid = userGuid;
                _userNickName = userNickName;
            }
        }

        public enum EChatType
        {
            All,
            World,
            System,
            WorldInvite,
            Room,
            Team
        }
    }
}