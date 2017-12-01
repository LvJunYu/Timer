// CaptionButton.cpp : implementation file
//

#include "StdAfx.h"
#include <commctrl.h>
#include "CaptionButton.h"

/////////////////////////////////////////////////////////////////////////////
// CCaptionButton
CCaptionButton::CCaptionButton()
{// begin constuctor
	// initilize button variables
	m_nNumButtons = 0;
	m_crTransparentColor = NULL;	// set default no transparent color
	m_nNumDefaultButtons = NULL;	// set to null will assume no captions
	m_fExStyles = 0;
	m_fStyles = 0;
	m_nDownNode = -1;
	m_nLastPointNode = -1;
	m_hSelectionBitmap = NULL;
	m_bCapturing = false;
	m_hWnd = NULL;
	m_bMyRedraw = false;
	m_wpOldWndProc = NULL;
	m_hToolTipWnd = NULL;
}// end constuctor

CCaptionButton::~CCaptionButton()
{// begin destructor
	// cleanup
	OnDestroy();
	// delete the linked list
	DeleteList();
}// end destructor

/***********  declareations of CCaptionButton ***********/
void CCaptionButton::Init(HWND hOwner)
{ // begin Init

	m_hWnd = hOwner;
	// subclass owner's m_hWnd message function
	PtrInit(m_PtrData,this);
	m_wpOldWndProc = (WNDPROC)::SetWindowLong(m_hWnd,GWL_WNDPROC,(LONG)(void *)m_PtrData);
	if(!m_hToolTipWnd)
	{// begin init tooltips
		// Ensure that the common control DLL is loaded  
//		InitCommonControls();
		// create a m_hToolTipWnd control. 
		m_hToolTipWnd = CreateWindow( TOOLTIPS_CLASS, NULL,
								WS_POPUP | TTS_NOPREFIX | TTS_ALWAYSTIP,
								CW_USEDEFAULT, CW_USEDEFAULT,
								CW_USEDEFAULT, CW_USEDEFAULT,
								m_hWnd, NULL, NULL,	NULL);

		SetWindowPos(m_hToolTipWnd, HWND_TOPMOST,0, 0, 0, 0,
					 SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
	}// end init tooltips
	
	// get the normal styles
	m_fStyles = GetWindowLong(m_hWnd,GWL_STYLE);
	// get the extra styles
	m_fExStyles = GetWindowLong(m_hWnd,GWL_EXSTYLE);
	// get upper left corner
//	RECT rWindowRect = {NULL};
//	GetWindowRect(m_hWnd,&rWindowRect);
} // end Init
/*************************************************/
void CCaptionButton::Draw(int nIndex, bool bHilited) const
{ // begin Draw
	// declarations

	HDC hdcWin = ::GetDC(m_hWnd);

	HBITMAP hBitmap = NULL;
	if(bHilited)
	{// begin hilited
		hBitmap = m_vButtons[nIndex].hHiliteBitmap;
	}// end hilited
	else
	{// begin not hitiled
		hBitmap = m_vButtons[nIndex].hNormalBitmap;
	}// end not hilited

	// get the bitmap's dimensions
	BITMAP bmDimensions = {NULL};
	::GetObject(m_vButtons[nIndex].hNormalBitmap, sizeof( bmDimensions ), &bmDimensions );

	// center the bitmap in the button area
	SIZE szSpacing = {bmDimensions.bmWidth,bmDimensions.bmHeight};
	szSpacing = CenterBitmap(szSpacing);

	// the the captions area
	RECT rWindowRect = {NULL};
	GetWindowRect(m_hWnd,&rWindowRect);
	// get button rectangle
	RECT rButtonRect = CalculateRect(nIndex+1 + m_nNumDefaultButtons,false);
	// draw it
	DrawTransparentBitmap(hdcWin
		,hBitmap
		,szSpacing.cx+rButtonRect.left-rWindowRect.left
		,szSpacing.cy+rButtonRect.top-rWindowRect.top
		,m_crTransparentColor);

	// cleanup
	::ReleaseDC(m_hWnd,hdcWin);

} // end Draw
/*************************************************/
bool CCaptionButton::AddButton(const UINT ID,HBITMAP hilite,HBITMAP normal,char *sTooltip,int nPos)
// the passed in HBITMAP's will be need to be deleted by the user if the function returns false
// not passing in nInsertPosition inserts the button at the end of the list (left most)
{ // begin AddButton

	if(GetNode(ID) > -1)
	{// begin already a node with that ID
		return false;
	}// end already a node with that ID

	if(!hilite)
		hilite = normal;

	ButtonData cbCurrent = CreateButton(ID,hilite,normal,sTooltip);
	Insert(&cbCurrent,nPos);
	if(nPos > -1)
		ReInitButtons();
	RedrawButtons();
	return true;	
} // end AddButton
int CCaptionButton::GetNode(const UINT &find) const
{ // begin GetNode
	for(int i = 0;i < m_vButtons.size();i++)
	{ // begin
		if(m_vButtons[i].nID == find)
			return i+1;
	} // end
	return -1;
} // end GetNode
/*************************************************/
void CCaptionButton::DeleteList(void)
{ // begin DeleteList
	int nSize = m_vButtons.size()-1;

	for(int i = nSize;i > -1;i--)
	{ // begin delete
		DeleteButton(i);
	} // end delete
} // end DeleteList
/*************************************************/
void CCaptionButton::DeleteNode(const UINT &nNodeID)
{ // begin DeleteNode
	// get the index of the button
	int nIndex = GetNode(nNodeID)-1;
	if(nIndex < 0)
		return;
	// create temp region
	HRGN hTempRgn = NULL;
	RECT rRect = {NULL};
	GetRgnBox(m_vButtons[m_vButtons.size()-1].hRgn,&rRect);
	hTempRgn = CreateRectRgnIndirect(&rRect);
	// free the node
	DeleteButton(nIndex);
	m_vButtons.erase(&m_vButtons[nIndex]);
	// force a resize
	m_vButtons.resize(m_vButtons.size());
	// need to clear the button last area since it won't be overdrawn
	UpdateRegion(hTempRgn);
	// cleanup
	DeleteObject(hTempRgn);
} // end DeleteNode
/*************************************************/
SIZE CCaptionButton::CenterBitmap(const SIZE &size) const
{// begin CenterBitmap
	SIZE temp = {(::GetSystemMetrics(SM_CXSIZE)-size.cx)/2,(::GetSystemMetrics(SM_CYSIZE)-size.cy)/2};
	return temp;
}// end CenterBitmap
/*************************************************/
bool CCaptionButton::Insert(CCaptionButton::ButtonData *insert,int nInsertPosition)
{// begin Insert
	if(nInsertPosition == -1)
	{// begin insert it as the first button
		nInsertPosition = m_nNumButtons-1;
	}// end insert it as the first button
	m_vButtons.insert(m_vButtons.begin()+nInsertPosition,*insert);
	return true;
}// end Insert
/*************************************************/
LRESULT CCaptionButton::WindowProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{// begin WindowProc
	switch(message)
	{ // begin switch

	case WM_SIZING:	case WM_MOVING:	case WM_MOVE: case WM_SIZE:
		OnWindowChange();
		break;
	case WM_NCPAINT:
		if(m_bMyRedraw)
			break;
		return OnNcPaint(message,wParam,lParam);
	case WM_SETTEXT:
	case WM_NCACTIVATE: 
		return OnNcPaint(message,wParam,lParam);
	case WM_SYSCOLORCHANGE: case WM_SETTINGCHANGE:
		OnSystemChange();
		break;
	case WM_NCMOUSEMOVE:
		if(m_bCapturing)
			break;
	case WM_MOUSEMOVE:
		OnMouseMove(message,wParam,lParam);
		break;
	case WM_NCLBUTTONDBLCLK: case WM_NCLBUTTONDOWN:
		if(!OnNcLButtonDown(message,wParam,lParam))
			return NULL;
		break;
	case WM_LBUTTONUP:
		if(OnLButtonUp(message,wParam,lParam))
			break;
	case WM_NCLBUTTONUP:
		if(!OnNcLButtonUp(message,wParam,lParam))
			return NULL;
		break;
	case WM_DESTROY:
		OnDestroy();
		break;
	} // end switch

	if( !m_hWnd )
		return FALSE;
	// pass the message to the owner m_hWnd's WndProc function
	return CallWindowProc(m_wpOldWndProc,hwnd,message,wParam,lParam);
}// end WindowProc
/*************************************************/
void CCaptionButton::SetTransparentColor(const int &color)
// if color == NULL there is transparent color
// use COLORREF(r,g,b) for the color varialbe
{// begin SetTransparentColor
	m_crTransparentColor = color;
}// end SetTransparentColor
/*************************************************/
void CCaptionButton::SetNumOfDefaultButtons(const UINT &type)
// set the number of button spaces to leave to the right of the first button
{// begin SetNumOfDefaultButtons
	m_nNumDefaultButtons = type;
}// end SetNumOfDefaultButtons
/*************************************************/
SIZE CCaptionButton::GetButtonSize() const
// get the size of a button for the current window
{// begin GetButtonSize
	SIZE szButtonSize = {NULL};
	if(m_fExStyles & WS_EX_TOOLWINDOW)
	{// begin toolwindow
		szButtonSize.cx = 25;//GetSystemMetrics(SM_CXSMSIZE);
		szButtonSize.cy = 25;//GetSystemMetrics(SM_CYSMSIZE);
	}// end toolwindow
	else
	{// begin appwindow
		szButtonSize.cx = 25;//GetSystemMetrics(SM_CXSIZE);
		szButtonSize.cy = 25;//GetSystemMetrics(SM_CYSIZE);
	}// end appwindow
	return szButtonSize;
}// end GetButtonSize
/*************************************************/
SIZE CCaptionButton::GetFrameSize() const
// gets the width of the window's frame
{// begin GetFrameSize
	SIZE sFrameSize = {NULL};

	sFrameSize.cx = GetSystemMetrics(SM_CXFRAME);
	sFrameSize.cy = GetSystemMetrics(SM_CYFRAME);

// 	if(m_fStyles & WS_THICKFRAME)
// 	{// begin resizing frame
// 		sFrameSize.cx = GetSystemMetrics(SM_CXSIZEFRAME);
// 		sFrameSize.cy = GetSystemMetrics(SM_CYSIZEFRAME);
// 	}// end resizing frame
// 	else
// 	{// begin non resizing frame
// 		sFrameSize.cx = GetSystemMetrics(SM_CXFIXEDFRAME);
// 		sFrameSize.cy = GetSystemMetrics(SM_CYFIXEDFRAME);
// 	}// end non resizing frame

	return sFrameSize;
}// end GetFrameSize
/*************************************************/
void CCaptionButton::DeleteButton(const UINT &ID)
// removes the button with the ID
{// begin DeleteButton
	DeleteNode(ID);
	m_nNumButtons--;
	ReInitButtons();
	RedrawButtons();
}// end DeleteButton
/*************************************************/
void CCaptionButton::RedrawButtons() const
// redraw all of the captions, taking special care of the currently selected one
{// begin RedrawButtons
	for(int i = 0;i < m_vButtons.size();i++)
	{ // begin
		if(m_nLastPointNode == i)
			Draw(i,true);
		else
			Draw(i,false);
	} // end
	// draw the node with the focus
	if(m_nDownNode != -1)
		DrawSelection(m_nDownNode);
}// end RedrawCapions
/*************************************************/
HRGN CCaptionButton::CalculateRgn(const int &offset) const
// creates a HRGN for a button a the offset
{// begin CalculateRgn
	RECT captionRect = CalculateRect(offset,false);
	return CreateRectRgnIndirect(&captionRect);
}// end CalculateRgn

RECT CCaptionButton::CalculateRect(const int &offset,bool toClient) const
// calculate the rectangular area of the button at the offset
{// begin CalculateRect
	
	RECT windowRect;
	SIZE frameSize = GetFrameSize();
	SIZE buttonSize = GetButtonSize();
	::GetWindowRect(m_hWnd,&windowRect);


	RECT buttonRect = {NULL};


	buttonRect.left = windowRect.right-(buttonSize.cx*offset+frameSize.cx);
	buttonRect.top = windowRect.top+frameSize.cy;
	buttonRect.right = buttonRect.left+buttonSize.cx;
	buttonRect.bottom = buttonRect.top+buttonSize.cy;

// 	CString winRecttop,framsizetop;
// 	winRecttop.Format("winrecttop %ld", windowRect.top);
// 	AfxMessageBox(winRecttop);
// 	framsizetop.Format("frameSizetop %ld", frameSize.cy);
// 	AfxMessageBox(framsizetop);
	
	// check for partially covered up menu, and compensate
	RECT menuRect = {NULL};
	HMENU windowMenu = GetMenu(m_hWnd);
	if(windowMenu)
	{
		GetMenuItemRect(m_hWnd,windowMenu,0,&menuRect);
		if(menuRect.bottom != 0)	// if it is not a child window
		{
			buttonRect.bottom = menuRect.top-1;
			buttonRect.top = buttonRect.bottom-buttonSize.cy;
		}
	}

	if(toClient)
	{
		POINT topLeft,bottomRight;
		topLeft.x = buttonRect.left;
		topLeft.y = buttonRect.top;
		bottomRight.x = buttonRect.right;
		bottomRight.y = buttonRect.bottom;

		ScreenToClient(m_hWnd,&topLeft);
		ScreenToClient(m_hWnd,&bottomRight);
		buttonRect = CRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
	}
	return buttonRect;
}// end CalculateRect

void CCaptionButton::DrawSelection(const int &num) const
// draw the selection state bitmap
{// begin DrawSelection
	// declarations

	HDC hdcWin = ::GetDC(m_hWnd);

	// get bitmap's dimensions
	BITMAP bitmapDimentions = {NULL};
	::GetObject(m_hSelectionBitmap, sizeof( bitmapDimentions ), &bitmapDimentions );
	SIZE spacingSize = {bitmapDimentions.bmWidth,bitmapDimentions.bmHeight}; 

	// center the bitmap in the button area
	spacingSize = CenterBitmap(spacingSize);

	RECT windowRect = {NULL};
	GetWindowRect(m_hWnd,&windowRect);
	// get button rectangle
	RECT captionRect = CalculateRect(num+1 + m_nNumDefaultButtons,false);
	// draw the selection bitmap
	DrawTransparentBitmap(hdcWin
		,m_hSelectionBitmap
		,spacingSize.cx+captionRect.left-windowRect.left
		,spacingSize.cy+captionRect.top-windowRect.top
		,m_crTransparentColor);

	// cleanup
	::ReleaseDC(m_hWnd,hdcWin);
}// end DrawSelection

void CCaptionButton::SetSelectionBitmap(HBITMAP bitmap)
// sets the mousedown bitmap
{// begin SetSelectionBitmap
	if(m_hSelectionBitmap)
		DeleteObject(m_hSelectionBitmap);
	m_hSelectionBitmap = bitmap;
}// end SetSelectionBitmap

bool CCaptionButton::ReplaceButton(const UINT replaceButtonID,const UINT newID,HBITMAP newHilite,HBITMAP newNormal,char *newToolTip)
// replaces a current button with a new one
{// begin ReplaceButton
	// find the node to replace
	for(int i = 0;i < m_vButtons.size();i++)
	{// begin traverse
		if(m_vButtons[i].nID == replaceButtonID)
		{// begin replace button
			// update tooltip text
			TOOLINFO ti,newTip;
			ti.cbSize = sizeof(TOOLINFO); 
			ti.hwnd = m_hWnd; 
			ti.uId = (UINT) m_vButtons[i].nID;
			// close the tooltip (if it is open)
			::SendMessage(m_hToolTipWnd, TTM_POP , 0, 0);
			// delete the tooltip
			::SendMessage(m_hToolTipWnd, TTM_DELTOOL, 0, (LPARAM) (LPTOOLINFO) &ti);
			// set new tooltip parameters
			newTip.cbSize = sizeof(TOOLINFO); 
			newTip.hwnd = m_hWnd; 
			newTip.uId = (UINT) m_vButtons[i].nID;
			newTip.hinst = NULL; 
			newTip.uFlags = TTF_SUBCLASS; 
			newTip.rect = CalculateRect(i+1+m_nNumDefaultButtons);
			newTip.lpszText = (LPSTR) newToolTip;
			newTip.uId = newID;
			// add tool tip
			::SendMessage(m_hToolTipWnd, TTM_ADDTOOL, 0, (LPARAM) (LPTOOLINFO) &newTip);
			// clean up the old button's bitmaps
			::DeleteObject(m_vButtons[i].hHiliteBitmap);
			::DeleteObject(m_vButtons[i].hNormalBitmap);
			// set the new bitmaps
			m_vButtons[i].hHiliteBitmap = newHilite;
			m_vButtons[i].hNormalBitmap = newNormal;
			// set the new ID
			m_vButtons[i].nID = newID;
			// this clear the region so the non - overlapping part does not show
			UpdateRegion(m_vButtons[i].hRgn);
			// draw it
			Draw(i,false);
			// reset current node
			m_nLastPointNode = -1;
			return true;
		}// end replace button
	}// end traverse
	return false;
}// end ReplaceButton

bool CCaptionButton::OnWindowChange()
// called when an aspect of the window's position changes
{// begin OnWindowChange
	ReInitButtons();
	return true;
}// end OnWindowChange

LRESULT CCaptionButton::OnNcPaint(UINT message,WPARAM wParam, LPARAM lParam)
// called when the non-client region needs to be painted
{// begin OnNcPaint
	LRESULT lResult = CallWindowProc(m_wpOldWndProc,m_hWnd,message,wParam,lParam);
	RedrawButtons();
	return lResult;
}// end OnNcPaint

LRESULT CCaptionButton::OnSystemChange()
// called when an aspect of the system changes
{// begin OnSystemChange
	RedrawButtons();
	return 0;
}// end OnSystemChange

LRESULT CCaptionButton::OnMouseMove(UINT message,WPARAM wParam, LPARAM lParam)
// called when the cursor moves
{// begin OnMouseMove
	POINTS pt = {NULL};
	POINT pnt = {NULL};
	if(m_bCapturing)
	{// begin convert coords
		// convert cursor coordinates
		pnt.x = LOWORD(lParam);
		pnt.y = HIWORD(lParam);
		ClientToScreen(m_hWnd,&pnt);
		pt.x = (short)pnt.x;
		pt.y = (short)pnt.y;
	}// end convert coords
	else if(message != WM_MOUSEMOVE )
	{// begin not capturing
		pt = MAKEPOINTS(lParam);			
	}// end not capturing
	int nCurrentNode = -1;
	for(int i = 0;i < m_nNumButtons;i++)
	{ // begin get current node
		if(nCurrentNode == -1 && ::PtInRegion(m_vButtons[i].hRgn, pt.x,pt.y))
		{
			// set this as the current button
			nCurrentNode = i;
			// done with this
			break;
		}
	} // end end get current node
	if(nCurrentNode != -1 && m_nLastPointNode != nCurrentNode)
	{ // begin Point in Region
			// check to see if the bitmaps are different
			if(m_vButtons[nCurrentNode].hHiliteBitmap != m_vButtons[nCurrentNode].hNormalBitmap)
			{// begin bitmaps are different
				// clear the drawing area
				UpdateRegion(m_vButtons[nCurrentNode].hRgn);
				// the point is in the current region so draw it in hilited mode
				if(m_nDownNode != -1 && m_nDownNode != nCurrentNode)
					Draw(nCurrentNode,false);
				else
					Draw(nCurrentNode,true);
			}// end bitmaps are different
			// draw it in mouse down mode
			if(m_nDownNode == nCurrentNode)
				DrawSelection(nCurrentNode);
	} // end Point in Region
	if(m_nLastPointNode != -1 && m_nLastPointNode != nCurrentNode)
	{
		if(m_vButtons[m_nLastPointNode].hHiliteBitmap != m_vButtons[m_nLastPointNode].hNormalBitmap || m_nDownNode == m_nLastPointNode)
		{// begin bitmaps are different
			UpdateRegion(m_vButtons[m_nLastPointNode].hRgn);
			// draw it in normal state
			Draw(m_nLastPointNode,false);
		}// end bitmaps are different
	}
	// set the last button
	m_nLastPointNode = nCurrentNode;
	return 1;
}// end OnMouseMove

LRESULT CCaptionButton::OnNcLButtonDown(unsigned long message, WPARAM wParam, LPARAM lParam)
// called when the left mouse button is pressed in the non-client area
{// begin OnNcLButtonDown
	POINTS pt = MAKEPOINTS(lParam);
	for(int i = 0;i < m_vButtons.size();i++)
	{ // begin traverse
		if(::PtInRegion(m_vButtons[i].hRgn, pt.x,pt.y))
		{ // begin button has been pressed
			// capture the mouse messages so that the cursor can be tracked outside of the window
			SetCapture(m_hWnd);
			m_bCapturing = true;
			DrawSelection(m_nDownNode=i);
			return 0;
		} // end button has been pressed
	} // end traverse
	

	
	return 1;
}// end OnNcLButtonDown

LRESULT CCaptionButton::OnNcLButtonUp(unsigned long message, WPARAM wParam, LPARAM lParam)
// called when the left mouse button is released in the non-client area
{// begin OnNcLButtonUp
	POINTS pt = {NULL};
	if(message == WM_LBUTTONUP)
	{// begin convert point
		// convert client cursor coordinates to screen coordinates
		POINT point;
		point.x = LOWORD(lParam);
		point.y = HIWORD(lParam);
		ClientToScreen(m_hWnd,&point);
		pt.x = (short)point.x;
		pt.y = (short)point.y;
	}// end convert point
	else
		pt = MAKEPOINTS(lParam);
	if(m_nDownNode != -1 )	
	{// begin the mouse was down
		if(::PtInRegion(m_vButtons[m_nDownNode].hRgn, pt.x,pt.y))
		{ // begin mouse on  same button
			if(GetNode(m_vButtons[m_nDownNode].nID) == -1)
				return 0;
			// send the message to the window that a button was pressed
			::SendMessage(m_hWnd,WM_CBLBUTTONCLICKED,m_vButtons[m_nDownNode].nID,lParam);
			if(m_nDownNode >= 0 )
			{
				// clear the region
				UpdateRegion(m_vButtons[m_nDownNode].hRgn);
				// draw it
				Draw(m_nDownNode,true);
			}
		} // end mouse on  same button
	}// end the mouse was down
	m_nDownNode = -1;
	return 1;
}// end OnNcLButtonUp

LRESULT CCaptionButton::OnDestroy()
// called when the window is about to be destroyed
{// begin OnDestroy
	// remove subclass of owner's m_hWnd message function
	if(m_wpOldWndProc)
	{
		SetWindowLong(m_hWnd,GWL_WNDPROC,(DWORD)m_wpOldWndProc);
	}
	// destroy the tooltip window
	if(m_hToolTipWnd)
	{
		SendMessage(m_hToolTipWnd,WM_DESTROY,NULL,NULL);
		m_hToolTipWnd = NULL;
	}
	// delete the selection bitmap
	if(m_hSelectionBitmap)
	{
		DeleteObject(m_hSelectionBitmap);
		m_hSelectionBitmap = NULL;
	}
	return 1;
}// end OnDestroy

LRESULT CCaptionButton::OnLButtonUp(unsigned long message, WPARAM wParam, LPARAM lParam)
// called when the left mouse button is press
// releases the capture of the mouse messages
{// begin OnLButtonUp
	if(m_bCapturing)
	{// begin currently capturing mouse
		ReleaseCapture();
		m_bCapturing = false;
		// reset the last node which was down
		m_nLastPointNode = -1;
		return 0;
	}// end currently capturing mouse
	return 1;
}// end OnLButtonUp

bool CCaptionButton::DeleteButton(int nIndex)
// cleans up the button at nIndex, but does no release the structure's memory
{// begin DeleteButton
	// remove the tooltip
	TOOLINFO ti = {NULL};
	ti.hwnd = m_hWnd; 
	ti.uId = (UINT) m_vButtons[nIndex].nID;
	ti.cbSize = sizeof(ti); 
	::SendMessage(m_hToolTipWnd, TTM_DELTOOL, 0, (LPARAM) (LPTOOLINFO) &ti);
	// release all memory in node
	DeleteObject(m_vButtons[nIndex].hNormalBitmap);
	DeleteObject(m_vButtons[nIndex].hHiliteBitmap);
	DeleteObject(m_vButtons[nIndex].hRgn);
	return true;
}// end DeleteButton

bool CCaptionButton::UpdateRegion(HRGN hUpdateRegion)
// clears the non-client drawing area of hUpdateRegion
// non-client region that is to be updated
{// begin UpdateRegion
	if(!hUpdateRegion)
		return false;
	m_bMyRedraw = true;
	// this clear the region so the non-overlapping part does not show
	//::SendMessage(m_hWnd,WM_NCPAINT,(WPARAM)hUpdateRegion,NULL);
	m_bMyRedraw = false;
	return true;
}// end UpdateRegion

CCaptionButton::ButtonData CCaptionButton::CreateButton(const UINT &ID, HBITMAP hilite, HBITMAP normal, char *sTooltip)
// returns a ButtonData
{// begin CreateButton
	ButtonData bdNewButton;
	// Calculate next button pos
	m_nNumButtons++;
	int buttonOffset = m_nNumButtons + m_nNumDefaultButtons;
	// initilize button params
	bdNewButton.nID = ID;
	bdNewButton.hNormalBitmap = normal;
	bdNewButton.hHiliteBitmap = hilite;
	bdNewButton.hRgn = CalculateRgn(buttonOffset);
	
	// Create a new tool tip
	TOOLINFO ti = {NULL};
	ti.cbSize = sizeof(ti); 
	ti.uFlags = TTF_SUBCLASS; 
	ti.hwnd = m_hWnd; 
	ti.hinst = NULL; 
	ti.uId = (UINT) ID;
	ti.lpszText = (LPSTR) sTooltip;
	ti.rect = CalculateRect(buttonOffset);
	// activate tool tip
    ::SendMessage(m_hToolTipWnd, TTM_ADDTOOL, 0, (LPARAM) (LPTOOLINFO) &ti);
	return bdNewButton;
}// end CreateButton

void CCaptionButton::ReInitButtons()
// recalculates all of the captions' regions and updates the tooltips accordingly
{// begin ReInitButtons
	int buttonOffset = 0;
	// Recalculate button pos
	for(int i = 0;i < m_vButtons.size();i++)
	{ // begin traverse
		// calc the button position
		buttonOffset = i+1 + m_nNumDefaultButtons;
		// clean up the previous region
		DeleteObject(m_vButtons[i].hRgn);
		// create the new region
		m_vButtons[i].hRgn = CalculateRgn(buttonOffset);
		// prepare the tooltip
		TOOLINFO ti = {NULL};
		ti.cbSize = sizeof(TOOLINFO);
		ti.hwnd = m_hWnd;
		// calc the button's rectangle
		ti.rect = CalculateRect(buttonOffset);
		ti.uId = m_vButtons[i].nID;
		ti.uFlags = TTF_SUBCLASS; 
		ti.hinst = NULL;
		// set the new position
		SendMessage(m_hToolTipWnd, TTM_NEWTOOLRECT, 0, (LPARAM) (LPTOOLINFO) &ti);	
	} // end traverse
}// end ReInitButtons

bool CCaptionButton::AddButton(const UINT nID,HICON hHilite,HICON hNormal,char *pTooltip,const int nInsertPostion)
// created a new button using HICONs
// the HICON's are destroyed by this function
{// begin AddButton
	HBITMAP hHiliteBitmap = IconToBitmap(hHilite,m_crTransparentColor);
	HBITMAP hNormalBitmap = IconToBitmap(hNormal,m_crTransparentColor);
	// clean up the icons
	DestroyIcon(hHilite);
	DestroyIcon(hNormal);
	return AddButton(nID,hHiliteBitmap,hNormalBitmap,pTooltip,nInsertPostion);
}// end AddButton

HBITMAP CCaptionButton::IconToBitmap(HICON hIcon, COLORREF crTrans)
// converts a HICON to a HBITMAP using the crTrans as the transparent color for the bitmap
// returns null if there is no icon
{// begin IconToBitmap
	if(!hIcon)
		return NULL;
	SIZE m_sDimensions = {16,16};
	// create DC
	HDC hScreenDC = ::GetDC(NULL);
	HDC hBitmapDC = CreateCompatibleDC(hScreenDC);
	HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC,m_sDimensions.cx,m_sDimensions.cy);
	// select the bitmap into the DC
	HGDIOBJ hOldBitmap = SelectObject(hBitmapDC,hBitmap);
	// release screen dc
	ReleaseDC(NULL,hScreenDC);
	// init dc to the transparent color
	HBRUSH hBrush = CreateSolidBrush(crTrans);
	// draw the icon
	DrawIconEx(hBitmapDC,0,0,hIcon,m_sDimensions.cx,m_sDimensions.cy,NULL,hBrush,DI_NORMAL|DI_IMAGE);
	// release the bitmap
	SelectObject(hBitmapDC,hOldBitmap);
	// clean up
	DeleteObject(hBrush);
	DeleteDC(hBitmapDC);
	// return our bitmap
	return hBitmap;
}// end IconToBitmap

HBITMAP CCaptionButton::CombineBitmaps(HBITMAP hSource, HBITMAP hOverlay, COLORREF crTrans)
{// begin CombineBitmaps
	if(!hSource || !hOverlay)
		return NULL;
	// get the image dimensions
	BITMAP bm = {NULL};
    GetObject(hSource, sizeof(BITMAP), (LPSTR)&bm);
	SIZE m_sDimensions = {bm.bmWidth,bm.bmHeight};
	// create DC
	HDC hScreenDC = ::GetDC(NULL);
	HDC hBitmapDC = CreateCompatibleDC(hScreenDC);
	// select the bitmap into the DC
	HBITMAP hBitmap = CreateCompatibleBitmap(hScreenDC,m_sDimensions.cx,m_sDimensions.cy);
	HGDIOBJ hOldBitmap = SelectObject(hBitmapDC,hBitmap);
	// release screen dc
	ReleaseDC(NULL,hScreenDC);
	// fill the background to the transparent color
	RECT rImage = {0,0,m_sDimensions.cx,m_sDimensions.cy};
	HBRUSH hBrush = CreateSolidBrush(crTrans);
	FillRect(hBitmapDC,&rImage,hBrush);
	// draw the source
	DrawTransparentBitmap(hBitmapDC,hSource,0,0,crTrans);
	// draw the overlay
	DrawTransparentBitmap(hBitmapDC,hOverlay,0,0,crTrans);
	// release the bitmap
	SelectObject(hBitmapDC,hOldBitmap);
	// clean up
	DeleteDC(hBitmapDC);
	DeleteObject(hBrush);
	// return our new bitmap
	return hBitmap;
}// end CombineBitmaps

char * CCaptionButton::GetButtonText(unsigned long int nID,char *pBuffer)
{// begin GetButtonText
	// set the default info
	TOOLINFO ti = {NULL};
	ti.hwnd = m_hWnd;
	ti.uId = nID;
	ti.lpszText = pBuffer;
	ti.cbSize = sizeof(TOOLINFO);
	// send the info
	SendMessage(m_hToolTipWnd, TTM_GETTEXT, 0, (LPARAM) (LPTOOLINFO) &ti);
	// return the buffer
	return pBuffer;
}// end GetButtonText

void CCaptionButton::DrawTransparentBitmap(HDC hDC, HBITMAP hBitmap, int x, int y, COLORREF crColour)
// written by Paul Reynolds: Paul.Reynolds@cmgroup.co.uk
// CodeGuru article: "Transparent Bitmap - True Mask Method"
// http://codeguru.earthweb.com/bitmap/CISBitmap.shtml
{
	COLORREF crOldBack = SetBkColor(hDC,RGB(255,255,255));
	COLORREF crOldText = SetTextColor(hDC,RGB(0,0,0));
	HDC dcImage, dcTrans;

	// Create two memory dcs for the image and the mask
	dcImage = CreateCompatibleDC(hDC);
	dcTrans = CreateCompatibleDC(hDC);

	// Select the image into the appropriate dc
	HGDIOBJ hOldBitmapImage = SelectObject(dcImage,hBitmap);

	// Create the mask bitmap
	HBITMAP bitmapTrans = NULL;
	// get the image dimensions
	BITMAP bm = {NULL};
    GetObject(hBitmap, sizeof(BITMAP), (LPSTR)&bm);
	int nWidth = bm.bmWidth;
	int nHeight = bm.bmHeight;
	bitmapTrans = CreateBitmap(nWidth, nHeight, 1, 1, NULL);

	// Select the mask bitmap into the appropriate dc
	HGDIOBJ hOldBitmapTrans = SelectObject(dcTrans,bitmapTrans);

	// Build mask based on transparent colour
	SetBkColor(dcImage,crColour);
	BitBlt(dcTrans, 0, 0, nWidth, nHeight, dcImage, 0, 0, SRCCOPY);

	// Do the work - True Mask method - cool if not actual display
	BitBlt(hDC,x, y, nWidth, nHeight, dcImage, 0, 0, SRCINVERT);
	BitBlt(hDC,x, y, nWidth, nHeight, dcTrans, 0, 0, SRCAND);
	BitBlt(hDC,x, y, nWidth, nHeight, dcImage, 0, 0, SRCINVERT);

	// Restore settings
	// don't delete this, since it is the bitmap
	SelectObject(dcImage,hOldBitmapImage);
	// delete bitmapTrans
	DeleteObject(SelectObject(dcTrans,hOldBitmapTrans));
	SetBkColor(hDC,crOldBack);
	SetTextColor(hDC,crOldText);
	// clean up
	DeleteDC(dcImage);
	DeleteDC(dcTrans);
}

