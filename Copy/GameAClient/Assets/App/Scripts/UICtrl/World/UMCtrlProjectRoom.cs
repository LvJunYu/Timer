using GameA.Game;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlProjectRoom : UMCtrlBase<UMViewProjectRoom>, IDataItemRenderer
    {
        private static string _countFormat = "{0}/{1}";
        private CardDataRendererWrapper<RoomInfo> _wrapper;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _wrapper.Content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.JoinRoomBtn.onClick.AddListener(OnJoinRoomBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.JoinRoomBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnJoinRoomBtn()
        {
            if (_wrapper != null)
            {
                RoomManager.Instance.SendRequestJoinRoom(_wrapper.Content.RoomId);
            }
        }

        public void Set(object obj)
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<RoomInfo>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }
            RefreshView();
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                Unload();
                return;
            }
            var room = _wrapper.Content;
            _cachedView.RoomId.text = room.RoomId.ToString();
            _cachedView.PlayerCount.text = string.Format(_countFormat, room.UserCount, room.MaxUserCount);
        }

        public void Unload()
        {
        }
    }
}