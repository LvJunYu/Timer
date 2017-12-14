using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlMultiRoom : UMCtrlBase<UMViewMultiRoom>, IDataItemRenderer
    {
        private CardDataRendererWrapper<RoomInfo> _wrapper;
        private static string _countFormat = "{0}/{1}";
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
            DictionaryTools.SetContentText(_cachedView.Title, room.Project.Name);
            DictionaryTools.SetContentText(_cachedView.RoomId, room.RoomId.ToString());
            DictionaryTools.SetContentText(_cachedView.PlayCountTxt,
                string.Format(_countFormat, room.UserCount, room.MaxUserCount));
            if (room.Project.IconPath != null)
            {
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, room.Project.IconPath,
                    _cachedView.DefaultCoverTexture);
            }
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }
    }
}