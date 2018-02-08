
using SoyEngine.Proto;

namespace GameA
{
    public class UMCtrlInviteFriend : UMCtrlWorldShareProject
    {
        public UMViewInviteFriend View
        {
            get { return _cachedView as UMViewInviteFriend; }
        }

        protected override void RefreshView()
        {
            base.RefreshView();
            if (_wrapper == null)
            {
                return;
            }
            View.AdvLvTxt.text =
                GameATools.GetLevelString(LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel);
            View.CreateLvTxt.text =
                GameATools.GetLevelString(LocalUser.Instance.User.UserInfoSimple.LevelData.CreatorLevel);
            View.InGameFlag.SetActive(_wrapper.Content.InGame);
        }
    }
}