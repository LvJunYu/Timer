using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlQQGrowAward : UPCtrlQQHallBase, IOnChangeHandler<long>
    {
        private Dictionary<int, Table_QQHallGrowAward> _growAwards = TableManager.Instance.Table_QQHallGrowAwardDic;
        private RectTransform _contentRect;
        private List<Table_QQHallGrowAward> _contentList;

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
                if (inx >= _contentList.Count)
                {
                    LogHelper.Error("OnItemRefresh Error Inx > count");
                    return;
                }
                item.Set(_contentList[inx]);
            }
        }
        protected  void RefreshView()
        {
           
            _cachedView.GridDataScroller.SetItemCount(_contentList.Count);
        }
    }
}