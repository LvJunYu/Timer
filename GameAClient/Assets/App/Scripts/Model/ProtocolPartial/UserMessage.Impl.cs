using SoyEngine.Proto;

namespace GameA
{
    public partial class UserMessage
    {
        private UserInfoDetail _userInfoDetail;
        
        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
        }
     
        protected override void OnSyncPartial (Msg_UserMessage msg)
        {
            base.OnSyncPartial ();
            _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
        }
    }
}