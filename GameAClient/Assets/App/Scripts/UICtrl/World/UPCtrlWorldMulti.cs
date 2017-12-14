using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UPCtrlWorldMulti : UPCtrlWorldPanelBase
    {
        public const int PageSize = 16;
        private List<CardDataRendererWrapper<RoomInfo>> _contentList = new List<CardDataRendererWrapper<RoomInfo>>();

        private Dictionary<long, CardDataRendererWrapper<RoomInfo>> _dict =
            new Dictionary<long, CardDataRendererWrapper<RoomInfo>>();

        private List<RoomInfo> _roomList;
        public CardDataRendererWrapper<RoomInfo> CurSelectRoom;


        public override void Open()
        {
            base.Open();
            _cachedView.GridDataScrollers[(int) _menu].ContentPosition = Vector2.zero;
            CurSelectRoom = null;
            RequestData();
            RefreshView();
        }

        public override void Close()
        {
            _cachedView.GridDataScrollers[(int) _menu].RefreshCurrent();
            base.Close();
        }

        public void Update()
        {
            
        }
        
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
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
            if (CurSelectRoom == null && _contentList.Count > 0)
            {
                _contentList[0].IsSelected = true;
                _contentList[0].BroadcastDataChanged();
                CurSelectRoom = _contentList[0];
                RefreshRoomInfo();
            }
        }

        private void OnItemClick(CardDataRendererWrapper<RoomInfo> item)
        {
            if (item == null || item.Content == null)
            {
                return;
            }
            if (CurSelectRoom != null)
            {
                if (item.Content.RoomId == CurSelectRoom.Content.RoomId) return;
                CurSelectRoom.IsSelected = false;
                CurSelectRoom.BroadcastDataChanged();
            }
            item.IsSelected = true;
            item.BroadcastDataChanged();
            CurSelectRoom = item;
            RefreshRoomInfo();
        }

        private void RefreshRoomInfo()
        {
            if (CurSelectRoom == null) return;
            var project = CurSelectRoom.Content.Project;
            var netData = project.NetData;
            if (netData == null) return;
            _cachedView.DescTxt.text = project.Summary;
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
//            _cachedView.ArriveScore.SetActiveEx(Game.PlayMode.Instance.SceneState.FinalCount > 0);
//            _cachedView.CollectGemScore.SetActiveEx(Game.PlayMode.Instance.SceneState.TotalGem > 0);
//            _cachedView.KillMonsterScore.SetActiveEx(Game.PlayMode.Instance.SceneState.MonsterCount > 0);
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