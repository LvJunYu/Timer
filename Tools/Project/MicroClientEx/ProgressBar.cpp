// ProgressBar.cpp : implementation file
//

#include "stdafx.h"
#include "MicroClientEx.h"
#include "ProgressBar.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// ProgressBar dialog


ProgressBar::ProgressBar(CWnd* pParent /*=NULL*/)
	: CDialog(/*ProgressBar::IDD, pParent*/)
{
	//{{AFX_DATA_INIT(ProgressBar)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void ProgressBar::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(ProgressBar)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

void ProgressBar::InitPogressBar(int pos_x,int pos_y,int totaleValue,unsigned short* resId_Bg,unsigned short* resId_Fill,unsigned short* resId_Empty,unsigned short* resId_Knob)
{
	this->pos_x = pos_x;
	this->pos_y = pos_y;
	this->processBasicData.currentPos = 0;
	this->processBasicData.totaleLength = totaleValue;
	this->progressBg = Gdiplus::Image::FromFile(resId_Bg);
	this->progressFill = Gdiplus::Image::FromFile(resId_Fill);
	this->progressEmpty = Gdiplus::Image::FromFile(resId_Empty);
	this->progressKnob = Gdiplus::Image::FromFile(resId_Knob);
	this->processImagRect.width_bg = this->progressBg->GetWidth();
	this->processImagRect.height_bg = this->progressBg->GetHeight();
	this->processImagRect.width_content = this->progressFill->GetWidth();
	this->processImagRect.height_content = this->progressFill->GetHeight();
	this->processImagRect.width_knob = this->progressKnob->GetWidth();
	this->processImagRect.height_knob = this->progressKnob->GetHeight();
}

void ProgressBar::InitOffset(int outer_offset_X,int outer_offset_Y,int content_offset_X,int content_offset_Y,int knob_offset_X,int knob_offset_Y)
{
	processOffset.outer_offset_X = outer_offset_X;
	processOffset.outer_offset_Y = outer_offset_Y;
	processOffset.content_offset_X = content_offset_X;
	processOffset.content_offset_Y = content_offset_Y;
	processOffset.knob_offset_X = knob_offset_X;
	processOffset.knob_offset_Y = knob_offset_Y;
}

bool ProgressBar::UpdateData(int newValue)
{
	if(this->processBasicData.currentPos <= this->processBasicData.totaleLength)
	{
		if(this->processBasicData.currentPos != newValue)
		{
			this->processBasicData.currentPos = newValue;
			return TRUE;
		}
		else return FALSE;
	}
	else return FALSE;
}

void ProgressBar::ProgressLayout(Graphics graph)
{
	graph.DrawImage(this->progressBg,this->pos_x, this->pos_y, 0, 0,this->processImagRect.width_bg,this->processImagRect.height_bg,UnitPixel);
	//emptyImag->SetBtnPos(pos_x + offset.content_offset_X,pos_y + offset.content_offset_Y);
	
	//contentImag->SetBtnPos(pos_x + offset.content_offset_X,pos_y + offset.content_offset_Y);
	graph.DrawImage(this->progressFill,this->pos_x + this->processOffset.content_offset_X,this->pos_y + this->processOffset.content_offset_Y, 0, 0,this->processImagRect.width_content,this->processImagRect.height_content,UnitPixel);

	//knobImag->SetBtnPos(pos_x + offset.knob_offset_X + basicData.currentPos*emptyImag->width/basicData.totaleLength,pos_y + offset.knob_offset_Y);
	graph.DrawImage(this->progressKnob,this->pos_x + this->processOffset.knob_offset_X + this->processBasicData.currentPos*this->progressFill->GetWidth()/this->processBasicData.totaleLength,this->pos_y + this->processOffset.knob_offset_Y, 0, 0,this->processImagRect.width_knob,this->processImagRect.height_knob,UnitPixel);
}
BEGIN_MESSAGE_MAP(ProgressBar, CDialog)
	//{{AFX_MSG_MAP(ProgressBar)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// ProgressBar message handlers
