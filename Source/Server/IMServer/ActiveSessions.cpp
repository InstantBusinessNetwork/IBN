// ActiveSessions.cpp: implementation of the CActiveSessions class.
//
//////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include <time.h>
#include "ibn_server.h"
#include "ActiveSessions.h"
#include "McLicenseVerify/McLicenseVerify.h"
#include "ExternalDeclarations.h"

//#define _OFS_WRITE_TRACE_INFO // Write Trace Info
//#pragma comment(lib,"crypt32.lib")

//#include "EventLog.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

/// События на окончание работы таймера...
//////////////////////////////////////////////////////////////////////

#ifdef _IBN_LICENSE_CHECKER

#define PRODUCT_GUID _T("{297C8BB0-44B6-43F9-B353-7E36E88A08C4}")

HRESULT LoadLicenseData(IXMLDOMDocument** ppDocument)
{
	//CEventLog::AddAppLog(_T("LoadLicenseData - Begin"),S_OK,EVENTLOG_ERROR_TYPE);

	HRESULT hr = E_INVALIDARG;

	if(!IsBadWritePtr(ppDocument, sizeof(IXMLDOMDocument*)))
	{
		hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL, IID_IXMLDOMDocument, (void**)ppDocument);

		//CEventLog::AddAppLog(_T("LoadLicenseData::m_pClassFactory->CreateInstance"),hr,EVENTLOG_ERROR_TYPE);

		if(hr == S_OK)
		{
			DWORD cbLicenseData;

			//CEventLog::AddAppLog(_T("LoadLicenseData::GetLicenseDataSize"),hr,EVENTLOG_ERROR_TYPE);

			hr = GetLicenseDataSize(NULL, PRODUCT_GUID, &cbLicenseData);
			if(SUCCEEDED(hr))
			{
				//CEventLog::AddAppLog(_T("LoadLicenseData::GetLicenseDataSize - OK"),hr,EVENTLOG_ERROR_TYPE);

				CHeapPtr<BYTE> pDataBuffer;
				pDataBuffer.Allocate(cbLicenseData);

				hr = GetLicenseData(NULL, PRODUCT_GUID, _T("Mediachase_IbnServer_4.7"), pDataBuffer, &cbLicenseData);

				//CEventLog::AddAppLog(_T("LoadLicenseData::GetLicenseData - OK"),hr,EVENTLOG_ERROR_TYPE);

				if(SUCCEEDED(hr))
					hr = CXMLUtil::MEM2XML(*ppDocument, pDataBuffer, cbLicenseData);
			}
			else
				hr = E_FAIL;
		}
	}

	//CEventLog::AddAppLog(_T("LoadLicenseData - End"),hr,EVENTLOG_ERROR_TYPE);

	return hr;
}

int LicenseCheckActiveUsers()
{
	//int &activeUsersCount 
	//CEventLog::AddAppLog(_T("LoadLicenseData::LicenseCheckUsersPerPortal - Begin"),S_OK,EVENTLOG_ERROR_TYPE);

	static	time_t	lastAccessTime			= 0;
	//static	BOOL	hashCheckUsersPerPortal = FALSE;
	static	int		hashActiveUsersCount	= 0;
	//static	BOOL	hasRealTimeMonitoring	= FALSE;

	int activeUsersCount = 0;

	if(hashActiveUsersCount==0||(lastAccessTime+60*60)<time(NULL))
	{
		lastAccessTime = time(NULL);

		CComPtr<IXMLDOMDocument> pLicenseDocument = NULL;
		HRESULT hr = LoadLicenseData(&pLicenseDocument);

		//CEventLog::AddAppLog(_T("LoadLicenseData::LoadLicenseData"),hr,EVENTLOG_ERROR_TYPE);

		if(hr==S_OK)
		{
			CComPtr<IXMLDOMNode>	pParamNode2	=	NULL;
			hr = pLicenseDocument->selectSingleNode(CComBSTR(L"license/params/param[@name='ActiveUsersCount']"),&pParamNode2);

			if(pParamNode2!=NULL)
			{
				CComBSTR	xmlNodeText2;
				pParamNode2->get_text(&xmlNodeText2);

				if(xmlNodeText2.Length()>0)
				{
					hashActiveUsersCount = _wtoi(xmlNodeText2);
					activeUsersCount = hashActiveUsersCount;
				}
			}
		}
	}

#ifdef _IBN_PERFORMANCE_MONITOR
	if(updateHasRealTimeMonitoring)
		CCounter::SetHasRealTimeMonitoring(hasRealTimeMonitoring);
#endif 

	activeUsersCount = hashActiveUsersCount;

	//CEventLog::AddAppLog(_T("LoadLicenseData::LicenseCheckUsersPerPortal - End"),S_OK,EVENTLOG_ERROR_TYPE);

	return activeUsersCount;
}

#endif //_IBN_LICENSE_CHECKER
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CActiveSessions::CActiveSessions()
{
	_idTimerThread = 0;
	_idSendThread = 0;
	_thread = 0;

	_hExitTimerEvent = CreateEvent(NULL, FALSE, TRUE, NULL);
	_hEventExit = CreateEvent(NULL, FALSE, TRUE, NULL);

	ASSERT(_hExitTimerEvent != NULL);
	ASSERT(_hEventExit!=NULL);

	_dwLicenseNumber = 0;
}

CActiveSessions::~CActiveSessions()
{
	if(_thread > 0)
		Terminate();

	CloseHandle (_hExitTimerEvent);
	CloseHandle (_hEventExit);
}

void CActiveSessions::AddAcitveSession(long id, LPCTSTR sid)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[Add] Id = %ld SID = %s"), id, sid);
	//g_SupportClass.AddToLog(1, str);
#endif


	CActiveSession* pNewActiveS = new CActiveSession();
	/// Инициализации ... pNewActiveS
	ASSERT(pNewActiveS != NULL);
	_matrix.Add(id, sid, pNewActiveS);
}

void CActiveSessions::DeleteActiveSession(long id)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	//	TRACE("\r\n Active Session's  Delete  Id = %ld ",ID);
#endif

	CActiveSession* pActiveSession = _matrix.Remove(id);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		pActiveSession->ClearInfo();
		pActiveSession->CloseSession();
		// delete Active Session
		pActiveSession->Delete();
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::DeleteActiveSession(LPCTSTR sid)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	//	TRACE("\r\n Active Session's  Delete  SID = %s ",SID);
#endif

	CActiveSession* pActiveSession = _matrix.Remove(sid);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		pActiveSession->ClearInfo();
		pActiveSession->CloseSession();
		// delete Active Session
		pActiveSession->Delete();
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::GetSIDByID(long id, LPTSTR &sid, size_t sidSize)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[GetSIDByID] Id = %ld"), id);
	//g_SupportClass.AddToLog(1, str);
	//	TRACE("\r\n Active Session's  GetSIDByID  ID = %ld",ID);
#endif

	CActiveSession* pActiveSession = _matrix.Get(id, sid, sidSize);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование

		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::GetIDbySID(LPCTSTR sid, long &id)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[GetIDBySID] Id = %s"), sid);
	//g_SupportClass.AddToLog(1, str);
	//	TRACE("\r\n Active Session's  GetIDbySID  SID = %s",SID);
#endif

	CActiveSession* pActiveSession = _matrix.Get(sid, id);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование

		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::SendEvent(long id, PBYTE pData, size_t size)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[SendEvent] Id = %ld"), id);
	//g_SupportClass.AddToLog(1, str);
	//	TRACE("\r\n Active Session's  SendEvent ID = %ld Size = %Iu Data = (*) %s (*)",ID,Size,pData);
#endif

	TCHAR sid[COFSMatrix::_sidSize];
	CActiveSession* pActiveSession = _matrix.Get(id, sid, COFSMatrix::_sidSize);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование
		pActiveSession->SetEvent(pData, size);
		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::UpdateConnection(long id, EXTENSION_CONTROL_BLOCK *pEcb, BOOL keepAlive)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[UpDateConnection] Id = %ld"), id);
	//g_SupportClass.AddToLog(1, str);
#endif

	TCHAR sid[COFSMatrix::_sidSize];
	CActiveSession* pActiveSession = _matrix.Get(id, sid, COFSMatrix::_sidSize);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование
		pActiveSession->UpdateConnection(pEcb, keepAlive);
		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::SetStatus(long id, long status)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	//	TRACE("\r\n Active Session's  SetStatus ID = %ld Status = %ld ",ID,Status);
#endif

	TCHAR sid[COFSMatrix::_sidSize];
	CActiveSession* pActiveSession = _matrix.Get(id, sid, COFSMatrix::_sidSize);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование
		pActiveSession->SetStatus(status);
		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}

void CActiveSessions::GetStatus(long id, long& status)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	//	TRACE("\r\n Active Session's  GetStatus ID = %ld ",ID);
#endif

	TCHAR sid[COFSMatrix::_sidSize];
	CActiveSession* pActiveSession= _matrix.Get(id, sid, COFSMatrix::_sidSize);
	ASSERT(pActiveSession != NULL);
	if(pActiveSession != NULL)
	{
		/// Использование
		status = pActiveSession->GetStatus();
		/// После окончания использования освободить
		pActiveSession->ReleasePointer();
	}
}



DWORD CActiveSessions::TimerThread(LPVOID pParam)
{
	CActiveSessions *pActiveSs = (CActiveSessions *)pParam;

	pActiveSs->Timer();

	return 0;
}

void CActiveSessions::Timer()
{
#ifdef _OFS_WRITE_TRACE_INFO
		TCHAR str[80];
#endif

	while(TRUE)
	{

#ifdef _OFS_WRITE_TRACE_INFO
		_stprintf_s(str, 80, _T("Active Session's ----- Timer -----"));
		//g_SupportClass.AddToLog(1, str);
		//	TRACE0("\r\n Active Session's ----- Timer -----");
#endif

		MYPOSITION pos;

		_matrix.GetStartPosition(pos);

		long id;
		TCHAR sid[COFSMatrix::_sidSize];
		CActiveSession* pActiveSession = NULL;
		_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);

		while(pActiveSession != NULL)
		{
			/////////////////
			/// Использование
			if(pActiveSession->CheckTime())
			{
				/// Отправить сообщение Удалением ///
				// ????? Рас// перед употреблением

#ifdef _OFS_WRITE_TRACE_INFO
				_stprintf_s(str, 80, _T("[Active Session]->[Delete] Id = %ld"), id);
				//g_SupportClass.AddToLog(1, str);
#endif

				if(_idSendThread)
					PostThreadMessage(_idSendThread, WM_CONNECT_TIMEOUT, id, NULL);
				//CActiveSession* pKillActiveS = NULL;
				//pKillActiveS = Matrix.Remove(ID);
				//pKillActiveS->ClearInfo();
				//pKillActiveS->CloseSession();
				//pKillActiveS->Delete();
				//pKillActiveS->ReleasePointer();
				// ?????
			}
			/// После окончания использования освободить
			pActiveSession->ReleasePointer();

			_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
		}

		DWORD dwExitFlag = WaitForSingleObject(_hEventExit, RefreshTime);//Sleep();

		if(dwExitFlag != WAIT_TIMEOUT)
			break;
	}

	/// Конец процесса ...
#ifdef _OFS_WRITE_TRACE_INFO
	//	TRACE0("\r\n Active Session's ----- Timer Stop -----");
#endif

	SetEvent(_hExitTimerEvent);
}
//Initialize
DWORD CActiveSessions::Initialize(DWORD idEventThread)
{
	if(_thread > 0)
		return S_OK;

	// Step 1. Initialize
	_idSendThread = idEventThread;

	ResetEvent(_hExitTimerEvent);
	ResetEvent(_hEventExit);

	// Step 2. Create Timer Thread
	_thread = _beginthreadex(NULL, 0, 
		(unsigned int (__stdcall *)(void *)) TimerThread,
		(LPVOID)this, 
		0, 
		(unsigned int *) &_idTimerThread);

	ASSERT(_thread > 0);

	return (_thread > 0 ? S_OK : E_FAIL);
}

//Terminate
DWORD CActiveSessions::Terminate(DWORD dwTime)
{

	if(_thread <= 0)
		return S_OK;

	SetEvent(_hEventExit);

	DWORD TimeExit = WaitForSingleObject(_hExitTimerEvent, dwTime);

	switch(TimeExit)
	{
	case WAIT_OBJECT_0:
		// Ol OK ...
		break;
	case WAIT_TIMEOUT:
		/// Thread can'not Del ...
		//ASSERT(FALSE);
		break;
	default :
		//ASSERT(FALSE);
		break;
	}
	CloseHandle (reinterpret_cast<HANDLE>(_thread));
	_thread = 0;

	clock_t finish;
	finish = clock() + clock_t(dwTime);

	try
	{
		if(finish > clock())
		{
			/// Закрыть ...
			MYPOSITION pos;
			if(_matrix.GetStartPosition(pos))
			{
				long id;
				TCHAR sid[COFSMatrix::_sidSize];
				CActiveSession* pActiveSession = NULL;
				_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
				while(pActiveSession && finish>clock())
				{
					ASSERT(pActiveSession != NULL);
					pActiveSession->CloseSession(); 
					pActiveSession->ReleasePointer();
					_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
				}
			}
		}
		//// Подождать когда все закроются ...
		while (finish>clock())
		{
			long id;
			TCHAR sid[COFSMatrix::_sidSize];
			CActiveSession* pActiveSession = NULL;
			MYPOSITION pos;

			while(finish > clock())
			{
				BOOL bExit = TRUE;
				_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
				while(pActiveSession && finish > clock())
				{
					if(!pActiveSession->IsClosed()) 
						bExit = FALSE;
					pActiveSession->ReleasePointer();
					_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
				}

				if(bExit)
					break;

				Sleep(1);
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}

	try
	{
		//// Просто убить ...
		MYPOSITION pos;
		if(_matrix.GetStartPosition(pos))
		{
			long id;
			TCHAR sid[COFSMatrix::_sidSize];
			CActiveSession* pActiveSession = NULL;
			_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);

			while(pActiveSession)
			{
				ASSERT(pActiveSession != NULL);
				if(!pActiveSession->IsClosed())
					pActiveSession->UpdateConnection(NULL, FALSE);
				pActiveSession->ReleasePointer();
				_matrix.Remove(id);
				_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}

	return (finish>clock())?TRUE:FALSE;
}

void CActiveSessions::SendEventForAll(PBYTE pData, size_t size)
{
	if(_thread <= 0)
		throw(ERR_UNKNOW);

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[Active Session]->[SendAllEvent]"));
	//g_SupportClass.AddToLog(1, str);
#endif

	MYPOSITION pos;
	if(_matrix.GetStartPosition(pos))
	{
		long id;
		TCHAR sid[COFSMatrix::_sidSize];
		CActiveSession* pActiveSession = NULL;
		_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);

		while(pActiveSession != NULL)
		{
			ASSERT(pActiveSession != NULL);

			pActiveSession->SetEvent(pData, size);
			pActiveSession->ReleasePointer();

			_matrix.GetNextAssoc(pos, id, sid, COFSMatrix::_sidSize, pActiveSession);
		}
	}
}

void CActiveSessions::CheckLicense()
{
#ifdef _IBN_LICENSE_CHECKER
	int ActiveUserCount = LicenseCheckActiveUsers();
	
	//if(LicenseCheckUsersPerPortal(ActiveUserCount, FALSE))
	//{
		// Get User Count For Current Portal [7/16/2004]
		/*_bstr_t cmdText = L"SELECT COUNT(*) FROM IM_ACTIVE_USER";

		long ActiveUserPerDomenCount = CADOUtil::RunCommand_ReturnLong(cmdText, NULL);

		if(ActiveUserCount != -1 && ActiveUserPerDomenCount >= ActiveUserCount)*/
	//		throw(ERR_LICENSE_LIMIT);
	//}
	//else
	//{
		if(ActiveUserCount != -1 && _matrix.GetCount() >= ActiveUserCount)
			throw(ERR_LICENSE_LIMIT);
	//}
#endif //_IBN_LICENSE_CHECKER
}
