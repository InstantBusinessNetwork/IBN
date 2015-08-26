// CoolInet2.cpp: implementation of the CCoolInet2 class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
//#include "OfsMessenger.h"
#include "CoolInet2.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
bstr_t GUIDGen()
{
	GUID NewGuid;
	CoCreateGuid(&NewGuid);
	LPOLESTR strW;
	StringFromCLSID(NewGuid,&strW);
	CString str = strW;
	CoTaskMemFree((void*)strW);
	return str.Mid(1,36);
}

CCoolInet2::CCoolInet2():hWorkThread(0),hExitWork(0),hExitEvent(0),
        hMarshalingEndEvent(0)
{
	CoInitialize(NULL);

	p_InetSession = NULL;
	bGetMarshaling = FALSE;
	
	
	hWorkThread = CreateThread(NULL,2048,WorkThread,(LPVOID)this,CREATE_SUSPENDED,&IDWorkThread);
	ASSERT(hWorkThread);
	
	hExitWork = CreateEvent(NULL,TRUE,FALSE,NULL);
	ASSERT(hExitWork);

	hExitEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
	ASSERT(hExitEvent);

	hMarshalingEndEvent  = CreateEvent(NULL,TRUE,FALSE,NULL);
	ASSERT(hMarshalingEndEvent);

	hStartEvent  = CreateEvent(NULL,TRUE,FALSE,NULL);
	ASSERT(hStartEvent);

	MarshalEvent = CreateEvent(NULL,TRUE,FALSE,NULL);
	ASSERT(MarshalEvent);

	ResumeThread(hWorkThread);

	DWORD dwResult = WaitForSingleObject(hStartEvent,30000);
	
	if(dwResult!=WAIT_OBJECT_0)
		ASSERT(TRUE);
	
	InitializeCriticalSection(&m_lock);
	
}

CCoolInet2::~CCoolInet2()
{
	/// Освободить захваченные конекты поинты ...
	try
	{
		SetEvent(hExitEvent);
		WaitForSingleObject(hWorkThread,10000);
	}
	catch(...)
	{
	}
///	EnterCriticalSection(&m_lock);
//	LeaveCriticalSection(&m_lock);
	CloseHandle(MarshalEvent);
	CloseHandle(hExitEvent);
	CloseHandle(hMarshalingEndEvent);
	CloseHandle(hStartEvent);
	CloseHandle(hExitWork);	
	CloseHandle(hWorkThread);
	DeleteCriticalSection(&m_lock);	

	CoUninitialize();
}
//// Команды косвенного обращения к Translatoru
void CCoolInet2::InitEventWindow(HWND hEventWnd)
{
	m_InetSessionEvent.m_NetLibTranslator.NLT_SetEventWindow(hEventWnd);
}
void CCoolInet2::LockTranslator()
{
	m_InetSessionEvent.m_NetLibTranslator.Lock();
}

void CCoolInet2::AddToTranslator(LONG Handle,HWND hReturnToWnd)
{
	m_InetSessionEvent.m_NetLibTranslator.NLT_AddToTranslate(Handle,hReturnToWnd);
}

void CCoolInet2::RemoveFromTranslator(LONG &Handle)
{
	m_InetSessionEvent.m_NetLibTranslator.NLT_Remove(Handle);
	Handle = 0;
}

void CCoolInet2::UnlockTranslator()
{
	m_InetSessionEvent.m_NetLibTranslator.UnLock();
}

////............................................

DWORD CCoolInet2::WorkThread(LPVOID pParam)
{
	CCoolInet2 *pMain = (CCoolInet2 *)pParam;
	
	HRESULT hr = CoInitialize(NULL);

	pMain->WorkSession();

	CoUninitialize();		
	
	return 0;
}

BOOL CCoolInet2::WorkSession()
{
	try
	{
		CreateSession();
		InitEventContainer();

		HANDLE MasEvent[2]={MarshalEvent,hExitEvent};

		SetEvent(hStartEvent);

		while(TRUE)
		{
            MSG msg ;			

			while(PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				DispatchMessage(&msg);
			}

			DWORD dwResult = MsgWaitForMultipleObjects(2,MasEvent,FALSE,INFINITE,QS_ALLINPUT);
			
			if((dwResult - WAIT_OBJECT_0 ) ==0)
			{
				HRESULT hr = CoMarshalInterThreadInterfaceInStream(__uuidof(ISession),p_InetSession,ppSession);
				
				if(FAILED(hr))
					ppSession = NULL;

				ResetEvent(MarshalEvent);
				SetEvent(hMarshalingEndEvent);
			}
			if((dwResult - WAIT_OBJECT_0 )==1) 
			{
				//ASSERT(FALSE);
				break;
			}
            /// Обработка Сообщений Windows...    
		}
		CloseEventContainer();
		CloseSession();
	}
	catch(...)
	{
		ASSERT(FALSE);
		return FALSE;
	}

	SetEvent(hExitWork);

	return TRUE;
}

void CCoolInet2::InitEventContainer()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
	p_InetSession->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		HRESULT hr = pCPContainer->FindConnectionPoint(__uuidof(_ISessionEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
			LPUNKNOWN pInterEvent = m_InetSessionEvent.GetInterface(&IID_IUnknown);
			hr = m_pSessionConnectionPoint->Advise(pInterEvent ,&dwSessionCookie);
			pCPContainer->Release();
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
}

void CCoolInet2::CloseEventContainer()
{
	IConnectionPointContainer* pCPContainer = NULL;
	IConnectionPointPtr  m_pSessionConnectionPoint;
	p_InetSession->QueryInterface(IID_IConnectionPointContainer,(void**)&pCPContainer);
	
	if (pCPContainer)
	{
		HRESULT hr = pCPContainer->FindConnectionPoint(__uuidof(_ISessionEvents),&m_pSessionConnectionPoint);
		if(SUCCEEDED(hr))
		{
//			LPUNKNOWN pInterEvent = m_InetSessionEvent.GetInterface(&IID_IUnknown);
			hr = m_pSessionConnectionPoint->Unadvise(dwSessionCookie);
			pCPContainer->Release();
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	else
	{
		ASSERT(FALSE);
	}
   dwSessionCookie = 0;
}

void CCoolInet2::CreateSession()
{
	///{567B302A-1DBF-4A75-8E46-65F54A58D91D}
	const CLSID CLSID_ATLNetLibV10 = {0x567B302A,0x1DBF,0x4A75,
		0x8E,0x46,0x65,0xF5,0x4A,0x58,0xD9,0x1D};
	
	HRESULT hr = ::CoCreateInstance(CLSID_ATLNetLibV10,NULL,CLSCTX_INPROC_SERVER,__uuidof(ISession),(LPVOID*)&p_InetSession);
	if(FAILED(hr))
		ASSERT(FALSE);
}

void CCoolInet2::CloseSession()
{
	long hr = p_InetSession->Release();
	p_InetSession = NULL;
}

BOOL CCoolInet2::SessionMarshaling(LPSTREAM *ppStm)
{
	EnterCriticalSection(&m_lock);
	try
	{
		ppSession      = ppStm;
		
		ResetEvent(hMarshalingEndEvent);
		SetEvent(MarshalEvent);

		DWORD pReturn = WaitForSingleObject(hMarshalingEndEvent,30000);

		if(pReturn!=WAIT_OBJECT_0)
			ASSERT(FALSE);
        
		bGetMarshaling = FALSE;
		ppSession      = NULL;
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	LeaveCriticalSection(&m_lock);

	return (ppStm!=NULL)?TRUE:FALSE;
}

ISession* CCoolInet2::GetSession()
{
	IStream *pStream = NULL;
	SessionMarshaling(&pStream);
	if(pStream==NULL) return NULL;

	ISession* pSession = NULL;

	HRESULT hr = CoGetInterfaceAndReleaseStream(pStream,__uuidof(ISession),(LPVOID*)&pSession);

	//pSession->AddRef();

	if(SUCCEEDED(hr))
		return pSession;

	return NULL;
}

