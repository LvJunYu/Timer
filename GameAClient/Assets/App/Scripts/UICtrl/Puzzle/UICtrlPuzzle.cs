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
        private Dictionary<int, Table_PuzzleSlot> _slots;//拼图装备栏
        private Dictionary<int, Table_Puzzle> _puzzles;//拼图读表
        private EPuzzleOrderType _orderType;
        private List<PictureFull> _userPictureFull;
        private List<PictureFull> _otherPictureFull;
        private List<PictureFull> _allPictureFull;
        private List<PictureFull> _usingPicFull;//装备的拼图
        private List<UMCtrlPuzzleItem> _allPuzzleItem;//所有拼图
        private List<UMCtrlPuzzleEquipLoc> _allEquipLocs;//所有槽位

        public PictureFull CurActivePicFull;
        public List<PictureFull> UsingPicFull
        {
            get { return _usingPicFull; }
            set { _usingPicFull = value; }
        }

        private void InitData()
        {
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
            _usingPicFull = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
            //测试用，实际应用服务器数据
            _usingPicFull = new List<PictureFull>(_slots.Count);
            for (int i = 0; i < _slots.Count; i++)
            {
                _usingPicFull.Add(null);
            }
        }

        private void InitUI()
        {
            //创建装备栏
            _allEquipLocs = new List<UMCtrlPuzzleEquipLoc>(_slots.Count);
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(key, unlockLv);
                _allEquipLocs.Add(equipLoc);
                equipLoc.Init(_cachedView.PuzzleLocsGrid);
                //显示装备的拼图
                if (UsingPicFull.Count > index && UsingPicFull[index] != null)
                    equipLoc.SetUI(UsingPicFull[index]);
                else
                    equipLoc.SetUI(null);
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
                code => { LogHelper.Error("Network error when get UserPictureFull, {0}", code); });
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserUsingPictureFullData, {0}", code); });
        }

        private void OnPuzzleCompound()
        {
            //if (CurActivePicFull == null || CurActivePicFull.Level > 1)
            //    return;
            if (_otherPictureFull.Contains(CurActivePicFull))
            {
                _otherPictureFull.Remove(CurActivePicFull);
                _userPictureFull.Add(CurActivePicFull);
            }
            SetPuzzleOrder();
        }

        private void SetPuzzleOrder()
        {
            SortOrder();
            for (int i = 0; i < _allPuzzleItem.Count; i++)
            {
                _allPuzzleItem[i].SetItem(_allPictureFull[i]);
            }
        }

        private void SetEquipLocs()
        {
            //_usingPicFull = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
            for (int i = 0; i < _allEquipLocs.Count; i++)
            {
                _allEquipLocs[i].SetUI(_usingPicFull[i]);
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
                    _userPictureFull.Sort((p, q) => p.AttriBonus.CompareTo(q.AttriBonus));
                    _otherPictureFull.Sort((p, q) => p.AttriBonus.CompareTo(q.AttriBonus));
                    break;
                default:
                    break;
            }
            _allPictureFull.AddRange(_userPictureFull);
            _allPictureFull.AddRange(_otherPictureFull);
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.Qulity.onValueChanged.AddListener(OnQulityToggle);
            _cachedView.Level.onValueChanged.AddListener(OnLevelToggle);
            _cachedView.Func.onValueChanged.AddListener(OnFuncToggle);
            _slots = TableManager.Instance.Table_PuzzleSlotDic;
            _puzzles = TableManager.Instance.Table_PuzzleDic;
            InitData();
            InitUI();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnPuzzleCompound, OnPuzzleCompound);
            RegisterEvent(EMessengerType.OnPuzzleEquip, SetEquipLocs);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SetEquipLocs();
        }

        private void OnFuncToggle(bool arg0)
        {
            if (arg0)
            {
                _orderType = EPuzzleOrderType.Func;
                SetPuzzleOrder();
            }
        }

        private void OnLevelToggle(bool arg0)
        {
            if (arg0)
            {
                _orderType = EPuzzleOrderType.Level;
                SetPuzzleOrder();
            }
        }

        private void OnQulityToggle(bool arg0)
        {
            if (arg0)
            {
                _orderType = EPuzzleOrderType.Qulity;
                SetPuzzleOrder();
            }
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzle>();
        }

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI;
        }
    }

    public enum EPuzzleOrderType
    {
        Qulity,
        Level,
        Func
    }
}