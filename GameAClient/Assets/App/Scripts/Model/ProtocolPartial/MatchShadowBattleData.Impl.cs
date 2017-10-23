
namespace GameA
{
    public partial class MatchShadowBattleData  {
        private UserInfoDetail _userDataDetail;
        
        public UserInfoDetail UserDataDetail
        {
            get { return _userDataDetail; }
        }
        
        protected override void OnSyncPartial ()
        {
            base.OnSyncPartial ();
            _userDataDetail = UserManager.Instance.UpdateData(_userData);
        }
        
    }
}