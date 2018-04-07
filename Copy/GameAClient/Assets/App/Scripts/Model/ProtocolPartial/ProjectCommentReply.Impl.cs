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
        }

        public UserInfoDetail TargetUserInfoDetail
        {
            get { return _targetUserInfoDetail; }
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
                        Messenger<long, ProjectCommentReply>.Broadcast(EMessengerType.OnReplyProjectComment, _commentId,
                            new ProjectCommentReply(res.Data));
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
                else
                {
                    LogHelper.Error("DeleteProjectCommentReply ResultCode = {0}", msg.ResultCode);
                    SocialGUIManager.ShowPopupDialog("删除失败");
                }
            }, code =>
            {
                LogHelper.Error("DeleteProjectCommentReply Fail code = {0}", code);
                SocialGUIManager.ShowPopupDialog("删除失败");
            });
        }
    }
}