using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlRelationshipFollow : UPCtrlRelationshipBase
    {
        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            LocalUser.Instance.RelationUserList.RequestMyFollows(() =>
            {
                _userInfoDetailList = LocalUser.Instance.RelationUserList.FollowList;
                HasInited = true;
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
//                TempData();
                RefreshView();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            }, code =>
            {
                _isRequesting = false;
                SocialGUIManager.ShowPopupDialog("请求数据失败。");
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlRelationLongItem();
            item.SetMenu(_menu);
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void TempData()
        {
//            base.TempData();
//            for (int i = 0; i < _userInfoDetailList.Count; i++)
//            {
//                _userInfoDetailList[i].UserInfoSimple.UserId += 1000;
//                _userInfoDetailList[i].UserInfoSimple.NickName =
//                    "关注" + _userInfoDetailList[i].UserInfoSimple.NickName;
//                _userInfoDetailList[i].UserInfoSimple.RelationWithMe.FollowedByMe = true;
//                _userInfoDetailList[i] = UserManager.Instance.UpdateData(_userInfoDetailList[i]);
//            }
//            LocalUser.Instance.RelationUserList.FollowList = _userInfoDetailList;
        }
    }
}