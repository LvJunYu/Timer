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
    public class UICtrlShopingCart : UICtrlGenericBase<UIViewShopingCart>
    {
        #region 常量与字段
        /// <summary>
        /// 保留um索引
        /// </summary>
        public List<UMCtrlCartCard> _cardList = new List<UMCtrlCartCard>();

        //private Dictionary<EAvatarPart, Msg_BuyAvatarPartItem> AvatarmsgDic = new Dictionary<EAvatarPart, Msg_BuyAvatarPartItem>();
        //private Dictionary<EAvatarPart, ShopItem> ShopItemDic = new Dictionary<EAvatarPart, ShopItem>();

        /// <summary>
        /// 购买列表
        /// </summary>
        public Dictionary<ShopItem, Msg_BuyAvatarPartItem> AvatarmsgDic = new Dictionary<ShopItem, Msg_BuyAvatarPartItem>();
        private List<Msg_BuyAvatarPartItem> _avatarmsg = new List<Msg_BuyAvatarPartItem>();

        #endregion
        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 刷新页面
        /// </summary>
        public void RefreshPage()
        {
            _cachedView.PriceGold.text = CalculateGoldCount().ToString();
            _cachedView.PriceDiamond.text = CalculateDiamondCount().ToString();
        }

        /// <summary>
        /// 计算当前时装购买需金币数量
        /// </summary>
        /// <returns></returns>
        private int CalculateGoldCount() 
        {
            int goldCount = 0;
            
            foreach (var item in AvatarmsgDic)
            {
                switch (item.Value.DurationType)
                {
                    case EBuyAvatarPartDurationType.BAPDT_1:
                         goldCount += item.Key.PriceGoldDay;
                         break;
                    case EBuyAvatarPartDurationType.BAPDT_7:
                         goldCount += item.Key.PriceGoldWeek;
                         break;
                    case EBuyAvatarPartDurationType.BAPDT_30:
                         goldCount += item.Key.PriceGoldMonth;
                         break;
                    case EBuyAvatarPartDurationType.BAPDT_Inf:
                         goldCount += item.Key.PriceGoldPermanent;
                         break;
                    default:
                        break;
                }
            }
            return goldCount;
        }
        /// <summary>
        /// 计算当前时装购买需钻石数量
        /// </summary>
        /// <returns></returns>
        private int CalculateDiamondCount()
        {
            int diamondCount = 0;

            foreach (var item in AvatarmsgDic)
            {
                switch (item.Value.DurationType)
                {
                    case EBuyAvatarPartDurationType.BAPDT_1:
                         diamondCount += item.Key.PriceDiamondDay;
                        break;
                    case EBuyAvatarPartDurationType.BAPDT_7:
                         diamondCount += item.Key.PriceDiamondWeek;
                        break;
                    case EBuyAvatarPartDurationType.BAPDT_30:
                         diamondCount += item.Key.PriceDiamondMonth;
                        break;
                    case EBuyAvatarPartDurationType.BAPDT_Inf:
                         diamondCount += item.Key.PriceDiamondPermanent;
                        break;
                    default:
                        break;
                }
            }
            return diamondCount;
        }


        private void SetAvatarMsgList()
        {
            foreach (var item in AvatarmsgDic)
            {
                _avatarmsg.Add(item.Value);
            }
        }

        public void AddToAvatarmsgDic(ShopItem type, Msg_BuyAvatarPartItem avatarmsg)
        {
            // 如果已经存在key 就更改value, 否则重新生成一个 key value 对。


            if (AvatarmsgDic.ContainsKey(type))
            {
                AvatarmsgDic[type] = avatarmsg;
            }
            else
            {
                AvatarmsgDic.Add(type, avatarmsg);
            }
        }      public void DelFromAvatarmsgDic(ShopItem type)
        {
            // 如果已经存在key 就更改value, 否则重新生成一个 key value 对。
            if (AvatarmsgDic.ContainsKey(type))
            {
                AvatarmsgDic.Remove(type);
            }
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            int a = 1;
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


        //private int CalculateTotalList(List<ShopItem> fittingFashionList,string countType)
        //{
        //    int totalValue = 0;
        //    for (int i = 0; i < fittingFashionList.Count; i++)
        //    {
        //        //fittingFashionList.pr
        //    }
        //    return  totalValue;
        //}

        /// <summary>
        /// 一键购买时制作um列表
        /// </summary>
        /// <param name="fittingFashionList"></param>
        public void Set(List<ShopItem> fittingFashionList)
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            //_cachedView.PriceGold.text = fittingFashion.PriceGoldDay.ToString();
            _cardList.Clear();
            for (int i = 0; i < fittingFashionList.Count; i++)
            {
                SetEachCard(fittingFashionList[i]);
            }
            RefreshPage();
        }

        public void Set(ShopItem fittingFashion)
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            _cardList.Clear();
            SetEachCard(fittingFashion);
            RefreshPage();
        }
        private void SetEachCard(ShopItem fittingFashion)
        {
            if (_cachedView != null)
            {        
                var UM = new UMCtrlCartCard();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(fittingFashion);
                _cardList.Add(UM);
            }
        }
        /// <summary>
        /// 创建
        /// </summary>
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            // InitTagGroup();
            InitBtn();
            InitGroupId();
        }

        private void InitBtn()
        {
            
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            _cachedView.PurchaseByGold.onClick.AddListener(()=> BuyFashion(ECurrencyType.CT_Gold));
            _cachedView.PurchaseByDiamond.onClick.AddListener(() => BuyFashion(ECurrencyType.CT_Diamond));

        }



        #region 接口
        protected override void InitGroupId()
        {
            _groupId = (int)EUIGroupType.MainUI;
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
            //CardList.Clear();
            SocialGUIManager.Instance.CloseUI<UICtrlShopingCart>();
            AvatarmsgDic.Clear();
            _avatarmsg.Clear();
        }

        private bool JudgeWhetherThereIsEnoughCurrency(ECurrencyType buyType)
        {
            bool ret = false;
            if (buyType == ECurrencyType.CT_Gold)
            {
                if (GameATools.CheckGold(CalculateGoldCount()))
                {
                    return ret = true;
                }
            }
            else if (buyType == ECurrencyType.CT_Diamond)
            {
                if (GameATools.CheckGold(CalculateDiamondCount()))
                {
                    return ret = true;
                }
            }
            return ret;

        }

        private void BuyFashion(ECurrencyType buyType)
        {
            SetAvatarMsgList();
            if (_avatarmsg.Count == 0)
            {
                SocialGUIManager.ShowPopupDialog("请至少选择一种时装", null,
                    new KeyValuePair<string, Action>("确定", () => {

                    })
                    );
            }

            else
            {
                if (JudgeWhetherThereIsEnoughCurrency(buyType))
                {
                    foreach (var item in _avatarmsg)
                    {
                        item.CurrencyType = buyType;
                    }
                    BuyAvatarPart(
                        _avatarmsg,
                        () =>
                        {
                            SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().Close();
                            SocialGUIManager.Instance.OpenUI<UICtrlFashionShopMainMenu>();

                            SocialGUIManager.ShowPopupDialog("购买成功", null,
                                new KeyValuePair<string, Action>("确定", () => { }));
                            //Debug.Log("______________购买成功");
                            OnCloseBtnClick();
                            LocalUser.Instance.ValidAvatarData.Request(LocalUser.Instance.UserGuid, () =>
                            {
                                //Set(listItem);
                                SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();

                            }, code =>
                            {
                                SocialGUIManager.ShowPopupDialog("刷新已拥有时装失败", null,
                                new KeyValuePair<string, Action>("确定", () => { }));
                                LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);

                            });
                            LocalUser.Instance.UsingAvatarData.Request(LocalUser.Instance.UserGuid, () =>
                            {
                                //Set(listItem);
                                SocialGUIManager.Instance.GetUI<UICtrlFashionShopMainMenu>().RefreshFashionShopPanel();

                            }, code =>
                            {
                                SocialGUIManager.ShowPopupDialog("刷新正使用时装失败", null,
                                new KeyValuePair<string, Action>("确定", () => { }));
                                LogHelper.Error("Network error when get BuyAvatarPart, {0}", code);

                            });

                        },
                        () =>
                        {
                            SocialGUIManager.ShowPopupDialog("购买失败", null,
                                new KeyValuePair<string, Action>("确定", () => { }));
                            //Debug.Log("______________购买失败");
                        }
                        );
                }
                else
                {
                    SocialGUIManager.ShowPopupDialog("货币不足", null,
                               new KeyValuePair<string, Action>("确定", () => { }));
                    //_cachedView.Message.text = "金币/钻石不足";
                }
            }
     
        }

        public void BuyAvatarPart(List<Msg_BuyAvatarPartItem> buyList,Action successCallback, Action failedCallback)
        {
            //bool putOn,
            //Action< Msg_SC_CMD_BuyAvatarPart > successCallback, Action<ENetResultCode> failedCallback,
            RemoteCommands.BuyAvatarPart(buyList, true,
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
                      //_cachedView.Message.text = "购买失败";
                  }
          );
        }
        #endregion 接口
        #endregion
    }
}
