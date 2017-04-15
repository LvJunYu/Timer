  /********************************************************************
  ** Filename : UMCtrlPersonalProjectCard.cs
  ** Author : quan
  ** Date : 11/11/2016 1:47 PM
  ** Summary : UMCtrlPersonalProjectCard.cs
  ***********************************************************************/

using System;
using System.Collections;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlPersonalProjectCard : UMCtrlCardBase<UMViewPersonalProjectCard>
    {
        private CardDataRendererWrapper<Project> _wrapper;
        public RectTransform Trans
        {
            get { return _cachedView.Trans; }
        }

        public override object Data
        {
            get { return _wrapper.Content; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.Card.onClick.AddListener(OnCardClick);
        }

        protected override void OnDestroy()
        {
            _cachedView.Card.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnCardClick()
        {
            _wrapper.FireOnClick();
        }

        public override void Set(object obj)
        {
            if(_wrapper != null)
            {
                _wrapper.OnDataChanged -= RefreshView;
            }
            _wrapper = obj as CardDataRendererWrapper<Project>;
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
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultCoverTexture);
                return;
            }
            RefreshCardMode(_wrapper.CardMode, _wrapper.IsSelected);
            if(_wrapper.Content == null)
            {
                _cachedView.EmptyDock.SetActive(true);
                _cachedView.InfoDock.SetActive(false);
            }
            else
            {
                _cachedView.EmptyDock.SetActive(false);
                _cachedView.InfoDock.SetActive(true);
                DictionaryTools.SetContentText(_cachedView.Name, _wrapper.Content.Name);
                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Icon, _wrapper.Content.IconPath, _cachedView.DefaultCoverTexture);
                _cachedView.CreateTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_wrapper.Content.UpdateTime);
//                DictionaryTools.SetContentText(_cachedView.ProjectCategoryText, EnumStringDefine.GetProjectCategoryString(_wrapper.Content.ProjectCategory));
            }
        }

        public override void Unload()
        {
            ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultCoverTexture);
        }

        protected void RefreshCardMode(ECardMode mode, bool isSelected)
        {
            _cardMode = mode;
            if(_cardMode == ECardMode.Selectable)
            {
                _cachedView.SelectableMask.enabled = true;
                _cachedView.SeletedMark.enabled = isSelected;
                _cachedView.UnsetectMark.enabled = !isSelected;
            }
            else
            {
                _cachedView.SelectableMask.enabled = false;
                _cachedView.SeletedMark.enabled = false;
                _cachedView.UnsetectMark.enabled = false;
            }
        }
    }
}
