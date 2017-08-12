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
        private UMCtrlPuzzleDetailItem _curUMPuzzleItem;
        private List<UMCtrlPuzzleFragmentItem> _fragmentsCache;
        private List<UMCtrlPuzzleFragmentItem> _curUMFragments;
        private const string _upgrateTxt = "升级";
        private const string _activeTxt = "合成";

        private void OnEquipBtn()
        {
            SocialGUIManager.Instance.OpenUI<UICtrlPuzzleSlots>(_puzzle);
        }

        private void OnActiveBtn()
        {
            if (!GameATools.CheckGold(_puzzle.CostMoeny, true))
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
                    RequesUserPictureFull();
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
                       RequesUserPictureFull();
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
            };
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

        private void SetFragments()
        {
            for (int i = 0; i < _curUMFragments.Count; i++)
            {
                _curUMFragments[i].SetData();
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

        private void SetUI()
        {
            //创建拼图碎片Item
            _curUMFragments.Clear();
            for (int i = 0; i < _puzzleFragments.Length; i++)
            {
                UMCtrlPuzzleFragmentItem puzzleFragment = CreatePuzzleFragment();
                _curUMFragments.Add(puzzleFragment);
                //测试用
                _puzzleFragments[i].TotalCount = UnityEngine.Random.Range(0, 4);
                puzzleFragment.SetData(_puzzleFragments[i]);
            }

            //拼图数据
            _curUMPuzzleItem.SetData(_puzzle);
            //文字信息
            SetTexts();
            //按钮信息
            SetButtons();
        }

        private void SetTexts()
        {
            _cachedView.NameTxt.text = _puzzle.Name;
            _cachedView.LvTxt.text = _puzzle.Level.ToString();
            _cachedView.DescTxt.text = _puzzle.Desc;
        }

        private void SetButtons()
        {
            //设置按钮是否可用
            _cachedView.Unable_Active.SetActive(!CheckActivable());
            _cachedView.ActiveBtn.gameObject.SetActive(CheckActivable());
            _cachedView.Unable_Equip.SetActive(!CheckEquipable());
            _cachedView.EquipBtn.gameObject.SetActive(CheckEquipable());
            //按钮上显示消耗的金币
            _cachedView.CostNumTxt.text = _puzzle.CostMoeny.ToString();
            //按钮上显示合成或升级文字
            if (_puzzle.CurState == EPuzzleState.HasActived)
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
            return _puzzle.CurState == EPuzzleState.HasActived;
        }

        private void OnPuzzleCompound()
        {
            //更新拼图
            _curUMPuzzleItem.SetData();
            //更新碎片
            SetFragments();
            SetTexts();
            SetButtons();
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
            _curUMFragments = new List<UMCtrlPuzzleFragmentItem>(9);
            //创建拼图
            _curUMPuzzleItem = new UMCtrlPuzzleDetailItem();
            _curUMPuzzleItem.Init(_cachedView.PuzzleItemPos);
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent(EMessengerType.OnPuzzleCompound, OnPuzzleCompound);
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