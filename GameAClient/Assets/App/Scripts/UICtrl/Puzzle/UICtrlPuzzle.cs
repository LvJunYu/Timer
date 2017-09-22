using SoyEngine;
using System.Collections.Generic;
using UnityEngine;
using GameA.Game;

namespace GameA
{
    /// <summary>
    /// 拼图页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIInGame)]
    public class UICtrlPuzzle : UICtrlAnimationBase<UIViewPuzzle>
    {
        private Dictionary<int, Table_PuzzleSlot> _slots; //装备栏数据
        private Dictionary<int, Table_Puzzle> _puzzles; //拼图表数据
        private EPuzzleOrderType _curOrderType;
        private List<PictureFull> _usingUserPicFull; //装备的拼图
        private List<PictureFull> _userPictureFull; //玩家拥有的拼图数据
        private List<PictureFull> _otherPictureFull;
        private List<PictureFull> _allPictureFull;
        private List<UMCtrlPuzzleItem> _allUMPuzzleItem; //所有拼图
        private List<UMCtrlPuzzleEquipLoc> _allUMEquipLocs; //所有槽位

        public PictureFull CurActivePicFull;

        public List<PictureFull> UsingPicFull
        {
            get { return _usingUserPicFull; }
            set { _usingUserPicFull = value; }
        }

        private void InitData()
        {
            _slots = TableManager.Instance.Table_PuzzleSlotDic;
            _puzzles = TableManager.Instance.Table_PuzzleDic;
            //已有拼图
            _userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            for (int i = 0; i < _userPictureFull.Count; i++)
            {
                _userPictureFull[i].InitData();
            }
            //未拥有拼图
            _otherPictureFull = new List<PictureFull>(_puzzles.Count - _userPictureFull.Count);
            foreach (int id in _puzzles.Keys)
            {
                if (_userPictureFull.Find(p => p.PictureId == id) == null)
                {
                    var picFull = new PictureFull(id);
                    _otherPictureFull.Add(picFull);
                }
            }
            //所有拼图排序
            _allPictureFull = new List<PictureFull>(_puzzles.Count);
            RefreshPuzzleOrder();

            //已装备拼图
            _usingUserPicFull = LocalUser.Instance.UserUsingPictureFullData.ItemDataList;
            //测试用，实际应用服务器数据
            _usingUserPicFull = new List<PictureFull>(_slots.Count);
            for (int i = 0; i < _slots.Count; i++)
            {
                _usingUserPicFull.Add(null);
            }
        }

        private void InitView()
        {
            InitData();
            //创建装备栏
            _allUMEquipLocs = new List<UMCtrlPuzzleEquipLoc>(_slots.Count);
            int index = 0;
            foreach (int key in _slots.Keys)
            {
                var unlockLv = _slots[key].UnlockLevel;
                var equipLoc = new UMCtrlPuzzleEquipLoc(key, unlockLv);
                _allUMEquipLocs.Add(equipLoc);
                equipLoc.Init(_cachedView.PuzzleLocsGrid, ResScenary);
                //显示装备的拼图
                if (index < _usingUserPicFull.Count && _usingUserPicFull[index] != null)
                    equipLoc.SetPic(_usingUserPicFull[index]);
                else
                    equipLoc.SetPic(null);
                index++;
            }

            //创建拼图
            _allUMPuzzleItem = new List<UMCtrlPuzzleItem>(_puzzles.Count);
            if (!_cachedView.PuzzleItemGridDataScroller.enabled)
            {
                for (int i = 0; i < _allPictureFull.Count; i++)
                {
                    var puzzle = new UMCtrlPuzzleItem();
                    _allUMPuzzleItem.Add(puzzle);
                    puzzle.Init(_cachedView.PuzzleItemGrid, ResScenary);
                    //puzzle.RefreshView();
                }
                _cachedView.PuzzleItemSrollRect.horizontal = false;
            }
            else
                _cachedView.PuzzleItemGridDataScroller.SetItemCount(_allPictureFull.Count);
        }

        public void OnPuzzleItemRefresh(IDataItemRenderer item, int inx)
        {
            if (inx >= _allPictureFull.Count)
            {
                LogHelper.Error("OnPuzzleItemRefresh Error: Inx > count");
                return;
            }
            item.Set(_allPictureFull[inx]);
        }

        private IDataItemRenderer CreateUmPuzzleItem(RectTransform parent)
        {
            //var puzzle = new UMCtrlPuzzleItem(_allPictureFull[i]);
            var puzzle = new UMCtrlPuzzleItem();
            _allUMPuzzleItem.Add(puzzle);
            puzzle.Init(parent, ResScenary);
            return puzzle;
        }

        private void OnPuzzleCompound()
        {
            if (_otherPictureFull.Contains(CurActivePicFull))
            {
                _otherPictureFull.Remove(CurActivePicFull);
                _userPictureFull.Add(CurActivePicFull);
            }
            if (_isOpen)
                RefreshPuzzleItems();
        }

        private void RefreshData()
        {
            //同步数据
            //            if (!LocalUser.Instance.UserPictureFull.IsInited)
            //                LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid, null,
            //                    code => { LogHelper.Error("Network error when get UserPictureFull, {0}", code); });
            _userPictureFull = LocalUser.Instance.UserPictureFull.ItemDataList;
            for (int i = 0; i < _userPictureFull.Count; i++)
            {
                _userPictureFull[i].InitData();
                var pic = _otherPictureFull.Find(p => p.PictureId == _userPictureFull[i].PictureId);
                if (pic != null)
                    _otherPictureFull.Remove(pic);
            }
            foreach (int id in _puzzles.Keys)
            {
                if (_userPictureFull.Find(p => p.PictureId == id) == null
                    && _otherPictureFull.Find(p => p.PictureId == id) == null)
                {
                    var picFull = new PictureFull(id);
                    _otherPictureFull.Add(picFull);
                }
            }
        }

        private void RefreshPuzzleItems()
        {
            //重新排序
            RefreshPuzzleOrder();
            //更新排序后的拼图信息
            for (int i = 0; i < _allUMPuzzleItem.Count; i++)
            {
                _allUMPuzzleItem[i].SetData(_allPictureFull[i]);
            }
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < _allUMEquipLocs.Count; i++)
            {
                _allUMEquipLocs[i].SetPic(_usingUserPicFull[i]);
            }
        }

        private void RefreshPuzzleOrder()
        {
            _allPictureFull.Clear();
            switch (_curOrderType)
            {
                case EPuzzleOrderType.Qulity:
                    _userPictureFull.Sort((p, q)
                        => 100 * p.Quality.CompareTo(q.Quality) - 10 * p.Level.CompareTo(q.Level) + p.AttriBonus.CompareTo(q.AttriBonus));
                    _otherPictureFull.Sort((p, q)
                        => 100 * p.Quality.CompareTo(q.Quality) - 10 * p.Level.CompareTo(q.Level) + p.AttriBonus.CompareTo(q.AttriBonus));
                    break;
                case EPuzzleOrderType.Level:
                    _userPictureFull.Sort((p, q)
                        => -100 * p.Level.CompareTo(q.Level) + 10 * p.Quality.CompareTo(q.Quality) + p.AttriBonus.CompareTo(q.AttriBonus));
                    _otherPictureFull.Sort((p, q)
                        => -100 * p.Level.CompareTo(q.Level) + 10 * p.Quality.CompareTo(q.Quality) + p.AttriBonus.CompareTo(q.AttriBonus));
                    break;
                case EPuzzleOrderType.Func:
                    _userPictureFull.Sort((p, q)
                        => 100 * p.AttriBonus.CompareTo(q.AttriBonus) + 10 * p.Quality.CompareTo(q.Quality) - p.Level.CompareTo(q.Level));
                    _otherPictureFull.Sort((p, q)
                        => 100 * p.AttriBonus.CompareTo(q.AttriBonus) + 10 * p.Quality.CompareTo(q.Quality) - p.Level.CompareTo(q.Level));
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
            _cachedView.PuzzleItemGridDataScroller.SetCallback(OnPuzzleItemRefresh, CreateUmPuzzleItem);
            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            //            LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid,
            //                () =>
            //                {
            //                    InitData();
            //                    InitView();
            //                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            //                },
            //                code =>
            //                {
            //                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            //                    LogHelper.Error("Network error when get UserPictureFull, {0}", code);
            //                });

            //            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "...");
            //            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid,
            //                () =>
            //                {
            //                    InitData();
            //                    InitView();
            //                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            //                },
            //                code =>
            //                {
            //                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
            //                    LogHelper.Error("Network error when get UserUsingPictureFullData, {0}", code);
            //                });
            InitView();
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnPuzzleCompound, OnPuzzleCompound);
            RegisterEvent(EMessengerType.OnPuzzleEquip, RefreshSlots);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshPuzzleItems();
            RefreshSlots();
        }

        protected override void OnOpenAnimationComplete()
        {
            base.OnOpenAnimationComplete();
            _cachedView.PuzzleItemSrollRect.horizontal = true;
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        private void OnFuncToggle(bool arg0)
        {
            if (arg0)
            {
                _curOrderType = EPuzzleOrderType.Func;
                RefreshPuzzleItems();
            }
        }

        private void OnLevelToggle(bool arg0)
        {
            if (arg0)
            {
                _curOrderType = EPuzzleOrderType.Level;
                RefreshPuzzleItems();
            }
        }

        private void OnQulityToggle(bool arg0)
        {
            if (arg0)
            {
                _curOrderType = EPuzzleOrderType.Qulity;
                RefreshPuzzleItems();
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