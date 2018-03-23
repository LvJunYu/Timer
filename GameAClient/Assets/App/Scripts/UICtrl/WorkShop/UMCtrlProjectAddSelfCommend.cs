using System;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    public class UMCtrlProjectAddSelfCommend : UMCtrlBase<UMViewProjectSelfCommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Project> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NoName = "未命名";
        private TimeSpan _lastTime = new TimeSpan(0, 0, 0);
        private TimeSpan _tempTime = new TimeSpan(0, 0, 0);

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
            _cachedView.SelectBtn.onClick.AddListener(OnSelectBtn);
            _cachedView.UnSelectBtn.onClick.AddListener(OnUnSelectBtn);
        }

        protected override void OnDestroy()
        {
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

            _wrapper = obj as CardDataRendererWrapper<Project>;
            if (_wrapper != null)
            {
                _wrapper.OnDataChanged += RefreshView;
            }

            RefreshView();
        }

        public void Unload()
        {
            Debug.Log("Unload");
        }

        public ScrollRect ScrollRect { get; set; }

        public void RefreshView()
        {
            if (_wrapper == null)
            {
                return;
            }

            RefreshProjectView();
        }

        private void RefreshProjectView()
        {
            bool emptyProject = _wrapper.Content == Project.EmptyProject;
            if (!emptyProject)
            {
                _cachedView.LastTimeText.SetActiveEx(true);
                _cachedView.SelectBtn.SetActiveEx(true);
                _cachedView.UnSelectBtn.SetActiveEx(false);
                _cachedView.LockImage.SetActiveEx(false);
                _cachedView.AddProjectBtn.SetActiveEx(false);
                Project p = _wrapper.Content;
                if (string.IsNullOrEmpty(p.Name))
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, NoName);
                }
                else
                {
                    DictionaryTools.SetContentText(_cachedView.TileText, p.Name);
                }

                ImageResourceManager.Instance.SetDynamicImage(_cachedView.ProjectBgImage, p.IconPath,
                    _cachedView.DefualtTexture);
                _lastTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(_wrapper.Content.ExtendData.LastSelfRecommendTime)
                        .AddDays(1) -
                    DateTimeUtil.GetServerTimeNow();
                if (_lastTime < TimeSpan.Zero)
                {
                    _cachedView.LastTimeText.SetActiveEx(false);
                    _cachedView.SelectBtn.SetActiveEx(true);
                    _cachedView.UnSelectBtn.SetActiveEx(false);
                }
                else
                {
                    _cachedView.SelectBtn.SetActiveEx(false);
                    _cachedView.UnSelectBtn.SetActiveEx(false);
                    _cachedView.LastTimeText.SetActiveEx(true);
                    _cachedView.LastTimeText.text =
                        string.Format("{0}:{1}:{2}", _lastTime.Hours, _lastTime.Minutes, _lastTime.Seconds);
                }
            }
        }

        public void UpdateLastTime()
        {
            if (_wrapper == null)
            {
                return;
            }

            if (_cachedView.LastTimeText.gameObject.activeSelf)
            {
                _tempTime = DateTimeUtil
                                .UnixTimestampMillisToLocalDateTime(_wrapper.Content.ExtendData.LastSelfRecommendTime)
                                .AddDays(1) -
                            DateTimeUtil.GetServerTimeNow();
                if ((_lastTime - _tempTime).Seconds > 1.0f)
                {
                    _lastTime = _tempTime;
                    _cachedView.LastTimeText.text =
                        string.Format("{0}:{1}:{2}", _lastTime.Hours, _lastTime.Minutes, _lastTime.Seconds);
                }
            }
        }


        private void OnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().AddSelfRecommendProject(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(true);
            }
        }

        private void OnUnSelectBtn()
        {
            if (SocialGUIManager.Instance.GetUI<UICtrlWorkShop>().ReomveSelfRecommendProject(_wrapper.Content))
            {
                _cachedView.UnSelectBtn.SetActiveEx(false);
            }
        }
    }
}