using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoReplyMessage : UMCtrlBase<UMViewPersonalInfoReplyMessage>, IDataItemRenderer, IUMPool
    {
        protected static string _contentFormat = "<color=orange>{0}</color>: {1}";
        protected static string _contentReplyFormat = "<color=orange>{0}</color>回复<color=orange>{1}</color>: {2}";
        private UserMessageReply _reply;
        protected bool _openPublishDock;
        public bool IsShow { get; private set; }
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public virtual object Data
        {
            get { return _reply; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
            _cachedView.ReplayBtn.onClick.AddListener(OnReplyBtn);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
            BadWordManger.Instance.InputFeidAddListen(_cachedView.InputField);
        }

        public virtual void Set(object obj)
        {
            _reply = obj as UserMessageReply;
            if (_reply == null)
            {
                Unload();
                return;
            }
            _openPublishDock = false;
            RefreshView();
        }

        protected virtual void RefreshView()
        {
            _cachedView.PublishDock.SetActive(_openPublishDock);
            UserInfoSimple user = _reply.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_reply.CreateTime));
            if (_reply.TargetUserInfoDetail != null)
            {
                DictionaryTools.SetContentText(_cachedView.Content, string.Format(_contentReplyFormat, user.NickName,
                    _reply.TargetUserInfoDetail.UserInfoSimple.NickName, _reply.Content));
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.Content,
                    string.Format(_contentFormat, user.NickName, _reply.Content));
            }
            Canvas.ForceUpdateCanvases();
        }

        protected virtual void OnInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
        }

        protected virtual void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _reply.Reply(_cachedView.InputField.text);
                //测试
                var reply = new UserMessageReply();
                reply.Content = _cachedView.InputField.text;
                reply.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                reply.Id = 4000;
                reply.MessageId = _reply.MessageId;
                reply.UserInfoDetail = LocalUser.Instance.User;
                reply.TargetUserInfoDetail = LocalUser.Instance.User;
                Messenger<long, UserMessageReply>.Broadcast(EMessengerType.OnReplyUserMessage, reply.MessageId, reply);
            }
            SetPublishDock(false);
        }

        protected virtual void OnReplyBtn()
        {
            SetPublishDock(!_openPublishDock);
        }

        protected virtual void SetPublishDock(bool value)
        {
            _openPublishDock = value;
            _cachedView.PublishDock.SetActive(_openPublishDock);
            Canvas.ForceUpdateCanvases();
            Messenger.Broadcast(EMessengerType.OnPublishDockActiveChanged);
            if (_openPublishDock)
            {
                if (UMCtrlPersonalInfoMessage.OpenInputCallBack != null)
                {
                    UMCtrlPersonalInfoMessage.OpenInputCallBack.Invoke();
                }
                UMCtrlPersonalInfoMessage.OpenInputCallBack = () => SetPublishDock(false);
                _cachedView.InputField.text = String.Empty;
                _cachedView.InputField.Select();
            }
            else
            {
                UMCtrlPersonalInfoMessage.OpenInputCallBack = null;
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.ReplayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        public void Unload()
        {
        }

        public void Hide()
        {
            IsShow = false;
            _cachedView.Trans.anchoredPosition = new Vector2(100000, 0);
        }

        public void Show()
        {
            IsShow = true;
            _cachedView.Trans.anchoredPosition = Vector2.zero;
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.SetParent(rectTransform);
        }
    }
}