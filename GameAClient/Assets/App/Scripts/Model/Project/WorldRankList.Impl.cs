/********************************************************************
** Filename : WorldProjectRecordRankList.cs
** Author : quan
** Date : 6/6/2017 5:13 PM
** Summary : WorldProjectRecordRankList.cs
***********************************************************************/

using System;
using System.Collections.Generic;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public partial class WorldRankList
    {
        private List<WorldRankItem.WorldRankHolder> _advenDayList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _advenWeekList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _advenMonthList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _advenTotalList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _craftsDayList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _craftsWeekList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _craftsMonthList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder> _craftsTotalList = new List<WorldRankItem.WorldRankHolder>();
        private List<WorldRankItem.WorldRankHolder>  _curList = new List<WorldRankItem.WorldRankHolder>();
        public bool IsEnd { get; private set; }

        public List<WorldRankItem.WorldRankHolder> AdvenDayList
        {
            get { return _advenDayList; }
        }

        public List<WorldRankItem.WorldRankHolder> AdvenWeekList
        {
            get { return _advenWeekList; }
        }

        public List<WorldRankItem.WorldRankHolder> AdvenMonthList
        {
            get { return _advenMonthList; }
        }

        public List<WorldRankItem.WorldRankHolder> AdvenTotalList
        {
            get { return _advenTotalList; }
        }

        public List<WorldRankItem.WorldRankHolder> CraftsDayList
        {
            get { return _craftsDayList; }
        }

        public List<WorldRankItem.WorldRankHolder> CraftsWeekList
        {
            get { return _craftsWeekList; }
        }

        public List<WorldRankItem.WorldRankHolder> CraftsMonthList
        {
            get { return _craftsMonthList; }
        }

        public List<WorldRankItem.WorldRankHolder> CraftsTotalList
        {
            get { return _craftsTotalList; }
        }

        public List<WorldRankItem.WorldRankHolder> CurList
        {
            get { return _curList; }
        }

//        public List<WorldRankItem.WorldRankHolder> AllList
//        {
//            get { return _allList; }
//        }
        protected override void OnSyncPartial(Msg_SC_DAT_WorldRankList msg)
        {
            base.OnSyncPartial();
            switch (CS_Type)
            {
                case EWorldRankType.WRT_Player:
                    switch (CS_TimeBucket)
                    {
                        case   ERankTimeBucket.RTB_Day:
                            _curList = _advenDayList;
                            break;
                        case   ERankTimeBucket.RTB_Week:
                            _curList = _advenWeekList;
                            break;
                        case   ERankTimeBucket.RTB_Month:
                            _curList = _advenMonthList;
                            break;
                        case   ERankTimeBucket.RTB_Total:
                            _curList = _advenTotalList;
                            break;
                    }
                    break;
                case EWorldRankType.WRT_Creator:
                    switch (CS_TimeBucket)
                    {
                        case   ERankTimeBucket.RTB_Day:
                            _curList = _craftsDayList;
                            break;
                        case   ERankTimeBucket.RTB_Week:
                            _curList = _craftsWeekList;
                            break;
                        case   ERankTimeBucket.RTB_Month:
                            _curList = _craftsMonthList;
                            break;
                        case   ERankTimeBucket.RTB_Total:
                            _curList = _craftsTotalList;
                            break;     
                    }
                    break;
            }
            if (_resultCode == (int) ECachedDataState.CDS_None
                || _resultCode == (int) ECachedDataState.CDS_Uptodate)
            {
                if (!_inited)
                {
                    IsEnd = true;
//                    MessengerAsync.Broadcast(EMessengerType.OnProjectRecordRankChanged);
                }
                return;
            }
          
            if (_resultCode == (int) ECachedDataState.CDS_Recreate)
            {
                _curList.Clear();
            }
            int i = _curList.Count;
            _rankList.ForEach(r => _curList.Add(new WorldRankItem.WorldRankHolder(r, i++)));
           
            IsEnd = _curList.Count < _cs_maxCount;
//            MessengerAsync.Broadcast(EMessengerType.OnProjectRecordRankChanged);
        }
    }
}