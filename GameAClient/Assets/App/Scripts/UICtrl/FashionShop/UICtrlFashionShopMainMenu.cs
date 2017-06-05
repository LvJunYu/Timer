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

        private UMCtrlFashionShopCard _headSelectedFashionCard=null;
        private UMCtrlFashionShopCard _upperSelectedFashionCard = null;
        private UMCtrlFashionShopCard _lowerSelectedFashionCard = null;
        private UMCtrlFashionShopCard _appendageSelectedFashionCard = null;



        #endregion
        #region 属性
        public UMCtrlFashionShopCard SelectHead
        {
            get { return _headSelectedFashionCard; }
            set {
                if (value != _headSelectedFashionCard)
                {
                    if (_headSelectedFashionCard != null)
                    {
                        _headSelectedFashionCard.DownMove();
                        //_headSelectedFashionCard.ChangeDock(false);
                    }
                    //之前的归为
                    _headSelectedFashionCard = value;
                    _headSelectedFashionCard.UpMove();

                    //_cachedView.SelectedHead.text = _headSelectedFashionCard.CardName;

                    //现在的改变位置
                    //_headSelectedFashionCard.ChangeDock(true);

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
                        //_upperSelectedFashionCard.ChangeDock(false);
                    }
                    //之前的归为
                    _upperSelectedFashionCard = value;
                    _upperSelectedFashionCard.UpMove();
                    //_cachedView.SelectedUpper.text = _upperSelectedFashionCard.CardName;

                    //现在的改变位置
                    //_upperSelectedFashionCard.ChangeDock(true);

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

                        //_lowerSelectedFashionCard.ChangeDock(false);
                    }
                    //之前的归为
                    _lowerSelectedFashionCard = value;
                    _lowerSelectedFashionCard.UpMove();
                    // _cachedView.SelectedLower.text = _lowerSelectedFashionCard.CardName;

                    //现在的改变位置
                    //_lowerSelectedFashionCard.ChangeDock(true);

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
                        //_appendageSelectedFashionCard.ChangeDock(false);
                    }
                    //之前的归为
                    _appendageSelectedFashionCard = value;
                    _appendageSelectedFashionCard.UpMove();
                    //_cachedView.SelectedAppendage.text = _appendageSelectedFashionCard.CardName;

                    //现在的改变位置
                    //_appendageSelectedFashionCard.ChangeDock(true);

                }
      
            }
        }
        #endregion

        #region 方法

        private bool JudgeItemOccupied(ShopItem listItem)
        {
            bool rst = false;
            switch (listItem._avatarType)
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
                default:
                    break;
            }
            return rst;
        }

        private bool JudgeItemOwned(ShopItem listItem)
        {
            LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            {
                //Set(listItem);
                SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();

            }, code =>
            {
                SocialGUIManager.ShowPopupDialog("刷新已拥有时装失败", null,
                new KeyValuePair<string, Action>("确定", () => { }));
                LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);
            });
            bool rst = false;
            switch (listItem._avatarType)
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
                default:
                    break;
            }
            return rst;
        }
        /// <summary>
        /// 如果正在选的不为空 并且 部位正在穿着的时装 并且 已经拥有 就穿上
        /// </summary>

	    private void OnCloseChangeFashion()
	    {
	        if (SelectHead != null && !JudgeItemOccupied(SelectHead.ItemInfo) && JudgeItemOwned(SelectHead.ItemInfo))
	        {
	            ChangeFashion(SelectHead.ItemInfo);
	        }
	 

	        if (SelectUpper != null && !JudgeItemOccupied(SelectUpper.ItemInfo)&&JudgeItemOwned(SelectUpper.ItemInfo))
            {
                ChangeFashion(SelectUpper.ItemInfo);
            }


            if (SelectLower != null && !JudgeItemOccupied(SelectLower.ItemInfo)&&JudgeItemOwned(SelectLower.ItemInfo))
            {
                ChangeFashion(SelectLower.ItemInfo);
            }


            if (SelectAppendage != null && !JudgeItemOccupied(SelectAppendage.ItemInfo)&&JudgeItemOwned(SelectAppendage.ItemInfo))
            {
                ChangeFashion(SelectAppendage.ItemInfo);
            }

        }





        private void ChangeFashion(ShopItem listItem)
        {
            if (JudgeItemOwned(listItem))
            {
                if (JudgeItemOccupied(listItem))
                {
                    
                }
                else
                {
                    ChangeAvatarPart(
                        listItem._avatarType,
                        listItem.Id,
                        () =>
                        {
                            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid, () =>
                            {
                                SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().ShowAllUsingAvatar();

                            }, code =>
                            {
                                LogHelper.Error("Network error when get UsingAvatarData, {0}", code);
                            });

                        },
                        () =>
                        {
                            //_cachedView.Message.text = "换装失败";
                        });
                }

            }
            else
            {
                //_cachedView.Message.text = "未拥有请购买";
            }

        }



        public void ChangeAvatarPart(
            EAvatarPart type,
            long partID,
            Action successCallback, Action failedCallback)
        {
            Debug.Log("______类型_______" + type + "______newId_______" + partID);
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
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            RefreshFashionShopPanel();
             
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            
            OnCloseChangeFashion();
            //OnRestoreFashionBtnClick();
            RefreshFashionShopPanel();
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().OnCloseBtnClick();
        }
        /// <summary>
        /// 初始化事件监听
        /// </summary>
        protected override void InitEventListener()
        {
            base.InitEventListener();
//			RegisterEvent(EMessengerType.OnChangeToAppMode, OnReturnToApp);
//            RegisterEvent(EMessengerType.OnAccountLoginStateChanged, OnEvent);
        }
/// <summary>
/// 试穿如果显示的是希望预览的图片
/// 如正在使用为空则显示none
/// 如果不为空则显示正在使用的服装
/// 其他则显示希望预览的图片
/// </summary>
/// <param name="type"></param>
/// <param name="previewTexture"></param>

	    //public void TryFashionOn(ShopItem listItem)
	    //{
     //       switch (listItem._avatarType)
     //       {

     //           case EAvatarPart.AP_Head:
     //               if (_cachedView.UsingHead.text == listItem.Id.ToString())
     //               {
     //                   if (LocalUser.Instance.UsingAvatarData.Head == null)
     //                   {
     //                       _cachedView.UsingHead.text = "none";
     //                   }
     //                   else
     //                   {
     //                       _cachedView.UsingHead.text
     //                       =TableManager.Instance.GetHeadParts((int) LocalUser.Instance.UsingAvatarData.Head.Id).PreviewTexture;
     //                   }
     //               }
     //               else
     //               {
     //                   _cachedView.UsingHead.text = listItem.Id.ToString();
     //               }
     //               ;
     //               break;
     //           case EAvatarPart.AP_Lower:
     //               if (_cachedView.UsingLower.text == listItem.Id.ToString())
     //               {
     //                   if (LocalUser.Instance.UsingAvatarData.Lower == null)
     //                   {
     //                       _cachedView.UsingLower.text = "none";
     //                   }
     //                   else
     //                   {
     //                       _cachedView.UsingLower.text
     //                       = TableManager.Instance.GetLowerBodyParts((int)LocalUser.Instance.UsingAvatarData.Lower.Id).PreviewTexture;
     //                   }
     //               }
     //               else
     //               {
     //                   _cachedView.UsingLower.text = listItem.Id.ToString();
     //               }
     //               ;
     //               break;
     //           case EAvatarPart.AP_Upper:
     //               if (_cachedView.UsingUpper.text == listItem.Id.ToString())
     //               {
     //                   if (LocalUser.Instance.UsingAvatarData.Upper == null)
     //                   {
     //                       _cachedView.UsingUpper.text = "none";
     //                   }
     //                   else
     //                   {
     //                       _cachedView.UsingUpper.text
     //                       = TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Upper.Id).PreviewTexture;
     //                   }
     //               }
     //               else
     //               {
     //                   _cachedView.UsingUpper.text = listItem.Id.ToString();
     //               }
     //               ;
     //               break;

     //           case EAvatarPart.AP_Appendage:
     //               if (_cachedView.UsingAppendage.text == listItem.Id.ToString())
     //               {
     //                   if (LocalUser.Instance.UsingAvatarData.Appendage == null)
     //                   {
     //                       _cachedView.UsingAppendage.text = "none";
     //                   }
     //                   else
     //                   {
     //                       _cachedView.UsingAppendage.text
     //                       = TableManager.Instance.GetAppendageParts((int)LocalUser.Instance.UsingAvatarData.Appendage.Id).PreviewTexture;
     //                   }
     //               }
     //               else
     //               {
     //                   _cachedView.UsingAppendage.text = listItem.Id.ToString();
     //               }
     //               ;
     //               break;
     //       }
     //   }

        /// <summary>
        /// 创建
        /// </summary>
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            InitTagGroup();
            SetRenderTexture();
            
        }

	    private void SetRenderTexture()
	    {
            
            _cachedView.Avatar.texture= SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().AvatarRenderTexture;

	    }

	    public void RefreshFashionShopPanel()
        {
            
            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            {
                SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().ShowAllUsingAvatar();
                //eshUsingAvatarPreview();
            }, code =>
            {
                LogHelper.Error("Network error when get UsingAvatarData, {0}", code);
            });
            LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            {
                InitPageData();
            }, code =>
            {
                LogHelper.Error("Network error when get ValidAvatarData, {0}", code);
            });
        }

    //public void RefreshUsingAvatarPreview()
	   // {
	   //     //Debug.Log(LocalUser.Instance.UsingAvatarData.Head);
    //        //Debug.Log(_cachedView);
    //        //Debug.Log(_cachedView.UsingHead);
	   //     if (LocalUser.Instance.UsingAvatarData.Head != null)
	   //     {
	   //         _cachedView.UsingHead.text = LocalUser.Instance.UsingAvatarData.Head.Id.ToString();
    //            //TableManager.Instance.GetHeadParts((int)LocalUser.Instance.UsingAvatarData.Head.Id).PreviewTexture;

	   //     }
	   //     if (LocalUser.Instance.UsingAvatarData.Upper != null)
	   //     {
    //            _cachedView.UsingUpper.text = LocalUser.Instance.UsingAvatarData.Upper.Id.ToString();
    //            //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Upper.Id).PreviewTexture;

    //        }
	   //     if (LocalUser.Instance.UsingAvatarData.Lower != null)
	   //     {
    //            _cachedView.UsingLower.text = LocalUser.Instance.UsingAvatarData.Lower.Id.ToString();
    //            //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Lower.Id).PreviewTexture;
               
	   //     }
    //        if (LocalUser.Instance.UsingAvatarData.Appendage!= null)
	   //     {
    //            _cachedView.UsingAppendage.text = LocalUser.Instance.UsingAvatarData.Appendage.Id.ToString();
    //            //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Appendage.Id).PreviewTexture;
          
	   //     }

	   // }
	    private void InitTagGroup()
	    {

            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page1Btn, OnFashionPage1ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page2Btn, OnFashionPage2ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page3Btn, OnFashionPage3ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page4Btn, OnFashionPage4ButtonClick);
//            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page5Btn, OnFashionPage5ButtonClick);


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

            //_usctrlFashionPage5 = new USCtrlFashionShop();
            //_usctrlFashionPage5.Init(_cachedView.FashionPage5);
            //_usctrlFashionPage5.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage5;


            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _cachedView.RestoreFashionBtn.onClick.AddListener(OnRestoreFashionBtnClick);
	        _cachedView.PurchaseAllFittingFashionBtn.onClick.AddListener(OnPurchaseAllFittingFashionBtnClick);
	    }


        private void OnPurchaseAllFittingFashionBtnClick()
        {
            //Debug.Log("————————SelectHead.ItemID——————————————" + SelectHead.ItemID);
            //Debug.Log("————————SelectHead.ItemID——————————————" + SelectHead.ItemID);
            //Debug.Log("————————SelectHead.ItemID——————————————" + SelectHead.ItemID);

            List<ShopItem> allFittingList = new List<ShopItem>();

            if (SelectHead != null&&!JudgeItemOwned(SelectHead.ItemInfo))
            {
                if(LocalUser.Instance.ValidAvatarData.GetItemInHeadDictionary(SelectHead.ItemID) == null)
                allFittingList.Add(SelectHead.ItemInfo);
            }
            if (SelectUpper!= null&&!JudgeItemOwned(SelectUpper.ItemInfo))
            {
               if( LocalUser.Instance.ValidAvatarData.GetItemInUpperDictionary(SelectUpper.ItemID) == null)
                allFittingList.Add(SelectUpper.ItemInfo);
            }
            if (SelectLower!= null && !JudgeItemOwned(SelectLower.ItemInfo))
            {
              if(LocalUser.Instance.ValidAvatarData.GetItemInLowerDictionary(SelectLower.ItemID) == null)
                allFittingList.Add(SelectLower.ItemInfo);
            }
            if (SelectAppendage!= null&&!JudgeItemOwned(SelectAppendage.ItemInfo))
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
            //SelectHead = null;
            //SelectUpper = null;
            //SelectLower = null;
            //SelectAppendage = null;
        }

        public void InitPageData()
        {
            if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Male)
            {
                SetFashionPage(1);
            }
            else if (LocalUser.Instance.User.UserInfoSimple.Sex == ESex.S_Female)
            {
                SetFashionPage(2);
            }
            else
            {
                SetFashionPage();
            }
        }
        public void SetFashionPage(int sex)
        {
            var dict = new Dictionary<int, List<ShopItem>>(); //建立字典 键是分页 值为每个分页的itemlist
            List<ShopItem> list = null;

            for (int i = 1; i <= TableManager.Instance.Table_FashionShopDic.Count; i++)//便利tablemanager
            {
                if (TableManager.Instance.Table_FashionShopDic[i].Sex == sex)
                {
                    var item = TableManager.Instance.Table_FashionShopDic[i]; //拿到tablemanager每一列
                    ShopItem shopitem = null;//建立shopitem
                    switch ((EAvatarPart)item.Type)
                    {
                        case EAvatarPart.AP_Appendage: //根据序号找到对应的item
                            shopitem = new ShopItem(TableManager.Instance.GetAppendageParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Head:
                            shopitem = new ShopItem(TableManager.Instance.GetHeadParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Lower:
                            shopitem = new ShopItem(TableManager.Instance.GetLowerBodyParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Upper:
                            shopitem = new ShopItem(TableManager.Instance.GetUpperBodyParts(item.ItemIdx));
                            break;
                    }

                    if (!dict.TryGetValue(item.PageIdx, out list))
                    {                                     //没拿到
                        list = new List<ShopItem>();    //建立list
                        dict.Add(item.PageIdx, list);   //放入字典 key：list value：list
                    }
                    list.Add(shopitem);//放入shopitem
                }
            }
            _usctrlFashionPage2.Set(dict[2]);
            _usctrlFashionPage1.Set(dict[1]);
            _usctrlFashionPage3.Set(dict[3]);
            _usctrlFashionPage4.Set(dict[4]);

        }

        private void SetFashionPage()
        {
            var dict = new Dictionary<int, List<ShopItem>>(); //建立字典 键是分页 值为每个分页的itemlist
            List<ShopItem> list = null;

            for (int i = 1; i <= TableManager.Instance.Table_FashionShopDic.Count; i++)//便利tablemanager
            {
                
                    var item = TableManager.Instance.Table_FashionShopDic[i]; //拿到tablemanager每一列
                    ShopItem shopitem = null;//建立shopitem
                    switch ((EAvatarPart)item.Type)
                    {
                        case EAvatarPart.AP_Appendage: //根据序号找到对应的item
                            shopitem = new ShopItem(TableManager.Instance.GetAppendageParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Head:
                            shopitem = new ShopItem(TableManager.Instance.GetHeadParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Lower:
                            shopitem = new ShopItem(TableManager.Instance.GetLowerBodyParts(item.ItemIdx));
                            break;
                        case EAvatarPart.AP_Upper:
                            shopitem = new ShopItem(TableManager.Instance.GetUpperBodyParts(item.ItemIdx));
                            break;
                    }

                    if (!dict.TryGetValue(item.PageIdx, out list))
                    {                                     //没拿到
                        list = new List<ShopItem>();    //建立list
                        dict.Add(item.PageIdx, list);   //放入字典 key：list value：list
                    }
                    list.Add(shopitem);//放入shopitem
                
            }
            _usctrlFashionPage2.Set(dict[2]);
            _usctrlFashionPage1.Set(dict[1]);
            _usctrlFashionPage3.Set(dict[3]);
            _usctrlFashionPage4.Set(dict[4]);

        }
        //private void TakeDataFromTablemanager()
        //{
        //    var headPartsDic = TableManager.Instance.Table_HeadPartsDic;
        //       var lowerPartsDic = TableManager.Instance.Table_LowerBodyPartsDic;
        //       var upperPartsDic = TableManager.Instance.Table_UpperBodyPartsDic;
        //       var appendagePartsDic = TableManager.Instance.Table_AppendagePartsDic;
        //    _itemMainDic.Add(1, _headParts);
        //       _itemMainDic.Add(2, _upperParts);
        //       _itemMainDic.Add(3, _lowerParts);
        //       _itemMainDic.Add(4, _appendageParts);
        //       for (int i = 0; i < headPartsDic.Count; i++)
        //    {
        //           ShopItem headPart = new ShopItem(headPartsDic[i]);
        //        _headParts.Add(headPart);
        //    }
        //       for (int i = 0; i < lowerPartsDic.Count; i++)
        //       {
        //           ShopItem lowerPart = new ShopItem(lowerPartsDic[i]);
        //           _lowerParts.Add(lowerPart);
        //       }
        //       for (int i = 0; i < upperPartsDic.Count; i++)
        //       {
        //           ShopItem upperPart = new ShopItem(upperPartsDic[i]);
        //           _upperParts.Add(upperPart);
        //       }
        //       for (int i = 0; i < appendagePartsDic.Count; i++)
        //    {
        //           ShopItem appendage = new ShopItem(appendagePartsDic[i]);
        //           _appendageParts.Add(appendage);
        //    }
        //   }
        //private void MakePageList()
        // {
        //        var fashionShopDic = TableManager.Instance.Table_FashionShopDic;
        //     for (int i = 0; i <fashionShopDic.Count; i++)
        //     {
        //         //ShopItem pageItem = new ShopItem(fashionShopDic[i]);
        //         var ItemType = fashionShopDic[i].Type;
        //            var ItemIdx = fashionShopDic[i].ItemIdx;
        //            var Sex = fashionShopDic[i].Sex;

        //                 switch (fashionShopDic[i].Type)
        //            {
        //                case 1:
        //                    _page1.Add(_itemMainDic[ItemType][ItemIdx]);
        //                    break;
        //                case 2:
        //                    _page2.Add(_itemMainDic[ItemType][ItemIdx]);
        //                    break;
        //                case 3:
        //                    _page3.Add(_itemMainDic[ItemType][ItemIdx]);
        //                    break;
        //                case 4:
        //                    _page4.Add(_itemMainDic[ItemType][ItemIdx]);
        //                    break;

        /*界面的切换这个打开和关闭*/
        private void OnFashionPage1ButtonClick(bool open)
		{
            _cachedView.SeletctedPage1Image.SetActiveEx(open);
            if (open)
			{
                _usctrlFashionPage1.Open ();
			}
			else 
			{
                _usctrlFashionPage1.Close();
			}
		}

		private void OnFashionPage2ButtonClick(bool open)
		{
            _cachedView.SeletctedPage2Image.SetActiveEx(open);

            if (open) {
                _usctrlFashionPage2.Open ();
			}else
			{
                _usctrlFashionPage2.Close ();
			}
		}
		private void OnFashionPage3ButtonClick(bool open)
		{
            _cachedView.SeletctedPage3Image.SetActiveEx(open);

            if (open){
                _usctrlFashionPage3.Open ();
			}else
			{
                _usctrlFashionPage3.Close ();
			}
		}
		private void OnFashionPage4ButtonClick(bool open)
		{
            _cachedView.SeletctedPage4Image.SetActiveEx(open);
            if (open) {
                _usctrlFashionPage4.Open ();
			} else
                _usctrlFashionPage4.Close ();
		}

        //private void OnFashionPage5ButtonClick(bool open)
        //{
        //    if (open)
        //    {
        //        _usctrlFashionPage5.Open();
        //    }
        //    else
        //        _usctrlFashionPage5.Close();
        //}
        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.MainUI;
        }			
        /// <summary>
        /// 关闭按钮
        /// </summary>
		private void OnCloseBtnClick () {
			SocialGUIManager.Instance.CloseUI<UICtrlFashionShopMainMenu> ();
		}
        #endregion 接口
        #endregion
    }
}
