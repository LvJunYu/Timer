using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class AdvProgressData
    {
        private List<UserInfoDetail> _friendsDetailDataList;

        public List<UserInfoDetail> FriendsDetailDataList
        {
            get
            {
                if (_friendsDetailDataList == null)
                {
                    OnSyncPartial();
                }
                return _friendsDetailDataList;
            }
        }

        protected override void OnSyncPartial(Msg_AdvProgressData msg )
        {
            if (_friendsDataList == null) return;
            _friendsDetailDataList = new List<UserInfoDetail>(_friendsDataList.Count);
            for (int i = 0; i < _friendsDataList.Count; i++)
            {
                _friendsDetailDataList.Add(UserManager.Instance.UpdateData(msg.FriendsDataList[i]));
            }
        }
    }
}