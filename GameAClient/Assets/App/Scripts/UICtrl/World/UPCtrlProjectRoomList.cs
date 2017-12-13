using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlProjectRoomList : UPCtrlProjectDetailBase
    {
        private List<RoomInfo> _dataList;
        private List<CardDataRendererWrapper<RoomInfo>> _contentList = new List<CardDataRendererWrapper<RoomInfo>>();

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.RoomGridDataScroller.Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.RoomGridDataScroller.ContentPosition = Vector2.zero;
            base.Close();
        }

        protected override void RequestData(bool append = false)
        {
            if (_mainCtrl.Project == null) return;
            RoomManager.Instance.RequestRoomList(append, _mainCtrl.Project.ProjectId);
        }

        protected override void RefreshView()
        {
            _dataList = RoomManager.Instance.RoomList;
            _cachedView.RoomGridDataScroller.OnViewportSizeChanged();
            _contentList.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _dataList.Count);
            for (int i = 0; i < _dataList.Count; i++)
            {
                CardDataRendererWrapper<RoomInfo> w = new CardDataRendererWrapper<RoomInfo>(_dataList[i], OnItemClick);
                _contentList.Add(w);
            }
            _cachedView.RoomGridDataScroller.SetItemCount(_contentList.Count);
        }

        private void OnItemClick(CardDataRendererWrapper<RoomInfo> item)
        {
            
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlProjectRoom();
            item.Init(parent, _resScenary);
            return item;
        }

        private void OnItemRefresh(IDataItemRenderer item, int inx)
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
                if (!RoomManager.Instance.IsEnd)
                {
                    if (inx > _contentList.Count - 2)
                    {
                        RequestData(true);
                    }
                }
            }
        }

        public override void Clear()
        {
            _dataList = null;
            _contentList.Clear();
            _cachedView.RoomGridDataScroller.ContentPosition = Vector2.zero;
        }
    }
}