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
        public PuzzleState CurState;
        public int Quality;

        public void SetData(PictureFull puzzle)
        {
            _puzzle = puzzle;
            Quality = puzzle.Quality;
            CurState = puzzle.CurState;
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

        /// <summary>
        /// 激活拼图
        /// </summary>
        public void ActivatePuzzle()
        {
            //CurState = PuzzleState.HasActived;
        }

        /// <summary>
        /// 装备拼图
        /// </summary>
        public void EquipPuzzle()
        {
            //CurState = PuzzleState.HasEquiped;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public void UpdateState()
        {


        }

        private bool CheckActivatable()
        {
            //for (int i = 0; i < PuzzleFragments.Count; i++)
            //{
            //    if (!PuzzleFragments[i].Owned)
            //        return false;
            //}
            return true;
        }

    }
}
