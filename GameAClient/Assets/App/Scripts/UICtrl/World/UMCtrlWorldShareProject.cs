using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldShareProject : UMCtrlBase<UMViewWorldShareProject>, IDataItemRenderer
    {
        private CardDataRendererWrapper<UserInfoDetail> _wrapper;
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
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
            _cachedView.SelectTog.onValueChanged.AddListener(OnSelectTogValueChanged);
        }

        private void OnSelectTogValueChanged(bool value)
        {
            if (_wrapper != null)
            {
                _wrapper.IsSelected = value;
                _wrapper.FireOnClick();
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.SelectTog.onValueChanged.RemoveAllListeners();
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnHeadBtn()
        {
            if (_wrapper != null)
            {
//                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetailShare>();
//                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content);
            }
        }

        public void Set(object obj)
        {
            _wrapper = obj as CardDataRendererWrapper<UserInfoDetail>;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                Unload();
                return;
            }
            UserInfoSimple user = _wrapper.Content.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
            _cachedView.SelectTog.isOn = _wrapper.IsSelected;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}