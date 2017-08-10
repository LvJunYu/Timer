using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameA
{
    /// <summary>
    /// 拼图详情的拼图
    /// </summary>
    public partial class UMCtrlPuzzleDetailItem : UMCtrlBase<UMViewPuzzleDetailItem>
    {
        private PictureFull _puzzle;

        public void SetData(PictureFull puzzle)
        {
            _puzzle = puzzle;
            SetData();
        }

        public void SetData()
        {
            _cachedView.Puzzle_Active.enabled = _puzzle.CurState == EPuzzleState.HasActived;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
        }


    }
}
