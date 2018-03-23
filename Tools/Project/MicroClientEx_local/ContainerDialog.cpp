// ontainerDialog.cpp : implementation file
//

#include "stdafx.h"
#include "MicroClientEx.h"
#include "ContainerDialog.h"
#include "MicroClientLang.h"
#include <mshtml.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// ContainerDialog dialog


ContainerDialog::ContainerDialog(CWnd* pParent /*=NULL*/)
: CDialog(ContainerDialog::IDD, pParent)
,m_change_flag(FALSE)
,m_fullscreen(FALSE)
{
	//{{AFX_DATA_INIT(ContainerDialog)
	// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void ContainerDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(ContainerDialog)
	DDX_Control(pDX, IDC_HOTKEY1, m_HotKey);
	DDX_Control(pDX, IDC_EXPLORER, m_WebBrowser);
	//}}AFX_DATA_MAP
}


BEGIN_MESSAGE_MAP(ContainerDialog, CDialog)
	//{{AFX_MSG_MAP(ContainerDialog)
	ON_WM_CLOSE()
	ON_WM_CREATE()
	ON_WM_NCLBUTTONDBLCLK()
	ON_WM_NCLBUTTONDOWN()
	ON_WM_NCLBUTTONUP()
	ON_WM_NCMOUSEMOVE()
	ON_WM_SIZE()
	ON_MESSAGE(WM_CBLBUTTONCLICKED, OnCBLButtonClicked)
	ON_COMMAND(ID_MENUITEM_FULLSCR, OnMenuitemFullscr)
	ON_COMMAND(ID_MENUITEM_CLEAN, OnMenuitemClean)
	ON_COMMAND(ID_MENUITEM_BOSS, OnMenuitemBoss)
	ON_MESSAGE(WM_HOTKEY,OnHotKey)
	ON_WM_NCHITTEST()
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_LBUTTONDOWN()
	//}}AFX_MSG_MAP
	ON_MESSAGE(MSGCOUNTERMOVE,ContainerDialog::OnAcceptMove)
	ON_MESSAGE(MSGWEBREFRESH,ContainerDialog::OnAcceptRefush)
	ON_MESSAGE(MSGWEBFULLSRC,ContainerDialog::OnAcceptFullsrc)
	ON_MESSAGE(MSGRESTORE,ContainerDialog::OnAcceptRestore)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// ContainerDialog message handlers

BOOL ContainerDialog::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	// TODO: Add extra initialization here
	//ModifyStyle(WS_BORDER,WS_SYSMENU|WS_MINIMIZEBOX|WS_MAXIMIZEBOX); 
	//windowsflag = RUNWINDOW;
	
	//DWORD dwstyle = GetWindowLong(GetSafeHwnd(),GWL_STYLE);
	//SetWindowLong(GetSafeHwnd(),GWL_STYLE,dwstyle|WS_CLIPCHILDREN);
	//m_WebBrowser.BringWindowToTop();
	//m_WebBrowser.SetVisible(FALSE);
	

	ModifyStyleEx(0,WS_EX_APPWINDOW);
	ModifyStyle(WS_BORDER,NULL);
	
	this->SetWindowText(syjlangstr17);
	
	if (windowsflag == LOGINWINDOW)
	{
		//SetWindowLong(this->GetSafeHwnd(),GWL_EXSTYLE,GetWindowLong(this->GetSafeHwnd(),GWL_EXSTYLE)^0x80000);
		HINSTANCE hInst = LoadLibrary("User32.DLL"); 
		if(hInst) 
		{ 
			typedef BOOL (WINAPI *MYFUNC)(HWND,COLORREF,BYTE,DWORD); 
			MYFUNC fun = NULL;
			//取得SetLayeredWindowAttributes函数指针 
			fun=(MYFUNC)GetProcAddress(hInst, "SetLayeredWindowAttributes");
			COLORREF maskColor = RGB(233,233,233);
			if(fun)fun(this->GetSafeHwnd(),maskColor,255,1); 
			FreeLibrary(hInst); 
		} 
	}
	
	return TRUE;  // return TRUE unless you set the focus to a control
	              // EXCEPTION: OCX Property Pages should return FALSE
}

BOOL ContainerDialog::PreTranslateMessage(MSG* pMsg) 
{
	// TODO: Add your specialized code here and/or call the base class
	BOOL bRet = FALSE;
	// give HTML page a chance to translate this message

	CWnd *pWnd;
	pWnd = CWnd::FromHandle(pMsg->hwnd);

	if(pMsg->hwnd == m_hWnd || IsChild(pWnd))
	{
		bRet = (BOOL)::SendMessage(m_WebBrowser.m_hWnd,WM_FORWARDMSG, 0, (LPARAM)pMsg);
	}


	if ((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_RETURN))
	{
		return false;
	}
	if(pMsg->message == WM_KEYDOWN && GetAsyncKeyState(VK_F5)<0)
	{
		m_WebBrowser.Refresh();
		return TRUE;
	}
	if(pMsg->message == WM_KEYDOWN && GetAsyncKeyState(VK_F11)<0)
	{
		//MessageBox("runf11");
		OnMenuitemFullscr();
		return TRUE;
	}
	if((pMsg->message == WM_KEYDOWN)&&(pMsg->wParam == VK_ESCAPE))
	{
		return false;
	}
	if (pMsg->message == WM_HOTKEY && pMsg->wParam == 100)
	{
		return false;
	}
	if (pMsg->message == WM_SYSKEYDOWN && pMsg->wParam == VK_F4)
	{
		::SendMessage(GetParent()->GetSafeHwnd(),WM_CLOSE,NULL,NULL);
		return TRUE;
	}

	if ((pMsg->message < WM_KEYFIRST || pMsg->message > WM_KEYLAST) && (pMsg->message < WM_MOUSEFIRST || pMsg->message > WM_MOUSELAST))
	{
		
		return FALSE;
	}
	

	return bRet;

	//return CDialog::PreTranslateMessage(pMsg);
}


void ContainerDialog::CalcWindowRect(LPRECT lpClientRect, UINT nAdjustType) 
{
	// TODO: Add your specialized code here and/or call the base class
	
	CDialog::CalcWindowRect(lpClientRect, nAdjustType);
}

void ContainerDialog::OnOK() 
{
	// TODO: Add extra validation here
}

BEGIN_EVENTSINK_MAP(ContainerDialog, CDialog)
    //{{AFX_EVENTSINK_MAP(ContainerDialog)
	ON_EVENT(ContainerDialog, IDC_EXPLORER, 259 /* DocumentComplete */, OnDocumentCompleteExplorer, VTS_DISPATCH VTS_PVARIANT)
	ON_EVENT(ContainerDialog, IDC_EXPLORER, 250 /* BeforeNavigate2 */, OnBeforeNavigate2Explorer, VTS_DISPATCH VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PVARIANT VTS_PBOOL)
	ON_EVENT(ContainerDialog, IDC_EXPLORER, 252 /* NavigateComplete2 */, OnNavigateComplete2Explorer, VTS_DISPATCH VTS_PVARIANT)
	//}}AFX_EVENTSINK_MAP
END_EVENTSINK_MAP()

void ContainerDialog::ReSize()
{
	CRect rect;   
	//CMicroClientExApp *app = (CMicroClientExApp*)AfxGetApp();
	GetClientRect(&rect); //取客户区大小  
	CPoint TLPoint,BRPoint;

	TLPoint.x = rect.left+3;
	TLPoint.y = rect.top+30;
	BRPoint.x = rect.right-6;
	BRPoint.y = rect.bottom-33;


	m_WebBrowser.SetLeft(TLPoint.x);
	m_WebBrowser.SetTop(TLPoint.y);
	m_WebBrowser.SetWidth(BRPoint.x);
	m_WebBrowser.SetHeight(BRPoint.y);
	Invalidate();
 
}

LRESULT ContainerDialog::DefWindowProc(UINT message, WPARAM wParam, LPARAM lParam) 
{
	// TODO: Add your specialized code here and/or call the base class
		LRESULT lrst=CDialog::DefWindowProc(message,wParam,lParam);						//对话框类窗口过程函数	

	if(!IsWindow(m_hWnd))													
		return lrst; 

	return lrst;
	//return CDialog::DefWindowProc(message, wParam, lParam);
}


void ContainerDialog::OnClose() 
{
	// TODO: Add your message handler code here and/or call default
	//if(::MessageBox(m_hWnd,"是否退出游戏?","退出游戏",MB_YESNO) == IDYES)
	if(::MessageBox(m_hWnd,syjlangstr11,syjlangstr12,MB_YESNO) == IDYES)
 	{
		if  (IsZoomed())
		{

			HWND hwTaskbar=::FindWindow("Shell_TrayWnd","");		// 获得任务栏窗口句柄			
			if(hwTaskbar!=NULL)
			{
			::SetWindowPos(hwTaskbar,HWND_TOPMOST,0,0,0,0,SWP_SHOWWINDOW);
			}
		}		
		//m_WebBrowser.DestroyWindow();
		//DestroyWindow();
		::SendMessage(GetParent()->GetSafeHwnd(),WM_CLOSE,NULL,NULL);
		CDialog::OnClose();
	}
	
}

LRESULT ContainerDialog::WindowProc(UINT message, WPARAM wParam, LPARAM lParam) 
{
	// TODO: Add your specialized code here and/or call the base class
	if (message == 0x00AE || message == 0x00AF)//预防点击标题栏 系统按钮会出现
	{
		return WM_NCPAINT;
	}

	return CDialog::WindowProc(message, wParam, lParam);
}

void ContainerDialog::OnCBLButtonClicked(WPARAM wParam, LPARAM lParam)
{//wparam传的是按钮的ID 此函数用于标题栏自定义按钮的响应

	switch(wParam)
	{
	case 1:
		m_WebBrowser.Refresh();
		break;
	case 2:
		CMenu menu,*pMenu;
		menu.LoadMenu(IDR_MENU1);
		CRect pt;
		int i = 8;
		pt = m_cbExtra2.CalculateRect(i,FALSE);
 		pMenu = menu.GetSubMenu(0);
		pMenu->ModifyMenu(0,MF_BYPOSITION,ID_MENUITEM_FULLSCR,syjlangstr29);
		pMenu->ModifyMenu(1,MF_BYPOSITION,ID_MENUITEM_BOSS,syjlangstr28);
		pMenu->ModifyMenu(2,MF_BYPOSITION,ID_MENUITEM_CLEAN,syjlangstr27);
// 		sMenu = menu.GetSubMenu(1);
// 		sMenu->ModifyMenu(0,MF_BYPOSITION,ID_MENUITEM_BOSS,syjlangstr28);
// 		tMenu = menu.GetSubMenu(2);
// 		tMenu->ModifyMenu(0,MF_BYPOSITION,ID_MENUITEM_CLEAN,syjlangstr27);
		pMenu->TrackPopupMenu(TPM_LEFTALIGN,pt.right,pt.bottom,this);
		break;
	}
}

// 菜单响应函数
void ContainerDialog::OnMenuitemFullscr() 
{
	if  (IsZoomed())
	{
		UpdateData(false);
		SendMessage(WM_SYSCOMMAND,SC_RESTORE);
		HWND hwTaskbar=::FindWindow("Shell_TrayWnd","");		// 获得任务栏窗口句柄			
		if(hwTaskbar!=NULL)
		{
			::SetWindowPos(hwTaskbar,HWND_TOPMOST,0,0,0,0,SWP_SHOWWINDOW);
		}
	}	
	else 
	{	
		SendMessage(WM_SYSCOMMAND,SC_MAXIMIZE);
	}

	SetForegroundWindow();
}

void ContainerDialog::OnMenuitemClean() 
{
	// TODO: Add your command handler code here
	if(DelInetTempFiles())
		{
			//MessageBox("清理成功！","清理报告");
			MessageBox(syjlangstr13,syjlangstr14);
		}
}

void ContainerDialog::OnMenuitemBoss() 
{
	// TODO: Add your command handler code here
	OnHotKey(100,NULL);
}

void ContainerDialog::OnHotKey(WPARAM wParam,LPARAM lParam)
{

	if(wParam == 100)
	{
	
		if(::IsWindowVisible(this->GetSafeHwnd()))
		{
			ShowWindow(SW_HIDE);
			
			HWND hwTaskbar=::FindWindow("Shell_TrayWnd","");		// 获得任务栏窗口句柄			
			if(hwTaskbar!=NULL)
			{
				::SetWindowPos(hwTaskbar,HWND_TOPMOST,0,0,0,0,SWP_SHOWWINDOW);
			}
		}
		else
		{
			ShowWindow(SW_SHOW);

			SetForegroundWindow();
		}
	}
}

LRESULT ContainerDialog::OnAcceptMove(WPARAM w, LPARAM l)
{
	int x = w;
	int y = l;
	MoveWindow(x+240,y+80,800,430);
	return 0;
}

LRESULT ContainerDialog::OnAcceptRefush(WPARAM w, LPARAM l)
{
	m_WebBrowser.Refresh();
	return 0;
}

LRESULT ContainerDialog::OnAcceptFullsrc(WPARAM w, LPARAM l)
{
	OnMenuitemFullscr();
	return 0;
}

LRESULT ContainerDialog::OnAcceptRestore(WPARAM w, LPARAM l)
{
	OnMenuitemFullscr();
	return 0;
}

void ContainerDialog::PostNcDestroy() 
{
	// TODO: Add your specialized code here and/or call the base class
	::SendMessage(AfxGetMainWnd()->m_hWnd,WM_CLOSE,0,0);
	CDialog::PostNcDestroy();
}

void ContainerDialog::OnPaint() 
{
	CPaintDC dc(this); // device context for painting

	// Do not call CDialog::OnPaint() for painting messages
}


int ContainerDialog::OnCreate(LPCREATESTRUCT lpCreateStruct) 
{
	if (CDialog::OnCreate(lpCreateStruct) == -1)
		return -1;
	
	// TODO: Add your specialized creation code here
	ModifyStyle(NULL,WS_MINIMIZEBOX);
	return 0;
}

UINT ContainerDialog::OnNcHitTest(CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	CRect rcWindow;
	GetWindowRect(&rcWindow);
	
	if (point.x>=rcWindow.left&&point.x<=rcWindow.left+4)
	{
		if(point.y>=rcWindow.top&&point.y<=rcWindow.top+4)
		{
			return HTTOPLEFT;
		}
		else if(point.y<=rcWindow.bottom&&point.y>=rcWindow.bottom-4)
		{
			return HTBOTTOMLEFT;
		}
		else
			return HTLEFT;
	}
	else if(point.x>=rcWindow.right-4&&point.x<=rcWindow.right)
	{
		
		if(point.y>=rcWindow.top&&point.y<=rcWindow.top+4)
		{
			return HTTOPRIGHT;
		}
		else if(point.y<=rcWindow.bottom&&point.y>=rcWindow.bottom-4)
		{
			return HTBOTTOMRIGHT;
		}
		else
			return HTRIGHT;		
	}
	else if (point.y>=rcWindow.top&&point.y<=rcWindow.top+4)
	{
		return HTTOP;
	}
	else	if(point.y<=rcWindow.bottom&&point.y>=rcWindow.bottom-4)
	{
		return HTBOTTOM;
	}
	else return HTCAPTION;

	return CDialog::OnNcHitTest(point);
}



void ContainerDialog::OnLButtonDown(UINT nFlags, CPoint point) 
{
	// TODO: Add your message handler code here and/or call default
	PostMessage(WM_NCLBUTTONDOWN,(WPARAM)HTCAPTION,MAKELPARAM(point.x,point.y));
	CDialog::OnLButtonDown(nFlags, point);
}


BOOL ContainerDialog::PreCreateWindow(CREATESTRUCT& cs) 
{
	// TODO: Add your specialized code here and/or call the base class
	//ModifyStyle(NULL,WS_SYSMENU|WS_MINIMIZEBOX);
	cs.style |= WS_CLIPCHILDREN;
	//cs.dwExStyle |= WS_EX_TOOLWINDOW;
	return CDialog::PreCreateWindow(cs);
}

void ContainerDialog::OnDocumentCompleteExplorer(LPDISPATCH pDisp, VARIANT FAR* URL) 
{
	// TODO: Add your control notification handler code here

	
}

void ContainerDialog::OnBeforeNavigate2Explorer(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel) 
{
	// TODO: Add your control notification handler code here
// 	ASSERT(V_VT(URL) == VT_BSTR);
// 	ASSERT(V_VT(TargetFrameName) == VT_BSTR);
// 	ASSERT(V_VT(PostData) == (VT_VARIANT | VT_BYREF));
// 	ASSERT(V_VT(Headers) == VT_BSTR);
// 	ASSERT(Cancel != NULL);
//  
// 	//USES_CONVERSION;
//  
// 	VARIANT* vtPostedData = V_VARIANTREF(PostData);
// 	CByteArray array;
// 	if (V_VT(vtPostedData) & VT_ARRAY)
// 	{
//  // must be a vector of bytes
// 		ASSERT(vtPostedData->parray->cDims == 1 && vtPostedData->parray->cbElements == 1);
//  
// 		vtPostedData->vt |= VT_UI1;
// 		 COleSafeArray safe(vtPostedData);
//  
// 		DWORD dwSize = safe.GetOneDimSize();
// 		LPVOID pVoid;
// 		safe.AccessData(&pVoid);
//  
// 		array.SetSize(dwSize);
// 		 LPBYTE lpByte = array.GetData();
//  
// 		memcpy(lpByte, pVoid, dwSize);
// 		 safe.UnaccessData();
// 	}
//  // make real parameters out of the notification
//  
// 	CString strTargetFrameName(V_BSTR(TargetFrameName));
// 	 CString strURL = V_BSTR(URL);
// 	CString strHeaders = V_BSTR(Headers);
// 	DWORD nFlags = V_I4(Flags);

	//m_WebBrowser.Navigate(strURL,NULL,NULL,NULL,NULL);

}

void ContainerDialog::OnSysCommand(UINT nID, LPARAM lParam)
{
	
	if (windowsflag == LOGINWINDOW)
	{
		if (nID == SC_MINIMIZE)
		{
			if (GetParent()->IsWindowVisible())
			{
				GetParent()->ShowWindow(SW_HIDE);
			}
		}
		else if (nID == SC_RESTORE)
		{
			if (!GetParent()->IsWindowVisible())
			{
				GetParent()->ShowWindow(SW_SHOW);
			}
			
		}
	}
	CDialog::OnSysCommand(nID, lParam);
}

void ContainerDialog::OnNavigateComplete2Explorer(LPDISPATCH pDisp, VARIANT FAR* URL) 
{
	// TODO: Add your control notification handler code here
	::ShowWindow(GetSafeHwnd(),SW_SHOW);
	UpdateWindow();
	
}
