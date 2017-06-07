﻿/********************************************************************
** Filename : WorldProjectRecentRecordList.cs
** Author : quan
** Date : 6/6/2017 4:51 PM
** Summary : WorldProjectRecentRecordList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectRecentRecordList : SyncronisticData
    {
        private List<Record> _allList = new List<Record>();

        public List<Record> AllList
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
                    MessengerAsync.Broadcast(EMessengerType.OnProjectRecentRecordChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_recordList);
            _recordList.Sort((r1, r2) =>-r1.CreateTime.CompareTo(r2.CreateTime));
            MessengerAsync.Broadcast(EMessengerType.OnProjectRecentRecordChanged);
        }
    }
}

