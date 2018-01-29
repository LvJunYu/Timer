using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldMulti : UPCtrlWorldPanelBase
    {
        public const int PageSize = 16;
        private List<CardDataRendererWrapper<RoomInfo>> _contentList = new List<CardDataRendererWrapper<RoomInfo>>();

        private Dictionary<long, CardDataRendererWrapper<RoomInfo>> _dict =
            new Dictionary<long, CardDataRendererWrapper<RoomInfo>>();

        private bool _isShowingSearchRoom;
        private List<RoomInfo> _roomList;
        private CardDataRendererWrapper<RoomInfo> _curSelectRoom;

        public CardDataRendererWrapper<RoomInfo> CurSelectRoom
        {
            get { return _curSelectRoom; }
            set
            {
                if (value == _curSelectRoom) return;
                if (value == null)
                {
                    _curSelectRoom.IsSelected = false;
                    _curSelectRoom.BroadcastDataChanged();
                    _curSelectRoom = null;
                    _cachedView.MultiDetailPannel.SetActive(false);
                }
                else
                {
                    if (_curSelectRoom != null)
                    {
                        _curSelectRoom.IsSelected = false;
                        _curSelectRoom.BroadcastDataChanged();
                    }
                    _curSelectRoom = value;
                    _curSelectRoom.IsSelected = true;
                    _curSelectRoom.BroadcastDataChanged();
                    _cachedView.MultiDetailPannel.SetActive(true);
                    RefreshRoomInfo();
                }
            }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.JoinRoomBtn.onClick.AddListener(OnJoinRoomBtn);
            _cachedView.SearchRoomBtn.onClick.AddListener(OnSearchRoomBtn);
            _cachedView.QuickJoinBtn.onClick.AddListener(OnQuickJoinBtn);
            _cachedView.GridDataScrollers[(int) _menu].Set(OnItemRefresh, GetItemRenderer);
        }

        public override void Open()
        {
            base.Open();
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
            CurSelectRoom = null;
            _isShowingSearchRoom = false;
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        public override void RequestData(bool append = false)
        {
            if (!append)
            {
                CurSelectRoom = null;
            }
            RoomManager.Instance.SendQueryRoomList(append);
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
                    if (_dict.ContainsKey(_roomList[i].RoomId))
                    {
                    }
                    else
                    {
                        _dict.Add(_roomList[i].RoomId, w);
                    }
                }
            }
            _cachedView.GridDataScrollers[(int) _menu].SetItemCount(_contentList.Count);
            _cachedView.MultiDetailPannel.SetActive(_contentList.Count != 0);
            if (CurSelectRoom == null && _contentList.Count > 0)
            {
                OnItemClick(_contentList[0]);
            }
            else
            {
                RefreshRoomInfo();
            }
        }

        private void OnQuickJoinBtn()
        {
            RoomManager.Instance.SendRequestQuickPlay();
        }

        private void OnSearchRoomBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.SearchRoomInputField.text))
            {
                RequestData();
            }
            else
            {
                long roomId;
                if (long.TryParse(_cachedView.SearchRoomInputField.text, out roomId))
                {
                    RoomManager.Instance.SendQueryRoom(roomId);
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("请输入正确的房间号！");
                }
            }
        }

        private void OnItemClick(CardDataRendererWrapper<RoomInfo> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            CurSelectRoom = item;
        }

        private void RefreshRoomInfo()
        {
            if (CurSelectRoom == null) return;
            var project = CurSelectRoom.Content.Project;
            if (project == null) return;
            var netData = project.NetData;
            if (netData == null) return;
            _cachedView.DescTxt.text = project.ShowSummary;
            _cachedView.PlayerCount.text = CurSelectRoom.Content.MaxUserCount.ToString();
            _cachedView.LifeCount.text = netData.GetLifeCount();
            _cachedView.ReviveTime.text = netData.GetReviveTime();
            _cachedView.ReviveProtectTime.text = netData.GetReviveProtectTime();
            _cachedView.TimeLimit.text = netData.GetTimeLimit();
            _cachedView.TimeOverCondition.text = netData.GetTimeOverCondition();
            _cachedView.WinScoreCondition.text = netData.WinScore.ToString();
            _cachedView.ArriveScore.text = netData.ArriveScore.ToString();
            _cachedView.CollectGemScore.text = netData.CollectGemScore.ToString();
            _cachedView.KillMonsterScore.text = netData.KillMonsterScore.ToString();
            _cachedView.KillPlayerScore.text = netData.KillPlayerScore.ToString();
            _cachedView.WinScoreCondition.SetActiveEx(netData.ScoreWinCondition);
//            _cachedView.ArriveScore.SetActiveEx(netData.ArriveScore > 0);
//            _cachedView.CollectGemScore.SetActiveEx(netData.CollectGemScore > 0);
//            _cachedView.KillMonsterScore.SetActiveEx(netData.KillMonsterScore > 0);
//            _cachedView.KillPlayerScore.SetActiveEx(netData.KillPlayerScore > 0);
        }

        private void OnJoinRoomBtn()
        {
            if (CurSelectRoom != null)
            {
                RoomManager.Instance.SendRequestJoinRoom(CurSelectRoom.Content.RoomId);
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
                if (!RoomManager.Instance.IsEnd && !_isShowingSearchRoom)
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
            if (CurSelectRoom != null && val == CurSelectRoom.Content.RoomId)
            {
                RefreshRoomInfo();
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

        public void OnQueryRoomRet(Msg_MC_QueryRoom msg)
        {
            _isShowingSearchRoom = true;
            CurSelectRoom = null;
            RefreshView();
        }

        public void OnRoomListChanged()
        {
            _isShowingSearchRoom = false;
            RefreshView();
        }

        public void OnJoinRoomFail()
        {
            _isShowingSearchRoom = false;
            RequestData();
        }

        public void OnChangeToAppMode()
        {
            _isShowingSearchRoom = false;
            RequestData();
        }
    }
}