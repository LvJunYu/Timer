using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;
using System;



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
        _cachedView.IsOccupied.text = JudgeItemOccupied(listItem) ? "此时装已装备" : "未装备";
        _cachedView.IsOwned.text = JudgeItemOwned(listItem) ? "此时装已拥有" : "未拥有";
        _cachedView.BuyFashion.onClick.AddListener(() =>
              BuyFashion(listItem));
        _cachedView.TryFashionOn.onClick.AddListener(() =>SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().
              TryFashionOn(listItem));
        _cachedView.ChangeFashion.onClick.AddListener(() =>
              ChangeFashion(listItem));
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
    private void BuyFashion(ShopItem listItem)
    {
        if (JudgeItemOwned(listItem))
        {
            _cachedView.Message.text = "您已经拥有无需再次购买";
        }
        else
        {
            if (listItem.PriceGoldDay <= LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin)
            {
                //默认先写的一天的价格 后续再根据选了几天的 对应传入 EBuyAvatarPartDurationType  ECurrencyType 以及  discountCouponId 现在为 0
                   BuyAvatarPart(
                   listItem._avatarType,
                   listItem.Id,
                   EBuyAvatarPartDurationType.BAPDT_1,
                   ECurrencyType.CT_Gold,
                   0,
                   () => {
                       LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
                       {
                           SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
                           _cachedView.Message.text = "购买成功";
                       }, code =>
                       {
                           LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);
                       });
                       
                   }, () =>
                   {
                       _cachedView.Message.text = "购买失败";
                   }
              );
            }
            else
            {
                _cachedView.Message.text = "金币/钻石不足";
            }
        }
        

    }

    public void BuyAvatarPart(EAvatarPart partType,
                                long partId,
                                EBuyAvatarPartDurationType durationType,
                                ECurrencyType currencyType,
                                long discountCouponId,
                                Action successCallback, Action failedCallback)
    {
        RemoteCommands.BuyAvatarPart(partType, partId, durationType, currencyType, discountCouponId,
                (Msg_SC_CMD_BuyAvatarPart ret) =>
                {
                    if (ret.ResultCode == (int)EBuyAvatarPartCode.BAPC_Success)
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

                },
                (ENetResultCode ret) =>
                {
                    _cachedView.Message.text = "购买失败";
                }
        );
    }




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
        bool rst = false;
        switch (listItem._avatarType)
        {
            case EAvatarPart.AP_Head:
                if (LocalUser.Instance.ValidAvatarData.ItemHead!=null)
                {
                    rst = LocalUser.Instance.ValidAvatarData.ItemHead.ContainsKey(listItem.Id);
                }
                break;
            case EAvatarPart.AP_Lower:
                if (LocalUser.Instance.ValidAvatarData.ItemLower != null)
                {
                    rst = LocalUser.Instance.ValidAvatarData.ItemLower.ContainsKey(listItem.Id);
                }
                break;
            case EAvatarPart.AP_Upper:
                if (LocalUser.Instance.ValidAvatarData.ItemUpper != null)
                {
                    rst = LocalUser.Instance.ValidAvatarData.ItemUpper.ContainsKey((int)listItem.Id);
                }
                break;
            case EAvatarPart.AP_Appendage:
                if (LocalUser.Instance.ValidAvatarData.ItemAppendage != null)
                {
                    rst = LocalUser.Instance.ValidAvatarData.ItemAppendage.ContainsKey(listItem.Id);
                }
                break;
            default:
                break;
        }
        return rst;

    }

    private void ChangeFashion(ShopItem listItem)
    {
        if (JudgeItemOwned(listItem))
        {
            if (JudgeItemOccupied(listItem))
            {
                _cachedView.Message.text = "正在使用";
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
                        SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
                        _cachedView.Message.text = "换装成功";
                    }, code =>
                    {
                        LogHelper.Error("Network error when get UsingAvatarData, {0}", code);
                    });
                    
                },
               () =>
               {
                   _cachedView.Message.text = "换装失败";
               });
            }

        }
        else { _cachedView.Message.text = "未拥有请购买"; }

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




}