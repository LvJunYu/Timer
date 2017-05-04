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
		private USCtrlShopping _usctrlBodyShopping;//上衣
		private USCtrlShopping _usctrlLegShopping;//腿
		private USCtrlShopping _usctrlZSusShopping;//装饰
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

			_cachedView.TagGroup.AddButton(_cachedView.USViewShopping.SYbutton, OnSYbuttonClick);//添加按钮
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

			_usctrlBodyShopping = new USCtrlShopping ();
			_usctrlBodyShopping.Init(_cachedView.SYViewShopping);
			_usctrlBodyShopping.ShoppingPage = USCtrlShopping.EShoppingPage.SYUIViewShopping;

			_usctrlLegShopping = new USCtrlShopping ();
			_usctrlLegShopping.Init(_cachedView.USViewShopping);
			_usctrlLegShopping = new USCtrlShopping ();
			_usctrlLegShopping.Init(_cachedView.KZViewShopping);
			_usctrlLegShopping.ShoppingPage = USCtrlShopping.EShoppingPage.KZUIViewShopping;

			_usctrlZSusShopping = new USCtrlShopping ();
			_usctrlZSusShopping .Init(_cachedView.ZSViewShopping);
			_usctrlZSusShopping.ShoppingPage = USCtrlShopping.EShoppingPage.ZSUIViewShopping;



			_cachedView.CloseBtn.onClick.AddListener (OnCloseBtnClick);
        }






		/*界面的切换这个打开和关闭*/
		private void OnSYbuttonClick(bool open)
		{
			if (open)
			{
				_usctrlBodyShopping.Open ();
			}
			else 
			{
				_usctrlBodyShopping.Close();
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
