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

#include "LogUtil.h"

#include <Shlwapi.h>

#pragma comment(lib,"Shlwapi.lib")
#pragma comment(lib,"zlib.lib")
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//XML配置相关节点KEY
const CString cfRoot = "Root";
const CString cfUrl = "Url";
const CString cfInfo = "Info";
const CString cfVersion = "Version";
const CString cfVroot = "Vroot";
const CString cfDownurl = "Downurl";

//验证的AUTH与SIGN字符串记录
static CString authUrl = "";

int totalNum;
float loadedNum;
vector<CString> fileContainer;
vector<CString> md5Container;
vector<CString> zipFilesContainer;
CString root = GetConfigValue(CString(CONFIG), cfRoot);
CString vroot = GetConfigValue(CString(CONFIG), cfVroot);
int singleFileCount = 0;
bool bDownloadFlag = false;
bool callbyjs = false;
const int MAX_DOWNLOAD_TIMES = 10; //最大下载次数
const int BUF_SIZE = 256; 
char szLogBuf[BUF_SIZE] = {0}; //日志缓冲区
CString starturl;//外部传入
int greenflag = 0;//是否绿色版0否1是


//////////////////////////////////////////////////////////////////////////
HANDLE hMutex1;//测试写log用

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

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExDlg message handlers

BOOL CMicroClientExDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	ModifyStyleEx(WS_EX_APPWINDOW,WS_EX_TOOLWINDOW);
	
	//接受命令行参数
	int argc = 0;

	LPWSTR *argv = ::CommandLineToArgvW(::GetCommandLineW(),&argc);

	for (int i = 0; i < argc; i++)
	{
		CString msg = argv[i];
		if (msg == "-url")
		{
			starturl = argv[i+1];
			//AfxMessageBox(starturl);
			break;
		}
	}
	// TODO: Add extra initialization here
// 	CString testurl = "www.t4game.com/auth=aaa&sign=bbb&channel=&ser=fff&version=eee";
// 	CString testaddurl = FindurlParam(testurl,"channel","&");
// 	AfxMessageBox(testaddurl);

	hFuncInst = LoadLibrary("User32.DLL"); 
	BOOL bRet=FALSE;
	if(hFuncInst) 
		UpdateLayeredWindow=(MYFUNC)GetProcAddress(hFuncInst, "UpdateLayeredWindow");
	else
	{
		AfxMessageBox("User32.dll ERROR!");
		exit(0);
	}
	
	
	//初始化gdiplus的环境
	// Initialize GDI+.
	m_Blend.BlendOp=0; //theonlyBlendOpdefinedinWindows2000
	m_Blend.BlendFlags=0; //nothingelseisspecial...
	m_Blend.AlphaFormat=1; //...
	m_Blend.SourceConstantAlpha=255;//AC_SRC_ALPHA
	bgImage = Gdiplus::Image::FromFile(L"res\\bg.png");
	if(!bgImage)
	{
		WriteLog("load bg.png failed.");
	}
	m_BakWidth  =bgImage->GetWidth();
	m_BakHeight =bgImage->GetHeight();
	MoveWindow(430,125,m_BakWidth,m_BakHeight);

	closeBtn = new PngBtn();
	closeBtn->InitButton(1190,197,CLOSE_NORMAL_STATE,CLOSE_OVER_STATE,CLOSE_ACTIVE_STATE);
	miniBtn = new PngBtn();
	miniBtn->InitButton(1166,182,MINI_NORMAL_STATE,MINI_OVER_STATE,MINI_ACTIVE_STATE);

	progressBarMain = new ProgressBar();
	progressBarMain->InitPogressBar(320,548,100,PROGRESSBAR_BG,PROGRESSBAR_FILL,PROGRESSBAR_EMPTY,PROGRESSBAR_KNOB);
	progressBarMain->InitOffset(0,0,0,1,0,0);
	
	progressBarVice = new ProgressBar();
	progressBarVice->InitPogressBar(320,558,100,PROGRESSBAR_BG,PROGRESSBAR_FILL,PROGRESSBAR_EMPTY,PROGRESSBAR_KNOB);
	progressBarVice->InitOffset(0,0,0,1,0,0);
	
	container = new ContainerDialog();
	container->Create(IDD_DIALOG_WEBBROWSER,this);


	readyToUse = TRUE;

	//container->MoveWindow(700,360,650,390); 
	container->MoveWindow(526,214,650,380); 
	if (starturl == "")
	{
		starturl = GetConfigValue(CString(CONFIG), cfUrl);
	}
	else
	{
		greenflag = 1;
	}

	GetServerVision();
	//container->m_WebBrowser.Navigate2(starturl);
	
	//container->m_WebBrowser.Navigate("http://172.18.1.79", NULL, NULL, NULL, NULL);
	container->m_WebBrowser.Navigate(starturl, NULL, NULL, NULL, NULL);
	//ntainer->m_WebBrowser.SetWindowPos( NULL,0,0,671,404,NULL);
	container->m_WebBrowser.MoveWindow(0,0,671,404);
	container->ShowWindow(SW_HIDE);
	RePaintWindow();

	//任务栏右键菜单
	//ModifyStyle(WS_BORDER,WS_SYSMENU|WS_MINIMIZEBOX|WS_MAXIMIZEBOX); 
	ModifyStyle(NULL,WS_MINIMIZEBOX);
	//不同分辨率桌面居中显示
	CWnd::CenterWindow(); 

	this->SetWindowText(syjlangstr0);

	ModifyStyle(WS_CAPTION,0);//去掉窗体的标题栏

	//复制配置文件，读取参数

	char szConfigFile[MAX_PATH] = "C:\\config.ini";
	
	char szNewConfigFile[MAX_PATH] = {'\0'};
	GetModuleFileName(NULL,szNewConfigFile,MAX_PATH);
	char* p = szNewConfigFile;
	while(strchr(p,'\\'))
	{
		p = strchr(p,'\\');
		p++;
	}
	*p = '\0';
	
	m_strLocalPath = szNewConfigFile;
	m_strLocalPath = m_strLocalPath + "ShenYouJi\\";
	//AfxMessageBox(m_strLocalPath);
	

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
// 		str.Format("第%d次,miniBtn_pos_x:%d  miniBtn_pos_y:%d",testi,miniBtn -> pos_x,miniBtn -> pos_y);
// 		AfxMessageBox(str);
// 	}
	
	graph.DrawImage(miniBtn->GetCurrentState(), miniBtn -> pos_x, miniBtn -> pos_y,miniBtn->width, miniBtn->height);
	graph.DrawImage(closeBtn->GetCurrentState(),closeBtn->pos_x, closeBtn->pos_y,closeBtn->width,closeBtn->height);
	//graph.DrawImage(progressBarMain->progressFill,progressBarMain->pos_x + progressBarMain->processOffset.content_offset_X,progressBarMain->pos_y + progressBarMain->processOffset.content_offset_Y, (int)(loading_procgrss_main* progressBarMain->processImagRect.width_content),progressBarMain->processImagRect.height_content);
	//graph.DrawImage(progressBarVice->progressFill,progressBarVice->pos_x + progressBarVice->processOffset.content_offset_X,progressBarVice->pos_y + progressBarVice->processOffset.content_offset_Y, (int)(loading_procgrss_vice* progressBarMain->processImagRect.width_content),progressBarVice->processImagRect.height_content);
	
	StringFormat stringformat;
	
	SolidBrush myBrush(Color(255,255,255,255)); // 白色 不透明
	FontFamily famali(L"宋体");
	Font myFont(&famali,10);
	
// 	if(!downloadFlag)
// 	{
// 		
// 		WCHAR * info_text = ConverIntToString((int)(loading_procgrss_main*100),downloadplanflg);
// 		graph.DrawString(info_text,GetInfoLenght(info_text),&myFont,PointF(500,530),&stringformat,&myBrush);
// 	}
	
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
	
	if(closeBtn->CalculateState(point.x,point.y) || miniBtn->CalculateState(point.x,point.y))Invalidate();
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


void CMicroClientExDlg::InvokeCallByJsJudgeClient()//用于js判断是否为新微端
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

void CMicroClientExDlg::InvokeCallWebClient(CString url)//用于判断是否js微端
{
	if (!callbyjs)
	{
		container->m_WebBrowser.MoveWindow(10,50,600,300);
		IvockeDownload((LPVOID)NULL);
		
		container->ModifyInitRunDialog();
		ShowWindow(SW_HIDE);
		//container->ShowWindow(SW_HIDE);
		callbyjs = true;
	}
	
}

UINT CMicroClientExDlg::SendInfo(LPVOID lpParam)
{
	//获得统计页面地址，长度<10则基本不可能OK，遂不发送统计，针对内网服务
	CString url = GetConfigValue(CString(CONFIG), cfInfo);
	if (url.GetLength() <= 10)
		return 0;
	
	//拆分auth
	CString auth = authUrl.Left(authUrl.Find('&'));
	//拼接GET
	CString version = "&version=WEBMC_";
	CString verSub = GetConfigValue(CString(CONFIG), cfVersion);
	CString installed = "&installed=2";
	CString macText = "&mac=";
	CString macAddr = GetMyMacAddr();
	//最终地址
	CString bingo = url + auth + version + verSub + installed + macText + macAddr;
	//准备发送
	CHttpClient conn;
	char* resp = new char[512];
	conn.HttpGet(bingo, NULL, resp);
	WriteLog( "oops launcher stat is done!" );
	delete[] resp;
	return 0;
}

UINT CMicroClientExDlg::IvockeDownload(LPVOID lpParam)
{	
	//确定能进游戏后，启动统计线程，发送微端统计给服务器，BYMM。
	AfxBeginThread(SendInfo,(LPVOID)NULL,THREAD_PRIORITY_NORMAL,0,0,NULL);
	return 1;
}

//读取XML文件(内存中读取)
void CMicroClientExDlg::ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container)
{
	try
	{
		fileContainer.clear();
		md5Container.clear();
		TiXmlDocument *myDocument = new TiXmlDocument();

		myDocument->Parse(bufferXML,0,TIXML_DEFAULT_ENCODING);
		TiXmlElement * pChild = myDocument->FirstChildElement();
	
		if ( pChild->NoChildren() )
		{
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
			return ;
		}
	}
	catch (...)
	{
		return;
	}
	return;
}

CString CMicroClientExDlg::ReadSerXml(CString key)
{
	try
	{
		TiXmlDocument *myDocument = new TiXmlDocument();
		myDocument->Parse(bufferXML,0,TIXML_DEFAULT_ENCODING);
		TiXmlElement* element = myDocument->FirstChildElement( (LPSTR) (LPCTSTR) key);
		CString str = element->FirstChild()->Value();
		delete []bufferXML;
		bufferXML = NULL;
		return str;
	}
	catch (...)
	{
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
	if(miniBtn->CalculatePressDown(point.x,point.y))
	{
		//this->ShowWindow(SW_MINIMIZE);
		this->ShowWindow(SW_HIDE);
		::SendMessage(container->GetSafeHwnd(),WM_SYSCOMMAND,SC_MINIMIZE,NULL);
		btnDownflag = TRUE;
	}
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
		::SendMessage(container->GetSafeHwnd(),MSGCOUNTERMOVE,x,y);
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

		
		if (container->GetSafeHwnd())
		{
			::SendMessage(container->GetSafeHwnd(),MSGWEBREFRESH,0,0);
 		}		
		return TRUE;
 	}

	if (pMsg->message == WM_KEYDOWN && GetAsyncKeyState(VK_F11)<0)
	{
		
		if (container->GetSafeHwnd())
		{
			::SendMessage(container->GetSafeHwnd(),MSGWEBFULLSRC,0,0);
 		}		
		return TRUE;
	}

	if((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_SPACE))
	{
		return false;
	}

	return CDialog::PreTranslateMessage(pMsg);
}

/* 
函数功能：读取ini配置文件
函数参数：keyName 参数关键字
		  retBuf  结果存储空间
		  size    存储空间长度
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

//根据进程名查找ID
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
//用于分析版本区间version为版本号，versionnum 为要存储解析出来的版本号
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


//找到url中响应的参数
/*
url为网页抛进来的url
urlparam为所要找的参数
urlparamsign为url中的参数间隔符
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


void CMicroClientExDlg::GetServerVision()
{
	if (greenflag == 0)
	{
		//下载版本校验文件
		if (vroot != "")
		{
			singleFileCount = 1;
			bDownloadFlag = Download(vroot + CONFIG,m_strLocalPath + CONFIG,TRUE);
			while(!bDownloadFlag)
			{
				if(singleFileCount++ == MAX_DOWNLOAD_TIMES)
				{
					memset(szLogBuf,'\0',BUF_SIZE);
					sprintf(szLogBuf,"Download File Failed: %s",vroot + CONFIG);
					WriteLog(szLogBuf);
					AfxMessageBox(syjlangstr5);
					exit(-1);
				}
				bDownloadFlag = Download(vroot + CONFIG,m_strLocalPath +CONFIG,TRUE);
			}
			CString m_serversion = ReadSerXml(cfVersion);
			CString m_localversion = GetConfigValue(CString(CONFIG),cfVersion);
			int serversion[VERSIONNUM];
			int localversion[VERSIONNUM];
			
			if (m_serversion == "" && m_localversion == "")
			{
				memset(szLogBuf,'\0',BUF_SIZE);
				sprintf(szLogBuf,syjlangstr6);
				WriteLog(szLogBuf);
			}
			else if (m_serversion == "")
			{
				memset(szLogBuf,'\0',BUF_SIZE);
				sprintf(szLogBuf,syjlangstr7);
				WriteLog(szLogBuf);
			}
			else if (m_localversion == "")
			{
				memset(szLogBuf,'\0',BUF_SIZE);
				sprintf(szLogBuf,syjlangstr8);
				WriteLog(szLogBuf);
			}
			else
			{		
				memset(localversion,0,VERSIONNUM);
				AnalyVersion(m_localversion,localversion);
				memset(serversion,0,VERSIONNUM);
				AnalyVersion(m_serversion,serversion);
				
				if (localversion[0] != serversion[0])
				{
					AfxMessageBox(syjlangstr9);
					CString m_downloadurl = GetConfigValue(CString(CONFIG),cfDownurl);
					ShellExecute(NULL,"open",m_downloadurl,NULL,NULL,SW_SHOWNORMAL);
					exit(-1);
				}
				else
				{
					if (localversion[1] != serversion[1])
					{
						if(AfxMessageBox(syjlangstr10,MB_YESNO) == IDYES)
						{				
							CString m_downloadurl = GetConfigValue(CString(CONFIG),cfDownurl);
							ShellExecute(NULL,"open",m_downloadurl,NULL,NULL,SW_SHOWNORMAL);
							exit(-1);
						}
					}
				}
			}
		}
	}

}

bool CMicroClientExDlg::Download(const CString& strFileURLInServer,const CString & strFileLocalFullPath,bool record)//存放到本地的路径 
{ 
	ASSERT(strFileURLInServer != ""); 
	ASSERT(strFileLocalFullPath != ""); 
	CInternetSession session; 
	CHttpConnection* pHttpConnection = NULL; 
	CHttpFile* pHttpFile = NULL; 
	CString strServer, strObject; 
	INTERNET_PORT wPort; 
	bool bReturn = false; 
	DWORD dwType; 
	const int nTimeOut = 10000; 
	session.SetOption(INTERNET_OPTION_CONNECT_TIMEOUT, nTimeOut); //重试之间的等待延时 
	session.SetOption(INTERNET_OPTION_DATA_RECEIVE_TIMEOUT,nTimeOut);
	session.SetOption(INTERNET_OPTION_DATA_SEND_TIMEOUT,nTimeOut);
	session.SetOption(INTERNET_OPTION_CONNECT_RETRIES, 5); //重试次数 
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
			CString temp = strFileLocalFullPath;
			for(int i = 0 ; i < temp.GetLength();++i)
			{
				if(temp[i] == '\\')
				{
					//创建文件目录
					CreateDirectory(temp.Left(i),NULL);
				}	
			}
			HANDLE hFile;
			if (!record)
			{
				hFile = CreateFile(strFileLocalFullPath, GENERIC_WRITE, 
					FILE_SHARE_WRITE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 
					NULL); //创建本地文件 
				if(hFile == INVALID_HANDLE_VALUE) 
				{ 
					pHttpFile->Close(); 
					pHttpConnection->Close(); 
					session.Close(); 

					memset(szLogBuf,'\0',256);
					sprintf(szLogBuf,"CreateFile %s Failed.",strFileLocalFullPath);
					WriteLog(szLogBuf);
					return false; 
				} 
			}
			char szInfoBuffer[1000]; //返回消息 
			memset(szInfoBuffer,'\0',1000);
			DWORD dwInfoBufferSize = sizeof(szInfoBuffer); 
			BOOL bResult = FALSE; 
			bResult = pHttpFile->QueryInfo(HTTP_QUERY_CONTENT_LENGTH, 
				(void*)szInfoBuffer,&dwInfoBufferSize,NULL); 
		
			DWORD dwFileSize = (DWORD)atoi(szInfoBuffer); //文件长度 
			const int BUFFER_LENGTH = 1024; 
			pszBuffer = new char[BUFFER_LENGTH]; //读取文件的缓冲 
			char* pszBufferAll = new char[dwFileSize]; //读取文件的缓冲 
			DWORD dwWrite, dwTotalWrite; 
			dwWrite = dwTotalWrite = 0; 
			UINT nRead = pHttpFile->Read(pszBuffer, BUFFER_LENGTH); //读取服务器上数据 
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
		
			if (!record)
			{
				if(!WriteFile(hFile, pszBufferAll, dwFileSize, &dwWrite, NULL)) //写到本地文件
				{
					memset(szLogBuf,'\0',256);
					sprintf(szLogBuf,"WriteFile %s Failed.",strFileLocalFullPath);
					WriteLog(szLogBuf);
					return false;
				}
				FlushFileBuffers(hFile);
				CloseHandle(hFile); 

				zipFilesContainer.push_back(strFileLocalFullPath);
			}
			else
			{
				bufferXML = new char[dwFileSize]; //读取xml文件到内存
				memcpy(bufferXML,pszBufferAll,dwFileSize);
			}
			loading_procgrss_vice = 1;
			delete[]pszBufferAll; 
			delete[]pszBuffer; 
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


