using System.Collections;
using System.Collections.Generic;
using GameA;
using SoyEngine.Proto;
using UnityEngine;
using SoyEngine;
using System;
using DG.Tweening;
using GameA.Game;

namespace GameA
{
    public class UMCtrlFashionShopCard : UMCtrlBase<UMViewFashionShopCard>
    {
        private string _cardName;
        private Table_FashionUnit _itemInfo;
        private int _itemID;
        private Tween _tweener;

        public string CardName
        {
            set { _cardName = value; }
            get { return _cardName; }
        }

        public Table_FashionUnit ItemInfo
        {
            set { _itemInfo = value; }
            get { return _itemInfo; }
        }

        public int ItemID
        {
            set { _itemID = value; }
            get { return _itemID; }
        }

        public void Set(Table_FashionUnit listItem)
        {
            ChangeBySex(listItem);
            _itemInfo = listItem;
            _itemID = listItem.Id;
            _cachedView.Name.text = listItem.Name;
            _cachedView.PriceGoldDay.text = listItem.PriceGoldDay.ToString();
            _cachedView.PriceDiamondDay.text = listItem.PriceDiamondDay.ToString();
            _itemInfo = listItem;
            _cachedView.PreviewTexture.text = listItem.PreviewTexture;
            _cachedView.Message.text = GetTime(listItem);
            _cachedView.PreviewBtn.onClick.AddListener(() =>
            {
                UpMove();
                FashionOnClick(listItem);
            });
            Sprite fashion = null;
            if (GameResourceManager.Instance.TryGetSpriteByName(listItem.PreviewTexture, out fashion))
            {
                _cachedView.FashionPreview.sprite = fashion;
            }
            _cachedView.BuyFashion.onClick.AddListener(() =>
            {
                SocialGUIManager.Instance.OpenUI<UICtrlShopingCart>();
                SocialGUIManager.Instance.GetUI<UICtrlShopingCart>().Set(listItem);
            }
                );
            if (JudgeItemOwned(listItem))
            {
                _cachedView.BuyFashion.SetActiveEx(false);
            }
            else
            {
                _cachedView.Message.SetActiveEx(false);
            }
        }

        private void ChangeBySex(Table_FashionUnit listItem)
        {
            Sprite Bg = null;
            if (listItem.Sex == 2)
            {
                if (GameResourceManager.Instance.TryGetSpriteByName("card_pink", out Bg))
                {
                    _cachedView.SexBg.sprite = Bg;
                }
                if (GameResourceManager.Instance.TryGetSpriteByName("img_pink", out Bg))
                {
                    _cachedView.Sexbottom.sprite = Bg;
                }
                if (GameResourceManager.Instance.TryGetSpriteByName("light_pink", out Bg))
                {
                    _cachedView.SexLight.sprite = Bg;
                }
                if (GameResourceManager.Instance.TryGetSpriteByName("name_bg_pink", out Bg))
                {
                    _cachedView.SexTitle.sprite = Bg;
                }
            }
        }

        private void SetFittingFashion(Table_FashionUnit listItem)
        {
            switch ((EAvatarPart) listItem.Type)
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

        private void FashionOnClick(Table_FashionUnit listItem)
        {
            if (JudgeItemOccupied(listItem))
            {
                //_cachedView.Message.text = "已穿戴";
            }
            else if (JudgeItemOwned(listItem))
            {
                //_cachedView.Message.text = "已经拥有并穿戴";
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


        private string GetTime(Table_FashionUnit listItem)
        {
            long now = DateTimeUtil.GetServerTimeNowTimestampMillis();
            long time = 0;
            if (JudgeItemOwned(listItem))
            {
                switch ((EAvatarPart) listItem.Type)
                {
                    case EAvatarPart.AP_Head:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInHeadDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Lower:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInLowerDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Upper:
                        time = LocalUser.Instance.ValidAvatarData.GetItemInUpperDictionary(listItem.Id).ExpirationTime;
                        break;
                    case EAvatarPart.AP_Appendage:
                        time =
                            LocalUser.Instance.ValidAvatarData.GetItemInAppendageDictionary(listItem.Id).ExpirationTime;
                        break;
                }
            }

            return formatTime(time - now);
        }

        public String formatTime(long ms)
        {
            int ss = 1000;
            int mi = ss*60;
            int hh = mi*60;
            int dd = hh*24;

            long day = ms/dd;
            long hour = (ms - day*dd)/hh;
            long minute = (ms - day*dd - hour*hh)/mi;
            long second = (ms - day*dd - hour*hh - minute*mi)/ss;
            long milliSecond = ms - day*dd - hour*hh - minute*mi - second*ss;

            String strDay = "" + day; //天  
            String strHour = "" + hour; //小时  
            String strMinute = "" + minute; //分钟  
            String strSecond = second < 10 ? "0" + second : "" + second; //秒  
            String strMilliSecond = milliSecond < 10 ? "0" + milliSecond : "" + milliSecond; //毫秒  
            strMilliSecond = milliSecond < 100 ? "0" + strMilliSecond : "" + strMilliSecond;

            if (day < 0 || hour < 0 || minute < 0)
                return "已过期";
            else if (day >= 31)
                return "永久";
            else
                return "有效期：" + "\n" + strDay + " 天  " + strHour + " 小时 " + strMinute + " 分钟 ";
        }

        public void UpMove()
        {
            if (_cachedView != null)
                _cachedView.GetComponent<RectTransform>().DOLocalMoveY(12, 0.4f, false);
        }

        public void DownMove()
        {
            if (_cachedView != null)
                _cachedView.GetComponent<RectTransform>().DOLocalMoveY(-1, 0.4f, false);
        }


        private bool JudgeItemOccupied(Table_FashionUnit listItem)
        {
            bool rst = false;
            switch ((EAvatarPart) listItem.Type)
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
            }
            return rst;
        }

        private bool JudgeItemOwned(Table_FashionUnit listItem)
        {
            bool rst = false;
            switch ((EAvatarPart) listItem.Type)
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
            }
            return rst;
        }

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