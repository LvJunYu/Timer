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
        private readonly WorldUserRecentRecordList _worldUserRecentRecordList = new WorldUserRecentRecordList();
        private readonly MatchShadowBattleData _matchShadowBattleData = new MatchShadowBattleData();
        private  readonly  WorldRankList _rankList = new WorldRankList();
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

        public WorldUserRecentRecordList WorldUserRecentRecordList
        {
            get { return _worldUserRecentRecordList; }
        }

        public MatchShadowBattleData MatchShadowBattleData
        {
            get { return _matchShadowBattleData; }
        }

        public WorldRankList RankList
        {
            get { return _rankList; }
        }
    }
}