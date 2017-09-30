using SoyEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlChat : UICtrlAnimationBase<UIViewChat>
    {
        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlChat>();
            YIMManager.Instance.LeaveChatRoom(YIMManager.Instance.WorldChatRoomId);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            YIMManager.Instance.SetCtrl(this);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.SendTexBtn.onClick.AddListener(OnSendTexBtn);
        }

        private void OnSendTexBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.InptField.text)) return;
            YIMManager.Instance.SendTextToRoom(_cachedView.InptField.text, YIMManager.Instance.WorldChatRoomId);
            _cachedView.InptField.text = string.Empty;
            _cachedView.Scrollbar.value = 0;
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            YIMManager.Instance.JoinChatRoom(YIMManager.Instance.WorldChatRoomId);
            _cachedView.Scrollbar.value = 0;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.PopUpUI;
        }

        public void ShowStatus(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowStatus(msg);
        }
        
        public void ShowTalkText(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowText(msg);
        }

        private List<UMCtrlChatTalkItem> _umCtrlChatTalkItemCache;

        private UMCtrlChatTalkItem CreateUMCtrlChatTalkItem()
        {
            if (null == _umCtrlChatTalkItemCache)
            {
                _umCtrlChatTalkItemCache = new List<UMCtrlChatTalkItem>();
            }
            var umCtrlChatTalkItem = _umCtrlChatTalkItemCache.Find(p => !p.IsShow);
            if (umCtrlChatTalkItem != null)
            {
                umCtrlChatTalkItem.Show();
            }
            else
            {
                umCtrlChatTalkItem = new UMCtrlChatTalkItem();
                umCtrlChatTalkItem.Init(_cachedView.ContentSizeFitter.rectTransform(),ResScenary);
                _umCtrlChatTalkItemCache.Add(umCtrlChatTalkItem);
            }
            return umCtrlChatTalkItem;
        }
    }
}