using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图
    /// </summary>
    public partial class UMCtrlPuzzleItem : UMCtrlBase<UMViewPuzzleItem>
    {
        private PictureFull _puzzle;

        public UMCtrlPuzzleItem(PictureFull puzzle)
        {
            _puzzle = puzzle;
        }

        public void SetItem()
        {
            _cachedView.DisActiveImg.enabled = !(_puzzle.CurState == PuzzleState.HasActived);
        }

        public void SetItem(PictureFull puzzle)
        {
            _puzzle = puzzle;
            SetItem();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.PuzzleDetail.onClick.AddListener(OnPuzzleDetailBtn);
        }

        private void OnPuzzleDetailBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPuzzleDetail>(_puzzle);
        }
    }
}
