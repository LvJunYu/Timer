using SoyEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SoyEngine.Proto;

namespace GameA
{
    /// <summary>
    /// 
    /// </summary>
	[UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlPuzzleDetail : UICtrlGenericBase<UIViewPuzzleDetail>
    {
        private PictureFull _puzzle;
        private PicturePart[] _puzzleFragments;
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
            _cachedView.ActiveBtn.onClick.AddListener(OnActiveBtn);
            _cachedView.EquipBtn.onClick.AddListener(OnEquipBtn);
            //碎片Item缓存
            _fragmentsCache = new List<UMCtrlPuzzleFragmentItem>(9);
            //创建拼图
            _puzzleItem = new UMCtrlPuzzleDetailItem();
            _puzzleItem.Init(_cachedView.PuzzleItemPos);
        }

        private void OnEquipBtn()
        {
            _puzzle.EquipPuzzle();
            //to do 通知服务器
            SocialGUIManager.Instance.OpenUI<UICtrlPuzzleSlots>(_puzzle);
        }

        private void OnActiveBtn()
        {
            _puzzle.ActivatePuzzle();
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在进入挑战关卡");
            RemoteCommands.CompoundPictureFull(_puzzle.PictureId, res =>
            {
                if (res.ResultCode == (int)ECompoundPictureFullCode.CPF_Success)
                {
                    Compound();
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("合成成功");
                }
                else
                {
                    SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                    LogHelper.Debug("合成失败");
                }

            }, code =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().CloseLoading(this);
                LogHelper.Debug("合成失败");
            });
            //to do 更新UI
        }

        private void Compound()
        {

        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _puzzle = parameter as PictureFull;
            _puzzleFragments = _puzzle.NeededFragments;
            UpdateUI();
        }

        private void UpdateUI()
        {
            //更新拼图数据
            _puzzleItem.SetData(_puzzle);
            _cachedView.NameTxt.text = _puzzle.Name;
            _cachedView.LvTxt.text = _puzzle.Level.ToString();
            _cachedView.DescTxt.text = _puzzle.Desc;

            //创建拼图碎片
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                var puzzleFragment = CreatePuzzleFragment();
                _puzzleFragments[i].TotalCount = 2;
                puzzleFragment.SetData(_puzzleFragments[i]);
            }

            //按钮状态
            _cachedView.Unable_Active.SetActive(!CheckActivable());
            _cachedView.Unable_Equip.SetActive(!CheckEquipable());
        }

        private bool CheckActivable()
        {
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                if (_puzzleFragments[i].TotalCount == 0)
                    return false;
            }
            return true;
        }

        private bool CheckEquipable()
        {
            return _puzzle.CurState == PuzzleState.HasActived;
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