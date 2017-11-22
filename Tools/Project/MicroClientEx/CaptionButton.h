//////////////////////
// CaptionButton.h //
//////////////////////
#ifndef _CAPTIONBUTTON_H_
#define _CAPTIONBUTTON_H_


#include <vector>

using namespace std;

#define WM_CBLBUTTONCLICKED WM_USER+1

////////////////////////
// 虚函数指针数据
typedef unsigned char PtrData[9];

//VC编译时链接虚函数通过下面3句机器码替换某个虚函数
void inline PtrInit(PtrData t, void * This)
{
	t[0] = 0xB9; // mov ecx, 
	*((DWORD *)(t+1)) = (DWORD) This; // this
	*((DWORD *)(t+5)) = 0x20FF018B; // mov eax, [ecx] 
	// jmp [eax]
}

class CCaptionButton
{

	// Construction
protected:
	virtual LRESULT WindowProc(HWND,UINT,WPARAM,LPARAM);
	PtrData m_PtrData;

public:
	struct ButtonData
	{ // begin ButtonData
		unsigned long int nID;
		HBITMAP hNormalBitmap;
		HBITMAP hHiliteBitmap;
		HRGN hRgn;
		// ButtonData constructor
		ButtonData::ButtonData() : nID(0) , hNormalBitmap(NULL), hHiliteBitmap(NULL), hRgn(NULL) {}
	}; // end ButtonData
	
	CCaptionButton();
	~CCaptionButton();

// Attributes
	void Init(HWND);
	void UnInit() {
		m_hWnd = NULL;
	}
	void SetSelectionBitmap(HBITMAP bitmap);
	void SetTransparentColor(const int &);
	void SetNumOfDefaultButtons(const UINT &);
	char * GetButtonText(unsigned long int nID,char *pBuffer);
	bool AddButton(const UINT ,HBITMAP hilite,HBITMAP normal,char*,int nInsertPostion = -1);
	bool AddButton(const UINT nID,HICON hHilite,HICON hNormal,char *pTooltip,const int nInsertPostion = -1);
	void DeleteButton(const UINT &);
	bool ReplaceButton(const UINT replaceButtonID,const UINT newID,HBITMAP newHilite,HBITMAP newNormal,char *newToolTip);
	static HBITMAP CombineBitmaps(HBITMAP hSource, HBITMAP hOverlay, COLORREF crTrans);
	static HBITMAP IconToBitmap(HICON hIcon,COLORREF crTrans);
	static void DrawTransparentBitmap(HDC hDC, HBITMAP hBitmap, int x, int y, COLORREF crColour);
	void RedrawButtons() const;

protected:
	ButtonData CreateButton(const UINT &ID,HBITMAP hilite,HBITMAP normal,char *tooltipString);

	void ReInitButtons();
	bool UpdateRegion(HRGN hUpdateRegion);
	bool DeleteButton(int nIndex);
public:
	RECT CalculateRect(const int &offset, bool toClient = true) const;
protected:
	HRGN CalculateRgn(const int &offset) const;
	bool Insert(ButtonData *pNode,int nInsertPosition = -1);
	void DeleteNode(const UINT &);
	void Draw(int, bool) const;
	void DrawSelection(const int &num) const;
	void DeleteList();
	SIZE CenterBitmap(const SIZE &size) const;
	int GetNode(const UINT &find) const;
	SIZE GetButtonSize() const;
	SIZE GetFrameSize() const;
	//void RedrawButtons() const;

	bool OnWindowChange(void);
	LRESULT OnLButtonUp(unsigned long int message,WPARAM wParam,LPARAM lParam);
	LRESULT OnDestroy();
	LRESULT OnNcLButtonUp(unsigned long int message,WPARAM wParam,LPARAM lParam);
	LRESULT OnNcLButtonDown(unsigned long int message,WPARAM wParam,LPARAM lParam);
	LRESULT OnMouseMove(UINT message,WPARAM wParam, LPARAM lParam);
	LRESULT OnSystemChange(void);
	LRESULT OnNcPaint(UINT message,WPARAM wParam, LPARAM lParam);

	/******* member variables ************/
	unsigned long int m_fStyles;
	unsigned long int m_fExStyles;
	int m_nLastPointNode;
	bool m_bMyRedraw;
	bool m_bCapturing;
	HWND m_hToolTipWnd;
	WNDPROC m_wpOldWndProc;
	HWND m_hWnd;
	vector <ButtonData> m_vButtons;
	int m_nDownNode;
	HBITMAP m_hSelectionBitmap;
	int m_nNumButtons;
	UINT m_crTransparentColor;
	UINT m_nNumDefaultButtons;
};

#endif