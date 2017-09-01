using System;
using UnityEngine;
using NewResourceSolution;
using SoyEngine;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    public class UMCtrlPuzzleItem : UMCtrlBase<UMViewPuzzleItem>, IDataItemRenderer
    {
        private PictureFull _puzzle;

        public UMCtrlPuzzleItem(PictureFull puzzle)
        {
            _puzzle = puzzle;
        }

        public UMCtrlPuzzleItem()
        {
        }

        public object Data
        {
            get { throw new NotImplementedException(); }
        }

        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public RectTransform Transform
        {
            get { return _cachedView.Trans; }
        }

        public void Set(object data)
        {
            _puzzle = data as PictureFull;
            RefreshView();
        }

        public void Unload()
        {
        }

        public void RefreshView()
        {
            _cachedView.DisActiveImg.sprite = _cachedView.Img.sprite = _puzzle.Icon;
            _cachedView.DisActiveImg.enabled = !(_puzzle.CurState == EPuzzleState.HasActived);
        }

        public void ResetData(PictureFull puzzle)
        {
            _puzzle = puzzle;
            RefreshView();
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