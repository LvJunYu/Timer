//{{AFX_INCLUDES()
#include "webbrowser2.h"
//}}AFX_INCLUDES
#if !defined(AFX_ONTAINERDIALOG_H__6E529423_FD9F_4FFC_94B4_218CBA23AE45__INCLUDED_)
#define AFX_ONTAINERDIALOG_H__6E529423_FD9F_4FFC_94B4_218CBA23AE45__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ontainerDialog.h : header file
//
#include "resource.h"
#include "DrawAppearance.h"
#include "CaptionButton.h"
#include "common_function.h"
#include <WinInet.h>//清理缓存头文件
/////////////////////////////////////////////////////////////////////////////
// ContainerDialog dialog

class ContainerDialog : public CDialog
{
// Construction
public:
	ContainerDialog(CWnd* pParent = NULL);   // standard constructor
	BOOL m_change_flag;
	BOOL m_fullscreen;
	void ModifyInitRunDialog();//初始化运行窗口
	void ReSize();

	// Dialog Data
	//{{AFX_DATA(ContainerDialog)
	enum { IDD = IDD_DIALOG_WEBBROWSER };
	CWebBrowser2	m_WebBrowser;
	CHotKeyCtrl	m_HotKey;
	CDrawAppearance m_drawApe;
	//}}AFX_DATA
	
	
	// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(ContainerDialog)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	virtual void CalcWindowRect(LPRECT lpClientRect, UINT nAdjustType = adjustBorder);
	virtual LRESULT DefWindowProc(UINT message, WPARAM wParam, LPARAM lParam);
	virtual LRESULT WindowProc(UINT message, WPARAM wParam, LPARAM lParam);
	virtual void PostNcDestroy();
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL
	
	// Implementation
protected:
	
	CCaptionButton m_cbExtra1;
	CCaptionButton m_cbExtra2;
	// Generated message map functions
	//{{AFX_MSG(ContainerDialog)
	virtual BOOL OnInitDialog();
	afx_msg void OnNcLButtonDblClk(UINT nHitTest, CPoint point);
	afx_msg void OnNcLButtonDown(UINT nHitTest, CPoint point);
	afx_msg void OnNcLButtonUp(UINT nHitTest, CPoint point);
	afx_msg void OnNcMouseMove(UINT nHitTest, CPoint point);
	afx_msg void OnSize(UINT nType, int cx, int cy);
	afx_msg void OnCBLButtonClicked(WPARAM, LPARAM);
	afx_msg void OnMenuitemFullscr();
	afx_msg void OnMenuitemClean();
	afx_msg void OnMenuitemBoss();
	afx_msg void OnHotKey(WPARAM wParam,LPARAM lParam);
	afx_msg void OnPaint();
	virtual void OnOK();
	afx_msg void OnClose();
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg UINT OnNcHitTest(CPoint point);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnDocumentCompleteExplorer(LPDISPATCH pDisp, VARIANT FAR* URL);
	afx_msg void OnBeforeNavigate2Explorer(LPDISPATCH pDisp, VARIANT FAR* URL, VARIANT FAR* Flags, VARIANT FAR* TargetFrameName, VARIANT FAR* PostData, VARIANT FAR* Headers, BOOL FAR* Cancel);
	afx_msg void OnNavigateComplete2Explorer(LPDISPATCH pDisp, VARIANT FAR* URL);
	DECLARE_EVENTSINK_MAP()
	//}}AFX_MSG
	afx_msg LRESULT OnAcceptMove(WPARAM w, LPARAM l);
	afx_msg LRESULT OnAcceptRefush(WPARAM w, LPARAM l);
	afx_msg LRESULT OnAcceptFullsrc(WPARAM w, LPARAM l);
	afx_msg LRESULT OnAcceptRestore(WPARAM w, LPARAM l);
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_ONTAINERDIALOG_H__6E529423_FD9F_4FFC_94B4_218CBA23AE45__INCLUDED_)
