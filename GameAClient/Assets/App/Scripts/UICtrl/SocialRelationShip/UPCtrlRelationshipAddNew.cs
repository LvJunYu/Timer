using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlRelationshipAddNew : UPCtrlRelationshipBase
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

        protected override IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlRelationShortItem();
            item.SetMenu(_menu);
            item.Init(parent, _resScenary);
            return item;
        }

        private void OnSearchBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.SeachInputField.text))
            {
                RequestData();
                return;
            }
            long shortId;
            if (long.TryParse(_cachedView.SeachInputField.text, out shortId))
            {
                RemoteCommands.SearchUserByShortId(shortId, msg =>
                {
                    if (msg.ResultCode == (int) ESearchUserByShortIdCode.SUBSIC_Success)
                    {
                        ShowSearchUser(UserManager.Instance.UpdateData(msg.Data));
                    }
                    else if (msg.ResultCode == (int) ESearchUserByShortIdCode.SUBSIC_NotExsit)
                    {
                        SocialGUIManager.ShowPopupDialog(string.Format("没有找到ID为{0}的玩家。", shortId));
                    }
                    else
                    {
                        SocialGUIManager.ShowPopupDialog("查找失败。");
                    }
                }, code => { SocialGUIManager.ShowPopupDialog("查找失败。"); });
            }
            else
            {
                SocialGUIManager.ShowPopupDialog("请输入正确玩家ID");
            }
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