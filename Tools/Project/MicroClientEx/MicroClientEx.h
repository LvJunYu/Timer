// MicroClientEx.h : main header file for the MICROCLIENTEX application
//

#if !defined(AFX_MICROCLIENTEX_H__EAA441C5_1782_4B50_92E4_B6F2CCA113B4__INCLUDED_)
#define AFX_MICROCLIENTEX_H__EAA441C5_1782_4B50_92E4_B6F2CCA113B4__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"		// main symbols

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExApp:
// See MicroClientEx.cpp for the implementation of this class
//

#define MSGCOUNTERMOVE (WM_USER + 108)
#define MSGFLOATCLOSE (WM_USER + 116)
#define MSGWEBREFRESH  (WM_USER + 124)
#define MSGWEBFULLSRC  (WM_USER + 132)
#define MSGRESTORE  (WM_USER + 140)
#define LOGINWINDOW 0

static int windowsflag = 0;

class CImpIDispatch;
class CMicroClientExApp : public CWinApp
{
public:
	CMicroClientExApp();

	HWND GetWindowHandleByPID(DWORD dwProcessID);

	CImpIDispatch*		m_pCustDisp;
// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMicroClientExApp)
	public:
	virtual BOOL InitInstance();
	int CMicroClientExApp::ExitInstance() ;
	//}}AFX_VIRTUAL

// Implementation

	//{{AFX_MSG(CMicroClientExApp)
		// NOTE - the ClassWizard will add and remove member functions here.
		//    DO NOT EDIT what you see in these blocks of generated code !
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

extern CMicroClientExApp theApp;
/////////////////////////////////////////////////////////////////////////////

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MICROCLIENTEX_H__EAA441C5_1782_4B50_92E4_B6F2CCA113B4__INCLUDED_)
