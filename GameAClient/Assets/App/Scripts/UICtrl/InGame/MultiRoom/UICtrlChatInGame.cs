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
        private Vector2 _openPos;

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OpenBtn.AddListener(EventTriggerType.PointerClick, eventData => OnClickOpenBtn());
            _cachedView.OpenBtn.AddListener(EventTriggerType.BeginDrag, eventData => OnBeginDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.Drag, eventData => OnDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.EndDrag, eventData => OnEndDrag());
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);

            _chat = new USCtrlChatInGame();
            _chat.ResScenary = ResScenary;
            _chat.Init(_cachedView.InGameChat);
        }

        protected override void OnDestroy()
        {
            _cachedView.OpenBtn.RemoveAllListener();
            _cachedView.CloseBtn.onClick.RemoveAllListeners();
            _chat.OnDestroy();
            base.OnDestroy();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _chat.Open();
            SetOpenState(false);
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
        }

        private void RefrshView()
        {
            _cachedView.OpenBtn.SetActiveEx(!_openState);
            _cachedView.OpenPannel.SetActive(_openState);
        }

        private void OnCloseBtn()
        {
            SetOpenState(false);
        }

        private void OnClickOpenBtn()
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
            RefrshView();
            if (value)
            {
                _openPos = _cachedView.PannelRtf.anchoredPosition;
                ClampWindow(_cachedView.PannelRtf);
            }
            else
            {
                _cachedView.PannelRtf.anchoredPosition = _openPos;
            }
        }

        private void OnBeginDrag(PointerEventData data)
        {
            _isDraging = true;
            _offsetPos = _cachedView.PannelRtf.position - GetWorldPos(data.position);
        }

        private void OnDrag(PointerEventData data)
        {
            _cachedView.PannelRtf.position = GetWorldPos(data.position) + _offsetPos;
            ClampWindow(_cachedView.OpenBtn.rectTransform());
        }

        private void OnEndDrag()
        {
            _isDraging = false;
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
            curPos.x = Mathf.Clamp(curPos.x, 0, _cachedView.Trans.rect.width - rtf.rect.width);
            curPos.y = Mathf.Clamp(curPos.y, 0,  _cachedView.Trans.rect.height - rtf.rect.height);
            _cachedView.PannelRtf.anchoredPosition = curPos;
        }
    }
}