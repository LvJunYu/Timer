using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class GmAdventureProjectPrepareList
    {
        private List<GmAdventureSection> _sectionsSyncList = new List<GmAdventureSection>();
        private readonly List<GmAdventureSection> _allList = new List<GmAdventureSection>();

        public bool IsEnd { get; private set; }

        public List<GmAdventureSection> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_GmAdventureProjectPrepareList msg)
        {
            _sectionsSyncList = new List<GmAdventureSection>();
            for (int i = 0; i < msg.SectionList.Count; i++)
            {
                _sectionsSyncList.Add(new GmAdventureSection(msg.SectionList[i]));
            }

            base.OnSyncPartial(msg);
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
//            if (_resultCode == (int) ECachedDataState.CDS_None
//                || _resultCode == (int) ECachedDataState.CDS_Uptodate)
//            {
//                if (!_inited)
//                {
//                    IsEnd = true;
////                    Messenger<long>.Broadcast(EMessengerType.OnUserPublishedProjectChanged, _cs_userId);
//                }
//
//                return;
//            }
//
//            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
//            {
//                _allList.Clear();
//            }
            _allList.Clear();
            _allList.AddRange(_sectionsSyncList);
            IsEnd = _sectionsSyncList.Count < _totalSectionCount;
//            Messenger<long>.Broadcast(EMessengerType.OnUserPublishedProjectChanged, _cs_userId);
        }

//        public void Requset(int startInx, int maxCount, Action successCallback, Action<ENetResultCode> failedCallback)
//        {
//            Request(LocalUser.Instance.UserGuid, startInx, maxCount, EPublishedProjectOrderBy.PPOB_PublishTime,
//                EOrderType.OT_Desc, successCallback, failedCallback);
//        }
    }
}