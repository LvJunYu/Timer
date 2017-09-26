/********************************************************************
** Filename : World.cs
** Author : quan
** Date : 6/7/2017 2:33 PM
** Summary : World.cs
***********************************************************************/

namespace GameA
{
    public class WorldData
    {
        private readonly WorldNewestProjectList _newestProjectList = new WorldNewestProjectList();
        private readonly WorldRecommendProjectList _recommendProjectList = new WorldRecommendProjectList();
        private readonly UserFavoriteWorldProjectList _userFavoriteProjectList = new UserFavoriteWorldProjectList();
        private readonly UserWorldProjectPlayHistoryList _userPlayHistoryList = new UserWorldProjectPlayHistoryList();

        public WorldNewestProjectList NewestProjectList
        {
            get { return _newestProjectList; }
        }

        public WorldRecommendProjectList RecommendProjectList
        {
            get { return _recommendProjectList; }
        }

        public UserFavoriteWorldProjectList UserFavoriteProjectList
        {
            get { return _userFavoriteProjectList; }
        }

        public UserWorldProjectPlayHistoryList UserPlayHistoryList
        {
            get { return _userPlayHistoryList; }
        }
    }
}

