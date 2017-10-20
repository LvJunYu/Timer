using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInfoRecords : UPCtrlPersonalInfoBase
    {
        private const int PageSize = 10;
        protected List<Record> _dataList;
        protected bool _isRequesting;
        private UserWorldProjectPlayHistoryList _data;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void OnDestroy()
        {
            _dataList = null;
            base.OnDestroy();
        }

        public override void Open()
        {
            base.Open();
            _data = AppData.Instance.WorldData.UserPlayHistoryList;
            RequestData();
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
            if (_isRequesting || _mainCtrl.UserInfoDetail == null) return;
            _isRequesting = true;
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }
            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize, () =>
            {
                _isRequesting = false;
                if (!_isOpen)
                {
                    return;
                }
                RefreshView();
            }, code =>
            {
                _isRequesting = false;
                SocialGUIManager.ShowPopupDialog("请求数据失败");
            });
        }

        public void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.GridDataScrollers[(int) _menu - 1].SetEmpty();
                return;
            }
            _cachedView.GridDataScrollers[(int) _menu - 1].SetItemCount(_dataList.Count);
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
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

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlPersonalInfoRecord();
            item.Init(parent, _resScenary);
            return item;
        }
    }
}