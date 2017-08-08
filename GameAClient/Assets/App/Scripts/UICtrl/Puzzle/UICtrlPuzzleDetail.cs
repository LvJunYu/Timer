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
    public class UICtrlPuzzleDetail : UICtrlGenericBase<UIViewPuzzleDetail>
    {
        private PuzzleData _puzzle;
        private PuzzleFragmentData[] _puzzleFragments;
        private UMCtrlPuzzleDetailItem _puzzleItem;
        private List<UMCtrlPuzzleFragmentItem> _fragmentsCache;

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI2;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _fragmentsCache = new List<UMCtrlPuzzleFragmentItem>(9);
            //创建拼图
            _puzzleItem = new UMCtrlPuzzleDetailItem();
            _puzzleItem.Init(_cachedView.PuzzleItemPos);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _puzzle = parameter as PuzzleData;
            UpdateUI();
        }

        private void UpdateUI()
        {
            //更新拼图数据
            _puzzleItem.SetData(_puzzle);
            _cachedView.NameTxt.text = _puzzle.Name;
            _cachedView.UnlockLvTxt.text = _puzzle.UnlockLv.ToString();
            _cachedView.DescTxt.text = _puzzle.Desc;

            //创建拼图碎片
            _puzzleFragments = _puzzle.PuzzleFragments;
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                var puzzleFragment = CreatePuzzleFragment();
                puzzleFragment.SetData(_puzzleFragments[i]);
            }
        }

        private UMCtrlPuzzleFragmentItem CreatePuzzleFragment()
        {
            //查看缓存
            foreach (var item in _fragmentsCache)
            {
                if (!item.IsShow)
                {
                    item.Show();
                    return item;
                }
            }
            //缓存没有，则创建新的
            var puzzleFragment = new UMCtrlPuzzleFragmentItem();
            puzzleFragment.Init(_cachedView.PuzzleFragmentGrid);
            //新的添加到缓存
            _fragmentsCache.Add(puzzleFragment);

            return puzzleFragment;
        }

        protected override void OnClose()
        {
            base.OnClose();
            //全部缓存起来
            foreach (var item in _fragmentsCache)
            {
                if (item.IsShow)
                    item.Collect();
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzleDetail>();
        }
    }
}