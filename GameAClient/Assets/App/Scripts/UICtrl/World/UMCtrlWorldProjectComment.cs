using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldProjectComment : UMCtrlBase<UMViewWorldProjectComment>, IDataItemRenderer
    {
        private ProjectComment _content;
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public object Data
        {
            get { return _content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Button.onClick.AddListener(OnCardClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        private void OnHeadBtn()
        {
            if (_content != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_content.UserInfoDetail);
            }
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
        }

        public void Set(object obj)
        {
            _content = obj as ProjectComment;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_content == null)
            {
                Unload();
                return;
            }
            ProjectComment data = _content;
            UserInfoSimple user = data.UserInfoDetail.UserInfoSimple;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevel,
                GameATools.GetLevelString(user.LevelData.PlayerLevel));
            DictionaryTools.SetContentText(_cachedView.CreateTime,
                DateTimeUtil.GetServerSmartDateStringByTimestampMillis(data.CreateTime));
            DictionaryTools.SetContentText(_cachedView.Content, data.Comment);
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultIconTexture);
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultIconTexture);
        }
    }
}