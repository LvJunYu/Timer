/********************************************************************
** Filename : World.cs
** Author : quan
** Date : 6/7/2017 2:33 PM
** Summary : World.cs
***********************************************************************/

using System;

namespace GameA
{
    public class WorldData
    {
        private readonly WorldNewestProjectList _newestProjectList = new WorldNewestProjectList();
        private readonly UserFavoriteWorldProjectList _userFavoriteProjectList = new UserFavoriteWorldProjectList();
        private readonly UserWorldProjectPlayHistoryList _userPlayHistoryList = new UserWorldProjectPlayHistoryList();

        public WorldNewestProjectList NewestProjectList
        {
            get { return this._newestProjectList; }
        }

        public UserFavoriteWorldProjectList UserFavoriteProjectList
        {
            get { return this._userFavoriteProjectList; }
        }

        public UserWorldProjectPlayHistoryList UserPlayHistoryList
        {
            get { return this._userPlayHistoryList; }
        }
    }
}

