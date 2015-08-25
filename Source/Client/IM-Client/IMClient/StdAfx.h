// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//

#if !defined(AFX_STDAFX_H__40F856E0_C9B5_420D_B297_D2ED1D9A50DD__INCLUDED_)
#define AFX_STDAFX_H__40F856E0_C9B5_420D_B297_D2ED1D9A50DD__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define VC_EXTRALEAN		// Exclude rarely-used stuff from Windows headers
#ifdef _DEBUG
#define _DEVELOVER_VERSION_L1
#endif

#ifndef WINVER
#define WINVER 0x0500
#endif

#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0500
#endif 

// Default  Section [5/20/2002]
#include <afxwin.h>         // MFC core and standard components
#include <afxext.h>         // MFC extensions
#include <afxdisp.h>        // MFC Automation classes
#include <afxdtctl.h>		// MFC support for Internet Explorer 4 Common Controls
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>			// MFC support for Windows Common Controls
#include <afxctl.h>
#include <afxpriv.h>
#include <afxtempl.h>
#include <afxmt.h>
#include <wininet.h>
#include <atlbase.h>
#include <afxcmn.h>

//#define		IBN_SCHEMA		_T("mpa://")
#define		IBN_SCHEMA		_T("ibn45://")

// Import Section [5/20/2002]
#import "ATL_NetLib.dll" no_namespace named_guids
#import "..\Components\MpaWebControl.dll" no_namespace named_guids
#import "..\Components\COMHIST.dll" no_namespace named_guids
#import "..\Components\MCFormSender.dll"  no_namespace named_guids

#import "McScreenCapture.tlb"  no_namespace named_guids

// Global Section
#include "GlobalMessengerDef.h"
#include "GlobalFunction.h"
#include "SupportXMLFunction.h"
#include "OfsConst.h"

// Log Addons  [9/30/2002]
#ifdef _DEVELOVER_VERSION_WITH_LOG_L1
#define _DOLOG
//#pragma comment (lib,"lib/logEX.lib")
#endif //_DEVELOVER_VERSION_WITH_LOG_L1

//#include "LogEX.h"
// Log Addons End [9/30/2002]

extern		OSVERSIONINFO	_VersionInfo;

#define		IsWinXPOrLate()		((_VersionInfo.dwMajorVersion>5)||(_VersionInfo.dwMajorVersion==5&&_VersionInfo.dwMinorVersion>=1))

#define		IsWin2K()			(_VersionInfo.dwMajorVersion==5&&_VersionInfo.dwMinorVersion==0)

#endif // _AFX_NO_AFXCMN_SUPPORT

#ifdef _UNICODE
#define _tstol _wtol
#else
#define _tstol atol
#endif

#define SAFERELEASE(x) if((x)!=NULL) { (x)->Release(); (x) = NULL; }
#define SAFEDELETE(x) if((x)!=NULL) { delete (x); (x) = NULL; }

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STDAFX_H__40F856E0_C9B5_420D_B297_D2ED1D9A50DD__INCLUDED_)
