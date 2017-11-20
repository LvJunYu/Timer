namespace YIMEngine
{
	public abstract class MessageInfoBase
	{
		private YIMEngine.ChatType chatType;
		private string strSenderID;
		private string strRecvID;
		private ulong iRequestID;
		private MessageBodyType messageType;
		private int iCreateTime;
        private uint iDistance;
        private bool bIsRead;
		public YIMEngine.ChatType ChatType {
			get {
				return chatType;
			}
			internal set {
				chatType = value;
			}
		}

		public int CreateTime {
			get {
				return iCreateTime;
			}
			set {
				iCreateTime = value;
			}
		}
		
		public string SenderID {
			get {
				return strSenderID;
			}
			set {
				strSenderID = value;
			}
		}
		
		public string RecvID {
			get {
				return strRecvID;
			}
			set {
				strRecvID = value;
			}
		}
		
		public MessageBodyType MessageType {
			get {
				return messageType;
			}
			set {
				messageType = value;
			}
		}

		public ulong RequestID {
			get {
				return iRequestID;
			}
			set {
				iRequestID = value;
			}
		}

        public uint Distance {
            get {
                return iDistance;
            }
            set {
                iDistance = value;
            }
        }

        public bool IsRead
        {
            get {
                return bIsRead;
            }
            set {
                bIsRead = value;
            }
        }
	}

	public class TextMessage:MessageInfoBase
	{
		private string strContent;
		
		public string Content {
			get {
				return strContent;
			}
			set {
				strContent = value;
			}
		}
	}

	public class CustomMessage:MessageInfoBase
	{
		private byte[] strContent;
		
		public byte[] Content {
			get {
				return strContent;
			}
			set {
				strContent = value;
			}
		}
	}

	public class FileMessage:MessageInfoBase
	{
		private string strFileName;
		private int iFileSize;
		private FileType fFileType;
		private string strExtension;
		private string strExtParam;

		public string ExtParam {
			get {
				return strExtParam;
			}
			set {
				strExtParam = value;
			}
		}

		public string FileExtension {
			get {
				return strExtension;
			}
			set {
				strExtension = value;
			}
		}

		public string FileName {
			get {
				return strFileName;
			}
			set {
				strFileName = value;
			}
		}

		public int FileSize {
			get {
				return iFileSize;
			}
			set {
				iFileSize = value;
			}
		}

		public FileType FileType {
			get {
				return fFileType;
			}
			set {
				fFileType = value;
			}
		}
	}


	public class GiftMessage:MessageInfoBase
	{
		private int iGiftCount;
		private int iGiftID;
		private ExtraGifParam strParam;
		private string strAnchor;

		public string Anchor {
			get {
				return strAnchor;
			}
			set {
				strAnchor = value;
			}
		}

		public int GiftID {
			get {
				return iGiftID;
			}
			set {
				iGiftID = value;
			}
		}

		public ExtraGifParam ExtParam {
			get {
				return strParam;
			}
			set {
				strParam = value;
			}
		}

		public int GiftCount {
			get {
				return iGiftCount;
			}
			set {
				iGiftCount = value;
			}
		}
	}

	public class VoiceMessage:MessageInfoBase
	{
		private string strText;
		private string strParam;
		private int iDuration;

		public int Duration {
			get {
				return iDuration;
			}
			set {
				iDuration = value;
			}
		}

		public string Text {
			get {
				return strText;
			}
			set {
				strText = value;
			}
		}

		public string Param {
			get {
				return strParam;
			}
			set {
				strParam = value;
			}
		}
	}
	public enum ErrorCode
	{
		Success = 0,
		EngineNotInit = 1,
		NotLogin = 2,	
		ParamInvalid = 3,
		TimeOut = 4,
		StatusError = 5,
		SDKInvalid = 6,
		AlreadyLogin = 7,
		ServerError = 8,
		NetError = 9,
		LoginSessionError = 10,
		NotStartUp = 11,
		FileNotExist = 12,
		SendFileError = 13,
		UploadFailed = 14,
		UsernamePasswordError = 15,
		UserStatusError = 16,	
		MessageTooLong = 17,	//消息太长
		ReceiverTooLong = 18,	//接收方ID过长(检查房间名)
		InvalidChatType = 19,	//无效聊天类型(私聊、聊天室)
		InvalidReceiver = 20,	//无效用户ID（私聊接受者为数字格式ID）
        UnknowError = 21,		//常见于发送房间消息，房间号并不存在。修正的方法是往自己joinRoom的房间id发消息，就可以发送成功。
        InvalidAppkey = 22,			//无效APPKEY
        ForbiddenSpeak = 23,			//被禁言
        CreateFileFailed = 24,     //无法创建文件
        UnsupportFormat = 25,			//不支持的文件格式
        ReceiverEmpty = 26,			//接收方为空
        RoomIDTooLong = 27,			//房间名太长
        ContentInvalid = 28,			//聊天内容严重非法
        NoLocationAuthrize = 29,		//未打开定位权限
        UnknowLocation = 30,			//未知位置
        Unsupport = 31,				//不支持该接口
        NoAudioDevice = 32,			//无音频设备
        AudioDriver = 33,				//音频驱动问题
        DeviceStatusInvalid = 34,		//设备状态错误
        ResolveFileError = 35,			//文件解析错误
        ReadWriteFileError = 36,		//文件读写错误
        NoLangCode = 37,				//语言编码错误
        TranslateUnable = 38,			//翻译接口不可用
        SpeechAccentInvalid = 39,		//语音识别方言无效
        SpeechLanguageInvalid = 40,	    //语音识别语言无效
        HasIllegalText = 41,			//消息含非法字符
        AdvertisementMessage = 42,		//消息涉嫌广告

		//服务器的错误码
		ALREADYFRIENDS = 1000,
		LoginInvalid = 1001,

		//语音部分错误码
		PTT_Start = 2000,
		PTT_Fail = 2001,
		PTT_DownloadFail = 2002,
		PTT_GetUploadTokenFail = 2003,
		PTT_UploadFail = 2004,
		PTT_NotSpeech = 2005,
        PTT_DeviceStatusError = 2006,		//音频设备状态错误
        PTT_IsSpeeching = 2007,			//已开启语音
        PTT_FileNotExist = 2008,
        PTT_ReachMaxDuration = 2009,		//达到语音最大时长限制
        PTT_SpeechTooShort = 2010,			//语音时长太短
        PTT_StartAudioRecordFailed = 2011,	//启动语音失败
        PTT_SpeechTimeout = 2012,			//音频输入超时
        PTT_IsPlaying = 2013,				//正在播放
        PTT_NotStartPlay = 2014,			//未开始播放
        PTT_CancelPlay = 2015,				//主动取消播放
        PTT_NotStartRecord = 2016,			//未开始语音
		Fail = 10000
	} 

	//群类型
	public enum GroupType
	{
		//普通群
		Group = 0,
		//聊天室
		ChatRoom = 1
	};

	public enum GroupEvent
	{
		Agree = 0,
		Refuse = 1,
	};

	public enum ChatType
	{
		Unknow = 0,
		PrivateChat = 1,
		RoomChat = 2,
	};

	public enum FileType
	{
		FileType_Other = 0,
		FileType_Audio = 1,
		FileType_Image = 2,
		FileType_Video = 3
	};
	public enum MessageBodyType
	{
		Unknow = 0,
		TXT = 1,
		CustomMesssage = 2,
		Emoji = 3,
		Image = 4,
		Voice = 5,
		Video = 6,
		File = 7,
		Gift = 8
	};
	public enum Command
	{
		CMD_UNKNOW = 0,
		CMD_LOGIN = 1,
		CMD_HEARTBEAT = 2,
		CMD_LOGOUT = 3,
		CMD_ENTER_ROOM = 4,
		CMD_LEAVE_ROOM = 5,
		CMD_SND_TEXT_MSG = 6,
		CMD_SND_VOICE_MSG = 7,
		CMD_SND_FILE_MSG = 8,
		CMD_GET_MSG = 9,
		CMD_GET_UPLOAD_TOKEN = 10,
		CMD_KICK_OFF = 11,
		CMD_SND_BIN_MSG = 12,
		CMD_RELOGIN = 13,
		CMD_CHECK_ONLINE = 14,
		CMD_SND_GIFT_MSG = 15,
		CMD_GET_ROOM_HISTORY_MSG = 16,
		CMD_GET_USR_INFO = 17,
		CMD_UPDATE_USER_INFO = 18,
        CMD_SND_TIPOFF_MSG = 19,
        CMD_GET_TIPOFF_MSG = 20,
        CMD_GET_DISTRICT = 21,
        CMD_GET_PEOPLE_NEARBY = 22,
        CMD_QUERY_NOTICE = 23,
		CMD_GET_FORBID_RECORD = 31,
		
		//服务器通知
		NOTIFY_LOGIN = 10001,
		NOTIFY_PRIVATE_MSG,
		NOTIFY_ROOM_MSG,

        NOTIFY_PRIVATE_MSG_V2 = 10012,
        NOTIFY_ROOM_MSG_V2 = 10013,

		//客户端(C接口使用)
		CMD_DOWNLOAD = 20001,
		CMD_SEND_MESSAGE_STATUS,
		CMD_RECV_MESSAGE,
		CMD_STOP_AUDIOSPEECH,
		CMD_QUERY_HISTORY_MESSAGE,
		CMD_GET_RENCENT_CONTACTS,
		CMD_RECEIVE_MESSAGE_NITIFY,
        CMD_QUERY_USER_STATUS,
        CMD_AUDIO_PLAY_COMPLETE,
        CMD_STOP_SEND_AUDIO,
        CMD_TRANSLATE_COMPLETE,
		CMD_DOWNLOAD_URL,
        CMD_GET_MICROPHONE_STATUS,
        CMD_USER_ENTER_ROOM,
        CMD_USER_LEAVE_ROOM,
        CMD_RECV_NOTICE,
        CMD_CANCEL_NOTICE
	}

	public enum ServerZone
	{
		China = 0,		// 中国
		Singapore = 1,	// 新加坡
		America = 2,		// 美国
		HongKong = 3,	// 香港
		Korea = 4,		// 韩国
		Australia = 5,	// 澳洲
		Deutschland = 6,	// 德国
		Brazil = 7,		// 巴西
		India = 8,		// 印度
		Japan = 9,		// 日本
		Ireland = 10,	// 爱尔兰
		ServerZone_Unknow = 9999
	};
	public class HistoryMsg
	{
		
		private ChatType iChatType;
		MessageBodyType iMessageType;
		string strParam;
		string strReceiveID;
		string strSenderID;
		ulong uMessageID;
		string strText;
		string strLocalPath;
		int iCreateTime;
		int iDuration;
        bool bIsRead;
        byte[] customMsg;

        // 消息内容类型
        public MessageBodyType MessageType {
			get {
				return iMessageType;
			}
			set {
				iMessageType = value;
			}
		}
		// 聊天类型，私聊 or 频道聊天，目前历史记录都是私聊
		public ChatType ChatType {
			get {
				return iChatType;
			}
			set {
				iChatType = value;
			}
		}
		// 消息收发时间
		public int CreateTime {
			get {
				return iCreateTime;
			}
			set {
				iCreateTime = value;
			}
		}
		// 语音消息的wav文件本地路径
		public string LocalPath {
			get {
				return strLocalPath;
			}
			set {
				strLocalPath = value;
			}
		}
		// 文本消息内容 或者 语音消息的文本识别内容
		public string Text {
			get {
				return strText;
			}
			set {
				strText = value;
			}
		}
		// 历史记录消息ID
		public ulong MessageID {
			get {
				return uMessageID;
			}
			set {
				uMessageID = value;
			}
		}

		// 消息发送者id
		public string SenderID {
			get {
				return strSenderID;
			}
			set {
				strSenderID = value;
			}
		}

		// 消息接收者id
		public string ReceiveID {
			get {
				return strReceiveID;
			}
			set {
				strReceiveID = value;
			}
		}

		// 如果是语音消息，该值表示语音消息的自定义附加参数
		public string Param {
			get {
				return strParam;
			}
			set {
				strParam = value;
			}
		}

		// 如果是语音消息，该值表示语音消息时长
		public int Duration {
			get {
				return iDuration;
			}
			set {
				iDuration = value;
			}
		}

        public bool IsRead {
            get {
                return bIsRead;
            }
            set {
                bIsRead = value;
            }
        }

		public byte[] CustomMsg{
			get{
                return System.Convert.FromBase64String(Text);
            }
		}
	};


    //用户状态
    public enum UserStatus
    {
        STATUS_ONLINE,	//在线
        STATUS_OFFLINE	//离线
    };

	public enum ForbidSpeakReason{
		Unkown = 0,         //未知
		AD      = 1,        //发广告
		Insult  = 2,        //侮辱
		Politics  = 3,      //政治敏感
		Terrorism  = 4,     //恐怖主义
		Reaction  = 5,      //反动
		Sexy  = 6,          //色情
		Other  = 7,         //其他
	};
	
	public enum LanguageCode
    {
        LANG_AUTO,
        LANG_AF,			// 南非荷兰语
        LANG_AM,			// 阿姆哈拉语
        LANG_AR,			// 阿拉伯语                   
        LANG_AR_AE,			// 阿拉伯语+阿拉伯联合酋长国    
        LANG_AR_BH,			// 阿拉伯语+巴林                
        LANG_AR_DZ,			// 阿拉伯语+阿尔及利亚          
        LANG_AR_EG,			// 阿拉伯语+埃及                
        LANG_AR_IQ,			// 阿拉伯语+伊拉克              
        LANG_AR_JO,			// 阿拉伯语+约旦                
        LANG_AR_KW,			// 阿拉伯语+科威特              
        LANG_AR_LB,			// 阿拉伯语+黎巴嫩              
        LANG_AR_LY,			// 阿拉伯语+利比亚              
        LANG_AR_MA,			// 阿拉伯语+摩洛哥              
        LANG_AR_OM,			// 阿拉伯语+阿曼                
        LANG_AR_QA,			// 阿拉伯语+卡塔尔              
        LANG_AR_SA,			// 阿拉伯语+沙特阿拉伯          
        LANG_AR_SD,			// 阿拉伯语+苏丹                
        LANG_AR_SY,			// 阿拉伯语+叙利亚              
        LANG_AR_TN,			// 阿拉伯语+突尼斯              
        LANG_AR_YE,			// 阿拉伯语+也门
        LANG_AZ,			// 阿塞拜疆
        LANG_BE,			// 白俄罗斯语                
        LANG_BE_BY,			// 白俄罗斯语+白俄罗斯          
        LANG_BG,			// 保加利亚语                
        LANG_BG_BG,			// 保加利亚语+保加利亚          
        LANG_BN,			// 孟加拉
        LANG_BS,			// 波斯尼亚语
        LANG_CA,			// 加泰罗尼亚语               
        LANG_CA_ES,			// 加泰罗尼亚语+西班牙          
        LANG_CA_ES_EURO,	// 加泰罗尼亚语+西班牙,euro     
        LANG_CO,			// 科西嘉
        LANG_CS,			// 捷克语                 
        LANG_CS_CZ,			// 捷克语+捷克共和国     
        LANG_CY,			// 威尔士语
        LANG_DA,			// 丹麦语                  
        LANG_DA_DK,			// 丹麦语+丹麦                  
        LANG_DE,			// 德语                      
        LANG_DE_AT,			// 德语+奥地利                  
        LANG_DE_AT_EURO,	// 德语+奥地利,euro             
        LANG_DE_CH,			// 德语+瑞士                    
        LANG_DE_DE,			// 德语+德国                    
        LANG_DE_DE_EURO,	// 德语+德国,euro               
        LANG_DE_LU,			// 德语+卢森堡                  
        LANG_DE_LU_EURO,	// 德语+卢森堡,euro             
        LANG_EL,			// 希腊语                     
        LANG_EL_GR,			// 希腊语+希腊  
        LANG_EN,			// 英语
        LANG_EN_AU,			// 英语+澳大利亚                
        LANG_EN_CA,			// 英语+加拿大                  
        LANG_EN_GB,			// 英语+英国                    
        LANG_EN_IE,			// 英语+爱尔兰                  
        LANG_EN_IE_EURO,	// 英语+爱尔兰,euro             
        LANG_EN_NZ,			// 英语+新西兰  
        LANG_EN_US,			// 英语+美国
        LANG_EN_ZA,			// 英语+南非   
        LANG_EO,			// 世界语
        LANG_ES,			// 西班牙语                 
        LANG_ES_BO,			// 西班牙语+玻利维亚            
        LANG_ES_AR,			// 西班牙语+阿根廷              
        LANG_ES_CL,			// 西班牙语+智利                
        LANG_ES_CO,			// 西班牙语+哥伦比亚            
        LANG_ES_CR,			// 西班牙语+哥斯达黎加          
        LANG_ES_DO,			// 西班牙语+多米尼加共和国      
        LANG_ES_EC,			// 西班牙语+厄瓜多尔            
        LANG_ES_ES,			// 西班牙语+西班牙              
        LANG_ES_ES_EURO,	// 西班牙语+西班牙,euro         
        LANG_ES_GT,			// 西班牙语+危地马拉            
        LANG_ES_HN,			// 西班牙语+洪都拉斯            
        LANG_ES_MX,			// 西班牙语+墨西哥              
        LANG_ES_NI,			// 西班牙语+尼加拉瓜            
        LANG_ET,			// 爱沙尼亚语       
        LANG_ES_PA,			// 西班牙语+巴拿马              
        LANG_ES_PE,			// 西班牙语+秘鲁                
        LANG_ES_PR,			// 西班牙语+波多黎哥            
        LANG_ES_PY,			// 西班牙语+巴拉圭              
        LANG_ES_SV,			// 西班牙语+萨尔瓦多            
        LANG_ES_UY,			// 西班牙语+乌拉圭              
        LANG_ES_VE,			// 西班牙语+委内瑞拉            
        LANG_ET_EE,			// 爱沙尼亚语+爱沙尼亚          
        LANG_EU,			// 巴斯克 
        LANG_FA,			// 波斯语
        LANG_FI,			// 芬兰语                  
        LANG_FI_FI,			// 芬兰语+芬兰                  
        LANG_FI_FI_EURO,	// 芬兰语+芬兰,euro             
        LANG_FR,			// 法语                        
        LANG_FR_BE,			// 法语+比利时                  
        LANG_FR_BE_EURO,	// 法语+比利时,euro             
        LANG_FR_CA,			// 法语+加拿大                  
        LANG_FR_CH,			// 法语+瑞士                    
        LANG_FR_FR,			// 法语+法国                    
        LANG_FR_FR_EURO,	// 法语+法国,euro               
        LANG_FR_LU,			// 法语+卢森堡                  
        LANG_FR_LU_EURO,	// 法语+卢森堡,euro
        LANG_FY,			// 弗里斯兰
        LANG_GA,			// 爱尔兰语
        LANG_GD,			// 苏格兰盖尔语
        LANG_GL,			// 加利西亚
        LANG_GU,			// 古吉拉特文
        LANG_HA,			// 豪撒语
        LANG_HAW,			// 夏威夷语
        LANG_HI,			// 印地语
        LANG_HR,			// 克罗地亚语                  
        LANG_HR_HR,			// 克罗地亚语+克罗地亚 
        LANG_HT,			// 海地克里奥尔
        LANG_HU,			// 匈牙利语                  
        LANG_HU_HU,			// 匈牙利语+匈牙利
        LANG_HY,			// 亚美尼亚
        LANG_ID,			// 印度尼西亚
        LANG_IG,			// 伊博
        LANG_IS,			// 冰岛语                     
        LANG_IS_IS,			// 冰岛语+冰岛                  
        LANG_IT,			// 意大利语                   
        LANG_IT_CH,			// 意大利语+瑞士                
        LANG_IT_IT,			// 意大利语+意大利              
        LANG_IT_IT_EURO,	// 意大利语+意大利,euro         
        LANG_IW,			// 希伯来语                    
        LANG_IW_IL,			// 希伯来语+以色列              
        LANG_JA,			// 日语                     
        LANG_JA_JP,			// 日语+日本    
        LANG_JW,			// 爪哇
        LANG_KA,			// 格鲁吉亚语
        LANG_KK,			// 哈萨克语
        LANG_KN,			// 卡纳达
        LANG_KM,			// 高棉语
        LANG_KO,			// 朝鲜语                      
        LANG_KO_KR,			// 朝鲜语+南朝鲜                
        LANG_KU,			// 库尔德
        LANG_KY,			// 吉尔吉斯斯坦
        LANG_LA,			// 拉丁语
        LANG_LB,			// 卢森堡语
        LANG_LO,			// 老挝
        LANG_LT,			// 立陶宛语
        LANG_LT_LT,			// 立陶宛语+立陶宛              
        LANG_LV,			// 拉托维亚语+列托              
        LANG_LV_LV,			// 拉托维亚语+列托     
        LANG_MG,			// 马尔加什
        LANG_MI,			// 毛利
        LANG_MK,			// 马其顿语
        LANG_MK_MK,			// 马其顿语+马其顿王国    
        LANG_ML,			// 马拉雅拉姆
        LANG_MA,			// 旁遮普语
        LANG_MN,			// 蒙古
        LANG_MR,			// 马拉地语
        LANG_MS,			// 马来语
        LANG_MT,			// 马耳他
        LANG_MY,			// 缅甸
        LANG_NL,			// 荷兰语
        LANG_NL_BE,			// 荷兰语+比利时                
        LANG_NL_BE_EURO,	// 荷兰语+比利时,euro           
        LANG_NL_NL,			// 荷兰语+荷兰                  
        LANG_NL_NL_EURO,	// 荷兰语+荷兰,euro   
        LANG_NE,			// 尼泊尔
        LANG_NO,			// 挪威语
        LANG_NO_NO,			// 挪威语+挪威                  
        LANG_NO_NO_NY,		// 挪威语+挪威,nynorsk          
        LANG_NY,			// 齐切瓦语
        LANG_PL,			// 波兰语
        LANG_PL_PL,			// 波兰语+波兰      
        LANG_PS,			// 普什图语
        LANG_PT,			// 葡萄牙语
        LANG_PT_BR,			// 葡萄牙语+巴西                
        LANG_PT_PT,			// 葡萄牙语+葡萄牙              
        LANG_PT_PT_EURO,	// 葡萄牙语+葡萄牙,euro         
        LANG_RO,			// 罗马尼亚语
        LANG_RO_RO,			// 罗马尼亚语+罗马尼亚          
        LANG_RU,			// 俄语
        LANG_RU_RU,			// 俄语+俄罗斯       
        LANG_SD,			// 信德
        LANG_SH,			// 塞波尼斯_克罗地亚语
        LANG_SH_YU,			// 塞波尼斯_克罗地亚语+南斯拉夫 
        LANG_SI,			// 僧伽罗语
        LANG_SK,			// 斯洛伐克语
        LANG_SK_SK,			// 斯洛伐克语+斯洛伐克          
        LANG_SL,			// 斯洛语尼亚语
        LANG_SL_SI,			// 斯洛语尼亚语+斯洛文尼亚     
        LANG_SM,			// 萨摩亚
        LANG_SN,			// 修纳
        LANG_SO,			// 索马里
        LANG_SQ,			// 阿尔巴尼亚语
        LANG_SQ_AL,			// 阿尔巴尼亚语+阿尔巴尼亚      
        LANG_SR,			// 塞尔维亚语
        LANG_SR_YU,			// 塞尔维亚语+南斯拉夫       
        LANG_ST,			// 塞索托语
        LANG_SU,			// 巽他语
        LANG_SV,			// 瑞典语
        LANG_SV_SE,			// 瑞典语+瑞典       
        LANG_SW,			// 斯瓦希里语
        LANG_TA,			// 泰米尔
        LANG_TE,			// 泰卢固语
        LANG_TG,			// 塔吉克斯坦
        LANG_TH,			// 泰语
        LANG_TH_TH,			// 泰语+泰国                    
        LANG_TL,			// 菲律宾
        LANG_TR,			// 土耳其语
        LANG_TR_TR,			// 土耳其语+土耳其              
        LANG_UK,			// 乌克兰语
        LANG_UK_UA,			// 乌克兰语+乌克兰    
        LANG_UR,			// 乌尔都语
        LANG_UZ,			// 乌兹别克斯坦
        LANG_VI,			// 越南
        LANG_XH,			// 科萨
        LANG_YID,			// 意第绪语
        LANG_YO,			// 约鲁巴语
        LANG_ZH,			// 汉语
        LANG_ZH_CN,			// 汉语+中国                    
        LANG_ZH_HK,			// 汉语+香港                    
        LANG_ZH_TW,			// 汉语+台湾     
        LANG_ZU				// 祖鲁语
    };

    public class GeographyLocation
    {
        uint iDistrictCode;
        string strCountry;
        string strProvince;
        string strCity;
        string strDistrictCounty;
        string strStreet;
        double fLongitude;
        double fLatitude;

        public uint DistrictCode {
            get {
                return iDistrictCode;
            }
            set {
                iDistrictCode = value;
            }
        }

        public string Country {
			get {
                return strCountry;
			}
			set {
                strCountry = value;
			}
		}

        public string Province {
            get {
                return strProvince;
            }
            set {
                strProvince = value;
            }
        }

        public string City {
            get {
                return strCity;
            }
            set {
                strCity = value;
            }
        }

        public string DistrictCounty {
            get {
                return strDistrictCounty;
            }
            set {
                strDistrictCounty = value;
            }
        }

        public string Street {
            get {
                return strStreet;
            }
            set {
                strStreet = value;
            }
        }

        public double Longitude
        {
            get {
                return fLongitude;
            }
            set {
                fLongitude = value;
            }
        }
        public double Latitude
        {
            get {
                return fLatitude;
            }
            set {
                fLatitude = value;
            }
        }
    };

	public class ForbiddenSpeakInfo
	{
		string channelID;
		bool isForbidRoom;
		int  reasonType;
		ulong  endTime;

		public string ChannelID{
			get{
				return channelID;
			}
			set{
				channelID = value;
			}
		}

		public bool IsForbidRoom{
			get{
				return isForbidRoom;
			}
			set{
				isForbidRoom = value;
			}
		}

		public int ReasonType{
			get{
				return reasonType;
			}
			set{
				reasonType = value;
			}
		}


		public ulong EndTime{
			get{
				return endTime;
			}
			set{
				endTime = value;
			}
		}
	};

    public class RelativeLocation
    {
        uint iDistance;
        double fLongitude;
        double fLatitude;
        string strUserID;
        string strCountry;
        string strProvince;
        string strCity;
        string strDistrictCounty;
        string strStreet;

        public uint Distance {
            get {
                return iDistance;
            }
            set {
                iDistance = value;
            }
        }

        public double Longitude {
            get {
                return fLongitude;
            }
            set {
                fLongitude = value;
            }
        }
        public double Latitude {
            get {
                return fLatitude;
            }
            set {
                fLatitude = value;
            }
        }

        public string UserID {
            get {
                return strUserID;
            }
            set {
                strUserID = value;
            }
        }

        public string Country {
            get {
                return strCountry;
            }
            set {
                strCountry = value;
            }
        }

        public string Province {
            get {
                return strProvince;
            }
            set {
                strProvince = value;
            }
        }

        public string City {
            get {
                return strCity;
            }
            set {
                strCity = value;
            }
        }

        public string DistrictCounty {
            get {
                return strDistrictCounty;
            }
            set {
                strDistrictCounty = value;
            }
        }

        public string Street {
            get {
                return strStreet;
            }
            set {
                strStreet = value;
            }
        }
    }

    public enum DistrictLevel
    {
        DISTRICT_UNKNOW,
        DISTRICT_COUNTRY,	// 国家
        DISTRICT_PROVINCE,	// 省份
        DISTRICT_CITY,		// 市
        DISTRICT_COUNTY,	// 区县
        DISTRICT_STREET		// 街道
    };

    // 举报处理结果
    public enum AccusationDealResult
    {
        ACCUSATIONRESULT_IGNORE,			// 忽略
        ACCUSATIONRESULT_WARNING,			// 警告
        ACCUSATIONRESULT_FROBIDDEN_SPEAK	// 禁言
    };

    public enum SpeechAccent
    {
        ACCENT_MANDARIN,	// 普通话(Android IOS Windows)
        ACCENT_YUEYU,		// 粤语(Android IOS Windows)
        ACCENT_SICHUAN,		// 四川话(Android IOS)
        ACCENT_HENAN		// 河南话(IOS)
    };

    public enum SpeechRecognizeLanguage
    {
        RECOGNIZELANG_ZH_CH,	// 简体中文(Android IOS Windows)
	    RECOGNIZELANG_ZH_TW,	// 繁体中文(Windows IOS)
	    RECOGNIZELANG_EN_US		// 英文(Android Windows IOS)
    };

    public enum AudioDeviceStatus
    {
        STATUS_AVAILABLE,		// 可用
        STATUS_NO_AUTHORITY,	// 无权限
        STATUS_MUTE,			// 静音
        STATUS_UNAVAILABLE		// 不可用
    };

    public class Notice
    {
        ulong uNoticeID;
	    int iNoticeType;
	    string strChannelID;
	    string strContent;
	    string strLinkeText;
	    string strLinkAddr;
	    uint iBeginTime;
	    uint iEndTime;

        public ulong NoticeID
        {
            get{
                return uNoticeID;
            }
            set{
                uNoticeID = value;
            }
        }
        
        public int NoticeType
        {
            get{
                return iNoticeType;
            }
            set{
                iNoticeType = value;
            }
        }

        public string ChannelID
        {
            get{
                return strChannelID;
            }
            set{
                strChannelID = value;
            }
        }

        public string Content
        {
            get{
                return strContent;
            }
            set{
                strContent = value;
            }
        }

        public string LinkText
        {
            get{
                return strLinkeText;
            }
            set{
                strLinkeText = value;
            }
        }

        public string LinkAddr
        {
            get{
                return strLinkAddr;
            }
            set{
                strLinkAddr = value;
            }
        }

        public uint BeginTime
        {
            get{
                return iBeginTime;
            }
            set{
                iBeginTime = value;
            }
        }

        public uint EndTime
        {
            get{
                return iEndTime;
            }
            set{
                iEndTime = value;
            }
        }
    }
}