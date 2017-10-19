using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlHeadPortraitSelect : UICtrlResManagedBase<UIViewHeadPortraitSelect>
    {
        private int _seletctedHeadImage;
        private int _headMaxNumber= 12;
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
//            SocialGUIManager.Instance.GetUI<UICtrlPersonalInformation>().SetHead(_seletctedHeadImage);
            SocialGUIManager.Instance.CloseUI<UICtrlHeadPortraitSelect>();
        }

        public void SetHead()
        {
            if (_cardList.Count > 0)
            {
                for (int i = 0; i < _cardList.Count; i++)
                {
                    _cardList[i].Destroy();
                }
            }
            _cardList.Clear();
            for (int i = 0; i < _headMaxNumber; i++)
            {
                var um = new UMCtrlHead();
                um.Init(_cachedView.Dock, ResScenary);
                um.Set(i);
                _cardList.Add(um);
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