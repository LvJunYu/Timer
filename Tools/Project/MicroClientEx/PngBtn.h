#if !defined(AFX_PNGBTN_H__6761C04A_7BCC_41DC_9A3A_3C6EECE2D3D1__INCLUDED_)
#define AFX_PNGBTN_H__6761C04A_7BCC_41DC_9A3A_3C6EECE2D3D1__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// PngBtn.h : header file
//

/////////////////////////////////////////////////////////////////////////////
// PngBtn dialog

#define CLOSE_NORMAL_STATE	L"res\\ui_exit_01.png"
#define CLOSE_ACTIVE_STATE	L"res\\ui_exit_03.png"
#define CLOSE_OVER_STATE	L"res\\ui_exit_02.png"
#define MINI_NORMAL_STATE	L"res\\ui_mini_01.png"
#define MINI_ACTIVE_STATE	L"res\\ui_mini_03.png"
#define MINI_OVER_STATE		L"res\\ui_mini_02.png"



#define NORMAL_STATE	0
#define OVER_STATE		1
#define ACTIVE_STATE	2

class PngBtn : public CDialog
{
// Construction
public:
	PngBtn(CWnd* pParent = NULL);   // standard constructor

	int pos_x;
	int pos_y;
	int width;
	int height;
	Image *normal_Image;
	Image *over_Image;
	Image *active_Image;
	Image *current_Image;
	
	Image *PngBtn::GetCurrentState();
	void PngBtn::SetBtnCurrentState(int state);
	bool PngBtn::CalculateState(int point_x,int point_y);
	bool PngBtn::CalculatePressDown(int point_x,int point_y);
	void PngBtn::InitButton(int pos_x,int pos_y,unsigned short* resId_Normal,unsigned short* resId_Over,unsigned short* resId_Active);
// Dialog Data
	//{{AFX_DATA(PngBtn)
	//enum { IDD = _UNKNOWN_RESOURCE_ID_ };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(PngBtn)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(PngBtn)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PNGBTN_H__6761C04A_7BCC_41DC_9A3A_3C6EECE2D3D1__INCLUDED_)
