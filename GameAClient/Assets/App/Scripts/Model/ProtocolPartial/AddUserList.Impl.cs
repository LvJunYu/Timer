using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class AddUserList
    {
        private List<UserInfoDetail> _dataDetailList;

        public List<UserInfoDetail> DataDetailList
        {
            get { return _dataDetailList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_AddUserList msg)
        {
            if (_dataList == null) return;
            _dataDetailList = new List<UserInfoDetail>(_dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                _dataDetailList.Add(UserManager.Instance.UpdateData(msg.DataList[i]));
            }
        }
    }
}