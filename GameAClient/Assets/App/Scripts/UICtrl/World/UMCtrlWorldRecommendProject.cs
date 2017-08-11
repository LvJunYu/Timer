  /********************************************************************
  ** Filename : UMCtrlWorldRecommendProject.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlWorldRecommendProject.cs
  ***********************************************************************/

using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlWorldRecommendProject : UMCtrlBase<UMViewWorldRecommendProject>
    {
        private Project _content;
        private EType _type;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Button.onClick.AddListener(OnCardClick);
            _cachedView.Trans.pivot = Vector2.one * 0.5f;
            _cachedView.Trans.anchorMin = Vector2.zero;
            _cachedView.Trans.anchorMax = Vector2.one;
            _cachedView.Trans.sizeDelta = Vector2.zero;
        }

        private void OnCardClick()
        {
            if (_content != null)
            {
                SocialGUIManager.Instance.OpenUI<UICtrlProjectDetail>(_content);
            }
        }

        public void SetNew()
        {
            _type = EType.New;
            RefreshView();
        }

        public void SetHot()
        {
            _type = EType.Hot;
            RefreshView();
        }

        public void Set(Project project)
        {
            _content = project;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_content == null)
            {
                Unload();
                _cachedView.HotLabel.SetActive(false);
                _cachedView.NewLabel.SetActive(false);
                _cachedView.BottomDock.SetActive(false);
                _cachedView.Button.enabled = false;
            }
            else
            {
                _cachedView.Button.enabled = true;
                Project p = _content;
                _cachedView.HotLabel.SetActiveEx(_type == EType.Hot);
                _cachedView.NewLabel.SetActiveEx(_type == EType.New);
                _cachedView.BottomDock.SetActiveEx(true);
                DictionaryTools.SetContentText(_cachedView.PlayCount, p.PlayCount.ToString());
                DictionaryTools.SetContentText(_cachedView.LikeCount, p.LikeCount.ToString());
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, p.IconPath, _cachedView.DefaultCoverTexture);
            }
        }

        public void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Cover, _cachedView.DefaultCoverTexture);
        }
        
        private enum EType
        {
            New,
            Hot
        }
    }
}
