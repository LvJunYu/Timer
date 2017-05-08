using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;

public class UMCtrlFashionShopCard : UMCtrlBase<UMViewFashionShopCard>
{
    //private EAvatarPart _Type;

    public void Set(ShopItem listItem)
    {
        //Debug.Log(listItem.Name);
        //Debug.Log(_cachedView);
        _cachedView.Name.text = listItem.Name;
        _cachedView.PriceGoldDay.text = listItem.PriceGoldDay.ToString();
        _cachedView.PreviewTexture.text = listItem.PreviewTexture;
        _cachedView.TryFashionOn.onClick.AddListener(()=> 
            SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().TryFashionOn(listItem._avatarType, listItem.PreviewTexture));

        //_cachedView._avatarType = listItem._avatarType;
        //_cachedView.Id = listItem.Id;
        //_cachedView.Description = listItem.Description;
        //_cachedView.Sex = listItem.Sex;
        //_cachedView.Character = listItem.Character;
        //_cachedView.PriceGoldMonth = listItem.PriceGoldMonth;
        //_cachedView.PriceGoldPermanent = listItem.PriceGoldPermanent;
        //_cachedView.PriceDiamondDay = listItem.PriceDiamondDay;
        //_cachedView.PriceDiamondWeek = listItem.PriceDiamondWeek;
        //_cachedView.PriceDiamondMonth = listItem.PriceDiamondMonth;
        //_cachedView.PriceDiamondPermanent = listItem.PriceDiamondPermanent;
        //_cachedView.BigTexture = listItem.BigTexture;
        //_cachedView.SmallTexture = listItem.SmallTexture;
        //_cachedView.SkinId = listItem.SkinId;
    }







}
