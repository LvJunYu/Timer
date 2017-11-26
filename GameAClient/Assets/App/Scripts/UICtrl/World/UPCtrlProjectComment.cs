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
        private bool _isPostComment;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PostCommentBtn.onClick.AddListener(OnPostCommentBtn);
            _cachedView.CommentInput.onEndEdit.AddListener(OnCommentInputEndEdit);
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
            _cachedView.CommentInput.text = string.Empty;
            base.Close();
        }

        private void OnCommentInputEndEdit(string arg0)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnPostCommentBtn();
            }
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

        private void OnPostCommentBtn()
        {
            if (_mainCtrl.Project == null || _mainCtrl.Project.ProjectUserData == null) return;
            if (!_mainCtrl.CheckPlayed("玩过才能评论哦~~现在进入关卡吗？"))
            {
                return;
            }
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
                    _mainCtrl.Project.Request();
                }
            });
        }

        public override void Clear()
        {
            _cachedView.CommentTableScroller.ContentPosition = Vector2.zero;
        }
    }
}