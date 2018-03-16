using SoyEngine;
using UnityEngine.Events;

namespace GameA
{
    public class USCtrlFriendItem : USCtrlBase<USViewFriendItem>
    {
        private UserInfoDetail _data;

        public UserInfoDetail Data
        {
            get { return _data; }
        }

        public void SetSelected(bool value)
        {
            _cachedView.SelectedObj.SetActive(value);
        }

        public void AddBtnListener(UnityAction action)
        {
            _cachedView.SelectBtn.onClick.AddListener(action);
        }

        public void SetData(UserInfoDetail userInfoDetail)
        {
            _data = userInfoDetail;
            _cachedView.NameTxt.text = GameATools.GetRawStr(userInfoDetail.UserInfoSimple.NickName, 6);
        }

        public void SetEnable(bool value)
        {
            _cachedView.SetActiveEx(value);
        }
    }
}