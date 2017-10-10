namespace GameA
{
    public partial class Mail
    {
        private UserInfoDetail _userInfoDetail;

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
        }

        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            _userInfoDetail = UserManager.Instance.UpdateData(_userInfo);
        }
    }
}