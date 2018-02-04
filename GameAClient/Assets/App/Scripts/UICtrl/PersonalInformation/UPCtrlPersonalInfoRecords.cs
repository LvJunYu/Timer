using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlPersonalInfoRecords : UPCtrlPersonalInfoBase
    {
        private const int PageSize = 10;
        private List<Record> _dataList;
        private WorldUserRecentRecordList _data = new WorldUserRecentRecordList();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.GridDataScrollers[(int) _menu - 1].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        private void RequestData(bool append = false)
        {
            if (_mainCtrl.UserInfoDetail == null) return;
            int startInx = 0;
            if (append)
            {
                startInx = _dataList.Count;
            }
            _data.Request(_mainCtrl.UserInfoDetail.UserInfoSimple.UserId, startInx, PageSize, () =>
            {
                _dataList = _data.AllList;
                if (_isOpen)
                {
                    RefreshView();
                }
            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("请求录像数据失败。");
            });
        }

        public override void RefreshView()
        {
            if (!_isOpen) return;
            if (_dataList == null)
            {
                _cachedView.EmptyObj.SetActive(true);
                _cachedView.GridDataScrollers[(int) _menu - 1].SetEmpty();
                return;
            }
            _cachedView.EmptyObj.SetActive(_dataList.Count == 0);
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

        public override void Clear()
        {
            base.Clear();
            _dataList = null;
        }
    }
}