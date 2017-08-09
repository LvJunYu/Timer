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
        public PictureFull CurActivePicFull;

        private Dictionary<int, Table_PuzzleSlot> _slots;//拼图装备栏
        private Dictionary<int, Table_Puzzle> _puzzles;//拼图读表

        private EPuzzleOrderType _orderType;
        private int _userLv;
        private List<PictureFull> _userPictureFull;
        private List<PictureFull> _otherPictureFull;
        private List<PictureFull> _allPictureFull;
        private List<PictureFull> _equipedPuzzles;//装备的拼图
        private List<UMCtrlPuzzleItem> _allPuzzleItem;

        private void InitData()
        {
            _userLv = LocalUser.Instance.User.UserInfoSimple.LevelData.PlayerLevel;
            _userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            for (int i = 0; i < _userPictureFull.Count; i++)
            {
                _userPictureFull[i].InitData();
            }
            _otherPictureFull = new List<PictureFull>(_puzzles.Count - _userPictureFull.Count);
            foreach (int key in _puzzles.Keys)
            {
                var puzzle = _puzzles[key];
                if (!TryGetPictureFull(puzzle.Id))
                {
                    var picFull = new PictureFull(puzzle);
                    _otherPictureFull.Add(picFull);
                }
            }
            _allPictureFull = new List<PictureFull>(_puzzles.Count);
            SortOrder();
            _equipedPuzzles = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
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
            _allPuzzleItem = new List<UMCtrlPuzzleItem>(_puzzles.Count);
            for (int i = 0; i < _allPictureFull.Count; i++)
            {
                var puzzle = new UMCtrlPuzzleItem(_allPictureFull[i]);
                puzzle.Init(_cachedView.PuzzleItemGrid);
                puzzle.SetItem();
                _allPuzzleItem.Add(puzzle);
            }
        }

        private void ActivePuzzle()
        {
            if (CurActivePicFull == null || CurActivePicFull.Level > 1)
                return;
            if (_otherPictureFull.Contains(CurActivePicFull))
            {
                _otherPictureFull.Remove(CurActivePicFull);
                _userPictureFull.Add(CurActivePicFull);
                RefreshUI();
            }
        }

        private void RefreshData()
        {
            SortOrder();
            //bool refreshed = false;
            //_userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            ////查看未获得的拼图
            //for (int i = 0; i < _otherPictureFull.Count; i++)
            //{
            //    PictureFull picture;
            //    if (TryGetPictureFull(_otherPictureFull[i].PictureId, out picture))
            //    {
            //        _otherPictureFull[i] = picture;
            //        refreshed = true;
            //    }
            //}
            //return refreshed;
        }

        private void RefreshUI()
        {
            RefreshData();
            for (int i = 0; i < _allPuzzleItem.Count; i++)
            {
                _allPuzzleItem[i].SetItem(_allPictureFull[i]);
            }
        }

        private void SortOrder()
        {
            _allPictureFull.Clear();
            switch (_orderType)
            {
                case EPuzzleOrderType.Qulity:
                    _userPictureFull.Sort((p, q) => p.Quality.CompareTo(q.Quality));
                    _otherPictureFull.Sort((p, q) => p.Quality.CompareTo(q.Quality));
                    break;
                case EPuzzleOrderType.Level:
                    _userPictureFull.Sort((p, q) => p.Level.CompareTo(q.Level));
                    _otherPictureFull.Sort((p, q) => p.Level.CompareTo(q.Level));
                    break;
                case EPuzzleOrderType.Func:
                    break;
                default:
                    break;
            }
            _allPictureFull.AddRange(_userPictureFull);
            _allPictureFull.AddRange(_otherPictureFull);
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
            Messenger.AddListener(EMessengerType.OnPuzzleCompound, ActivePuzzle);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }
    }

    public enum EPuzzleOrderType
    {
        Qulity,
        Level,
        Func
    }
}