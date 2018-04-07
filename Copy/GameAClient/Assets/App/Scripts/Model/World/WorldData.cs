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
        private readonly WorldBestProjectList _worldBestProjectList = new WorldBestProjectList();

        private readonly WorldOfficialRecommendProjectList _worldOfficialProjectList =
            new WorldOfficialRecommendProjectList();

        private readonly WorldOfficialRecommendPrepareProjectList _worldOfficialPrepareProjectList =
            new WorldOfficialRecommendPrepareProjectList();

        private readonly WorldOfficialRecommendCandidateProjectList _worldOfficialCandidateProjectList =
            new WorldOfficialRecommendCandidateProjectList();

        public WorldOfficialRecommendCandidateProjectList WorldOfficialCandidateProjectList
        {
            get { return _worldOfficialCandidateProjectList; }
        }

        public WorldOfficialRecommendPrepareProjectList WorldOfficialPrepareProjectList
        {
            get { return _worldOfficialPrepareProjectList; }
        }

        public WorldOfficialRecommendProjectList WorldOfficialProjectList
        {
            get { return _worldOfficialProjectList; }
        }

        private readonly WorldSelfRecommendProjectList _worldSelfRecommendProjectList =
            new WorldSelfRecommendProjectList();

        private readonly WorldFollowedUserProjectList
            _worldFollowedUserProjectList = new WorldFollowedUserProjectList();

        private readonly WorldRankList _rankList = new WorldRankList();

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

        public WorldFollowedUserProjectList WorldFollowedUserProjectList
        {
            get { return _worldFollowedUserProjectList; }
        }

        public WorldBestProjectList WorldBestProjectList
        {
            get { return _worldBestProjectList; }
        }

        public WorldRankList RankList
        {
            get { return _rankList; }
        }

        public WorldSelfRecommendProjectList WorldSelfRecommendProjectList
        {
            get { return _worldSelfRecommendProjectList; }
        }
    }
}