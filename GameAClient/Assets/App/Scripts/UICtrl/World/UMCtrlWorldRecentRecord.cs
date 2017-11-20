using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRecentRecord : UMCtrlBase<UMViewWorldRecentRecord>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Record> _wrapper;
        private static string _successStr = "成功";
        private static string _failStr = "失败";
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
            _cachedView.PlayBtn.onClick.AddListener(OnCardClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnHeadBtn()
        {
            if (_wrapper != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.UserInfoDetail);
            }
        }

        private void OnCardClick()
        {
            if (_wrapper != null)
            {
                _wrapper.FireOnClick();
            }
        }

        public void Set(object obj)
        {
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<Record>;
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
            Record record = _wrapper.Content;
            UserInfoSimple user = record.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.SuceessTxt,
                record.Result == (int) EGameResult.GR_Success ? _successStr : _failStr);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.DateTxt,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(record.CreateTime));
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock,
                _cachedView.BlueImg, _cachedView.SuperBlueImg, _cachedView.BlueYearVipImg);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}