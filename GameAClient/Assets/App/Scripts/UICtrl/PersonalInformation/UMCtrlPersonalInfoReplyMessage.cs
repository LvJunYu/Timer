using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoReplyMessage : UMCtrlBase<UMViewPersonalInfoReplyMessage>, IDataItemRenderer
    {
        private static string _contentFormat = "<color=orange>{0}</color>:{1}";
        private static string _contentReplyFormat = "<color=orange>{0}</color>回复<color=orange>{1}</color>:{2}";
        private UserMessageReply _reply;
        private bool _openPublishDock;
        public bool IsShow { get; private set; }
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _reply; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
            _cachedView.ReplayBtn.onClick.AddListener(OnReplayBtn);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        public void Set(object obj)
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

        public void RefreshView()
        {
            _cachedView.PublishDock.SetActive(_openPublishDock);
            UserInfoSimple user = _reply.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_reply.CreateTime));
            if (_reply.RelayOther)
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

        private void OnInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
        }

        private void OnSendBtn()
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
                reply.RelayOther = true;
                reply.UserInfoDetail = LocalUser.Instance.User;
                reply.TargetUserInfoDetail = LocalUser.Instance.User;
                Messenger<long, UserMessageReply>.Broadcast(EMessengerType.OnReplyMessage, reply.MessageId, reply);
            }
            SetPublishDock(false);
        }

        private void OnReplayBtn()
        {
            SetPublishDock(!_openPublishDock);
        }

        private void SetPublishDock(bool value)
        {
            if (_reply == null) return;
            _openPublishDock = value;
            _cachedView.PublishDock.SetActive(_openPublishDock);
            Canvas.ForceUpdateCanvases();
            Messenger.Broadcast(EMessengerType.OnMessageBoardElementSizeChanged);
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
            _cachedView.SetActiveEx(false);
        }

        public void Show()
        {
            IsShow = true;
            _cachedView.SetActiveEx(true);
        }
    }
}