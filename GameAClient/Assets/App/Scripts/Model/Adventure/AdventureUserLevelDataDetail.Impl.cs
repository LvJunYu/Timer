namespace GameA
{
    public partial class AdventureUserLevelDataDetail
    {
        private UserInfoDetail _highScoreFriendInfoDetail;

        public UserInfoDetail HighScoreFriendInfoDetail
        {
            get { return _highScoreFriendInfoDetail; }
        }

        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            _highScoreFriendInfoDetail = UserManager.Instance.UpdateData(_highScoreFriendInfo);
        }
    }
}