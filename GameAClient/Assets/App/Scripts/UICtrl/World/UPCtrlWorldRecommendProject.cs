﻿
namespace GameA
{
    public class UPCtrlWorldRecommendProject : UPCtrlWorldProjectBase
    {
        private WorldRecommendProjectList _data;

        protected override void RequestData(bool append = false)
        {
            _data = AppData.Instance.WorldData.RecommendProjectList;
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            _data.Request(0, () =>
            {
                _isRequesting = false;
                _projectList = _data.ProjectList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code => { _isRequesting = false; });
        }
    }
}