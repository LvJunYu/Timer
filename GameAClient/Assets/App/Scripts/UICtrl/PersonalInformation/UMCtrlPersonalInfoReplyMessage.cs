using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalInfoReplyMessage : UMCtrlBase<UMViewPersonalInfoReplyMessage>, IDataItemRenderer
    {
        private static string _contentFormat = "{0}:{1}";
        private static string _contentReplyFormat = "{0}回复{1}:{2}";
        private UserMessageReplay _reply;
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
            _cachedView.ReplayBtn.onClick.AddListener(OnReplayBtn);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.InputField.onEndEdit.AddListener(OnInputEndEdit);
        }

        private void OnInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnSendBtn();
            }
            _cachedView.PublishDock.SetActive(false);
        }

        private void OnSendBtn()
        {
        }

        private void OnReplayBtn()
        {
            _cachedView.PublishDock.SetActive(true);
            _cachedView.InputField.Select();
        }

        protected override void OnDestroy()
        {
            _cachedView.ReplayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        public void Set(object obj)
        {
            _reply = obj as UserMessageReplay;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_reply == null)
            {
                Unload();
                return;
            }
            _cachedView.PublishDock.SetActive(false);
            UserInfoSimple user = _reply.UserInfoDetail.UserInfoSimple;
            _cachedView.InputField.text = String.Empty;
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
        }

        public void Unload()
        {
        }
    }
}