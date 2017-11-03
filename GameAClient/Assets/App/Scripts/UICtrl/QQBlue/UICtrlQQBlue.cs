using System.Collections.Generic;
using NewResourceSolution;
using SoyEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome, EUIAutoSetupType.Create)]
    public class UICtrlQQBlue : UICtrlAnimationBase<UIViewQQBlue>
    {
        private EMenu _curMenu = EMenu.None;
        private bool _pushGoldEnergyStyle;
        private UPCtrlQQBlueBase _curMenuCtrl;
        private UPCtrlQQBlueBase[] _menuCtrlArray;
        private string _renewImage = "icon_blue_renew";
        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseButton.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlQQBlueBase[(int)EMenu.Max];
            
       
            var upCtrlNewPlayerAwardPanel = new UPCtrlQQBlueNewPlayer();
            upCtrlNewPlayerAwardPanel.Set(ResScenary);
            upCtrlNewPlayerAwardPanel.SetMenu(EMenu.NewPlayer);
            upCtrlNewPlayerAwardPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.NewPlayer] = upCtrlNewPlayerAwardPanel;     
          
            var upCtrlEveryDayAwardPanel = new UPCtrlQQBlueEveryDayAward();
            upCtrlEveryDayAwardPanel.Set(ResScenary);
            upCtrlEveryDayAwardPanel.SetMenu(EMenu.Everyday);
            upCtrlEveryDayAwardPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Everyday] = upCtrlEveryDayAwardPanel;
            
            var upCtrlGrowAwardPanel = new UPCtrlQQBlueGrowAward();
            upCtrlGrowAwardPanel.Set(ResScenary);
            upCtrlGrowAwardPanel.SetMenu(EMenu.Grow);
            upCtrlGrowAwardPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Grow] = upCtrlGrowAwardPanel;
            
            
            var upCtrlIntroduceAwardPanel = new UPCtrlQQBluePrivilegeIntroduce();
            upCtrlIntroduceAwardPanel.Set(ResScenary);
            upCtrlIntroduceAwardPanel.SetMenu(EMenu.Introduce);
            upCtrlIntroduceAwardPanel.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Introduce] = upCtrlIntroduceAwardPanel;
            
            
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
            for (int i = 0; i < _cachedView.DredgeBtnGroup.Length; i++)
            {
                _cachedView.DredgeBtnGroup[i].onClick.AddListener(OnDredgeBtnGroupBtn);
                if (LocalUser.Instance.User.UserInfoSimple.BlueVipData.IsBlueVip)
                {
                    _cachedView.DredgeBtnGroup[i].GetComponent<Image>().sprite = JoyResManager.Instance.GetSprite(_renewImage);
                }
                _cachedView.DredgeBtnGroup[i].GetComponent<Image>().sprite = JoyResManager.Instance.GetSprite(_renewImage);

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
            SocialGUIManager.Instance.CloseUI<UICtrlQQBlue>();
        }

        private void OnDredgeBtnGroupBtn()
        {
            WWebViewManager.Instance.Open(ERequestType.OpenBlueVip);
//            Application.OpenURL("http://pay.qq.com/ipay/index.shtml?n=3&c=xxzxgj,xxqgame&aid=VIP.APP*****.PLATqqgame&ch=qdqb,kj");
        }

        public enum EMenu
        {
            None = -1,
            NewPlayer,
            Everyday,
            Grow,
            Introduce,
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