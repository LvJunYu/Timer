/********************************************************************
** Filename : PlayerManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:32:45
** Summary : PlayerManager
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA.Game
{
    public class PlayerManager : IDisposable
    {
        private static PlayerManager _instance;

        public static PlayerManager Instance
        {
            get { return _instance ?? (_instance = new PlayerManager()); }
        }

        private List<RoomUser> _userDataList = new List<RoomUser>(6);
        private List<PlayerBase> _playerList = new List<PlayerBase>(10); // 可能赋null，需要判空
        private MainPlayer _mainPlayer;

        public MainPlayer MainPlayer
        {
            get { return _mainPlayer; }
        }

        public List<RoomUser> UserDataList
        {
            get { return _userDataList; }
        }

        public List<PlayerBase> PlayerList
        {
            get { return _playerList; }
        }

        public void SetUserData(List<RoomUser> users)
        {
            _userDataList = users;
        }

        public void JoinRoom(Msg_RC_RoomUserInfo msg)
        {
            var user = new RoomUser();
            user.Init(msg.UserGuid, msg.NickName, true);
            _userDataList.Add(user);
            _playerList.Add(null);
        }

        public PlayerBase GetPlayerById(int id)
        {
            return _playerList.Find(p => p.Id == id);
        }

        public PlayerBase GetPlayerByInx(int inx)
        {
            if (inx < 0 || inx >= _playerList.Count)
            {
                return null;
            }
            return _playerList[inx];
        }

        public RoomUser GetRoomUserByInx(int inx)
        {
            return _userDataList[inx];
        }

        public void Add(PlayerBase player, int roomInx = 0)
        {
            if (!GM2DGame.Instance.GameMode.IsMulti)
            {
                RoomUser roomUser = new RoomUser();
                roomUser.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserInfoSimple.NickName, true);
                player.Set(roomUser);
                player.Setup(GM2DGame.Instance.GameMode.GetMainPlayerInput());
            }
            else
            {
                if (roomInx < _userDataList.Count)
                {
                    player.Set(_userDataList[roomInx]);
                }
                else
                {//EditTest
                    RoomUser roomUser = new RoomUser();
                    roomUser.Init(LocalUser.Instance.UserGuid, LocalUser.Instance.User.UserInfoSimple.NickName, true);
                    player.Set(roomUser);
                }
                player.Setup(player.IsMain
                    ? GM2DGame.Instance.GameMode.GetMainPlayerInput()
                    : GM2DGame.Instance.GameMode.GetOtherPlayerInput());
            }
            if (roomInx < _playerList.Count)
            {
                _playerList[roomInx] = player;
            }
            else
            {
                _playerList.Add(player);
            }
            if (player.IsMain)
            {
                if (_mainPlayer != null)
                {
                    PlayMode.Instance.DestroyUnit(_mainPlayer);
                }
                _mainPlayer = player as MainPlayer;
            }
        }

        public void Reset()
        {
            _playerList.Clear();
            _mainPlayer = null;
        }

        public void Dispose()
        {
            _playerList.Clear();
            _mainPlayer = null;
            _instance = null;
        }

        public void AddGhost(MainPlayer playerBase)
        {
            playerBase.Setup(GM2DGame.Instance.GameMode.GetOtherPlayerInput());
            playerBase.SetPos(new IntVec2(1, 1));
            _mainPlayer = playerBase;
        }
    }
}