namespace GameA
{
    public partial class SearchUser
    {
        private UserInfoDetail _dataDetail;

        public UserInfoDetail DataDetail
        {
            get { return _dataDetail; }
        }

        protected override void OnSyncPartial()
        {
            _dataDetail = UserManager.Instance.UpdateData(_data);
        }
    }
}