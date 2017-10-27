using SoyEngine.Proto;

namespace GameA
{
    public partial class Mail
    {
        private UserInfoDetail _userInfoDetail;

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
        }

        protected override void OnSyncPartial(Msg_Mail msg)
        {
            base.OnSyncPartial ();
            _userInfoDetail = UserManager.Instance.UpdateData(msg.UserInfo);
        }
//        private string _mailfetched = "icon_enclosure_d";
//        private string _mailUnfetched = "icon_enclosure";
//        private string _mailRead = "icon_mail_open";
//        private string _mailUnRead = "icon_mail";
    }
}