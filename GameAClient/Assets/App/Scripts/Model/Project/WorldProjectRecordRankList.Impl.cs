/********************************************************************
** Filename : WorldProjectRecordRankList.cs
** Author : quan
** Date : 6/6/2017 5:13 PM
** Summary : WorldProjectRecordRankList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectRecordRankList : SyncronisticData
    {
        private List<RecordRankHolder> _allList = new List<RecordRankHolder>();

        public List<RecordRankHolder> AllList
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
                    MessengerAsync.Broadcast(EMessengerType.OnProjectRecordRankChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            int i = _allList.Count;
            _recordList.ForEach(r => _allList.Add(new RecordRankHolder(r, i++)));
            MessengerAsync.Broadcast(EMessengerType.OnProjectRecordRankChanged);
        }
    }
}

