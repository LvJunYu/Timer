// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if !defined(AFX_STDAFX_H__F3F52147_60F7_4425_AD68_4E553E93764F__INCLUDED_)
#define AFX_STDAFX_H__F3F52147_60F7_4425_AD68_4E553E93764F__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define VC_EXTRALEAN		// Exclude rarely-used stuff from Windows headers

#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#include <afxdisp.h>        // MFC Automation classes
#include <afxdtctl.h>		// MFC support for Internet Explorer 4 Common Controls
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>			// MFC support for Windows Common Controls
#endif // _AFX_NO_AFXCMN_SUPPORT

#ifndef ULONG_PTR
#define ULONG_PTR unsigned long*
#endif
#include "GDI+\\gdiplus.h"   ////请修改为你的头文件路径
using namespace Gdiplus;  
#pragma comment(lib, "GDI+\\gdiplus.lib") ////请修改为你的.lib文件路径

#define _WTL_NO_AUTOMATIC_NAMESPACE
#define  _WTL_USE_CSTRING
#define _CSTRING_NS

#include <ATLBASE.H>
//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STDAFX_H__F3F52147_60F7_4425_AD68_4E553E93764F__INCLUDED_)
