using System;
using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using SoyEngine.Proto;
using GameA.Game;
using UnityEngine.UI;

namespace GameA
{
    [UIAutoSetup (EUIAutoSetupType.Add)]
    public class UICtrlHeadPortraitSelect : UICtrlInGameBase<UIViewHeadPortraitSelect>
    {
        private int _seletctedHeadImage;
        private int _headMaxNumber=6;
        private List<UMCtrlHead> _cardList = new List<UMCtrlHead>();


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
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.OKBtn.onClick.AddListener(OnOKBtn);
        }


        private void OnCloseBtn ()
        {
            SocialGUIManager.Instance.CloseUI <UICtrlHeadPortraitSelect>();
        }

        protected override void OnOpen(object parameter)
        {
            SetHead();
        }

        private void OnOKBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlPersonalInformation>().SetHead(SpriteNameDefine.GetHeadImage(_seletctedHeadImage));
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPortraitSelect>();
        }

        public void SetHead()
        {
            for (int i = 0; i < _headMaxNumber; i++)
            {
                var UM = new UMCtrlHead();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(SpriteNameDefine.GetHeadImage(i));
                _cardList.Add(UM);
            }
        }



        public void InitTagGroup(Button button,Action<bool>function)
        {
            _cachedView.TagGroup.AddButton(button, function);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait2, OnHead2Seleted);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait3, OnHead3Seleted);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait4, OnHead4Seleted);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait5, OnHead5Seleted);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait6, OnHead6Seleted);
        }



        //private void OnHead2Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead2Image.SetActiveEx(open);
        //    _seletctedHeadImage = 2;
        //}

        //private void OnHead3Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead3Image.SetActiveEx(open);
        //    _seletctedHeadImage = 3;
        //}

        //private void OnHead4Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead4Image.SetActiveEx(open);
        //    _seletctedHeadImage = 4;
        //}

        //private void OnHead5Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead5Image.SetActiveEx(open);
        //    _seletctedHeadImage = 5;
        //}

        //private void OnHead6Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead6Image.SetActiveEx(open);
        //    _seletctedHeadImage = 6;
        //}
    }
}