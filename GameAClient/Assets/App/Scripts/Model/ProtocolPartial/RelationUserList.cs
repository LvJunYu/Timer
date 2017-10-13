using System.Collections.Generic;

namespace GameA
{
    public partial class RelationUserList
    {
        private List<UserInfoDetail> _dataDetailList;

        public List<UserInfoDetail> DataDetailList
        {
            get { return _dataDetailList; }
        }
        
        protected override void OnSyncPartial()
        {
            if (_dataList == null) return;
            _dataDetailList = new List<UserInfoDetail>(_dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                _dataDetailList.Add(UserManager.Instance.UpdateData(_dataList[i]));
            }
        }
    }
}