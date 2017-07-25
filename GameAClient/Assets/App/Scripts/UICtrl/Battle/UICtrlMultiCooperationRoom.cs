using System;
using GameA.Game;
using SoyEngine;

namespace GameA
{
    [UIAutoSetup]
    public class UICtrlMultiCooperationRoom: UICtrlGenericBase<UIViewMultiCooperationRoom>
    {
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshView();
        }

        public void RefreshView()
        {
            Room room = RoomManager.Instance.Room;
            _cachedView.RoomIdText.text = "" + room.Id;
            if (room.Users.Count > 0)
            {
                RoomUser roomUser = room.Users[0];
                DictionaryTools.SetContentText(_cachedView.User1NameText, getUserString(room, roomUser));
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.User1NameText, "空");
            }
            if (room.Users.Count > 1)
            {
                RoomUser roomUser = room.Users[1];
                DictionaryTools.SetContentText(_cachedView.User2NameText, getUserString(room, roomUser));
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.User2NameText, "空");
            }
            if (room.HostUser != null && room.HostUser.Guid == LocalUser.Instance.UserGuid)
            {
                _cachedView.ReadyBtn.gameObject.SetActive(false);
                _cachedView.CancelBtn.gameObject.SetActive(false);
                _cachedView.StartBtn.gameObject.SetActive(true);
            }
            else
            {
                RoomUser roomUser;
                if (room.TryGetUser(LocalUser.Instance.UserGuid, out roomUser))
                {
                    if (roomUser.Ready)
                    {
                        _cachedView.ReadyBtn.gameObject.SetActive(false);
                        _cachedView.CancelBtn.gameObject.SetActive(true);
                        _cachedView.StartBtn.gameObject.SetActive(false);
                    }
                    else
                    {
                        _cachedView.ReadyBtn.gameObject.SetActive(true);
                        _cachedView.CancelBtn.gameObject.SetActive(false);
                        _cachedView.StartBtn.gameObject.SetActive(false);
                    }
                }
                else
                {
                    LogHelper.Error(GetType().Name + ".RefreshView LocalUser roomUser is null");
                }
            }
        }

        private String getUserString(Room room, RoomUser roomUser)
        {
            String stateStr;
            if (room.HostUser != null && room.HostUser.Guid == roomUser.Guid)
            {
                stateStr = "房主";
            }
            else
            {
                stateStr = roomUser.Ready ? "已准备" : "未准备";
            }
            return roomUser.Name + "|" + stateStr;
        }


        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ReadyBtn.onClick.AddListener(OnReadyBtnClick);
            _cachedView.CancelBtn.onClick.AddListener(OnCancelReadyBtnClick);
            _cachedView.StartBtn.onClick.AddListener(OnStartBattleBtnClick);
            _cachedView.ExitBtn.onClick.AddListener(OnExitRoomBtnClick);
        }

        private void OnReadyBtnClick()
        {
            RoomManager.Instance.Room.SetUserReady(true);
        }

        private void OnCancelReadyBtnClick()
        {
            RoomManager.Instance.Room.SetUserReady(false);
        }

        private void OnStartBattleBtnClick()
        {
            if (!RoomManager.Instance.Room.CanStart())
            {
                return;
            }
            RoomManager.Instance.Room.SetUserReady(true);
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在开始");
        }

        private void OnExitRoomBtnClick()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在退出房间");
            RoomManager.Instance.Room.SelfExit(() =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在退出房间");
                SocialGUIManager.Instance.CloseUI<UICtrlMultiCooperationRoom>();
                SocialGUIManager.Instance.OpenUI<UICtrlMultiCooperationLobby>();
            }, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在退出房间");
                SocialGUIManager.ShowPopupDialog("退出房间失败");
            });
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnRoomInfoChanged, OnRoomInfoChanged);
            RegisterEvent(EMessengerType.OnRoomPlayerReadyChanged, OnRoomInfoChanged);
            RegisterEvent<long>(EMessengerType.OnRoomPlayerEnter, OnUserInfoChanged);
            RegisterEvent<long>(EMessengerType.OnRoomPlayerExit, OnUserInfoChanged);
            RegisterEvent(EMessengerType.OnRoomWarnningHost, OnRoomWarnningHost);
            RegisterEvent(EMessengerType.OnOpenBattle, OnOpenBattle);
        }

        private void OnOpenBattle()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        }

        private void OnRoomWarnningHost()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
        }

        private void OnRoomInfoChanged()
        {
            if (!_isViewCreated)
            {
                return;
            }
            RefreshView();
        }

        private void OnUserInfoChanged(long userId)
        {
            if (!_isViewCreated)
            {
                return;
            }
            RefreshView();
        }
        
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }
    }
}