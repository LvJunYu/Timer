namespace GameA
{
    public class UPCtrlRelationshipFans : UPCtrlRelationshipBase<UMCtrlRelationLongItem>
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestFans(LocalUser.Instance.UserGuid, () =>
            {
                _userInfoDetailList = LocalUser.Instance.RelationUserList.FanList;
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
                _userInfoDetailList[i].UserInfoSimple.UserId += 2000;
                _userInfoDetailList[i].UserInfoSimple.NickName =
                    "粉丝" + _userInfoDetailList[i].UserInfoSimple.NickName;
                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.FollowMe = true;
                _userInfoDetailList[i] = UserManager.Instance.UpdateData(_userInfoDetailList[i]);
            }
            LocalUser.Instance.RelationUserList.FanList = _userInfoDetailList;
        }
    }
}