using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public class UPCtrlRelationshipAddNew : UPCtrlRelationshipBase<UMCtrlRelationShortItem>
    {
        private AddUserList _addDataAddUserList;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.SearchBtn.onClick.AddListener(OnSearchBtn);
        }

        protected override void RequestData()
        {
            if (_isRequesting)
            {
                return;
            }
            _addDataAddUserList = LocalUser.Instance.AddUserList;
            _isRequesting = true;
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, string.Empty);
            _addDataAddUserList.Request(LocalUser.Instance.UserGuid, () =>
            {
                _userInfoDetailList = _addDataAddUserList.DataDetailList;
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
                TempData();
                RefreshView();
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            });
        }

        private void OnSearchBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.SeachInputField.text))
            {
                RequestData();
                return;
            }
            SearchUser searchUser = new SearchUser();
            searchUser.Request(_cachedView.SeachInputField.text, () => ShowSearchUser(searchUser.DataDetail), code =>
            {
                //临时数据
                UserInfoSimple user = new UserInfoSimple();
                user.NickName = _cachedView.SeachInputField.text;
                user.Sex = ESex.S_Female;
                ShowSearchUser(UserManager.Instance.UpdateData(user));
            });
        }

        private void ShowSearchUser(UserInfoDetail user)
        {
            if (_userInfoDetailList == null)
            {
                _userInfoDetailList = new List<UserInfoDetail>();
            }
            _userInfoDetailList.Clear();
            _userInfoDetailList.Add(user);
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_userInfoDetailList.Count);
        }
    }
}