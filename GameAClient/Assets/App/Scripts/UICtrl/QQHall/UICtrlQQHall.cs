using System.Collections.Generic;
using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlQQHall : UICtrlAnimationBase<UIViewQQHall>
    {
        private EMenu _curMenu = EMenu.None;
        private bool _pushGoldEnergyStyle;
        private UPCtrlQQHallBase _curMenuCtrl;
        private UPCtrlQQHallBase[] _menuCtrlArray;
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseButton.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlQQHallBase[(int)EMenu.Max];
            
            var upCtrlWorldNewPlayerAwardPanel = new UPCtrlQQNewPlayer();
            upCtrlWorldNewPlayerAwardPanel.Set(ResScenary);
            upCtrlWorldNewPlayerAwardPanel.SetMenu(EMenu.NewPlayer);
            upCtrlWorldNewPlayerAwardPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.NewPlayer] = upCtrlWorldNewPlayerAwardPanel;
            
            var upCtrlQQGrowAward = new UPCtrlQQGrowAward();
            upCtrlQQGrowAward.Set(ResScenary);
            upCtrlQQGrowAward.SetMenu(EMenu.Grow);
            upCtrlQQGrowAward.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Grow] = upCtrlQQGrowAward;
            
            var upCtrlQQEveryDayAward = new UPCtrlQQEveryDayAward();
            upCtrlQQEveryDayAward.Set(ResScenary);
            upCtrlQQEveryDayAward.SetMenu(EMenu.Everyday);
            upCtrlQQEveryDayAward.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Everyday] = upCtrlQQEveryDayAward;
            
            
            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var inx = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i],
                    b => ClickMenu(inx, b));
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }
         
        }

        protected override void OnDestroy()
        {
          
            base.OnDestroy();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
   
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            Clear();
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.NewPlayer, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
        }

        private void Clear()
        {
           
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
       
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
        }



        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }
        private void ChangeMenu(EMenu menu)
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _curMenu = menu;
            var inx = (int) _curMenu;
            if (inx < _menuCtrlArray.Length)
            {
                _curMenuCtrl = _menuCtrlArray[inx];
            }
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Open();
            }
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlQQHall>();
        }

        public enum EMenu
        {
            None = -1,
            NewPlayer,
            Grow,
            Everyday,
            Max
        }
        public class Award
        {
            public Award(UMCtrlQQAward.AwardEnum type, int num)
            {
                Type = type;
                Num = num;
            }

            public UMCtrlQQAward.AwardEnum Type;
            public int Num;
        }
        public  static  List<Award> NewPlayerAwards= new List<Award>()
            {new Award(UMCtrlQQAward.AwardEnum.Diamond, 10),new Award(UMCtrlQQAward.AwardEnum.Coin, 50) };
        public  static  List<Award> EveryDayACtiveAwards= new List<Award>()
            {new Award(UMCtrlQQAward.AwardEnum.Diamond, 20),new Award(UMCtrlQQAward.AwardEnum.Coin, 30) };
    }
}