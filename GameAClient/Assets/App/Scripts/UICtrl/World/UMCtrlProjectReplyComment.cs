using SoyEngine;
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
            _cachedView.DeleteDock.SetActive(_reply.UserInfo.UserId == LocalUser.Instance.UserGuid ||
                                             SocialGUIManager.Instance.GetUI<UICtrlProjectDetail>().IsMyself);
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

        protected override void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _reply.Reply(_cachedView.InputField.text);
            }
            SetPublishDock(false);
        }
        
        protected override void OnDeleteBtn()
        {
            _reply.Delete();
        }
    }
}