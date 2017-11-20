using System;
using NewResourceSolution;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRank : UMCtrlBase<UMViewWorldRankList>, IDataItemRenderer
    {
        private CardDataRendererWrapper<WorldRankItem.WorldRankHolder> _wrapper;
        private EWorldRankType _eWorldRankType;
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
        }

        private void OnHeadBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.Record.UserInfoDetail);
        }

        public void Set(object obj)
        {
            _wrapper = obj as CardDataRendererWrapper<WorldRankItem.WorldRankHolder>;
            RefreshView();
        }

        public void SetType(EWorldRankType eWorldRankType)
        {
            _eWorldRankType = eWorldRankType;
        }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                Unload();
                return;
            }
            WorldRankItem.WorldRankHolder holder = _wrapper.Content;
            WorldRankItem record = holder.Record;
            UserInfoSimple user = record.UserInfoDetail.UserInfoSimple;
            var rank = holder.Rank + 1;
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
            DictionaryTools.SetContentText(_cachedView.UserLevel, GetLevel(user.LevelData));
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.Score, record.Count.ToString());
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
        }

        private string GetLevel(UserLevel userLevel)
        {
            if (_eWorldRankType == EWorldRankType.WRT_Player)
            {
                return userLevel.PlayerLevel.ToString();
            }
            if (_eWorldRankType == EWorldRankType.WRT_Creator)
            {
                return userLevel.CreatorLevel.ToString();
            }
            return String.Empty;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}