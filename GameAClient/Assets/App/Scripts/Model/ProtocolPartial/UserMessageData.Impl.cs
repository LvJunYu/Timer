using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class UserMessageData
    {
        private List<UserMessage> _allList = new List<UserMessage>();
        public bool IsEnd { get; private set; }

        public List<UserMessage> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial()
        {
            base.OnSyncPartial();
            if (_resultCode == (int) ECachedDataState.CDS_None
                || _resultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    IsEnd = true;
                }
                return;
            }
            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                _allList.Clear();
            }
            _allList.AddRange(_dataList);
            _dataList.Sort((r1, r2) => -r1.CreateTime.CompareTo(r2.CreateTime));
            IsEnd = _dataList.Count < _cs_maxCount;
        }
    }
}