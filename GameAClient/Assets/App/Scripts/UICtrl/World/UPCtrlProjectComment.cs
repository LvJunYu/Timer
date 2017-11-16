using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectComment : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>, IOnChangeHandler<long>
    {
        private const int _pageSize = 10;
        private List<ProjectComment> _contentList;
        private WorldProjectCommentList _data;
        private bool _isPostComment;
        private EResScenary _resScenary;

        public bool HasComment
        {
            get { return _contentList != null && _contentList.Count > 0; }
        }

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

        private void RequestData(bool append = false)
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

        private void RefreshView()
        {
            if (_mainCtrl.Project == null || _contentList == null)
            {
                _cachedView.CommentTableScroller.SetEmpty();
                DictionaryTools.SetContentText(_cachedView.CommentCountTxt,
                    string.Format(UICtrlProjectDetail.CountFormat, 0));
            }
            else
            {
                _cachedView.CommentTableScroller.SetItemCount(_contentList.Count);
                DictionaryTools.SetContentText(_cachedView.CommentCountTxt,
                    string.Format(UICtrlProjectDetail.CountFormat, _contentList.Count));
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProjectComment();
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
            if (_mainCtrl.Project.ProjectUserData.PlayCount == 0)
            {
                SocialGUIManager.ShowPopupDialog("玩过才能评论哦~~");
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
                    RefreshView();
                }
            });
        }

        public void OnChangeHandler(long val)
        {
            if (_isOpen)
            {
                RefreshView();
            }
        }

        public void OnChangeToApp()
        {
            RequestData();
        }

        public void Set(EResScenary resScenary)
        {
            _resScenary = resScenary;
        }
    }
}