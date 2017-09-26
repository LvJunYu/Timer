/********************************************************************
** Filename : UPCtrlWorldProjectComment.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldProjectComment.cs
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectComment : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>
    {
        #region 常量与字段
        private const int PageSize = 10;
        private Project _content;
        private List<ProjectComment> _contentList = new List<ProjectComment>();
        private WorldProjectCommentList _data;
        private bool _isPostComment;
        private EResScenary _resScenary;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void SetData(Project project, EResScenary resScenary)
        {
            _content = project;
            _resScenary = resScenary;
            ClearCommentInput();
            if (_content == null)
            {
                _data = null;
            }
            else
            {
                _data = _content.CommentList;
            }
        }

        public override void Open()
        {
            base.Open();
            _cachedView.CommentListDock.SetActive(true);
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            _cachedView.CommentListDock.SetActive(false);
            base.Close();
        }
        #region private
        #endregion private

        private void RefreshView()
        {
            if (_content == null)
            {
                _cachedView.CommentListTableScroller.SetEmpty();
                return;
            }
            List<ProjectComment> list = _data.AllList;
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                ProjectComment r = list[i];
                _contentList.Add(r);
            }
            _cachedView.CommentListTableScroller.SetItemCount(_contentList.Count);
        }


        private void RequestData(bool append = false)
        {
            if (_content == null)
            {
                return;
            }
            int startInx = 0;
            if (append)
            {
                startInx = _contentList.Count;
            }
            Project requestP = _content;
            _content.RequestCommentList(startInx, PageSize, ()=>{
                if (!_isOpen) {
                    return;
                }
                if (_content != null && _content.ProjectId == requestP.ProjectId) {
                    RefreshView();
                }
            }, code=>{
            });
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if(inx >= _contentList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_contentList[inx]);
            if (!_data.IsEnd)
            {
                if(inx > _contentList.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlWorldProjectComment();
            item.Init(parent, _resScenary);
            return item;
        }

        private void PostComment()
        {
            if (_isPostComment)
            {
                return;
            }
            string comment = _cachedView.CommentInput.text;
            if (string.IsNullOrEmpty(comment))
            {
                return;
            }
            _isPostComment = true;
            _content.SendComment(comment, flag => {
                _isPostComment = false;
                if (flag) {
                    ClearCommentInput();
                    RefreshView();
                }
            });
        }

        private void ClearCommentInput()
        {
            _cachedView.CommentInput.text = string.Empty;
        }
        #region 接口
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CommentListTableScroller.Set(OnItemRefresh, GetItemRenderer);
            _cachedView.PostCommentBtn.onClick.AddListener(OnPostCommentBtnClick);
        }
        
        private void OnPostCommentBtnClick()
        {
            PostComment();
        }

        #endregion 接口

        #endregion

    }
}
