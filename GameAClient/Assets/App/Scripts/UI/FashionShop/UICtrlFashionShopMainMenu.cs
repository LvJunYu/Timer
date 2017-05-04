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
		private USCtrlShopping _usctrlAllShopping;//对应的界面按钮对应的各个界面实现切换
		private USCtrlShopping _usctrlHeadShopping;//头
		private USCtrlShopping _usctrlFashionPage1;//第一页
		private USCtrlShopping _usctrlLegShopping;//腿
		private USCtrlShopping _usctrlZSusShopping;//装饰

	    private List<ShopItem> _page1 = new List<ShopItem>();
        private List<ShopItem> _page2 = new List<ShopItem>();
        private List<ShopItem> _page3 = new List<ShopItem>();
        private List<ShopItem> _page4 = new List<ShopItem>();
        private List<ShopItem> _headParts = new List<ShopItem>();
        private List<ShopItem> _upperParts = new List<ShopItem>();
        private List<ShopItem> _lowerParts = new List<ShopItem>();
        private List<ShopItem> _appendageParts = new List<ShopItem>();
	    private Dictionary<int,List<ShopItem>> _itemMainDic = new Dictionary<int, List<ShopItem>>();




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
        /// 创建
        /// </summary>
        protected override void OnViewCreated()
        {
            
            base.OnViewCreated();
//			_cachedView.TagGroup.AddButton(_cachedView.CloseBtn, OnSYbuttonClick);

			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.Page1Btn,OnFashionPage1ButtonClick);
			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.KZbutton, OnKZbuttonClick);
			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.MZbutton, OnMZbuttonClick);
			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.XZbutton, OnXZbuttonClick);
			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.ZSbutton, OnZSbuttonClick);


			_usctrlAllShopping = new USCtrlShopping ();
			_usctrlAllShopping.Init(_cachedView.XZViewShopping);
			_usctrlAllShopping.ShoppingPage = USCtrlShopping.EShoppingPage.XZUIViewShopping;

			_usctrlHeadShopping = new USCtrlShopping ();
			_usctrlHeadShopping.Init(_cachedView.MZViewShopping);
			_usctrlHeadShopping.ShoppingPage = USCtrlShopping.EShoppingPage.MZUIViewShopping;

            _usctrlFashionPage1 = new USCtrlShopping ();
            _usctrlFashionPage1.Init(_cachedView.FashionPage1);
            _usctrlFashionPage1.ShoppingPage = USCtrlShopping.EShoppingPage.FashionPage1;

			_usctrlLegShopping = new USCtrlShopping ();
			_usctrlLegShopping.Init(_cachedView.USViewShopping);
			_usctrlLegShopping = new USCtrlShopping ();
			_usctrlLegShopping.Init(_cachedView.KZViewShopping);
			_usctrlLegShopping.ShoppingPage = USCtrlShopping.EShoppingPage.KZUIViewShopping;

			_usctrlZSusShopping = new USCtrlShopping ();
			_usctrlZSusShopping .Init(_cachedView.ZSViewShopping);
			_usctrlZSusShopping.ShoppingPage = USCtrlShopping.EShoppingPage.ZSUIViewShopping;



			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtnClick);

            TakeDataFromTablemanager();

        }

	    private void TakeDataFromTablemanager()
	    {
	        var headPartsDic = TableManager.Instance.Table_HeadPartsDic;
            var lowerPartsDic = TableManager.Instance.Table_LowerBodyPartsDic;
            var upperPartsDic = TableManager.Instance.Table_UpperBodyPartsDic;
            var appendagePartsDic = TableManager.Instance.Table_AppendagePartsDic;
	        _itemMainDic.Add(1, _headParts);
            _itemMainDic.Add(2, _upperParts);
            _itemMainDic.Add(3, _lowerParts);
            _itemMainDic.Add(4, _appendageParts);



            for (int i = 0; i < headPartsDic.Count; i++)
	        {
                ShopItem headPart = new ShopItem(headPartsDic[i]);
	            _headParts.Add(headPart);
	        }
            for (int i = 0; i < lowerPartsDic.Count; i++)
            {
                ShopItem lowerPart = new ShopItem(lowerPartsDic[i]);
                _lowerParts.Add(lowerPart);
            }
            for (int i = 0; i < upperPartsDic.Count; i++)
            {
                ShopItem upperPart = new ShopItem(upperPartsDic[i]);
                _upperParts.Add(upperPart);
            }
            for (int i = 0; i < appendagePartsDic.Count; i++)
	        {
                ShopItem appendage = new ShopItem(appendagePartsDic[i]);
                _appendageParts.Add(appendage);
	        }

        }


    private void MakePageList()
	    {
            var fashionShopDic = TableManager.Instance.Table_FashionShopDic;
	        for (int i = 0; i <fashionShopDic.Count; i++)
	        {
	            //ShopItem pageItem = new ShopItem(fashionShopDic[i]);
	            var ItemType = fashionShopDic[i].Type;
                var ItemIdx = fashionShopDic[i].ItemIdx;
                var Sex = fashionShopDic[i].Sex;

                     switch (fashionShopDic[i].Type)
                {
                    case 1:
                        _page1.Add(_itemMainDic[ItemType][ItemIdx]);
                        break;
                    case 2:
                        _page2.Add(_itemMainDic[ItemType][ItemIdx]);
                        break;
                    case 3:
                        _page3.Add(_itemMainDic[ItemType][ItemIdx]);
                        break;
                    case 4:
                        _page4.Add(_itemMainDic[ItemType][ItemIdx]);
                        break;


                }



                
	        }

        }





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

		private void OnKZbuttonClick(bool open)
		{
			if (open) {
				_usctrlLegShopping.Open ();
			}else
			{
				_usctrlLegShopping.Close ();
			}
		}
		private void OnXZbuttonClick(bool open)
		{
			if(open){
				_usctrlAllShopping.Open ();
			}else
			{
				_usctrlAllShopping.Close ();
			}
		}
		private void OnZSbuttonClick(bool open)
		{
			if (open) {
				_usctrlZSusShopping.Open ();
			} else
				_usctrlZSusShopping.Close ();
		}
//		private void OnBuybuttonClick(bool open)
//		{
//			 
//		}
		private void OnMZbuttonClick(bool open)
		{
			if(open){
				_usctrlHeadShopping.Open ();
			}
			else 
			{
				_usctrlHeadShopping.Close();
		   }
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
