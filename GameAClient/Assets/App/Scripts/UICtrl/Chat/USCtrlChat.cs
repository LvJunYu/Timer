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
        private ChatData.EChatType _currentChatTypeTag = ChatData.EChatType.All;
        private ChatData.EChatType _currentSendType = ChatData.EChatType.World;
        private Stack<UMCtrlChat> _umPool = new Stack<UMCtrlChat>(70);
        private List<ChatData.Item> _contentList;
        private List<UMCtrlChat> _umList = new List<UMCtrlChat>(70);
        private EResScenary _resScenary;
        private EScene _scene;
        public EResScenary ResScenary
        {
            get { return _resScenary; }
            set { _resScenary = value; }
        }

        public EScene Scene
        {
            get { return _scene; }
            set { _scene = value; }
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
            if (chatType == ChatData.EChatType.Room ||chatType == ChatData.EChatType.World || chatType == ChatData.EChatType.Team)
            {
                SetSendChatType(chatType);
            }
        }

        private void SetSendChatType(ChatData.EChatType chatType)
        {
            _currentSendType = chatType;
            if (chatType == ChatData.EChatType.World)
            {
                _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "世界";
            }
            else if (chatType == ChatData.EChatType.Room)
            {
                _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "房间";
            }
            else if (chatType == ChatData.EChatType.Team)
            {
                _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "队伍";
            }
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

            if (_currentSendType == ChatData.EChatType.Room)
            {
                if (!AppData.Instance.ChatData.SendRoomChat(inputContent, ERoomChatType.ERCT_Room))
                {
                    return;
                }
            }
            else if (_currentSendType == ChatData.EChatType.Team)
            {
                if (!AppData.Instance.ChatData.SendTeamChat(inputContent))
                {
                    return;
                }
            }
            else if (_currentSendType == ChatData.EChatType.World)
            {
                if (!AppData.Instance.ChatData.SendWorldChat(inputContent))
                {
                    return;
                }
            }
            else
            {
                LogHelper.Warning("Send Chat fail");
            }

            _cachedView.ChatInput.text = String.Empty;
            _cachedView.ChatInput.ActivateInputField();
        }

        private void OnSendChatTypeClick()
        {
            if (_scene == EScene.Home)
            {
                return;
            }
            if (_currentSendType == ChatData.EChatType.World)
            {
                if (_scene == EScene.Room)
                {
                    SetSendChatType(ChatData.EChatType.Room);
                }
                else if (_scene == EScene.Team)
                {
                    SetSendChatType(ChatData.EChatType.Team);
                }
            }
            else
            {
                SetSendChatType(ChatData.EChatType.World);
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

        public void OnChatItemClick(ChatData.Item item, string href)
        {
            if (_scene == EScene.Room || _scene == EScene.Team)
            {
                return;
            }

            if (href == "user")
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在加载用户数据");
                UserManager.Instance.GetDataOnAsync(item.ChatUser.UserGuid, detail =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(detail);
                }, () =>
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    SocialGUIManager.ShowPopupDialog("用户数据获取失败");
                });
            }
            else if (href == "room")
            {
                RoomManager.Instance.SendRequestJoinRoom(item.Param);
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