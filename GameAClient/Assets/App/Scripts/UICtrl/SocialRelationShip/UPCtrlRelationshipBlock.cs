namespace GameA
{
    public class UPCtrlRelationshipBlock : UPCtrlRelationshipBase<UMCtrlRelationShortItem>
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestBlocks(LocalUser.Instance.UserGuid, () =>
            {
                _userInfoDetailList = LocalUser.Instance.RelationUserList.BlockList;
                _hasInited = true;
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
                TempData();
                //同步数据
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
                _userInfoDetailList[i].UserInfoSimple.UserId += 4000;
                _userInfoDetailList[i].UserInfoSimple.NickName =
                    "屏蔽" + _userInfoDetailList[i].UserInfoSimple.NickName;
                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.BlockedByMe = true;
                _userInfoDetailList[i] = UserManager.Instance.UpdateData(_userInfoDetailList[i]);
            }
            LocalUser.Instance.RelationUserList.BlockList = _userInfoDetailList;
        }
    }
}