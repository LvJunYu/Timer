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
	int m_BakWidth; // ����ͼ���	
	int m_BakHeight; // ����ͼ���
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
	void CMicroClientExDlg::InvokeCallByJsJudgeClient();//����js����΢���ж��Ƿ�Ϊ�°汾
	static UINT CMicroClientExDlg::IvockeDownload(LPVOID lpParam);
	//static void CMicroClientExDlg::OnMerge();
	static bool CMicroClientExDlg::Download(const CString& strFileURLInServer,const CString & strFileLocalFullPath,bool record = FALSE);//��ŵ����ص�·�� 
	static void CMicroClientExDlg::ReadXmlFile(CString& szFileName,vector<CString> &fileContainer,vector<CString> &md5Container);	
	//static void CMicroClientExDlg::UnCompressFile(CString strFilePath,CString desFilePath);
	//static UINT CMicroClientExDlg::IvockeDownloadThread1(LPVOID lpParam);
	static BOOL GetIniConfigParam(char* keyName, char* retBuf, int size,char* szPath);
	//static BOOL CheckMd5Num(LPVOID lpparam);//У��md5��
	static UINT SendInfo(LPVOID lpParam); //����΢��ͳ��
	static void AnalyVersion(CString version,int* versionnum);
	static CString ReadSerXml(CString key);
	CString FindurlParam(CString url,CString urlparam,CString urlparamsign);//����js�׵Ĳ���
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

//���л��ַ���(��̬�������ܷ������ַ)
static CString serialCode ;

//�����Ƿ����
static bool downloadFlag = TRUE;

//���ؽ�����ʾ(����)
static int downloadplanflg = 0;

//��������ֵ
static float loading_procgrss_main = 0;

//��������ֵ
static float loading_procgrss_vice = 0;

//XML����(ȫ�ֱ���)
static char* bufferXML;

//xml����Ŀ¼���ص���ǰλ��
static int recentNum;

//��Դ�б�
#define CONFIG	"config.xml"
#define THREADNUM "threadnum.xml"
#define VERSIONNUM 4//�汾���������

//���ص���Ϸ�ļ�����·��
static CString m_strLocalPath;

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_MICROCLIENTEXDLG_H__D48107E2_CBEC_46AB_A3AB_C2AA4F8471B7__INCLUDED_)
