using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldMulti : UPCtrlWorldPanelBase
    {
        public const int PageSize = 8;
        private List<CardDataRendererWrapper<RoomInfo>> _contentList = new List<CardDataRendererWrapper<RoomInfo>>();

        private Dictionary<long, CardDataRendererWrapper<RoomInfo>> _dict =
            new Dictionary<long, CardDataRendererWrapper<RoomInfo>>();

        private UMCtrlProject.ECurUI _eCurUi;
        private List<RoomInfo> _roomList;
        private RoomInfo _curSelectRoom;

        public override void Open()
        {
            base.Open();
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _eCurUi = UMCtrlProject.ECurUI.Multi;
            _cachedView.JoinRoomBtn.onClick.AddListener(OnJoinRoomBtn);
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void RequestData(bool append = false)
        {
            RoomManager.Instance.RequestRoomList(append);
        }

        public override void RefreshView()
        {
            _roomList = RoomManager.Instance.RoomList;
            _cachedView.EmptyObj.SetActiveEx(_roomList.Count == 0);
            _contentList.Clear();
            _dict.Clear();
            _contentList.Capacity = Mathf.Max(_contentList.Capacity, _roomList.Count);
            for (int i = 0; i < _roomList.Count; i++)
            {
                if (!_dict.ContainsKey(_roomList[i].RoomId))
                {
                    CardDataRendererWrapper<RoomInfo> w =
                        new CardDataRendererWrapper<RoomInfo>(_roomList[i], OnItemClick);
                    _contentList.Add(w);
                    _dict.Add(_roomList[i].RoomId, w);
                }
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
            _cachedView.MultiDetailPannel.SetActive(_contentList.Count != 0);
            if (_curSelectRoom == null && _contentList.Count > 0)
            {
                _contentList[0].IsSelected = true;
                _contentList[0].BroadcastDataChanged();
                _curSelectRoom = _contentList[0].Content;
                RefreshRoomInfo();
            }
        }

        private void OnItemClick(CardDataRendererWrapper<RoomInfo> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            item.IsSelected = true;
            item.BroadcastDataChanged();
            _curSelectRoom = item.Content;
            RefreshRoomInfo();
        }

        private void RefreshRoomInfo()
        {
        }

        private void OnJoinRoomBtn()
        {
            if (_curSelectRoom != null)
            {
                RoomManager.Instance.SendRequestJoinRoom(_curSelectRoom.RoomId);
            }
        }

        private IDataItemRenderer GetItemRenderer(RectTransform parent)
        {
            var item = new UMCtrlMultiRoom();
            item.Init(parent, _resScenary);
            return item;
        }

        protected override void OnItemRefresh(IDataItemRenderer item, int inx)
        {
            if (_unload)
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

        public override void OnChangeHandler(long val)
        {
            CardDataRendererWrapper<RoomInfo> w;
            if (_dict.TryGetValue(val, out w))
            {
                w.BroadcastDataChanged();
            }
        }

        public override void Clear()
        {
            base.Clear();
            _unload = true;
            _contentList.Clear();
            _dict.Clear();
            _roomList = null;
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
        }
    }
}