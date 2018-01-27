/********************************************************************
** Filename : PlayerManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:32:45
** Summary : PlayerManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;

namespace GameA.Game
{
    public class PlayerManager : IDisposable
    {
        private static PlayerManager _instance;

        public static PlayerManager Instance
        {
            get { return _instance ?? (_instance = new PlayerManager()); }
        }

        public const int MaxTeamCount = 6;
        private List<PlayerBase> _playerList = new List<PlayerBase>(MaxTeamCount); // 可能赋null，需要判空
        private MainPlayer _mainPlayer;
        private RoomInfo _roomInfo;

        public MainPlayer MainPlayer
        {
            get { return _mainPlayer; }
        }

        public List<PlayerBase> PlayerList
        {
            get { return _playerList; }
        }

        public RoomUser[] RoomUsers
        {
            get
            {
                if (_roomInfo == null)
                {
                    return null;
                }

                return _roomInfo.RoomUserArray;
            }
        }

        public RoomInfo RoomInfo
        {
            get { return _roomInfo; }
        }

        public void Add(PlayerBase player, int roomInx = 0)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti)
            {
                RoomUser roomUser = new RoomUser();
                if (GM2DGame.Instance.EGameRunMode == EGameRunMode.PlayRecord)
                {
                    var user = GM2DGame.Instance.GameMode.Record.UserInfo;
                    roomUser.Init(user.UserId, user.NickName, true);
                }
                else
                {
                    roomUser.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserInfoSimple.NickName, true);
                }

                player.Set(roomUser);
                player.Setup(GM2DGame.Instance.GameMode.GetMainPlayerInput());
            }
            else
            {
                if (GM2DGame.Instance.EGameRunMode == EGameRunMode.Edit)
                {
                    RoomUser roomUser = new RoomUser();
                    roomUser.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserInfoSimple.NickName, true);
                    player.Set(roomUser);
                    player.Setup(GM2DGame.Instance.GameMode.GetMainPlayerInput());
                }
                else
                {
                    var userArray = RoomUsers;
                    if (roomInx < userArray.Length && userArray[roomInx] != null)
                    {
                        player.Set(userArray[roomInx]);
                    }
                    else
                    {
                        LogHelper.Error("roomUser is null");
                        return;
                    }

                    player.Setup(player.IsMain
                        ? GM2DGame.Instance.GameMode.GetMainPlayerInput()
                        : GM2DGame.Instance.GameMode.GetOtherPlayerInput());   
                }
            }

            _playerList.Add(player);
            if (player.IsMain)
            {
                _mainPlayer = player as MainPlayer;
            }
        }

        public PlayerBase GetPlayerByRoomIndex(int index)
        {
            if (_roomInfo != null)
            {
                var roomUsers = _roomInfo.RoomUserArray;
                if (index < roomUsers.Length && roomUsers[index] != null)
                {
                    return roomUsers[index].Player;
                }
            }

            return null;
        }

        public void Reset()
        {
            _playerList.Clear();
            _mainPlayer = null;
        }

        public void Dispose()
        {
            Reset();
            _instance = null;
        }

        public bool CheckAllPlayerSiTouLe()
        {
            for (int i = 0; i < _playerList.Count; i++)
            {
                if (_playerList[i] == null || !_playerList[i].SiTouLe)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            _roomInfo = roomInfo;
        }
    }
}