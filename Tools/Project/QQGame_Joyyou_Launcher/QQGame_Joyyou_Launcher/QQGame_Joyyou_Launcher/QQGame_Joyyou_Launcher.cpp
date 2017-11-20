// QQGame_Joyyou_Launcher.cpp : 定义应用程序的入口点。
//

#include "stdafx.h"
#include "QQGame_Joyyou_Launcher.h"
using std::cout;
using std::endl;

#define MAX_LOADSTRING 100

// 全局变量: 
HINSTANCE hInst;								// 当前实例
TCHAR szTitle[MAX_LOADSTRING];					// 标题栏文本
TCHAR szWindowClass[MAX_LOADSTRING];			// 主窗口类名

// 此代码模块中包含的函数的前向声明: 
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);

const TCHAR* szDllPath = _T("\\QQGameProcMsgHelper.dll");
const char* szCreateClientObjFunc = "CreateClientProcMsgObject";
const char* szReleaseClientObjFunc = "ReleaseClientProcMsgObject";

typedef IClientProcMsgObject* (*CreateObjFunc)();
typedef void(*ReleaseObjFunc)(IClientProcMsgObject*);

static int split(const std::string& str, std::vector<std::string>& ret_, std::string sep = ",")
{
	if (str.empty())
	{
		return 0;
	}

	std::string tmp;
	std::string::size_type pos_begin = str.find_first_not_of(sep);
	std::string::size_type comma_pos = 0;

	while (pos_begin != std::string::npos)
	{
		comma_pos = str.find(sep, pos_begin);
		if (comma_pos != std::string::npos)
		{
			tmp = str.substr(pos_begin, comma_pos - pos_begin);
			pos_begin = comma_pos + sep.length();
		}
		else
		{
			tmp = str.substr(pos_begin);
			pos_begin = comma_pos;
		}

		if (!tmp.empty())
		{
			ret_.push_back(tmp);
			tmp.clear();
		}
	}
	return 0;
}


int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO:  在此放置代码。

	//第一步：初步检查命令行是否符合要求：非空
	if (NULL == lpCmdLine || _tcslen(lpCmdLine) == 0)
	{
		/*
		命令行格式，确认不是符合格式要求的，请直接退出游戏，MessageBox只为Demo演示要注意，别真的一直弹哈
		*/
		//::MessageBox(NULL, "启动命令行为空时，不符合要求，请将程序终止!！", "【请开发者关注】", MB_OK);
		return FALSE;
	}
	m_strCmdline = lpCmdLine;

	//第二步：解析命令行，先用,号分隔字符串，再用等号分隔
	std::vector<std::string> vecPara;
	std::string cmdLine = (std::string)lpCmdLine;
	split(cmdLine, vecPara);

	typedef std::pair<std::string, std::string> CommandValuePair;
	std::vector<CommandValuePair> vecKey2Data;

	for (unsigned int i = 0; i < vecPara.size(); ++i)
	{
		std::vector<std::string> vecTmp;
		split(vecPara[i], vecTmp, "=");
		if (vecTmp.size() == 2)
		{
			vecKey2Data.push_back(CommandValuePair(vecTmp[0], vecTmp[1]));
		}
	}
	//--命令行参数解析完毕

	//--开始判断参数是否是指定格式的
	BOOL bHaveID = FALSE;
	BOOL bHaveKey = FALSE;
	BOOL bHaveProcPara = FALSE;

	for (unsigned int i = 0; i < vecKey2Data.size(); ++i)
	{
		if (vecKey2Data[i].first.compare("ID") == 0 && !vecKey2Data[i].second.empty())
		{
			bHaveID = TRUE;
			m_strOpenId = vecKey2Data[i].second;
			continue;
		}

		if (vecKey2Data[i].first.compare("Key") == 0 && !vecKey2Data[i].second.empty())
		{
			bHaveKey = TRUE;
			m_strOpenKey = vecKey2Data[i].second;
			continue;
		}

		if (vecKey2Data[i].first.compare("PROCPARA") == 0 && !vecKey2Data[i].second.empty())
		{
			bHaveProcPara = TRUE;
			m_strProcPara = vecKey2Data[i].second;
			continue;
		}
	}

	if (!bHaveID || !bHaveKey || !bHaveProcPara)
	{
		/*
		命令行格式，确认不是符合格式要求的，请直接退出游戏，MessageBox只为Demo演示要注意，别真的一直弹哈
		*/
		//::MessageBox(NULL, "命令行格式和数据不符合要求（参见文档说明），请将程序终止!！", "【请开发者关注】", MB_OK);
		return FALSE;
	}
	//--不符合要求，缺少数据，则直接退出主程序

	m_launcher = new Launcher();
	m_launcher->ConncetPipe();

	MSG msg;
	HACCEL hAccelTable;

	// 初始化全局字符串
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_QQGAME_JOYYOU_LAUNCHER, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// 执行应用程序初始化: 
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_QQGAME_JOYYOU_LAUNCHER));

	// 主消息循环: 
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}



//
//  函数:  MyRegisterClass()
//
//  目的:  注册窗口类。
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_QQGAME_JOYYOU_LAUNCHER));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	//wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_QQGAME_JOYYOU_LAUNCHER);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   函数:  InitInstance(HINSTANCE, int)
//
//   目的:  保存实例句柄并创建主窗口
//
//   注释: 
//
//        在此函数中，我们在全局变量中保存实例句柄并
//        创建和显示主程序窗口。
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // 将实例句柄存储在全局变量中

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   //ShowWindow(hWnd, nCmdShow);
   ShowWindow(hWnd, SW_HIDE);
   UpdateWindow(hWnd);
   m_hWnd = hWnd;
   return TRUE;
}

//
//  函数:  WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的:    处理主窗口的消息。
//
//  WM_COMMAND	- 处理应用程序菜单
//  WM_PAINT	- 绘制主窗口
//  WM_DESTROY	- 发送退出消息并返回
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);
		// TODO:  在此添加任意绘图代码...
		EndPaint(hWnd, &ps);
		break;
	case WM_DESTROY:

		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

BOOL Launcher::LoadLibrary()
{
	// 读取 REG_SZ 类型键值的代码

	HKEY  hKey = NULL;
	DWORD dwSize = 0;
	DWORD dwDataType = 0;
	char* lpValue = NULL;
	LPCTSTR const lpValueName = _T("HallDirectory");

	LONG lRet = ::RegOpenKeyEx(HKEY_LOCAL_MACHINE,
		_T("Software\\Tencent\\QQGame\\SYS"),
		0,
		KEY_QUERY_VALUE,
		&hKey);
	if (ERROR_SUCCESS != lRet)
	{
		//::MessageBox(NULL, "注册表读取失败，程序退出!", "退出提示", MB_OK);
		::PostMessage(m_hWnd, WM_QUIT, 0, 0);
		return FALSE;
	}
	// Call once RegQueryValueEx to retrieve the necessary buffer size
	::RegQueryValueEx(hKey,
		lpValueName,
		0,
		&dwDataType,
		(LPBYTE)lpValue,  // NULL
		&dwSize); // will contain the data size

	// Alloc the buffer
	lpValue = new char[dwSize + 1];
	lpValue[dwSize] = '\0';

	// Call twice RegQueryValueEx to get the value
	lRet = ::RegQueryValueEx(hKey,
		lpValueName,
		0,
		&dwDataType,
		(LPBYTE)lpValue,
		&dwSize);
	::RegCloseKey(hKey);
	if (ERROR_SUCCESS != lRet)
	{
		//::MessageBox(NULL, "注册表读取失败，程序退出!", "退出提示", MB_OK);
		::PostMessage(m_hWnd, WM_QUIT, 0, 0);
		return FALSE;
	}
	

	//取到程序的当前路径
	char szPath[MAX_PATH] = { 0 };
	//把dll路径变成绝对路径
	std::string strDllPath = lpValue;
	strDllPath += szDllPath;
	m_hModule = ::LoadLibrary(strDllPath.c_str());

	delete[] lpValue;
	if (m_hModule)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

IClientProcMsgObject* Launcher::CreateObj()
{
	IClientProcMsgObject* pServerObj = NULL;
	CreateObjFunc pCreateObjFunc = NULL;

	pCreateObjFunc = (CreateObjFunc)::GetProcAddress(m_hModule, szCreateClientObjFunc);

	if (NULL == pCreateObjFunc)
	{
		return pServerObj;
	}

	pServerObj = pCreateObjFunc();
	return pServerObj;
}

void Launcher::ReleaseObj(IClientProcMsgObject* pObj)
{
	ReleaseObjFunc pReleaseObjFunc = NULL;

	pReleaseObjFunc = (ReleaseObjFunc)::GetProcAddress(m_hModule, szReleaseClientObjFunc);

	if (NULL == pReleaseObjFunc)
	{
		return;
	}

	pReleaseObjFunc(pObj);
	return;
}

void Launcher::ConncetPipe()
{
	if (!LoadLibrary())
	{
		return;
	}

	std::string strShowMsg = "Begin connect:";
	strShowMsg += m_strProcPara;
	ShowMsg(strShowMsg.c_str());

	//创建对象
	m_pProcMsgObj = CreateObj();
	if (m_pProcMsgObj == NULL)
	{
		return;
	}

	m_pProcMsgObj->Initialize();
	m_pProcMsgObj->AddEventHandler(this);
	BOOL bSucc = m_pProcMsgObj->Connect(m_strProcPara.c_str());
	if (!bSucc)
	{
		ShowMsg("ConnectFailed");
		/*
		连接失败，请直接退出游戏，未连接上时，大厅会认为游戏未启动，所以请直接退出
		MessageBox只是为了Demo演示，别真弹出来，悄悄退出就好啦，退出功能请开发商根据自己的情况实现
		*/
		//::MessageBox(NULL, "管道连接失败，程序退出!", "退出提示", MB_OK);
		::PostMessage(m_hWnd, WM_QUIT, 0, 0);
	}

	PROCMSG_DATA stProcMsgData = { 0 };
	stProcMsgData.nCommandID = CS_REQ_NEWCONNECTION;
	SendMsgToGame(m_pProcMsgObj, &stProcMsgData);
}

void Launcher::Release()
{
	if (NULL != m_pProcMsgObj)
	{
		m_pProcMsgObj->RemoveEventHandler(this);
		m_pProcMsgObj->Disconnect();
		m_pProcMsgObj = NULL;
	}
}

void Launcher::OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj)
{
	ShowMsg("Connect Succ");
}

void Launcher::OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode)
{
	ShowMsg("ConnectFailed");
	/*
	连接失败，请直接退出游戏，未连接上时，大厅会认为游戏未启动，所以请直接退出
	MessageBox只是为了Demo演示，别真弹出来，悄悄退出就好啦，退出功能请开发商根据自己的情况实现
	*/
	//::MessageBox(NULL, "管道连接失败，程序退出!", "退出提示", MB_OK);
	PostQuitMessage(0);
}

void Launcher::OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj)
{
	ShowMsg("ConnectDestroy");
}

void Launcher::OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen, const BYTE* pRecvBuf)
{
	PROCMSG_DATA stProcMsgData = { 0 };

	int nTotalLen = 0;
	int nCopyLen = 0;

	nCopyLen = sizeof(stProcMsgData.nCommandID);
	memcpy_s(&stProcMsgData.nCommandID, nCopyLen, pRecvBuf, nCopyLen);
	nTotalLen += nCopyLen;

	nCopyLen = sizeof(stProcMsgData.nDataLen);
	memcpy_s(&stProcMsgData.nDataLen, nCopyLen, pRecvBuf + nTotalLen, nCopyLen);
	nTotalLen += nCopyLen;

	nCopyLen = stProcMsgData.nDataLen;
	memcpy_s(stProcMsgData.abyData, MAX_PROCMSG_DATABUF_LEN, pRecvBuf + nTotalLen, nCopyLen);

	switch (stProcMsgData.nCommandID)
	{
	case SC_WND_BRINGTOP:
	{
							ShowMsg("Receive msg SC_WND_BRINGTOP\n");
	}
		break;
	case SC_HALL_CMDPARA:
	{
							std::string strMsg = "Receive msg SC_HALL_CMDPARA ：";
							strMsg += (char*)stProcMsgData.abyData;
							ShowMsg(strMsg.c_str());
	}
		break;
	case SC_RESPONSE_NEWCONN:
	{
								//收到新的管道信息，再启动一个进程
								char szConnName[MAX_CONNECTION_NAME_SIZE] = { 0 };
								memcpy_s(szConnName, MAX_CONNECTION_NAME_SIZE, stProcMsgData.abyData, stProcMsgData.nDataLen);

								//再启动一个进程，将启动参数传给它
								ReStartApp(szConnName);

								m_pProcMsgObj->Disconnect();
								::PostMessage(m_hWnd, WM_QUIT, 0, 0);
	}
		break;
	case SC_RESPONSE_NEWCONN_RUFUSE:
	{
									   ShowMsg("Receive msg  SC_RESPONSE_NEWCONN_RUFUSE，拒绝新连接");
	}
		break;
	default:
		break;
	}
}

/*
这里模拟再次打开这个测试程序，如果接入方有用这个消息的需要，请按自己实际的需求来实现代码
*/
void Launcher::ReStartApp(LPCSTR lpszProc)
{
	//启动游戏
	STARTUPINFOA siStartupInfo = { 0 };
	siStartupInfo.cb = sizeof(siStartupInfo);
	PROCESS_INFORMATION stProcessInformation = { 0 };

	std::string strProc;
	std::string::size_type posProc = m_strCmdline.find("PROCPARA=") + 9;

	std::string strStartPara = m_strCmdline.substr(0, posProc);
	strStartPara += lpszProc;

	TCHAR szCmdLine[1024] = { 0 };
	std::string strCmdline = "JoyGame.exe";
	strCmdline += " ";
	strCmdline += strStartPara;

	CreateProcessA(NULL, (LPSTR)strCmdline.c_str(), NULL, NULL, FALSE
		, CREATE_NEW_CONSOLE, NULL, NULL, &siStartupInfo, &stProcessInformation);
}

void Launcher::SendMsgToGame(IClientProcMsgObject* pProcMsgObj, PROCMSG_DATA* pProcMsgData)
{
	if (NULL == pProcMsgData || NULL == pProcMsgObj)
	{
		_ASSERT(FALSE);
		return;
	}
	if (!pProcMsgObj->IsConnected())
	{
		ShowMsg("pipe is not connected");
	}

	int nBufLen = pProcMsgData->nDataLen + sizeof(pProcMsgData->nCommandID) + sizeof(pProcMsgData->nDataLen);
	DWORD dwRet = pProcMsgObj->SendMessage(nBufLen, (BYTE*)pProcMsgData);

	if (dwRet)
	{
		std::string strMsg = "SendMsgtoGame error :";
		char szErr[10] = { 0 };
		itoa(dwRet, szErr, 10);
		strMsg += szErr;
		ShowMsg(strMsg.c_str());
	}
}

void Launcher::ShowMsg(LPCTSTR lpszMsg)
{
}