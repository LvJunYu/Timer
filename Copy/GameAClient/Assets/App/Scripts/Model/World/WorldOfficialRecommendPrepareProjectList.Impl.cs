using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldOfficialRecommendPrepareProjectList
    {
        private List<WorldOfficialRecommendPrepareProject> _projectSyncList =
            new List<WorldOfficialRecommendPrepareProject>();

        private List<WorldOfficialRecommendPrepareProject> _allList =
            new List<WorldOfficialRecommendPrepareProject>();

        public bool IsEnd { get; private set; }

        public List<WorldOfficialRecommendPrepareProject> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_WorldOfficialRecommendPrepareProjectList msg)
        {
            _projectSyncList = new List<WorldOfficialRecommendPrepareProject>();
            for (int i = 0; i < msg.ProjectList.Count; i++)
            {
                _projectSyncList.Add(new WorldOfficialRecommendPrepareProject(msg.ProjectList[i]));
            }

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