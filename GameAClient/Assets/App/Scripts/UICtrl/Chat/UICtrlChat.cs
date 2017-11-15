using System.Collections.Generic;
using SoyEngine;
using UnityEngine;
using YIMEngine;

namespace GameA
{
    /// <summary>
    /// 聊天页面
    /// </summary>
    [UIResAutoSetup(EResScenary.UIHome)]
    public class UICtrlChat : UICtrlAnimationBase<UIViewChat>
    {
        private EMenu _curMenu = EMenu.None;
        private UPCtrlChatBase _curMenuCtrl;
        private UPCtrlChatBase[] _menuCtrlArray;
        private bool _isEditing;

        protected override void OnViewCreated()
        {
            base.OnViewCreated();
            _cachedView.CloseBtn.onClick.AddListener(OnCloseBtn);
            _cachedView.SendTexBtn.onClick.AddListener(OnSendTexBtn);
            _cachedView.FaceBtn.onClick.AddListener(OnFaceBtn);
            _cachedView.VoiceBtn.OnPress += OnStartRecordVoiceBtn;
            _cachedView.VoiceBtn.OnRelease += OnSendVoiceBtn;
            _cachedView.InptField.onEndEdit.AddListener(OnInptFieldEndEdit);
            _cachedView.InptField.onValueChanged.AddListener(OnInptFieldonValueChanged);
            _menuCtrlArray = new UPCtrlChatBase[(int) EMenu.Max];

            var upCtrlChatWorld = new UPCtrlChatWorld();
            upCtrlChatWorld.SetResScenary(ResScenary);
            upCtrlChatWorld.SetMenu(EMenu.World);
            upCtrlChatWorld.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.World] = upCtrlChatWorld;

            var upCtrlChatFriend = new UPCtrlChatFriend();
            upCtrlChatFriend.SetResScenary(ResScenary);
            upCtrlChatFriend.SetMenu(EMenu.Friend);
            upCtrlChatFriend.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.Friend] = upCtrlChatFriend;

            var upCtrlChatSystem = new UPCtrlChatSystem();
            upCtrlChatSystem.SetResScenary(ResScenary);
            upCtrlChatSystem.SetMenu(EMenu.System);
            upCtrlChatSystem.Init(this, _cachedView);
            _menuCtrlArray[(int) EMenu.System] = upCtrlChatSystem;

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
            if (parameter != null)
            {
                _curMenu = (EMenu) parameter;
            }
            if (_curMenu == EMenu.None)
            {
                _cachedView.TabGroup.SelectIndex((int) EMenu.World, true);
            }
            else
            {
                _cachedView.TabGroup.SelectIndex((int) _curMenu, true);
            }
        }

        protected override void OnClose()
        {
            if (_curMenuCtrl != null)
            {
                _curMenuCtrl.Close();
            }
            _isEditing = false;
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

        protected override void InitEventListener()
        {
            base.InitEventListener();
            RegisterEvent<string>(EMessengerType.OnReceiveStatus, ShowStatus);
            RegisterEvent<YIMManager.MyMessage>(EMessengerType.OnSendText, SendText);
            RegisterEvent<TextMessage>(EMessengerType.OnReceiveText, ReceiveText);
            RegisterEvent<YIMManager.MyVoice>(EMessengerType.OnSendVoice, SendVoice);
            RegisterEvent<VoiceMessage>(EMessengerType.OnReceiveVoice, ReceiveVoice);
        }

//        protected override void SetPartAnimations()
//        {
//            base.SetPartAnimations();
//            SetPart(_cachedView.PanelRtf, EAnimationType.MoveFromDown);
//            SetPart(_cachedView.MaskRtf, EAnimationType.Fade);
//        }

        protected override void InitGroupId()
        {
            _groupId = (int) EUIGroupType.Overlay;
        }

        public override void OnUpdate()
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                if (!_isEditing)
                {
                    _cachedView.InptField.Select();
                    _isEditing = true;
                }
            }
            //刷洗好友在线信息
            if (_menuCtrlArray != null && _menuCtrlArray[(int) EMenu.Friend] != null)
            {
                ((UPCtrlChatFriend) _menuCtrlArray[(int) EMenu.Friend]).OnUpdate();
            }
        }

        public void SetToFriend(int inx, List<UserInfoDetail> _friends)
        {
            ((UPCtrlChatFriend) _menuCtrlArray[(int) EMenu.Friend]).ChatToFriend(inx, _friends);
        }

        protected void OnSendTexBtn()
        {
            if (string.IsNullOrEmpty(_cachedView.InptField.text)) return;
            if (_curMenu == EMenu.World)
            {
                YIMManager.Instance.SendTextToRoom(_cachedView.InptField.text, YIMManager.Instance.WorldChatRoomId);
                _cachedView.InptField.text = string.Empty;
            }
            else if (_curMenu == EMenu.Friend)
            {
                YIMManager.Instance.SendTextToUser(_cachedView.InptField.text,
                    ((UPCtrlChatFriend) _curMenuCtrl).CurFriendId.ToString());
                _cachedView.InptField.text = string.Empty;
            }
        }

        private void OnFaceBtn()
        {
//            CompassManager.Instance.Register(LocalUser.Instance.UserGuid.ToString(), "836144322");
//            CompassManager.Instance.Online();
//            CompassManager.Instance.Consume(LocalUser.Instance.UserGuid.ToString(), "836144322","33");
//            CompassManager.Instance.Recharge(LocalUser.Instance.UserGuid.ToString(), "836144322","33");
//            CompassManager.Instance.Quit(LocalUser.Instance.UserGuid.ToString(), "836144322","33");
        }

        protected void OnStartRecordVoiceBtn()
        {
#if UNITY_STANDALONE_WIN

#else
            if (_curMenu == EMenu.World)
            {
                YIMManager.Instance.StartAudioRecordToRoom(YIMManager.Instance.WorldChatRoomId);
            }
            else if (_curMenu == EMenu.Friend)
            {
                YIMManager.Instance.StartAudioRecordToUser(((UPCtrlChatFriend) _curMenuCtrl).CurFriendId.ToString());
            }
#endif
        }

        protected void OnSendVoiceBtn()
        {
#if UNITY_STANDALONE_WIN

#else
            YIMManager.Instance.StopAudioMessage();
#endif
        }

        private void ShowStatus(string msg)
        {
            if (_cachedView == null)
            {
                SocialGUIManager.Instance.CreateView<UICtrlChat>();
            }
            var chatInfo = new ChatInfo();
            chatInfo.Content = msg;
            chatInfo.EChatSender = EChatSender.System;
            chatInfo.EChatType = EChatType.Text;
            _menuCtrlArray[(int) EMenu.System].AddChatItem(chatInfo);
        }

        private void SendText(YIMManager.MyMessage message)
        {
            if (_cachedView == null)
            {
                SocialGUIManager.Instance.CreateView<UICtrlChat>();
            }
            //当前页面显示
            var chatInfo = new ChatInfo();
            chatInfo.Content = message.TextContent;
            chatInfo.EChatSender = EChatSender.Myself;
            chatInfo.EChatType = EChatType.Text;
            chatInfo.SenderInfoDetail = LocalUser.Instance.User;
            chatInfo.ReceiverId = message.ReciverId;
            if (message.ChatType == ChatType.RoomChat &&
                message.ReciverId == YIMManager.Instance.WorldChatRoomId)
            {
                _menuCtrlArray[(int) EMenu.World].AddChatItem(chatInfo);
            }
            else if (message.ChatType == ChatType.PrivateChat)
            {
                _menuCtrlArray[(int) EMenu.Friend].AddChatItem(chatInfo);
                UserManager.Instance.GetDataOnAsync(long.Parse(message.ReciverId),
                    userInfoDetail => chatInfo.ReceiverInfoDetail = userInfoDetail);
            }
        }

        private void ReceiveText(TextMessage message)
        {
            if (_cachedView == null)
            {
                SocialGUIManager.Instance.CreateView<UICtrlChat>();
            }
            if (message.ChatType == ChatType.RoomChat && message.RecvID == YIMManager.Instance.WorldChatRoomId)
            {
                UserManager.Instance.GetDataOnAsync(long.Parse(message.SenderID), userInfoDetail =>
                {
                    var chatInfo = new ChatInfo();
                    chatInfo.Content = message.Content;
                    chatInfo.EChatSender = EChatSender.Other;
                    chatInfo.EChatType = EChatType.Text;
                    chatInfo.SenderInfoDetail = userInfoDetail;
                    chatInfo.ReceiverInfoDetail = LocalUser.Instance.User;
                    chatInfo.ReceiverId = LocalUser.Instance.User.UserInfoSimple.UserId.ToString();
                    _menuCtrlArray[(int) EMenu.World].AddChatItem(chatInfo);
                });
            }
            else if (message.ChatType == ChatType.PrivateChat)
            {
                UserManager.Instance.GetDataOnAsync(long.Parse(message.SenderID), userInfoDetail =>
                {
                    var chatInfo = new ChatInfo();
                    chatInfo.Content = message.Content;
                    chatInfo.EChatSender = EChatSender.Other;
                    chatInfo.EChatType = EChatType.Text;
                    chatInfo.SenderInfoDetail = userInfoDetail;
                    chatInfo.ReceiverInfoDetail = LocalUser.Instance.User;
                    chatInfo.ReceiverId = LocalUser.Instance.User.UserInfoSimple.UserId.ToString();
                    _menuCtrlArray[(int) EMenu.Friend].AddChatItem(chatInfo);
                });
            }
        }

        private void SendVoice(YIMManager.MyVoice voiceMessage)
        {
            if (_cachedView == null)
            {
                SocialGUIManager.Instance.CreateView<UICtrlChat>();
            }
            var chatInfo = new ChatInfo();
            chatInfo.Content = string.Format("(语音)……\r\n 识别结果：{0}", voiceMessage.Text);
            chatInfo.EChatSender = EChatSender.Myself;
            chatInfo.EChatType = EChatType.Voice;
            chatInfo.SenderInfoDetail = LocalUser.Instance.User;
            chatInfo.SavePath = voiceMessage.SavePath;
            chatInfo.ReceiverId = voiceMessage.RecvID;
            if (voiceMessage.ChatType == ChatType.RoomChat &&
                voiceMessage.RecvID == YIMManager.Instance.WorldChatRoomId)
            {
                _menuCtrlArray[(int) EMenu.World].AddChatItem(chatInfo);
            }
            else if (voiceMessage.ChatType == ChatType.PrivateChat)
            {
                _menuCtrlArray[(int) EMenu.Friend].AddChatItem(chatInfo);
                UserManager.Instance.GetDataOnAsync(long.Parse(voiceMessage.RecvID),
                    userInfoDetail => chatInfo.ReceiverInfoDetail = userInfoDetail);
            }
        }

        private void ReceiveVoice(VoiceMessage message)
        {
            if (_cachedView == null)
            {
                SocialGUIManager.Instance.CreateView<UICtrlChat>();
            }
            if (message.ChatType == ChatType.RoomChat && message.RecvID == YIMManager.Instance.WorldChatRoomId)
            {
                UserManager.Instance.GetDataOnAsync(long.Parse(message.SenderID), userInfoDetail =>
                {
                    var chatInfo = new ChatInfo();
                    chatInfo.Content = string.Format("(语音)……\r\n 识别结果：{0}", message.Text);
                    chatInfo.EChatSender = EChatSender.Other;
                    chatInfo.EChatType = EChatType.Voice;
                    chatInfo.SenderInfoDetail = userInfoDetail;
                    chatInfo.ReceiverInfoDetail = LocalUser.Instance.User;
                    chatInfo.ReceiverId = LocalUser.Instance.User.UserInfoSimple.UserId.ToString();
                    _menuCtrlArray[(int) EMenu.World].AddChatItem(chatInfo);
                });
            }
            else if (message.ChatType == ChatType.PrivateChat)
            {
                UserManager.Instance.GetDataOnAsync(long.Parse(message.SenderID), userInfoDetail =>
                {
                    var chatInfo = new ChatInfo();
                    chatInfo.Content = string.Format("(语音)……\r\n 识别结果：{0}", message.Text);
                    chatInfo.EChatSender = EChatSender.Other;
                    chatInfo.EChatType = EChatType.Voice;
                    chatInfo.SenderInfoDetail = userInfoDetail;
                    chatInfo.ReceiverInfoDetail = LocalUser.Instance.User;
                    chatInfo.ReceiverId = LocalUser.Instance.User.UserInfoSimple.UserId.ToString();
                    _menuCtrlArray[(int) EMenu.Friend].AddChatItem(chatInfo);
                });
            }
        }

        private void OnInptFieldEndEdit(string arg0)
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                _cachedView.SendTexBtn.Select();
                OnSendTexBtn();
            }
            CoroutineProxy.Instance.StartCoroutine(CoroutineProxy.RunNextFrame(() => _isEditing = false));
        }

        private void OnInptFieldonValueChanged(string arg0)
        {
            _isEditing = true;
        }

        private void OnCloseBtn()
        {
            SocialGUIManager.Instance.CloseUI<UICtrlChat>();
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

        public enum EMenu
        {
            None = -1,
            World,
            Friend,
            System,
            Max
        }
    }
}