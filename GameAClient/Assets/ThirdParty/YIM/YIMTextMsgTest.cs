using UnityEngine;
using System.Collections.Generic;
using YIMEngine;

public class YIMTextMsgTest : MonoBehaviour,
    LoginListen,
    MessageListen,
    ChatRoomListen,
    DownloadListen,
    ContactListen,
	NoticeListen
{
    private Vector2 m_Position = Vector2.zero;
    private string m_InGameLog = "";
    private bool manualLogout = false;
    private int userid=1;

    public class MyMessage{
        public ulong id;
        public string textContent;
        public string filePath;
        public string reciver;
        public int status; //假设0发送中，1发送成功，2发送失败
        public ChatType chatType;
    }

    //记录消息状态
    private Dictionary<ulong, MyMessage> messageCahceList = new Dictionary<ulong, MyMessage>();


    // Use this for initialization
    void Start()
    {
        //注册回调
        IMAPI.Instance().SetLoginListen(this);
        IMAPI.Instance().SetMessageListen(this);
        IMAPI.Instance().SetChatRoomListen(this);
        IMAPI.Instance().SetDownloadListen(this);
        //初始化
        IMAPI.Instance().Init("YOUMEE9036C7A45B37ED5F7AC6F0C6479BB48EC2CA10E", "y7/AAtmf6Ry6awljGzwbs0x6VFPQcckKlpHQCno1KiA3YEu1g5UwLPbVxBEnSLGfbPDfE8Qj/qYp1shwQVptV3uvmVsueIaHKrzZ+uOY715kpAnNGIYEQsp7Pi0QV07/Ii3z9oRShJKdj/I7qWM9UYF0+pJxkCnAgkR8SD15tuUBAAE=");
        userid = Random.Range(1,100000);

    }

   
    void showStatus(string msg)
    {
        m_InGameLog += msg;
        m_InGameLog += "\r\n";
    }
    void OnGUI()
    {

        int inset = Screen.width / 20;
        int space = Screen.width / 30;
        int btnsOneRow = 3;
        int btnWidth = (Screen.width - inset * 2 - space * (btnsOneRow - 1)) / btnsOneRow;
        int btnHeight = btnWidth / 3;

        int labelX = inset;
        int labelY = inset + (btnHeight + space) * 5;
        int labelWidth = Screen.width - labelX * 2;
        int labelHeight = Screen.height - labelY;

        GUI.BeginGroup(new Rect(labelX, labelY, labelWidth, labelHeight));

        m_Position = GUILayout.BeginScrollView(m_Position, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
        GUILayout.Label(m_InGameLog);
        GUILayout.EndScrollView();

        GUI.EndGroup();

        if (GUI.Button(new Rect(inset, inset, btnWidth, btnHeight), "login"))
        {
            ErrorCode errorcode = IMAPI.Instance().Login(userid.ToString(), "123456");
            Debug.Log("sendmessage: " + "errorcode: " + errorcode);
        }

        if (GUI.Button(new Rect(inset + btnWidth + space, inset, btnWidth, btnHeight), "logout"))
        {
            Debug.Log("logout");
            showStatus("logout");
            manualLogout = true;
            IMAPI.Instance().Logout();
            //VoiceChannelPlugin.ExitChannel();
        }


        if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset, btnWidth, btnHeight), "joinchatroom"))
        {
            Debug.Log("joinchatroom");
            IMAPI.Instance().JoinChatRoom("1001");

            //VoiceChannelPlugin.StartTalking();
        }

        if (GUI.Button(new Rect(inset + (btnWidth + space), inset + (btnHeight + space) * 1, btnWidth, btnHeight), "SendText to Room"))
        {
            MyMessage msg = new MyMessage();
            msg.id = 0;
            msg.textContent = "这是一条发给1001房间的消息";
            msg.reciver = "1001";
            msg.chatType = ChatType.RoomChat;
            IMAPI.Instance().SendTextMessage(msg.reciver, msg.chatType, msg.textContent,ref msg.id);
            messageCahceList.Add(msg.id,msg);
            ShowMsg(msg);
            //VoiceChannelPlugin.StartTalking();

        }

        if (GUI.Button(new Rect(inset + (btnWidth + space), inset + (btnHeight + space) * 2, btnWidth, btnHeight), "SendText Message to user"))
        {
            MyMessage msg = new MyMessage();
            msg.id = 0;
            msg.textContent = "这是一条发给自己的消息";
            msg.reciver = userid.ToString();
            msg.chatType = ChatType.PrivateChat;
            IMAPI.Instance().SendTextMessage(msg.reciver, msg.chatType, msg.textContent,ref msg.id);
            messageCahceList.Add(msg.id,msg);
            ShowMsg(msg);
        }

        if (GUI.Button(new Rect(inset + (btnWidth + space) * 1, inset + (btnHeight + space) * 3, btnWidth, btnHeight), "leavechatroom"))
        {
            Debug.Log("leavechatroom");
            showStatus("leavechatroom");
            IMAPI.Instance().LeaveChatRoom("1001");
            //VoiceChannelPlugin.StartTalking();
        }

        if (GUI.Button(new Rect(inset + (btnWidth + space) * 1, inset + (btnHeight + space) * 4, btnWidth, btnHeight), "filter"))
        {
            showStatus(IMAPI.GetFilterText("这是江泽明de胡锦涛哦的法轮功的水电费水电费"));
        }

    }

    #region LoginListen implementation

    public void OnLogin(ErrorCode errorcode, string strYouMeID)
    {
        showStatus("OnLogin: errorcode" + errorcode + " userID:" + strYouMeID);
        if(errorcode == ErrorCode.Success){
            Debug.Log("登陆成功");
            if(needJoinRoom){
                needJoinRoom=false;
                IMAPI.Instance().JoinChatRoom("1001");
            }
        }
    }

    bool needJoinRoom = false;
    bool gameOnline = true; //假设游戏是在线状态
    public void OnLogout()
    {
        showStatus("OnLogout");
        if(!manualLogout && gameOnline){
            needJoinRoom=true;
            IMAPI.Instance().Login("1001", "123456");
        }
    }

    #endregion

    #region MessageListen implementation

	public void OnSendMessageStatus(ulong iRequestID,  YIMEngine.ErrorCode errorcode, bool isForbidRoom, int reasonType, ulong forbidEndTime)
    {
        MyMessage msg;
        if(messageCahceList.TryGetValue(iRequestID,out msg)){
            if(errorcode == ErrorCode.Success){
                msg.status = 1;
            }else{
                msg.status = 2;
            }
            UpdateMsg(msg);
            messageCahceList.Remove(iRequestID);
        }
        Debug.Log("OnSendMessageStatus request:" + iRequestID + "errorcode:" + errorcode);
    }
	public void OnSendAudioMessageStatus(ulong iRequestID,  YIMEngine.ErrorCode errorcode,string strText,string strAudioPath,int iDuration, bool isForbidRoom, int reasonType, ulong forbidEndTime)
    {
        Debug.Log("OnSendAudioMessageStatus request:" + iRequestID + " errorcode:" + errorcode + " 识别结果:" + strText + " path:" + strAudioPath +" 语音时长:"+iDuration);

        //如果自己发送的语音消息发送成功，就播放出来
        if(errorcode == ErrorCode.Success){
            YIMAudioPlayer.Instance.PlayAudioFile(strAudioPath);
        }
    }

	public void OnStartSendAudioMessage(ulong iRequestID, YIMEngine.ErrorCode errorcode, string strText, string strAudioPath, int iDuration)
	{
	}

    public void OnRecvMessage(MessageInfoBase message)
    {
        if (message.MessageType == MessageBodyType.TXT)
        {
            TextMessage textMsg = (TextMessage)message;
            showStatus(textMsg.Content);
            Debug.Log("OnRecvMessage text:" + textMsg.Content + " send:" + textMsg.SenderID + "recv:" + textMsg.RecvID);
        }
        else if (message.MessageType == MessageBodyType.CustomMesssage)
        {
            CustomMessage customMsg = (CustomMessage)message;
            Debug.Log("OnRecvMessage custom:" + System.Convert.ToBase64String(customMsg.Content) + " send:" + customMsg.SenderID + "recv:" + customMsg.RecvID);
        }
        else if (message.MessageType == MessageBodyType.Voice)
        {
            VoiceMessage voiceMsg = (VoiceMessage)message;
            Debug.Log("OnRecvMessage voice 文本识别结果:" + voiceMsg.Text + " send:" + voiceMsg.SenderID + "recv:" + voiceMsg.RecvID +" 时长:"+voiceMsg.Duration);

            //下载收到的语音消息
            IMAPI.Instance().DownloadAudioFile(voiceMsg.RequestID, GetUniqAudioPath());
        }
    }

    //获取到本地私聊历史纪录
    public void OnQueryHistoryMessage(YIMEngine.ErrorCode errorcode, string targetID, int remain, List <YIMEngine.HistoryMsg> messageList){
        
    }

    public void OnStopAudioSpeechStatus(YIMEngine.ErrorCode errorcode, ulong iRequestID,string strDownloadURL,int iDuration,int iFileSize,string strLocalPath,string strText){
        
    }

	//新消息通知
	public void OnRecvNewMessage(YIMEngine.ChatType chatType,string targetID)
	{
	}

	public void OnAccusationResultNotify(AccusationDealResult result, string userID, uint accusationTime)
	{
	}

	public void OnGetForbiddenSpeakInfo( YIMEngine.ErrorCode errorcode, List<ForbiddenSpeakInfo> forbiddenSpeakList )
	{
	}

    #endregion

    #region ChatRoomListen implementation

    public void OnJoinRoom(ErrorCode errorcode, string strChatRoomID)
    {
		showStatus("OnJoinRoom: errorcode" + errorcode + " roomID:" + strChatRoomID);
    }
    public void OnLeaveRoom(ErrorCode errorcode, string strChatRoomID)
    {
		showStatus("OnLeaveRoom: errorcode" + errorcode + " roomID:" + strChatRoomID);
    }

	public void OnUserJoinChatRoom(string strRoomID, string strUserID)
	{
	}

	public void OnUserLeaveChatRoom(string strRoomID, string strUserID)
	{
	}

    #endregion

    public void OnGetContact(List<string> contactLists){
        
    }

	public void OnGetUserInfo(ErrorCode code, string userID, IMUserInfo userInfo)
	{
	}

	public void OnQueryUserStatus(ErrorCode code, string userID, UserStatus status)
	{
	}

	public void OnDownload( YIMEngine.ErrorCode errorcode, YIMEngine.MessageInfoBase message, string strSavePath)
	{
		//如果下载收到的语音消息成功，就播放
		if(errorcode == ErrorCode.Success){
			YIMAudioPlayer.Instance.PlayAudioFile(strSavePath);
		}
	}

	public void OnDownloadByUrl( YIMEngine.ErrorCode errorcode, string strFromUrl, string strSavePath )
	{
	}

	#region YIMEngine.NoticeListen implementation
	public  void OnRecvNotice(YIMEngine.Notice notice)
	{
	}
	public void OnCancelNotice(ulong noticeID, string channelID)
	{
	}

	#endregion

    void UpdateMsg(MyMessage msg){
        //这个函数是假设用来更新消息的发送是否成功的状态
    }

    void ShowMsg(MyMessage msg){
        //这个函数是假设用来添加消息到UI上
        showStatus(msg.textContent);
    }

    string GetUniqAudioPath()
    {
        return Application.temporaryCachePath + "/YoumeIMAudioCache/"+ System.Guid.NewGuid().ToString().Replace("-", "") + ".wav";
    }

    void OnApplicationQuit(){
        Debug.LogError("Quit");
        #if UNITY_EDITOR
        manualLogout = true;
        IMAPI.Instance().Logout();
        #else
        IMAPI.Instance().UnInit();
        #endif
    }

}
