/********************************************************************
** Filename : UserFavoriteWorldProjectList.Impl.cs
** Author : quan
** Date : 6/7/2017 2:47 PM
** Summary : UserFavoriteWorldProjectList.Impl.cs
***********************************************************************/

using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UserFavoriteWorldProjectList
    {
        private List<Project> _allList = new List<Project>();
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
                    MessengerAsync.Broadcast(EMessengerType.OnUserFavoriteProjectListChanged);
                }
                return;
            }
            if (_resultCode == (int)ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_projectList);
            IsEnd = _projectList.Count < _cs_maxCount;
            MessengerAsync.Broadcast(EMessengerType.OnUserFavoriteProjectListChanged);
        }
    }
}

