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
            LocalUser.Instance.RelationUserList.Request(LocalUser.Instance.UserGuid,
                ERelationUserType.RUT_FollowEachOther, 0, _maxFollows, ERelationUserOrderBy.RUOB_Friendliness,
                EOrderType.OT_Asc, () =>
                {
                    _userInfoDetailList = LocalUser.Instance.RelationUserList.DataDetailList;
                    _hasInited = true;
                    _isRequesting = false;
                    if (!_isOpen)
                    {
                        return;
                    }
                    TempData();
                    LocalUser.Instance.RelationUserList.FriendList = _userInfoDetailList;
                    RefreshView();
                }, code => { _isRequesting = false; });
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