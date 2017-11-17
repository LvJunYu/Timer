using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class Table_QQHallGrowAwardStatus
    {
        public Table_QQHallGrowAward GrowAward;
        public EQQGameRewardStatus Status;
        public EQQGamePrivilegeType Type;
        public Table_QQHallGrowAwardStatus(Table_QQHallGrowAward award,EQQGameRewardStatus status ,EQQGamePrivilegeType type)
        {
            GrowAward = award;
            Status = status;
            Type = type;
        }
    }

    public class UPCtrlQQGrowAward : UPCtrlQQHallBase, IOnChangeHandler<long>
    {
        private Dictionary<int, Table_QQHallGrowAward> _growAwards = TableManager.Instance.Table_QQHallGrowAwardDic;
        private RectTransform _contentRect;
        private List<Table_QQHallGrowAward> _contentList;
        private List<int> _statusList = new List<int>();
        private List<Table_QQHallGrowAwardStatus>  _hallGrowAwardStatuses = new List<Table_QQHallGrowAwardStatus>();
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            Init();
            _cachedView.GridDataScroller.Set(OnItemRefresh, GetItemRenderer);
         
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            _isOpen = true;
          
            RefreshView();
        }

        private void ReqestData()
        {
            LocalUser.Instance.QqGameReward.Request(0,
                () =>
                {
                    if (_isOpen)
                    {
                        _statusList = LocalUser.Instance.QqGameReward.HallGrow;
                        
                        _hallGrowAwardStatuses.Clear();
                        for (int i = 0; i < _contentList.Count; i++)
                        {
                            if (_statusList.Count <i+1)
                            {
                                _statusList.Add((int)EQQGameRewardStatus.QGRS_Unsatisfied);
                            }
                           _hallGrowAwardStatuses.Add(new Table_QQHallGrowAwardStatus (_contentList[i],
                                (EQQGameRewardStatus)_statusList[i],EQQGamePrivilegeType.QGPT_Hall )); 
                        }
                        RefreshView();
                    }
                },
                code => { });
        }

        public override void Close()
        {
            base.Close();
            _isOpen = false;
        }

        private void Init()
        {
            _contentList = new List<Table_QQHallGrowAward>();
            foreach (var item in _growAwards)
            {
                _contentList.Add(item.Value);
            }
        }

        public void OnChangeHandler(long val)
        {
            if (_isOpen)
            {
                RefreshView();
            }
        }

        protected IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlQQAwardGrow();
            item.Init(parent, _resScenary);
            return item;
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (!_isOpen)
            {
                item.Set(null);
            }
            else
            {
                if (inx >= _hallGrowAwardStatuses.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_hallGrowAwardStatuses[inx]);
            }
        }
        protected  void RefreshView()
        {
           
            _cachedView.GridDataScroller.SetItemCount(_hallGrowAwardStatuses.Count);
        }
    }
}