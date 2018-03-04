#include "stdafx.h"
#include "ImpIDispatch.h"
#include "MicroClientExDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif


CString cszCB_IsOurCustomBrowser		= "CB_IsOurCustomBrowser";
CString cszCB_Close						= "CB_Close";
CString cszCB_CustomFunction			= "CB_CustomFunction";
CString cszCB_CustomFunctionWithParams	= "CB_CustomFunctionWithParams";
CString cszCB_CustomFunctionAdd			= "CB_CustomFunctionAdd";
CString cszCB_ShowModalDialog			= "CB_ShowModalDialog";
CString cszCB_ShowModelessDialog		= "CB_ShowModelessDialog";
CString cszCB_InvokeCallByJsJudgeClient = "CB_InvokeCallByJsJudgeClient";
CString cszCB_InvokeCallWebClient		= "CB_InvokeCallWebClient";

#define	DISPID_CB_IsOurCustomBrowser		1
#define DISPID_CB_Close						2
#define DISPID_CB_CustomFunction			3
#define DISPID_CB_CustomFunctionWithParams	4
#define	DISPID_CB_CustomFunctionAdd			5
#define DISPID_CB_ShowModalDialog			6
#define DISPID_CB_ShowModelessDialog		7
#define DisPid_CB_InvokeCallByJsJudgeClient 8
#define DisPid_CB_InvokeCallWebClient       9

CImpIDispatch::CImpIDispatch(void)
{
	m_cRef = 0;
}

CImpIDispatch::~CImpIDispatch(void)
{
	ASSERT(m_cRef==0);
}

STDMETHODIMP CImpIDispatch::QueryInterface(REFIID riid, void** ppv)
{
	*ppv = NULL;
	
	if(IID_IDispatch == riid)
	{
        *ppv = this;
	}
	
	if(NULL != *ppv)
    {
		((LPUNKNOWN)*ppv)->AddRef();
		return NOERROR;
	}
	
	return E_NOINTERFACE;
}


STDMETHODIMP_(ULONG) CImpIDispatch::AddRef(void)
{
	return ++m_cRef;
}

STDMETHODIMP_(ULONG) CImpIDispatch::Release(void)
{
	return --m_cRef;
}

STDMETHODIMP CImpIDispatch::GetTypeInfoCount(UINT* /*pctinfo*/)
{
	return E_NOTIMPL;
}

STDMETHODIMP CImpIDispatch::GetTypeInfo(
			/* [in] */ UINT /*iTInfo*/,
            /* [in] */ LCID /*lcid*/,
            /* [out] */ ITypeInfo** /*ppTInfo*/)
{
	return E_NOTIMPL;
}

STDMETHODIMP CImpIDispatch::GetIDsOfNames(
			/* [in] */ REFIID riid,
            /* [size_is][in] */ OLECHAR** rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID* rgDispId)
{
	HRESULT		hr		= NOERROR;
	UINT		i		= 0;
	CString		cszName	= rgszNames[i];

	for(i=0; i<cNames; ++i)
	{
// 		if(cszName == cszCB_IsOurCustomBrowser)
// 		{
// 			rgDispId[i] = DISPID_CB_IsOurCustomBrowser;
// 		}
		/*else */if(cszName == cszCB_Close)
		{
			rgDispId[i] = DISPID_CB_Close;
		}
// 		else if(cszName == cszCB_CustomFunction)
// 		{
// 			rgDispId[i] = DISPID_CB_CustomFunction;
// 		}
// 		else if(cszName == cszCB_CustomFunctionWithParams)
// 		{
// 			rgDispId[i] = DISPID_CB_CustomFunctionWithParams;
// 		}
// 		else if(cszName == cszCB_CustomFunctionAdd)
// 		{
// 			rgDispId[i] = DISPID_CB_CustomFunctionAdd;
// 		}
// 		else if(cszName == cszCB_ShowModalDialog)
// 		{
// 			rgDispId[i] = DISPID_CB_ShowModalDialog;
// 		}
// 		else if(cszName == cszCB_ShowModelessDialog)
// 		{
// 			rgDispId[i] = DISPID_CB_ShowModelessDialog;
// 		}
		else if(cszName == cszCB_InvokeCallByJsJudgeClient)
		{
			rgDispId[i] = DisPid_CB_InvokeCallByJsJudgeClient;
		}
		else if(cszName == cszCB_InvokeCallWebClient)
		{
			rgDispId[i] = DisPid_CB_InvokeCallWebClient;
		}
		else
		{
			// One or more are unknown so set the return code accordingly
			hr = ResultFromScode(DISP_E_UNKNOWNNAME);
			rgDispId[i] = DISPID_UNKNOWN;
		}

	}
	return hr;
}

STDMETHODIMP CImpIDispatch::Invoke(
			/* [in] */ DISPID dispIdMember,
			/* [in] */ REFIID /*riid*/,
			/* [in] */ LCID /*lcid*/,
			/* [in] */ WORD wFlags,
			/* [out][in] */ DISPPARAMS* pDispParams,
			/* [out] */ VARIANT* pVarResult,
			/* [out] */ EXCEPINFO* /*pExcepInfo*/,
			/* [out] */ UINT* puArgErr)
{
	CMicroClientExDlg*	pDlg	= (CMicroClientExDlg*)theApp.m_pMainWnd;
// 	if(dispIdMember == DISPID_CB_IsOurCustomBrowser)
// 	{
// 		if(wFlags & DISPATCH_PROPERTYGET)
// 		{
// 			if(pVarResult != NULL)
// 			{
// 				VariantInit(pVarResult);
// 				V_VT(pVarResult) = VT_BOOL;
// 				V_BOOL(pVarResult) = true;
// 			}
// 		}
// 		
// 		if(wFlags & DISPATCH_METHOD)
// 		{
// 			VariantInit(pVarResult);
// 			V_VT(pVarResult) = VT_BOOL;
// 			V_BOOL(pVarResult) = true;
// 		}
// 	}
	
	if(dispIdMember == DISPID_CB_Close) 
	{
		if(wFlags & DISPATCH_PROPERTYGET)
		{
			if(pVarResult != NULL)
			{
				VariantInit(pVarResult);
				V_VT(pVarResult) = VT_BOOL;
				V_BOOL(pVarResult) = true;
			}
		}
		
		if(wFlags & DISPATCH_METHOD)
		{
			pDlg->EndDialog(0);// 打开此端口方便以后js调用
		}
	}
	
// 	调用应用程序指令(无参数)
// 		if(dispIdMember == DISPID_CB_CustomFunction) 
// 		{
// 			if(wFlags & DISPATCH_PROPERTYGET)
// 			{
// 				if(pVarResult != NULL)
// 				{
// 					VariantInit(pVarResult);
// 					V_VT(pVarResult) = VT_BOOL;
// 					V_BOOL(pVarResult) = true;
// 				}
// 			}
// 			
// 			if(wFlags & DISPATCH_METHOD)
// 			{
// 	 			//pDlg->InvokeFunc();
// 				//pDlg->InvokeCallByJsJudgeClient();
// 			}
// 		}
// 		
// 		if(dispIdMember == DISPID_CB_CustomFunctionWithParams) 
// 		{
// 			if(wFlags & DISPATCH_PROPERTYGET)
// 			{
// 				if(pVarResult != NULL)
// 				{
// 					VariantInit(pVarResult);
// 					V_VT(pVarResult) = VT_BOOL;
// 					V_BOOL(pVarResult) = true;
// 				}
// 			}
// 			
// 			if(wFlags & DISPATCH_METHOD)
// 			{
// 				// arguments come in reverse order
// 				// for some reason
// 				CString	strArg1	= pDispParams->rgvarg[0].bstrVal;	// in case you want a CString copy
// 				pDlg->InvokeCallGameThread(strArg1);
// 				//int		iArg2	= pDispParams->rgvarg[0].intVal;
// 				//pDlg->InvokeFuncWithParams(strArg1, iArg2);
// 			}
// 		}
// 	
// 		if(dispIdMember == DISPID_CB_CustomFunctionAdd) 
// 		{
// 			if(wFlags & DISPATCH_PROPERTYGET)
// 			{
// 				if(pVarResult != NULL)
// 				{
// 					VariantInit(pVarResult);
// 					V_VT(pVarResult) = VT_BOOL;
// 					V_BOOL(pVarResult) = true;
// 				}
// 			}
// 			
// 			if(wFlags & DISPATCH_METHOD)
// 			{
// 				// arguments come in reverse order
// 				// for some reason
// 				int		iArg1	= pDispParams->rgvarg[1].intVal;
// 				int		iArg2	= pDispParams->rgvarg[0].intVal;
// 				//pDlg->InvokeFuncAdd(iArg1, iArg2);
// 			}
// 		}
// 		
// 		if(dispIdMember == DISPID_CB_ShowModelessDialog)
// 		{
// 			if(wFlags & DISPATCH_PROPERTYGET)
// 			{
// 				if(pVarResult != NULL)
// 				{
// 					VariantInit(pVarResult);
// 					V_VT(pVarResult) = VT_BOOL;
// 					V_BOOL(pVarResult) = true;
// 				}
// 			}
// 			
// 			if(wFlags & DISPATCH_METHOD)
// 			{
// 				// arguments come in reverse order
// 				// for some reason
// 				CString cszArg1= pDispParams->rgvarg[4].bstrVal;	// in case you want a CString copy
// 				int iArg2= pDispParams->rgvarg[3].intVal;
// 				int iArg3= pDispParams->rgvarg[2].intVal;
// 				int iArg4= pDispParams->rgvarg[1].intVal;
// 				int iArg5= pDispParams->rgvarg[0].intVal;
// 				//pDlg->ShowModalLessDlg(cszArg1, CRect(iArg2, iArg3, iArg4, iArg5));
// 			}
// 		}
// 		
// 		if(dispIdMember == DISPID_CB_ShowModalDialog) 
// 		{
// 			if(wFlags & DISPATCH_PROPERTYGET)
// 			{
// 				if(pVarResult != NULL)
// 				{
// 					VariantInit(pVarResult);
// 					V_VT(pVarResult) = VT_BOOL;
// 					V_BOOL(pVarResult) = true;
// 				}
// 			}
// 			
// 			if(wFlags & DISPATCH_METHOD)
// 			{
// 				// arguments come in reverse order
// 				// for some reason
// 				CString cszArg1= pDispParams->rgvarg[4].bstrVal;	// in case you want a CString copy
// 				int iArg2= pDispParams->rgvarg[3].intVal;
// 				int iArg3= pDispParams->rgvarg[2].intVal;
// 				int iArg4= pDispParams->rgvarg[1].intVal;
// 				int iArg5= pDispParams->rgvarg[0].intVal;
// 				//pDlg->ShowModalDlg(cszArg1, CRect(iArg2, iArg3, iArg4, iArg5));
// 			}
// 		}

	if (dispIdMember == DisPid_CB_InvokeCallByJsJudgeClient)
	{
		if(wFlags & DISPATCH_PROPERTYGET)
		{
			if(pVarResult != NULL)
			{
				VariantInit(pVarResult);
				V_VT(pVarResult) = VT_BOOL;
				V_BOOL(pVarResult) = true;
			}
		}

		if(wFlags & DISPATCH_METHOD)
		{
			pDlg->InvokeCallByJsJudgeClient();
		}
	}

	if (dispIdMember == DisPid_CB_InvokeCallWebClient)
	{
		if(wFlags & DISPATCH_PROPERTYGET)
		{
			if(pVarResult != NULL)
			{
				VariantInit(pVarResult);
				V_VT(pVarResult) = VT_BOOL;
				V_BOOL(pVarResult) = true;
			}
		}

		if(wFlags & DISPATCH_METHOD)
		{
			CString	strArg1	= pDispParams->rgvarg[0].bstrVal;
			pDlg->InvokeCallWebClient(strArg1);
		}
	}
	
	return S_OK;
}

