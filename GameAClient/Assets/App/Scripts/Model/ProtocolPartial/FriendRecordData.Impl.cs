using System.Collections.Generic;

namespace GameA
{
    public partial class FriendRecordData  {
        private List<UserInfoDetail> _userDetailList;

        public List<UserInfoDetail> UserDetailList
        {
            get { return _userDetailList; }
        }

        protected override void OnSyncPartial()
        {
            if (_userList == null) return;
            _userDetailList = new List<UserInfoDetail>(_userList.Count);
            for (int i = 0; i < _userList.Count; i++)
            {
                _userDetailList.Add(UserManager.Instance.UpdateData(_userList[i]));
            }
        }
    }
}