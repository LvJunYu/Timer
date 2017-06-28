﻿using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;
using NewResourceSolution;
using System;
using DG.Tweening;

namespace GameA
{

    public class UMCtrlFashionShopCard : UMCtrlBase<UMViewFashionShopCard>
    {
        private string _cardName;
        private ShopItem _itemInfo;
        private int _itemID;
        private Tween _tweener;
        


        public string CardName
        {
            set { _cardName = value; }
            get { return _cardName; }
        }
        public ShopItem ItemInfo
        {
            set { _itemInfo = value; }
            get { return _itemInfo; }
        }
        public int ItemID
        {
            set { _itemID = value; }
            get { return _itemID; }
        }
        public void Set(ShopItem listItem)
        {
            //Debug.Log(listItem.Name);
            //Debug.Log(_cachedView);
            ChangeBySex(listItem);
            _itemInfo = listItem;
            _itemID = listItem.Id;
            _cachedView.Name.text = listItem.Name;
            _cachedView.PriceGoldDay.text = listItem.PriceGoldDay.ToString();
            _cachedView.PriceDiamondDay.text = listItem.PriceDiamondDay.ToString();
            _itemInfo = listItem;
            _cachedView.PreviewTexture.text = listItem.PreviewTexture;
            _cachedView.Message.text = GetTime(listItem).ToString();

            _cachedView.PreviewBtn.onClick.AddListener(() =>
            {
                UpMove();
                FashionOnClick(listItem);
            });
            Sprite fashion = null;
            //Debug.Log("____________预览图" + listItem.PreviewTexture);

            if (ResourcesManager.Instance.TryGetSprite(listItem.PreviewTexture, out fashion))
            {
                //Debug.Log("____________时装" + fashion.name);

                _cachedView.FashionPreview.sprite = fashion;
            }
            _cachedView.IsOccupied.text = JudgeItemOccupied(listItem) ? "此时装已装备" : "未装备";
            _cachedView.IsOwned.text = JudgeItemOwned(listItem) ? "此时装已拥有" : "未拥有";
            //Debug.Log("______________________________Set UMCtrlFashionShopCard " + listItem.Id + " name:" +
            //          listItem.Name +
            //          " " + _cachedView.IsOccupied.text +
            //          " " + _cachedView.IsOwned.text);
            _cachedView.BuyFashion.onClick.AddListener(() =>
            {
                //BuyFashion(listItem);
                SocialGUIManager.Instance.OpenUI<UICtrlShopingCart>();
                SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().Set(listItem);
                //Debug.Log("购买:" + listItem.Name);
            }



                );
            //_cachedView.TryFashionOn.onClick.AddListener(() =>
            //{
            //    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().TryFashionOn(listItem);
            //    SetFittingFashion(listItem);
            //}
            //    );
            //_cachedView.FashionBtn.onClick.AddListener(() =>
            //{
            //    ChangeFashion(listItem);
            //    SetFittingFashion(listItem);
            //}
            //    );

            if (JudgeItemOwned(listItem))
            {
                _cachedView.BuyFashion.SetActiveEx(false);
            }
            else
            {
                _cachedView.Message.SetActiveEx(false);

            }
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

        private void ChangeBySex(ShopItem listItem)
        {
            Sprite Bg=null;
            if (listItem.Sex == 2)
            {
                if (ResourcesManager.Instance.TryGetSprite("card_pink", out Bg))
                {
                    _cachedView.SexBg.sprite = Bg;
                }
                if (ResourcesManager.Instance.TryGetSprite("img_pink", out Bg))
                {
                    _cachedView.Sexbottom.sprite = Bg;
                }
                if (ResourcesManager.Instance.TryGetSprite("light_pink", out Bg))
                {
                    _cachedView.SexLight.sprite = Bg;
                }
                if (ResourcesManager.Instance.TryGetSprite("name_bg_pink", out Bg))
                {
                    _cachedView.SexTitle.sprite = Bg;
                }
            }
        }

        private void SetFittingFashion(ShopItem listItem)
        {
            switch (listItem._avatarType)
            {
                case EAvatarPart.AP_Head:
                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectHead = this;
                    break;
                case EAvatarPart.AP_Lower:
                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectLower = this;
                    break;
                case EAvatarPart.AP_Upper:
                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectUpper = this;
                    break;
                case EAvatarPart.AP_Appendage:
                    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().SelectAppendage = this;
                    break;
                default:
                    break;
            }
        }

        private void FashionOnClick(ShopItem listItem)
        {
            if (JudgeItemOccupied(listItem))
            {
                //_cachedView.Message.text = "已穿戴";
                //SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().TryOnAvatar(listItem);
            }
            else if (JudgeItemOwned(listItem))
            {
                //_cachedView.Message.text = "已经拥有并穿戴";
                // ChangeFashion(listItem);
                SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().TryOnAvatar(listItem);
                SetFittingFashion(listItem);
            }
            else
            {
                //_cachedView.Message.text = "在试穿";
               
            }
            SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().TryOnAvatar(listItem);
            SetFittingFashion(listItem);
        }


        private string GetTime(ShopItem listItem)
        {
 
            long now= DateTimeUtil.GetServerTimeNowTimestampMillis();
            long time=0;
            if (JudgeItemOwned(listItem))
            {
                switch (listItem._avatarType)
                {
                    case EAvatarPart.AP_Head:
                        time =  LocalUser.Instance.ValidAvatarData.GetItemInHeadDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Lower:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInLowerDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Upper:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInUpperDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Appendage:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInAppendageDictionary(listItem.Id).ExpirationTime;
                        break;
                    default:
                        break;
                }

                //LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
                //{
                //    Set(listItem);
                //    //SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
                //    _cachedView.Message.text = "购买成功";
                //}, code => { LogHelper.Error("Network error when get BuyAvatarPart, {0}", code); });

            }

           return formatTime(time-now);
        }

        public  String formatTime(long ms)
        {

            int ss = 1000;
            int mi = ss * 60;
            int hh = mi * 60;
            int dd = hh * 24;

            long day = ms / dd;
            long hour = (ms - day * dd) / hh;
            long minute = (ms - day * dd - hour * hh) / mi;
            long second = (ms - day * dd - hour * hh - minute * mi) / ss;
            long milliSecond = ms - day * dd - hour * hh - minute * mi - second * ss;

            String strDay = "" + day; //天  
            String strHour ="" + hour;//小时  
            String strMinute = "" + minute;//分钟  
            String strSecond = second < 10 ? "0" + second : "" + second;//秒  
            String strMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond;//毫秒  
            strMilliSecond = milliSecond < 100 ? "0" + strMilliSecond : "" + strMilliSecond;

            if (day < 0 || hour < 0 || minute < 0)
            return "已过期";
            else if (day >= 31)
            return "永久";
            else
            return "有效期："+ "\n" + strDay + " 天  " + strHour + " 小时 " + strMinute + " 分钟 " ;
        }


        //private void BuyFashion(ShopItem listItem)
        //{
        //    if (JudgeItemOwned(listItem))
        //    {
        //        _cachedView.Message.text = "您已经拥有无需再次购买";
        //    }
        //    else
        //    {
        //        if (listItem.PriceGoldDay <= LocalUser.Instance.User.UserInfoSimple.LevelData.GoldCoin)
        //        {
        //            //默认先写的一天的价格 后续再根据选了几天的 对应传入 EBuyAvatarPartDurationType  ECurrencyType 以及  discountCouponId 现在为 0
        //            BuyAvatarPart(
        //                listItem._avatarType,
        //                listItem.Id,
        //                EBuyAvatarPartDurationType.BAPDT_1,
        //                ECurrencyType.CT_Gold,
        //                0,
        //                () =>
        //                {
        //                    LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
        //                    {
        //                        Set(listItem);
        //                        //SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
        //                        _cachedView.Message.text = "购买成功";
        //                    }, code =>
        //                    {
        //                        LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);
        //                    });

        //                }, () =>
        //                {
        //                    _cachedView.Message.text = "购买失败";
        //                }
        //                );
        //        }
        //        else
        //        {
        //            _cachedView.Message.text = "金币/钻石不足";
        //        }
        //    }


        //}

        //public void BuyAvatarPart(EAvatarPart partType,
        //    long partId,
        //    EBuyAvatarPartDurationType durationType,
        //    ECurrencyType currencyType,
        //    long discountCouponId,
        //    Action successCallback, Action failedCallback)
        //{
        //    Msg_BuyAvatarPartItem msg = new Msg_BuyAvatarPartItem();
        //    msg.PartType = partType;
        //    msg.PartId = partId;
        //    msg.CurrencyType = currencyType;
        //    msg.DiscountCouponId = discountCouponId;
        //    msg.DurationType = durationType;
        //    List<Msg_BuyAvatarPartItem> buyList = new List<Msg_BuyAvatarPartItem>();
        //    buyList.Add(msg);
        //    //bool putOn,
        //    //Action< Msg_SC_CMD_BuyAvatarPart > successCallback, Action<ENetResultCode> failedCallback,
        //      RemoteCommands.BuyAvatarPart(buyList,true,
        //            (Msg_SC_CMD_BuyAvatarPart ret) =>
        //            {
        //                if (ret.ResultCode == (int)EBuyAvatarPartCode.BAPC_Success)
        //                {

        //                    if (null != successCallback)
        //                    {
        //                        successCallback.Invoke();
        //                    }
        //                }
        //                else
        //                {
        //                    if (null != failedCallback)
        //                    {
        //                        failedCallback.Invoke();
        //                    }
        //                }

        //            },
        //            (ENetResultCode ret) =>
        //            {
        //                _cachedView.Message.text = "购买失败";
        //            }
        //    );
        //}


        public void UpMove()
        {
            if(_cachedView!=null)
            _cachedView.GetComponent<RectTransform>().DOLocalMoveY(12, 0.4f, false);
        }

        public void DownMove()
        {
            if (_cachedView != null)
                _cachedView.GetComponent<RectTransform>().DOLocalMoveY(-1, 0.4f, false);
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
            //LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
            //{
            //    //Set(listItem);
            //    SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();

            //}, code =>
            //{
            //    SocialGUIManager.ShowPopupDialog("刷新已拥有时装失败", null,
            //    new KeyValuePair<string, Action>("确定", () => { }));
            //    LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);
            //});
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

        //private void ChangeFashion(ShopItem listItem)
        //{
        //    if (JudgeItemOwned(listItem))
        //    {
        //        if (JudgeItemOccupied(listItem))
        //        {
        //            _cachedView.Message.text = "正在使用";
        //        }
        //        else
        //        {
        //            ChangeAvatarPart(
        //                listItem._avatarType,
        //                listItem.Id,
        //                () =>
        //                {
        //                    LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid, () =>
        //                    {
        //                        Set(listItem);
        //                        //SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();
        //                        _cachedView.Message.text = "换装成功";
        //                    }, code =>
        //                    {
        //                        LogHelper.Error("Network error when get UsingAvatarData, {0}", code);
        //                    });

        //                },
        //                () =>
        //                {
        //                    _cachedView.Message.text = "换装失败";
        //                });
        //        }

        //    }
        //    else
        //    {
        //        _cachedView.Message.text = "未拥有请购买";
        //    }

        //}

        //public void ChangeAvatarPart(
        //    EAvatarPart type,
        //    long partID,
        //    Action successCallback, Action failedCallback)
        //{
        //    RemoteCommands.ChangeAvatarPart(type, partID, (ret) =>
        //    {
        //        if (ret.ResultCode == (int)EChangeAvatarPartCode.CAPC_Success)
        //        {

        //            if (null != successCallback)
        //            {
        //                successCallback.Invoke();
        //            }
        //        }
        //        else
        //        {
        //            if (null != failedCallback)
        //            {
        //                failedCallback.Invoke();
        //            }
        //        }
        //    }, (errorCode) =>
        //    {
        //        if (null != failedCallback)
        //        {
        //            failedCallback.Invoke();
        //        }
        //    });
        //}

        //public void ChangeDock(bool selected)
        //{
        //    if (selected)
        //    {
        //        Init(_cachedView.DockSelected as RectTransform);
        //    }
        //    else
        //    {
        //        Init(_cachedView.DockUnSelected as RectTransform);
        //    }
        //}

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void DestoryUmCard()
        {
            OnDestroy();
        }




    }
}