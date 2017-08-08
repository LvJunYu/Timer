using SoyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPuzzle : UICtrlGenericBase<UIViewPuzzle>
    {
        private int _maxEquipedNum;
        private int _userLv;
        private int[] _unLockLv;
        private int _maxPuzzleNum;//拼图总数
        private PictureFull[] _puzzles;

        private void InitData()
        {
            _maxEquipedNum = LocalUser.Instance.UserUsingPictureFullData.SlotCount;
            _userLv = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            //临时数据
            _unLockLv = new int[_maxEquipedNum];
            for (int i = 0; i < _maxEquipedNum; i++)
            {
                _unLockLv[i] = i + 1;
            }

            //所有拼图
            _maxPuzzleNum = TableManager.Instance.Table_PuzzleDic.Count;
            //LocalUser.Instance.UserPictureFull

           _puzzles = new PictureFull[_maxPuzzleNum];
            for (int i = 1; i <= _maxPuzzleNum; i++)
            {
                _puzzles[i] = new PictureFull();
                var puzzleFragments = new PicturePart[i + 1];
                for (int j = 0; j < puzzleFragments.Length; j++)
                {
                    puzzleFragments[j] = new PicturePart();
                    puzzleFragments[j].HaveNum = j;
                    puzzleFragments[j].Name = "碎片" + j;
                }
                _puzzles[i].PuzzleFragments = puzzleFragments;
            }
        }

        private void GetUserData()
        {
            LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UsingAvatarData, {0}", code); });
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            InitData();
            InitUI();
        }

        private void InitUI()
        {
            //创建装备栏
            for (int i = 0; i < _maxEquipedNum; i++)
            {
                var equipLoc = new UMCtrlPuzzleEquipLoc();
                equipLoc.UnlockLv = _unLockLv[i];
                equipLoc.IsLock = _unLockLv[i] >= _userLv;
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