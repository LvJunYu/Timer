using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInforRecords : UPCtrlPersonalInforBase
    {
        private const int PageSize = 10;
        protected List<Record> _dataList;
        private UserWorldProjectPlayHistoryList _data;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetTalkItemRenderer);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            RefreshView();
            RequestData();
        }

        public override void Close()
        {
            
            base.Close();
        }

        private void RequestData(bool append = false)
        {
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }
            _data.Request(LocalUser.Instance.Account.UserGuid, startInx, PageSize, () =>
            {
                if (!_isOpen)
                {
                    return;
                }
                RefreshView();
            }, code => { });
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu - 1].SetEmpty();
            }
            _cachedView.GridDataScrollers[(int) _menu - 1].SetItemCount(_dataList.Count);
        }

        protected void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _dataList.Count)
            {
                LogHelper.Error("OnItemRefresh Error Inx > count");
                return;
            }
            item.Set(_dataList[inx]);
            if (!_data.IsEnd)
            {
                if (inx > _dataList.Count - 2)
                {
                    RequestData(true);
                }
            }
        }

        protected virtual IDataItemRenderer GetTalkItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoRecord();
            item.Init(parent, _resScenary);
            return item;
        }
    }
}