using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlRelationshipFriends : UPCtrlRelationshipBase<UMCtrlRelationLongItem>
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestFriends(LocalUser.Instance.UserGuid, () =>
            {
                _userInfoDetailList = LocalUser.Instance.RelationUserList.FriendList;
                _hasInited = true;
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
                TempData();
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
                _userInfoDetailList[i].UserInfoSimple.NickName =
                    "好友" + _userInfoDetailList[i].UserInfoSimple.NickName;
                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.IsFriend = true;
            }
        }
    }
}