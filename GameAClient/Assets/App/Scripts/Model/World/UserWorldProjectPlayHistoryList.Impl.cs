/********************************************************************
** Filename : UserWorldProjectPlayHistoryList.cs
** Author : quan
** Date : 6/6/2017 5:13 PM
** Summary : UserWorldProjectPlayHistoryList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class UserWorldProjectPlayHistoryList : SyncronisticData
    {
        private List<Project> _allList = new List<Project>();

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
                    MessengerAsync.Broadcast(EMessengerType.OnUserWorldProjectPlayHistoryListChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_projectList);
            MessengerAsync.Broadcast(EMessengerType.OnUserWorldProjectPlayHistoryListChanged);
        }
    }
}

