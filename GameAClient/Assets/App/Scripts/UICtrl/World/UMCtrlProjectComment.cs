using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectComment : UMCtrlPersonalInfoMessage
    {
        private const int pageSize = 5;
        private ProjectComment _comment;
        private UMCtrlProjectReplyComment _firstReplay;
        private List<ProjectCommentReply> _dataList;
        private List<UMCtrlProjectReplyComment> _umCache = new List<UMCtrlProjectReplyComment>(8);
        private bool _showVersionLine;

        public override object Data
        {
            get { return _comment; }
        }

        protected override void InitOnViewCreated()
        {
            _firstReplay = new UMCtrlProjectReplyComment();
            _firstReplay.Init(_cachedView.FirstReplyRtf, _resScenary);
            Messenger<ProjectCommentReply, long>.AddListener(EMessengerType.OnDeleteProjectCommentReply,
                OnDeleteProjectCommentReply);
        }

        public override void Set(object obj)
        {
            _comment = obj as ProjectComment;
            if (_comment == null)
            {
                Unload();
                return;
            }
            _unfold = false;
            _openPublishDock = false;
            _dataList = _comment.ReplyList.AllList;
            RefreshView();
        }

        public void SetVersionLineEnable(bool value)
        {
            _showVersionLine = value;
        }

        protected override void RequestData(bool append = false)
        {
            if (_comment == null) return;
            if (_comment.ReplyList.IsEnd) return;
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }
            _comment.ReplyList.Request(_comment.Id, startInx, pageSize, () =>
            {
                _dataList = _comment.ReplyList.AllList;
                RefreshReplyDock(true);
            }, code => { SocialGUIManager.ShowPopupDialog("获取数据失败。"); });
        }

        protected override void RefreshView()
        {
            _cachedView.DeleteDock.SetActive(_comment.UserInfo.UserId == LocalUser.Instance.UserGuid ||
                                             SocialGUIManager.Instance.GetUI<UICtrlPersonalInformation>().IsMyself);
//            _cachedView.ReplayBtn.SetActiveEx(false);
            _cachedView.PublishDock.SetActive(_openPublishDock);
            _cachedView.PraiseCountTxt.SetActiveEx(_comment.LikeCount > 0);
            UserInfoSimple user = _comment.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_comment.CreateTime));
            DictionaryTools.SetContentText(_cachedView.PraiseCountTxt, _comment.LikeCount.ToString());
            DictionaryTools.SetContentText(_cachedView.ReplayCountTxt, _comment.ReplyCount.ToString());
            DictionaryTools.SetContentText(_cachedView.Content,
                string.Format(_contentFormat, user.NickName, _comment.Comment));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultIconTexture);
            _cachedView.VersionLineDock.SetActive(_showVersionLine);
            _cachedView.Line.SetActive(!_showVersionLine);
            RefreshReplyDock();
            Canvas.ForceUpdateCanvases();
        }

        protected override void RefreshReplyDock(bool Broadcast = false)
        {
            _cachedView.ReplayDock.SetActive(_comment.ReplyCount > 0);
            if (_comment.ReplyCount > 0)
            {
                _unfold = _dataList.Count > 0;
                _cachedView.FoldBtn.SetActiveEx(_unfold);
                _cachedView.FirstReplyRtf.SetActiveEx(!_unfold);
                if (!_unfold && _firstReplay != null)
                {
                    _firstReplay.Set(_comment.FirstReply);
                }
                if (_dataList.Count == 0)
                {
                    ClearItem();
                    _cachedView.MoreTxt.text = string.Format(_totalFormat, _comment.ReplyCount);
                    _cachedView.MoreBtn.SetActiveEx(true);
                }
                else
                {
                    int remainCount = _comment.ReplyCount - _dataList.Count;
                    if (remainCount < 0)
                    {
                        LogHelper.Error("_message.ReplayCount<_dataList.Count");
                        return;
                    }
                    _cachedView.MoreTxt.text = string.Format(_moreFormat, remainCount);
                    _cachedView.MoreBtn.SetActiveEx(remainCount > 0);
                    ClearItem();
                    for (int i = 0; i < _dataList.Count; i++)
                    {
                        GetItem().Set(_dataList[i]);
                    }
                }
            }
            Canvas.ForceUpdateCanvases();
            if (Broadcast)
            {
                Messenger.Broadcast(EMessengerType.OnPublishDockActiveChanged);
            }
        }

        protected override void OnFoldBtn()
        {
            _dataList.Clear();
            RefreshReplyDock(true);
        }

        protected override void OnSendBtn()
        {
            if (!string.IsNullOrEmpty(_cachedView.InputField.text))
            {
                _comment.Reply(_cachedView.InputField.text);
                //测试
                var reply = new ProjectCommentReply();
                reply.Content = _cachedView.InputField.text;
                reply.CreateTime = DateTimeUtil.GetServerTimeNowTimestampMillis();
                reply.Id = 3000;
                reply.CommentId = _comment.Id;
                reply.RelayOther = false;
                reply.UserInfoDetail = LocalUser.Instance.User;
                Messenger<long, ProjectCommentReply>.Broadcast(EMessengerType.OnReplyProjectComment, _comment.Id,
                    reply);
            }
            SetPublishDock(false);
        }

        protected override void OnPraiseBtn()
        {
            if (_comment == null) return;
            _comment.UpdateLike(!_comment.UserLike, RefreshView);
        }

        protected override void OnHeadBtn()
        {
            if (_comment != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_comment.UserInfoDetail);
            }
        }
        
        protected override void OnDeleteBtn()
        {
            _comment.Delete();
        }
        
        private void OnDeleteProjectCommentReply(ProjectCommentReply reply, long messageId)
        {
            if (_comment != null && _comment.Id == messageId)
            {
                if (_dataList.Contains(reply))
                {
                    _dataList.Remove(reply);
                    RefreshReplyDock(true);
                }
            }
        }

        protected override void ClearOnDestroy()
        {
            Messenger<ProjectCommentReply, long>.RemoveListener(EMessengerType.OnDeleteProjectCommentReply,
                OnDeleteProjectCommentReply);
        }

        private UMCtrlProjectReplyComment GetItem()
        {
            var item = UMPoolManager.Instance.Get<UMCtrlProjectReplyComment>(_cachedView.ReplayRtf, _resScenary);
            _umCache.Add(item);
            return item;
        }

        private void ClearItem()
        {
            _umCache.ForEach(p => UMPoolManager.Instance.Free(p));
            _umCache.Clear();
        }

        public override void Unload()
        {
            ClearItem();
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}