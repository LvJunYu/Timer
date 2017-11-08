using SoyEngine.Proto;

namespace GameA
{
    public partial class AdventureUserLevelDataDetail
    {
        private UserInfoDetail _highScoreFriendInfoDetail;

        public UserInfoDetail HighScoreFriendInfoDetail
        {
            get { return _highScoreFriendInfoDetail; }
        }

        protected override void OnSyncPartial (Msg_SC_DAT_AdventureUserLevelDataDetail msg)
        {
            base.OnSyncPartial ();
            _highScoreFriendInfoDetail = UserManager.Instance.UpdateData(msg.HighScoreFriendInfo);
        }
    }
}