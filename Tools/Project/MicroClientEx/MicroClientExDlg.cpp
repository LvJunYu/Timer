// MicroClientExDlg.cpp : implementation file
//GetConfigFile

#include "stdafx.h"
#include "MicroClientEx.h"
#include "MicroClientExDlg.h"
#include "common_function.h"
#include "CountChecksum.h"
#include "zlib.h"
#include "zconf.h"
#include <afxinet.h>
#include "Tlhelp32.h"
#include "HttpClient.h"
#include "MicroClientLang.h"
#include <fstream>
#include "DecompressStream.h"

#include <process.h>

#include "LogUtil.h"
#include <stdio.h> 
#include <afxcoll.h>

#include <Shlwapi.h>
using namespace std;

#pragma comment(lib,"Shlwapi.lib")
#pragma comment(lib,"zlib.lib")
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//XML������ؽڵ�KEY
const CString cfRoot = "Root";
const CString cfUrl = "Url";
const CString cfInfo = "Info";
const CString cfVersion = "Version";
const CString cfVroot = "Vroot";
const CString cfDownurl = "Downurl";
const CString cfZip = ".dat";
//��֤��AUTH��SIGN�ַ�����¼
static CString authUrl = "";

bool isUseMultiThread = true;
const int MAXThread = 4;
CRITICAL_SECTION g_csLock;

CMapStringToOb* localFilesMap=NULL;

int totalNum;
int loadedNum;
int processInx;
vector<CString> fileContainer;
vector<CString> md5Container;
vector<CString> zipFilesContainer;
CString root = GetConfigValue(CString(CONFIG), cfRoot);
CString vroot = GetConfigValue(CString(CONFIG), cfVroot);
int singleFileCount = 0;
bool bDownloadFlag = true;
bool callbyjs = false;
const int MAX_DOWNLOAD_TIMES = 10; //������ش���
const int BUF_SIZE = 256; 
char szLogBuf[BUF_SIZE] = {0}; //��־������
CString starturl;//�ⲿ����
int greenflag = 0;//�Ƿ���ɫ��0��1��
bool hasStart = false;
int MaxRetryCount = 3;
const TCHAR* szDllPath = _T("\\QQGameProcMsgHelper.dll");
const char* szCreateClientObjFunc = "CreateClientProcMsgObject";
const char* szReleaseClientObjFunc = "ReleaseClientProcMsgObject";

typedef IClientProcMsgObject* (*CreateObjFunc)();
typedef void(*ReleaseObjFunc)(IClientProcMsgObject*);
WCHAR szInfo[32];

WCHAR pszbuf[16];
bool showProgress = false;
bool showHeart = true;

CString notUncompressPath = "JoyGame_Data\\StreamingAssets\\";


//////////////////////////////////////////////////////////////////////////
HANDLE hMutex1;//����дlog��

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExDlg dialog
bool readyToUse = FALSE;
CMicroClientExDlg::CMicroClientExDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CMicroClientExDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CMicroClientExDlg)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	//_CrtSetBreakAlloc(5231);
}

void CMicroClientExDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CMicroClientExDlg)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CMicroClientExDlg, CDialog)
	//{{AFX_MSG_MAP(CMicroClientExDlg)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONUP()
	ON_WM_LBUTTONDOWN()
	ON_WM_MOVE()
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

void Quit()
{
	exit(-1);
}

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExDlg message handlers

BOOL CMicroClientExDlg::OnInitDialog()
{
	m_launcher = this;
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	//ModifyStyleEx(WS_EX_APPWINDOW,WS_EX_TOOLWINDOW);
	ModifyStyleEx(0,WS_EX_APPWINDOW);
	
	this->SetWindowText(syjlangstr17);
	
	//���������в���
	int argc = __argc;

#ifndef _DEBUG
	if (argc != 2)
	{
		Quit();
	}
	LPTSTR lpCmdLine = __argv[1];
	//��һ������������������Ƿ����Ҫ�󣺷ǿ�
	if (NULL == lpCmdLine || _tcslen(lpCmdLine) == 0)
	{
		/*
		�����и�ʽ��ȷ�ϲ��Ƿ��ϸ�ʽҪ��ģ���ֱ���˳���Ϸ��MessageBoxֻΪDemo��ʾҪע�⣬�����һֱ����
		*/
		::MessageBox(NULL, "����������Ϊ��ʱ��������Ҫ���뽫������ֹ!��", "���뿪���߹�ע��", MB_OK);
		Quit();
	}
	m_strCmdline = lpCmdLine;

	//�ڶ��������������У�����,�ŷָ��ַ��������õȺŷָ�
	std::vector<std::string> vecPara;
	std::string cmdLine = (std::string)lpCmdLine;
	split(cmdLine, vecPara, ",");

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
	//--�����в����������

	//--��ʼ�жϲ����Ƿ���ָ����ʽ��
	BOOL bHaveID = FALSE;
	BOOL bHaveKey = FALSE;
	BOOL bHaveProcPara = FALSE;

	for (i = 0; i < vecKey2Data.size(); ++i)
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
		�����и�ʽ��ȷ�ϲ��Ƿ��ϸ�ʽҪ��ģ���ֱ���˳���Ϸ��MessageBoxֻΪDemo��ʾҪע�⣬�����һֱ����
		*/
		::MessageBox(NULL, "�����и�ʽ�����ݲ�����Ҫ�󣨲μ��ĵ�˵�������뽫������ֹ!��", "���뿪���߹�ע��", MB_OK);
		Quit();
	}
	//--������Ҫ��ȱ�����ݣ���ֱ���˳�������

	m_launcher->ConncetPipe();
#endif

//	starturl = "file://c:/Temp/index.html";
	// TODO: Add extra initialization here
// 	CString testurl = "www.t4game.com/auth=aaa&sign=bbb&channel=&ser=fff&version=eee";
// 	CString testaddurl = FindurlParam(testurl,"channel","&");
// 	AfxMessageBox(testaddurl);

	hFuncInst = ::LoadLibrary("User32.DLL"); 
	BOOL bRet=FALSE;
	if(hFuncInst) 
		UpdateLayeredWindow=(MYFUNC)GetProcAddress(hFuncInst, "UpdateLayeredWindow");
	else
	{
		AfxMessageBox("User32.dll ERROR!");
		exit(0);
	}
	
	loadedNum = 0;
	//��ʼ��gdiplus�Ļ���
	// Initialize GDI+.
	m_Blend.BlendOp=0; //theonlyBlendOpdefinedinWindows2000
	m_Blend.BlendFlags=0; //nothingelseisspecial...
	m_Blend.AlphaFormat=1; //...
	m_Blend.SourceConstantAlpha=255;//AC_SRC_ALPHA
	bgImage = Gdiplus::Image::FromFile(RES_IMG_BG);
	if(!bgImage)
	{
		WriteLog("load bg.png failed.");
	}
	m_BakWidth  =bgImage->GetWidth();
	m_BakHeight =bgImage->GetHeight();
	MoveWindow(430,125,m_BakWidth,m_BakHeight);

	closeBtn = new PngBtn();
	closeBtn->InitButton(762, 28,CLOSE_NORMAL_STATE,CLOSE_OVER_STATE,CLOSE_ACTIVE_STATE);
	//miniBtn = new PngBtn();
	//miniBtn->InitButton(990,20,MINI_NORMAL_STATE,MINI_OVER_STATE,MINI_ACTIVE_STATE);
	//startBtn = new PngBtn();
	//startBtn->InitButton(940,420,START_NORMAL_STATE,START_OVER_STATE,START_ACTIVE_STATE);

	progressBarMain = new ProgressBar();
	progressBarMain->InitPogressBar(154,400,100,PROGRESSBAR_BG,PROGRESSBAR_FILL,PROGRESSBAR_EMPTY,PROGRESSBAR_KNOB);
	progressBarMain->InitOffset(0,0,0,1,0,0);
	
	//progressBarVice = new ProgressBar();
	//progressBarVice->InitPogressBar(580,633,100,PROGRESSBAR_BG,PROGRESSBAR_FILL,PROGRESSBAR_EMPTY,PROGRESSBAR_KNOB);
	//progressBarVice->InitOffset(0,0,0,1,0,0);
	
	//container = new ContainerDialog();
	//container->Create(IDD_DIALOG_WEBBROWSER,this);


	readyToUse = TRUE;

	if (starturl == "")
	{
		//starturl = GetConfigValue(CString(CONFIG), cfUrl);
	}
	else
	{
		greenflag = 1;
	}

	//GetServerVision();
	//container->m_WebBrowser.Navigate2(starturl);
	
	//container->m_WebBrowser.Navigate("www.baidu.com", NULL, NULL, NULL, NULL);
	//ntainer->m_WebBrowser.SetWindowPos( NULL,0,0,671,404,NULL);
	//container->m_WebBrowser.MoveWindow(0,0, 800,430);
	//container->MoveWindow(240, 80, 800, 430);
	//container->ShowWindow(SW_HIDE);
	RePaintWindow();

	//�������Ҽ��˵�
	//ModifyStyle(WS_BORDER,WS_SYSMENU|WS_MINIMIZEBOX|WS_MAXIMIZEBOX); 
	ModifyStyle(NULL,WS_MINIMIZEBOX);
	//��ͬ�ֱ������������ʾ
	CWnd::CenterWindow(); 

	//ModifyStyle(WS_CAPTION,0);//ȥ������ı�����

	//���������ļ�����ȡ����

	//char szConfigFile[MAX_PATH] = "C:\\config.ini";
	
	char szNewConfigFile[MAX_PATH] = {'\0'};
	GetModuleFileName(NULL,szNewConfigFile,MAX_PATH);
	char* p = szNewConfigFile;
	while(strchr(p,'\\'))
	{
		p = strchr(p,'\\');
		p++;
	}
	*p = '\0';
	
	m_strLocalRootPath = szNewConfigFile;
	
	m_strLocalPath = szNewConfigFile;
	m_strLocalPath = m_strLocalPath + "Game\\";
	
	m_strTempPath = szNewConfigFile;
	m_strTempPath = m_strTempPath + "Download\\";
	//AfxMessageBox(m_strLocalPath);
	
/*
	if (greenflag == 0)
	{
		strcat(szNewConfigFile,"\\gameconfig.ini");
		if (!PathFileExists(szNewConfigFile))
		{
			CopyFile(szConfigFile,szNewConfigFile,FALSE);
			
			if(-1 == GetFileAttributes(szConfigFile))
			{
				memset(szConfigFile,'\0',MAX_PATH);
				strcpy(szConfigFile,szNewConfigFile);
			}
			
			if(strcmp(szConfigFile,"C:\\config.ini") == 0)
			{
				DeleteFile(szConfigFile);
			}
		}
		
	}
	*/

	AfxBeginThread(IvockeDownload,(LPVOID)NULL,THREAD_PRIORITY_NORMAL,0,0,NULL);
	return TRUE;  // return TRUE  unless you set the focus to a control
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.
void CMicroClientExDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		//SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		RePaintWindow();
		CDialog::OnPaint();
	}
}



// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CMicroClientExDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

int testi = 0;

void CMicroClientExDlg::RePaintWindow()
{
	int Transparent=255;
	HDC hdcTemp=GetDC()->m_hDC;
	m_hdcMemory=CreateCompatibleDC(hdcTemp);
	HBITMAP hBitMap=CreateCompatibleBitmap(hdcTemp,m_BakWidth,m_BakHeight);
	SelectObject(m_hdcMemory,hBitMap);
	if(Transparent<0||Transparent>100)	Transparent=100;
	
	m_Blend.SourceConstantAlpha=int(Transparent*2.55);//1~255
	HDC hdcScreen=::GetDC (m_hWnd);
	RECT rct;
	GetWindowRect(&rct);
	POINT ptWinPos={rct.left,rct.top};
	Graphics graph(m_hdcMemory);
	
	Point points[] = { Point(0, 0), 
					Point(m_BakWidth, 0), 
					Point(0, m_BakHeight)
					};
	
	Status status = graph.DrawImage(bgImage, points, 3);

// 	CString str;
// 
// 	if (testi < 10)
// 	{
// 		testi++;
// 		str.Format("��%d��,miniBtn_pos_x:%d  miniBtn_pos_y:%d",testi,miniBtn -> pos_x,miniBtn -> pos_y);
// 		AfxMessageBox(str);
// 	}
	
	//graph.DrawImage(miniBtn->GetCurrentState(), miniBtn -> pos_x, miniBtn -> pos_y,miniBtn->width, miniBtn->height);
	graph.DrawImage(closeBtn->GetCurrentState(),closeBtn->pos_x, closeBtn->pos_y,closeBtn->width,closeBtn->height);
	//graph.DrawImage(startBtn->GetCurrentState(),startBtn->pos_x, startBtn->pos_y,startBtn->width,startBtn->height);

	graph.DrawImage(progressBarMain->progressFill, progressBarMain->pos_x + progressBarMain->processOffset.content_offset_X, progressBarMain->pos_y + progressBarMain->processOffset.content_offset_Y, 0, 0, (int)(loading_procgrss_main* progressBarMain->processImagRect.width_content),progressBarMain->processImagRect.height_content, UnitPixel);
	int offsetKnobX = -21;
	int offsetKnobY = -10;
	if(showHeart)
	{
		graph.DrawImage(progressBarMain->progressKnob, progressBarMain->pos_x + offsetKnobX + (int)(loading_procgrss_main* progressBarMain->processImagRect.width_content), progressBarMain->pos_y + offsetKnobY, 0, 0, progressBarMain->processImagRect.width_knob,progressBarMain->processImagRect.height_knob, UnitPixel);
	}
	//graph.DrawImage(progressBarVice->progressFill,progressBarVice->pos_x + progressBarVice->processOffset.content_offset_X,progressBarVice->pos_y + progressBarVice->processOffset.content_offset_Y, (int)(loading_procgrss_vice* progressBarMain->processImagRect.width_content),progressBarVice->processImagRect.height_content);
	
/*
	StringFormat stringformat;
	
	SolidBrush myBrush(Color(255,255,255,255)); // ��ɫ ��͸��
	FontFamily famali(L"����");
	Font myFont(&famali,10);
	
 	if(!downloadFlag)
 	{
 		
 		WCHAR * info_text = ConverIntToString((int)(loading_procgrss_main*100),downloadplanflg);
 		//graph.DrawString(info_text,GetInfoLenght(info_text),&myFont,PointF(500,530),&stringformat,&myBrush);
 	}
	*/
	
	graph.SetSmoothingMode(SmoothingModeAntiAlias);
	graph.SetInterpolationMode(InterpolationModeHighQualityBicubic);
    
	GraphicsPath myPath;

	swprintf(pszbuf,L"%d/%d\0", loadedNum, totalNum);

    FontFamily fontFamily(L"����");
    int emSize = 18;
    StringFormat strformat;
	
	Pen pen(Color(80, 101, 42), 3);
	SolidBrush brush(Color(255, 255, 255));

	if(showProgress)
	{
		myPath.AddString(pszbuf, wcslen(pszbuf), &fontFamily,
			FontStyleRegular, emSize, Point(752, 398), &strformat);
		graph.DrawPath(&pen, &myPath);
		graph.FillPath(&brush, &myPath);
	}

	emSize = 16;
	GraphicsPath myPath2;
	int centerX = 460;
	int len = wcslen(szInfo);
	myPath2.AddString(szInfo, wcslen(szInfo), &fontFamily,
		FontStyleRegular, emSize, Point(centerX - emSize * len/2, 428), &strformat);
	graph.DrawPath(&pen, &myPath2);
	graph.FillPath(&brush, &myPath2);
	
	
	SIZE sizeWindow={m_BakWidth,m_BakHeight};
	POINT ptSrc={0,0};
	DWORD dwExStyle=GetWindowLong(m_hWnd,GWL_EXSTYLE);
	if((dwExStyle&0x80000)!=0x80000)
		SetWindowLong(m_hWnd,GWL_EXSTYLE,dwExStyle^0x80000);
	
	BOOL bRet=FALSE;
	bRet= UpdateLayeredWindow( m_hWnd,hdcScreen,&ptWinPos,
		&sizeWindow,m_hdcMemory,&ptSrc,0,&m_Blend,2);
	
	graph.ReleaseHDC(m_hdcMemory);
	::ReleaseDC(m_hWnd,hdcScreen);
	hdcScreen=NULL;
	::ReleaseDC(m_hWnd,hdcTemp);
	hdcTemp=NULL;
	DeleteObject(hBitMap);
	DeleteDC(m_hdcMemory);
	m_hdcMemory=NULL;
}

void CMicroClientExDlg::OnMouseMove(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	
	if(closeBtn->CalculateState(point.x, point.y)/*
		|| miniBtn->CalculateState(point.x, point.y)
		|| startBtn->CalculateState(point.x, point.y)*/)Invalidate();
	CDialog::OnMouseMove(nFlags, point);	
}

WCHAR* CMicroClientExDlg::ConverIntToString(int num,int printftype)
{
	WCHAR * sz = new WCHAR[100];
	if(printftype == 0)
	{
		swprintf(sz,L"syjlangstr1 %d%% (%d/%d)\0",num,(int)loadedNum,totalNum);
	}
	else if (printftype == 1)
	{
		swprintf(sz,L"syjlangstr2...\0");
	}
	else if (printftype == 2)
	{
		swprintf(sz,L"syjlangstr3...\0");
	}
	else if (printftype == 3)
	{
		swprintf(sz,L"syjlangstr4...\0");
	}
	return sz;
}

int CMicroClientExDlg::GetInfoLenght(WCHAR * _array)
{
	int count = 0;
	for(int i = 0 ; i < 100 ;++i)
	{
		if(_array[i] != '\0')
		{
			count ++;
		}
		else
		{	
			//count ++;
			break;
		}
	}
	return count;
}

void CMicroClientExDlg::OnLButtonUp(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
// 	if(closeBtn->CalculatePressDown(point.x,point.y))this->OnOK();
// 	if(miniBtn->CalculatePressDown(point.x,point.y))this->ShowWindow(SW_MINIMIZE);
// 	::SendMessage(GetSafeHwnd(),WM_SYSCOMMAND,0xF012,0); 
	CDialog::OnLButtonUp(nFlags, point);
}


void CMicroClientExDlg::InvokeCallByJsJudgeClient()//����js�ж��Ƿ�Ϊ��΢��
{
	
}

void CMicroClientExDlg::InvokeCallGameThread(CString url)//
{
// 	AfxMessageBox(url);
// 	ShowWindow(SW_HIDE);
// 	//container->m_WebBrowser.MoveWindow(3,30,644,357);
// 	container->m_WebBrowser.MoveWindow(10,50,600,300);
// 	
// 	container->ModifyInitRunDialog();
}

void CMicroClientExDlg::InvokeCallWebClient(CString url)//�����ж��Ƿ�js΢��
{
	if (!callbyjs)
	{	
		IvockeDownload((LPVOID)NULL);
		
		ShowWindow(SW_HIDE);
		//container->ShowWindow(SW_HIDE);
		callbyjs = true;
	}
	
}

UINT CMicroClientExDlg::SendInfo(LPVOID lpParam)
{
	return 0;
	/*
	//���ͳ��ҳ���ַ������<10�����������OK���첻����ͳ�ƣ������������
	CString url = GetConfigValue(CString(CONFIG), cfInfo);
	if (url.GetLength() <= 10)
		return 0;
	
	//���auth
	CString auth = authUrl.Left(authUrl.Find('&'));
	//ƴ��GET
	CString version = "&version=WEBMC_";
	CString verSub = GetConfigValue(CString(CONFIG), cfVersion);
	CString installed = "&installed=2";
	CString macText = "&mac=";
	CString macAddr = GetMyMacAddr();
	//���յ�ַ
	CString bingo = url + auth + version + verSub + installed + macText + macAddr;
	//׼������
	CHttpClient conn;
	char* resp = new char[512];
	conn.HttpGet(bingo, NULL, resp);
	WriteLog( "oops launcher stat is done!" );
	delete[] resp;
	return 0;
	*/
}

UINT __stdcall CMicroClientExDlg::DownloadFile(LPVOID lpParam)
{
	int index = 0;
	while(true)
	{
		EnterCriticalSection(&g_csLock);
		index = processInx;
		processInx++;
		LeaveCriticalSection(&g_csLock);

		if(index >= totalNum)
		{
			break;
		}
		CString fileName = fileContainer[index];
		CString fileMD5 = md5Container[index];
		
		CString filePath = m_strLocalPath + fileName;
		CString fileTempPath = m_strTempPath + fileName + cfZip;
		CString fileUrl = root + m_serversion + "/" + fileName + cfZip;

		EnterCriticalSection(&g_csLock);
		localFilesMap->RemoveKey(fileName);
		LeaveCriticalSection(&g_csLock);

		fileUrl.Replace('\\', '/');
		WIN32_FIND_DATA  FindFileData;
		HANDLE hFind;
		hFind = FindFirstFile(filePath, &FindFileData);
		bDownloadFlag = hFind != INVALID_HANDLE_VALUE;
		int tryCount = 0;
		for	(;tryCount<MaxRetryCount;tryCount++) 
		{
			if(bDownloadFlag)
			{
				CString localMd5 = CMD5Checksum::GetMD5(filePath);
				if(localMd5 == fileMD5)
				{
					cout<<fileUrl+" success";
					break;
				}
				else
				{
					cout<<fileUrl+" redownload";
				}
			}
			if (fileName.Left(notUncompressPath.GetLength()) == notUncompressPath)
			{
				bDownloadFlag = Download(fileUrl, true, filePath, false, fileTempPath);
			}
			else
			{
				bDownloadFlag = Download(fileUrl, true, filePath, true, fileTempPath);
			}
			if(!bDownloadFlag)
			{
				cout<<fileUrl+" download failed";
			}
		}
		if(tryCount >= MaxRetryCount)
		{
			AfxMessageBox("���������Ƿ�����");
			exit(-1);
		}
		//add lock
		EnterCriticalSection(&g_csLock);
		loadedNum++;
		loading_procgrss_main=((float)loadedNum) / totalNum;
		m_launcher->Invalidate();
		LeaveCriticalSection(&g_csLock);
	}
	return 0;
}

UINT CMicroClientExDlg::IvockeDownload(LPVOID lpParam)
{	
	//ȷ���ܽ���Ϸ������ͳ���̣߳�����΢��ͳ�Ƹ���������BYMM��
	//AfxBeginThread(SendInfo,(LPVOID)NULL,THREAD_PRIORITY_NORMAL,0,0,NULL);
	loading_procgrss_main = 0;
	loading_procgrss_vice = 0;

	singleFileCount = 1;
	
	swprintf(szInfo, L"%s", L"���ڻ�ȡ���°汾��Ϣ");
	m_launcher->Invalidate();
	if (CheckNeedUpdate())
	{
		swprintf(szInfo, L"%s", L"���ڻ�ȡ�����ļ��б�");
		m_launcher->Invalidate();
		bDownloadFlag = false;
		while(!bDownloadFlag)
		{
			if(singleFileCount++ == MAX_DOWNLOAD_TIMES)
			{
				memset(szLogBuf,'\0',BUF_SIZE);
				sprintf(szLogBuf,"Download File Failed: %s", root + m_serversion + "/" + MANIFEST +cfZip);
				WriteLog(szLogBuf);
				AfxMessageBox("���������Ƿ�����");
				exit(-1);
			}
			bDownloadFlag = Download(root + m_serversion + "/" + MANIFEST +cfZip, true, m_strLocalPath + MANIFEST, true, m_strTempPath + MANIFEST + cfZip);
		}

		vector<CString> localFiles;
		GetFiles(m_strLocalPath, localFiles);
		int pathStartPos = m_strLocalPath.GetLength();
		CString ignorePath = "JoyGame_Data\\JoyYou\\";
		localFilesMap = new CMapStringToOb(1024);
		for (int i=0; i<localFiles.size(); i++)
		{
			CString fullPathName = localFiles[i];
			CString filePathName = fullPathName.Mid(pathStartPos);
			if (filePathName.Left(ignorePath.GetLength()) == ignorePath)
			{
				continue;
			}
			localFilesMap->SetAt(filePathName, NULL);
		}
		
		swprintf(szInfo, L"%s", L"���������ļ�");
		//vector<CString> files;
		//vector<CString> md5s;
		
		m_launcher->Invalidate();
		ReadXmlFile(m_strLocalPath +"/" + MANIFEST, fileContainer, md5Container);
	
		totalNum = fileContainer.size();
		showProgress = true;
		if(isUseMultiThread)
		{
			HANDLE hThread[MAXThread];
			unsigned int uiThreadIdAry[MAXThread];
			InitializeCriticalSection(&g_csLock);
			unsigned int * uiThreadId = uiThreadIdAry;
			for (int i=0; i<MAXThread; i++)
			{
				hThread[i] = (HANDLE)_beginthreadex(NULL, 0, &DownloadFile, NULL, 0, uiThreadId);
				uiThreadId++;
			} 
			
			DWORD dwRet = WaitForMultipleObjects(MAXThread, hThread, true, INFINITE);  
			if ( dwRet == WAIT_TIMEOUT )  
			{  
				for (int i=0; i<MAXThread; i++)
				{
					TerminateThread(hThread[i],0); 
				}
			}
			DeleteCriticalSection(&g_csLock);
		}
		else
		{
			for(i = 0; i < fileContainer.size();++i)
			{
				CString filePath = m_strLocalPath + fileContainer[i];
				CString fileTempPath = m_strTempPath + fileContainer[i] + cfZip;
				CString fileUrl = root + m_serversion + "/" + fileContainer[i] + cfZip;
				localFilesMap->RemoveKey(fileContainer[i]);
				fileUrl.Replace('\\', '/');
				WIN32_FIND_DATA  FindFileData;
				HANDLE hFind;
				hFind = FindFirstFile(filePath, &FindFileData);
				bDownloadFlag = hFind != INVALID_HANDLE_VALUE;
				int tryCount = 0;
				for	(;tryCount<MaxRetryCount;tryCount++) 
				{
					if(bDownloadFlag)
					{
						CString localMd5 = CMD5Checksum::GetMD5(filePath);
						if(localMd5 == md5Container[i])
						{
							cout<<fileUrl+" success";
							break;
						}
						else
						{
							cout<<fileUrl+" md5 fail";
						}
					}
					
					if (fileContainer[i].Left(notUncompressPath.GetLength()) == notUncompressPath)
					{
						bDownloadFlag = Download(fileUrl, true, filePath, false, fileTempPath);
					}
					else
					{
						bDownloadFlag = Download(fileUrl, true, filePath, true, fileTempPath);
					}
					
					if(!bDownloadFlag)
					{
						cout<<fileUrl+" download fail";
					}
				}
				if(tryCount >= MaxRetryCount)
				{
					memset(szLogBuf,'\0',BUF_SIZE);
					sprintf(szLogBuf,"Download File Failed: %s", fileUrl);
					WriteLog(szLogBuf);
					AfxMessageBox("���������Ƿ�����");
					exit(-1);
				}
				
				loadedNum++;
				loading_procgrss_main=((float)loadedNum) / totalNum;
				m_launcher->Invalidate();
			}
		}
		CopyFile(m_strTempPath + CONFIG, CString(CONFIG), FALSE);
		
		CString key;
		CObject* obj;
		for (POSITION pos = localFilesMap->GetStartPosition(); pos != NULL;)
		{
			localFilesMap->GetNextAssoc(pos, key, (CObject*&)obj);
			CString fileToDelete = m_strLocalPath + key;
			DeleteFile(fileToDelete);
		}
		DeleteDirs(m_strTempPath);
	}
	//AfxMessageBox("ok");
	showProgress = false;
	showHeart = false;
	swprintf(szInfo, L"%s", L"����������Ϸ");
	m_launcher->Invalidate();
	m_launcher->StartGame();
	loading_procgrss_main = 1;
	loading_procgrss_vice = 1;
	if(localFilesMap != NULL)
	{
		localFilesMap->RemoveAll();
		delete localFilesMap;
	}
	return 1;
}

//��ȡXML�ļ�
void CMicroClientExDlg::ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container)
{
	TiXmlDocument *myDocument = NULL;
	try
	{
		fileContainer.clear();
		md5Container.clear();
		
		myDocument = new TiXmlDocument(szFileName);
		myDocument->LoadFile();
		if(myDocument->Error())
		{
			delete myDocument;
			return;
		}

		//TiXmlDocument *myDocument = new TiXmlDocument();
		//myDocument->Parse(bufferXML,0,TIXML_DEFAULT_ENCODING);

		TiXmlElement * pChild = myDocument->FirstChildElement();
	
		if ( pChild->NoChildren() )
		{
			delete myDocument;
			return;
		}
		else if( ! pChild->NoChildren() )
		{
		
			TiXmlElement *pFirst = pChild->FirstChildElement();
			
			while ( NULL != pFirst )
			{
				TiXmlElement *urlElement = pFirst->FirstChildElement();
				CString fileName = CString(urlElement->FirstChild()->Value());
				CString md5Code = CString(urlElement->NextSibling()->FirstChild()->Value());
				pFirst = pFirst->NextSiblingElement();
				fileContainer.push_back(fileName);
				md5Container.push_back(md5Code);
			}
			delete []bufferXML;
			bufferXML = NULL;
			delete myDocument;
			return ;
		}
	}
	catch (...)
	{
	}
	delete myDocument;
	return;
}

CString CMicroClientExDlg::ReadSerXml(CString key)
{
	TiXmlDocument *myDocument = NULL;
	try
	{
		myDocument = new TiXmlDocument();
		myDocument->Parse(bufferXML,0,TIXML_DEFAULT_ENCODING);
		TiXmlElement* element = myDocument->FirstChildElement( (LPSTR) (LPCTSTR) key);
		CString str = element->FirstChild()->Value();
		delete []bufferXML;
		bufferXML = NULL;
		delete myDocument;
		return str;
	}
	catch (...)
	{
		delete myDocument;
		return "";
	}
}

void CMicroClientExDlg::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	
	BOOL btnDownflag = FALSE;
	if(closeBtn->CalculatePressDown(point.x,point.y))
	{
		this->OnOK();
		btnDownflag = TRUE;
	}
	/*
	if(miniBtn->CalculatePressDown(point.x,point.y))
	{
		this->ShowWindow(SW_MINIMIZE);
		//this->ShowWindow(SW_HIDE);
		//::SendMessage(container->GetSafeHwnd(),WM_SYSCOMMAND,SC_MINIMIZE,NULL);
		btnDownflag = TRUE;
	}
	if(!hasStart && startBtn->CalculatePressDown(point.x,point.y))
	{
		AfxBeginThread(IvockeDownload,(LPVOID)NULL,THREAD_PRIORITY_NORMAL,0,0,NULL);
		//IvockeDownload((LPVOID)NULL);
		hasStart = true;
	}
	*/
	if(btnDownflag == FALSE)
	{
		::SendMessage(GetSafeHwnd(),WM_SYSCOMMAND,0xF012,0); 
	}
	CDialog::OnLButtonDown(nFlags, point);
}

void CMicroClientExDlg::OnMove(int x, int y) 
{
	CDialog::OnMove(x, y);
	if(readyToUse)
	{
		//::SetWindowPos(container->GetSafeHwnd(),NULL,x+250,y+122,0,0,SWP_NOSIZE);
		//::SetWindowPos(container->GetSafeHwnd(),NULL,x+250,y+122,0,0,SWP_NOACTIVATE|SWP_NOSIZE);
		//::SendMessage(container->GetSafeHwnd(),MSGCOUNTERMOVE,x,y);
	}
	// TODO: Add your message handler code here
}


void CMicroClientExDlg::OnOK() 
{
	// TODO: Add extra validation here
	CDialog::OnOK();
}

BOOL CMicroClientExDlg::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	if ((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_RETURN))
	{
		return false;
	}

	if((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_ESCAPE))
	{
		return false;
	}

	if(pMsg->message == WM_KEYDOWN && GetAsyncKeyState(VK_F5)<0)
	{

		
		//if (container->GetSafeHwnd())
		{
			//::SendMessage(container->GetSafeHwnd(),MSGWEBREFRESH,0,0);
 		}		
		return TRUE;
 	}

	if (pMsg->message == WM_KEYDOWN && GetAsyncKeyState(VK_F11)<0)
	{
		
		//if (container->GetSafeHwnd())
		{
			//::SendMessage(container->GetSafeHwnd(),MSGWEBFULLSRC,0,0);
 		}		
		return TRUE;
	}

	if((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_SPACE))
	{
		return false;
	}
	RePaintWindow();
	return CDialog::PreTranslateMessage(pMsg);
}

/* 
�������ܣ���ȡini�����ļ�
����������keyName �����ؼ���
		  retBuf  ����洢�ռ�
		  size    �洢�ռ䳤��
 */
BOOL CMicroClientExDlg::GetIniConfigParam(char* keyName, char* retBuf, int size,char* szPath)
{
	if(GetFileAttributes(szPath) == -1)
	{
		return FALSE;
	}

	char* appName = "CONFIG"; 
	int ret = GetPrivateProfileString(appName, keyName, "", retBuf, size, szPath);
	if(ret > 0)
	{
		return TRUE;
	}
	else
	{
		return FALSE;
	}
}

//���ݽ���������ID
DWORD CMicroClientExDlg::GetProcessByName(LPCTSTR name)
{
	PROCESSENTRY32 pe;
	DWORD id = 0;
	HANDLE hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS,0);
	pe.dwSize = sizeof(PROCESSENTRY32);
	if(!Process32First(hSnapshot,&pe))
		return 0;
	while(1)
	{
		pe.dwSize = sizeof(PROCESSENTRY32);
		if(Process32Next(hSnapshot,&pe) == FALSE)
			break;
		if(strcmp(pe.szExeFile,name) == 0)
		{
			id = pe.th32ProcessID;
			break;
		}
	}
	
	CloseHandle(hSnapshot);
	return id;
}

//////////////////////////////////////////////////////////////////////////
//���ڷ����汾����versionΪ�汾�ţ�versionnum ΪҪ�洢���������İ汾��
void CMicroClientExDlg::AnalyVersion(CString version,int* versionnum)
{
	int m_i = 0,m_length;
	CString buf;
	int m_num;
	CString versioncopy = version + ".";
	
	while(1)
	{
		m_num = versioncopy.Find('.');
		if (m_num == -1)
		{
			break;
		}
		buf = versioncopy.Left(m_num);
		versionnum[m_i] = atoi(_T(buf));
		m_length = versioncopy.Delete(0,m_num+1);
		versioncopy = versioncopy.Right(m_length);
		m_i++;
		
	}
}


//�ҵ�url����Ӧ�Ĳ���
/*
urlΪ��ҳ�׽�����url
urlparamΪ��Ҫ�ҵĲ���
urlparamsignΪurl�еĲ��������
*/
CString CMicroClientExDlg::FindurlParam(CString url,CString urlparam,CString urlparamsign)
{
	CString paramvalue = "";
	int urlparamlength;
	int urlparamsignlocationbegin;
	int urlparamsingnlocationend;
	int urldeletelength;
	CString urlcopy;
	CString buf;

	urlcopy = url + urlparamsign;
	urlparamlength = urlparam.GetLength();

	while (urlcopy.Find(_T(urlparamsign)) >= 0)
	{
		urlparamsignlocationbegin = urlcopy.Find(_T(urlparamsign));
		urldeletelength = urlcopy.Delete(0,urlparamsignlocationbegin+1);
		urlcopy = urlcopy.Right(urldeletelength);
		urlparamsingnlocationend = urlcopy.Find(_T(urlparamsign));
		if (urlparamsingnlocationend < 0)
		{
			break;
		}
		buf = urlcopy.Left(urlparamsingnlocationend);
		if (buf.Left(urlparamlength) == urlparam)
		{
			paramvalue = buf.Right(buf.GetLength() - urlparamlength - 1);
			break;
		}

	}

	return paramvalue;



}

BOOL CMicroClientExDlg::PreCreateWindow(CREATESTRUCT& cs) 
{
	// TODO: Add your specialized code here and/or call the base class
	//cs.style |= WS_CLIPCHILDREN;
	return CDialog::PreCreateWindow(cs);
}



bool CMicroClientExDlg::Download(const CString& strFileURLInServer, bool record, const CString & strFileLocalFullPath, bool uncompress, const CString& tempPath)//��ŵ����ص�·�� 
{ 
	ASSERT(strFileURLInServer != "");
	CInternetSession session; 
	CHttpConnection* pHttpConnection = NULL; 
	CHttpFile* pHttpFile = NULL; 
	CString strServer, strObject; 
	INTERNET_PORT wPort; 
	bool bReturn = false; 
	DWORD dwType; 
	const int nTimeOut = 100000; 
	session.SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, nTimeOut); //����֮��ĵȴ���ʱ 
	session.SetOption(INTERNET_OPTION_DATA_RECEIVE_TIMEOUT,nTimeOut*60);
	session.SetOption(INTERNET_OPTION_DATA_SEND_TIMEOUT,nTimeOut);
	session.SetOption(INTERNET_OPTION_CONNECT_RETRIES, 5); //���Դ���
	char* pszBuffer = NULL; 
	char szLogBuf[256] = {0};

	try 
	{ 
		AfxParseURL(strFileURLInServer, dwType, strServer, strObject, wPort); 
		DeleteUrlCacheEntry(/*strFileURLInServer*/strServer + strObject);
		pHttpConnection = session.GetHttpConnection(strServer, wPort); 
		pHttpFile = pHttpConnection->OpenRequest(CHttpConnection::HTTP_VERB_GET, strObject,NULL,1,NULL,NULL,INTERNET_FLAG_RELOAD|INTERNET_FLAG_DONT_CACHE); 
		if(pHttpFile->SendRequest() == FALSE) 
		{
			return false; 
		}
		DWORD dwStateCode; 
		pHttpFile->QueryInfoStatusCode(dwStateCode);
		if(dwStateCode == HTTP_STATUS_OK) 
		{ 
			CString writePath;
			if (uncompress)
			{
				writePath = tempPath;
			}
			else
			{
				writePath = strFileLocalFullPath;
			}
			CreateDirs(writePath);
			HANDLE hFile;
			if (record)
			{
				hFile = CreateFile(writePath, GENERIC_WRITE, 
					FILE_SHARE_WRITE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 
					NULL); //���������ļ� 
				if(hFile == INVALID_HANDLE_VALUE) 
				{ 
					pHttpFile->Close(); 
					pHttpConnection->Close(); 
					session.Close(); 

					memset(szLogBuf,'\0',256);
					sprintf(szLogBuf,"CreateFile %s Failed.", writePath);
					WriteLog(szLogBuf);
					return false; 
				} 
			}
			char szInfoBuffer[1000]; //������Ϣ 
			memset(szInfoBuffer,'\0',1000);
			DWORD dwInfoBufferSize = sizeof(szInfoBuffer); 
			BOOL bResult = FALSE; 
			bResult = pHttpFile->QueryInfo(HTTP_QUERY_CONTENT_LENGTH, 
				(void*)szInfoBuffer,&dwInfoBufferSize,NULL); 
		
			DWORD dwFileSize = (DWORD)atoi(szInfoBuffer); //�ļ����� 
			const int BUFFER_LENGTH = 1024; 
			pszBuffer = new char[BUFFER_LENGTH]; //��ȡ�ļ��Ļ��� 
			char* pszBufferAll = new char[dwFileSize]; //��ȡ�ļ��Ļ��� 
			DWORD dwWrite, dwTotalWrite; 
			dwWrite = dwTotalWrite = 0; 
			UINT nRead = pHttpFile->Read(pszBuffer, BUFFER_LENGTH); //��ȡ������������ 
			int progressCount = 0;
			loading_procgrss_vice = 0;
			while(nRead > 0) 
			{ 
				if(dwFileSize > 0 && progressCount < dwFileSize/BUFFER_LENGTH + 1)
				{
					loading_procgrss_vice = GetProgressNum(dwFileSize,BUFFER_LENGTH,progressCount);
				}
				for (int i =0;i<nRead;++i)
				{
					pszBufferAll[i+BUFFER_LENGTH*progressCount] = pszBuffer[i];
				}
				progressCount++;
				dwTotalWrite += dwWrite; 
				nRead = pHttpFile->Read(pszBuffer, BUFFER_LENGTH); 
			} 
		
			if (record)
			{
				if(!WriteFile(hFile, pszBufferAll, dwFileSize, &dwWrite, NULL)) //д�������ļ�
				{
					memset(szLogBuf,'\0',256);
					sprintf(szLogBuf,"WriteFile %s Failed.", writePath);
					WriteLog(szLogBuf);
					return false;
				}
				FlushFileBuffers(hFile);
				CloseHandle(hFile); 

				if (uncompress)
				{
					CString dest = strFileLocalFullPath;
					if(!UnCompressFile(dest, writePath))
					{
						return false;
					}
				}
			}
			else
			{
				bufferXML = new char[dwFileSize]; //��ȡxml�ļ����ڴ�
				memcpy(bufferXML,pszBufferAll,dwFileSize);
			}
			loading_procgrss_vice = 1;
			delete[] pszBufferAll;
			delete[] pszBuffer;
			pszBuffer = NULL; 
			pszBufferAll = NULL;
			bReturn = true; 
		} 
	} 
	catch(CException* e) 
	{ 
		TCHAR tszErrString[256]; 
		e->GetErrorMessage(tszErrString, sizeof(tszErrString)); 
		TRACE(_T("Download XSL error! URL: %s,Error: %s"), strFileURLInServer, tszErrString); 
		e->Delete(); 
	}
	catch(...)
	{
		memset(szLogBuf,'\0',256);
		sprintf(szLogBuf,"Download %s Failed. exception",strFileLocalFullPath);
		WriteLog(szLogBuf);
	}

	if(pszBuffer != NULL) 
	{ 
		delete[]pszBuffer; 
	} 
	if(pHttpFile != NULL) 
	{ 
		pHttpFile->Close(); 
		delete pHttpFile; 
	} 
	if(pHttpConnection != NULL) 
	{ 
		pHttpConnection->Close(); 
		delete pHttpConnection; 
	} 
	session.Close(); 
	return bReturn; 
} 

bool CMicroClientExDlg::UnCompressFile(CString DestName, CString SrcName)
{
	CreateDirs(DestName);
	ofstream destS(DestName, ios::out|ios::trunc|ios::binary);
	if(!destS.is_open())
	{
		return false;
	}
	DecompressStream dds(destS);
	ifstream srcS(SrcName, ios::in|ios::binary);
	if(!srcS.is_open())
	{
		dds.close();
		return false;
	}
/*
    char buff[4096];
    int n=0;
    while(!srcS.eof())//û�е��ļ�ĩβ
    {
        srcS.read(buff,4096);
        n=int(srcS.gcount());//ʵ�ʶ�ȡ��
        dds.write(buff,n);
    }
	*/
	
	dds<<srcS.rdbuf();
	dds.close();
	srcS.close();
	return true;
}

bool CMicroClientExDlg::CheckNeedUpdate()
{
	bDownloadFlag = false;
	while(!bDownloadFlag)
	{
		if(singleFileCount++ == MAX_DOWNLOAD_TIMES)
		{
			memset(szLogBuf,'\0',BUF_SIZE);
			sprintf(szLogBuf,"Download File Failed: %s",vroot);
			WriteLog(szLogBuf);
			AfxMessageBox("���������Ƿ�����");
			exit(-1);
		}
		bDownloadFlag = Download(vroot, true, m_strTempPath + CONFIG, false, "");
	}

	m_serversion = GetConfigValue(m_strTempPath + CONFIG, cfVersion);
	//�������ص�ַΪ�������ĵ�ַ
	root = GetConfigValue(m_strTempPath + CONFIG, cfRoot);
	CString m_localversion = GetConfigValue(CString(CONFIG), cfVersion);
	int serversion[VERSIONNUM] = {0};
	int localversion[VERSIONNUM] = {0};
	
	if (m_serversion == "")
	{
		memset(szLogBuf,'\0',BUF_SIZE);
		sprintf(szLogBuf,syjlangstr7);
		WriteLog(szLogBuf);
		AfxMessageBox("��ȡ�汾��Ϣʧ��");
		exit(-1);
	}
	else if (m_localversion == "")
	{
		memset(szLogBuf,'\0',BUF_SIZE);
		sprintf(szLogBuf,syjlangstr8);
		WriteLog(szLogBuf);
		AfxMessageBox("��ȡ�汾��Ϣʧ��");
		exit(-1);
	}
	else
	{
		AnalyVersion(m_localversion,localversion);
		AnalyVersion(m_serversion,serversion);
		for (int i=0; i<VERSIONNUM; i++)
		{
			if(serversion[i]>localversion[i])
			{
				return true;
			}
			else if (serversion[i]<localversion[i])
			{
				return false;
			}
		}
	}
	return false;
}

/*
int CMicroClientExDlg::UnCompressFile(CString DestName, CString SrcName)
{
	char* uSourceBuffer = new char[20 * 1024 *1024];
	char* uDestBuffer = new char[20 * 1024 *1024];
    FILE* fp3;  //������ѹ�ļ����ļ�ָ��  
    FILE* fp4;  //������ѹ�ļ����ļ�ָ��  
    int err;  
    //errno_t err; //��������Ķ���  
                 //������ѹ���ļ�  
    //err = fopen_s(&fp3, SrcName, "r+b");  
    fp3 = fopen(SrcName, "r+b");  
    if (fp3==NULL)  
    {  
        printf("�ļ���ʧ��! \n");  
        return 1;  
    }  
  
    //��ȡ����ѹ�ļ��Ĵ�С  
    long ucur = ftell(fp3);  
    fseek(fp3, 0L, SEEK_END);  
    long ufileLength = ftell(fp3);  
    fseek(fp3, ucur, SEEK_SET);  
  
    //��ȡ�ļ���buffer  
    fread(uSourceBuffer, ufileLength, 1, fp3);
    fclose(fp3);  
  
    uLongf uDestBufferLen = 20 * 1024 *1024;//�˴�������Ҫ�㹻�������ɽ�ѹ��������  
    //��ѹ��buffer�е�����  
    err = uncompress((Bytef*)uDestBuffer, (uLongf*)&uDestBufferLen, (Bytef*)uSourceBuffer, (uLongf)ufileLength);  
  
    if (err != Z_OK)  
    {  
        //cout << "��ѹ��ʧ�ܣ�" << err << endl;  
        return 1;  
    }  
  
    //����һ���ļ�����д���ѹ���������  
    //err = fopen_s(&fp4, DestName, "wb");  
    fp4 = fopen(DestName, "wb");  
    if (fp4==NULL)  
    {  
        printf("��ѹ���ļ�����ʧ��! \n");  
        return 1;  
    }  
  
    printf("д������... \n");  
    fwrite(uDestBuffer, uDestBufferLen, 1, fp4);  
    fclose(fp4);
	delete[] uSourceBuffer;
	delete[] uDestBuffer;
    return 0;  
}
*/

BOOL CMicroClientExDlg::LoadLibrary()
{
	// ��ȡ REG_SZ ���ͼ�ֵ�Ĵ���

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
		//::MessageBox(NULL, "ע����ȡʧ�ܣ������˳�!", "�˳���ʾ", MB_OK);
		Quit();
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
		//::MessageBox(NULL, "ע����ȡʧ�ܣ������˳�!", "�˳���ʾ", MB_OK);
		Quit();
		return FALSE;
	}
	

	//ȡ������ĵ�ǰ·��
	char szPath[MAX_PATH] = { 0 };
	//��dll·����ɾ���·��
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

IClientProcMsgObject* CMicroClientExDlg::CreateObj()
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

void CMicroClientExDlg::ReleaseObj(IClientProcMsgObject* pObj)
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

void CMicroClientExDlg::ConncetPipe()
{
	if (!LoadLibrary())
	{
		return;
	}

	std::string strShowMsg = "Begin connect:";
	strShowMsg += m_strProcPara;
	ShowMsg(strShowMsg.c_str());

	//��������
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
		����ʧ�ܣ���ֱ���˳���Ϸ��δ������ʱ����������Ϊ��Ϸδ������������ֱ���˳�
		MessageBoxֻ��Ϊ��Demo��ʾ�����浯�����������˳��ͺ������˳������뿪���̸����Լ������ʵ��
		*/
		::MessageBox(NULL, "�ܵ�����ʧ�ܣ������˳�!", "�˳���ʾ", MB_OK);
		Quit();
	}
}

void CMicroClientExDlg::Release()
{
	if (NULL != m_pProcMsgObj)
	{
		m_pProcMsgObj->RemoveEventHandler(this);
		m_pProcMsgObj->Disconnect();
		m_pProcMsgObj = NULL;
	}
}

void CMicroClientExDlg::OnConnectSucc(IClientProcMsgObject* pClientProcMsgObj)
{
	ShowMsg("Connect Succ");
}

void CMicroClientExDlg::OnConnectFailed(IClientProcMsgObject* pClientProcMsgObj, DWORD dwErrorCode)
{
	ShowMsg("ConnectFailed");
	/*
	����ʧ�ܣ���ֱ���˳���Ϸ��δ������ʱ����������Ϊ��Ϸδ������������ֱ���˳�
	MessageBoxֻ��Ϊ��Demo��ʾ�����浯�����������˳��ͺ������˳������뿪���̸����Լ������ʵ��
	*/
	//::MessageBox(NULL, "�ܵ�����ʧ�ܣ������˳�!", "�˳���ʾ", MB_OK);
	PostQuitMessage(0);
}

void CMicroClientExDlg::OnConnectionDestroyed(IClientProcMsgObject* pClientProcMsgObj)
{
	ShowMsg("ConnectDestroy");
}

void CMicroClientExDlg::OnReceiveMsg(IClientProcMsgObject* pClientProcMsgObj, long lRecvLen, const BYTE* pRecvBuf)
{
	PROCMSG_DATA stProcMsgData = { 0 };

	int nTotalLen = 0;
	int nCopyLen = 0;

	nCopyLen = sizeof(stProcMsgData.nCommandID);
	memcpy(&stProcMsgData.nCommandID, pRecvBuf, nCopyLen);
	nTotalLen += nCopyLen;

	nCopyLen = sizeof(stProcMsgData.nDataLen);
	memcpy(&stProcMsgData.nDataLen, pRecvBuf + nTotalLen, nCopyLen);
	nTotalLen += nCopyLen;

	nCopyLen = stProcMsgData.nDataLen;
	memcpy(stProcMsgData.abyData, pRecvBuf + nTotalLen, nCopyLen);

	switch (stProcMsgData.nCommandID)
	{
	case SC_WND_BRINGTOP:
	{
		ShowMsg("Receive msg SC_WND_BRINGTOP\n");
		if (HMODULE hUser32 = ::GetModuleHandle("user32"))
		{
			typedef void (WINAPI * PROCSWITCHTOTHISWINDOW)(HWND, BOOL);
			if (PROCSWITCHTOTHISWINDOW procSwitchToThisWindow =
					reinterpret_cast<PROCSWITCHTOTHISWINDOW>(::GetProcAddress(hUser32,
					"SwitchToThisWindow")) )
			{
				procSwitchToThisWindow(AfxGetApp()->m_pMainWnd->m_hWnd, TRUE);
			}
			else
			{
				::SetForegroundWindow(AfxGetApp()->m_pMainWnd->m_hWnd);
			}
		}
	}
		break;
	case SC_HALL_CMDPARA:
	{
							std::string strMsg = "Receive msg SC_HALL_CMDPARA ��";
							strMsg += (char*)stProcMsgData.abyData;
							ShowMsg(strMsg.c_str());
	}
		break;
	case SC_RESPONSE_NEWCONN:
	{
								//�յ��µĹܵ���Ϣ��������һ������
								char szConnName[MAX_CONNECTION_NAME_SIZE] = { 0 };
								memcpy(szConnName, stProcMsgData.abyData, stProcMsgData.nDataLen);

								//������һ�����̣�����������������
								ReStartApp(szConnName);

								m_pProcMsgObj->Disconnect();
								Quit();
	}
		break;
	case SC_RESPONSE_NEWCONN_RUFUSE:
	{
									   ShowMsg("Receive msg  SC_RESPONSE_NEWCONN_RUFUSE���ܾ�������");
	}
		break;
	default:
		break;
	}
}

/*
����ģ���ٴδ�������Գ���������뷽���������Ϣ����Ҫ���밴�Լ�ʵ�ʵ�������ʵ�ִ���
*/
void CMicroClientExDlg::ReStartApp(LPCSTR lpszProc)
{
	//������Ϸ
	STARTUPINFOA siStartupInfo = { 0 };
	siStartupInfo.cb = sizeof(siStartupInfo);
	PROCESS_INFORMATION stProcessInformation = { 0 };

	std::string strProc;
	std::string::size_type posProc = m_strCmdline.find("PROCPARA=") + 9;

	std::string strStartPara = m_strCmdline.substr(0, posProc);
	strStartPara += lpszProc;

	TCHAR szCmdLine[1024] = { 0 };
	std::string strCmdline = "Game\\JoyGame.exe";
	strCmdline += " ";
	strCmdline += strStartPara;

	CreateProcessA(NULL, (LPSTR)strCmdline.c_str(), NULL, NULL, FALSE
		, CREATE_NEW_CONSOLE, NULL, NULL, &siStartupInfo, &stProcessInformation);
}

void CMicroClientExDlg::SendMsgToGame(IClientProcMsgObject* pProcMsgObj, PROCMSG_DATA* pProcMsgData)
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

	sprintf(szLogBuf,"SendMessage, len: %d, realLen: %d, ret: %d", nBufLen, sizeof(*pProcMsgData), dwRet);
	WriteLog(szLogBuf);

	if (dwRet)
	{
		std::string strMsg = "SendMsgtoGame error :";
		char szErr[10] = { 0 };
		itoa(dwRet, szErr, 10);
		strMsg += szErr;
		ShowMsg(strMsg.c_str());
	}
}

void CMicroClientExDlg::ShowMsg(LPCTSTR lpszMsg)
{
}

void CMicroClientExDlg::StartGameOnDebug()
{
	return;
	//������Ϸ
	STARTUPINFOA siStartupInfo = { 0 };
	siStartupInfo.cb = sizeof(siStartupInfo);
	PROCESS_INFORMATION stProcessInformation = { 0 };
	TCHAR szCmdLine[1024] = { 0 };
	std::string strCmdline = "Game\\JoyGame.exe";

	CreateProcessA(NULL, (LPSTR)strCmdline.c_str(), NULL, NULL, FALSE
		, CREATE_NEW_CONSOLE, NULL, NULL, &siStartupInfo, &stProcessInformation);
	Quit();
}

void CMicroClientExDlg::StartGame()
{
#ifdef _DEBUG
	StartGameOnDebug();
	return;
#endif
	PROCMSG_DATA stProcMsgData = { 0 };
	stProcMsgData.nCommandID = CS_REQ_NEWCONNECTION;
	SendMsgToGame(m_pProcMsgObj, &stProcMsgData);
}

