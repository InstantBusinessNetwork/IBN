#include "stdafx.h"
#include "eventlog.h"

CEventLog::CEventLog(void)
{
}

CEventLog::~CEventLog(void)
{
}

HRESULT CEventLog::Register(void)
{
	CRegKey pRegEventLog,pRegEventApp;
	CString strPath,str;
	HRESULT hr;

	DWORD dwFLen = GetModuleFileName(g_Module.m_hInst, CStrBuf(str, MAX_PATH), MAX_PATH);
	if( dwFLen == 0 )
		return E_FAIL;

#ifndef MAGRUL_RELEASE
	const TCHAR * const c_szAtlPerfServicesKey = _T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\IBN 4.7 Server");
#else
	const TCHAR * const c_szAtlPerfServicesKey = _T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\MagRul 4.7 Server");
#endif

	hr = pRegEventLog.Create(HKEY_LOCAL_MACHINE,c_szAtlPerfServicesKey);
	if(hr!=ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(hr);

	hr = pRegEventApp.Create(pRegEventLog,_T("InstMsg Server 4.7"));
	if(hr!=ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(hr);

	hr = pRegEventApp.SetStringValue(_T("EventMessageFile"),str);
	if(hr!=ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(hr);

	hr = pRegEventApp.SetDWORDValue(_T("TypesSupported"),
		(DWORD)EVENTLOG_ERROR_TYPE | EVENTLOG_WARNING_TYPE |EVENTLOG_INFORMATION_TYPE);
	if(hr!=ERROR_SUCCESS)
		return HRESULT_FROM_WIN32(hr);

	pRegEventApp.Close();
	pRegEventLog.Close();

	return hr;
}

HRESULT CEventLog::UnRegister(void)
{
	CRegKey pRegEventLog;
	CString str;
	HRESULT hr;

	DWORD dwFLen = GetModuleFileName(g_Module.m_hInst, CStrBuf(str, MAX_PATH), MAX_PATH);
	if( dwFLen == 0 )
		return E_FAIL;

#ifndef MAGRUL_RELEASE
	const TCHAR * const c_szAtlPerfServicesKey = _T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\IBN 4.7 Server");
#else
	const TCHAR * const c_szAtlPerfServicesKey = _T("SYSTEM\\CurrentControlSet\\Services\\EventLog\\MagRul 4.7 Server");
#endif

	hr = pRegEventLog.Open(HKEY_LOCAL_MACHINE,c_szAtlPerfServicesKey);
	if(FAILED(hr))
		return hr;

	hr = pRegEventLog.DeleteSubKey(_T("InstMsg Server 4.7"));
	if(FAILED(hr))
		return hr;
	pRegEventLog.Close();
	return hr;
}

int CEventLog::AddAppLog(LPCTSTR szSTR, DWORD ID, DWORD dwType, LPVOID lpRawData, DWORD dwDataSize)
{
	HANDLE hEventLog;
	WORD stringCount = ((szSTR != NULL) ? 2 : 1);

	LPCTSTR strings[2] = {CADOUtil::szCompanyId, szSTR};
	try
	{
		hEventLog = RegisterEventSource(NULL, _T("InstMsg Server 4.7"));
		if(hEventLog != NULL)
		{
			ReportEvent(hEventLog, (WORD)dwType, (WORD)0, ID, NULL, stringCount, dwDataSize, strings, lpRawData);
			DeregisterEventSource(hEventLog);
		}
	}
	catch(...)
	{
	}
	return 0;
}
