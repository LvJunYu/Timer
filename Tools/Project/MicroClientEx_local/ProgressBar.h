#if !defined(AFX_PROGRESSBAR_H__FB5A43FD_9CF6_4E20_AC50_08EE167CB0AF__INCLUDED_)
#define AFX_PROGRESSBAR_H__FB5A43FD_9CF6_4E20_AC50_08EE167CB0AF__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
// ProgressBar.h : header file
//

struct ProcessBasicData
{
public:
	//unsigned short *	bgImag_ID;
	//unsigned short *	knobImag_ID;
	//unsigned short *	emptyImag_ID;
	//unsigned short *	contentImag_ID;
	int	currentPos;
	int	totaleLength;
};

struct ProcessOffset
{
public:
	int	outer_offset_X;
	int	outer_offset_Y;
	int	content_offset_X;
	int	content_offset_Y;
	int	knob_offset_X;
	int	knob_offset_Y;
};

struct ProcessImagRect
{
public:
	int width_bg;
	int height_bg;
	int width_content;
	int height_content;
	int width_knob;
	int height_knob;
};

//进度条资源
#define PROGRESSBAR_BG		L"res\\ui_kuang.png"
#define PROGRESSBAR_EMPTY	L"res\\ui_progress_01.png"
#define PROGRESSBAR_FILL	L"res\\ui_progress_01.png"
#define PROGRESSBAR_KNOB	L"res\\ui_knob.png"
/////////////////////////////////////////////////////////////////////////////
// ProgressBar dialog

class ProgressBar : public CDialog
{
// Construction
public:
	ProgressBar(CWnd* pParent = NULL);   // standard constructor

	int pos_x;
	int pos_y;
	Image *progressBg;
	Image *progressFill;
	Image *progressEmpty;
	Image *progressKnob;
	ProcessBasicData processBasicData;
	ProcessOffset processOffset;
	ProcessImagRect processImagRect;

	bool ProgressBar::UpdateData(int newValue);
	void ProgressBar::ProgressLayout(Graphics graph);
	void ProgressBar::InitOffset(int outer_offset_X,int outer_offset_Y,int content_offset_X,int content_offset_Y,int knob_offset_X,int knob_offset_Y);
	void ProgressBar::InitPogressBar(int pos_x,int pos_y,int totaleValue,unsigned short* resId_Bg,unsigned short* resId_Fill,unsigned short* resId_Empty,unsigned short* resId_Knob);
// Dialog Data
	//{{AFX_DATA(ProgressBar)
	//enum { IDD = _UNKNOWN_RESOURCE_ID_ };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA


// Overrides
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(ProgressBar)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:

	// Generated message map functions
	//{{AFX_MSG(ProgressBar)
		// NOTE: the ClassWizard will add member functions here
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_PROGRESSBAR_H__FB5A43FD_9CF6_4E20_AC50_08EE167CB0AF__INCLUDED_)
