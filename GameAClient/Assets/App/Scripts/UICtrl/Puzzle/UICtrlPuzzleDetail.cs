using SoyEngine;
using System.Collections.Generic;
using NewResourceSolution;
using UnityEngine;
using SoyEngine.Proto;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
    [UIAutoSetup]
    public class UICtrlPuzzleDetail : UICtrlAnimationBase<UIViewPuzzleDetail>
    {
        //碎片间距
        private const float _halfSpacing = 90;
        private const float _quarterSpacing = -35;
        private const float _sixthSpacing = -30;
        private const float _ninthSpacing =-60;

        //按钮文字
        private const string _upgrateTxt = "升级";

        private const string _activeTxt = "合成";
        private const string _maxLvTxt = "等级MAX";

        private const string _halfImageName = "img_puzzle_half_{0}{0}";
        private const string _quarterImageName = "img_puzzle_quarter_{0}{0}";
        private const string _sixthImageName = "img_puzzle_sixth_{0}{0}";
        private const string _ninthImageName = "img_puzzle_ninth_{0}{0}";

        private const string _halfMaskImageName = "img_puzzle_half_{0}{0}_mask";
        private const string _quarterMaskImageName = "img_puzzle_quarter_{0}{0}_mask";
        private const string _sixthMaskImageName = "img_puzzle_sixth_{0}{0}_mask";
        private const string _ninthMaskImageName = "img_puzzle_ninth_{0}{0}_mask";

        private PictureFull _puzzle;
        private PicturePart[] _puzzleFragments;
        private UMCtrlPuzzleDetailItem _curUMPuzzleItem;
        private List<UMCtrlPuzzleFragmentItem> _fragmentsCache;
        private List<UMCtrlPuzzleFragmentItem> _curUMFragments;
        private Sprite[] _halfImages = new Sprite[2];
        private Sprite[] _quarterImages = new Sprite[4];
        private Sprite[] _sixthImages = new Sprite[6];
        private Sprite[] _ninthImages = new Sprite[9];
        private Sprite[] _halfMaskImages = new Sprite[2];
        private Sprite[] _quarterMaskImages = new Sprite[4];
        private Sprite[] _sixthMaskImages = new Sprite[6];
        private Sprite[] _ninthMaskImages = new Sprite[9];

        public Sprite GetSprite(EPuzzleType puzzleType, int index, bool isMask = true)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    if (isMask)
                    {
                        if (null == _halfMaskImages[index - 1])
                            _halfMaskImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _halfMaskImages[index - 1];
                    }
                    else
                    {
                        if (null == _halfImages[index - 1])
                            _halfImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _halfImages[index - 1];
                    }
                case EPuzzleType.Quarter:
                    if (isMask)
                    {
                        if (null == _quarterMaskImages[index - 1])
                            _quarterMaskImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _quarterMaskImages[index - 1];
                    }
                    else
                    {
                        if (null == _quarterImages[index - 1])
                            _quarterImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _quarterImages[index - 1];
                    }
                case EPuzzleType.Sixth:
                    if (isMask)
                    {
                        if (null == _sixthMaskImages[index - 1])
                            _sixthMaskImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _sixthMaskImages[index - 1];
                    }
                    else
                    {
                        if (null == _sixthImages[index - 1])
                            _sixthImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _sixthImages[index - 1];
                    }
                case EPuzzleType.Ninth:
                    if (isMask)
                    {
                        if (null == _ninthMaskImages[index - 1])
                            _ninthMaskImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index));
                        return _ninthMaskImages[index - 1];
                    }
                    else
                    {
                        if (null == _ninthImages[index - 1])
                            _ninthImages[index - 1] =
                                ResourcesManager.Instance.GetSprite(GetSpriteName(puzzleType, index, false));
                        return _ninthImages[index - 1];
                    }
                default:
                    return null;
            }
        }

        private string GetSpriteName(EPuzzleType puzzleType, int index, bool isMask = true)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    if (isMask)
                        return string.Format(_halfMaskImageName, index);
                    else
                        return string.Format(_halfImageName, index);
                case EPuzzleType.Quarter:
                    if (isMask)
                        return string.Format(_quarterMaskImageName, index);
                    else
                        return string.Format(_quarterImageName, index);
                case EPuzzleType.Sixth:
                    if (isMask)
                        return string.Format(_sixthMaskImageName, index);
                    else
                        return string.Format(_sixthImageName, index);
                case EPuzzleType.Ninth:
                    if (isMask)
                        return string.Format(_ninthMaskImageName, index);
                    else
                        return string.Format(_ninthImageName, index);
                default:
                    return null;
            }
        }

        private void RefreshView()
        {
            //拼图数据
            _curUMPuzzleItem.SetData(_puzzle);

            //创建拼图碎片Item
            _puzzleFragments = _puzzle.NeededFragments;
            _curUMFragments.Clear();
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                UMCtrlPuzzleFragmentItem puzzleFragment = CreatePuzzleFragment();
                _curUMFragments.Add(puzzleFragment);
                //测试用
                _puzzleFragments[i].AddFrag(Random.Range(0, 2));
                puzzleFragment.SetData(_puzzleFragments[i], _puzzle, _curUMPuzzleItem);
            }
            //设置碎片间距
            _cachedView.PuzzleFragmentGrid.spacing = GetGridSpace(_puzzle.PuzzleType);
            _cachedView.PuzzleFragmentGrid.rectTransform().anchoredPosition = Vector2.zero;
            //锁住拖拽
            _cachedView.FragsScrollRect.horizontal = false;
            //文字信息
            RefreshTexts();
            //按钮信息
            RefreshButtons();
        }

        private float GetGridSpace(EPuzzleType puzzleType)
        {
            switch (puzzleType)
            {
                case EPuzzleType.Half:
                    return _halfSpacing;
                case EPuzzleType.Quarter:
                    return _quarterSpacing;
                case EPuzzleType.Sixth:
                    return _sixthSpacing;
                case EPuzzleType.Ninth:
                    return _ninthSpacing;
                default:
                    return 0;
            }
        }

        private void RefreshFragments()
        {
            for (int i = 0; i < _curUMFragments.Count; i++)
            {
                _curUMFragments[i].RefreshView();
            }
        }

        private void RefreshTexts()
        {
            _cachedView.NameTxt.text = _puzzle.Name;
            _cachedView.LvTxt.text = _puzzle.Level.ToString();
            _cachedView.DescTxt.text = _puzzle.Desc;
        }

        private void RefreshButtons()
        {
            //设置按钮是否可用
            _cachedView.Unable_Active.SetActive(!CheckActivable());
            _cachedView.ActiveBtn.gameObject.SetActive(CheckActivable());
            _cachedView.Unable_Equip.SetActive(!CheckEquipable());
            _cachedView.EquipBtn.gameObject.SetActive(CheckEquipable());
            //按钮上显示消耗的金币
            _cachedView.CostNumTxt.text = _puzzle.CostMoeny.ToString();
            //_cachedView.CostNumTxt.gameObject.SetActive(true);
            //按钮上显示合成或升级文字
            if (_puzzle.CurState == EPuzzleState.HasActived)
            {
                if (_puzzle.Level >= _puzzle.PuzzleTable.MaxLevel)
                {
                    //_cachedView.CostNumTxt.gameObject.SetActive(false);
                    _cachedView.ActiveTxt.text = _maxLvTxt;
                }
                else
                {
                    _cachedView.ActiveTxt.text = _upgrateTxt;
                }
            }
            else
                _cachedView.ActiveTxt.text = _activeTxt;
        }

        private bool CheckActivable()
        {
            if (_puzzle.Level >= _puzzle.PuzzleTable.MaxLevel)
                return false;
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                if (_puzzleFragments[i].TotalCount == 0)
                    return false;
            }
            return true;
        }

        private bool CheckEquipable()
        {
            return _puzzle.CurState == EPuzzleState.HasActived;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPuzzleDetail>();
        }

        private void OnEquipBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPuzzleSlots>(_puzzle);
        }

        private void OnActiveBtn()
        {
            if (!GameATools.CheckGold(_puzzle.CostMoeny))
                return;
            //通知服务器
            if (_puzzle.CurState == EPuzzleState.HasActived)
                UpgradeCommand();
            else
                CompoundCommand();
        }

        private void CompoundCommand()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在合成拼图");
            RemoteCommands.CompoundPictureFull(_puzzle.PictureId, res =>
            {
                if (res.ResultCode == (int)ECompoundPictureFullCode.CPF_Success)
                {
                    CompoundOrUpgrade();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("合成失败");
                }
            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                //测试，服务器完成后删除
                LogHelper.Debug("服务器请求失败，客服端合成测试");
                CompoundOrUpgrade();
                //LogHelper.Debug("合成失败");
            });
        }

        private void UpgradeCommand()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在升级拼图");
            RemoteCommands.UpgradePictureFull(_puzzle.PictureId, _puzzle.Level + 1, res =>
            {
                if (res.ResultCode == (int)ECompoundPictureFullCode.CPF_Success)
                {
                    CompoundOrUpgrade();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("升级失败");
                }
            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                //测试，服务器完成后删除
                LogHelper.Debug("服务器请求失败，客服端升级测试");
                CompoundOrUpgrade();
                //LogHelper.Debug("升级失败");
            });
        }

        private void CompoundOrUpgrade()
        {
            //消耗金币
            if (!GameATools.LocalUseGold(_puzzle.CostMoeny))
            {
                LogHelper.Debug("Don't have enough moeny !!");
                return;
            }
            //消耗材料
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                _puzzleFragments[i].TotalCount--;
            }
            //升级
            _puzzle.ActivatePuzzle();
            //传递当前合成的拼图
            SocialGUIManager.Instance.GetUI<UICtrlPuzzle>().CurActivePicFull = _puzzle;
            Messenger.Broadcast(EMessengerType.OnPuzzleCompound);
            LogHelper.Debug("合成/升级成功");
        }

        private void OnPuzzleCompound()
        {
            //更新拼图
            _curUMPuzzleItem.RefreshView();
            //更新碎片
            RefreshFragments();
            RefreshTexts();
            RefreshButtons();
        }

        private void OnPuzzleFragChanged()
        {
            if (!_isOpen) return;
            //更新拼图
            _curUMPuzzleItem.RefreshView();
            //更新碎片
            RefreshFragments();

            RefreshButtons();
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
            puzzleFragment.Init(_cachedView.PuzzleFragmentGrid.transform as RectTransform);
            //新的添加到缓存
            _fragmentsCache.Add(puzzleFragment);

            return puzzleFragment;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.ActiveBtn.onClick.AddListener(OnActiveBtn);
            _cachedView.EquipBtn.onClick.AddListener(OnEquipBtn);
            //碎片Item缓存
            _fragmentsCache = new List<UMCtrlPuzzleFragmentItem>(9);
            //当前拼图所需碎片
            _curUMFragments = new List<UMCtrlPuzzleFragmentItem>(9);
            //创建拼图
            _curUMPuzzleItem = new UMCtrlPuzzleDetailItem();
            _curUMPuzzleItem.Init(_cachedView.PuzzleItemPos);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnPuzzleCompound, OnPuzzleCompound);
            RegisterEvent(EMessengerType.OnPuzzleFragChanged, OnPuzzleFragChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _puzzle = parameter as PictureFull;
            if (_puzzle == null)
            {
                LogHelper.Error("parameter as PictureFul is null");
                OnClose();
                return;
            }
            RefreshView();
        }

        protected override void OnOpenAnimationComplete()
        {
            base.OnOpenAnimationComplete();
            //6、9拼图才打开水平拖动
            if (_puzzle.PuzzleType > EPuzzleType.Quarter)
                _cachedView.FragsScrollRect.horizontal = true;
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _animationType = EAnimationType.PopupFromDown;
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

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI2;
        }
    }
}