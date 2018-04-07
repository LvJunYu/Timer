using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlRank : UMCtrlBase<UMViewRank>, IDataItemRenderer
    {
        private CardDataRendererWrapper<RecordRankHolder> _wrapper;
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
            _cachedView.Button.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnHeadBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.Record.UserInfoDetail);
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
            _wrapper = obj as CardDataRendererWrapper<RecordRankHolder>;
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
            Record record = _wrapper.Content.Record;
            UserInfoSimple user = record.UserInfo;
            int rank = _wrapper.Content.Rank + 1;
            if (rank <= 3)
            {
                _cachedView.RankImage.sprite = JoyResManager.Instance.GetSprite(SpriteNameDefine.GetRank(rank));
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.RankText, rank.ToString());
            }
            _cachedView.RankText.SetActiveEx(rank > 3);
            _cachedView.RankImage.SetActiveEx(rank <= 3);
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevel, user.LevelData.PlayerLevel.ToString());
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.Score, record.Score.ToString());
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}