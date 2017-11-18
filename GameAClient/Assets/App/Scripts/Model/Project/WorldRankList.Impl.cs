using System.Collections.Generic;
using SoyEngine.Proto;

namespace GameA
{
    public partial class WorldRankList
    {
        private const int _maxCount = 200; //总榜最大人数
        private List<WorldRankItem.WorldRankHolder> _allList = new List<WorldRankItem.WorldRankHolder>();
        public bool IsEnd { get; private set; }

        public List<WorldRankItem.WorldRankHolder> AllList
        {
            get { return _allList; }
        }

        protected override void OnSyncPartial(Msg_SC_DAT_WorldRankList msg)
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
            IsEnd = _rankList.Count < _cs_maxCount;
            int curRank = _allList.Count;
            for (int j = 0; j < _rankList.Count; j++)
            {
                if (curRank == _maxCount)
                {
                    IsEnd = true;
                    break;
                }
                _allList.Add(new WorldRankItem.WorldRankHolder(_rankList[j], curRank++));
            }
        }
    }
}