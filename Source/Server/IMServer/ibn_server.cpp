// ofs_server.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "ibn_server.h"
#include <initguid.h>
#include "adoutil.h"

#include "eventlog.h"
#include "counter.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#ifdef _DEBUG
CDebugReportHook g_ReportHook;
#endif

//Global variable
CExeModule			g_Module;
CMyWinApp			g_App;
CSupportClass		g_SupportClass;
CActiveSessions		g_ActiveSessions;
COfs_serverApp		g_Extension;

BEGIN_OBJECT_MAP(ObjectMap)
	//OBJECT_ENTRY(CLSID_MCmWeb,CMCmWeb)
END_OBJECT_MAP()

//Static variable
static COfs_serverApp* g_pServer = NULL;

static BOOL IsFirstRequest = TRUE;

COfs_serverApp::COfs_serverApp()
{
	m_StopOleEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	ASSERT(g_pServer == NULL); // only one server instance
	g_pServer = this;

#ifdef _IBN_PERFORMANCE_MONITOR
	StartPerf();
#endif

	InitializeCriticalSection(&criticalSection);
}

COfs_serverApp::~COfs_serverApp()
{
	if(m_StopOleEvent)
		CloseHandle(m_StopOleEvent);
	g_pServer = NULL;

	DeleteCriticalSection(&criticalSection);
}

/*************************************************************
/*	The First entry point
/*	GetExtensionVersion
/**************************************************************/

BOOL COfs_serverApp::GetExtensionVersion(HSE_VERSION_INFO* pVer)
{
	//CEventLog::AddAppLog("GetExtensionVersion");
	try
	{
		HRESULT hr = CXMLUtil::SetFreeThreadedClassFactory();
		if(FAILED(hr))
			goto CleanUp;

		//LoadConnectionString();

		//if(!CheckDirectory()) goto CleanUp;

		// Move to Initialize Method
		// Check SQL Connection and SQL Server version

		// Load Connection String
		TCHAR moduleFileName[2048] = _T("");
		GetModuleFileName(g_Module.m_hInst, moduleFileName, 2048);

		CString strModuleFileName =  moduleFileName;
		strModuleFileName = strModuleFileName.Mid(0, strModuleFileName.ReverseFind(_T('\\'))+1);
		strModuleFileName.Replace(_T("\\\\?\\"), _T(""));

		// Initialize Connection String
		BOOL bCheckConfigFile = ConfigFileExists(strModuleFileName);

		if(bCheckConfigFile)
		{
			LoadConnectionStringFromConfigFile(strModuleFileName);
		}
		else 
		{
			strModuleFileName = _T("Registry");
			LoadConnectionStringFromRegistry();
		}

		// Check Connection String
		if(S_OK!=CheckConnectionString(strModuleFileName))
			goto CleanUp;

		//Sleep(10000);
		
		// OZ 2008-07-30 Remember Current process, solve iis recycle problem
		IbnPipeManger::StopExternalActivity(CADOUtil::bGlobalCompany, CADOUtil::szCompanyId);

		//CADOUtil::RegisterCurrentProcess();

		//if(S_OK != CheckConnectionString())
		//	goto CleanUp;

		if(!g_SupportClass.Initialize(dwDelSessionsCallBackID))
			goto CleanUp;

		if(S_OK != g_ActiveSessions.Initialize(dwDelSessionsCallBackID))
			goto CleanUp;

		if(!m_pISAPIExt.Initialize())
			goto CleanUp;

		//if(S_OK != StartOLE()) goto CleanUp;

#ifdef _IBN_LICENSE_CHECKER
		int ActiveUsers = LicenseCheckActiveUsers();
#endif

		// Move to Initialize Method
		// 2005/06/16 Initialize HasRealTimeMonitoring
/*#ifdef _IBN_PERFORMANCE_MONITOR
		HRESULT hr = StartPerf();
		if(FAILED(hr))
		{
			CCounter::SetHasRealTimeMonitoring(FALSE);
			CEventLog::AddAppLog(NULL, MSG_CANNOT_START_PERFMON, EVENTLOG_WARNING_TYPE, &hr, (DWORD)sizeof(HRESULT));
		}
#endif

		if(S_OK != m_PipeManager.Start())
			goto CleanUp;*/

		pVer->dwExtensionVersion = MAKELONG( HSE_VERSION_MINOR, HSE_VERSION_MAJOR );

#ifndef MAGRUL_RELEASE
		strncpy_s(pVer->lpszExtensionDesc, HSE_MAX_EXT_DLL_NAME_LEN, "Mediachase IBN Server", HSE_MAX_EXT_DLL_NAME_LEN);
#else
		strncpy_s(pVer->lpszExtensionDesc, HSE_MAX_EXT_DLL_NAME_LEN, "Radius-Soft MagRul Server", HSE_MAX_EXT_DLL_NAME_LEN);
#endif
		CEventLog::AddAppLog(NULL);

		return TRUE;
	}
	catch(...)
	{
	}

	//========================================

CleanUp:
	try
	{
		// Move to Initialize Method
		//StopPerf();

		//StopOLE();
		m_pISAPIExt.Terminate();
		g_ActiveSessions.Terminate();
		g_SupportClass.Terminate();

		// Move to Initialize Method
		//m_PipeManager.Stop();
		CXMLUtil::m_pClassFactory.Release();
	}
	catch(...)
	{
	}

	return FALSE;
}

/*************************************************************
/*	The Second entry point
/*	HttpExtensionProc
/**************************************************************/

DWORD COfs_serverApp::HttpExtensionProc(LPEXTENSION_CONTROL_BLOCK pECB)
{
	RevertToSelf();

	HRESULT hr;

	if(IsFirstRequest)
	{
		EnterCriticalSection(&criticalSection);

		if(IsFirstRequest)
		{
			//CEventLog::AddAppLog("HttpExtensionProc First Run");

/*			TCHAR moduleFileName[2048] = _T("");
			GetModuleFileName(g_Module.m_hInst, moduleFileName, MAX_PATH*sizeof(2048));

			CString strModuleFileName =  moduleFileName;
			strModuleFileName = strModuleFileName.Mid(0, strModuleFileName.ReverseFind(_T('\\'))+1);
			strModuleFileName.Replace(_T("\\\\?\\"), _T(""));

			// Read Host
			//CHAR httpHost[2048];
			//DWORD httpHostLen = 2048;
			//pECB->GetServerVariable(pECB->ConnID, "HTTP_HOST", httpHost, &httpHostLen);

			// Read APP ID
			//CHAR appPath[2048];
			//DWORD appPathLen = 2048;
			//pECB->GetServerVariable(pECB->ConnID, "APPL_PHYSICAL_PATH", appPath, &appPathLen);

			// Initialize Connection String
			BOOL bCheckConfigFile = ConfigFileExists(strModuleFileName);

			if(bCheckConfigFile)
			{
				LoadConnectionStringFromConfigFile(strModuleFileName);
			}
			else 
			{
				LoadConnectionStringFromRegistry();
			}

			// Check Connection String
			CheckConnectionString(strModuleFileName);
			*/

			// Init Performance monitor
			#ifdef _IBN_PERFORMANCE_MONITOR
			hr = StartPerf();
			if(FAILED(hr))
			{
				CCounter::SetHasRealTimeMonitoring(FALSE);
				CEventLog::AddAppLog(NULL, MSG_CANNOT_START_PERFMON, EVENTLOG_WARNING_TYPE, &hr, (DWORD)sizeof(HRESULT));
			}
			#endif

			// Step 2. Init Command Pipe
			hr = m_PipeManager.Start(CADOUtil::bGlobalCompany, CADOUtil::szCompanyId);
			if(FAILED(hr))
				CEventLog::AddAppLog(_T("Create Named Pipe Error"), UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));

			// Step 3. Clear SQL Active Session
			hr = InitializeActiveUsers();

			IsFirstRequest = FALSE;
		}

		LeaveCriticalSection(&criticalSection);
	}

	DWORD retVal = HSE_STATUS_ERROR;

	try
	{
		if(m_pISAPIExt.AddRequest(pECB))
			retVal = HSE_STATUS_PENDING;
	}
	catch(...)
	{
	}

	return retVal;
}

/*HRESULT COfs_serverApp::Initialize()
{
	HRESULT hr = S_OK;

	// Step 1. Init Performance monitor
#ifdef _IBN_PERFORMANCE_MONITOR
	hr = StartPerf();
	if(FAILED(hr))
	{
		CCounter::SetHasRealTimeMonitoring(FALSE);
		CEventLog::AddAppLog(NULL, MSG_CANNOT_START_PERFMON, EVENTLOG_WARNING_TYPE, &hr, (DWORD)sizeof(HRESULT));
	}
#endif

	// Step 2. Init Command Pipe
	hr = m_PipeManager.Start();

	if(FAILED(hr))
		CEventLog::AddAppLog("Create Named Pipe Error", UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));

	// Step 3. Clear SQL Active Session
	hr = InitializeActiveUsers();

	return hr;
}*/

//*************************************************************
//
//
//*************************************************************
BOOL COfs_serverApp::TerminateExtension(DWORD dwFlags)
{
	//CEventLog::AddAppLog("TerminateExtension", SERVER_STOPPED);
#ifdef _IBN_PERFORMANCE_MONITOR
	//StopPerf();
#endif

	//StopOLE();
	m_PipeManager.Stop();

	// Clear Active sessions
	//ClearSqlActiveUsers();

	g_ActiveSessions.Terminate(3000);

	g_SupportClass.Terminate();
	m_pISAPIExt.Terminate();

	CXMLUtil::m_pClassFactory.Release();

	CEventLog::AddAppLog(NULL, SERVER_STOPPED);
	dwFlags = 1;
	return true;
}

//************************************************************
//
//
//************************************************************
extern long GetGMTtime_t();

HRESULT COfs_serverApp::CheckConnectionString(const TCHAR* configFile)
{
	HRESULT hr = S_OK;

	try
	{
		_RecordsetPtr RecSet;

		CADOUtil::RunSP_ReturnRS(_bstr_t("ProductVersionGet"), RecSet, NULL);

		if(CADOUtil::CheckRecordSetState(RecSet))
		{
			// Read version
			FieldPtr field = RecSet->Fields->GetItem(0L);
			bstr_t version = field->Value.bstrVal;

			RecSet->Close();

			// Parce version
			WCHAR* point1 = wcschr((BSTR)version,'.');
			WCHAR* point2 = wcschr(point1+1,'.');
			//WCHAR* point3 = wcschr(point2+1,'.');

			int majorVersion = 0;
			int buildId = 0;

			if((BSTR)version!=NULL)
				majorVersion = _wtol((BSTR)version);
			if(point2!=NULL)
				buildId = _wtol(point2+1);
				
			// Check MS SQL version = "9.00.3068.00" should be great or equal to 8.00.760.00
			if(!(majorVersion>8 || majorVersion==8 && buildId>=760))
			{
				CEventLog::AddAppLog(
					bstr_t(L"Current MS SQL database version is not supported, should be larger or equal to 8.00.760.00. The current version is ") 
					+ version 
					+ bstr_t(". Config File: ") + bstr_t(configFile), 
					UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
				hr = E_FAIL;
			}
		}
		else 
			throw (int)656; // CheckRecordSetState Problem
	}
	catch(_com_error err)
	{
		hr = err.Error();
		CEventLog::AddAppLog(bstr_t(L"SQL Error: ") + err.Description() + bstr_t(". Config File: ") + bstr_t(configFile), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}
	catch(...)
	{
		hr = E_FAIL;
		CEventLog::AddAppLog(bstr_t(L"SQL Unknown Error") + bstr_t(". Config File: ") + bstr_t(configFile), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}

	return hr;
}

HRESULT COfs_serverApp::InitializeActiveUsers()
{
	HRESULT hr = S_OK;

	try
	{
		CParamArray m_ParamArray;
		LONG ltime = GetGMTtime_t(); 

		m_ParamArray.AddLong(ltime);
		CADOUtil::RunSP_ReturnLong(_bstr_t("OM_KILL_ALL"),&m_ParamArray);
	}
	catch(_com_error err)
	{
		hr = err.Error();
		CEventLog::AddAppLog(bstr_t(L"SQL Error: ") + err.Description(), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}
	catch(...)
	{
		hr = E_FAIL;
		CEventLog::AddAppLog(_T("SQL Unknown Error"), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}

	return hr;
}


HRESULT COfs_serverApp::ClearSqlActiveUsers()
{
	HRESULT hr = S_OK;

	/*try
	{
		CParamArray m_ParamArray;
		LONG ltime = GetGMTtime_t(); 

		m_ParamArray.AddLong(ltime);
		CADOUtil::RunSP_ReturnLong(_bstr_t("OM_CLOSE_ALL"),&m_ParamArray);
	}
	catch(_com_error err)
	{
		hr = err.Error();
		CEventLog::AddAppLog(bstr_t(L"SQL Error: ") + err.Description(), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}
	catch(...)
	{
		hr = E_FAIL;
		CEventLog::AddAppLog(_T("SQL Unknown Error"), UNABLE_TO_START, EVENTLOG_ERROR_TYPE);
	}*/

	return hr;
}

BOOL COfs_serverApp::ConfigFileExists(const CHAR* appPath)
{
	// Get Config File
	CString configFileName = appPath;
	configFileName += _T("..\\Portal\\web.config");

	// Try Open
	HANDLE hFile = CreateFile(configFileName,GENERIC_READ,FILE_SHARE_READ|FILE_SHARE_WRITE,NULL,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,NULL);

	if(hFile!= INVALID_HANDLE_VALUE)
	{
		CloseHandle(hFile);
		return TRUE;
	}

	return FALSE;
}


void COfs_serverApp::LoadConnectionStringFromConfigFile(const CHAR* appPath)
{
	// Initialze variables
	CADOUtil::szConnectionString[0] = _T('\0');
	CADOUtil::szCompanyId[0] = _T('\0');
	CADOUtil::bGlobalCompany = FALSE;

	// Get Config File Path
	CString configFileName = appPath;
	configFileName += _T("..\\Portal\\web.config");
	//configFileName += _T("..\\Root\\web.config");

	// Try Open
	CComPtr<IXMLDOMDocument> webConfig;

	HRESULT hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL, IID_IXMLDOMDocument, (void**)&webConfig);

	if(hr==S_OK)
	{
		VARIANT_BOOL isSuccessful = VARIANT_FALSE;
		hr = webConfig->load(CComVariant(configFileName), &isSuccessful);

		if(isSuccessful == VARIANT_TRUE)
		{
			//CComBSTR bsXml;
			//webConfig->get_xml(&bsXml);

			// Read Connection Srting
			CComPtr<IXMLDOMNode> xmlConnectionStringAttr;
			hr = webConfig->selectSingleNode(CComBSTR(L"configuration/appSettings/add[@key='ConnectionString']/@value"), 
				&xmlConnectionStringAttr);

			// Copy Connection String
			if(xmlConnectionStringAttr!=NULL)
			{
				CComVariant value;
				hr = xmlConnectionStringAttr->get_nodeValue(&value);

				CComBSTR bsConnectionString = L"Provider=SQLOLEDB.1;";
				bsConnectionString += value.bstrVal;

				_tcscpy_s(CADOUtil::szConnectionString, 500, CW2CT(bsConnectionString));
			}
			else
			{
				CEventLog::AddAppLog(_T("Load ConnectionString Error. File ") + configFileName, UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));
			}

			// Read Company UID
			CComPtr<IXMLDOMNode> xmlCompanyUidAttr;
			hr = webConfig->selectSingleNode(CComBSTR(L"configuration/appSettings/add[@key='CompanyUid']/@value"), 
				&xmlCompanyUidAttr);

			// Copy Company UID
			if(xmlCompanyUidAttr!=NULL)
			{
				CComVariant value;
				hr = xmlCompanyUidAttr->get_nodeValue(&value);

				_tcscpy_s(CADOUtil::szCompanyId, 500, CW2CT(value.bstrVal));
			}
			else
			{
				CEventLog::AddAppLog(_T("Load CompanyUid Error. File ") + configFileName, UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));
			}

		}
		else
		{
			CEventLog::AddAppLog(_T("Open File Error. File ") + configFileName, UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));
		}
	}
	else
	{
		CEventLog::AddAppLog(_T("XMLDOMDocument::CreateInstance Error."), UNABLE_TO_START, EVENTLOG_ERROR_TYPE, &hr, (DWORD)sizeof(HRESULT));
	}

}

void COfs_serverApp::LoadConnectionStringFromRegistry()
{
	//TCHAR reg_FilePath[500];
	TCHAR LinkConnectionString[500] = _T("");
	TCHAR reg_SQLServer[256] = _T("");
	TCHAR reg_SQLPassword[256] = _T("");
	TCHAR reg_SQLDBName[256] = _T("");

	HKEY hkey= NULL;
	DWORD datasize;

#ifndef MAGRUL_RELEASE

#ifdef _MCWIN64
	LPCTSTR sIbnServerInstallPath = _T("SOFTWARE\\Wow6432Node\\Mediachase\\Instant Business Network\\4.7\\Server\\Install");
#endif
#ifdef _MCWIN32
	LPCTSTR sIbnServerInstallPath = _T("SOFTWARE\\Mediachase\\Instant Business Network\\4.7\\Server\\Install");
#endif

#else

#ifdef _MCWIN64
	LPCTSTR sIbnServerInstallPath = _T("SOFTWARE\\Wow6432Node\\Radius-Soft\\MagRul\\4.7\\Server\\Install");
#endif
#ifdef _MCWIN32
	LPCTSTR sIbnServerInstallPath = _T("SOFTWARE\\Radius-Soft\\MagRul\\4.7\\Server\\Install");
#endif

#endif

	RegOpenKeyEx(HKEY_LOCAL_MACHINE, sIbnServerInstallPath, 0, KEY_READ, &hkey);

	//G_SQL_SERVER
	//G_SQL_DB_IBN
	//G_SQL_IBN_PASSWORD
	datasize= sizeof(reg_SQLServer);
	if(RegQueryValueEx(hkey, _T("MC_SQL_SERVER"), NULL, NULL, (LPBYTE)&reg_SQLServer, &datasize) != ERROR_SUCCESS)
		reg_SQLServer[0] = _T('\0');
	datasize= sizeof(reg_SQLDBName);
	if(RegQueryValueEx(hkey, _T("MC_SQL_DATABASE"), NULL, NULL, (LPBYTE)&reg_SQLDBName, &datasize) != ERROR_SUCCESS)
		reg_SQLDBName[0] = _T('\0');
	datasize= sizeof(reg_SQLPassword);
	if(RegQueryValueEx(hkey, _T("IBN_SQL_PASSWORD"), NULL, NULL, (LPBYTE)&reg_SQLPassword, &datasize) != ERROR_SUCCESS)
		reg_SQLPassword[0] = _T('\0');
	//datasize= sizeof(reg_FilePath);
	//if(RegQueryValueEx(hkey, _T("G_STORAGE_FOLDER"), NULL, NULL, (LPBYTE)&reg_FilePath, &datasize) != ERROR_SUCCESS)
	//	reg_FilePath[0] = _T('\0');

	RegCloseKey(hkey);

	_stprintf_s(LinkConnectionString, 500,
		_T("Provider=SQLOLEDB.1; User ID=\"IBN\"; Password=\"%s\"; Initial Catalog=%s; Data Source=%s"), 
		reg_SQLPassword, reg_SQLDBName, reg_SQLServer);

	//CEventLog::AddAppLog(LinkConnectionString,UNABLE_STARTED,EVENTLOG_ERROR_TYPE);

	_tcscpy_s(CADOUtil::szConnectionString, 500, LinkConnectionString);
	CADOUtil::szCompanyId[0] = _T('\0');
	CADOUtil::bGlobalCompany = TRUE;

	//if(_tcslen(reg_FilePath) != 0)
	//{
	// m_SupportClass.szLocalStoragePath = new TCHAR[_tcslen(reg_FilePath) + _tcslen(_T("%sinstmsg"))+10];
	//	_stprintf(m_SupportClass.szLocalStoragePath,_T("%sinstmsg"),reg_FilePath);
	//}
	//else
	//	m_SupportClass.szLocalStoragePath = NULL;
}


//******************************************************************
//
//	Extern functions
//
//******************************************************************
BOOL CMyWinApp::InitInstance()
{
	CWinApp::InitInstance();
	g_Module.Init(ObjectMap, m_hInstance);
	DisableThreadLibraryCalls(m_hInstance);

	return true;
};

int CMyWinApp::ExitInstance( )
{
	g_Module.Term();
	return CWinApp::ExitInstance();
};

extern "C" BOOL WINAPI GetExtensionVersion(HSE_VERSION_INFO *pVer)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return g_pServer->GetExtensionVersion(pVer);
}


extern "C" DWORD WINAPI HttpExtensionProc(LPEXTENSION_CONTROL_BLOCK pECB)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return g_pServer->HttpExtensionProc(pECB);
}

extern "C" BOOL WINAPI TerminateExtension(DWORD dwFlags)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return g_pServer->TerminateExtension(dwFlags);
}

STDAPI DllRegisterServer(void)
{
#ifdef UNICODE
	ATLASSERT(FALSE);
#endif
#ifdef _UNICODE
	ATLASSERT(FALSE);
#endif

	SetThreadLocale(MAKELCID(MAKELANGID(LANG_ENGLISH,SUBLANG_ENGLISH_US),SORT_DEFAULT));

	/*HRESULT hres = */CEventLog::Register();
	//return hres;

	//if (hres != S_OK)
	//	return hres;

	return g_Module.RegisterServer(TRUE);
}

/////////////////////////////////////////////////////////////////////////////
// DllUnregisterServer - Removes entries from the system registry

STDAPI DllUnregisterServer(void)
{
	/*HRESULT hres = */CEventLog::UnRegister();
	return g_Module.UnregisterServer(TRUE);
}


HRESULT COfs_serverApp::StartOLE()
{

	HANDLE hThread = (HANDLE)_beginthreadex(NULL, NULL, 
		(unsigned int (__stdcall *)(void *)) ThreadProc,
		(LPVOID)this, 
		0, 
		(unsigned int *) &m_dwThreadID);

	//HANDLE hThread = CreateThread(NULL,NULL,ThreadProc,(LPVOID)this,NULL,&m_dwThreadID);
	if(hThread != NULL)
	{
		CloseHandle(hThread);
		return S_OK;
	}
	return HRESULT_FROM_WIN32(GetLastError());
}

DWORD WINAPI COfs_serverApp::ThreadProc(LPVOID lpParam)
{
	COfs_serverApp* pClass = (COfs_serverApp*) lpParam;
	HRESULT hr;

	CoInitialize(NULL);
	hr = g_Module.Init(ObjectMap, g_App.m_hInstance);
	g_Module.dwThreadID = GetCurrentThreadId();


	//hr = _Module.RegisterClassObjects(CLSCTX_LOCAL_SERVER, REGCLS_MULTIPLEUSE);

	MSG msg;
	while (GetMessage(&msg, 0, 0, 0))
		DispatchMessage(&msg);

	//_Module.RevokeClassObjects();

	CoUninitialize();
	SetEvent(pClass->m_StopOleEvent);
	return 0;
}

void COfs_serverApp::StopOLE()
{
	PostThreadMessage(m_dwThreadID, WM_QUIT, 0, 0);
	WaitForSingleObject(m_StopOleEvent, 10000);
}

//HRESULT COfs_serverApp::StartPerf(void)
//{
//	for (int i=0; i<3; i++)
//		perfObject[i] = NULL;
//
//	HRESULT hr = perfObjMgr.Initialize();
//	if(SUCCEEDED(hr))
//	{
//		CPerfLock lock(&perfObjMgr);
//
//		hr = lock.GetStatus();
//		if (SUCCEEDED(hr))
//		{
//			hr = perfObjMgr.CreateInstanceByName<MCMperfMonitorSampleObject>(L"IM Server", &perfObject[0]);
//			if (SUCCEEDED(hr))
//			{
//				if(perfObject[0] != NULL)
//				{
//#ifdef _IBN_PERFORMANCE_MONITOR
//					//CCounter::m_ActiveUser = (LONG*)&(perfObject[0]->m_nCounter);
//					//if (CCounter::HasRealTimeMonitoring)
//					//{
//					CCounter::m_ulActiveUser =&perfObject[0]->m_ulActiveUser;
//					CCounter::m_ulCrRequest =&perfObject[0]->m_ulCrRequest;
//					CCounter::m_ulCrCommand =&perfObject[0]->m_ulCrCommand;
//					CCounter::m_ulCrRcvFile =&perfObject[0]->m_ulCrRcvFile;
//					CCounter::m_ulCrSendFile =&perfObject[0]->m_ulCrSendFile;
//					CCounter::m_ulCrAlive =&perfObject[0]->m_ulCrAlive;
//					CCounter::m_ulMaxRequest =&perfObject[0]->m_ulMaxRequest;
//					CCounter::m_ulMaxCommand =&perfObject[0]->m_ulMaxCommand;
//					CCounter::m_ulMaxRcvFile =&perfObject[0]->m_ulMaxRcvFile;
//					CCounter::m_ulMaxSendFile =&perfObject[0]->m_ulMaxSendFile;
//					CCounter::m_ulMaxAlive =&perfObject[0]->m_ulMaxAlive;
//					CCounter::m_ullAvrSQL =&perfObject[0]->m_ullAvrSQL;
//					CCounter::m_ulAvrSQLBase =&perfObject[0]->m_ulAvrSQLBase;
//					CCounter::m_ullAvrCommand =&perfObject[0]->m_ullAvrCommand;
//					CCounter::m_ulAvrCommandBase =&perfObject[0]->m_ulAvrCommandBase;
//					CCounter::m_ullAvrAlive =&perfObject[0]->m_ullAvrAlive;
//					CCounter::m_ulAvrAliveBase =&perfObject[0]->m_ulAvrAliveBase;
//					CCounter::m_ullAvrSendFile =&perfObject[0]->m_ullAvrSendFile;
//					CCounter::m_ulAvrSendFileBase =&perfObject[0]->m_ulAvrSendFileBase;
//					CCounter::m_ullAvrRcvFile =&perfObject[0]->m_ullAvrRcvFile;
//					CCounter::m_ulAvrRcvFileBase =&perfObject[0]->m_ulAvrRcvFileBase;
//					CCounter::m_ullAvrRequest =&perfObject[0]->m_ullAvrRequest;
//					CCounter::m_ulAvrRequestBase =&perfObject[0]->m_ulAvrRequestBase;
//					//}
//#endif
//				}
//			}
//		}
//	}
//	return hr;
//}
//
//HRESULT COfs_serverApp::StopPerf(void)
//{
//	perfObjMgr.UnInitialize();
//	return S_OK;
//}
