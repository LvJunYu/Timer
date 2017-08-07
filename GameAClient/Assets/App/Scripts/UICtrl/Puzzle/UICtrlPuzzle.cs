using SoyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPuzzle : UICtrlGenericBase<UIViewPuzzle>
    {
        private UMCtrlPuzzleItem _puzzleItem;

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI2;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            InitUI();
        }

        private void InitUI()
        {
            _puzzleItem = new UMCtrlPuzzleItem();
            _puzzleItem.Init(_cachedView.PuzzleItemPos);
            for (int i = 0; i < 8; i++)
            {
                var puzzleItem = new UMCtrlPuzzleFragmentItem();
                puzzleItem.Init(_cachedView.PuzzleFragmentGrid);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }
    }
}