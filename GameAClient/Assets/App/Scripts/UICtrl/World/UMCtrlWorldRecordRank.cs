/********************************************************************
  ** Filename : UMCtrlWorldRecordRank.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldRecordRank.cs
  ***********************************************************************/

using NewResourceSolution;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRecordRank : UMCtrlBase<UMViewWorldRecordRank>, IDataItemRenderer
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
            _cachedView.Button.onClick.AddListener(OnCardClick);
            _cachedView.HeadBtn.onClick.AddListener(OnHeadBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.Button.onClick.RemoveAllListeners();
            _cachedView.HeadBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnHeadBtn()
        {
            if (_wrapper != null)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlProjectDetail>();
                SocialGUIManager.Instance.OpenUI<UICtrlPersonalInformation>(_wrapper.Content.Record.UserInfoDetail);
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
            RecordRankHolder holder = _wrapper.Content;
            Record record = holder.Record;
            UserInfoSimple user = record.UserInfoDetail.UserInfoSimple;
            var rank = holder.Rank + 1;
            if (rank <= 3)
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
            _cachedView.LayoutElement.enabled = false;
            DictionaryTools.SetContentText(_cachedView.UserName, user.NickName);
            DictionaryTools.SetContentText(_cachedView.UserLevel, user.LevelData.PlayerLevel.ToString());
            ImageResourceManager.Instance.SetDynamicImage(_cachedView.UserIcon, user.HeadImgUrl,
                _cachedView.DefaultUserIconTexture);
            DictionaryTools.SetContentText(_cachedView.Score, record.Score.ToString());
            user.BlueVipData.RefreshBlueVipView(_cachedView.BlueVipDock, _cachedView.BlueImg, _cachedView.SuperBlueImg,
                _cachedView.BlueYearVipImg);
            Canvas.ForceUpdateCanvases();
            _cachedView.LayoutElement.enabled = _cachedView.LayoutElement.rectTransform().rect.width >=
                                                _cachedView.LayoutElement.preferredWidth;
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.UserIcon,
                _cachedView.DefaultUserIconTexture);
        }
    }
}