﻿using System.Collections;
using System;
using UnityEngine;

namespace GameA
{
    /// <summary>
    /// 拼图碎片
    /// </summary>
    public class UMCtrlPuzzleFragmentItem : UMCtrlBase<UMViewPuzzleFragmentItem>
    {
        private PicturePart _fragment;
        public bool IsShow;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            IsShow = true;
        }

        public void Collect()
        {
            IsShow = false;
            _cachedView.gameObject.SetActive(false);
        }

        public void Show()
        {
            _cachedView.gameObject.SetActive(true);
            IsShow = true;
        }

        public void SetData(PicturePart fragment)
        {
            _fragment = fragment;
            _cachedView.HaveNumTxt.text = fragment.TotalCount.ToString();
            _cachedView.NameTxt.text = fragment.Name;
        }
    }
}
