// stdafx.h : include file for standard system include files,
//  or project specific include files that are used frequently, but
//      are changed infrequently
//
#if !defined(AFX_STDAFX_H__2D9A657C_4B2E_4A5A_95C8_96CC81D545F3__INCLUDED_)
#define AFX_STDAFX_H__2D9A657C_4B2E_4A5A_95C8_96CC81D545F3__INCLUDED_

#define _ATL_PERF_REGISTER 

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

//#ifndef _WIN32_WINNT
//#define _WIN32_WINNT 0x0500
//#endif
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0500
#endif

//#define UNICODE
//#define _UNICODE
#ifdef _DEBUG
//#define new DEBUG_CLIENTBLOCK
#endif

#define _ATL_APARTMENT_THREADED
#define ERR_OUT_ALREADY_IN              (LONG)100
#define ERR_OUT_WRONG_PASSWORD          (LONG)101
#define ERR_OUT_WRONG_SID               (LONG)102
#define ERR_OUT_WRONG_NAME              (LONG)103
#define ERR_OUT_WRONG_REQUEST           (LONG)104
#define ERR_UNKNOW                      (LONG)105
//#define ERR_OUT_GLOBAL                  (LONG)106
#define ERR_WRONG_XML                   (LONG)111
#define ERR_WRONG_SID                   (LONG)112
#define ERR_WRONG_ID                    (LONG)113
#define ERR_WRONG_PASSWORD              (LONG)114
#define ERR_UNABLE_SEND                 (LONG)117
#define ERR_UNNKOWN_XML                 (LONG)118
#define ERR_UNABLE_READ                 (LONG)119
#define ERR_ALREADY_SENT                (LONG)120
#define ERR_NOT_RECIPIENTS              (LONG)121
#define ERR_OLD_PROTOCOL                (LONG)122
#define ERR_LICENSE_LIMIT               (LONG)130
#define ERR_OUT_GLOBAL_1                (LONG)141
#define ERR_OUT_GLOBAL_2                (LONG)142
#define ERR_OUT_GLOBAL_3                (LONG)143
#define ERR_OUT_GLOBAL_4                (LONG)144
#define ERR_OUT_GLOBAL_5                (LONG)145
#define ERR_OUT_GLOBAL_6                (LONG)146

#include <limits.h>

#include <afx.h> 
#include <afxwin.h>
#include <afxmt.h>		// for synchronization objects
#include <afxext.h>
#include <HttpExt.h>
#include <afxdisp.h> 
#include <initguid.h>
#include <afxtempl.h>

#include <atlbase.h>

//You may derive a class from CComModule and use it if you want to override
//something, but do not change the name of _Module
class CExeModule : public CComModule
{
public:
	CExeModule()
	{
	/*	dwBegin = GetProcessHeaps(300,Begin);
		HANDLE h = CreateFile(_T("c:\\BeginMem.txt"),
					GENERIC_WRITE,
					NULL,
					NULL,
					CREATE_ALWAYS,
					FILE_ATTRIBUTE_NORMAL,
					NULL);

		for(int k = GetProcessHeaps(300,Begin); k!= 0;k--)
		{
			DWORD dwOut;
			WriteFile(h,"\r\n\r\nheap \r\n\r\n",11,&dwOut,NULL);
			PROCESS_HEAP_ENTRY pEntry;
			pEntry.lpData = NULL;
			while(HeapWalk(Begin[k-1],&pEntry))
			{
				WriteFile(h,"\r\n\r\nbegin\r\n\r\n",11,&dwOut,NULL);
				WriteFile(h,pEntry.lpData,pEntry.cbData,&dwOut,NULL);
			};
		}
		CloseHandle(h);*/
/*		_CrtMemCheckpoint( &s1 );

#ifdef _DEBUG
		int tmpFlag = _CrtSetDbgFlag( _CRTDBG_REPORT_FLAG );

		// Turn on leak-checking bit
		tmpFlag |= _CRTDBG_LEAK_CHECK_DF;

		// Turn off CRT block checking bit
		tmpFlag |= _CRTDBG_CHECK_CRT_DF;
		tmpFlag |= _CRTDBG_CHECK_ALWAYS_DF;

		// Set flag to the new value
		_CrtSetDbgFlag( tmpFlag );
#endif*/
	}
	~CExeModule()
	{
		/*dwBegin = GetProcessHeaps(300,Begin);
		HANDLE h = CreateFile(_T("c:\\endMem.txt"),
					GENERIC_WRITE,
					NULL,
					NULL,
					CREATE_ALWAYS,
					FILE_ATTRIBUTE_NORMAL,
					NULL);
		
		for(int k = GetProcessHeaps(300,Begin); k!= 0;k--)
		{
			DWORD dwOut;
			WriteFile(h,"\r\n\r\nheap \r\n\r\n",11,&dwOut,NULL);
			PROCESS_HEAP_ENTRY pEntry;
			pEntry.lpData = NULL;
			while(HeapWalk(Begin[k-1],&pEntry))
			{
				WriteFile(h,"\r\n\r\nbegin\r\n\r\n",11,&dwOut,NULL);
				WriteFile(h,pEntry.lpData,pEntry.cbData,&dwOut,NULL);
			};
		}
		CloseHandle(h);*/

/*		_CrtMemCheckpoint( &s2 );

		if ( _CrtMemDifference( &s3, &s1, &s2) ) 
			_CrtMemDumpStatistics( &s3 );

#ifdef _DEBUG
		_CrtDumpMemoryLeaks();
		_CrtMemDumpAllObjectsSince(NULL);
#endif*/
	}
public:

	//_CrtMemState s1, s2, s3;

	//DWORD dwBegin,dwEnd;
	//HANDLE Begin[300];
	//HANDLE End[300];
//	LONG Unlock();
	DWORD dwThreadID;
//	HANDLE hEventShutdown;
//	void MonitorShutdown();
//	bool StartMonitor();
//	bool bActivity;
};
extern CExeModule g_Module;

class CMyWinApp : public CWinApp
{
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance( );
};

#include <atlcom.h>
#include <initguid.h> 
#include <msxml2.h>


//#include "ExchangeData.h"
//#include <atlperf.h>
#include <windows.h>
//#import "msxml.dll" named_guids 
#include "atlutil.h"

#define WM_UPDATE_USER					(WM_USER + 250)
#define WM_UPDATE_GROUP					(WM_USER + 251)
#define WM_UPDATE_STUBS					(WM_USER + 252)
#define WM_UPDATE_ALERT					(WM_USER + 253)

#define WM_UPDATE_NET_ALERT				(WM_USER + 254)
#define WM_UPDATE_NET_MESSAGE			(WM_USER + 255)
#define WM_LOGOFF_NET_MESSAGE			(WM_USER + 256)

#define WM_UPDATE_USERSTUBS				(WM_USER + 257)

#define WM_STOP_ACTIVITY				(WM_USER + 258)

#define WM_CHANGE_STATUS				(WM_USER + 259)

/******************************************************************************/
#ifndef _DEBUG
#define _IBN_LICENSE_CHECKER // Закоментарить чтобы отключить проверку лицензии
#endif
/******************************************************************************/

/******************************************************************************/
//#define _IBN_PERFORMANCE_MONITOR // Закоментарить чтобы отключить performance monitor
/******************************************************************************/

#ifndef HSE_REQ_SET_FLUSH_FLAG
#define HSE_REQ_SET_FLUSH_FLAG (HSE_REQ_END_RESERVED+43)
#endif


#ifdef _MCWIN64
#import "C:\Program Files\Common Files\System\ado\msado15.dll" no_namespace rename("EOF", "EndOfFile")
#endif
#ifdef _MCWIN32
#import "C:\Program Files (x86)\Common Files\System\ado\msado15.dll" no_namespace rename("EOF", "EndOfFile")
#endif

#include "DBFile.h"

#endif // !defined(AFX_STDAFX_H__2D9A657C_4B2E_4A5A_95C8_96CC81D545F3__INCLUDED_)
