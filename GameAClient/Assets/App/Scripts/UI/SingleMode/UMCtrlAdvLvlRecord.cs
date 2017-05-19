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
    public class UMCtrlAdvLvlRecord : UMCtrlBase<UMViewAdvLvlRecord>, IDataItemRenderer
    {
        private Record _record;
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
            get { return _record; }
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PlayBtn.onClick.AddListener(OnPlayBtn);
        }

        protected override void OnDestroy()
        {
            _cachedView.PlayBtn.onClick.RemoveAllListeners();
            base.OnDestroy();
        }

        private void OnPlayBtn ()
        {
        }

        public void Set(object obj)
        {
            _record = obj as Record;
            RefreshView();
        }

        public void RefreshView()
        {
            if(_record == null)
            {
                return;
            }
            else
            {
//                _cachedView.Title = 
//                _cachedView.InfoDock.SetActive(true);
//                DictionaryTools.SetContentText(_cachedView.Title, _wrapper.Content.Name);
//                ImageResourceManager.Instance.SetDynamicImage(_cachedView.Cover, _wrapper.Content.IconPath, _cachedView.DefaultCoverTexture);
////                _cachedView.SeletedMark.SetActiveEx (_wrapper.IsSelected);
//                _cachedView.PublishTime.text = DateTimeUtil.GetServerSmartDateStringByTimestampMillis(_wrapper.Content.CreateTime);
////                DictionaryTools.SetContentText(_cachedView.ProjectCategoryText, EnumStringDefine.GetProjectCategoryString(_wrapper.Content.ProjectCategory));
            }
        }

        public void Unload()
        {
        }

        protected void RefreshCardMode(ECardMode mode, bool isSelected)
        {
//            _cardMode = mode;
//            if(_cardMode == ECardMode.Selectable)
//            {
//                _cachedView.SelectableMask.enabled = true;
//                _cachedView.SeletedMark.enabled = isSelected;
//                _cachedView.UnsetectMark.enabled = !isSelected;
//            }
//            else
//            {
//                _cachedView.SelectableMask.enabled = false;
//                _cachedView.SeletedMark.enabled = false;
//                _cachedView.UnsetectMark.enabled = false;
//            }
        }
    }
}
