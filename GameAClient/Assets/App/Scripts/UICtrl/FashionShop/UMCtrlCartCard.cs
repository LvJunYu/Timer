using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;
using System;
using GameA.Game;
using NewResourceSolution;

namespace GameA
{

    public class UMCtrlCartCard : UMCtrlBase<UMViewCartCard>
    {
        private EBuyAvatarPartDurationType _durationType;
        //private bool _ifChecked;
        private Table_FashionUnit _itemInfo;
        private Msg_BuyAvatarPartItem _msg = new Msg_BuyAvatarPartItem();

        private void SetMsgToDic()
        {
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().AddToAvatarmsgDic(_itemInfo, _msg);
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().RefreshPage();
        }

        private void DelMsgFromDic()
        {
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().DelFromAvatarmsgDic(_itemInfo);
            SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().RefreshPage();
        }


        public void Set(Table_FashionUnit listItem)
        {
            _msg.PartType = (EAvatarPart)listItem.Type;
            _msg.PartId = listItem.Id;
            _msg.CurrencyType = ECurrencyType.CT_Gold;
            _msg.DiscountCouponId = 0;
            _msg.DurationType = EBuyAvatarPartDurationType.BAPDT_1;
            _itemInfo = listItem;
            SetMsgToDic();
            _cachedView.Name.text = listItem.Name;
            _cachedView.PriceGold.text = listItem.PriceGoldDay.ToString();
            _cachedView.PriceDiamond.text = listItem.PriceDiamondDay.ToString();
            _cachedView.TimeElect.onValueChanged.AddListener(Timeselect);
            _cachedView.PitchOn.onValueChanged.AddListener(Checked);
            Sprite fashion=null;
            if (ResourcesManager.Instance.TryGetSprite(listItem.PreviewTexture, out fashion))
            {
                _cachedView.FashionPreview.sprite = fashion;
            }
        }

        private void Timeselect(int timeType)
        {
            switch (timeType)
            {
                case 0:
                {
                    _msg.DurationType = EBuyAvatarPartDurationType.BAPDT_1;
                
                        _cachedView.PriceGold.text = _itemInfo.PriceGoldDay.ToString();
                        _cachedView.PriceDiamond.text = _itemInfo.PriceDiamondDay.ToString();
                    }
                    break;
                case 1:
                {
                    _msg.DurationType = EBuyAvatarPartDurationType.BAPDT_7;
                        _cachedView.PriceGold.text = _itemInfo.PriceGoldWeek.ToString();
                        _cachedView.PriceDiamond.text = _itemInfo.PriceDiamondWeek.ToString();
                    }
                    break;
                case 2:
                {
                    _msg.DurationType = EBuyAvatarPartDurationType.BAPDT_30;
                        _cachedView.PriceGold.text = _itemInfo.PriceGoldMonth.ToString();
                        _cachedView.PriceDiamond.text = _itemInfo.PriceDiamondMonth.ToString();

                    }
                    break;
                case 3:
                {
                    _msg.DurationType = EBuyAvatarPartDurationType.BAPDT_Inf;
                        _cachedView.PriceGold.text = _itemInfo.PriceGoldPermanent.ToString();
                        _cachedView.PriceDiamond.text = _itemInfo.PriceDiamondPermanent.ToString();
                    }
                    break;
            }
            SetMsgToDic();
        }

        private void Checked(bool ifChecked)
        {
            if (!ifChecked)
            {
                DelMsgFromDic();
            }
            else
            {
                SetMsgToDic();
            }
            //_ifChecked = ifChecked;
        }


    }
}
