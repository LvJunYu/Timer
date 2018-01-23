using System;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class ProjectComment
    {
        private UserInfoDetail _userInfoDetail;
        private UserInfoDetail _targetUserInfoDetail;
        private ProjectCommentReplyData _replyList = new ProjectCommentReplyData();

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
        }

        public ProjectCommentReplyData ReplyList
        {
            get { return _replyList; }
        }

        public void UpdateLike(bool like, Action successCallback = null, Action<ENetResultCode> failedCallback = null)
        {
            if (_userLike == like)
            {
                if (failedCallback != null)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }

                return;
            }

            RemoteCommands.UpdateWorldProjectCommentLike(_id, like, ret =>
            {
                if (ret.ResultCode != (int) EUpdateWorldProjectCommentLikeCode.UWPCLC_Success)
                {
                    LogHelper.Error("UpdateWorldProjectCommentLike fail, resultCode = {0}", ret.ResultCode);
                    if (failedCallback != null)
                    {
                        failedCallback.Invoke((ENetResultCode) ret.ResultCode);
                    }
                    return;
                }

                _userLike = like; 
                if (successCallback != null)
                {
                    successCallback.Invoke();
                }
            }, code =>
            {
                LogHelper.Error("UpdateWorldProjectCommentLike fail, code = {0}", code);
                if (failedCallback != null)
                {
                    failedCallback.Invoke(ENetResultCode.NR_None);
                }
            });
        }

        protected override void OnSyncPartial(Msg_ProjectComment msg)
        {
            base.OnSyncPartial();
            _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
//            _targetUserInfoDetail = UserManager.Instance.UpdateData(msg.TargetUserInfo);
        }

        public void Reply(string content, Action successCallback = null, Action failedCallback = null)
        {
            var testRes = CheckTools.CheckMessage(content);
            if (testRes == CheckTools.ECheckMessageResult.Success)
            {
                RemoteCommands.ReplyProjectComment(_id, content, 0, res =>
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

        public void LocalAddReply(ProjectCommentReply reply)
        {
            _replyCount++;
            _firstReply = reply;
            _replyList.AllList.Add(reply);
            _replyList.AllList.Sort((r1, r2) => -r1.CreateTime.CompareTo(r2.CreateTime));
        }

        public void Delete()
        {
            RemoteCommands.DeleteProjectCommentReply(_id, msg =>
            {
                if (msg.ResultCode == (int) EDeleteProjectCommentCode.DPCC_Success)
                {
                    Messenger<ProjectComment>.Broadcast(EMessengerType.OnDeleteProjectComment, this);
                }
            }, null);
        }
    }
}