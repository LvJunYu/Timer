using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlChat : UICtrlAnimationBase<UIViewChat>
    {
        private bool _refreshScrollbar;
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
            _cachedView.ScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        }

        private void OnScrollRectValueChanged(Vector2 arg0)
        {
            //只在增加内容时滚到最下
            if (_refreshScrollbar)
            {
                _cachedView.Scrollbar.value = 0;
                _refreshScrollbar = false;
            }
        }

        private void Refresh()
        {
            _cachedView.VerticalLayoutGroup.enabled = true;
            _cachedView.ContentSizeFitter.enabled = true;
        }

        private void OnSendTexBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.InptField.text)) return;
            YIMManager.Instance.SendTextToRoom(_cachedView.InptField.text, YIMManager.Instance.WorldChatRoomId);
            _cachedView.InptField.text = string.Empty;
            //刷新Layout，先关闭ContentSizeFitter，否则显示不正确
            _cachedView.ContentSizeFitter.enabled = false;
            _cachedView.VerticalLayoutGroup.enabled = false;
            _refreshScrollbar = true;
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(Refresh));
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            YIMManager.Instance.Destroy();
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
                umCtrlChatTalkItem.Init(_cachedView.ContentSizeFitter.rectTransform(), ResScenary);
                _umCtrlChatTalkItemCache.Add(umCtrlChatTalkItem);
            }
            return umCtrlChatTalkItem;
        }
    }
}