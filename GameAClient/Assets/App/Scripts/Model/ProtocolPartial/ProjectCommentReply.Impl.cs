using System;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class ProjectCommentReply
    {
        private UserInfoDetail _userInfoDetail;

        private UserInfoDetail _targetUserInfoDetail;

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
            set { _userInfoDetail = value; } //测试用
        }

        public UserInfoDetail TargetUserInfoDetail
        {
            get { return _targetUserInfoDetail; }
            set { _targetUserInfoDetail = value; } //测试用
        }

        protected override void OnSyncPartial(Msg_ProjectCommentReply msg)
        {
            base.OnSyncPartial();
            _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
            _targetUserInfoDetail = UserManager.Instance.UpdateData(msg.TargetUserInfo);
        }

        public void Reply(string content, Action successCallback = null, Action failedCallback = null)
        {
            var testRes = CheckTools.CheckMessage(content);
            if (testRes == CheckTools.ECheckMessageResult.Success)
            {
                RemoteCommands.ReplyProjectComment(_commentId, content, _id, res =>
                {
                    if (res.ResultCode == (int) EReplyUserMessageCode.RUMC_Success)
                    {
                        if (successCallback != null)
                        {
                            successCallback.Invoke();
                        }
                    }
                    else
                    {
                        if (failedCallback != null)
                        {
                            failedCallback.Invoke();
                        }
                    }
                }, code =>
                {
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke();
                    }
                });
            }
            else
            {
                SocialGUIManager.ShowCheckMessageRes(testRes);
                if (failedCallback != null)
                {
                    failedCallback.Invoke();
                }
            }
        }

        public void Delete()
        {
            RemoteCommands.DeleteProjectCommentReply(_id, msg =>
            {
                if (msg.ResultCode == (int) EDeleteProjectCommentReplyCode.DPCPC_Success)
                {
                    Messenger<ProjectCommentReply, long>.Broadcast(EMessengerType.OnDeleteProjectCommentReply, this, _commentId);
                }
            }, null);
        }
    }
}