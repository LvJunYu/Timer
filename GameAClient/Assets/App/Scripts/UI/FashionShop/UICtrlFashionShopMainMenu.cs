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
	public class UICtrlFashionShopMainMenu : UISocialCtrlBase<UIViewFashionShopMainMenu>
    {
		#region 常量与字段
		private USCtrlFashionShop _usctrlFashionPage1;
		private USCtrlFashionShop _usctrlFashionPage2;
		private USCtrlFashionShop _usctrlFashionPage3;
		private USCtrlFashionShop _usctrlFashionPage4;
	    private USCtrlFashionShop _usctrlFashionPage5;




        #endregion

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnOpen (object parameter)
        {
            base.OnOpen (parameter);
            RefreshFashionShopPanel();
            //			TableManager.Instance.GetFashionShop ();
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
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

	    public void TryFashionOn(ShopItem listItem)
	    {
            switch (listItem._avatarType)
            {

                case EAvatarPart.AP_Head:
                    if (_cachedView.UsingHead.text == listItem.Id.ToString())
                    {
                        if (LocalUser.Instance.UsingAvatarData.Head == null)
                        {
                            _cachedView.UsingHead.text = "none";
                        }
                        else
                        {
                            _cachedView.UsingHead.text
                            =TableManager.Instance.GetHeadParts((int) LocalUser.Instance.UsingAvatarData.Head.Id).PreviewTexture;
                        }
                    }
                    else
                    {
                        _cachedView.UsingHead.text = listItem.Id.ToString();
                    }
                    ;
                    break;
                case EAvatarPart.AP_Lower:
                    if (_cachedView.UsingLower.text == listItem.Id.ToString())
                    {
                        if (LocalUser.Instance.UsingAvatarData.Lower == null)
                        {
                            _cachedView.UsingLower.text = "none";
                        }
                        else
                        {
                            _cachedView.UsingLower.text
                            = TableManager.Instance.GetLowerBodyParts((int)LocalUser.Instance.UsingAvatarData.Lower.Id).PreviewTexture;
                        }
                    }
                    else
                    {
                        _cachedView.UsingLower.text = listItem.Id.ToString();
                    }
                    ;
                    break;
                case EAvatarPart.AP_Upper:
                    if (_cachedView.UsingUpper.text == listItem.Id.ToString())
                    {
                        if (LocalUser.Instance.UsingAvatarData.Upper == null)
                        {
                            _cachedView.UsingUpper.text = "none";
                        }
                        else
                        {
                            _cachedView.UsingUpper.text
                            = TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Upper.Id).PreviewTexture;
                        }
                    }
                    else
                    {
                        _cachedView.UsingUpper.text = listItem.Id.ToString();
                    }
                    ;
                    break;

                case EAvatarPart.AP_Appendage:
                    if (_cachedView.UsingAppendage.text == listItem.Id.ToString())
                    {
                        if (LocalUser.Instance.UsingAvatarData.Appendage == null)
                        {
                            _cachedView.UsingAppendage.text = "none";
                        }
                        else
                        {
                            _cachedView.UsingAppendage.text
                            = TableManager.Instance.GetAppendageParts((int)LocalUser.Instance.UsingAvatarData.Appendage.Id).PreviewTexture;
                        }
                    }
                    else
                    {
                        _cachedView.UsingAppendage.text = listItem.Id.ToString();
                    }
                    ;
                    break;
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        protected override void OnViewCreated()
        {
            //RefreshFashionShopPanel();
        }

        public void RefreshFashionShopPanel()
        {
            
            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            {
            RefreshUsingAvatarPreview();
            }, code =>
            {
                LogHelper.Error("Network error when get UsingAvatarData, {0}", code);
            });
            LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            {

            }, code =>
            {
                LogHelper.Error("Network error when get ValidAvatarData, {0}", code);
            });
            InitTagGroup();
            InitPageData();
        }

        public void RefreshUsingAvatarPreview()
	    {
	        //Debug.Log(LocalUser.Instance.UsingAvatarData.Head);
            //Debug.Log(_cachedView);
            //Debug.Log(_cachedView.UsingHead);
	        if (LocalUser.Instance.UsingAvatarData.Head != null)
	        {
	            _cachedView.UsingHead.text = LocalUser.Instance.UsingAvatarData.Head.Id.ToString();
                //TableManager.Instance.GetHeadParts((int)LocalUser.Instance.UsingAvatarData.Head.Id).PreviewTexture;

	        }
	        if (LocalUser.Instance.UsingAvatarData.Upper != null)
	        {
                _cachedView.UsingUpper.text = LocalUser.Instance.UsingAvatarData.Upper.Id.ToString();
                //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Upper.Id).PreviewTexture;

            }
	        if (LocalUser.Instance.UsingAvatarData.Lower != null)
	        {
                _cachedView.UsingLower.text = LocalUser.Instance.UsingAvatarData.Lower.Id.ToString();
                //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Lower.Id).PreviewTexture;
               
	        }
            if (LocalUser.Instance.UsingAvatarData.Appendage!= null)
	        {
                _cachedView.UsingAppendage.text = LocalUser.Instance.UsingAvatarData.Appendage.Id.ToString();
                //TableManager.Instance.GetUpperBodyParts((int)LocalUser.Instance.UsingAvatarData.Appendage.Id).PreviewTexture;
          
	        }

	    }
	    private void InitTagGroup()
	    {
            base.OnViewCreated();

            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page1Btn, OnFashionPage1ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page2Btn, OnFashionPage2ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page3Btn, OnFashionPage3ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page4Btn, OnFashionPage4ButtonClick);
            _cachedView.TagGroup.AddButton(_cachedView.USViewShop.Page5Btn, OnFashionPage5ButtonClick);

            //_usctrlAllShopping = new USCtrlFashionShop();
            //_usctrlAllShopping.Init(_cachedView.XZViewShopping);
            //_usctrlAllShopping.ShoppingPage = USCtrlFashionShop.EShoppingPage.XZUIViewShopping;

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

            _usctrlFashionPage5 = new USCtrlFashionShop();
            _usctrlFashionPage5.Init(_cachedView.FashionPage5);
            _usctrlFashionPage5.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage5;


            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);


        }

	    public void InitPageData()
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
	                    shopitem =new ShopItem(TableManager.Instance.GetAppendageParts(item.ItemIdx));
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
            //for (int i = 0; i < listcount; i++)
            //   { 

            //}
            _usctrlFashionPage2.Set(dict[2]);
            _usctrlFashionPage1.Set(dict[1]);
	        _usctrlFashionPage3.Set(dict[3]);
	        _usctrlFashionPage4.Set(dict[4]);
            _usctrlFashionPage5.Set(dict[5]);




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
			if (open) {
                _usctrlFashionPage2.Open ();
			}else
			{
                _usctrlFashionPage2.Close ();
			}
		}
		private void OnFashionPage3ButtonClick(bool open)
		{
			if(open){
                _usctrlFashionPage3.Open ();
			}else
			{
                _usctrlFashionPage3.Close ();
			}
		}
		private void OnFashionPage4ButtonClick(bool open)
		{
			if (open) {
                _usctrlFashionPage4.Open ();
			} else
                _usctrlFashionPage4.Close ();
		}

        private void OnFashionPage5ButtonClick(bool open)
        {
            if (open)
            {
                _usctrlFashionPage5.Open();
            }
            else
                _usctrlFashionPage5.Close();
        }


        #region 接口
        protected override void InitGroupId()
        {
			_groupId = (int)EUIGroupType.PopUpUI;
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
