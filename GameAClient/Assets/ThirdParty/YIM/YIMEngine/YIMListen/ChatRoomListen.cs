namespace YIMEngine
{
	public interface ChatRoomListen
	{
		void OnJoinRoom(YIMEngine.ErrorCode errorcode,string strChatRoomID);
		void OnLeaveRoom(YIMEngine.ErrorCode errorcode,string strChatRoomID);
	    void OnUserJoinChatRoom(string strRoomID, string strUserID);
	    void OnUserLeaveChatRoom(string strRoomID, string strUserID);
	}

}