using SoyEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlSocialRelationship : UICtrlAnimationBase<UIViewSocialRelationship>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship> _curMenuCtrl;
        private UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>[] _menuCtrlArray;
        private bool _pushGoldEnergyStyle;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnReturnBtnClick);
            _menuCtrlArray = new UPCtrlBase<UICtrlSocialRelationship, UIViewSocialRelationship>[(int) EMenu.Max];
            var upCtrlRelationshipFollow = new UPCtrlRelationshipFollow();
            upCtrlRelationshipFollow.Set(ResScenary);
            upCtrlRelationshipFollow.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Follow] = upCtrlRelationshipFollow;

            var upCtrlRelationshipFans = new UPCtrlRelationshipFans();
            upCtrlRelationshipFans.Set(ResScenary);
            upCtrlRelationshipFans.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Fans] = upCtrlRelationshipFans;

            var upCtrlRelationshipFriends = new UPCtrlRelationshipFriends();
            upCtrlRelationshipFriends.Set(ResScenary);
            upCtrlRelationshipFriends.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Friends] = upCtrlRelationshipFriends;

            var upCtrlRelationshipAddNew = new UPCtrlRelationshipAddNew();
            upCtrlRelationshipAddNew.Set(ResScenary);
            upCtrlRelationshipAddNew.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.AddNew] = upCtrlRelationshipAddNew;

            var upCtrlRelationshipBlock = new UPCtrlRelationshipBlock();
            upCtrlRelationshipBlock.Set(ResScenary);
            upCtrlRelationshipBlock.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Block] = upCtrlRelationshipBlock;

            for (int i = 0; i < _cachedView.MenuButtonAry.Length; i++)
            {
                var index = i;
                _cachedView.TabGroup.AddButton(_cachedView.MenuButtonAry[i], _cachedView.MenuSelectedButtonAry[i],
                    b => ClickMenu(index, b));
                if (i < _menuCtrlArray.Length && null != _menuCtrlArray[i])
                {
                    _menuCtrlArray[i].Close();
                }
            }
        }

        protected override void OnDestroy()
        {
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != null)
                {
                    _menuCtrlArray[i].OnDestroy();
                }
            }
            _curMenuCtrl = null;
            base.OnDestroy();
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.MainUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<long>(EMessengerType.OnRelationShipDataChanged, OnRelationShipDataChanged);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.Follow, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
            if (!_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PushStyle(UICtrlGoldEnergy.EStyle.GoldDiamond);
                _pushGoldEnergyStyle = true;
            }
        }

        protected override void OnClose()
        {
            if (_pushGoldEnergyStyle)
            {
                SocialGUIManager.Instance.GetUI<UICtrlGoldEnergy>().PopStyle();
                _pushGoldEnergyStyle = false;
            }
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            base.OnClose();
        }

        protected override void SetAnimationType()
        {
            base.SetAnimationType();
            _firstDelayFrames = 0;
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

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }

        private void OnReturnBtnClick()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlWorld>();
        }

        private void OnRelationShipDataChanged(long userId)
        {
            if (!_isOpen)
            {
                return;
            }
            if (_curMenuCtrl != null)
            {
                ((IOnChangeHandler<long>) _curMenuCtrl).OnChangeHandler(userId);
            }
        }

        private enum EMenu
        {
            None = -1,
            Follow,
            Fans,
            Friends,
            AddNew,
            Block,
            Max
        }
    }
}