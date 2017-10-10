namespace GameA
{
    public partial class ProjectComment
    {
        private UserInfoDetail _userInfoDetail;
        
        private UserInfoDetail _targetUserInfoDetail;

        public UserInfoDetail UserInfoDetail
        {
            get { return _userInfoDetail; }
        }
        
        public UserInfoDetail TargetUserInfoDetail
        {
            get { return _targetUserInfoDetail; }
        }

        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            _userInfoDetail = UserManager.Instance.UpdateData(_userInfo);
            _targetUserInfoDetail = UserManager.Instance.UpdateData(_targetUserInfo);
        }
    }
}