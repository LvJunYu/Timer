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
        //临时数据，接数据时修改
        private int _maxEquipedNum = 8;
        private int _curLv = 5;
        private int[] _unLockLv;
        private int _maxPuzzleNum = 35;
        private PuzzleData[] _puzzles;

         private void GetTempData()
        {
            _unLockLv = new int[_maxEquipedNum];
            for (int i = 0; i < _maxEquipedNum; i++)
            {
                _unLockLv[i] = i + 1;
            }
            _puzzles = new PuzzleData[_maxPuzzleNum];
            for (int i = 0; i < _maxPuzzleNum; i++)
            {
                _puzzles[i] = new PuzzleData();
            }
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            GetTempData();
            InitUI();
        }

        private void InitUI()
        {
            //创建装备栏
            for (int i = 0; i < _maxEquipedNum; i++)
            {
                var equipLoc = new UMCtrlPuzzleEquipLoc();
                equipLoc.UnlockLv = _unLockLv[i];
                equipLoc.IsLock = _unLockLv[i] >= _curLv;
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
            }
            //创建拼图
            for (int i = 0; i < _maxPuzzleNum; i++)
            {
                var puzzle = new UMCtrlPuzzleItem();
                puzzle.SetData(_puzzles[i]);
                puzzle.Init(_cachedView.PuzzleItemGrid);
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }
    }
}