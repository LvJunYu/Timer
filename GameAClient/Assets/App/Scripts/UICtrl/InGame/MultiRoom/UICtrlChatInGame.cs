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

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGameMainUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.OpenBtn.AddListener(EventTriggerType.PointerClick, eventData => OnClickOpenBtn());
            _cachedView.OpenBtn.AddListener(EventTriggerType.BeginDrag,
                eventData => OnBeginDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.Drag, eventData => OnDrag(eventData as PointerEventData));
            _cachedView.OpenBtn.AddListener(EventTriggerType.EndDrag,
                eventData => OnEndDrag(eventData as PointerEventData));
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
            RefrshView();
        }

        protected override void OnClose()
        {
            _chat.Close();
            base.OnClose();
        }

        private void RefrshView()
        {
        }

        private void OnCloseBtn()
        {
        }

        private void OnClickOpenBtn()
        {
        }

        private void OnBeginDrag(PointerEventData data)
        {
            _offsetPos = _cachedView.PannelRtf.position - GetWorldPos(data.position);
        }

        private void OnDrag(PointerEventData data)
        {
            _cachedView.PannelRtf.position = GetWorldPos(data.position, true) + _offsetPos;
        }

        private void OnEndDrag(PointerEventData data)
        {
        }
        
        private Vector3 GetWorldPos(Vector3 screenPos, bool clamp = false)
        {
            if (null == _uiCamera)
            {
                _uiCamera = SocialGUIManager.Instance.UIRoot.Canvas.worldCamera;
            }

            if (clamp)
            {
            }
            return _uiCamera.ScreenToWorldPoint(screenPos);
        }
    }
}