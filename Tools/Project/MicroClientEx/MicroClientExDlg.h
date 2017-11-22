// MicroClientExDlg.h : header file
//

#if !defined(AFX_MICROCLIENTEXDLG_H__D48107E2_CBEC_46AB_A3AB_C2AA4F8471B7__INCLUDED_)
#define AFX_MICROCLIENTEXDLG_H__D48107E2_CBEC_46AB_A3AB_C2AA4F8471B7__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CMicroClientExDlg dialog
#include <vector>
#include "PngBtn.h"
#include "ProgressBar.h"
#include "ContainerDialog.h"
using namespace std;
#include "resource.h"

class CMicroClientExDlg : public CDialog
{
// Construction
public:
	CMicroClientExDlg(CWnd* pParent = NULL);	// standard constructor

	HDC m_hdcMemory;
	int m_BakWidth; // 背景图像宽	
	int m_BakHeight; // 背景图像高
	BLENDFUNCTION m_Blend;
	Image *bgImage;

	ContainerDialog *container;
	PngBtn *closeBtn;
	PngBtn *miniBtn;
	ProgressBar *progressBarVice;
	ProgressBar *progressBarMain;
	HINSTANCE hFuncInst ;
	typedef BOOL (WINAPI *MYFUNC)(HWND,HDC,POINT*,SIZE*,HDC,POINT*,COLORREF,BLENDFUNCTION*,DWORD);          
	MYFUNC UpdateLayeredWindow;

	void CMicroClientExDlg::RePaintWindow();
	DWORD GetProcessByName(LPCTSTR name);
	
	int CMicroClientExDlg::GetInfoLenght(WCHAR * _array);
	static WCHAR* CMicroClientExDlg::ConverIntToString(int num,int printftype = 0);
	void CMicroClientExDlg::InvokeCallGameThread(CString url);
	void CMicroClientExDlg::InvokeCallWebClient(CString url);
	void CMicroClientExDlg::InvokeCallByJsJudgeClient();//用于js调用微端判断是否为新版本
	static UINT CMicroClientExDlg::IvockeDownload(LPVOID lpParam);
	//static void CMicroClientExDlg::OnMerge();
	static bool CMicroClientExDlg::Download(const CString& strFileURLInServer,const CString & strFileLocalFullPath,bool record = FALSE);//存放到本地的路径 
	static void CMicroClientExDlg::ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container);	
	//static void CMicroClientExDlg::UnCompressFile(CString strFilePath,CString desFilePath);
	//static UINT CMicroClientExDlg::IvockeDownloadThread1(LPVOID lpParam);
	static BOOL GetIniConfigParam(char* keyName, char* retBuf, int size,char* szPath);
	//static BOOL CheckMd5Num(LPVOID lpparam);//校验md5码
	static UINT SendInfo(LPVOID lpParam); //发送微端统计
	static void AnalyVersion(CString version,int* versionnum);
	static CString ReadSerXml(CString key);
	CString FindurlParam(CString url,CString urlparam,CString urlparamsign);//查找js抛的参数
	void GetServerVision();
	
	

	

// Dialog Data
	//{{AFX_DATA(CMicroClientExDlg)
	enum { IDD = IDD_MICROCLIENTEX_DIALOG };
		// NOTE: the ClassWizard will add data members here
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CMicroClientExDlg)
	public:
	virtual BOOL PreTranslateMessage(MSG* pMsg);
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CMicroClientExDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnMouseMove(UINT nFlags, CPoint point);
	afx_msg void OnLButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnLButtonDown(UINT nFlags, CPoint point);
	afx_msg void OnMove(int x, int y);
	virtual void OnOK();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

//序列化字符串(静态变量才能访问其地址)
static CString serialCode ;

//下载是否完成
static bool downloadFlag = TRUE;

//下载进度显示(文字)
static int downloadplanflg = 0;

//主进度条值
static float loading_procgrss_main = 0;

//副进度条值
static float loading_procgrss_vice = 0;

//XML缓存(全局变量)
static char* bufferXML;

//xml下载目录下载到当前位置
static int recentNum;

//资源列表
#define CONFIG	"config.xml"
#define THREADNUM "threadnum.xml"
#define VERSIONNUM 4//版本号数组个数

//下载的游戏文件保存路径
static CString m_strLocalPath;

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MICROCLIENTEXDLG_H__D48107E2_CBEC_46AB_A3AB_C2AA4F8471B7__INCLUDED_)
