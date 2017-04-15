/********************************************************************
** Filename : UMCtrlProjectCard
** Author : Dong
** Date : 2015/4/30 22:30:30
** Summary : UMCtrlProjectCard
***********************************************************************/

using System;
using System.Collections;
using SoyEngine;

namespace GameA
{
    public class UMCtrlProjectCardSimple : UMCtrlCardBase<UMViewProjectCardSimple>
    {
        private CardDataRendererWrapper<Project> _wrapper;

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
            if(null == _wrapper)
            {
                ImageResourceManager.Instance.SetDynamicImageDefault(_cachedView.Icon, _cachedView.DefaultCoverTexture);
                return;
            }
            RefreshCardMode(_wrapper.CardMode, _wrapper.IsSelected);
            if(_wrapper.Content.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Public && !_wrapper.Content.IsValid)
            {
                DictionaryTools.SetContentText(_cachedView.Name, "[已删除]");
            }
            else
            {
                DictionaryTools.SetContentText(_cachedView.Name, _wrapper.Content.Name);
            }

            ImageResourceManager.Instance.SetDynamicImage(_cachedView.Icon, _wrapper.Content.IconPath, _cachedView.DefaultCoverTexture);

            if(_wrapper.Content.ProjectStatus == SoyEngine.Proto.EProjectStatus.PS_Private)
            {
                _cachedView.CreateTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_wrapper.Content.UpdateTime);
            }
            else
            {
                _cachedView.CreateTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_wrapper.Content.CreateTime);
            }
//            DictionaryTools.SetContentText(_cachedView.ProjectCategoryText, EnumStringDefine.GetProjectCategoryString(_wrapper.Content.ProjectCategory));
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
