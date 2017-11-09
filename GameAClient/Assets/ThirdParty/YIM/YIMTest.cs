using UnityEngine;
using System.Collections.Generic;
using YIMEngine;

namespace U3dTest
{
    public class YIMTest :
        LoginListen,
        MessageListen,
        ChatRoomListen,
        AudioPlayListen,
        ContactListen,
        LocationListen
    {

        private Vector2 m_Position = Vector2.zero;
        private string m_InGameLog = "";
        private const string voicePath = "/sdcard/abc.wav";
        void Init()
        {
            IMAPI.Instance().SetLoginListen(this);
            IMAPI.Instance().SetMessageListen(this);
            IMAPI.Instance().SetChatRoomListen(this);
            IMAPI.Instance().SetAudioPlayListen(this);
            IMAPI.Instance().SetContactListen(this);
            IMAPI.Instance().SetLocationListen(this);
			IMAPI.Instance().Init("YOUMEE9036C7A45B37ED5F7AC6F0C6479BB48EC2CA10E",
			                                "y7/AAtmf6Ry6awljGzwbs0x6VFPQcckKlpHQCno1KiA3YEu1g5UwLPbVxBEnSLGfbPDfE8Qj/qYp1shwQVptV3uvmVsueIaHKrzZ+uOY715kpAnNGIYEQsp7Pi0QV07/Ii3z9oRShJKdj/I7qWM9UYF0+pJxkCnAgkR8SD15tuUBAAE=");
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
            int labelY = inset + (btnHeight + space) * 8;
            int labelWidth = Screen.width - labelX * 2;
            int labelHeight = Screen.height - labelY;

            GUI.BeginGroup(new Rect(labelX, labelY, labelWidth, labelHeight));

            m_Position = GUILayout.BeginScrollView(m_Position, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));
            GUILayout.Label(m_InGameLog);
            GUILayout.EndScrollView();

            GUI.EndGroup();

            if (GUI.Button(new Rect(inset, inset, btnWidth, btnHeight), "login"))
            {
                ErrorCode errorcode = IMAPI.Instance().Login("123", "123456");
                Debug.Log("login errorcode: " + errorcode);
            }

            if (GUI.Button(new Rect(inset + btnWidth + space, inset, btnWidth, btnHeight), "logout"))
            {
                Debug.Log("logout");
                showStatus("logout");
                IMAPI.Instance().Logout();
            }

            if (GUI.Button(new Rect(inset, inset + btnHeight + space, btnWidth, btnHeight), "init"))
            {
                Debug.Log("init");
                showStatus("init");
                IMAPI.Instance().Init("YOUMEBC2B3171A7A165DC10918A7B50A4B939F2A187D0", "r1+ih9rvMEDD3jUoU+nj8C7VljQr7Tuk4TtcByIdyAqjdl5lhlESU0D+SoRZ30sopoaOBg9EsiIMdc8R16WpJPNwLYx2WDT5hI/HsLl1NJjQfa9ZPuz7c/xVb8GHJlMf/wtmuog3bHCpuninqsm3DRWiZZugBTEj2ryrhK7oZncBAAE=");
            }

            if (GUI.Button(new Rect(inset + btnWidth + space, inset + btnHeight + space, btnWidth, btnHeight), "uninit"))
            {
                Debug.Log("uninit");
                showStatus("uninit");
                IMAPI.Instance().UnInit();
            }

            if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset + (btnHeight + space) * 2, btnWidth, btnHeight), "sendmessage"))
            {
				ulong iRequestID = 0;
				IMAPI.Instance().SendTextMessage("777", ChatType.RoomChat, "my message",ref iRequestID);

               
				ErrorCode errorcode = IMAPI.Instance().SendAudioMessage("777", ChatType.RoomChat, ref iRequestID);
                Debug.Log("sendmessage: RequestID:" + iRequestID + "errorcode: " + errorcode);

            }

            if (GUI.Button(new Rect(inset, inset + (btnHeight + space) * 3, btnWidth, btnHeight), "joinchatroom"))
            {
                Debug.Log("joinchatroom");
                IMAPI.Instance().JoinChatRoom("1001");
            }

            if (GUI.Button(new Rect(inset + btnWidth + space, inset + (btnHeight + space) * 3, btnWidth, btnHeight), "leavechatroom"))
            {
                Debug.Log("leavechatroom");
                showStatus("leavechatroom");
                IMAPI.Instance().LeaveChatRoom("1001");
            }

            if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset + (btnHeight + space) * 3, btnWidth, btnHeight), "sendcustommessage"))
            {
                Debug.Log("sendcustommessage");
                showStatus("sendcustommessage");
				ulong iRequestID = 0;
                string strText = "112345";

                ErrorCode errorcode = IMAPI.Instance().SendCustomMessage("1001", ChatType.PrivateChat, System.Text.Encoding.UTF8.GetBytes(strText), ref iRequestID);
                Debug.Log("sendmessage: RequestID:" + iRequestID + "errorcode: " + errorcode);
            }

            if (GUI.Button(new Rect(inset, inset + (btnHeight + space) * 4, btnWidth, btnHeight), "Clear"))
            {
                m_InGameLog = "";
            }

            if (GUI.Button(new Rect(inset + btnWidth, inset + (btnHeight + space) * 4, btnWidth, btnHeight), "StopAudio"))
            {
                IMAPI.Instance().StopAudioMessage("");
            }

            if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset + (btnHeight + space) * 4, btnWidth, btnHeight), "filter"))
            {
                int level = 0;
                string strResult = IMAPI.GetFilterText("这是江泽明de胡锦涛哦的法轮功的水电费水电费", ref level);
                showStatus("result:" + strResult + " level:" + level);
            }

            if (GUI.Button(new Rect(inset, inset + (btnHeight + space) * 5, btnWidth, btnHeight), "QueryUserStatus"))
            {
                IMAPI.Instance().QueryUserStatus("1001");
                Debug.Log("QueryUserStatus 1001");
            }

            if (GUI.Button(new Rect(inset + btnWidth + space, inset + (btnHeight + space) * 5, btnWidth, btnHeight), "GetAudioCache"))
            {
                string strPath = IMAPI.Instance().GetAudioCachePath();
                Debug.Log("audio cache path:" + strPath);
            }

            if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset + (btnHeight + space) * 5, btnWidth, btnHeight), "PlayAudio"))
            {
                string path = "E:\\test\\bd_1.wav";
                ErrorCode errorcode = IMAPI.Instance().StartPlayAudio(path);
                Debug.Log("errorcode:" + errorcode + " path:" + path);
            }

			if (GUI.Button(new Rect(inset + (btnWidth + space) * 2, inset + (btnHeight + space) * 6, btnWidth, btnHeight), "GetForbid"))
			{
				ErrorCode errorcode = IMAPI.Instance().GetForbiddenSpeakInfo();
				Debug.Log("errorcode:" + errorcode);
			}
		}

        public void LogIn()
        {
            ErrorCode errorcode = IMAPI.Instance().Login("123", "123456");
            Debug.Log("login errorcode: " + errorcode);
        }

        public void SendTextMessage()
        {
            ulong iRequestID = 0;
            IMAPI.Instance().SendTextMessage("777", ChatType.RoomChat, "my message",ref iRequestID);
        }
        
        public void SendVoiceMessage()
        {
            ulong iRequestID = 0;
            ErrorCode errorcode = IMAPI.Instance().SendAudioMessage("777", ChatType.RoomChat, ref iRequestID);
            Debug.Log("sendmessage: RequestID:" + iRequestID + "errorcode: " + errorcode);
        }

        public void StopVoiceMessage()
        {
            IMAPI.Instance().StopAudioMessage("");
        }

        public void PlayVoice()
        {
            string path = "E:\\test\\bd_1.wav";
            ErrorCode errorcode = IMAPI.Instance().StartPlayAudio(voicePath);
            Debug.Log("errorcode:" + errorcode + " path:" + path);
        }
		
		#region LoginListen implementation
		
		public void OnLogin(ErrorCode errorcode, string strYouMeID)
        {
            Debug.Log ("OnLogin: errorcode" + errorcode + " userid:" + strYouMeID);
            showStatus("OnLogin: errorcode" + errorcode + " contact:" + strYouMeID);
        }

        public void OnLogout()
        {
            showStatus("OnLogout");
        }

        #endregion

        #region MessageListen implementation
		//获取消息历史纪录回调
		public void OnQueryHistoryMessage(ErrorCode errorcode, string targetID, int remain, List <HistoryMsg> messageList)
		{
		}
            
		public void OnSendMessageStatus(ulong iRequestID, ErrorCode errorcode, bool isForbidRoom, int reasonType, ulong forbidEndTime)
		{
			Debug.Log("OnSendMessageStatus request:" + iRequestID + "errorcode:" + errorcode);
			Debug.Log ("forbid:" + isForbidRoom + "," + reasonType + "," + forbidEndTime);
        }
        public void OnStartSendAudioMessage(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration)
        {
            Debug.Log("OnStopSendAudioMessage request:" + iRequestID + "errorcode:" + errorcode);
        }
		public void OnSendAudioMessageStatus(ulong iRequestID, ErrorCode errorcode, string strText, string strAudioPath, int iDuration, bool isForbidRoom, int reasonType, ulong forbidEndTime)
		{
			Debug.Log("OnSendAudioMessageStatus request:" + iRequestID + "errorcode:" + errorcode + " text:" + strText + " path:" + strAudioPath);
			Debug.Log ("forbid:" + isForbidRoom + "," + reasonType + "," + forbidEndTime);
        }
        public void OnStopAudioSpeechStatus(ErrorCode errorcode, ulong iRequestID,string strDownloadURL,int iDuration,int iFileSize,string strLocalPath,string strText)
        {

        }
        public void OnRecvMessage(MessageInfoBase message)
        {
            if (message.MessageType == MessageBodyType.TXT)
            {
                TextMessage textMsg = (TextMessage)message;
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
                Debug.Log("OnRecvMessage voice:" + voiceMsg.Text + " send:" + voiceMsg.SenderID + "recv:" + voiceMsg.RecvID);
                IMAPI.Instance().DownloadAudioFile(voiceMsg.RequestID, voicePath);
            }
        }
        public void OnRecvNewMessage(ChatType chatType,string targetID)
		{

		}
        public void OnTranslateTextComplete(ErrorCode errorcode, uint requestID, string text, LanguageCode destLangCode)
        {

        }

		public void OnGetForbiddenSpeakInfo( ErrorCode errorcode, List<ForbiddenSpeakInfo> forbiddenSpeakList )
		{
			Debug.Log ("OnGetForbiddenSpeakInfo:" + errorcode + "," + forbiddenSpeakList.Count);
			for (int i = 0; i < forbiddenSpeakList.Count; i++) {
				ForbiddenSpeakInfo info = forbiddenSpeakList[i];
				Debug.Log("jinyan:" +  info.ChannelID + "," + info.IsForbidRoom + "," + info.ReasonType + "," + info.EndTime );
			}
		}

        #endregion

        #region ChatRoomListen implementation

        public void OnJoinRoom(ErrorCode errorcode, string strChatRoomID)
        {

        }
        public void OnLeaveRoom(ErrorCode errorcode, string strChatRoomID)
        {
        }
        public void OnUserJoinChatRoom(string strRoomID, string strUserID)
        {
        }
        public void OnUserLeaveChatRoom(string strRoomID, string strUserID)
        {
        }
        public void OnAccusationResultNotify(AccusationDealResult result, string userID, uint accusationTime)
        {

        }
        #endregion

        #region AudioPlayListen implementation

        public void OnPlayCompletion(ErrorCode errorcode, string path)
        {
            Debug.Log("play audio done errorcode:" + errorcode);
        }
        public void OnGetMicrophoneStatus(AudioDeviceStatus status)
        {

        }
        public void OnDownload( ErrorCode errorcode, MessageInfoBase message, string strSavePath)
        {
            //如果下载收到的语音消息成功，就播放
            if(errorcode == ErrorCode.Success){
                YIMAudioPlayer.Instance.PlayAudioFile(strSavePath);
            }
        }
        #endregion

        #region ContactListen implementation

        public void OnGetContact(List<string> contactLists)
        {

        }

        public void OnGetUserInfo(ErrorCode code, string userID, IMUserInfo userInfo)
        {
            Debug.Log("OnGetUserInfo code:" + code + " userInfo: " + userInfo.ToJsonString());
        }

        public void OnQueryUserStatus(ErrorCode code, string userID, UserStatus status)
        {
            Debug.Log("OnQueryUserStatus code:" + code + " userID: " + userID + " status:" + status);
        }

        #endregion


        #region LocationListen implementation

        public void OnUpdateLocation(ErrorCode errorcode, GeographyLocation location)
        {

        }

        public void OnGetNearbyObjects(ErrorCode errorcode, List<RelativeLocation> neighbourList, uint startDistance, uint endDistance)
        {

        }

        #endregion
        
    }
}
