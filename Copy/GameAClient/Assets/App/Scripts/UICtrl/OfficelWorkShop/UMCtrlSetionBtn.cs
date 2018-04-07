using System;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlSetionBtn : UMCtrlBase<UMViewSectionBtn>, IUMPoolable
    {
        private int _sectionindex;
        private Action<int> _callback;
        public bool IsShow { get; private set; }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.ItemBtn.onClick.AddListener(OnBtn);
        }

        public void Show()
        {
            IsShow = true;
        }

        public void Hide()
        {
            IsShow = true;
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.SetParent(rectTransform);
        }

        public void SetSectionIndex(int index, Action<int> callback)
        {
            _callback = callback;
            _sectionindex = index;
            _cachedView.SectionIndexText.text = string.Format("第{0}章", index);
        }

        private void OnBtn()
        {
            if (_callback != null)
            {
                _callback.Invoke(_sectionindex);
                _cachedView.SelectImage.SetActiveEx(true);
            }
        }

        public void OnNoSelect()
        {
            _cachedView.SelectImage.SetActiveEx(false);
        }
    }
}