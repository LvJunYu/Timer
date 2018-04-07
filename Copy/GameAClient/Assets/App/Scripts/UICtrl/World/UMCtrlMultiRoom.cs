using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlMultiRoom : UMCtrlBase<UMViewMultiRoom>, IDataItemRenderer
    {
        private CardDataRendererWrapper<RoomInfo> _wrapper;
        private const string _countFormat = "{0}/{1}";
        private const string WaitStr = "等待中";
        private const string HasStartedStr = "游戏中";
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
            _cachedView.Button.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }

            base.OnDestroy();
        }

        private void OnCardClick()
        {
            _wrapper.FireOnClick();
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

            _cachedView.SelectObj.SetActive(_wrapper.IsSelected);
            var room = _wrapper.Content;
            if (room.Project != null)
            {
                DictionaryTools.SetContentText(_cachedView.Title, room.Project.Name);
                if (room.Project.IconPath != null)
                {
                    ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, room.Project.IconPath,
                        _cachedView.DefaultCoverTexture);
                }
            }

            DictionaryTools.SetContentText(_cachedView.RoomId, room.RoomId.ToString());
            DictionaryTools.SetContentText(_cachedView.StateTxt, room.HasStarted ? HasStartedStr : WaitStr);
            DictionaryTools.SetContentText(_cachedView.PlayCountTxt,
                string.Format(_countFormat, room.UserCount, room.MaxUserCount));
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }
    }
}