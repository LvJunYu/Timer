
using System;
using System.Collections;
using System.Collections.Generic;
using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class USCtrlFashionShop : USCtrlBase <USViewFashionShop>
	{
        private EShoppingPage _shoppingPage;
        public EShoppingPage ShoppingPage
        {
            get
            {
                return this._shoppingPage;
            }
            set
            {
                _shoppingPage = value;
            }
        }

        public void Open()
        {
            _cachedView.gameObject.SetActive(true);
            Refreash();
        }

	    private void Refreash()
	    {
	        switch (_shoppingPage)
	        {
	            case EShoppingPage.FashionPage1:
	                if (SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectHead != null)
	                {
	                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectHead.UpMove();
	                }
	                break;
	            case EShoppingPage.FashionPage2:
	                if (SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectUpper != null)
	                {
	                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectUpper.UpMove();
	                }
	                break;
	            case EShoppingPage.FashionPage3:

	                if (SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectLower != null)
	                {
	                    {
	                        SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectLower.UpMove();
	                    }
	                }
	                break;
	            case EShoppingPage.FashionPage4:
	                if (SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectAppendage != null)
	                {
	                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectAppendage.UpMove();
	                }
	                break;

            }
        }

	    public void Close()
        {
            _cachedView.gameObject.SetActive(false);
        }
	    /// <summary>
        /// 枚举分页
        /// </summary>
        public enum EShoppingPage
        {
            FashionPage1,
            FashionPage2,
            FashionPage3,
            FashionPage4,
            FashionPage5,
        }
        /// <summary>
        /// 根据字符串返回枚举
        /// </summary>
        /// <param name="USUI"></param>
	    public void USUIShopping(EShoppingPage USUI)
	    {
	        switch (USUI)
	        {
	            case EShoppingPage.FashionPage1:
	                break;
	            case EShoppingPage.FashionPage2:
	                break;
	            case EShoppingPage.FashionPage3:
	                break;
	            case EShoppingPage.FashionPage4:
	                break;
                case EShoppingPage.FashionPage5:
                    break;
            }
	    }
        private List<UMCtrlFashionShopCard> _cardList = new List<UMCtrlFashionShopCard>();

        public void Set(List<Table_FashionUnit> pageList, EResScenary resScenary)
        {
            if (_cardList.Count > 0)
            {
                for (int i = _cardList.Count - 1; i >= 0; i--)
                {
                    _cardList[i].Destroy();
                }
            }
            _cardList.Clear();
            for (int i = 0; i < pageList.Count; i++)
            {
                var UM = new UMCtrlFashionShopCard();
                UM.Init(_cachedView.Dock as RectTransform, resScenary);
                UM.Set(pageList[i]);
                
                _cardList.Add(UM);
            }

        }

    }
}
