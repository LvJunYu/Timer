#include "exporter.h"
#include <atlbase.h>
#include <atlconv.h>

const char* szCreateClientObjFunc = "CreateClientProcMsgObject";
const char* szReleaseClientObjFunc = "ReleaseClientProcMsgObject";

typedef IClientProcMsgObject* (*CreateObjFunc)();
typedef void(*ReleaseObjFunc)(IClientProcMsgObject*);

BOOL __stdcall Initialize(LPCWSTR path)
{
	m_hModule = LoadLibrary(path);
	if (m_hModule)
	{
		return TRUE;
	}
	else
	{
		itoa(GetLastError(), buf, 10);
		log(buf);
		return FALSE;
	}
}

IClientProcMsgObject* __stdcall CreateClientProcMsgObject()
{
	IClientProcMsgObject* pServerObj = NULL;
	CreateObjFunc pCreateObjFunc = NULL;

	pCreateObjFunc = (CreateObjFunc)::GetProcAddress(m_hModule, szCreateClientObjFunc);

	if (NULL == pCreateObjFunc)
	{
		return pServerObj;
	}
	pServerObj = pCreateObjFunc();
	m_EventHandler = new EventHandler();
	if (NULL == pServerObj)
	{
		return pServerObj;
	}
	return pServerObj;
}

void __stdcall ReleaseClientProcMsgObject(IClientProcMsgObject* pObj)
{
	ReleaseObjFunc pReleaseObjFunc = NULL;

	pReleaseObjFunc = (ReleaseObjFunc)::GetProcAddress(m_hModule, szReleaseClientObjFunc);

	if (NULL == pReleaseObjFunc)
	{
		return;
	}
	pObj->RemoveEventHandler(m_EventHandler);
	if (NULL != m_EventHandler)
	{
		delete m_EventHandler;
		m_EventHandler = NULL;
	}
	pReleaseObjFunc(pObj);
}

void __stdcall Release()
{
	if (m_hModule != NULL)
	{
		FreeLibrary(m_hModule);
		m_hModule = NULL;
	}
}


void __stdcall SetLogCallback(FunLog fun)
{
	m_loger = fun;
}
void __stdcall SetOnConnectionSuccCallback(FunOnConnectSucc fun)
{
	m_OnConnectSucc = fun;
}
void __stdcall SetOnConnectionFailedCallback(FunOnConnectFailed fun)
{
	m_OnConnectFailed = fun;
}
void __stdcall SetOnConnectionDestroyedCallback(FunOnConnectionDestroyed fun)
{
	m_OnConnectionDestroyed = fun;
}
void __stdcall SetOnReceiveMsgCallback(FunOnReceiveMsg fun)
{
	m_OnReceiveMsg = fun;
}



BOOL __stdcall IClientProcMsgObject_Initialize(IClientProcMsgObject* obj)
{
	log("IClientProcMsgObject.Initialize");
	if (!obj->Initialize())
	{
		return FALSE;
	}
	obj->AddEventHandler(m_EventHandler);
	return TRUE;
}

BOOL __stdcall IClientProcMsgObject_Connect(IClientProcMsgObject* obj, LPCWSTR lpszConnectionName)
{
	log("IClientProcMsgObject.Connect");
	LPCSTR str = CW2A(lpszConnectionName);
	BOOL ret = obj->Connect(str);
	char c[128] = { 0 };
	int length = sprintf(c, "IClientProcMsgObject.Connect, Result: %d", ret);
	log(c);
	return ret;
}

void __stdcall IClientProcMsgObject_Disconnect(IClientProcMsgObject* obj)
{
	obj->Disconnect();
}

BOOL __stdcall IClientProcMsgObject_IsConnected(IClientProcMsgObject* obj)
{
	return obj->IsConnected();
}

DWORD __stdcall IClientProcMsgObject_SendMessage(IClientProcMsgObject* obj, long lLen, const BYTE* pbySendBuf)
{
	return obj->SendMessage(lLen, pbySendBuf);
}

void __stdcall IClientProcMsgObject_AddEventHandler(IClientProcMsgObject* obj, IClientProcMsgEventHandler* pEventHandler)
{
	obj->AddEventHandler(pEventHandler);
}

void __stdcall IClientProcMsgObject_RemoveEventHandler(IClientProcMsgObject* obj, IClientProcMsgEventHandler* pEventHandler)
{
	obj->RemoveEventHandler(pEventHandler);
}

void EventHandler::OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj)
{
	log("OnConnectSucc");
	if (NULL == m_EventHandler)
	{
		return;
	}
	m_OnConnectSucc(pClientProcMsgObj);
}
void EventHandler::OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode)
{
	char c[128] = {0};
	int length = sprintf(c, "OnConnectFailed, ErrorCode: %d", dwErrorCode);
	log(c);
	if (NULL == m_EventHandler)
	{
		return;
	}
	m_OnConnectFailed(pClientProcMsgObj, dwErrorCode);
}
void EventHandler::OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj)
{
	if (NULL == m_EventHandler)
	{
		return;
	}
	m_OnConnectionDestroyed(pClientProcMsgObj);
}
void EventHandler::OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen, const BYTE* pRecvBuf)
{
	if (NULL == m_EventHandler)
	{
		return;
	}
	m_OnReceiveMsg(pClientProcMsgObj, lRecvLen, pRecvBuf);
}

void log(const char* msg)
{
	if (NULL != m_loger)
	{
		m_loger(msg);
	}
}