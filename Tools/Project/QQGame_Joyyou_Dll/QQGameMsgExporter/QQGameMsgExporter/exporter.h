#pragma once
#include "ClientProcMsgObject_i.h"
#include "windows.h"


typedef void(__stdcall *FunLog)(const char*);
typedef void(__stdcall *FunOnConnectSucc)(IClientProcMsgObject* pClientProcMsgObj);
typedef void(__stdcall *FunOnConnectFailed)(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode);
typedef void(__stdcall *FunOnConnectionDestroyed)(IClientProcMsgObject* pClientProcMsgObj);
typedef void(__stdcall *FunOnReceiveMsg)(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen);

extern "C"
{
	__declspec(dllexport) BOOL __stdcall Initialize(LPCSTR);
	__declspec(dllexport) IClientProcMsgObject* __stdcall CreateClientProcMsgObject();
	__declspec(dllexport) void __stdcall ReleaseClientProcMsgObject(IClientProcMsgObject* pObj);
	__declspec(dllexport) void __stdcall Release();

	__declspec(dllexport) void __stdcall SetLogCallback(FunLog fun);
	__declspec(dllexport) void __stdcall SetOnConnectionSuccCallback(FunOnConnectSucc fun);
	__declspec(dllexport) void __stdcall SetOnConnectionFailedCallback(FunOnConnectFailed fun);
	__declspec(dllexport) void __stdcall SetOnConnectionDestroyedCallback(FunOnConnectionDestroyed fun);
	__declspec(dllexport) void __stdcall SetOnReceiveMsgCallback(FunOnReceiveMsg fun, BYTE* pRecvBuf, long lLen);

	__declspec(dllexport) BOOL __stdcall IClientProcMsgObject_Initialize(IClientProcMsgObject*);
	__declspec(dllexport) BOOL __stdcall IClientProcMsgObject_Connect(IClientProcMsgObject*, LPCSTR);
	__declspec(dllexport) void __stdcall IClientProcMsgObject_Disconnect(IClientProcMsgObject*);
	__declspec(dllexport) BOOL __stdcall IClientProcMsgObject_IsConnected(IClientProcMsgObject*);
	__declspec(dllexport) DWORD __stdcall IClientProcMsgObject_SendMessage(IClientProcMsgObject*, long lLen, const BYTE* pbySendBuf);
	__declspec(dllexport) void __stdcall IClientProcMsgObject_AddEventHandler(IClientProcMsgObject*, IClientProcMsgEventHandler* pEventHandler);
	__declspec(dllexport) void __stdcall IClientProcMsgObject_RemoveEventHandler(IClientProcMsgObject*, IClientProcMsgEventHandler* pEventHandler);
}

class EventHandler : public IClientProcMsgEventHandler
{
public:
	virtual void OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj);
	virtual void OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode);
	virtual void OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj);
	virtual void OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen, const BYTE* pRecvBuf);
};

HMODULE m_hModule;
EventHandler *m_EventHandler;
FunLog m_loger;
FunOnConnectSucc m_OnConnectSucc;
FunOnConnectFailed m_OnConnectFailed;
FunOnConnectionDestroyed m_OnConnectionDestroyed;
FunOnReceiveMsg m_OnReceiveMsg;
BYTE* m_ReceiveBuffer;
long m_ReceiveBufferLen;
char buf[1024];

