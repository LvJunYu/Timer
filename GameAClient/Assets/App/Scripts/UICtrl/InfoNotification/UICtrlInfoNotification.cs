using GameA.Game;
using SoyEngine;
using SoyEngine.Proto;

namespace GameA
{
    [UIResAutoSetup(EResScenary.UICommon)]
    public class UICtrlInfoNotification : UICtrlAnimationBase<UIViewInfoNotification>, ICheckOverlay
    {
        private const string ReplyFormat = "回复<color=#F4A251>{0}</color>";
        private EMenu _curMenu = EMenu.None;
        private UPCtrlInfoNotificationBase _curMenuCtrl;
        private UPCtrlInfoNotificationBase[] _menuCtrlArray;
        private NotificationDataItem _curReplyData;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.SendBtn.onClick.AddListener(OnSendBtn);
            _cachedView.ClearBtn.onClick.AddListener(OnClearBtn);
            _cachedView.CancelBtn.onClick.AddListener(OnCancelBtn);
            _cachedView.MaskBtn.onClick.AddListener(OnCancelBtn);
            _menuCtrlArray = new UPCtrlInfoNotificationBase[(int) EMenu.Max];

            var upCtrlInfoNotificationBasic = new UPCtrlInfoNotificationBasic();
            upCtrlInfoNotificationBasic.Set(ResScenary);
            upCtrlInfoNotificationBasic.SetMenu(EMenu.Basic);
            upCtrlInfoNotificationBasic.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Basic] = upCtrlInfoNotificationBasic;

            var upCtrlInfoNotificationMyFollow = new UPCtrlInfoNotificationMyFollow();
            upCtrlInfoNotificationMyFollow.Set(ResScenary);
            upCtrlInfoNotificationMyFollow.SetMenu(EMenu.Follow);
            upCtrlInfoNotificationMyFollow.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Follow] = upCtrlInfoNotificationMyFollow;

            var upCtrlInfoNotificationMyProject = new UPCtrlInfoNotificationMyProject();
            upCtrlInfoNotificationMyProject.Set(ResScenary);
            upCtrlInfoNotificationMyProject.SetMenu(EMenu.MyProject);
            upCtrlInfoNotificationMyProject.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.MyProject] = upCtrlInfoNotificationMyProject;

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

        protected override void OnOpen(object parameter)
        {
            base.OnOpen(parameter);
            SocialGUIManager.Instance.CloseUI<UICtrlInfoNotificationRaw>();
            SetReplyPannel(false);
            RefreshReds();
            if (_curMenu == EMenu.None)
            {
                _curMenu = EMenu.Basic;
            }

            _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            for (int i = 0; i < _menuCtrlArray.Length; i++)
            {
                if (_menuCtrlArray[i] != _curMenuCtrl)
                {
                    _menuCtrlArray[i].RequestData();
                } 
            }
        }

        public void RefreshReds()
        {
            var cache = InfoNotificationManager.Instance.NotificationDataCache;
            var basicHasNew = false;
            var projectHasNew = false;
            if (cache != null)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    var menu = InfoNotificationManager.CheckMenu(cache[i]);
                    if (menu == EMenu.Basic)
                    {
                        basicHasNew = true;
                    }
                    else if (menu == EMenu.MyProject)
                    {
                        projectHasNew = true;
                    }
                }
            }

            _cachedView.RedAry[(int) EMenu.Basic].SetActive(basicHasNew);
            _cachedView.RedAry[(int) EMenu.MyProject].SetActive(projectHasNew);
            if (basicHasNew)
            {
                _curMenu = EMenu.Basic;
            }
            else if (projectHasNew)
            {
                _curMenu = EMenu.MyProject;
            }
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }

            SocialGUIManager.Instance.OpenUI<UICtrlInfoNotificationRaw>();
            base.OnClose();
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

        protected override void SetPartAnimations()
        {
            base.SetPartAnimations();
            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Notification;
        }

        private void OnClearBtn()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.ClearData();
            }
        }

        private void OnSendBtn()
        {
            var str = _cachedView.ReplyInputField.text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            if (_curReplyData != null)
            {
                if (_curReplyData.Type == ENotificationDataType.NDT_ProjectCommentReply)
                {
                    _curReplyData.ProjectCommentReply.Reply(str, ReplyFinish);
                }
                else if (_curReplyData.Type == ENotificationDataType.NDT_UserMessageBoardReply)
                {
                    _curReplyData.UserMessageReply.Reply(str, ReplyFinish);
                }
            }
        }

        private void ReplyFinish()
        {
            _curReplyData.MarkRead();
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.OnMarkRead(_curReplyData);
            }

            SetReplyPannel(false);
        }

        private void OnCancelBtn()
        {
            SetReplyPannel(false);
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlInfoNotification>();
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

        public void SetReplyPannel(bool value, NotificationDataItem data = null)
        {
            _cachedView.ReplayPannel.SetActiveEx(value);
            if (value)
            {
                _curReplyData = data;
                if (data != null)
                {
                    _cachedView.ReplyTxt.text = string.Format(ReplyFormat, data.Sender.NickName);
                }
            }
            else
            {
                _cachedView.ReplyInputField.text = string.Empty;
            }
        }

        public enum EMenu
        {
            None = -1,
            Basic,
            Follow,
            MyProject,
            Max
        }
    }
}