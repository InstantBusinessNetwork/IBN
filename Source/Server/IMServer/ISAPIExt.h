// ISAPIExt.h: interface for the CISAPIExt class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ISAPIEXT_H__01F69D14_DA55_4457_ACA1_D3451C904AC4__INCLUDED_)
#define AFX_ISAPIEXT_H__01F69D14_DA55_4457_ACA1_D3451C904AC4__INCLUDED_
#include "isapirequest.h"
#include "atlutil.h"
#include "stdafx.h"

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#define REDIRECT_URL "http://www.mediachase.ru"


#define HTTP_HEADER_CACHE			"Cache-Control: no-cache, no-store, private, no-transform"
#define HTTP_HEADER_PRAGMA			"Pragma: no-cache"
#define HTTP_HEADER_CONTENTTYPE		"Content-type: %s"
#define HTTP_HEADER_IMERROR			"IMErrorCode: %d"
#define HTTP_HEADER_CONTENTLENGTH	"Content-Length: %d"
#define HTTP_HEADER_TRANSFERENCODING "Transfer-encoding: chunked"
#define HTTP_HEADER_KEEPALIVE		"Connection: Keep-Alive"
#define HTTP_HEADER_STATUS			"%d %s"
#define RN							"\r\n"

class CISAPIExtBase
{
public:
	virtual VOID Execute(CISAPIRequest*, OVERLAPPED*)
	{
	}
};

class CWorker
{
public:
	typedef CISAPIRequest* RequestType;
	
	virtual void Terminate(void* pvWorkerParam)
	{
		ATLASSERT(pvWorkerParam != NULL);
		CoUninitialize();
	}

	virtual void Execute(CISAPIRequest* pRequest, void* pvWorkParam, OVERLAPPED* pOverlapped)
	{
		CISAPIExtBase* pIsapiExtBase = reinterpret_cast<CISAPIExtBase*>(pvWorkParam);
		if(pIsapiExtBase != NULL)
			pIsapiExtBase->Execute(pRequest, pOverlapped);
	}

	virtual BOOL Initialize(void* pvWorkerParam)
	{
		ATLASSERT(pvWorkerParam != NULL);
		HRESULT hr = ::CoInitializeEx(NULL, COINIT_MULTITHREADED);
		return SUCCEEDED(hr);
	}
};

typedef CThreadPool<CWorker> CISAPIThreadPool;

class CISAPIExt: public CISAPIExtBase
{
public:
	CISAPIExt();
	virtual ~CISAPIExt();

	BOOL Initialize();
	VOID Terminate();

	virtual VOID Execute(CISAPIRequest* pRequest, OVERLAPPED* pOverlapped);
	BOOL AddRequest(LPEXTENSION_CONTROL_BLOCK pECB);
	BOOL SendRequest(CISAPIRequest* pRequest);

protected:
	static void* WINAPI PFN_HSE_IO_COMPLETION_CALLBACK1(LPEXTENSION_CONTROL_BLOCK lpECB, PVOID pContext, DWORD cbIO, DWORD dwError);

	BOOL GetCustomHeader(CISAPIRequest* pRequest, LPCTSTR szName, LPTSTR szValue, size_t valueSizeChars);
	BOOL UnpackHeader(CISAPIRequest* pRequest);
	BOOL UnPackCommand(CISAPIRequest* pRequest);
	BOOL ProcessCommand(CISAPIRequest* pRequest, OVERLAPPED* pOverlapped);
	BOOL BeginFileWrite(DWORD dwSize);

	BOOL SessionUpdate(CISAPIRequest* pRequest);

	BOOL SetCompletionCallback(CISAPIRequest* pRequest);
	VOID SendRedirect(CISAPIRequest* pRequest, LPSTR szURL);
	BOOL SendHeaders(CISAPIRequest* pRequest);
	VOID EndRequest(CISAPIRequest* pRequest);
	
	CISAPIThreadPool m_pPostThreadPool;
	CISAPIThreadPool m_pGetThreadPool;

	BOOL m_bIsInit;
	CComPtr<IXMLDOMDocument> m_Doc;
};


#endif // !defined(AFX_ISAPIEXT_H__01F69D14_DA55_4457_ACA1_D3451C904AC4__INCLUDED_)
