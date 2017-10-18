using System;
using UnityEngine;
using System.Collections.Generic;
using GameA;
using SoyEngine;
using YIMEngine;
using EMessengerType = GameA.EMessengerType;

public class YIMManager :
    LoginListen,
    MessageListen,
    ChatRoomListen,
    DownloadListen,
    ContactListen,
    NoticeListen
{
    public class MyMessage
    {
        public ulong id;
        public string textContent;
        public string filePath;
        public string reciver;
        public int status; //假设0发送中，1发送成功，2发送失败
        public ChatType chatType;

        public void Clear()
        {
            id = 0;
            textContent = filePath = reciver = null;
            status = 0;
            chatType = ChatType.Unknow;
        }
    }

    public readonly string WorldChatRoomId = "10001";
    private Dictionary<ulong, MyMessage> messageCahceList = new Dictionary<ulong, MyMessage>(); //记录消息状态
    private MyMessage _myMessage = new MyMessage();
    private bool _manualLogout;
    private long _userid;
    private static YIMManager _instance;

    public static YIMManager Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new YIMManager();
            }
            return _instance;
        }
    }

    public YIMManager()
    {
        //注册回调
        IMAPI.Instance().SetLoginListen(this);
        IMAPI.Instance().SetMessageListen(this);
        IMAPI.Instance().SetChatRoomListen(this);
        IMAPI.Instance().SetDownloadListen(this);
        //初始化
        IMAPI.Instance().Init("YOUMEE9036C7A45B37ED5F7AC6F0C6479BB48EC2CA10E",
            "y7/AAtmf6Ry6awljGzwbs0x6VFPQcckKlpHQCno1KiA3YEu1g5UwLPbVxBEnSLGfbPDfE8Qj/qYp1shwQVptV3uvmVsueIaHKrzZ+uOY715kpAnNGIYEQsp7Pi0QV07/Ii3z9oRShJKdj/I7qWM9UYF0+pJxkCnAgkR8SD15tuUBAAE=");
        Messenger.AddListener(SoyEngine.EMessengerType.OnAccountLogout, Logout);
        Messenger.AddListener(EMessengerType.OnApplicationQuit, Destroy);
    }

    private void ShowStatus(string msg)
    {
//        Messenger<string>.Broadcast(EMessengerType.OnReceiveStatus, msg);
    }

    private void ReceiveText(TextMessage message)
    {
        Messenger<TextMessage>.Broadcast(EMessengerType.OnReceiveText, message);
    }

    private void ReceiveVoice(VoiceMessage message)
    {
        Messenger<VoiceMessage>.Broadcast(EMessengerType.OnReceiveVoice, message);
    }

    public void Login()
    {
        _userid = LocalUser.Instance.UserGuid;
        ErrorCode errorcode = IMAPI.Instance().Login(_userid.ToString(), "123456");
        LogHelper.Debug("sendmessage: " + "errorcode: " + errorcode);
    }

    public void Logout()
    {
        LeaveChatRoom(WorldChatRoomId);
        _manualLogout = true;
        IMAPI.Instance().Logout();
        LogHelper.Debug("logout");
    }

    public void JoinChatRoom(string roomId)
    {
        IMAPI.Instance().JoinChatRoom(roomId);
        ShowStatus("joinchatroom");
        LogHelper.Debug("joinchatroom");
    }

    public void LeaveChatRoom(string roomId)
    {
        IMAPI.Instance().LeaveChatRoom(roomId);
        ShowStatus("leavechatroom");
        LogHelper.Debug("leavechatroom");
    }

    public void SendTextToUser(string content, string userId)
    {
        SendTextMessage(content, userId, ChatType.PrivateChat);
    }

    public void SendTextToRoom(string content, string roomId)
    {
        SendTextMessage(content, roomId, ChatType.RoomChat);
    }

    private void SendTextMessage(string content, string id, ChatType chatType)
    {
        _myMessage.Clear();
        _myMessage.id = 0;
        _myMessage.textContent = content;
        _myMessage.reciver = id;
        _myMessage.chatType = chatType;
        IMAPI.Instance().SendTextMessage(_myMessage.reciver, _myMessage.chatType, _myMessage.textContent,
            ref _myMessage.id);
        messageCahceList.Add(_myMessage.id, _myMessage);
        ShowTalk(_myMessage);
    }

    public void StartAudioRecordToUser(string userId)
    {
        StartAudioRecord(userId, ChatType.PrivateChat);
    }

    public void StartAudioRecordToRoom(string roomId)
    {
        StartAudioRecord(roomId, ChatType.RoomChat);
    }

    LRUCache<ulong, string> iRequestCache = new LRUCache<ulong, string>(64);

    private void StartAudioRecord(string id, ChatType chatType)
    {
        ulong iRequestID = 0;
        ErrorCode errorcode = IMAPI.Instance().SendAudioMessage(id, chatType, ref iRequestID);
        iRequestCache.Insert(iRequestID, id);
        LogHelper.Debug("sendmessage: RequestID:" + iRequestID + "errorcode: " + errorcode);
    }

    public void StopAudioMessage()
    {
        IMAPI.Instance().StopAudioMessage("");
    }

    #region LoginListen implementation

    bool _gameOnline = true; //假设游戏是在线状态

    public void OnLogin(ErrorCode errorcode, string strYouMeID)
    {
        ShowStatus("OnLogin: errorcode" + errorcode + " userID:" + strYouMeID);
        JoinChatRoom(WorldChatRoomId);
    }

    public void OnLogout()
    {
        ShowStatus("OnLogout");
        if (!_manualLogout && _gameOnline)
        {
            IMAPI.Instance().Login(_userid.ToString(), "123456");
        }
    }

    #endregion

    #region MessageListen implementation

    public void OnSendMessageStatus(ulong iRequestID, ErrorCode errorcode, bool isForbidRoom, int reasonType,
        ulong forbidEndTime)
    {
        MyMessage msg;
        if (messageCahceList.TryGetValue(iRequestID, out msg))
        {
            if (errorcode == ErrorCode.Success)
            {
                msg.status = 1;
            }
            else
            {
                msg.status = 2;
            }
            UpdateMsg(msg);
            messageCahceList.Remove(iRequestID);
        }
        LogHelper.Debug("OnSendMessageStatus request:" + iRequestID + "errorcode:" + errorcode);
    }

    private bool CheckRequestId(ref ulong iRequestID, ulong id)
    {
        return iRequestID == id;
    }

    public void OnSendAudioMessageStatus(ulong iRequestID, ErrorCode errorcode, string strText,
        string strAudioPath, int iDuration, bool isForbidRoom, int reasonType, ulong forbidEndTime)
    {
        LogHelper.Debug("OnSendAudioMessageStatus request:" + iRequestID + " errorcode:" + errorcode + " 识别结果:" +
                        strText +
                        " path:" + strAudioPath + " 语音时长:" + iDuration);
        //如果自己发送的语音消息发送成功，就播放出来
        if (errorcode == ErrorCode.Success)
        {
            string receiveId;
            if (iRequestCache.TryGetItem(iRequestID, out receiveId))
            {
                ShowVoice(strText, receiveId);
            }
            else
            {
                ShowVoice(strText, null);
            }
            YIMAudioPlayer.Instance.PlayAudioFile(strAudioPath);
        }
    }

    public void OnStartSendAudioMessage(ulong iRequestID, ErrorCode errorcode, string strText,
        string strAudioPath, int iDuration)
    {
    }

    public void OnRecvMessage(MessageInfoBase message)
    {
        if (message.MessageType == MessageBodyType.TXT)
        {
            TextMessage textMsg = (TextMessage) message;
            ReceiveText(textMsg);
            LogHelper.Debug("OnRecvMessage text:" + textMsg.Content + " send:" + textMsg.SenderID + "recv:" +
                            textMsg.RecvID);
        }
        else if (message.MessageType == MessageBodyType.CustomMesssage)
        {
            CustomMessage customMsg = (CustomMessage) message;
            LogHelper.Debug("OnRecvMessage custom:" + System.Convert.ToBase64String(customMsg.Content) + " send:" +
                            customMsg.SenderID + "recv:" + customMsg.RecvID);
        }
        else if (message.MessageType == MessageBodyType.Voice)
        {
            VoiceMessage voiceMsg = (VoiceMessage) message;
            LogHelper.Debug("OnRecvMessage voice 文本识别结果:" + voiceMsg.Text + " send:" + voiceMsg.SenderID + "recv:" +
                            voiceMsg.RecvID + " 时长:" + voiceMsg.Duration);
            //下载收到的语音消息
            IMAPI.Instance().DownloadAudioFile(voiceMsg.RequestID, GetUniqAudioPath());
            ReceiveVoice(voiceMsg);
        }
    }

//获取到本地私聊历史纪录
    public void OnQueryHistoryMessage(ErrorCode errorcode, string targetID, int remain,
        List<HistoryMsg> messageList)
    {
    }

    public void OnStopAudioSpeechStatus(ErrorCode errorcode, ulong iRequestID, string strDownloadURL,
        int iDuration, int iFileSize, string strLocalPath, string strText)
    {
    }

//新消息通知
    public void OnRecvNewMessage(ChatType chatType, string targetID)
    {
    }

    public void OnAccusationResultNotify(AccusationDealResult result, string userID, uint accusationTime)
    {
    }

    public void OnGetForbiddenSpeakInfo(ErrorCode errorcode, List<ForbiddenSpeakInfo> forbiddenSpeakList)
    {
    }

    #endregion

    #region ChatRoomListen implementation

    public void OnJoinRoom(ErrorCode errorcode, string strChatRoomID)
    {
        ShowStatus("OnJoinRoom: errorcode" + errorcode + " roomID:" + strChatRoomID);
    }

    public void OnLeaveRoom(ErrorCode errorcode, string strChatRoomID)
    {
        ShowStatus("OnLeaveRoom: errorcode" + errorcode + " roomID:" + strChatRoomID);
    }

    public void OnUserJoinChatRoom(string strRoomID, string strUserID)
    {
    }

    public void OnUserLeaveChatRoom(string strRoomID, string strUserID)
    {
    }

    #endregion

    public void OnGetContact(List<string> contactLists)
    {
    }

    public void OnGetUserInfo(ErrorCode code, string userID, IMUserInfo userInfo)
    {
    }

    public void OnQueryUserStatus(ErrorCode code, string userID, UserStatus status)
    {
        if (_userStatusCallBackDic.ContainsKey(userID))
        {
            _userStatusCallBackDic[userID].Invoke(status);
            _userStatusCallBackDic.Remove(userID);
        }
    }

    public void OnDownload(ErrorCode errorcode, MessageInfoBase message, string strSavePath)
    {
        //如果下载收到的语音消息成功，就播放
        if (errorcode == ErrorCode.Success)
        {
            YIMAudioPlayer.Instance.PlayAudioFile(strSavePath);
        }
    }

    public void OnDownloadByUrl(ErrorCode errorcode, string strFromUrl, string strSavePath)
    {
    }

    #region YIMEngine.NoticeListen implementation

    public void OnRecvNotice(Notice notice)
    {
    }

    public void OnCancelNotice(ulong noticeID, string channelID)
    {
    }

    #endregion

    private void UpdateMsg(MyMessage msg)
    {
        //这个函数是假设用来更新消息的发送是否成功的状态
    }

    private void ShowTalk(MyMessage message)
    {
        //这个函数是假设用来添加消息到UI上
        Messenger<MyMessage>.Broadcast(EMessengerType.OnSendText, message);
    }

    private void ShowVoice(string msg, string id)
    {
        //这个函数是假设用来添加消息到UI上
        Messenger<string, string>.Broadcast(EMessengerType.OnSendVoice, msg, id);
    }

    private string GetUniqAudioPath()
    {
        return Application.temporaryCachePath + "/YoumeIMAudioCache/" +
               System.Guid.NewGuid().ToString().Replace("-", "") + ".wav";
    }

    private Dictionary<string, Action<UserStatus>> _userStatusCallBackDic =
        new Dictionary<string, Action<UserStatus>>(5);

    public void CheckUserOnLine(string userId, Action<UserStatus> userStatusCallBack)
    {
        IMAPI.Instance().QueryUserStatus(userId);
        if (userStatusCallBack != null)
        {
            if (_userStatusCallBackDic.ContainsKey(userId))
            {
                _userStatusCallBackDic[userId] = userStatusCallBack;
            }
            else
            {
                _userStatusCallBackDic.Add(userId, userStatusCallBack);
            }
        }
    }

    public void Destroy()
    {
        LeaveChatRoom(WorldChatRoomId);
#if UNITY_EDITOR
        _manualLogout = true;
        IMAPI.Instance().Logout();
#else
        IMAPI.Instance().UnInit();
#endif
    }
}