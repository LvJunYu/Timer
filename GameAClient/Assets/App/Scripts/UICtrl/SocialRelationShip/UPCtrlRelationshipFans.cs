using SoyEngine.Proto;

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
            LocalUser.Instance.RelationUserList.Request(LocalUser.Instance.UserGuid, ERelationUserType.RUT_FollowMe, 0,
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
                    LocalUser.Instance.RelationUserList.FanList = _userInfoDetailList;
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
            }
        }
    }
}