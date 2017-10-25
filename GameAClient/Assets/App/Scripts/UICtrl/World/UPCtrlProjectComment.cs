using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectComment : UPCtrlProjectDetailBase
    {
        private List<ProjectComment> _contentList;
        private WorldProjectCommentList _data;
        private bool _isPostComment;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
//            _cachedView.PostCommentBtn.onClick.AddListener(OnPostCommentBtn);
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
            if (_mainCtrl.Project == null)
            {
                _cachedView.GridDataScrollers[(int) _menu].SetEmpty();
                return;
            }
            if (_contentList == null)
            {
                _cachedView.CommentTableScroller.SetEmpty();
            }
            else
            {
                _cachedView.CommentTableScroller.SetItemCount(_contentList.Count);
            }
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProjectComment();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
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

        private void OnPostCommentBtn()
        {
            if (_isPostComment)
            {
                return;
            }
            if (string.IsNullOrEmpty(_cachedView.CommentInput.text))
            {
                return;
            }
            _isPostComment = true;
            _mainCtrl.Project.SendComment(_cachedView.CommentInput.text, flag =>
            {
                _isPostComment = false;
                if (flag)
                {
                    _cachedView.CommentInput.text = string.Empty;
                    RefreshView();
                }
            });
        }
    }
}