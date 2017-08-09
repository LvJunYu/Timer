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
        private Dictionary<int, Table_Puzzle> _puzzles;//拼图读表
        private List<PictureFull> _userPictureFull;
        private List<PictureFull> _otherPictureFull;
        private List<PictureFull> _equipedPuzzles;//装备的拼图

        private void InitData()
        {
            _userLv = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            _userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            _otherPictureFull = new List<PictureFull>(_puzzles.Count - _userPictureFull.Count);
            foreach (int key in _puzzles.Keys)
            {
                var puzzle = _puzzles[key];
                if (!TryGetPictureFull(puzzle.Id))
                {
                    _otherPictureFull.Add(new PictureFull(puzzle));
                }
            }
            _equipedPuzzles = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
        }

        private bool RefreshData()
        {
            bool refreshed = false;
            _userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            //_otherPictureFull.Clear();
            //_otherPictureFull = new List<PictureFull>(_puzzles.Count - _userPictureFull.Count);
            //foreach (int key in _puzzles.Keys)
            //{
            //    var puzzle = _puzzles[key];
            //    if (!CheckOwned(puzzle.Id))
            //    {
            //        _otherPictureFull.Add(new PictureFull(puzzle));
            //    }
            //}

            //查看未获得的拼图
            for (int i = 0; i < _otherPictureFull.Count; i++)
            {
                PictureFull picture;
                if (TryGetPictureFull(_otherPictureFull[i].PictureId, out picture))
                {
                    _otherPictureFull[i] = picture;
                    refreshed = true;
                }
            }
            return refreshed;
        }

        private bool TryGetPictureFull(long id, out PictureFull picture)
        {
            for (int i = 0; i < _userPictureFull.Count; i++)
            {
                if (_userPictureFull[i].PictureId == id)
                {
                    picture = _userPictureFull[i];
                    return true;
                }
            }
            picture = null;
            return false;
        }

        private bool TryGetPictureFull(long id)
        {
            PictureFull picture;
            return TryGetPictureFull(id, out picture);
        }

        private void RequestData()
        {
            LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UsingAvatarData, {0}", code); });
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        private void InitUI()
        {
            //创建装备栏
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(unlockLv, unlockLv > _userLv);
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
                //显示装备的拼图
                if (_equipedPuzzles.Count > index && _equipedPuzzles[index] != null)
                    equipLoc.SetData(_equipedPuzzles[index]);
                else
                    equipLoc.SetData(null);
                index++;
            }

            //创建拼图
            for (int i = 0; i < _userPictureFull.Count; i++)
            {
                var puzzle = new UMCtrlPuzzleItem();
                puzzle.SetData(_userPictureFull[i]);
                puzzle.Init(_cachedView.PuzzleItemGrid);
            }
            for (int i = 0; i < _otherPictureFull.Count; i++)
            {
                var puzzle = new UMCtrlPuzzleItem();
                puzzle.SetData(_otherPictureFull[i]);
                puzzle.Init(_cachedView.PuzzleItemGrid);
            }
        }

        private void RefreshUI()
        {

        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _slots = TableManager.Instance.Table_PuzzleSlotDic;
            _puzzles = TableManager.Instance.Table_PuzzleDic;
            InitData();
            InitUI();
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (RefreshData())
                RefreshUI();
            //RefreshUserData();
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }
    }
}