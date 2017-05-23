
using System;
using System.Collections;
using System.Collections.Generic;
using SoyEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
	public class USCtrlFashionShop : USCtrlBase <USViewFashionShop>
	{
        //private UMCtrlFashionShopCard _curSelectedPrivateCard;
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

        private List<UMCtrlFashionShopCard> _cardList = new List<UMCtrlFashionShopCard>();
   
        public void Set(List<ShopItem> pageList)
        {
            if (_cardList.Count > 0)
            {
                for (int i = _cardList.Count-1; i >= 0; i--)
                {
                    _cardList[i].DestoryUmCard();
                }
            }
            _cardList.Clear();
                for (int i = 0; i < pageList.Count; i++)
                {
                    var UM = new UMCtrlFashionShopCard();
                    UM.Init(_cachedView.Dock as RectTransform);
                    UM.Set(pageList[i]);

                    _cardList.Add(UM);
                }
            


        }

    }
}
