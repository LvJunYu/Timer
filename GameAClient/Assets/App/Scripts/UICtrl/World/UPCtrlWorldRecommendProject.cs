/********************************************************************
** Filename : UPCtrlWorldRecommendProject.cs
** Author : quan
** Date : 6/7/2017 3:30 PM
** Summary : UPCtrlWorldRecommendProject.cs
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;

namespace GameA
{
    public class UPCtrlWorldRecommendProject : UPCtrlBase<UICtrlWorld, UIViewWorld>, IOnChangeHandler<long>
    {
        #region 常量与字段
        private List<Project> _contentList;
        private List<UMCtrlWorldRecommendProject> _umCtrlWorldRecommendProjects;
        private WorldRecommendProjectList _data;
        private bool _isRequest;
        private bool _hasInited;
        #endregion

        #region 属性

        #endregion

        #region 方法
        
        public override void Open()
        {
            base.Open();
            _cachedView.RecommendPanel.SetActiveEx(true);
            _data = AppData.Instance.WorldData.RecommendProjectList;
            if (!_hasInited)
            {
                RequestData();
            }
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.RecommendPanel.SetActiveEx(false);
            base.Close();
        }

        #region private
        
        private void RequestData()
        {
            if (_isRequest)
            {
                return;
            }
            _isRequest = true;
            _data.Request(0, ()=>
            {
                _hasInited = true;
                _isRequest = false;
                if (!_isOpen) {
                    return;
                }
                RefreshView();
            }, code=>{
                _isRequest = false;
            });
        }

        private void RefreshView()
        {
            _contentList = _data.ProjectList;
            for (int i = 0; i < _umCtrlWorldRecommendProjects.Count; i++)
            {
                if (i < _contentList.Count)
                {
                    Project p = _contentList[i];
                    _umCtrlWorldRecommendProjects[i].Set(p);
                }
                else
                {
                    _umCtrlWorldRecommendProjects[i].Set(null);
                }
            }
        }
        #endregion private

        #region 接口

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _umCtrlWorldRecommendProjects = new List<UMCtrlWorldRecommendProject>(_cachedView.RecommendCardDockAry.Length);
            for (int i = 0; i < _cachedView.RecommendCardDockAry.Length; i++)
            {
                var umCtrl = new UMCtrlWorldRecommendProject();
                umCtrl.Init(_cachedView.RecommendCardDockAry[i]);
                _umCtrlWorldRecommendProjects.Add(umCtrl);
                if (i < 2)
                {
                    umCtrl.SetHot();
                }
                else
                {
                    umCtrl.SetNew();
                }
            }
            _cachedView.RefreshRecommendButton.onClick.AddListener(RequestData);
        }

        public void OnChangeHandler(long val)
        {
            if (_contentList != null)
            {
                int inx = _contentList.FindIndex(p => p.ProjectId == val);
                if (inx >= 0)
                {
                    _umCtrlWorldRecommendProjects[inx].RefreshView();
                }
            }
        }
        #endregion 接口

        #endregion

    }
}
