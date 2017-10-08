using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SoyEngine.LitJson;

#if UNITY_IOS && !UNITY_EDITOR
using AOT;
#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

namespace YIMEngine{
	
	public class IMAPI
	{
		//所有的C接口的导出		
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_Init([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strAppKey,[MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strAppSecrect);

		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern void IM_Uninit();


		//login
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_Login([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strYouMeID,[MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
		)] string strPasswd,
		[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
		UnmanagedType.LPTStr
		#endif
		)] string strToken);

		//logout
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_Logout();


	
		//send text message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SendTextMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string  strRecvID,int iChatType,[MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
		)] string strContent,ref ulong iRequestID);

		//send custom message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SendCustomMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
		)] string  strRecvID,int iChatType,[MarshalAs(UnmanagedType.LPArray)] byte[] buffer ,int bufferLen, ref ulong iRequestID);

		//send audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SendAudioMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string  strRecvID,int iChatType,ref ulong iRequestID);

		//send audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SendOnlyAudioMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string  strRecvID,int iChatType,ref ulong iRequestID);



		//send audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SendFile([MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string  strRecvID,int iChatType,[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string  strFilePath,[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string  strExtParam,int iFileType, ref ulong iRequestID);


		//stop audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_StopAudioMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strParam);

		//cancle audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_CancleAudioMessage();


		//download audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_DownloadFile(ulong iSerial,[MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strSavePath);

		//download audio message
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_DownloadFileByURL(
			[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
			UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string strFromUrl,
		[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string strSavePath);


		//join chatroom
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_JoinChatRoom([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string  strChatRoomID);

		//leave chatroom
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_LeaveChatRoom([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strChatRoomID);



		//new  start audiospeech
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_StartAudioSpeech(ref ulong iRequestID,bool translate);

		//start stopspeech
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_StopAudioSpeech();


		//queryhistory 
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_QueryHistoryMessage([MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
					UnmanagedType.LPTStr
		#endif
		)] string targetID,int chatType,ulong MessageID,int count,int directioin);

		//start deletehistory
		#if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
		#else
			[DllImport("yim")]
		#endif
        private static extern int IM_DeleteHistoryMessage(ChatType chatType, ulong time);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_DeleteHistoryMessageByID(ulong messageID);

		//queryroom message 
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_QueryRoomHistoryMessageFromServer([MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
					UnmanagedType.LPTStr
		#endif
				)] string targetID);


		//convert amr to wav 
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_ConvertAMRToWav([MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
					UnmanagedType.LPTStr
		#endif
		)] string strSrcPath,[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string strSrcDestPath);



		//发送礼物
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_SendGift([MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
					UnmanagedType.LPTStr
		#endif
				)] string strRecvID,[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
					UnmanagedType.LPTStr
		#endif
		)] string strChannel,int iGiftID,int iGiftCount,
			[MarshalAs(
				UnmanagedType.LPStr
			)] string strExtParam,ref ulong iRequestID);




		//sdkver
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_GetSDKVer();


		//getmessget 
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern System.IntPtr IM_GetMessage();


		//getmessget 
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern System.IntPtr IM_GetMessage2();


		//popmessage
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern void IM_PopMessage(System.IntPtr pBuffer);



		//getfiltertext
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern System.IntPtr IM_GetFilterText([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string strSource, ref int level);

		//set cache path for audio file
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern void IM_SetAudioCacheDir([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string cachePath);
		
		//destroyfiltertext
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern void IM_DestroyFilterText(System.IntPtr pBuffer);

		//set mode
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern void IM_SetMode(int iMode);//设置模式0 正式环境 1开发环境 2 测试环境 3 商务环境。 默认正式环境。所以客户不需要调用这个接口

        #if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport("__Internal")]
        #else
        [DllImport("yim")]
        #endif
        private static extern void IM_SetServerZone(int zone);


		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern void IM_OnPause();

		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern void IM_OnResume();





		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
				[DllImport("yim")]
		#endif
		private static extern int IM_MultiSendTextMessage(
			[MarshalAs(
				UnmanagedType.LPStr
			)] string strReceives,

			[MarshalAs(
		#if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
		UnmanagedType.LPWStr 
		#else
			UnmanagedType.LPTStr
		#endif
		)] string strText);


		//getcontact
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SetReceiveMessageSwitch([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
                UnmanagedType.LPTStr
        #endif
        )] string targets, bool bAutoRecv);


		//getcontact
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
        private static extern int IM_GetNewMessage([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string targets);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
        [DllImport("yim")]
        #endif
        private static extern int IM_TranslateText(ref uint requestID, [MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string text, LanguageCode destLangCode, LanguageCode srcLangCode);


		//getcontact
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_GetRecentContacts();

		//getcontact
		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_SetUserInfo([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string userInfo);

		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_GetUserInfo([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
        UnmanagedType.LPWStr 
        #else
        UnmanagedType.LPTStr
        #endif
        )] string userID);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
		#else
		    [DllImport("yim")]
		#endif
        private static extern int IM_QueryUserStatus([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
                UnmanagedType.LPTStr
        #endif
        )] string userID);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern void IM_SetVolume(float volume);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_StartPlayAudio([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string path);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_StopPlayAudio();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern bool IM_IsPlaying();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_SetRoomHistoryMessageSwitch([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
                UnmanagedType.LPTStr
        #endif
        )] string roomID, bool save);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
            private static extern System.IntPtr IM_GetAudioCachePath();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern void IM_DestroyAudioCachePath(System.IntPtr pBuffer);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern bool IM_ClearAudioCachePath();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_GetCurrentLocation();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_GetNearbyObjects(int count, [MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string serverAreaID, YIMEngine.DistrictLevel districtlevel, bool resetStartDistance);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern void IM_SetUpdateInterval(uint interval);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern void IM_SetKeepRecordModel(bool keep);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_SetSpeechRecognizeParam(SpeechAccent accent, SpeechRecognizeLanguage language);

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_GetMicrophoneStatus();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
	    private static extern int IM_Accusation([MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string userID, ChatType source, int reason, [MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string description, [MarshalAs(
        #if (UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN
            UnmanagedType.LPWStr
        #else
            UnmanagedType.LPTStr
        #endif
        )] string extraParam);
        
        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
	    private static extern int IM_QueryNotice();

        #if UNITY_IPHONE && !UNITY_EDITOR
		    [DllImport("__Internal")]
        #else
            [DllImport("yim")]
        #endif
        private static extern int IM_SetMessageRead(ulong messageID, bool read);

		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal")]
		#else
		[DllImport("yim")]
		#endif
		private static extern int IM_GetForbiddenSpeakInfo();


		/****************************************************************************************/
		private static IMAPI s_Instance = null;
		private LoginListen m_loginListen;
		private MessageListen m_messageListen;
		private ChatRoomListen m_groupListen;
		private DownloadListen m_downloadListen;
		private ContactListen m_contactListen;
        private AudioPlayListen m_audioPlayListen;
        private LocationListen m_locationListen;
        private NoticeListen m_noticeListen;

        //tranlate callback quen
        private Dictionary<uint, System.Action<ErrorCode,string, LanguageCode, LanguageCode>> tranlateCallbackQuen = new Dictionary <uint, System.Action<ErrorCode,string, LanguageCode, LanguageCode>>();

        public static IMAPI Instance()
		{
			if (s_Instance == null) 
			{
				s_Instance = new IMAPI();
			}
			return s_Instance;
		}

		public static int GetSDKVer()
		{
			return IM_GetSDKVer ();
		}
		public void SetLoginListen(LoginListen listen)
		{
			m_loginListen = listen;
		}

	
		public void SetMessageListen(MessageListen listen)
		{
			m_messageListen = listen;
		}
		public void SetChatRoomListen(ChatRoomListen listen)
		{
			m_groupListen = listen;
		}
		public void SetDownloadListen(DownloadListen listen)
		{
			m_downloadListen = listen;
		}
		public void SetContactListen(ContactListen listen)
		{
			m_contactListen = listen;
		}
        public void SetAudioPlayListen(AudioPlayListen listen)
        {
            m_audioPlayListen = listen;
        }
        public void SetLocationListen(LocationListen listen)
        {
            m_locationListen = listen;
        }
        public void SetNoticeListen(NoticeListen listen)
        {
            m_noticeListen = listen;
        }

		#if ((UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN) && (UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_8)
		[DllImport("msc")]
		public static extern int MSPLogin(string usr, string pwd, string _params);
		#endif

		//api 
		public static bool inited=false;
		public ErrorCode Init(string strAppKey,string strSecrect)
		{
			#if !UNITY_EDITOR && UNITY_ANDROID
				if(!inited){
					inited =true;
					AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
					AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
					AndroidJavaClass YouMeManager = new AndroidJavaClass ("com.youme.im.IMEngine");
					YouMeManager.CallStatic ("init", currentActivity);
				}
			#elif ((UNITY_STANDALONE_WIN && ! UNITY_EDITOR_OSX) || UNITY_EDITOR_WIN) && (UNITY_4_7 || UNITY_4_6 || UNITY_4_5 || UNITY_4_8)
			if(!inited){
				inited =true;
				string dllPath =  Application.dataPath+"\\Plugins"+"\\msc.dll";
				if(System.IO.File.Exists(dllPath)){
					MSPLogin("","","");
				}
			}
			#endif
//			Debug.Log ("start init");

			ErrorCode code = (ErrorCode)IM_Init (strAppKey, strSecrect);
            if(code == ErrorCode.Success){
				GameObject yimUpdateObj = new GameObject ("youme_update_once");
				GameObject.DontDestroyOnLoad (yimUpdateObj);
				yimUpdateObj.hideFlags = HideFlags.HideInHierarchy;
				yimUpdateObj.AddComponent <YIMUpdateObject>();

            }
            return code;
		}

		public int SetAutoRecvMsg(List<string> targets, bool bAutoRecv)
		{
			JsonData recvJsonArray = new JsonData ();
			for (int i = 0; i < targets.Count; i++) {
				recvJsonArray.Add (targets [i]);	
			}
            return IM_SetReceiveMessageSwitch(recvJsonArray.ToJson(), bAutoRecv);
		}

        // 是否保存房间消息记录
        public void SetRoomHistoryMessageSwitch(List<string> roomIDs,bool save)
        {
			JsonData recvJsonArray = new JsonData ();
            for (int i = 0; i < roomIDs.Count; i++) {
                recvJsonArray.Add (roomIDs [i]);    
            }
            IM_SetRoomHistoryMessageSwitch(recvJsonArray.ToJson(), save);
        }

        public ErrorCode GetNewMessage(List<string> targets)
		{
            JsonData recvJsonArray = new JsonData ();
            for (int i = 0; i < targets.Count; i++) {
                recvJsonArray.Add (targets [i]);    
            }
            return (ErrorCode)IM_GetNewMessage (recvJsonArray.ToJson());
		}

		private class YIMUpdateObject :MonoBehaviour{

			void Start() {
				InvokeRepeating("YIMUpdate", 0.5f, 0.05f);
			}

			void YIMUpdate ()
			{				
				System.IntPtr pBuffer = IM_GetMessage2();
				if( pBuffer == System.IntPtr.Zero ){
					return;
				}
				string strMessage = Marshal.PtrToStringAuto(pBuffer);
				// Debug.Log("recv message:" + strMessage);
				if(null != strMessage)
				{
					try{
						IMAPI.Instance().ParseJsonMessageCallback(strMessage);
					}catch(System.Exception e){
						Debug.LogError(e.StackTrace);
					}
				}

				IM_PopMessage(pBuffer);
			}

		}

		public void UnInit()
		{
			#if UNITY_EDITOR
            Logout();
			#else
			IM_Uninit ();
			#endif
		}

		//statci 
        public static string GetFilterText(string strSource, ref int level)
		{
            System.IntPtr pBuffer = IM_GetFilterText(strSource, ref level);
			if( pBuffer == System.IntPtr.Zero ){
				return strSource;
			}
			string strMessage = Marshal.PtrToStringAuto(pBuffer);
			IM_DestroyFilterText (pBuffer);
			return strMessage;
		}

		public static string GetFilterText(string strSource)
		{
            int level = 0;
            System.IntPtr pBuffer = IM_GetFilterText(strSource, ref level);
			if( pBuffer == System.IntPtr.Zero ){
				return strSource;
			}
			string strMessage = Marshal.PtrToStringAuto(pBuffer);
			IM_DestroyFilterText (pBuffer);
			return strMessage;
		}


		void ParseJsonMessageCallback(string strMessage)
		{
			JsonData jsonMessage =  JsonMapper.ToObject (strMessage);
			Command command = (Command)(int)jsonMessage ["Command"];
			ErrorCode errorcode = (ErrorCode)(int)jsonMessage ["Errorcode"];
			switch (command) {
			case Command.CMD_RECEIVE_MESSAGE_NITIFY:
				{
					if (null != m_messageListen) {
                        ChatType chatType = (ChatType)(int)jsonMessage ["ChatType"];
                        string targetID = (string)jsonMessage ["TargetID"];
                        m_messageListen.OnRecvNewMessage (chatType,targetID);
					}
				}
				break;
			case Command.CMD_GET_RENCENT_CONTACTS:
				{
					List<string> contactLists = new List<string> ();
					JsonData contactJson = jsonMessage ["contacts"];
					for (int i = 0; i < contactJson.Count; i++) {
						contactLists.Add ((string)contactJson [i]);
					}
					if (null != m_contactListen) {
						m_contactListen.OnGetContact (contactLists);
					}
				}
				break;
			case Command.CMD_QUERY_HISTORY_MESSAGE:
				{
					int iRemainCount =(int) jsonMessage["Remain"];
					string strTargetID = (string)jsonMessage["TargetID"];
					JsonData msgListJson = jsonMessage["messageList"];
					List<YIMEngine.HistoryMsg> hisoryLists=new List<YIMEngine.HistoryMsg>();
					for (int i = 0; i < msgListJson.Count; i++)
					{
						YIMEngine.HistoryMsg pMsg=new HistoryMsg();
						JsonData msg = msgListJson[i];

						pMsg.ChatType =(ChatType)(int) msg["ChatType"];
						pMsg.MessageType = (MessageBodyType)(int)msg["MessageType"];
						pMsg.ReceiveID = (string)msg["ReceiveID"];
						pMsg.SenderID = (string)msg["SenderID"];
						pMsg.MessageID =ulong.Parse(msg["Serial"].ToString());
						pMsg.CreateTime = (int)msg ["CreateTime"];
                        if (msg.Keys.Contains("IsRead"))
                        {
                            pMsg.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                        }
						if (pMsg.MessageType == MessageBodyType.TXT) {
							pMsg.Text = (string)msg ["Content"];
						} else if (pMsg.MessageType == MessageBodyType.CustomMesssage) {
							//base64
							pMsg.Text = (string)msg ["Content"];
						} else if (pMsg.MessageType == MessageBodyType.Voice) {
							pMsg.Text =(string) msg["Text"];
							pMsg.LocalPath = (string)msg ["LocalPath"];
							pMsg.Duration = (int)msg["Duration"];
							pMsg.Param = (string)msg["Param"];
						} else if (pMsg.MessageType == MessageBodyType.File) { 
							pMsg.LocalPath = (string)msg ["LocalPath"];
							pMsg.Param = (string)msg["Param"];
						}

                            hisoryLists.Add(pMsg);
					}
					if (null != m_messageListen) {
						m_messageListen.OnQueryHistoryMessage(errorcode,strTargetID,iRemainCount,hisoryLists);
					}
				}
				break;
			case Command.CMD_STOP_AUDIOSPEECH:
				{
					string strDownloadURL = (string)jsonMessage ["DownloadURL"];
					int iDuration = (int)jsonMessage["Duration"];
					int iFileSize = (int)jsonMessage["FileSize"];
					string strLocalPath = (string)jsonMessage["LocalPath"];
					string strRequestID = (string)jsonMessage["RequestID"];
					string strText = (string)jsonMessage["Text"];
					if (null != m_messageListen) {
						m_messageListen.OnStopAudioSpeechStatus (errorcode, ulong.Parse (strRequestID), strDownloadURL, iDuration, iFileSize, strLocalPath, strText);
					}
				}
				break;
			case Command.CMD_DOWNLOAD:
				{
					if(null != m_downloadListen)
					{
						string strSavePath = (string)jsonMessage["SavePath"];

						MessageBodyType bodyType = (MessageBodyType)(int)jsonMessage["MessageType"];
						if (bodyType == MessageBodyType.Voice) {
							VoiceMessage message = new VoiceMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.Text = (string)jsonMessage ["Text"];
							message.Param = (string)jsonMessage ["Param"];
							message.Duration = (int)jsonMessage ["Duration"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
							if (jsonMessage.Keys.Contains ("Distance")) {
								message.Distance = (uint)(int)jsonMessage ["Distance"];
							}
                            if (jsonMessage.Keys.Contains("IsRead")){
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }
							m_downloadListen.OnDownload (errorcode, message, strSavePath);
						} else if (bodyType == MessageBodyType.File) {
							FileMessage message = new FileMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.FileName = (string)jsonMessage ["FileName"];
							message.FileSize = (int)jsonMessage ["FileSize"];
							message.FileType = (FileType)(int)jsonMessage ["FileType"];
							message.FileExtension = (string)jsonMessage ["FileExtension"];
							message.ExtParam = (string)jsonMessage ["ExtraParam"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
							if (jsonMessage.Keys.Contains ("Distance")) {
								message.Distance = (uint)(int)jsonMessage ["Distance"];
							}
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }

							m_downloadListen.OnDownload (errorcode, message, strSavePath);
						}
					}
				}
				break;
			case Command.CMD_DOWNLOAD_URL:
				{
					if (null != m_downloadListen) {
						string strFromUrl = (string)jsonMessage["FromUrl"];
						string strSavePath = (string)jsonMessage["SavePath"];
						m_downloadListen.OnDownloadByUrl( errorcode, strFromUrl, strSavePath );
					}
				}
				break;
			case Command.CMD_LOGIN:
				{
					if(m_loginListen != null)
					{
						m_loginListen.OnLogin(errorcode,(string)jsonMessage ["UserID"]);
					}
				}
				break;
			case Command.CMD_LOGOUT:
				{
					if(m_loginListen != null)
					{
						m_loginListen.OnLogout();
					}
				}
				break;

			case Command.CMD_SEND_MESSAGE_STATUS:
				{
					if(null != m_messageListen)
					{
						m_messageListen.OnSendMessageStatus(ulong.Parse(jsonMessage["RequestID"].ToString()),errorcode, 
					                                    ((int)jsonMessage["IsForbidRoom"] == 0 ? false : true ), 
					                                    (int)jsonMessage["reasonType"],
					                                    ulong.Parse(jsonMessage["forbidEndTime"].ToString()));
					}
				}
				break;
			case Command.CMD_SND_VOICE_MSG:
			{
				if(null != m_messageListen)
				{
					string strText = (string)jsonMessage["Text"];
					string strLocalPath = (string)jsonMessage["LocalPath"];
					int iDuration = (int)jsonMessage["Duration"];
						m_messageListen.OnSendAudioMessageStatus(ulong.Parse(jsonMessage["RequestID"].ToString()),errorcode,strText,strLocalPath,iDuration,
					                                         ((int)jsonMessage["IsForbidRoom"] == 0 ? false : true) , 
					                                         (int)jsonMessage["reasonType"],
					                                         ulong.Parse(jsonMessage["forbidEndTime"].ToString()));
				}
			}
				break;
            case Command.CMD_STOP_SEND_AUDIO:
                {
                    if (null != m_messageListen)
                    {
                        string strText = (string)jsonMessage["Text"];
                        string strLocalPath = (string)jsonMessage["LocalPath"];
                        int iDuration = (int)jsonMessage["Duration"];
                        m_messageListen.OnStartSendAudioMessage(ulong.Parse(jsonMessage["RequestID"].ToString()), errorcode, strText, strLocalPath, iDuration);
                    }
                }
                break;
			case Command.CMD_GET_USR_INFO:
				{
					if(null != m_contactListen)
					{
                        string strUserID = (string)jsonMessage["UserID"];
						string strUserInfo = (string)jsonMessage["UserInfo"];
                        m_contactListen.OnGetUserInfo(errorcode, strUserID, new IMUserInfo().ParseFromJsonString(strUserInfo));
                    }
				}
                break;
            case Command.CMD_RECV_MESSAGE:
			{
				if(null != m_messageListen)
				{
					MessageBodyType bodyType = (MessageBodyType)(int)jsonMessage["MessageType"];
						if (bodyType == MessageBodyType.TXT) {
							TextMessage message = new TextMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.Content = (string)jsonMessage ["Content"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
                            if (jsonMessage.Keys.Contains("Distance"))
                            {
                                message.Distance = (uint)(int)jsonMessage["Distance"];
                            }
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }
							m_messageListen.OnRecvMessage (message);
						} else if (bodyType == MessageBodyType.CustomMesssage) {
							CustomMessage message = new CustomMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							string strBase64Content = (string)jsonMessage ["Content"];
							message.Content = System.Convert.FromBase64String (strBase64Content);
							message.CreateTime = (int)jsonMessage ["CreateTime"];
                            if (jsonMessage.Keys.Contains("Distance"))
                            {
                                message.Distance = (uint)(int)jsonMessage["Distance"];
                            }
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }
							m_messageListen.OnRecvMessage (message);
						} else if (bodyType == MessageBodyType.Voice) {
							VoiceMessage message = new VoiceMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.Text = (string)jsonMessage ["Text"];
							message.Param = (string)jsonMessage ["Param"];
							message.Duration = (int)jsonMessage ["Duration"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
                            if (jsonMessage.Keys.Contains("Distance"))
                            {
                                message.Distance = (uint)(int)jsonMessage["Distance"];
                            }
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }
							m_messageListen.OnRecvMessage (message);
						} else if (bodyType == MessageBodyType.Gift) {
							GiftMessage message = new GiftMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
                            if (jsonMessage.Keys.Contains("Distance"))
                            {
                                message.Distance = (uint)(int)jsonMessage["Distance"];
                            }
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }
							message.ExtParam = new ExtraGifParam ().ParseFromJsonString ((string)jsonMessage ["Param"]);
							message.GiftID = (int)jsonMessage ["GiftID"];
							message.GiftCount = (int)jsonMessage ["GiftCount"];
							message.Anchor = (string)jsonMessage ["Anchor"];

							m_messageListen.OnRecvMessage (message);
						} else if (bodyType == MessageBodyType.File) 
						{
							FileMessage message = new FileMessage ();
							message.ChatType = (ChatType)(int)jsonMessage ["ChatType"];
							message.RequestID = ulong.Parse (jsonMessage ["Serial"].ToString ());
							message.MessageType = bodyType;
							message.RecvID = (string)jsonMessage ["ReceiveID"];
							message.SenderID = (string)jsonMessage ["SenderID"];
							message.FileName = (string)jsonMessage ["FileName"];
							message.FileSize = (int)jsonMessage ["FileSize"];
							message.FileType = (FileType)(int)jsonMessage ["FileType"];
							message.FileExtension = (string)jsonMessage ["FileExtension"];
							message.ExtParam = (string)jsonMessage ["ExtraParam"];
							message.CreateTime = (int)jsonMessage ["CreateTime"];
                            if (jsonMessage.Keys.Contains("Distance"))
                            {
                                message.Distance = (uint)(int)jsonMessage["Distance"];
                            }
                            if (jsonMessage.Keys.Contains("IsRead"))
                            {
                                message.IsRead = (int)jsonMessage["IsRead"] == 1 ? true : false;
                            }

							m_messageListen.OnRecvMessage (message);
						}
				}
			}
				break;
	
			case Command.CMD_ENTER_ROOM:
			{
				if(null != m_groupListen)
				{
					string iChatRoomID = (string)jsonMessage["GroupID"];
					// GroupEvent evtType  = (GroupEvent)(int)jsonMessage["GroupEvt"];
					m_groupListen.OnJoinRoom(errorcode,iChatRoomID);
				}
			}
			break;
			case Command.CMD_LEAVE_ROOM:
				{
					if(null != m_groupListen)
					{
						string iChatRoomID = (string)jsonMessage["GroupID"];
						m_groupListen.OnLeaveRoom(errorcode,iChatRoomID);
					}
				}
				break;
            case Command.CMD_USER_ENTER_ROOM:
                {
                    string strChannelID = (string)jsonMessage["ChannelID"];
                    string strUserID = (string)jsonMessage["UserID"];
                    m_groupListen.OnUserJoinChatRoom(strChannelID, strUserID);
                }
                break;
            case Command.CMD_USER_LEAVE_ROOM:
                {
                    string strChannelID = (string)jsonMessage["ChannelID"];
                    string strUserID = (string)jsonMessage["UserID"];
                    m_groupListen.OnUserLeaveChatRoom(strChannelID, strUserID);
                }
                break;
            case Command.CMD_QUERY_USER_STATUS:
            {
                if (null != m_contactListen)
                {
                    string strUserID = (string)jsonMessage["UserID"];
                    UserStatus status = (UserStatus)(int)jsonMessage["Status"];
                    m_contactListen.OnQueryUserStatus(errorcode, strUserID, status);
                }
            }
                break;
            case Command.CMD_AUDIO_PLAY_COMPLETE:
            {
                if(null != m_audioPlayListen)
                {
                    string path = (string)jsonMessage["Path"];
                    m_audioPlayListen.OnPlayCompletion(errorcode, path);
                }
            }
                break;
            case Command.CMD_GET_DISTRICT:
            {
                if (null != m_locationListen)
                {
                    GeographyLocation location = new GeographyLocation();
                    location.DistrictCode = (uint)(int)jsonMessage["DistrictCode"];
                    location.Country = (string)jsonMessage["Country"];
                    location.Province = (string)jsonMessage["Province"];
                    location.City = (string)jsonMessage["City"];
                    location.DistrictCounty = (string)jsonMessage["DistrictCounty"];
                    location.Street = (string)jsonMessage["Street"];
                    location.Longitude = (double)jsonMessage["Longitude"];
                    location.Latitude = (double)jsonMessage["Latitude"];

                    m_locationListen.OnUpdateLocation(errorcode, location);
                }
            }
                break;
            case Command.CMD_GET_PEOPLE_NEARBY:
            {
                uint startDistance = (uint)(int)jsonMessage["StartDistance"];
                uint endDistance = (uint)(int)jsonMessage["EndDistance"];
                JsonData neighbourListJson = jsonMessage["NeighbourList"];
                List<YIMEngine.RelativeLocation> heighbourLists = new List<YIMEngine.RelativeLocation>();
                for (int i = 0; i < neighbourListJson.Count; i++)
                {
                    YIMEngine.RelativeLocation relativeLocation = new RelativeLocation();
                    JsonData locationJson = neighbourListJson[i];

                    relativeLocation.Distance = (uint)(int)locationJson["Distance"];
                    relativeLocation.UserID = (string)locationJson["UserID"];
                    relativeLocation.Longitude = (double)locationJson["Longitude"];
                    relativeLocation.Latitude = (double)locationJson["Latitude"];
                    relativeLocation.Country = (string)locationJson["Country"];
                    relativeLocation.Province = (string)locationJson["Province"];
                    relativeLocation.City = (string)locationJson["City"];
                    relativeLocation.DistrictCounty = (string)locationJson["DistrictCounty"];
                    relativeLocation.Street = (string)locationJson["Street"];

                    heighbourLists.Add(relativeLocation);
                }
                if (null != m_locationListen)
                {
                    m_locationListen.OnGetNearbyObjects(errorcode, heighbourLists, startDistance, endDistance);
                }
            }
                break;
            case Command.CMD_TRANSLATE_COMPLETE:
            {
                if (null != m_messageListen)
                {
                    uint requestID = (uint)(int)jsonMessage["RequestID"];
                    string text = (string)jsonMessage["Text"];
                    LanguageCode srcLangCode = (LanguageCode)((int)jsonMessage["SrcLangCode"]);
                    LanguageCode destLangCode = (LanguageCode)((int)jsonMessage["DestLangCode"]);
                    //m_messageListen.OnTranslateTextComplete(errorcode, requestID, text, destLangCode);
                    
					System.Action<ErrorCode, string, LanguageCode, LanguageCode> callback = null;
					bool finded = tranlateCallbackQuen.TryGetValue(requestID, out callback);
					if (finded)
					{
						tranlateCallbackQuen.Remove(requestID);
						if(callback!=null) callback(errorcode, text, srcLangCode, destLangCode);
					}
                }
            }
                break;
            case Command.CMD_GET_MICROPHONE_STATUS:
            {
                if (null != m_audioPlayListen)
                {
                    AudioDeviceStatus status = (AudioDeviceStatus)(int)jsonMessage["Status"];
                    m_audioPlayListen.OnGetMicrophoneStatus(status);
                }
            }
                break;
            case Command.CMD_GET_TIPOFF_MSG:
            {
                if (null != m_messageListen)
                {
                    AccusationDealResult result = (AccusationDealResult)(int)jsonMessage["Result"];
                    string userID = (string)jsonMessage["UserID"];
                    uint accusationTime = (uint)(int)jsonMessage["AccusationTime"];

                    m_messageListen.OnAccusationResultNotify(result, userID, accusationTime);
                }
            }
                break;
            case Command.CMD_RECV_NOTICE:
		    {
                if (null != m_noticeListen)
                {
                    YIMEngine.Notice notice = new Notice();
                    notice.NoticeID = ulong.Parse(jsonMessage["NoticeID"].ToString());
                    notice.ChannelID = (string)jsonMessage["ChannelID"];
                    notice.NoticeType = (int)jsonMessage["NoticeType"];
                    notice.Content = (string)jsonMessage["NoticeContent"];
                    notice.LinkText = (string)jsonMessage["LinkText"];
                    notice.LinkAddr = (string)jsonMessage["LinkAddress"];
                    notice.BeginTime = (uint)(int)jsonMessage["BeginTime"];
                    notice.EndTime = (uint)(int)jsonMessage["EndTime"];

                    m_noticeListen.OnRecvNotice(notice);
                }
		    }
			    break;
		    case Command.CMD_CANCEL_NOTICE:
		    {
                if (null != m_noticeListen)
                {
                    ulong noticeID = ulong.Parse(jsonMessage["NoticeID"].ToString());
			        string channelID = (string)jsonMessage["ChannelID"];
                    m_noticeListen.OnCancelNotice(noticeID, channelID);
                }
		    }
			    break;
			case Command.CMD_GET_FORBID_RECORD:
			{
				if( null != m_messageListen )
				{
					JsonData forbidListJson = jsonMessage["ForbiddenSpeakList"];

					List<YIMEngine.ForbiddenSpeakInfo> forbidLists = new List<YIMEngine.ForbiddenSpeakInfo>();
					for (int i = 0; i < forbidListJson.Count; i++)
					{
						YIMEngine.ForbiddenSpeakInfo  info = new ForbiddenSpeakInfo();
						JsonData forbidJson = forbidListJson[i];
						
						info.ChannelID = (string)forbidJson["ChannelID"];
						info.IsForbidRoom = (int)forbidJson["IsForbidRoom"] == 0 ? false : true ;
						info.ReasonType = (int)forbidJson["reasonType"];
						info.EndTime = ulong.Parse(forbidJson["forbidEndTime"].ToString());
					
						forbidLists.Add(info);
					}

					m_messageListen.OnGetForbiddenSpeakInfo(errorcode, forbidLists );
				}
			}
				break;
			default:
					break;
			}
		}

		//login
		public ErrorCode Login(string strYouMeID,string strPasswd,string strToken="")
		{
//            Debug.Log("start login");
            return (ErrorCode)IM_Login(strYouMeID,strPasswd,strToken);
		}


		public ErrorCode Logout()
		{
			return  (ErrorCode)IM_Logout ();
		}

	


		public ErrorCode SendTextMessage(string strRecvID,ChatType chatType,string strContent,ref ulong iRequestID)
		{
			return (ErrorCode)IM_SendTextMessage(strRecvID,(int)chatType,strContent,ref iRequestID);
		}
		public ErrorCode SendFile(string strRecvID,ChatType chatType,string strFilePath,string strExtParam,FileType fileType, ref ulong iRequestID)
		{
			return (ErrorCode)IM_SendFile(strRecvID,(int)chatType,strFilePath,strExtParam,(int)fileType,ref iRequestID);
		}

		public ErrorCode SendAudioMessage(string strRecvID,ChatType chatType,ref ulong iRequestID)
		{
			return (ErrorCode)IM_SendAudioMessage(strRecvID,(int)chatType,ref iRequestID);
		}

		public ErrorCode SendOnlyAudioMessage(string strRecvID,ChatType chatType,ref ulong iRequestID)
		{
			return (ErrorCode)IM_SendOnlyAudioMessage(strRecvID,(int)chatType,ref iRequestID);
		}
		//strParam can be empty
		public ErrorCode StopAudioMessage(string strParam)
		{
			return (ErrorCode)IM_StopAudioMessage (strParam);
		}

		public ErrorCode CancleAudioMessage()
		{
			return (ErrorCode)IM_CancleAudioMessage ();
		}
		public ErrorCode DownloadAudioFile(ulong iRequestID,string strSavePath)
		{
			return (ErrorCode)IM_DownloadFile (iRequestID, strSavePath);
		}

		public ErrorCode DownloadFileByUrl( string strFromUrl, string strSavePath )
		{
			return (ErrorCode)IM_DownloadFileByURL (strFromUrl, strSavePath);
		
		}
		public ErrorCode SendCustomMessage(string strRecvID,ChatType chatTpye,byte[] customMsg,ref ulong iRequestID)
		{
			return (ErrorCode)IM_SendCustomMessage(strRecvID,(int)chatTpye,customMsg,customMsg.Length, ref iRequestID);
		}

		//chatroom
		public ErrorCode JoinChatRoom (string strChatRoomID)
		{
			return (ErrorCode)IM_JoinChatRoom(strChatRoomID);
		}
        
		public ErrorCode LeaveChatRoom (string strChatRoomID)
		{
			return (ErrorCode)IM_LeaveChatRoom(strChatRoomID);
		}

		private IMAPI()
		{
		}

        public void SetServerZone(ServerZone zone){
            IM_SetServerZone((int)zone);
        }

		public void SetAudioCachePath(string cachePath){
            IM_SetAudioCacheDir(cachePath);
        }

		//新加接口
		//查询消息记录
        public ErrorCode QueryHistoryMessage(string targetID, ChatType chatType, ulong startMessageID, int count, int direction)
		{
			return (ErrorCode)IM_QueryHistoryMessage (targetID, (int)chatType, startMessageID, count, direction);
		}

        /*清理消息记录
	    YIMChatType:私聊消息、房间消息
	    time：删除指定时间之前的消息*/
	    public ErrorCode DeleteHistoryMessage(ChatType chatType, ulong time)
        {
            return (ErrorCode)IM_DeleteHistoryMessage(chatType, time);
        }
        
        //删除指定messageID对应消息
	    public ErrorCode DeleteHistoryMessageByID(ulong messageID)
        {
            return (ErrorCode)IM_DeleteHistoryMessageByID(messageID);
        }

        public ErrorCode SetMessageRead(ulong messageID, bool read)
        {
            return (ErrorCode)IM_SetMessageRead(messageID, read);
        }

		//查询房间历史消息(房间最近N条聊天记录)
		public ErrorCode QueryRoomHistoryMessageFromServer(string roomID)
		{
			return (ErrorCode)IM_QueryRoomHistoryMessageFromServer(roomID);
		}

		//开始语音（不通过游密发送该语音消息，由调用方发送，调用StopAudioSpeech完成语音及上传后会回调OnStopAudioSpeechStatus）
		public ErrorCode StartAudioSpeech(ref ulong requestID, bool translate ) 
		{
			return (ErrorCode)IM_StartAudioSpeech (ref requestID, translate);
		}
		//停止语音（不通过游密发送该语音消息，由调用方发送，完成语音及上传后会回调OnStopAudioSpeechStatus）
		public ErrorCode StopAudioSpeech()
		{
			return (ErrorCode)IM_StopAudioSpeech ();
		}
		//转换AMR格式到WAV格式
		public ErrorCode ConvertAMRToWav(string amrFilePath, string wavFielPath )
		{
			return (ErrorCode)IM_ConvertAMRToWav (amrFilePath, wavFielPath);
		}


		//应用前后台切换调用
		public void OnResume()
		{
			IM_OnResume ();
		}
		public void OnPause()
		{
			IM_OnPause();
		}
		//发送礼物
		//extraParam:附加参数 格式为json {"nickname":"","server_area":"","location":"","score":"","level":"","vip_level":"","extra":""}
		public ErrorCode SendGift(string strAnchorId,string strChannel,int iGiftID,int iGiftCount,ExtraGifParam extParam,ref ulong serial)
		{
			return (ErrorCode)IM_SendGift (strAnchorId, strChannel, iGiftID, iGiftCount, extParam.ToJsonString() , ref serial);
		}

		//群发消息
		public ErrorCode MultiSendTextMessage(List<string> recvLists,string strText)
		{				
			JsonData recvJsonArray = new JsonData ();
			for (int i = 0; i < recvLists.Count; i++) {
				recvJsonArray.Add (recvLists [i]);	
			}
			return(ErrorCode)IM_MultiSendTextMessage (recvJsonArray.ToJson(), strText);
		}

		//获取最近联系人
		public ErrorCode GetHistoryContact()
		{
			return (ErrorCode)IM_GetRecentContacts ();
		}

		public ErrorCode GetUserInfo(string userID){
            return (ErrorCode)IM_GetUserInfo(userID);
        }

		public ErrorCode SetUserInfo(IMUserInfo userInfo){
            return (ErrorCode)IM_SetUserInfo(userInfo.ToJsonString());
        }

        //查询用户在线状态
        public ErrorCode QueryUserStatus(string userID)
        {
            return (ErrorCode)IM_QueryUserStatus(userID);
        }

        //设置播放语音音量(volume:0.0-1.0)
        public void SetVolume(float volume)
        {
            IM_SetVolume(volume);
        }

        //播放语音
        public ErrorCode StartPlayAudio(string path)
        {
            return (ErrorCode)IM_StartPlayAudio(path);
        }

	    //停止语音播放
	    public ErrorCode StopPlayAudio()
        {
            return (ErrorCode) IM_StopPlayAudio();
        }

	    //查询播放状态
	    public bool IsPlaying()
        {
            return IM_IsPlaying();
        }        

        //获取语音缓存目录
        public string GetAudioCachePath()
        {
            string strPath = "";
            System.IntPtr pBuffer = IM_GetAudioCachePath();
            if (pBuffer == System.IntPtr.Zero)
            {
                return strPath;
            }
            strPath = Marshal.PtrToStringAuto(pBuffer);
            IM_DestroyAudioCachePath(pBuffer);
            return strPath;
        }
        
        //清理语音缓存目录
        public bool ClearAudioCachePath()
        {
            return IM_ClearAudioCachePath();
        }

        // 文本翻译
        public void TranslateText( string text, LanguageCode destLangCode, LanguageCode srcLangCode,System.Action<ErrorCode,string, LanguageCode, LanguageCode> callback)
		{
            uint requestID = 0;
            var code = (ErrorCode) IM_TranslateText(ref requestID, text, destLangCode, srcLangCode);
            if(code == ErrorCode.Success)
            {
                tranlateCallbackQuen.Add(requestID, callback);
            }
            else
            {
                callback(code,"", srcLangCode, destLangCode);
            }
           
		}

        public ErrorCode GetCurrentLocation()
        {
            return (ErrorCode) IM_GetCurrentLocation();
        }
	    
        // 获取附近的目标	count:获取数量（一次最大200） serverAreaID：区服	districtlevel：行政区划等级		resetStartDistance：是否重置查找起始距离
        public ErrorCode GetNearbyObjects(int count, string serverAreaID, DistrictLevel districtlevel = YIMEngine.DistrictLevel.DISTRICT_UNKNOW, bool resetStartDistance = false)
        {
            return (ErrorCode) IM_GetNearbyObjects(count, serverAreaID, districtlevel, resetStartDistance);
        }

        // 设置位置更新间隔(单位：分钟)
	    public void SetUpdateInterval(uint interval)
        {
            IM_SetUpdateInterval(interval);
        }

        public void SetKeepRecordModel(bool keep)
        {
            IM_SetKeepRecordModel(keep);
        }

        public int SetSpeechRecognizeParam(SpeechAccent accent, SpeechRecognizeLanguage language)
        {
           return IM_SetSpeechRecognizeParam(accent, language);
        }

        public int GetMicrophoneStatus()
        {
            return IM_GetMicrophoneStatus();
        }

	    public int Accusation(string userID, ChatType source, int reason, string description, string extraParam)
        {
            return IM_Accusation(userID, source, reason, description, extraParam);
        }

	    public int QueryNotice()
        {
            return IM_QueryNotice();
        }

		public ErrorCode GetForbiddenSpeakInfo()
		{
			return (ErrorCode)IM_GetForbiddenSpeakInfo ();
		}
	}
}
#endif