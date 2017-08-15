using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UserPublishedWorldProjectList
    {
        private readonly List<Project> _allList = new List<Project>();
        
        public bool IsEnd { get; private set; }
        public List<Project> AllList
        {
            get { return _allList; }
        }
        
        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (_resultCode == (int)ECachedDataState.CDS_None
                || _resultCode == (int)ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    IsEnd = true;
                    Messenger.Broadcast(EMessengerType.OnUserPublishedProjectChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_projectList);
            IsEnd = _projectList.Count < _cs_maxCount;
            Messenger.Broadcast(EMessengerType.OnUserPublishedProjectChanged);
        }

        public void Requset(int startInx, int maxCount, Action successCallback, Action<ENetResultCode> failedCallback)
        {
            Request(LocalUser.Instance.UserGuid, startInx, maxCount, EPublishedProjectOrderBy.PPOB_PublishTime,
                EOrderType.OT_Desc, successCallback, failedCallback);
        }
    }
}