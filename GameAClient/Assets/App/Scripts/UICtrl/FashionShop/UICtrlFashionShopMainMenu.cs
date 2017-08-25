/********************************************************************
** Filename : UICtrlFashionShop
** Author : Quan
** Date : 2015/4/30 16:35:16
** Summary : UICtrlSingleMode
* 单人游戏UI控制
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlFashionShopMainMenu : UICtrlGenericBase<UIViewFashionShopMainMenu>
    {
        #region 常量与字段
        private USCtrlFashionShop _usctrlFashionPage1;
        private USCtrlFashionShop _usctrlFashionPage2;
        private USCtrlFashionShop _usctrlFashionPage3;
        private USCtrlFashionShop _usctrlFashionPage4;
        private UMCtrlFashionShopCard _headSelectedFashionCard = null;
        private UMCtrlFashionShopCard _upperSelectedFashionCard = null;
        private UMCtrlFashionShopCard _lowerSelectedFashionCard = null;
        private UMCtrlFashionShopCard _appendageSelectedFashionCard = null;
        private RectTransform _head;
        private RectTransform _upBody;
        private RectTransform _lowerpart;
        private UIParticleItem _uiParticleItem;

        #endregion


        #region 属性
        public RectTransform Head
        {
            get { return _head; }
            set { _head = value; }
        }
        public RectTransform UpBody
        {
            get { return _upBody; }
            set { _upBody = value; }
        }
        public RectTransform Lowerpart
        {
            get { return _lowerpart; }
            set { _lowerpart = value; }
        }
        public UIParticleItem UiParticleItem
        {
            get
            {
                //if (_uiParticleItem != null)
                //{
                //    _uiParticleItem.Particle.DestroySelf();
                //}
                return _uiParticleItem;
            }
            set
            {
                if (_uiParticleItem != null)
                {
                    _uiParticleItem.Particle.DestroySelf();
                }
                _uiParticleItem = value;
            }
        }

        public UMCtrlFashionShopCard SelectHead
        {
            get { return _headSelectedFashionCard; }
            set
            {
                if (value != _headSelectedFashionCard)
                {
                    if (_headSelectedFashionCard != null)
                    {
                        _headSelectedFashionCard.DownMove();
                    }
                    _headSelectedFashionCard = value;
                    _headSelectedFashionCard.UpMove();
                }
            }
        }

        public UMCtrlFashionShopCard SelectUpper
        {
            get { return _upperSelectedFashionCard; }
            set
            {
                if (value != _upperSelectedFashionCard)
                {
                    if (_upperSelectedFashionCard != null)
                    {
                        _upperSelectedFashionCard.DownMove();
                    }
                    _upperSelectedFashionCard = value;
                    _upperSelectedFashionCard.UpMove();
                }
            }
        }

        public UMCtrlFashionShopCard SelectLower
        {
            get { return _lowerSelectedFashionCard; }
            set
            {
                if (value != _lowerSelectedFashionCard)
                {
                    if (_lowerSelectedFashionCard != null)
                    {
                        _lowerSelectedFashionCard.DownMove();
                    }
                    _lowerSelectedFashionCard = value;
                    _lowerSelectedFashionCard.UpMove();
                }
            }
        }

        public UMCtrlFashionShopCard SelectAppendage
        {
            get { return _appendageSelectedFashionCard; }
            set
            {
                if (value != _appendageSelectedFashionCard)
                {
                    if (_appendageSelectedFashionCard != null)
                    {
                        _appendageSelectedFashionCard.DownMove();
                    }
                    _appendageSelectedFashionCard = value;
                    _appendageSelectedFashionCard.UpMove();
                }
            }
        }
        #endregion

        #region 方法

        public void SetFashionPage(int sex)
        {
            var dict = new Dictionary<int, List<Table_FashionUnit>>(); //建立字典 键是分页 值为每个分页的itemlist
            List<Table_FashionUnit> list = null;
            for (int i = 0; i < TableManager.Instance.Table_FashionShopDic.Count; i++)
            {
                var item = TableManager.Instance.Table_FashionShopDic.ElementAt(i).Value;
                if (item.Sex != sex)
                {
                    continue;
                }
                Table_FashionUnit fashionUnit = null; //建立shopitem
                if (TableManager.Instance.GetFashionUnit(item.ItemIdx) == null)
                {
                    LogHelper.Error("Network error when setFashionPage, {0}", item.Type);
                    continue;
                }
                fashionUnit = TableManager.Instance.GetFashionUnit(item.ItemIdx);
                if (!dict.TryGetValue(item.PageIdx, out list))
                {
                    //没拿到
                    list = new List<Table_FashionUnit>(); //建立list
                    dict.Add(item.PageIdx, list); //放入字典 key：list value：list
                }
                list.Add(fashionUnit); //放入shopitem
            }
            _usctrlFashionPage2.Set(dict[2]);
            _usctrlFashionPage1.Set(dict[1]);
            _usctrlFashionPage3.Set(dict[3]);
            _usctrlFashionPage4.Set(dict[4]);
        }

        public void RefreshFashionShopPanel()
        {
            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid,
                () => { SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().ShowAllUsingAvatar(); },
                code => { LogHelper.Error("Network error when get UsingAvatarData, {0}", code); });
            LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () => { InitPageData(); },
                code => { LogHelper.Error("Network error when get ValidAvatarData, {0}", code); });
        }

        public void ChangeAvatarPart(
            EAvatarPart type,
            long partID,
            Action successCallback, Action failedCallback)
        {
            RemoteCommands.ChangeAvatarPart(type, partID, (ret) =>
            {
                if (ret.ResultCode == (int)EChangeAvatarPartCode.CAPC_Success)
                {
                    if (null != successCallback)
                    {
                        successCallback.Invoke();
                    }
                }
                else
                {
                    if (null != failedCallback)
                    {
                        failedCallback.Invoke();
                    }
                }
            }, (errorCode) =>
            {
                if (null != failedCallback)
                {
                    failedCallback.Invoke();
                }
            });
        }

        private bool JudgeItemOccupied(Table_FashionUnit listItem)
        {
            bool rst = false;
            switch ((EAvatarPart) listItem.Type)
            {
                case EAvatarPart.AP_Head:
                    if (LocalUser.Instance.UsingAvatarData.Head != null)
                    {
                        rst = listItem.Id == LocalUser.Instance.UsingAvatarData.Head.Id ? true : false;
                    }
                    break;
                case EAvatarPart.AP_Lower:
                    if (LocalUser.Instance.UsingAvatarData.Lower != null)
                    {
                        rst = listItem.Id == LocalUser.Instance.UsingAvatarData.Lower.Id ? true : false;
                    }
                    break;
                case EAvatarPart.AP_Upper:
                    if (LocalUser.Instance.UsingAvatarData.Upper != null)
                    {
                        rst = listItem.Id == LocalUser.Instance.UsingAvatarData.Upper.Id ? true : false;
                    }
                    break;
                case EAvatarPart.AP_Appendage:
                    if (LocalUser.Instance.UsingAvatarData.Appendage != null)
                    {
                        rst = listItem.Id == LocalUser.Instance.UsingAvatarData.Appendage.Id ? true : false;
                    }
                    break;
            }
            return rst;
        }

        private bool JudgeItemOwned(Table_FashionUnit listItem)
        {
            //LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            //{
            //    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
            //}, code =>
            //{
            //    SocialGUIManager.ShowPopupDialog("刷新已拥有时装失败", null,
            //        new KeyValuePair<string, Action>("确定", () => { }));
            //    LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);
            //});
            bool rst = false;
            switch ((EAvatarPart) listItem.Type)
            {
                case EAvatarPart.AP_Head:
                    rst = LocalUser.Instance.ValidAvatarData.GetItemInHeadDictionary(listItem.Id) != null ? true : false;
                    break;
                case EAvatarPart.AP_Lower:
                    rst = LocalUser.Instance.ValidAvatarData.GetItemInLowerDictionary(listItem.Id) != null
                        ? true
                        : false;
                    break;
                case EAvatarPart.AP_Upper:
                    rst = LocalUser.Instance.ValidAvatarData.GetItemInUpperDictionary(listItem.Id) != null
                        ? true
                        : false;
                    break;
                case EAvatarPart.AP_Appendage:
                    rst = LocalUser.Instance.ValidAvatarData.GetItemInAppendageDictionary(listItem.Id) != null
                        ? true
                        : false;
                    break;
            }
            return rst;
        }

        /// <summary>
        /// 如果正在选的时装不是已经穿上的但是已经拥有 关闭界面时穿上时装
        /// </summary>
        private void OnCloseChangeFashion()
        {
            if (SelectHead != null && !JudgeItemOccupied(SelectHead.ItemInfo) && JudgeItemOwned(SelectHead.ItemInfo))
            {
                ChangeFashion(SelectHead.ItemInfo);
            }

            if (SelectUpper != null && !JudgeItemOccupied(SelectUpper.ItemInfo) && JudgeItemOwned(SelectUpper.ItemInfo))
            {
                ChangeFashion(SelectUpper.ItemInfo);
            }

            if (SelectLower != null && !JudgeItemOccupied(SelectLower.ItemInfo) && JudgeItemOwned(SelectLower.ItemInfo))
            {
                ChangeFashion(SelectLower.ItemInfo);
            }

            if (SelectAppendage != null && !JudgeItemOccupied(SelectAppendage.ItemInfo) &&
                JudgeItemOwned(SelectAppendage.ItemInfo))
            {
                ChangeFashion(SelectAppendage.ItemInfo);
            }
        }

        private void ChangeFashion(Table_FashionUnit listItem)
        {
            if (JudgeItemOwned(listItem))
            {
                if (!JudgeItemOccupied(listItem))
                {
                    ChangeAvatarPart(
                        (EAvatarPart) listItem.Type,
                        listItem.Id,
                        () =>
                        {
                            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid,
                                () => { SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().ShowAllUsingAvatar(); },
                                code => { LogHelper.Error("Network error when get UsingAvatarData, {0}", code); });
                        },
                        null);
                }
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            RefreshFashionShopPanel();
            Head = _cachedView.Head;
            UpBody = _cachedView.UpBody;
            Lowerpart = _cachedView.Lowerpart;
        }

        protected override void OnClose()
        {
            base.OnClose();
            OnCloseChangeFashion();
            RefreshFashionShopPanel();
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().OnCloseBtnClick();
            _uiParticleItem.Particle.DestroySelf();
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitTagGroup();
            SetRenderTexture();
        }

        private void SetRenderTexture()
        {
            _cachedView.Avatar.texture = SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().AvatarRenderTexture;
        }

        private void InitTagGroup()
        {
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page1Btn, OnFashionPage1ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page2Btn, OnFashionPage2ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page3Btn, OnFashionPage3ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page4Btn, OnFashionPage4ButtonClick);

            _usctrlFashionPage1 = new USCtrlFashionShop();
            _usctrlFashionPage1.Init(_cachedView.FashionPage1);
            _usctrlFashionPage1.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage1;

            _usctrlFashionPage2 = new USCtrlFashionShop();
            _usctrlFashionPage2.Init(_cachedView.FashionPage2);
            _usctrlFashionPage2.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage2;

            _usctrlFashionPage3 = new USCtrlFashionShop();
            _usctrlFashionPage3.Init(_cachedView.FashionPage3);
            _usctrlFashionPage3.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage3;

            _usctrlFashionPage4 = new USCtrlFashionShop();
            _usctrlFashionPage4.Init(_cachedView.FashionPage4);
            _usctrlFashionPage4.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage4;

            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _cachedView.RestoreFashionBtn.onClick.AddListener(OnRestoreFashionBtnClick);
            _cachedView.PurchaseAllFittingFashionBtn.onClick.AddListener(OnPurchaseAllFittingFashionBtnClick);
        }

        private void OnPurchaseAllFittingFashionBtnClick()
        {
            List<Table_FashionUnit> allFittingList = new List<Table_FashionUnit>();

            if (SelectHead != null && !JudgeItemOwned(SelectHead.ItemInfo))
            {
                if (LocalUser.Instance.ValidAvatarData.GetItemInHeadDictionary(SelectHead.ItemID) == null)
                    allFittingList.Add(SelectHead.ItemInfo);
            }
            if (SelectUpper != null && !JudgeItemOwned(SelectUpper.ItemInfo))
            {
                if (LocalUser.Instance.ValidAvatarData.GetItemInUpperDictionary(SelectUpper.ItemID) == null)
                    allFittingList.Add(SelectUpper.ItemInfo);
            }
            if (SelectLower != null && !JudgeItemOwned(SelectLower.ItemInfo))
            {
                if (LocalUser.Instance.ValidAvatarData.GetItemInLowerDictionary(SelectLower.ItemID) == null)
                    allFittingList.Add(SelectLower.ItemInfo);
            }
            if (SelectAppendage != null && !JudgeItemOwned(SelectAppendage.ItemInfo))
            {
                if (LocalUser.Instance.ValidAvatarData.GetItemInAppendageDictionary(SelectAppendage.ItemID) == null)
                    allFittingList.Add(SelectAppendage.ItemInfo);
            }

            if (allFittingList.Count == 0)
            {
                SocialGUIManager.ShowPopupDialog("请至少选择一件时装", null,
                    new KeyValuePair<string, Action>("确定", () => { }));
            }
            else
            {
                SocialGUIManager.Instance.OpenUI<UICtrlShopingCart>();
                SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().Set(allFittingList);
            }
        }

        private void OnRestoreFashionBtnClick()
        {
            SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().ShowAllUsingAvatar();
        }

        private void InitPageData()
        {
            if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Male)
            {
                SetFashionPage((int) ESex.S_Male);
            }
            else if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Female)
            {
                SetFashionPage((int) ESex.S_Male);
            }
            else
            {
            }
        }


        private void OnFashionPage1ButtonClick(bool open)
        {
            _cachedView.SeletctedPage1Image.SetActiveEx(open);
            if (open)
            {
                _usctrlFashionPage1.Open();
            }
            else
            {
                _usctrlFashionPage1.Close();
            }
        }

        private void OnFashionPage2ButtonClick(bool open)
        {
            _cachedView.SeletctedPage2Image.SetActiveEx(open);
            if (open)
            {
                _usctrlFashionPage2.Open();
            }
            else
            {
                _usctrlFashionPage2.Close();
            }
        }

        private void OnFashionPage3ButtonClick(bool open)
        {
            _cachedView.SeletctedPage3Image.SetActiveEx(open);
            if (open)
            {
                _usctrlFashionPage3.Open();
            }
            else
            {
                _usctrlFashionPage3.Close();
            }
        }

        private void OnFashionPage4ButtonClick(bool open)
        {
            _cachedView.SeletctedPage4Image.SetActiveEx(open);
            if (open)
            {
                _usctrlFashionPage4.Open();
            }
            else
                _usctrlFashionPage4.Close();
        }

        #region 接口

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        private void OnCloseBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlFashionShopMainMenu>();
        }

        #endregion 接口

        #endregion
    }
}
