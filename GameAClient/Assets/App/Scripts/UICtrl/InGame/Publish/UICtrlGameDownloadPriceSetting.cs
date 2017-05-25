  /********************************************************************
  ** Filename : UICtrlGameDownloadPriceSetting.cs
  ** Author : quan
  ** Date : 11/12/2016 10:00 PM
  ** Summary : UICtrlGameDownloadPriceSetting.cs
  ***********************************************************************/


using SoyEngine;
using System;
using UnityEngine;
using SoyEngine.Proto;
using SoyEngine;
using UnityEngine.UI;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup(EUIAutoSetupType.Add)]
    public class UICtrlGameDownloadPriceSetting : UICtrlInGameBase<UIViewGameDownloadPriceSetting>
    {
        #region 常量与字段
        private int _price;
        #endregion

        #region 属性

        #endregion

        #region 方法

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.FreeBtn.onClick.AddListener(OnFreeBtnClick);
            _cachedView.ForbidBtn.onClick.AddListener(OnForbidBtnClick);
            _cachedView.PaidBtn.onClick.AddListener(OnPaidBtnClick);
            for(int i=0; i<_cachedView.PriceBtnAry.Length; i++)
            {
                int inx = i;
                _cachedView.PriceBtnAry[inx].onClick.AddListener(()=>OnPriceBtnClick(inx));
                _cachedView.PriceBtnAry[inx].GetComponentInChildren<Text>().text = ""+_cachedView.PriceAry[inx];
            }
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if(parameter == null)
            {
                LogHelper.Error("UICtrlGameDownloadPriceSetting OnOpen, parameter is null");
            }
            else
            {
                _price = (int)parameter;
                RefreshFlag();
            }
        }


        private void RefreshFlag()
        {
            DisableAllFlag();
            if(_price < 0)
            {
                _cachedView.ForbidFlag.enabled = true;
            }
            else if(_price == 0)
            {
                _cachedView.FreeFlag.enabled = true;
            }
            else
            {
                _cachedView.PaidFlag.enabled = true;
                for(int i=0; i<_cachedView.PriceAry.Length; i++)
                {
                    if(_cachedView.PriceAry[i] == _price)
                    {
                        _cachedView.PriceFlagAry[i].enabled = true;
                        break;
                    }
                }
            }
        }

        private void DisableAllFlag()
        {
            _cachedView.FreeFlag.enabled = false;
            _cachedView.ForbidFlag.enabled = false;
            _cachedView.PaidFlag.enabled = false;
            for(int i=0; i<_cachedView.PriceFlagAry.Length; i++)
            {
                _cachedView.PriceFlagAry[i].enabled = false;
            }
        }

        private void OnFreeBtnClick()
        {
            ConfirmPrice(0);
        }

        private void OnForbidBtnClick()
        {
            ConfirmPrice(-1);
        }

        private void OnPaidBtnClick()
        {
            if(_price > 0)
            {
                return;
            }
            DisableAllFlag();
            _cachedView.PaidFlag.enabled = true;
        }

        private void OnPriceBtnClick(int inx)
        {
            ConfirmPrice(_cachedView.PriceAry[inx]);
        }

        private void ConfirmPrice(int price)
        {
//            GM2DGUIManager.Instance.GetUI<UICtrlPublish>().DownloadPrice = price;
            SocialGUIManager.Instance.CloseUI<UICtrlGameDownloadPriceSetting>();
        }

        #endregion
    }
}