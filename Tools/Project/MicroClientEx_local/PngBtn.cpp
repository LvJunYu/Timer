// PngBtn.cpp : implementation file
//

#include "stdafx.h"
#include "MicroClientEx.h"
#include "PngBtn.h"
#include "LogUtil.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// PngBtn dialog


PngBtn::PngBtn(CWnd* pParent /*=NULL*/)
	: CDialog(/*PngBtn::IDD, pParent*/)
{
	//{{AFX_DATA_INIT(PngBtn)
		// NOTE: the ClassWizard will add member initialization here
	//}}AFX_DATA_INIT
}


void PngBtn::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(PngBtn)
		// NOTE: the ClassWizard will add DDX and DDV calls here
	//}}AFX_DATA_MAP
}

void PngBtn::InitButton(int pos_x,int pos_y,unsigned short* resId_Normal,unsigned short* resId_Over,unsigned short* resId_Active)
{
	//PngBtn *btn = new PngBtn();
	this->pos_x = pos_x;
	this->pos_y = pos_y;
	this->normal_Image = Gdiplus::Image::FromFile(resId_Normal);
	if(!this->normal_Image)
	{
		WriteLog("load normal btn failed.");
	}
	this->over_Image = Gdiplus::Image::FromFile(resId_Over);
	if(!this->over_Image)
	{
		WriteLog("load over btn failed.");
	}
	this->active_Image = Gdiplus::Image::FromFile(resId_Active);
	if(!this->active_Image)
	{
		WriteLog("load over btn failed.");
	}
	
	this->current_Image = this->normal_Image;
	this->width = this->current_Image->GetWidth();
	this->height = this->current_Image->GetHeight();
}

void PngBtn::SetBtnCurrentState(int state)
{
	switch(state)
	{
		case NORMAL_STATE:
			this->current_Image = this->normal_Image;
			break;
		case OVER_STATE:
			this->current_Image = this->over_Image;
			break;
		case ACTIVE_STATE:
			this->current_Image = this->active_Image;
			break;
	}
	this->width = this->current_Image->GetWidth();
	this->height = this->current_Image->GetHeight();
}

Image* PngBtn::GetCurrentState()
{
	return this->current_Image;
}

bool PngBtn::CalculatePressDown(int point_x,int point_y)
{
	if(point_x >= this->pos_x - 3 && point_x <= this->width + this->pos_x + 3 &&  point_y >= this->pos_y - 3&& point_y <= this->height + this->pos_y + 3)
	{
		return TRUE;
	}
	else return FALSE;
}

bool PngBtn::CalculateState(int point_x,int point_y)
{
	if(point_x >= this->pos_x - 3 && point_x <= this->width + this->pos_x + 3 &&  point_y >= this->pos_y - 3&& point_y <= this->height + this->pos_y + 3)
	{
// 		CString str;
// 		str.Format("point_x: %d,point_y: %d,this->width: %d,this->hight: %d,this->pos_x: %d,this->pos_y: %d",point_x,point_y,this->width,this->height,this->pos_x,this->pos_y);
// 		AfxMessageBox(str);
		if(this->current_Image != this->over_Image)
		{
			this->current_Image = this->over_Image;
			return TRUE;
		}
	}
	else
	{
		if(this->current_Image != this->normal_Image)
		{
			this->current_Image = this->normal_Image;
			return TRUE;
		}
	}
	return FALSE;
}

BEGIN_MESSAGE_MAP(PngBtn, CDialog)
	//{{AFX_MSG_MAP(PngBtn)
		// NOTE: the ClassWizard will add message map macros here
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// PngBtn message handlers
