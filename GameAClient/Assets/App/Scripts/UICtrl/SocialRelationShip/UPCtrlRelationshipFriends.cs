using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlRelationshipFriends : UPCtrlRelationshipBase
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            _data = LocalUser.Instance.RelationUserList;
            _data.Request(LocalUser.Instance.UserGuid, ERelationUserType.RUT_FollowEachOther, 0, _maxFollows,
                ERelationUserOrderBy.RUOB_Friendliness, EOrderType.OT_Asc, () =>
                {
                    _userInfoDetailList = _data.DataDetailList;
                    _hasInited = true;
                    _isRequesting = false;
                    if (!_isOpen)
                    {
                        return;
                    }
                    TempData();
                    RefreshView();
                }, code => { _isRequesting = false; });
        }
    }
}