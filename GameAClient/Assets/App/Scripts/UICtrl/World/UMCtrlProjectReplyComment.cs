﻿using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectReplyComment : UMCtrlPersonalInfoReplyMessage
    {
        private ProjectCommentReply _reply;

        public override object Data
        {
            get { return _reply; }
        }

        public override void Set(object obj)
        {
            _reply = obj as ProjectCommentReply;
            if (_reply == null)
            {
                Unload();
                return;
            }
            _openPublishDock = false;
            RefreshView();
        }

        protected override void RefreshView()
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

        protected override void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _reply.Reply(_cachedView.InputField.text);
                //测试
                var reply = new ProjectCommentReply();
                reply.Content = _cachedView.InputField.text;
                reply.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                reply.Id = 4000;
                reply.CommentId = _reply.CommentId;
                reply.RelayOther = true;
                reply.UserInfoDetail = LocalUser.Instance.User;
                reply.TargetUserInfoDetail = LocalUser.Instance.User;
                Messenger<long, ProjectCommentReply>.Broadcast(EMessengerType.OnReplyProjectComment, reply.CommentId, reply);
            }
            SetPublishDock(false);
        }
    }
}