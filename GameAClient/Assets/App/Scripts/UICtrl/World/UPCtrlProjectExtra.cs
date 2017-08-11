/********************************************************************
** Filename : UPCtrlWorldProjectDetail.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldProjectDetail.cs
***********************************************************************/

using SoyEngine;

namespace GameA
{
    public class UPCtrlProjectExtra : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>
    {
        #region 常量与字段
        private UPCtrlProjectDetail _upCtrlProjectDetail;
        private UPCtrlProjectRecordRank _upCtrlRecordRank;
        private UPCtrlProjectComment _upCtrlProjectComment;
        private UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>[] _ctrlAry;

        private UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail> _curCtrl;
        #endregion

        #region 属性

        #endregion

        #region 方法

        public void SetData(Project project)
        {
            _upCtrlProjectDetail.SetData(project);
            _upCtrlRecordRank.SetData(project);
            _upCtrlProjectComment.SetData(project);
            
            _cachedView.TabGroup.SelectIndex(0, true);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.TabGroup.SelectIndex(0, true);
        }
        #region private
        #endregion private
        #region 接口
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _upCtrlProjectDetail = new UPCtrlProjectDetail();
            _upCtrlProjectDetail.Init(_mainCtrl, _cachedView);
            _upCtrlRecordRank = new UPCtrlProjectRecordRank();
            _upCtrlRecordRank.Init(_mainCtrl, _cachedView);
            _upCtrlProjectComment = new UPCtrlProjectComment();
            _upCtrlProjectComment.Init(_mainCtrl, _cachedView);
            _ctrlAry = new UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>[]
            {
                _upCtrlProjectDetail,
                _upCtrlRecordRank,
                _upCtrlProjectComment,
            };
            _cachedView.TabGroup.AddButton(_cachedView.DetailTab,
                _cachedView.DetailTab2, flag=>OnTabChange(0, flag));
            _cachedView.TabGroup.AddButton(_cachedView.RecordRankTab,
                _cachedView.RecordRankTab2, flag=>OnTabChange(1, flag));
            _cachedView.TabGroup.AddButton(_cachedView.CommentListTab,
                _cachedView.CommentListTab2, flag=>OnTabChange(2, flag));
        }

        private void OnTabChange(int inx, bool open)
        {
            if (open)
            {
                if (_curCtrl != null)
                {
                    _curCtrl.Close();
                }
                _curCtrl = _ctrlAry[inx];
                if (_curCtrl != null)
                {
                    _curCtrl.Open();
                }
            }
        }
        #endregion 接口

        #endregion

    }
}
