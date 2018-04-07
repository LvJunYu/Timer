using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class GmMasterProjectList
    {
        private List<Project> _projectSyncList = new List<Project>();
        private readonly List<Project> _allList = new List<Project>();

        public bool IsEnd { get; private set; }

        public List<Project> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_GmMasterProjectList msg)
        {
            _projectSyncList = ProjectManager.Instance.UpdateData(msg.ProjectList);
            base.OnSyncPartial(msg);
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (_resultCode == (int) ECachedDataState.CDS_None
                || _resultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    IsEnd = true;
                }

                return;
            }

            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }

            _allList.AddRange(_projectSyncList);
            IsEnd = _projectSyncList.Count < _cs_maxCount;
        }
    }
}