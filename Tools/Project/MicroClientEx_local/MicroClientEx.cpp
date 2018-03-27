// MicroClientEx.cpp : Defines the class behaviors for the application.
//

#include "stdafx.h"
#include "MicroClientEx.h"
#include "MicroClientExDlg.h"
#include "ImpIDispatch.h"
#include "Tlhelp32.h"
#include "MicroClientLang.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


/////////////////////////////////////////////////////////////////////////////
// CMicroClientExApp

BEGIN_MESSAGE_MAP(CMicroClientExApp, CWinApp)
	//{{AFX_MSG_MAP(CMicroClientExApp)
		// NOTE - the ClassWizard will add and remove mapping macros here.
		//    DO NOT EDIT what you see in these blocks of generated code!
	//}}AFX_MSG
	ON_COMMAND(ID_HELP, CWinApp::OnHelp)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExApp construction

CMicroClientExApp::CMicroClientExApp()
{
	// TODO: add construction code here,
	// Place all significant initialization in InitInstance
}

/////////////////////////////////////////////////////////////////////////////
// The one and only CMicroClientExApp object

CMicroClientExApp theApp;

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExApp initialization

BOOL CMicroClientExApp::InitInstance()
{
//_CrtSetBreakAlloc(90358);

	const char* UNIQUE_NAME = "JoyGame";
	HANDLE hMutex = CreateMutex(NULL,NULL,UNIQUE_NAME);
	if(GetLastError() == ERROR_ALREADY_EXISTS)
	{
		HWND hOldWnd;
		if ((hOldWnd=::FindWindow(NULL,_T(syjlangstr17))) != NULL)
		{
			ShowWindow(hOldWnd,SW_RESTORE|SW_SHOWNORMAL);
			SetForegroundWindow(hOldWnd);
			return -1;
		}
		//AfxMessageBox("神游记微端正在运行");
		return -1;
	}
	ReleaseMutex(hMutex);

	m_pCustDisp = new CImpIDispatch();
	AfxEnableControlContainer();
	CoInitialize(NULL);
	//SetDialogBkColor(RGB(233,233,233),RGB(233,233,233));
	GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR           gdiplusToken;
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
	// Standard initialization
	// If you are not using these features and wish to reduce the size
	//  of your final executable, you should remove from the following
	//  the specific initialization routines you do not need.

	CMicroClientExDlg dlg;
	m_pMainWnd = &dlg;
	int nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with OK
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: Place code here to handle when the dialog is
		//  dismissed with Cancel
	}

	Gdiplus::GdiplusShutdown(gdiplusToken);
	// Since the dialog has been closed, return FALSE so that we exit the
	//  application, rather than start the application's message pump.
	return FALSE;
}
int CMicroClientExApp::ExitInstance() 
{
	// TODO: Add your specialized code here and/or call the base class
	if(m_pCustDisp)
	{
		delete m_pCustDisp;
		m_pCustDisp = NULL;
	}
	CoUninitialize();
	
	return CWinApp::ExitInstance();
}

HWND CMicroClientExApp::GetWindowHandleByPID(DWORD dwProcessID)
{
	HWND h = GetTopWindow(0);
	while(h)
	{
		DWORD pid = 0;
		DWORD dwTheardId = GetWindowThreadProcessId(h,&pid);

		if (dwTheardId != 0)
		{
			if (pid == dwProcessID)
			{
				return h;
			}
		}

		h = GetNextWindow(h,GW_HWNDNEXT);

	}
	return 0;
}