using System;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class USCtrlChatInGame : USCtrlBase<USViewChatInGame>
    {
        private ChatData.EChatType _currentChatTypeTag = ChatData.EChatType.Room;
        private ChatData.EChatType _currentSendType = ChatData.EChatType.Camp;
        private Stack<UMCtrlChatInGame> _umPool = new Stack<UMCtrlChatInGame>(70);
        private List<ChatData.Item> _contentList;
        private List<UMCtrlChatInGame> _umList = new List<UMCtrlChatInGame>(70);
        private EResScenary _resScenary;

        public EResScenary ResScenary
        {
            get { return _resScenary; }
            set { _resScenary = value; }
        }

        public override void Init(USViewChatInGame view)
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
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.ChatInput.onEndEdit.AddListener(str =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnSendBtn();
                }
            });
            BadWordManger.Instance.InputFeidAddListen(_cachedView.ChatInput);
            _cachedView.RoomTog.onValueChanged.AddListener(value =>
            {
                if (value)
                {
                    SelectChatTypeTag(ChatData.EChatType.Room);
                }
            });
            _cachedView.CampTog.onValueChanged.AddListener(value =>
            {
                if (value)
                {
                    SelectChatTypeTag(ChatData.EChatType.Camp);
                }
            });
            _cachedView.ChatTypeBtn.onClick.AddListener(OnSendChatTypeClick);
            SetSendChatType(ChatData.EChatType.Camp);
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
            SetSendChatType(chatType);
        }

        private void SetSendChatType(ChatData.EChatType chatType)
        {
            _currentSendType = chatType;
            if (chatType == ChatData.EChatType.Camp)
            {
                _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "阵营";
            }
            else if (chatType == ChatData.EChatType.Room)
            {
                _cachedView.ChatTypeBtn.GetComponentInChildren<Text>().text = "全部";
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

        private void OnSendBtn()
        {
            var inputContent = _cachedView.ChatInput.text;
            if (string.IsNullOrEmpty(inputContent))
            {
                return;
            }

            if (_currentSendType == ChatData.EChatType.Camp)
            {
                if (!AppData.Instance.ChatData.SendRoomChat(inputContent, ERoomChatType.ERCT_Camp,
                    TeamManager.Instance.GetMyTeamInxList()))
                {
                    return;
                }
            }
            else if (_currentSendType == ChatData.EChatType.Room)
            {
                if (!AppData.Instance.ChatData.SendRoomChat(inputContent, ERoomChatType.ERCT_Room))
                {
                    return;
                }
            }
            else
            {
                LogHelper.Error("OnSendBtn Fail, _currentSendType = {0}", _currentSendType);
                return;
            }

            _cachedView.ChatInput.text = String.Empty;
            _cachedView.ChatInput.ActivateInputField();
        }

        private void OnSendChatTypeClick()
        {
            if (_currentSendType == ChatData.EChatType.Camp)
            {
                SetSendChatType(ChatData.EChatType.Room);
            }
            else
            {
                SetSendChatType(ChatData.EChatType.Camp);
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

        private void FreeUmItem(UMCtrlChatInGame umCtrlChat)
        {
            umCtrlChat.SetParent(_cachedView.PoolDock);
            _umPool.Push(umCtrlChat);
        }

        private UMCtrlChatInGame GetUmItem()
        {
            if (_umPool.Count > 0)
            {
                return _umPool.Pop();
            }

            var um = new UMCtrlChatInGame();
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
    }
}