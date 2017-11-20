using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQBlueGrowAward : UPCtrlQQBlueBase, IOnChangeHandler<long>
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
            _cachedView.GrowAwardDataScroller.Set(OnItemRefresh, GetItemRenderer);
         
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
            ReqestData();
        }

        private void ReqestData()
        {
            LocalUser.Instance.QqGameReward.Request(0,
                () =>
                {
                    SetContent(true);
                },
                code =>
                {
                   SetContent(false);
                });
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
            _cachedView.GrowAwardDataScroller.SetItemCount(_hallGrowAwardStatuses.Count);
        }

        private void SetContent(bool success)
        {
            if (_isOpen)
            {
                _statusList = LocalUser.Instance.QqGameReward.BlueGrow;
                if (_statusList == null)
                {
                    _statusList = new List<int>();
                }
                        
                _hallGrowAwardStatuses.Clear();
                for (int i = 0; i < _contentList.Count; i++)
                {
                    if (_statusList.Count <i+1)
                    {
                        _statusList.Add((int)EQQGameRewardStatus.QGRS_Unsatisfied);
                    }
                    if (success)
                    {
                        _hallGrowAwardStatuses.Add(new Table_QQHallGrowAwardStatus (_contentList[i],
                            (EQQGameRewardStatus)_statusList[i] ,EQQGamePrivilegeType.QGPT_BlueVip ));  
                    }
                    else
                    {
                        _hallGrowAwardStatuses.Add(new Table_QQHallGrowAwardStatus (_contentList[i],
                            EQQGameRewardStatus.QGRS_Unsatisfied ,EQQGamePrivilegeType.QGPT_BlueVip ));  
                    }
              
                }
                RefreshView();   
            }
        }
    }
}