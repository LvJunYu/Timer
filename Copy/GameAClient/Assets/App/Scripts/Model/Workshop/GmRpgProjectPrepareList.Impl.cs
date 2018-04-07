using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class GmRpgProjectPrepareList
    {
        private List<GmPrepareProject> _projectSyncList = new List<GmPrepareProject>();
        private readonly List<GmPrepareProject> _allList = new List<GmPrepareProject>();

        public bool IsEnd { get; private set; }

        public List<GmPrepareProject> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_GmRpgProjectPrepareList msg)
        {
            _projectSyncList.Clear();
            for (int i = 0; i < msg.ProjectList.Count; i++)
            {
                _projectSyncList.Add(new GmPrepareProject(msg.ProjectList[i]));
            }

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
//                    Messenger<long>.Broadcast(EMessengerType.OnUserPublishedProjectChanged, _cs_userId);
                }

                return;
            }

            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }

            _allList.AddRange(_projectSyncList);
            IsEnd = _projectSyncList.Count < _cs_maxCount;
//            Messenger<long>.Broadcast(EMessengerType.OnUserPublishedProjectChanged, _cs_userId);
        }

//        public void Requset(int startInx, int maxCount, Action successCallback, Action<ENetResultCode> failedCallback)
//        {
//            Request(LocalUser.Instance.UserGuid, startInx, maxCount, EPublishedProjectOrderBy.PPOB_PublishTime,
//                EOrderType.OT_Desc, successCallback, failedCallback);
//        }
    }
}