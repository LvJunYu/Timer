/********************************************************************
** Filename : UPCtrlWorldProjectDetail.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldProjectDetail.cs
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace GameA
{
    public class UPCtrlWorldProjectDetail : UPCtrlBase<UICtrlWorld, UIViewWorld>
    {
        #region 常量与字段
        private Project _content;
        private UPCtrlWorldProjectRecentRecord _upCtrlRecentRecord;
        private UPCtrlWorldProjectRecordRank _upCtrlRecordRank;
        private UPCtrlWorldProjectComment _upCtrlProjectComment;
        private UPCtrlBase<UICtrlWorld, UIViewWorld>[] _ctrlAry;

        private UPCtrlBase<UICtrlWorld, UIViewWorld> _curCtrl;
        #endregion

        #region 属性

        #endregion

        #region 方法

        public void SetData(Project project)
        {
            _content = project;
            _upCtrlRecentRecord.SetData(project);
            _upCtrlRecordRank.SetData(project);
            _upCtrlProjectComment.SetData(project);

            _upCtrlRecentRecord.Close();
            _upCtrlRecordRank.Close();
            _upCtrlProjectComment.Close();
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
            _upCtrlRecentRecord = new UPCtrlWorldProjectRecentRecord();
            _upCtrlRecentRecord.Init(_mainCtrl, _cachedView);
            _upCtrlRecordRank = new UPCtrlWorldProjectRecordRank();
            _upCtrlRecordRank.Init(_mainCtrl, _cachedView);
            _upCtrlProjectComment = new UPCtrlWorldProjectComment();
            _upCtrlProjectComment.Init(_mainCtrl, _cachedView);
            _ctrlAry = new UPCtrlBase<UICtrlWorld, UIViewWorld>[]
            {
                _upCtrlRecentRecord,
                _upCtrlRecordRank,
                _upCtrlProjectComment,
            };
            _cachedView.TabGroup.AddButton(_cachedView.RecentRecordTab,
                _cachedView.RecentRecordTab2, flag=>OnTabChange(0, flag));
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
