using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    public class USCtrlMultiTeam : USCtrlBase<USViewMultiTeam>
    {
        private Msg_MC_RoomUserInfo _user;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.DeleteBtn.onClick.AddListener(OnDeleteBtn);
        }

        public void Set(Msg_MC_RoomUserInfo user)
        {
            _user = user;
            _cachedView.SetActiveEx(_user != null);
            if (_user == null)
            {
                Unload();
            }
            else
            {
                RefreshView();
            }
        }

        private void RefreshView()
        {
            _cachedView.NameTxt.text = GameATools.GetRawStr(_user.NickName, 6);
            UserManager.Instance.GetDataOnAsync(_user.UserGuid, user =>
            {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg, user.UserInfoSimple.HeadImgUrl,
                    _cachedView.DefaultCoverTexture);
            });
            bool isMyself = _user.UserGuid == LocalUser.Instance.UserGuid;
            _cachedView.BgSelectedObj.SetActive(isMyself);
            _cachedView.InGameObj.SetActive(_user.Status == EMCUserStatus.MCUS_InGame);
            _cachedView.DeleteBtn.SetActiveEx(!isMyself && LocalUser.Instance.MutiBattleData.IsMyTeam);
        }

        private void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.DefaultCoverTexture);
        }

        private void OnDeleteBtn()
        {
            RoomManager.Instance.SendKickTeam(_user.UserGuid);
        }
    }
}