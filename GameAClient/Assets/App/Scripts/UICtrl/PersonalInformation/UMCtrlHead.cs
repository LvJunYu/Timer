using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlHead : UMCtrlBase<UMViewHead>, IDataItemRenderer
    {
        private string _url;
        private UICtrlHeadPhotoChoose _uiCtrlHeadPhotoChoose;
        public int Index { get; set; }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _url; }
        }

        public void Set(object data)
        {
            if (data == null)
            {
                Unload();
                return;
            }
            _url = data.ToString();
            _cachedView.HeadTog.isOn = _url == _uiCtrlHeadPhotoChoose.CurHeadUrl;
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.HeadImg, _url, _cachedView.DefaultHeadTexture);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.HeadImg, _cachedView.DefaultHeadTexture);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.HeadTog.onValueChanged.AddListener(OnHeadTogValueChanged);
            _cachedView.HeadTog.group = _uiCtrlHeadPhotoChoose.TogGroup;
        }

        protected override void OnDestroy()
        {
            Unload();
            base.OnDestroy();
        }

        private void OnHeadTogValueChanged(bool arg0)
        {
            if (arg0)
            {
                _uiCtrlHeadPhotoChoose.CurHeadUrl = _url;
            }
        }

        public void SetUICtrl(UICtrlHeadPhotoChoose uiCtrlHeadPhotoChoose)
        {
            _uiCtrlHeadPhotoChoose = uiCtrlHeadPhotoChoose;
        }
    }
}