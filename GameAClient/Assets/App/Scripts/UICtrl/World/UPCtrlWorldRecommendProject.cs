using System.Collections.Generic;

namespace GameA
{
    public class UPCtrlWorldRecommendProject : UPCtrlWorldProjectBase
    {
        private List<UMCtrlWorldRecommendProject> _umCtrlWorldRecommendProjects;
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