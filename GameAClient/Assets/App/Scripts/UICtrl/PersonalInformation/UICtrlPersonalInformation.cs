using SoyEngine;
using UnityEngine;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlPersonalInformation : UICtrlAnimationBase<UIViewPersonalInformation>, ICheckOverlay
    {
        public UserInfoDetail UserInfoDetail;
        public bool IsMyself;
        private long _lastUserId = -1;
        private EMenu _curMenu = EMenu.None;
        private const string _numFormat = "({0})";
        private const string _maxShow = "(99+)";
        private const string _maxShowLong = "(999+)";
        private UPCtrlPersonalInfoBase _curMenuCtrl;
        private UPCtrlPersonalInfoBase[] _menuCtrlArray;
        public int MessageCount;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.FollowBtn.onClick.AddListener(OnFollowBtn);
            _cachedView.ChatBtn.onClick.AddListener(OnChatBtn);
            _cachedView.BlockBtn.onClick.AddListener(OnBlockBtn);
            _menuCtrlArray = new UPCtrlPersonalInfoBase[(int) EMenu.Max];
            var upCtrlPersonalInfoBasicInfo = new UPCtrlPersonalInfoBasicInfo();
            upCtrlPersonalInfoBasicInfo.SetResScenary(ResScenary);
            upCtrlPersonalInfoBasicInfo.SetMenu(EMenu.BasicInfo);
            upCtrlPersonalInfoBasicInfo.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.BasicInfo] = upCtrlPersonalInfoBasicInfo;

            var upCtrlPersonalInfoAchieve = new UPCtrlPersonalInfoAchieve();
            upCtrlPersonalInfoAchieve.SetResScenary(ResScenary);
            upCtrlPersonalInfoAchieve.SetMenu(EMenu.AchivementInfo);
            upCtrlPersonalInfoAchieve.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.AchivementInfo] = upCtrlPersonalInfoAchieve;

            var upCtrlPersonalInfoProjectPublish = new UpCtrlPersonalInfoProjectPublish();
            upCtrlPersonalInfoProjectPublish.SetResScenary(ResScenary);
            upCtrlPersonalInfoProjectPublish.SetMenu(EMenu.Publish);
            upCtrlPersonalInfoProjectPublish.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Publish] = upCtrlPersonalInfoProjectPublish;

            var upCtrlPersonalInfoProjectCollect = new UpCtrlPersonalInfoProjectCollect();
            upCtrlPersonalInfoProjectCollect.SetResScenary(ResScenary);
            upCtrlPersonalInfoProjectCollect.SetMenu(EMenu.Collects);
            upCtrlPersonalInfoProjectCollect.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Collects] = upCtrlPersonalInfoProjectCollect;

            var upCtrlPersonalInforRecords = new UPCtrlPersonalInfoRecords();
            upCtrlPersonalInforRecords.SetResScenary(ResScenary);
            upCtrlPersonalInforRecords.SetMenu(EMenu.Records);
            upCtrlPersonalInforRecords.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Records] = upCtrlPersonalInforRecords;

            var upCtrlPersonalInfoMessageBoard = new UPCtrlPersonalInfoMessageBoard();
            upCtrlPersonalInfoMessageBoard.SetResScenary(ResScenary);
            upCtrlPersonalInfoMessageBoard.SetMenu(EMenu.MessageBoard);
            upCtrlPersonalInfoMessageBoard.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MessageBoard] = upCtrlPersonalInfoMessageBoard;

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
            _groupId = (int) EUIGroupType.FrontUI;
        }

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<UserInfoDetail>(EMessengerType.OnRelationShipChanged, OnRelationShipChanged);
            RegisterEvent<UserInfoDetail>(EMessengerType.OnUserInfoChanged, OnUserInfoChanged);
            RegisterEvent(EMessengerType.OnMessageBoardElementSizeChanged, OnMessageBoardElementSizeChanged);
            RegisterEvent<long, UserMessageReply>(EMessengerType.OnReplyMessage, OnReplyMessage);
        }

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            UserInfoDetail = parameter as UserInfoDetail;
            if (null == UserInfoDetail)
            {
                SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
                return;
            }
            UserInfoDetail.Request(UserInfoDetail.UserInfoSimple.UserId, RefreshView, null);
            _cachedView.MessageNum.SetActiveEx(false);
            _cachedView.MessageSelectedNum.SetActiveEx(false);
            //测试
            MessageCount = Random.Range(6, 30);
            RefreshMessageNum(MessageCount);
            RefreshView();
            if (UserInfoDetail.UserInfoSimple.UserId != _lastUserId)
            {
                _curMenu = EMenu.BasicInfo;
                _lastUserId = UserInfoDetail.UserInfoSimple.UserId;
            }
            _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            Clear();
            base.OnClose();
        }

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        private void RefreshView()
        {
            IsMyself = UserInfoDetail.UserInfoSimple.UserId == LocalUser.Instance.UserGuid;
            _cachedView.BtnsObj.SetActiveEx(!IsMyself);
            _cachedView.AvatarRawImage.texture =
                SocialGUIManager.Instance.GetUI<UICtrlFashionSpine>().AvatarRenderTexture;
            RefreshBtns();
        }

        private void OnReplyMessage(long messageId, UserMessageReply reply)
        {
            if (_curMenu == EMenu.MessageBoard)
            {
                ((UPCtrlPersonalInfoMessageBoard) _curMenuCtrl).OnReplyMessage(messageId, reply);
            }
        }

        private void OnMessageBoardElementSizeChanged()
        {
            _cachedView.MessageTableDataScroller.RefreshAllSizes();
        }

        private void Clear()
        {
//            UserInfoDetail = null;
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                _menuCtrlArray[i].Clear();
            }
        }

        private void OnUserInfoChanged(UserInfoDetail user)
        {
            if (_isOpen && user == UserInfoDetail)
            {
                if (_curMenu == EMenu.BasicInfo)
                {
                    _curMenuCtrl.RefreshView();
                }
//                int count = UserInfoDetail.MessageCount;
//                RefreshMessageNum(count);
            }
        }

        public void RefreshMessageNum(int count)
        {
            _cachedView.MessageNum.SetActiveEx(count > 0);
            _cachedView.MessageSelectedNum.SetActiveEx(count > 0);
            if (count > 0)
            {
                _cachedView.MessageNum.text =
                    count < 100 ? string.Format(_numFormat, count) : _maxShow;
                _cachedView.MessageSelectedNum.text =
                    count < 1000 ? string.Format(_numFormat, count) : _maxShowLong;
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

        private void ClickMenu(int selectInx, bool open)
        {
            if (open)
            {
                ChangeMenu((EMenu) selectInx);
            }
        }

        private void RefreshBtns()
        {
            if (IsMyself) return;
            bool block = UserInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe;
            bool follow = UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe;

            _cachedView.FollowBtnTxt.text = follow ? RelationCommonString.FollowedStr : RelationCommonString.FollowStr;
            _cachedView.BlockBtnTxt.text = block ? RelationCommonString.BlockedStr : RelationCommonString.BlockStr;
            _cachedView.FollowBtn.SetActiveEx(!block);
            _cachedView.ChatBtn.SetActiveEx(false);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>();
        }

        private void OnFollowBtn()
        {
            if (UserInfoDetail.UserInfoSimple.RelationWithMe.FollowedByMe)
            {
                LocalUser.Instance.RelationUserList.RequestRemoveFollowUser(UserInfoDetail);
            }
            else
            {
                LocalUser.Instance.RelationUserList.RequestFollowUser(UserInfoDetail);
            }
        }

        private void OnBlockBtn()
        {
            if (UserInfoDetail.UserInfoSimple.RelationWithMe.BlockedByMe)
            {
                LocalUser.Instance.RelationUserList.RequestRemoveBlockUser(UserInfoDetail);
            }
            else
            {
                LocalUser.Instance.RelationUserList.RequestBlockUser(UserInfoDetail);
            }
        }

        private void OnChatBtn()
        {
//            LocalUser.Instance.RelationUserList.RequestChat(UserInfoDetail,
//                () => SocialGUIManager.Instance.CloseUI<UICtrlPersonalInformation>());
        }

        private void OnRelationShipChanged(UserInfoDetail userInfoDetail)
        {
            if (userInfoDetail == UserInfoDetail)
            {
                RefreshBtns();
            }
        }

        public enum EMenu
        {
            None = -1,
            BasicInfo,
            AchivementInfo,
            Publish,
            Collects,
            Records,
            MessageBoard,
            Max
        }
    }
}