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
        private List<UMCtrlPuzzleFragmentItem> _curFragments;
        private const string _upgrateTxt = "升级";
        private const string _activeTxt = "合成";

        private void OnEquipBtn()
        {
            _puzzle.EquipPuzzle();
            SocialGUIManager.Instance.OpenUI<UICtrlPuzzleSlots>(_puzzle);
        }

        private void OnActiveBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlLittleLoading>().OpenLoading(this, "正在合成拼图");
            RemoteCommands.CompoundPictureFull(_puzzle.PictureId, res =>
            {
                if (res.ResultCode == (int)ECompoundPictureFullCode.CPF_Success)
                {
                    Compound();
                    RequesUserPictureFull();
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
                //测试，服务器完成后删除
                LogHelper.Debug("合成测试");
                Compound();
                //LogHelper.Debug("合成失败");
            });
        }

        private void Compound()
        {
            //消耗材料
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                _puzzleFragments[i].TotalCount--;
            }
            //升级
            _puzzle.ActivatePuzzle();
            Messenger.Broadcast(EMessengerType.OnPuzzleCompound);
        }

        private void RefreshFragments()
        {
            for (int i = 0; i < _curFragments.Count; i++)
            {
                _curFragments[i].RefreshData();
            }
        }

        private void RequesUserPictureFull()
        {
            LocalUser.Instance.UserPictureFull.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserPictureFull, {0}", code); });
            LocalUser.Instance.UserPicturePart.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserPicturePart, {0}", code); });
        }

        private void RequesUsingPictureFull()
        {
            LocalUser.Instance.UserUsingPictureFullData.Request(LocalUser.Instance.UserGuid, null,
                code => { LogHelper.Error("Network error when get UserUsingPictureFullData, {0}", code); });
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            _puzzle = parameter as PictureFull;
            _puzzleFragments = _puzzle.NeededFragments;
            SetUI();
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
            _curFragments = new List<UMCtrlPuzzleFragmentItem>(9);
            //创建拼图
            _puzzleItem = new UMCtrlPuzzleDetailItem();
            _puzzleItem.Init(_cachedView.PuzzleItemPos);
            //监听事件
            Messenger.AddListener(EMessengerType.OnPuzzleCompound, RefreshFragments);
            Messenger.AddListener(EMessengerType.OnPuzzleCompound, SetButtons);
            Messenger.AddListener(EMessengerType.OnPuzzleCompound, SetPuzzleItem);
        }

        private void SetUI()
        {
            //更新拼图数据
            SetPuzzleItem();

            //创建拼图碎片
            _curFragments.Clear();
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                UMCtrlPuzzleFragmentItem puzzleFragment = CreatePuzzleFragment();
                _curFragments.Add(puzzleFragment);
                _puzzleFragments[i].TotalCount = 2;
                puzzleFragment.SetData(_puzzleFragments[i]);
            }

            //按钮信息
            SetButtons();
        }

        private void SetPuzzleItem()
        {
            _puzzleItem.SetData(_puzzle);
            _cachedView.NameTxt.text = _puzzle.Name;
            _cachedView.LvTxt.text = _puzzle.Level.ToString();
            _cachedView.DescTxt.text = _puzzle.Desc;
        }

        private void SetButtons()
        {
            _cachedView.Unable_Active.SetActive(!CheckActivable());
            _cachedView.Unable_Equip.SetActive(!CheckEquipable());
            SetActiveTxt();
        }

        private void SetActiveTxt()
        {
            if (_puzzle.CurState == PuzzleState.HasActived)
                _cachedView.ActiveTxt.text = _upgrateTxt;
            else
                _cachedView.ActiveTxt.text = _activeTxt;
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

        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.PopUpUI2;
        }

    }
}