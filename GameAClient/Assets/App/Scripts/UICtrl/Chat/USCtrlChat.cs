using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlChat : USCtrlBase<USViewChat>
    {
        private const string UserStr = "user";
        private const string RoomStr = "room";
        private const int HomeLongWidth = 234;
        private const int HomeShorWidth = 164;
        private const int HomeMinHeight = 46;
        private const int RoomLongWidth = 226;
        private const int RoomShorWidth = 152;
        private const int RoomMinHeight = 45;
        private int _inputShortWidth;
        private int _inputLongWidth;
        private int _minHeight;

        private ChatData.EChatType _currentChatTypeTag = ChatData.EChatType.All;
        private ChatData.EChatType _currentSendType = ChatData.EChatType.World;
        private Stack<UMCtrlChat> _umPool = new Stack<UMCtrlChat>(70);
        private List<ChatData.Item> _contentList;
        private List<UMCtrlChat> _umList = new List<UMCtrlChat>(70);
        private EResScenary _resScenary;
        private EScene _scene;
        private USCtrlFriendItem[] _usCtrlFriendItems;
        private UserInfoDetail _curChatUser;
        private UserInfoDetail _curSelectedUser;

        public EResScenary ResScenary
        {
            get { return _resScenary; }
            set { _resScenary = value; }
        }

        public EScene Scene
        {
            get { return _scene; }
            set
            {
                if (value == EScene.Room)
                {
                    _inputLongWidth = RoomLongWidth;
                    _inputShortWidth = RoomShorWidth;
                    _minHeight = RoomMinHeight;
                }
                else
                {
                    _inputLongWidth = HomeLongWidth;
                    _inputShortWidth = HomeShorWidth;
                    _minHeight = HomeMinHeight;
                }

                _scene = value;
            }
        }

        public override void Init(USViewChat view)
        {
            base.Init(view);
            AppData.Instance.ChatData.OnChatListAppend += OnChatListAppend;
            AppData.Instance.ChatData.OnChatListCutHead += OnChatListCutHead;
        }

        public override void OnDestroy()
        {
            AppData.Instance.ChatData.OnChatListAppend -= OnChatListAppend;
            AppData.Instance.ChatData.OnChatListCutHead -= OnChatListCutHead;
            base.OnDestroy();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtnsDockBtn.onClick.AddListener(() => SetBtnsDockOpen(false));
            _cachedView.CloseChatFriendDockBtn.onClick.AddListener(() => SetFriendsDockOpen(false));
            _cachedView.ChatFriendBtn.onClick.AddListener(OnSelectFriendBtn);
            _cachedView.CheckInfoBtn.onClick.AddListener(OnCheckInfoBtn);
            _cachedView.PrivateChatBtn.onClick.AddListener(OnPrivateChatBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);

            _cachedView.SendContentBtn.onClick.AddListener(OnSendBtnClick);
            _cachedView.ChatInput.onEndEdit.AddListener(str =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnSendBtnClick();
                }
            });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.ChatInput);
            for (int i = 0; i < _cachedView.ChatTypeTagArray.Length; i++)
            {
                var tog = _cachedView.ChatTypeTagArray[i];
                var chatType = (ChatData.EChatType) i;
                tog.onValueChanged.AddListener(flag =>
                {
                    if (flag)
                    {
                        SelectChatTypeTag(chatType);
                    }
                });
            }

            _cachedView.ChatTypeBtn.onClick.AddListener(OnSendChatTypeClick);
            var list = _cachedView.ChatFriendsGridRtf.GetComponentsInChildren<USViewFriendItem>();
            _usCtrlFriendItems = new USCtrlFriendItem[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                var usCtrlFriendItem = new USCtrlFriendItem();
                usCtrlFriendItem.Init(list[i]);
                usCtrlFriendItem.AddBtnListener(() =>
                {
                    _curChatUser = usCtrlFriendItem.Data;
                    RefreshFriendsChatView();
                    SetFriendsDockOpen(false);
                });
                _usCtrlFriendItems[i] = usCtrlFriendItem;
            }

            if (_scene == EScene.Room)
            {
                SetSendChatType(ChatData.EChatType.Room);
            }
            else
            {
                SetSendChatType(ChatData.EChatType.World);
            }
        }

        public override void Open()
        {
            base.Open();
            SelectChatTypeTag(_currentChatTypeTag);
            _cachedView.PrivateChatRedPoint.SetActive(_currentChatTypeTag != ChatData.EChatType.Friends &&
                                                      AppData.Instance.ChatData.PrivateChatHasNew);
            SetFriendsDockOpen(false);
            SetBtnsDockOpen(false);
        }

        private void SelectChatTypeTag(ChatData.EChatType chatType)
        {
            _currentChatTypeTag = chatType;
            if (!_isOpen)
            {
                return;
            }

            _contentList = AppData.Instance.ChatData.GetList(chatType);
            RefreshView();
            switch (chatType)
            {
                case ChatData.EChatType.Friends:
                    AppData.Instance.ChatData.PrivateChatHasNew = false;
                    _cachedView.PrivateChatRedPoint.SetActive(false);
                    SetSendChatType(chatType);
                    break;
                case ChatData.EChatType.World:
                case ChatData.EChatType.Room:
                case ChatData.EChatType.Team:
                    SetSendChatType(chatType);
                    break;
            }
        }

        private void SetSendChatType(ChatData.EChatType chatType)
        {
            _currentSendType = chatType;
            bool isFriendChat = _currentSendType == ChatData.EChatType.Friends;
            _cachedView.ChatFriendBtn.SetActiveEx(isFriendChat);
            _cachedView.ChatInput.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                isFriendChat ? _inputShortWidth : _inputLongWidth);
            if (isFriendChat)
            {
                RefreshFriendsChatView();
            }

            switch (chatType)
            {
                case ChatData.EChatType.World:
                    _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "世界";
                    break;
                case ChatData.EChatType.Room:
                    _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "房间";
                    break;
                case ChatData.EChatType.Team:
                    _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "队伍";
                    break;
                case ChatData.EChatType.Friends:
                    _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "私聊";
                    break;
            }
        }

        private void RefreshFriendsChatView()
        {
            if (_curChatUser == null)
            {
                _curChatUser = AppData.Instance.ChatData.CurChatUser;
            }

            _cachedView.ChatFriendTxt.text = _curChatUser == null
                ? string.Empty
                : GameATools.GetRawStr(_curChatUser.UserInfoSimple.NickName, 6);
        }

        private void RefreshBtnsDock(Vector2 pos)
        {
            _cachedView.CheckInfoBtn.SetActiveEx(_scene == EScene.Home);
            _cachedView.FollowBtn.SetActiveEx(_curSelectedUser.UserInfoSimple.UserId != LocalUser.Instance.UserGuid &&
                                              !_curSelectedUser.UserInfoSimple.RelationWithMe.FollowedByMe);
            _cachedView.BtnsGridRtf.position = pos;
            Canvas.ForceUpdateCanvases();
            var curPos = _cachedView.BtnsGridRtf.anchoredPosition;
            curPos.x = Mathf.Clamp(curPos.x, 0,
                _cachedView.BtnsDockRtf.rect.width - _cachedView.BtnsGridRtf.rect.width);
            curPos.y = Mathf.Clamp(curPos.y, -_cachedView.BtnsDockRtf.rect.height + _cachedView.BtnsGridRtf.rect.height,
                0);
            _cachedView.BtnsGridRtf.anchoredPosition = curPos;
        }

        private void RefreshView()
        {
            if (_contentList == null)
            {
                return;
            }

            for (int i = _umList.Count - 1; i >= 0; i--)
            {
                FreeUmItem(_umList[i]);
            }

            _umList.Clear();
            for (int i = 0; i < _contentList.Count; i++)
            {
                var item = _contentList[i];
                var um = GetUmItem();
                um.SetParent(_cachedView.ChatContentDock);
                um.Set(item);
                _umList.Add(um);
            }

            ScrolToEnd();
        }

        private void OnSendBtnClick()
        {
            var inputContent = _cachedView.ChatInput.text;
            if (string.IsNullOrEmpty(inputContent))
            {
                return;
            }

            switch (_currentSendType)
            {
                case ChatData.EChatType.World:
                    if (!AppData.Instance.ChatData.SendWorldChat(inputContent))
                    {
                        return;
                    }

                    break;
                case ChatData.EChatType.Friends:
                    if (!AppData.Instance.ChatData.SendPrivateChat(inputContent, _curChatUser))
                    {
                        return;
                    }

                    break;
                case ChatData.EChatType.Room:
                    if (!AppData.Instance.ChatData.SendRoomChat(inputContent, ERoomChatType.ERCT_Room))
                    {
                        return;
                    }

                    break;
                case ChatData.EChatType.Team:
                    if (!AppData.Instance.ChatData.SendTeamChat(inputContent))
                    {
                        return;
                    }

                    break;
                default:
                    LogHelper.Warning("Send Chat fail");
                    break;
            }

            _cachedView.ChatInput.text = String.Empty;
            _cachedView.ChatInput.ActivateInputField();
        }

        private void OnSendChatTypeClick()
        {
            switch (_scene)
            {
                case EScene.Room:
                    if (_currentSendType == ChatData.EChatType.World)
                    {
                        SetSendChatType(ChatData.EChatType.Room);
                    }
                    else if (_currentSendType == ChatData.EChatType.Room)
                    {
                        SetSendChatType(ChatData.EChatType.Friends);
                    }
                    else
                    {
                        SetSendChatType(ChatData.EChatType.World);
                    }

                    break;
                case EScene.Home:
                    if (_currentSendType == ChatData.EChatType.World)
                    {
                        SetSendChatType(ChatData.EChatType.Friends);
                    }
                    else
                    {
                        SetSendChatType(ChatData.EChatType.World);
                    }

                    break;
                case EScene.Team:
                    if (_currentSendType == ChatData.EChatType.World)
                    {
                        SetSendChatType(ChatData.EChatType.Team);
                    }
                    else if (_currentSendType == ChatData.EChatType.Team)
                    {
                        SetSendChatType(ChatData.EChatType.Friends);
                    }
                    else
                    {
                        SetSendChatType(ChatData.EChatType.World);
                    }

                    break;
            }
        }

        private void OnSelectFriendBtn()
        {
            SetFriendsDockOpen(true);
        }

        private void OnCheckInfoBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_curSelectedUser);
            SetBtnsDockOpen(false);
        }

        private void OnPrivateChatBtn()
        {
            AppData.Instance.ChatData.SetCurPrivateChatUser(_curSelectedUser);
            _curChatUser = _curSelectedUser;
            _cachedView.ChatTypeTagArray[(int) ChatData.EChatType.Friends].isOn = true;
            SetBtnsDockOpen(false);
            _cachedView.ChatInput.ActivateInputField();
        }

        private void OnFollowBtn()
        {
            LocalUser.Instance.RelationUserList.RequestFollowUser(_curSelectedUser);
            SetBtnsDockOpen(false);
        }

        private void SetFriendsDockOpen(bool value)
        {
            _cachedView.ChatFriendsBgRtf.SetActiveEx(value);
            if (value)
            {
                var users = AppData.Instance.ChatData.ChatUsers.ToArray();
                for (int i = 0; i < _usCtrlFriendItems.Length; i++)
                {
                    UserInfoDetail user = i < users.Length ? users[users.Length - i - 1] : null;
                    _usCtrlFriendItems[i].SetEnable(user != null);
                    if (user != null)
                    {
                        _usCtrlFriendItems[i].SetData(user);
                        _usCtrlFriendItems[i].SetSelected(_curChatUser == user);
                    }
                }

                Canvas.ForceUpdateCanvases();
                var height = Mathf.Max(_cachedView.ChatFriendsGridRtf.rect.height, _minHeight);
                _cachedView.ChatFriendsBgRtf.rectTransform()
                    .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        private void SetBtnsDockOpen(bool value)
        {
            _cachedView.BtnsDockRtf.SetActiveEx(value);
            if (!value)
            {
                _curSelectedUser = null;
            }
        }

        private void OnChatListAppend(ChatData.EChatType chatType, ChatData.Item item)
        {
            if (!_isOpen)
            {
                return;
            }

            if (chatType != _currentChatTypeTag)
            {
                if (chatType == ChatData.EChatType.Friends && item.ChatUser.UserGuid != LocalUser.Instance.UserGuid)
                {
                    _cachedView.PrivateChatRedPoint.SetActive(true);
                }

                return;
            }

            var um = GetUmItem();
            um.SetParent(_cachedView.ChatContentDock);
            um.Set(item);
            _umList.Add(um);
            ScrolToEnd();
        }

        private void OnChatListCutHead(ChatData.EChatType chatType)
        {
            if (!_isOpen)
            {
                return;
            }

            if (chatType != _currentChatTypeTag)
            {
                return;
            }

            RefreshView();
        }

        private void FreeUmItem(UMCtrlChat umCtrlChat)
        {
            umCtrlChat.SetParent(_cachedView.PoolDock);
            _umPool.Push(umCtrlChat);
        }

        private UMCtrlChat GetUmItem()
        {
            if (_umPool.Count > 0)
            {
                return _umPool.Pop();
            }

            var um = new UMCtrlChat();
            um.MainCtrl = this;
            um.Init(_cachedView.PoolDock, _resScenary);
            return um;
        }

        private void ScrolToEnd()
        {
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
            {
                _cachedView.ChatScrollRect.verticalNormalizedPosition = 0;
            }));
        }

        public void OnChatItemClick(ChatData.Item item, string href, Vector2 pos)
        {
            if (href == UserStr)
            {
                UserManager.Instance.GetDataOnAsync(item.ChatUser.UserGuid,
                    detail =>
                    {
                        _curSelectedUser = detail;
                        SetBtnsDockOpen(true);
                        RefreshBtnsDock(pos);
                    }, () => { SocialGUIManager.ShowPopupDialog("用户数据获取失败"); });
            }
            else if (href == RoomStr)
            {
                if (_scene == EScene.Home)
                {
                    RoomManager.Instance.SendRequestJoinRoom(item.Param);
                }
            }
        }

        public enum EScene
        {
            Room,
            Home,
            Team
        }
    }
}