using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlRelationshipFollow : UPCtrlRelationshipBase<UMCtrlRelationLongItem>
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestFollows(LocalUser.Instance.UserGuid, () =>
            {
                _userInfoDetailList = LocalUser.Instance.RelationUserList.FollowList;
                _hasInited = true;
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
                if (_userInfoDetailList == null || _userInfoDetailList.Count == 0)
                {
                    TempData();
                }
                RefreshView();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                _isRequesting = false;
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        protected override void TempData()
        {
            base.TempData();
            for (int i = 0; i < _userInfoDetailList.Count; i++)
            {
                _userInfoDetailList[i].UserInfoSimple.UserId += 1000;
                _userInfoDetailList[i].UserInfoSimple.NickName =
                    "关注" + _userInfoDetailList[i].UserInfoSimple.NickName;
                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.FollowedByMe = true;
                _userInfoDetailList[i] = UserManager.Instance.UpdateData(_userInfoDetailList[i]);
            }
            LocalUser.Instance.RelationUserList.FollowList = _userInfoDetailList;
        }
    }
}