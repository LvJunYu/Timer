/********************************************************************
** Filename : WorldProjectCommentList.cs
** Author : quan
** Date : 6/6/2017 3:14 PM
** Summary : WorldProjectCommentList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;

namespace GameA
{
    public partial class WorldProjectCommentList : SyncronisticData
    {
        private List<ProjectComment> _allList = new List<ProjectComment>();

        public List<ProjectComment> AllList
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
                    MessengerAsync.Broadcast(EMessengerType.OnProjectCommentChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_commentList);
            _allList.Sort((p1, p2) => -p1.CreateTime.CompareTo(p2.CreateTime));
            MessengerAsync.Broadcast(EMessengerType.OnProjectCommentChanged);
        }

        public void AddNewComment(ProjectComment data)
        {
            _allList.Add(data);
            _allList.Sort((p1, p2) => -p1.CreateTime.CompareTo(p2.CreateTime));
        }
    }
}

