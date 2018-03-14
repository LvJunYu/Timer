using System;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.WebCam;

namespace GameA
{
    public class UMCtrlProjectAddSelfCommend : UMCtrlBase<UMViewProjectSelfCommend>, IDataItemRenderer
    {
        private CardDataRendererWrapper<Project> _wrapper;
        private int _index;
        public int Index { get; set; }
        private const string NoName = "未命名";
        private EUserSelfRecommendType _type;

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
                TimeSpan lasTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(_wrapper.Content.ExtendData.LastSelfRecommendTime)
                        .AddDays(1) -
                    DateTimeUtil.GetServerTimeNow();
                if (lasTime < TimeSpan.Zero)
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
                        lasTime.ToString();
                }
            }
        }

        public void UpdateLastTime()
        {
            if (_wrapper == null)
            {
                return;
            }

            if (_cachedView.LastTimeText.gameObject.active)
            {
                TimeSpan lasTime =
                    DateTimeUtil.UnixTimestampMillisToLocalDateTime(_wrapper.Content.ExtendData.LastSelfRecommendTime)
                        .AddDays(1) -
                    DateTimeUtil.GetServerTimeNow();
                _cachedView.LastTimeText.text =
                    lasTime.ToString();
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