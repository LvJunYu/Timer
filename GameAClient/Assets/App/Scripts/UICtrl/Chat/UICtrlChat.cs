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
        private List<UMCtrlChatTalkItem> _umCtrlChatTalkItemCache;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            YIMManager.Instance.SetCtrl(this);
            YIMManager.Instance.JoinChatRoom(YIMManager.Instance.WorldChatRoomId);
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.SendTexBtn.onClick.AddListener(OnSendTexBtn);
            _cachedView.StartRecordVoiceBtn.onClick.AddListener(OnStartRecordVoiceBtn);
            _cachedView.SendVoiceBtn.onClick.AddListener(OnSendVoiceBtn);
            _cachedView.ScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
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

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlChat>();
//            YIMManager.Instance.LeaveChatRoom(YIMManager.Instance.WorldChatRoomId);
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

        private void RefreshContent(bool showLastContent = true)
        {
            //刷新Layout，先关闭ContentSizeFitter，否则显示不正确
            _cachedView.ContentSizeFitter.enabled = false;
            _cachedView.VerticalLayoutGroup.enabled = false;
            _refreshScrollbar = showLastContent;
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() =>
            {
                _cachedView.VerticalLayoutGroup.enabled = true;
                _cachedView.ContentSizeFitter.enabled = true;
            }));
        }

        private void OnSendTexBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.InptField.text)) return;
            YIMManager.Instance.SendTextToRoom(_cachedView.InptField.text, YIMManager.Instance.WorldChatRoomId);
            _cachedView.InptField.text = string.Empty;
        }

        private void OnStartRecordVoiceBtn()
        {
            YIMManager.Instance.StartAudioRecordToRoom(YIMManager.Instance.WorldChatRoomId);
        }

        private void OnSendVoiceBtn()
        {
            YIMManager.Instance.StopAudioMessage();
        }

        public void ShowStatus(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowStatus(msg);
            RefreshContent();
        }

        public void ShowTalkText(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowText(msg);
            RefreshContent();
        }

        public void ShowReceiveTalk(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowText(string.Format(GM2DUIConstDefine.ChatReceiveTextFormat,
                LocalUser.Instance.User.UserInfoSimple.NickName, msg));
            RefreshContent();
        }

        public void ShowVoice(string msg)
        {
            CreateUMCtrlChatTalkItem().ShowText(string.Format("语音……\r\n{0}", msg));
            RefreshContent();
        }

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