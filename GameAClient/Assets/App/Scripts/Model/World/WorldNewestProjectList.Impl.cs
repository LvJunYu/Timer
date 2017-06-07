/********************************************************************
** Filename : WorldNewestProjectList.cs
** Author : quan
** Date : 6/6/2017 5:13 PM
** Summary : WorldNewestProjectList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldNewestProjectList : SyncronisticData
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
                    MessengerAsync.Broadcast(EMessengerType.OnNewestProjectListChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_projectList);
            MessengerAsync.Broadcast(EMessengerType.OnNewestProjectListChanged);
        }
    }
}

