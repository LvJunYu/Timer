using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectComment : UPCtrlProjectDetailBase
    {
        private const int _pageSize = 10;
        private List<ProjectComment> _contentList;
        private WorldProjectCommentList _data;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CommentTableScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _contentList = null;
            Clear();
            base.Close();
        }

        protected override void RequestData(bool append = false)
        {
            if (_mainCtrl.Project == null) return;
            _data = _mainCtrl.Project.CommentList;
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            _mainCtrl.Project.RequestCommentList(startInx, _pageSize, () =>
            {
                _contentList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { });
        }

        protected override void RefreshView()
        {
            if (_mainCtrl.Project == null || _contentList == null)
            {
                _cachedView.CommentTableScroller.SetEmpty();
            }
            else
            {
                _cachedView.CommentTableScroller.SetItemCount(_contentList.Count);
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectComment();
            item.Init(parent, _resScenary);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_contentList[inx]);
                if (!_data.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        public void OnReplyProjectComment(long commentId, ProjectCommentReply reply)
        {
            var comment = _contentList.Find(p => p.Id == commentId);
            if (comment != null)
            {
                comment.LocalAddReply(reply);
                _cachedView.CommentTableScroller.RefreshCurrent();
            }
        }
        
        public override void Clear()
        {
            _cachedView.CommentInput.text = string.Empty;
            _cachedView.CommentTableScroller.ContentPosition = Vector2.zero;
        }

    }
}