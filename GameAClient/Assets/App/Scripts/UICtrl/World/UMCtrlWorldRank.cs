
using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRank : UMCtrlBase<UMViewWorldRankList>, IDataItemRenderer
    {
        private CardDataRendererWrapper<WorldRankItem.WorldRankHolder> _wrapper;
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }

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
        }

        private void OnHeadBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.Record.UserInfoDetail);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnCardClick()
        {
//            _wrapper.FireOnClick();
        }

        public void Set(object obj)
        {
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<WorldRankItem.WorldRankHolder>;
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }
            RefreshView();
        }

        public void RefreshView()
        {
            if(_wrapper == null)
            {
                Unload();
                return;
            }
            WorldRankItem.WorldRankHolder holder = _wrapper.Content;
            WorldRankItem record = holder.Record;
            UserInfoSimple user = record.UserInfo;
            var rank = holder.Rank + 1;
            if (rank <=3)
            {
                _cachedView.RankText.SetActiveEx(false);
                _cachedView.RankImage.SetActiveEx(true);
                _cachedView.RankImage.sprite = JoyResManager.Instance.GetSprite(SpriteNameDefine.GetRank(rank));
            }
            else
            {
                _cachedView.RankText.SetActiveEx(true);
                _cachedView.RankImage.SetActiveEx(false);
                DictionaryTools.SetContentText(_cachedView.RankText, rank.ToString());
            }
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevel, user.LevelData.PlayerLevel.ToString());
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl, _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.Score, record.Count.ToString());
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon, _cachedView.DefaultUserIconTexture);
        }

    }
}