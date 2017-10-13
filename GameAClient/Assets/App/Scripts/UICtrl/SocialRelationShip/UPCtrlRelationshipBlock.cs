using SoyEngine.Proto;

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
            LocalUser.Instance.RelationUserList.Request(LocalUser.Instance.UserGuid, ERelationUserType.RUT_BlockByMe, 0,
                _maxFollows, ERelationUserOrderBy.RUOB_Friendliness, EOrderType.OT_Asc, () =>
                {
                    _userInfoDetailList = LocalUser.Instance.RelationUserList.DataDetailList;
                    _hasInited = true;
                    _isRequesting = false;
                    if (!_isOpen)
                    {
                        return;
                    }
                    TempData();
                    //同步数据
                    LocalUser.Instance.RelationUserList.BlockList = _userInfoDetailList;
                    RefreshView();
                }, code => { _isRequesting = false; });
        }

        protected override void TempData()
        {
            base.TempData();
            for (int i = 0; i < _userInfoDetailList.Count; i++)
            {
                _userInfoDetailList[i].UserInfoSimple.NickName =
                    "屏蔽" + _userInfoDetailList[i].UserInfoSimple.NickName;
                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.BlockedByMe = true;
            }
        }
    }
}