using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldOfficialRecommendProjectList
    {
        private List<Project> _projectSyncList;
        private List<Project> _allList = new List<Project>();
        public bool IsEnd { get; private set; }

        public List<Project> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_WorldOfficialRecommendProjectList msg)
        {
            _projectSyncList = ProjectManager.Instance.UpdateData(msg.ProjectList);
            base.OnSyncPartial(msg);
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (_resultCode == (int) ECachedDataState.CDS_None || _resultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    IsEnd = true;
//                    MessengerAsync.Broadcast(EMessengerType.OnWorldSelfRecommendProjectListChanged);
                }

                return;
            }

            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }

            _allList.AddRange(_projectSyncList);
            IsEnd = _projectSyncList.Count < _cs_maxCount;
//            MessengerAsync.Broadcast(EMessengerType.OnWorldSelfRecommendProjectListChanged);
        }
    }
}