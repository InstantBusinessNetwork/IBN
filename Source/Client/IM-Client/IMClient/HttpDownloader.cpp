// HttpDownloader.cpp: implementation of the CHttpDownloader class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "HttpDownloader.h"
#include "Resource.h"
#include "GlobalFunction.h"

#include "atlenc.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CHttpDownloader::CHttpDownloader()
{
	m_hInternet = 0;
	m_hConnect = 0;
	m_hRequest = 0;
	m_longAbort = FALSE;
	m_pStream = NULL;
	m_hWnd = NULL;
	m_nMessage = 0;
	m_dwTotalSize = 0;
	m_dwDownloaded = 0;

	m_dwTimeout = 60000;
	m_dwConnectRetryCount = 3;

    m_ProxyType = GetOptionInt(IDS_NETOPTIONS, IDS_ACCESSTYPE, INTERNET_OPEN_TYPE_PRECONFIG);
	m_ProxyName.Format(_T("http=http://%s:%s"), GetOptionString(IDS_NETOPTIONS, IDS_PROXYNAME, _T("")), GetOptionString(IDS_NETOPTIONS, IDS_PROXYPORT, _T("")));
	
//	m_UseFireWall = GetOptionInt(IDS_NETOPTIONS, IDS_USEFIREWALL, FALSE);
//	m_FireWallUser = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, "");
//	m_FireWallPass = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, "");

	m_hEvent = ::CreateEvent(NULL, TRUE, TRUE, NULL);
}

CHttpDownloader::~CHttpDownloader()
{
	Clear();
	if(m_pStream)
	{
		m_pStream->Release();
		m_pStream = NULL;
	}
	CloseHandle(m_hEvent);
}

HRESULT CHttpDownloader::ConnectToServer(_bstr_t &strBuffer)
{
	BOOL bResult;
	DWORD dwStatus;
	DWORD nTimeoutCounter;
	_bstr_t strMethod;
	_bstr_t strUrl = m_request.url;

	LPCTSTR szProxyName = NULL;
	if(m_ProxyType == INTERNET_OPEN_TYPE_PROXY)
		szProxyName = m_ProxyName;
	
//// InternetOpen \\\\	
	if(!m_hInternet)
		m_hInternet = InternetOpen(_T("McHttpDownloader"), m_ProxyType, szProxyName, NULL, INTERNET_FLAG_ASYNC);
	if(!m_hInternet)
	{
		strBuffer = _T("InternetOpen failed");
		return E_FAIL;
	}

	
	InternetSetStatusCallback(m_hInternet, (INTERNET_STATUS_CALLBACK)CallbackFunc);
//// InternetOpen ////	
	
	if(m_longAbort > 0)
		return E_ABORT;

//// InternetConnect \\\\	
	if(!m_hConnect)
	{
		m_hConnect = InternetConnect(m_hInternet, m_request.server, (short)m_request.port, NULL, NULL, INTERNET_SERVICE_HTTP, NULL, (DWORD)&m_context);
	}
	if(m_hConnect == NULL)
	{
		strBuffer = _T("InternetConnect failed");
		return INET_E_CANNOT_CONNECT;
	}
//// InternetConnect ////	
	
	nTimeoutCounter = 0;

NewConnect:

	strMethod = _T("GET");
	strBuffer = _T("");

//// OpenRequest \\\\	
	if(m_hRequest)
	{
		::InternetCloseHandle(m_hRequest);
		m_hRequest = NULL;
	}
	
	if(m_longAbort > 0)
		return E_ABORT;

	DWORD dwFlags = 
		INTERNET_FLAG_KEEP_CONNECTION	|
		INTERNET_FLAG_NO_CACHE_WRITE	|
		INTERNET_FLAG_RELOAD			|
		INTERNET_FLAG_PRAGMA_NOCACHE	|
		INTERNET_FLAG_NO_UI			|
		INTERNET_FLAG_NO_COOKIES		|
		INTERNET_FLAG_IGNORE_CERT_CN_INVALID  |
		INTERNET_FLAG_NO_AUTO_REDIRECT  | 
		INTERNET_FLAG_HYPERLINK |
		(m_request.bUseSSL ?
							(INTERNET_FLAG_SECURE/*|
							INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTPS|
							INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP*/)
							:
							0);
		
	m_context.op = HTTP_DOWNLOADER_OP_OPEN_REQUEST;
	m_hRequest = HttpOpenRequest(m_hConnect, strMethod, strUrl, _T("HTTP/1.1"), NULL, NULL, dwFlags, (DWORD)&m_context);
	if(m_hRequest == NULL)
	{
		strBuffer = _T("HttpOpenRequest failed");
		return E_FAIL;
	}

	if(m_ProxyType == INTERNET_OPEN_TYPE_PROXY)
	{
		if(GetOptionInt(IDS_NETOPTIONS,IDS_USEFIREWALL,0))
		{
			CString fireWallUser =  GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLUSER, _T(""));
			CString fireWallPass = GetOptionString(IDS_NETOPTIONS, IDS_FIREWALLPASS, _T(""));

			//////////////////////////////////////////////////////////////////////////

			int HeaderLen = ATL::ProxyAuthorizationStringGetRequiredLength(fireWallUser, fireWallPass);
			LPTSTR strHeader = new TCHAR[HeaderLen+1];
			ZeroMemory(strHeader,HeaderLen+1);

			HRESULT hr = ATL::ProxyAuthorizationString(fireWallUser, fireWallPass, strHeader, &HeaderLen);

			ASSERT(hr==S_OK);

			HttpAddRequestHeaders(m_hRequest, strHeader, HeaderLen, HTTP_ADDREQ_FLAG_ADD );

			delete []strHeader;
			//////////////////////////////////////////////////////////////////////////
		}
	}
	
//// OpenRequest ////	
	
NewRequest:

	if(m_longAbort > 0)
		return E_ABORT;

	m_context.op = HTTP_DOWNLOADER_OP_SEND_REQUEST;
	bResult = HttpSendRequest(m_hRequest, NULL , 0, (LPVOID)(BYTE*)(LPCTSTR)strBuffer, lstrlen(strBuffer));
	if(!bResult && 997 == GetLastError())		// Overlapped I/O operation is in progress.
		bResult = WaitForComplete(m_dwTimeout); // Resolve host name, connect, send request, receive response.
	if(!bResult)
	{
		DWORD dwErrCode = GetLastError();
//		ATLTRACE("Send Request error = %d \r\n",dwErrCode);
		
		if(dwErrCode == 6)		// The handle is invalid.
			goto NewConnect;
		
		if(dwErrCode == ERROR_INTERNET_TIMEOUT)	// timeout
		{
			if(++nTimeoutCounter < m_dwConnectRetryCount)
				goto NewConnect;
			else
			{
				strBuffer = _T("Timeout");
				return E_FAIL;//INET_E_CONNECTION_TIMEOUT;
			}
		}

		strBuffer = _T("SendRequest failed");
		return E_FAIL;
	}
	
	dwStatus = GetHttpStatus();

	if(dwStatus == 401 || dwStatus == 407) // Denied or Proxy asks password
	{
		if(ERROR_INTERNET_FORCE_RETRY == 
		   InternetErrorDlg(GetDesktopWindow(), m_hRequest,
				ERROR_INTERNET_INCORRECT_PASSWORD,
				FLAGS_ERROR_UI_FILTER_FOR_ERRORS | 
				FLAGS_ERROR_UI_FLAGS_CHANGE_OPTIONS |
				FLAGS_ERROR_UI_FLAGS_GENERATE_DATA,
				NULL))
			{
				goto NewRequest;
			}
					
	}

	if(dwStatus != 200) // Not OK
	{		
		strBuffer = _T("SendRequest returned with error");
		return INET_E_CANNOT_CONNECT;
	} 

	return S_OK;
}

DWORD CHttpDownloader::GetHttpStatus()
{
	LPVOID lpOutBuffer	= new char[4];
	DWORD  dwSize		= 4;
	DWORD  Status		= 0;
	DWORD  err			= 0;
	BOOL bResult;
	
ret:
	m_context.op = HTTP_DOWNLOADER_OP_GET_STATUS;
	bResult = HttpQueryInfo(m_hRequest, HTTP_QUERY_STATUS_CODE, (LPVOID)lpOutBuffer, &dwSize, NULL);
	if(!bResult&&997 == GetLastError())
		WaitForComplete(m_dwTimeout);
	if(!bResult)
	{
		err = GetLastError();
		
		if(err == ERROR_HTTP_HEADER_NOT_FOUND)
		{
			return false; //throw();
		}
		else
		{
			if (err == ERROR_INSUFFICIENT_BUFFER)
			{
				if(lpOutBuffer!=NULL) 
					delete[] lpOutBuffer;

				lpOutBuffer = new char[dwSize];
				goto ret;
			}
			else
			{
				return Status; //throw();
			}
		}
	}
	else
	{
//		ATLTRACE("HTTP STATUS - %s \r\n", lpOutBuffer);
		Status = atol((char*)lpOutBuffer);
		delete[] lpOutBuffer;
		lpOutBuffer = NULL;
		
		return Status;
	}
}

HRESULT CHttpDownloader::WorkFunction()
{
	HRESULT hr;
	_bstr_t strBuffer;
	
	strBuffer = _T("");

	if(m_longAbort > 0)
	{
		hr = E_ABORT;
		goto EndWorkFunc;
	}

	if(m_request.pCritSect)
		EnterCriticalSection(m_request.pCritSect);
	
	if(m_longAbort > 0)
	{
		hr = E_ABORT;
		goto EndWorkFunc;
	}

	hr = ConnectToServer(strBuffer);
	if(FAILED(hr))
		goto EndWorkFunc;

	hr = ReadData(strBuffer);
	if(FAILED(hr) || m_dwDownloaded < m_dwTotalSize)
	{
		strBuffer = _T("Cannot read data");
		goto EndWorkFunc;
	}
	
EndWorkFunc:

//	TRACE(_T("HTTP OPERATION = %d\n"), m_context.op);

	Clear();
	
	if(m_pStream)
	{
		LARGE_INTEGER li = {0, 0};
		hr = m_pStream->Seek(li, STREAM_SEEK_SET, NULL);
	}
	
	if(m_request.pCritSect)
	{
		try
		{
			LeaveCriticalSection(m_request.pCritSect);
		}
		catch(...)
		{
//			MCTRACE(0, _T("LeaveCriticalSection(%08x)"), m_request.pCritSect);
		}
	}
	
	return hr;
}

HRESULT CHttpDownloader::ReadData(_bstr_t &strBuffer)
{
	HRESULT hr = E_FAIL;
	IStream *pStream = NULL;
	INTERNET_BUFFERS ib ={0};
	ULONG ulWritten;
	BOOL bResult;
	DWORD dwErr;

	TCHAR buf[32];
	DWORD dwBufferLength;
	TCHAR *szNULL = _T("\x0");


	// Get file size
	dwBufferLength = 32*sizeof(TCHAR);
	if(::HttpQueryInfo(m_hRequest, HTTP_QUERY_CONTENT_LENGTH, buf, &dwBufferLength, NULL))
	{
		m_dwTotalSize = _tcstoul(buf, &szNULL, 10);
		if(m_hWnd != NULL && m_nMessage != 0)
			::PostMessage(m_hWnd, m_nMessage, m_dwDownloaded, m_dwTotalSize);
	}

	//Check MD5 hash
	if(m_request.md5 != NULL && _tcslen(m_request.md5) >= 22)
	{
		dwBufferLength = 32*sizeof(TCHAR);
		if(::HttpQueryInfo(m_hRequest, HTTP_QUERY_CONTENT_MD5, buf, &dwBufferLength, NULL))
		{
			if(0 == _tcsnicmp(m_request.md5, buf, 22))
			{
				if(m_hWnd != NULL && m_nMessage != 0)
					::PostMessage(m_hWnd, m_nMessage, m_dwTotalSize, m_dwTotalSize);
			}
				return S_OK;
		}
	}


	hr = CreateStreamOnHGlobal(NULL, TRUE, &pStream);
	if(FAILED(hr))
	{
		return hr;
	}
	
	ib.lpcszHeader	 = NULL;
	ib.dwHeadersLength = NULL;
	ib.lpvBuffer		 = new TCHAR[COMMAND_BUFF_SIZE_PART];
	ib.dwBufferLength	 = COMMAND_BUFF_SIZE_PART;
	ib.dwStructSize	 = sizeof(ib);

	do
	{
		ib.dwBufferLength	= COMMAND_BUFF_SIZE_PART;

		if(m_longAbort > 0)
		{
			hr = E_ABORT;
			break;
		}

		m_context.op = HTTP_DOWNLOADER_OP_READ_DATA;
		bResult = InternetReadFileEx(m_hRequest, &ib, IRF_ASYNC | IRF_USE_CONTEXT, (DWORD)&m_context);
		dwErr = GetLastError();
		if(!bResult && dwErr == 997)		// Overlapped I/O operation is in progress.
		{
			bResult = WaitForComplete(m_dwTimeout);
			if(bResult)
				continue;
		}
		
		if(bResult)
		{
			if(ib.dwBufferLength) 
			{
				hr = pStream->Write(ib.lpvBuffer, ib.dwBufferLength, &ulWritten);
				if(FAILED(hr))
				{
					strBuffer = _T("Cannot write to stream");
					break;
				}
				m_dwDownloaded += ib.dwBufferLength;
				if(m_hWnd != NULL && m_nMessage != 0)
					::PostMessage(m_hWnd, m_nMessage, m_dwDownloaded, m_dwTotalSize);
			}
		}
		else
		{
			hr = E_FAIL;
			break;
		}
//		Sleep(1);
	} while(ib.dwBufferLength);

	if(ib.lpvBuffer)
	{
		delete[] ib.lpvBuffer;
		ib.lpvBuffer  = NULL;
	}

	if(SUCCEEDED(hr) && pStream)
	{
		m_pStream = pStream;
		return hr;
	}
	else
	{
		if(pStream)
			pStream->Release();
		pStream = NULL;
	}
	
	return hr;
}

HRESULT CHttpDownloader::Load(LPCTSTR szUrl, IStream **ppStream, LPCTSTR szMD5)
{
	HRESULT hr = E_FAIL;

	m_longAbort	=	0;
	Clear();

	*ppStream = NULL;
	m_dwDownloaded = 0;
	m_dwTotalSize = 0;

	m_request.md5 = szMD5;
	hr = ParseUrl(szUrl);
	if(SUCCEEDED(hr))
	{
		if(m_pStream)
		{
			m_pStream->Release();
			m_pStream = NULL;
		}

		m_context.op = HTTP_DOWNLOADER_OP_IDLE;
		m_context.hEvent = m_hEvent;
		SetEvent(m_hEvent);

		hr = WorkFunction();
		if(SUCCEEDED(hr))
		{
			m_pStream->AddRef();
			*ppStream = m_pStream;
		}
	}
	return hr;
}

HRESULT CHttpDownloader::ParseUrl(LPCTSTR szUrlIn)
{
	if(!lstrlen(szUrlIn))
		return INET_E_INVALID_URL;
	
	URL_COMPONENTS uc;
	memset(&uc, 0, sizeof(uc));
	uc.dwStructSize = sizeof(uc);
	
	TCHAR szServer[1024];
	TCHAR szUrl[1024];
//	TCHAR szScheme[1024];
	
	uc.lpszHostName = szServer;
	uc.dwHostNameLength = sizeof(szServer);
	uc.lpszUrlPath = szUrl;
	uc.dwUrlPathLength = sizeof(szUrl);
	
//	uc.lpszScheme		=szScheme;
//	uc.dwSchemeLength	= sizeof(szScheme);
	
	if(!InternetCrackUrl(szUrlIn, lstrlen(szUrlIn), 0, &uc))
		return INET_E_INVALID_URL;

	if(uc.nScheme==INTERNET_SCHEME_HTTPS)
		m_request.bUseSSL	=	TRUE;
	
	if(lstrlen(szServer))
		m_request.server = szServer;
	if(uc.nPort != 0)
		m_request.port = uc.nPort;
	if(lstrlen(szUrl))
		m_request.url = szUrl;
	
	return S_OK;
}


void CHttpDownloader::EnableProgress(HWND hWnd, UINT nMessage)
{
	m_hWnd = hWnd;
	m_nMessage = nMessage;
}

void CHttpDownloader::Abort()
{
	InterlockedExchange(&m_longAbort, 1);
	Clear();
	SetEvent(m_hEvent);
}

void CHttpDownloader::Clear()
{
	if(m_hRequest)
	{
		InternetCloseHandle(m_hRequest);
		m_hRequest = NULL;
	}
	if(m_hConnect)
	{
		InternetCloseHandle(m_hConnect);
		m_hConnect = NULL;
	}
	if(m_hInternet)
	{
		InternetCloseHandle(m_hInternet);
		m_hInternet = NULL;
	}
}

void __stdcall CHttpDownloader::CallbackFunc(HINTERNET hInternet, DWORD dwContext, DWORD dwInternetStatus, LPVOID lpvStatusInformation, DWORD dwStatusInformationLength)
{
	//	TRACE(_T("INTERNET CALLBACK %08x %08x %d\n"), hInternet, dwContext, dwInternetStatus);
	if(IsBadWritePtr((LPVOID)dwContext,sizeof(CHttpDownloader :: HttpDownloaderContext)))
		return;

	CHttpDownloader::HttpDownloaderContext *pContext = (CHttpDownloader::HttpDownloaderContext*)dwContext;
	INTERNET_ASYNC_RESULT* pINTERNET_ASYNC_RESULT = NULL;

	BOOL bRetVal =  FALSE;

	switch (dwInternetStatus)
	{
    case INTERNET_STATUS_REQUEST_COMPLETE:
		pINTERNET_ASYNC_RESULT = (INTERNET_ASYNC_RESULT*)lpvStatusInformation;

		if(pINTERNET_ASYNC_RESULT->dwError!=ERROR_INTERNET_OPERATION_CANCELLED)
		{
			pContext->dwError = pINTERNET_ASYNC_RESULT->dwError;
			bRetVal = SetEvent(pContext->hEvent);
		}
        break;

	/*
	case INTERNET_STATUS_RESPONSE_RECEIVED:
			if(pContext)
			{
				if(pContext->op == HTTP_DOWNLOADER_OP_READ_DATA)
					SetEvent(pContext->hEvent);
			}
	        break;*/
	
		
	case INTERNET_STATUS_RESOLVING_NAME:
	case INTERNET_STATUS_NAME_RESOLVED:
	case INTERNET_STATUS_CONNECTING_TO_SERVER:
	case INTERNET_STATUS_CONNECTED_TO_SERVER:
	case INTERNET_STATUS_SENDING_REQUEST:
	case INTERNET_STATUS_REQUEST_SENT:
	case INTERNET_STATUS_RECEIVING_RESPONSE:
	case INTERNET_STATUS_CTL_RESPONSE_RECEIVED:
	case INTERNET_STATUS_PREFETCH:
	case INTERNET_STATUS_CLOSING_CONNECTION:
	case INTERNET_STATUS_CONNECTION_CLOSED:
	case INTERNET_STATUS_HANDLE_CREATED:
	case INTERNET_STATUS_HANDLE_CLOSING:
	case INTERNET_STATUS_REDIRECT:
	case INTERNET_STATUS_INTERMEDIATE_RESPONSE:
	case INTERNET_STATUS_STATE_CHANGE:
    default:
        break;
	}
}

BOOL CHttpDownloader::WaitForComplete(DWORD dwMilliseconds)
{
	ResetEvent(m_hEvent);
	DWORD dw = WaitForSingleObject(m_hEvent, dwMilliseconds);
	//TRACE("\r\n == after WaitForSingleObject == \r\n");
	if(m_longAbort > 0)
	{
//		TRACE(_T("WaitForComplete: ABORTED, OP = %d\n"), m_context.op);
		SetLastError(ERROR_INTERNET_CONNECTION_ABORTED);
		return FALSE;
	}
	if(dw == WAIT_TIMEOUT)
	{
//		TRACE(_T("WaitForComplete: TIMEOUT, OP = %d\n"), m_context.op);
		SetLastError(ERROR_INTERNET_TIMEOUT);
		return FALSE;
	}
	
	if(m_context.dwError != ERROR_SUCCESS)
		return FALSE;
	
	return (dw == WAIT_OBJECT_0);
}
