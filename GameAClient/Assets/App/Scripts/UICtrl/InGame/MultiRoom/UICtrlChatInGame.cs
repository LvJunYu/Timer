using SoyEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlChatInGame : UICtrlInGameBase<UIViewChatInGame>
    {
        private USCtrlChatInGame _chat;
        private Vector3 _offsetPos;
        private Camera _uiCamera;
        private bool _openState;
        private bool _isDraging;
        private int _closeTimer;
        private Vector2 _curPos;
        private EMenu _curMenu;
        private UPCtrlChatInGameQuickChat _upCtrlChatInGameQuickChat;

        public USCtrlChatInGame Chat
        {
            get { return _chat; }
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _upCtrlChatInGameQuickChat = new UPCtrlChatInGameQuickChat();
            _upCtrlChatInGameQuickChat.Init(this, _cachedView);
            _cachedView.OpenBtn.AddListener(EventTriggerType.PointerClick, eventData => OnOpenBtn());
            _cachedView.OpenBtn.AddListener(EventTriggerType.BeginDrag,
                eventData => OnBeginDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.Drag, eventData => OnDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.EndDrag, eventData => OnEndDrag());

            _cachedView.OpenPannelBtn.AddListener(EventTriggerType.BeginDrag,
                eventData => OnBeginDrag(eventData as PointerEventData));
            _cachedView.OpenPannelBtn.AddListener(EventTriggerType.Drag,
                eventData => OnDrag(eventData as PointerEventData, true));
            _cachedView.OpenPannelBtn.AddListener(EventTriggerType.EndDrag, eventData => OnEndDrag(true));
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _chat = new USCtrlChatInGame();
            _chat.ResScenary = ResScenary;
            _chat.MainCtrl = this;
            _chat.Init(_cachedView.InGameChat);
            _curPos = _cachedView.PannelRtf.anchoredPosition;

            _cachedView.ChatHistoryTog.onValueChanged.AddListener(OnChatHistoryTog);
            _cachedView.QuickChatTog.onValueChanged.AddListener(OnQuickChatTog);
        }

        private void OnQuickChatTog(bool value)
        {
            if (value)
            {
                SetMenu(EMenu.QuickChat);
            }
        }

        private void OnChatHistoryTog(bool value)
        {
            if (value)
            {
                SetMenu(EMenu.ChatHistory);
            }
        }

        public void SetMenu(EMenu menu, bool force = false)
        {
            if (force)
            {
                _cachedView.ChatHistoryTog.isOn = menu == EMenu.ChatHistory;
                _cachedView.QuickChatTog.isOn = menu == EMenu.QuickChat;
            }
            if (_curMenu == menu)
            {
                return;
            }
            _curMenu = menu;
            _cachedView.ChatHistoryPannel.SetActive(menu == EMenu.ChatHistory);
            if (_curMenu == EMenu.ChatHistory)
            {
                _upCtrlChatInGameQuickChat.Close();
                _chat.Open();
            }
            else
            {
                _upCtrlChatInGameQuickChat.Open();
                _chat.Close();
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.OpenBtn.RemoveAllListener();
            _cachedView.OpenPannelBtn.RemoveAllListener();
            _cachedView.CloseBtn.onClick.RemoveAllListeners();
            _chat.OnDestroy();
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SetOpenState(false);
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _chat.OnUpdate();
            if (!_openState)
            {
                if (_closeTimer == 0 && Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnOpenBtn();
                }
            }

            if (_closeTimer > 0)
            {
                _closeTimer--;
            }
        }

        public void OnCloseBtn()
        {
            SetOpenState(false);
        }

        private void OnOpenBtn()
        {
            if (_isDraging)
            {
                return;
            }

            SetOpenState(true);
        }

        private void SetOpenState(bool value)
        {
            _openState = value;
            _cachedView.OpenBtn.SetActiveEx(!_openState);
            _cachedView.OpenPannel.SetActiveEx(_openState);
            if (value)
            {
                SetMenu(EMenu.ChatHistory, true);
                _curPos = _cachedView.PannelRtf.anchoredPosition;
                ClampWindow(_cachedView.OpenPannel);
            }
            else
            {
                _closeTimer = 2;
                SetMenu(EMenu.None);
                _cachedView.PannelRtf.anchoredPosition = _curPos;
            }
        }
        
        private void OnBeginDrag(PointerEventData data)
        {
            _isDraging = true;
            _offsetPos = _cachedView.PannelRtf.position - GetWorldPos(data.position);
        }

        private void OnDrag(PointerEventData data, bool isOpen = false)
        {
            _cachedView.PannelRtf.position = GetWorldPos(data.position) + _offsetPos;
            if (isOpen)
            {
                ClampWindow(_cachedView.OpenPannel);
            }
            else
            {
                ClampWindow(_cachedView.OpenBtn.rectTransform());
            }
        }

        private void OnEndDrag(bool isOpen = false)
        {
            _isDraging = false;
            if (isOpen)
            {
                _curPos = _cachedView.PannelRtf.anchoredPosition;
            }
        }

        private Vector3 GetWorldPos(Vector3 screenPos)
        {
            if (null == _uiCamera)
            {
                _uiCamera = SocialGUIManager.Instance.UIRoot.Canvas.worldCamera;
            }

            return _uiCamera.ScreenToWorldPoint(screenPos);
        }

        private void ClampWindow(RectTransform rtf)
        {
            var curPos = _cachedView.PannelRtf.anchoredPosition;
            curPos.x = Mathf.Clamp(curPos.x, -rtf.anchoredPosition.x,
                _cachedView.Trans.rect.width - rtf.rect.width - rtf.anchoredPosition.x);
            curPos.y = Mathf.Clamp(curPos.y, -rtf.anchoredPosition.y,
                _cachedView.Trans.rect.height - rtf.rect.height - rtf.anchoredPosition.y);
            _cachedView.PannelRtf.anchoredPosition = curPos;
        }

        public enum EMenu
        {
            None,
            QuickChat,
            ChatHistory
        }
    }
}