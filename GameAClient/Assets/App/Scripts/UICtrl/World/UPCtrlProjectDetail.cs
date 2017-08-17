/********************************************************************
** Filename : UPCtrlProjectDetail.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlProjectDetail.cs
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    public class UPCtrlProjectDetail : UPCtrlBase<UICtrlProjectDetail, UIViewProjectDetail>
    {
        #region 常量与字段
        private Project _content;
        private List<UMCtrlUser40> _umCtrlUser40List;
        #endregion

        #region 属性

        #endregion

        #region 方法
        public void SetData(Project project)
        {
            _content = project;
        }

        public override void Open()
        {
            base.Open();
            _cachedView.DetailDock.SetActive(true);
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            _cachedView.DetailDock.SetActive(false);
            base.Close();
        }
        #region private
        #endregion private

        private void RefreshView()
        {
            if (_content != null)
            {
                for (int i = 0; i < _umCtrlUser40List.Count; i++)
                {
                    if (i<_content.RecentPlayedUserList.DataList.Count)
                    {
                        _umCtrlUser40List[i].Show();
                        _umCtrlUser40List[i].Set(_content.RecentPlayedUserList.DataList[i].UserData);
                    }
                    else
                    {
                        _umCtrlUser40List[i].Hide();
                    }
                }
            }
            else
            {
                for (int i = 0; i < _umCtrlUser40List.Count; i++)
                {
                    _umCtrlUser40List[i].Hide();
                }
            }
        }


        private void RequestData()
        {
            var projectId = _content.ProjectId;
            _content.RecentPlayedUserList.Request(() =>
            {
                if (_isOpen && _content != null && _content.ProjectId == projectId)
                {
                    RefreshView();
                }
            }, code =>
            {
            });
        }

        #region 接口

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _umCtrlUser40List = new List<UMCtrlUser40>(5);
            for (int i = 0; i < 5; i++)
            {
                var umCtrl = new UMCtrlUser40();
                umCtrl.Init(_cachedView.RecentPlayUserDock);
                umCtrl.Hide();
                _umCtrlUser40List.Add(umCtrl);
            }
        }

        #endregion 接口

        #endregion

    }
}
