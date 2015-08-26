// stdafx.h : include file for standard system include files,
//      or project specific include files that are used frequently,
//      but are changed infrequently

#if !defined(AFX_STDAFX_H__706A7E47_BA9F_4A04_A034_7AD6C453AE4D__INCLUDED_)
#define AFX_STDAFX_H__706A7E47_BA9F_4A04_A034_7AD6C453AE4D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#define STRICT
//#ifndef _WIN32_WINNT
//#define _WIN32_WINNT 0x0400
//#endif
#define _ATL_APARTMENT_THREADED
#define INCL_WINSOCK_API_PROTOTYPES 1

//#include <Winsock2.h>
#include <msxml2.h>
#include <atlbase.h>
//You may derive a class from CComModule and use it if you want to override
//something, but do not change the name of _Module
extern CComModule _Module;

#include <math.h>

#ifdef _DEBUG
//#define _NOTIMEOUT
#endif
//#define _DOLOG
#include <list>
using namespace std;
#include <atlcom.h>
#include "defConst.h"
#include "structs.h"
#include "LogEX.h"
//#include "Mswsock.h"


#define OBJECT_CREATE_DELETE  2
//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_STDAFX_H__706A7E47_BA9F_4A04_A034_7AD6C453AE4D__INCLUDED)
