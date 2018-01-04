using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UserMessageReply
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

        protected override void OnSyncPartial(Msg_UserMessageReply msg)
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
                RemoteCommands.ReplyUserMessage(_messageId, content, _id, res =>
                {
                    if (res.ResultCode == (int) EReplyUserMessageCode.RUMC_Success)
                    {
                        Messenger<long, UserMessageReply>.Broadcast(EMessengerType.OnReplyUserMessage, _messageId,
                            new UserMessageReply(res.Data));
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
    }
}