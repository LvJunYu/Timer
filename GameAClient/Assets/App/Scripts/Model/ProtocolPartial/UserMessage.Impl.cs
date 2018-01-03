using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UserMessage
    {
        private UserInfoDetail _userInfoDetail;
        private UserMessageReplyData _replyList = new UserMessageReplyData();

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
            set { _userInfoDetail = value; } //测试用
        }

        public UserMessageReplyData ReplyList
        {
            get { return _replyList; }
        }

        protected override void OnSyncPartial(Msg_UserMessage msg)
        {
            base.OnSyncPartial();
            _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
        }

        public void Reply(string content, Action successCallback = null, Action failedCallback = null)
        {
            var testRes = CheckTools.CheckMessage(content);
            if (testRes == CheckTools.ECheckMessageResult.Success)
            {
                RemoteCommands.ReplyUserMessage(_id, content, 0, res =>
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

        public void LocalAddReply(UserMessageReply reply)
        {
            _replyCount++;
            _firstReply = reply;
            _replyList.AllList.Add(reply);
            _replyList.AllList.Sort((r1, r2) => -r1.CreateTime.CompareTo(r2.CreateTime));
        }
    }
}