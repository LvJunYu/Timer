#pragma once

#include "resource.h"
#include "ClientProcMsgObject_i.h"
#include "Define.h"
#include <vector>
#include <map>
#include <string>
#include <windows.h>
#include <iostream>

std::string m_strCmdline;
std::string m_strOpenId;
std::string m_strOpenKey;
std::string m_strProcPara;
HWND m_hWnd;

class Launcher : public IClientProcMsgEventHandler
{
public:
	virtual void OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj);
	virtual void OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode);
	virtual void OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj);
	virtual void OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen, const BYTE* pRecvBuf);
	void ConncetPipe();
	void Release();

private:
	IClientProcMsgObject* m_pProcMsgObj;
	HMODULE m_hModule;

	BOOL LoadLibrary();
	IClientProcMsgObject* CreateObj();
	void ReleaseObj(IClientProcMsgObject* pObj);
	void ReStartApp(LPCSTR lpszProc);
	void SendMsgToGame(IClientProcMsgObject* pProcMsgObj, PROCMSG_DATA* pProcMsgData);

	void ShowMsg(LPCTSTR lpszMsg);
};

Launcher* m_launcher;