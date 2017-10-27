
using SoyEngine.Proto;

namespace GameA
{
    public partial class MatchShadowBattleData  {
        private UserInfoDetail _userDataDetail;
        
        public UserInfoDetail UserDataDetail
        {
            get { return _userDataDetail; }
        }
        
        protected override void OnSyncPartial (Msg_SC_DAT_MatchShadowBattleData msg)
        {
            base.OnSyncPartial ();
            _userDataDetail = UserManager.Instance.UpdateData(msg.UserData);
        }
        
    }
}