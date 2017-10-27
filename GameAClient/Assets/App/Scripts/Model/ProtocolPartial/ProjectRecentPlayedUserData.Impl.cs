using SoyEngine.Proto;

namespace GameA
{
    public partial class ProjectRecentPlayedUserData
    {
        private UserInfoDetail _userDataDetail;
        
        public UserInfoDetail UserDataDetail
        {
            get { return _userDataDetail; }
        }
        
        protected override void OnSyncPartial (Msg_ProjectRecentPlayedUserData msg)
        {
            base.OnSyncPartial ();
            _userDataDetail = UserManager.Instance.UpdateData(msg.UserData);
        }
    }
}