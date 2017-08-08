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
        private int _userLv;
        private Dictionary<int, Table_PuzzleSlot> _slots;//拼图装备栏
        private Dictionary<int, Table_Puzzle> _puzzles;//拼图装备栏
        private PictureFull[] _puzzleDatas;

        private void InitData()
        {
            _userLv = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            _slots = TableManager.Instance.Table_PuzzleSlotDic;

            //所有拼图
            _puzzles = TableManager.Instance.Table_PuzzleDic;
            //LocalUser.Instance.UserPictureFull
            _puzzleDatas = new PictureFull[_puzzles.Count];
            int i = 0;
            foreach (int key in _puzzles.Keys)
            {
                _puzzleDatas[i] = new PictureFull(_puzzles[key]);
                i++;
            }
        }

        private void GetUserData()
        {
            LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UsingAvatarData, {0}", code); });
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        private void InitUI()
        {
            //创建装备栏
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(unlockLv, unlockLv >= _userLv);
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
            }
            //创建拼图
            for (int i = 0; i < _puzzles.Count; i++)
            {
                var puzzle = new UMCtrlPuzzleItem();
                puzzle.SetData(_puzzleDatas[i]);
                puzzle.Init(_cachedView.PuzzleItemGrid);
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
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            //GetUserData();
            InitData();
            InitUI();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }
    }
}