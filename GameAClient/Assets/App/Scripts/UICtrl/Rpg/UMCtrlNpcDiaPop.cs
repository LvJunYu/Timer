using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    public class UMCtrlNpcDiaPop : UMCtrlBase<UMViewNpcDiaPop>, IUMPoolable
    {
        public bool IsShow { get; private set; }
        private Vector3 _pos;
        private bool _diaIsNull;

        public void SetPos(Vector3 pos)
        {
            _pos = pos;
        }

        public void SetStr(string dialog)
        {
            _diaIsNull = string.IsNullOrEmpty(dialog);
            _cachedView.DiaText.text = dialog;
        }

        public void Hide()
        {
            IsShow = false;
            _cachedView.Trans.anchoredPosition = new Vector2(1000000000000, 1000000000000);
        }

        public void Show()
        {
            if (_diaIsNull)
            {
                return;
            }
            IsShow = true;
            _cachedView.Trans.anchoredPosition = _pos;
        }

        public void SetDymicPos(Vector3 pos)
        {
            _pos = pos;
            _cachedView.Trans.anchoredPosition = _pos;
        }

        public void SetParent(RectTransform rectTransform)
        {
            _cachedView.Trans.SetParent(rectTransform);
        }
    }
}