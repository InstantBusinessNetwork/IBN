// ibn_server.h : main header file for the OFS_SERVER DLL


#if !defined(AFX_OFS_SERVER_H__D9E9B180_7746_4DE6_ABA9_4F1B4D8AD7F9__INCLUDED_)
#define AFX_OFS_SERVER_H__D9E9B180_7746_4DE6_ABA9_4F1B4D8AD7F9__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef __AFXWIN_H__
#error include 'stdafx.h' before including this file for PCH
#endif

//#define SERVER_STARTED (DWORD)600
//#define UNABLE_STARTED (DWORD)601

#include "resource.h"
#include "SupportClass.h"
//#include "CoolNetQueue.h"
#include "ActiveSessions.h"
//#include "MCMperfMonitor.h"
#include "ISAPIExt.h"

#include "IbnPipeManger.h"

/*************************************************************************
/*			COfs_serverApp
/*
/************************************************************************/
//#define LogTemplate _T("Mediachase IM Server unable start \r\n Error description = \"%s\"")

class COfs_serverApp
{
public:
	COfs_serverApp();
	~COfs_serverApp();

public:
	BOOL TerminateExtension(DWORD dwFlags);
	BOOL GetExtensionVersion(HSE_VERSION_INFO* pVer);
	DWORD HttpExtensionProc(LPEXTENSION_CONTROL_BLOCK pECB);


	DWORD dwDelSessionsCallBackID;
private:
	IbnPipeManger m_PipeManager;

	CRITICAL_SECTION criticalSection;
	//Ole thread
	static DWORD WINAPI ThreadProc(LPVOID lpParam);
	DWORD m_dwThreadID;
	//MCMperfMonitor perfObjMgr;
	//MCMperfMonitorSampleObject* perfObject[3];

	
	HRESULT CheckConnectionString(const TCHAR* configFile);
	HRESULT InitializeActiveUsers();
	HRESULT ClearSqlActiveUsers();

	BOOL ConfigFileExists(const CHAR* appPath);

	void LoadConnectionStringFromRegistry();
	void LoadConnectionStringFromConfigFile(const CHAR* appPath);

	void StopOLE();
	HRESULT StartOLE();

	//HRESULT StartPerf(void);

	HANDLE m_StopOleEvent;

	CISAPIExt m_pISAPIExt;
public:

	//HRESULT StopPerf(void);
};

#endif // !defined(AFX_OFS_SERVER_H__D9E9B180_7746_4DE6_ABA9_4F1B4D8AD7F9__INCLUDED_)
