/********************************************************************
** Filename : PlayerManager
** Author : Dong
** Date : 2017/6/20 星期二 下午 2:32:45
** Summary : PlayerManager
***********************************************************************/

using System;
using System.Collections.Generic;

namespace GameA.Game
{
    public class PlayerManager : IDisposable
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance
        {
            get { return _instance ?? (_instance = new PlayerManager()); }
        }

        private List<RoomUser> _userDataList;
        private List<PlayerBase> _playerList = new List<PlayerBase>(10);
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

        public void Add(PlayerBase player)
        {
            if (_userDataList == null)
            {
                RoomUser roomUser = new RoomUser();
                roomUser.Init(LocalUser.Instance.UserGuid, null, true);
                player.Set(roomUser);
                player.Setup(GM2DGame.Instance.GameMode.GetMainPlayerInput());
            }
            else
            {
                player.Set(_userDataList[_playerList.Count]);
                player.Setup(player.IsMain
                    ? GM2DGame.Instance.GameMode.GetMainPlayerInput()
                    : GM2DGame.Instance.GameMode.GetOtherPlayerInput());
            }
            _playerList.Add(player);
            if (player.IsMain)
            {
                _mainPlayer = player as MainPlayer;
            }
        }

        public void Dispose()
        {
            _playerList.Clear();
            _mainPlayer = null;
            _instance = null;
        }
    }
}
