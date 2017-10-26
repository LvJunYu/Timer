using System.Collections.Generic;
using SoyEngine;
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

        private void OnSearchBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.SeachInputField.text))
            {
                RequestData();
                return;
            }
            if (CheckTools.CheckNickName(_cachedView.SeachInputField.text) != CheckTools.ECheckNickNameResult.Success)
            {
                SocialGUIManager.ShowPopupDialog("您输入的昵称格式错误");
                return;
            }
            RemoteCommands.SearchUser(_cachedView.SeachInputField.text, ret =>
            {
                if (ret.ResultCode == (int) ESearchUserCode.SUC_Success)
                {
                    ShowSearchUser(UserManager.Instance.UpdateData(ret.Data));
                }
                else if (ret.ResultCode == (int) ESearchUserCode.SUC_NotExsit)
                {
                    SocialGUIManager.ShowPopupDialog(string.Format("没有找到昵称为{0}的玩家。",
                        _cachedView.SeachInputField.text));
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("查找失败。");
                }
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("查找失败。");
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