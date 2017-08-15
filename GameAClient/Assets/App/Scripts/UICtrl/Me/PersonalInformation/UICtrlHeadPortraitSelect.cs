using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;
using GameA.Game;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlHeadPortraitSelect : UICtrlInGameBase<UIViewHeadPortraitSelect>
    {
        private int _seletctedHeadImage;

        protected override void InitGroupId ()
        {
            _groupId = (int)EUIGroupType.InGamePopup;
        }

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            //    public Button Exit;
            //public Button AddFriend;
            //public Button Modification;
            //public Button SelectPhoto;
            InitTagGroup();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
        }


        private void OnCloseBtn ()
        {
            SocialGUIManager.Instance.CloseUI <UICtrlHeadPortraitSelect>();
        }

        private void OnOKBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlPersonalInformation>().SetHead(SpriteNameDefine.GetHeadImage(_seletctedHeadImage));
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPortraitSelect>();
        }




        private void InitTagGroup()
        {
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait1, OnHead1Seleted);
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait2, OnHead2Seleted);
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait3, OnHead3Seleted);
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait4, OnHead4Seleted);
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait5, OnHead5Seleted);
            _cachedView.TagGroup.AddButton(_cachedView.HeadPortrait6, OnHead6Seleted);

            //_usctrlFashionPage1 = new USCtrlFashionShop();
            //_usctrlFashionPage1.Init(_cachedView.FashionPage1);
            //_usctrlFashionPage1.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage1;

            //_usctrlFashionPage2 = new USCtrlFashionShop();
            //_usctrlFashionPage2.Init(_cachedView.FashionPage2);
            //_usctrlFashionPage2.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage2;

            //_usctrlFashionPage3 = new USCtrlFashionShop();
            //_usctrlFashionPage3.Init(_cachedView.FashionPage3);
            //_usctrlFashionPage3.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage3;

            //_usctrlFashionPage4 = new USCtrlFashionShop();
            //_usctrlFashionPage4.Init(_cachedView.FashionPage4);
            //_usctrlFashionPage4.ShoppingPage = USCtrlFashionShop.EShoppingPage.FashionPage4;

            //_cachedView.CloseBtn.onClick.AddListener(OnCloseBtnClick);
            //_cachedView.RestoreFashionBtn.onClick.AddListener(OnRestoreFashionBtnClick);
            //_cachedView.PurchaseAllFittingFashionBtn.onClick.AddListener(OnPurchaseAllFittingFashionBtnClick);
        }

        private void OnHead1Seleted(bool open)
        {
            _cachedView.SeletctedHead1Image.SetActiveEx(open);
            _seletctedHeadImage = 1;
        }

        private void OnHead2Seleted(bool open)
        {
            _cachedView.SeletctedHead2Image.SetActiveEx(open);
            _seletctedHeadImage = 2;
        }

        private void OnHead3Seleted(bool open)
        {
            _cachedView.SeletctedHead3Image.SetActiveEx(open);
            _seletctedHeadImage = 3;
        }

        private void OnHead4Seleted(bool open)
        {
            _cachedView.SeletctedHead4Image.SetActiveEx(open);
            _seletctedHeadImage = 4;
        }

        private void OnHead5Seleted(bool open)
        {
            _cachedView.SeletctedHead5Image.SetActiveEx(open);
            _seletctedHeadImage = 5;
        }

        private void OnHead6Seleted(bool open)
        {
            _cachedView.SeletctedHead6Image.SetActiveEx(open);
            _seletctedHeadImage = 6;
        }
    }
}