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

        public int SeletctedHeadImage
        {
            get { return _seletctedHeadImage; }
            set { _seletctedHeadImage = value; }
        }

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
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPortraitSelect>();
        }

        protected override void OnOpen(object parameter)
        {
            SetHead();
        }

        private void OnOKBtn()
        {
            SocialGUIManager.Instance.GetUI<UICtrlPersonalInformation>().SetHead(_seletctedHeadImage);
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPortraitSelect>();
        }

        public void SetHead()
        {
            for (int i = 0; i < _headMaxNumber; i++)
            {
                var UM = new UMCtrlHead();
                UM.Init(_cachedView.Dock as RectTransform);
                UM.Set(i);
                _cardList.Add(UM);
            }
        }

        public void InitTagGroup(Button button,Action<bool>function)
        {
            _cachedView.TagGroup.AddButton(button, function);
            //_cachedView.TagGroup.AddButton(_cachedView.HeadPortrait2, OnHead2Seleted);
        }
        //private void OnHead2Seleted(bool open)
        //{
        //    _cachedView.SeletctedHead2Image.SetActiveEx(open);
        //    _seletctedHeadImage = 2;
        //}
    }
}