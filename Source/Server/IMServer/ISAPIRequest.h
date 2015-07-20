#ifndef _ISAPI_REQUEST_
#define _ISAPI_REQUEST_

#include "xmlutil.h"
#include "Counter.h"

class CISAPIExt;
#define MAX_COMMANDSIZE 13072

class CISAPIRequest
{
public:
	const DWORD m_dwBufferSize;

	//typedefs
	typedef enum ERequestType
	{
		rtNone,
		rtGET,
		rtPOST
	} RequestType;
	typedef enum EIMRequestType
	{
		imrtNone,
		imrtCommand,
		imrtAlive,
		imrtSendFile,
		imrtRcvFile
	} IMRequestType;

	typedef enum ERequestState
	{
		rsReadHeader,
		rsReadClient,
		rsSendHeader,
		rsWriteClient,
		rsEnd

	} RequestState;

	// constructor
	CISAPIRequest()
		: m_dwBufferSize(MAX_COMMANDSIZE)
	{
		m_hIncomingFile = 0L;
		m_lpOutgoingBuffer = NULL;
		m_pbIncomingBuffer = NULL;
		m_szFileORurl = NULL;
	};

	// destructor
	virtual ~CISAPIRequest()
	{
#ifdef _IBN_PERFORMANCE_MONITOR
		if (CCounter::GetHasRealTimeMonitoring())
		{
			//m_dwTimeStart = GetTickCount();
			DWORD dwTime, dwTimeStop = GetTickCount();
			if(dwTimeStop < m_dwTimeStart)
				dwTime = (0xFFFFFFFF - m_dwTimeStart) + dwTimeStop;
			else
				dwTime = dwTimeStop - m_dwTimeStart;

			::InterlockedIncrement((LONG*)CCounter::m_ulAvrRequestBase);
			(*CCounter::m_ullAvrRequest) += dwTime;
			::InterlockedDecrement((LONG*)CCounter::m_ulCrRequest);

			switch(m_imRequestType)
			{
			case imrtCommand:
				::InterlockedIncrement((LONG*)CCounter::m_ulAvrCommandBase);
				(*CCounter::m_ullAvrCommand) += dwTime;
				::InterlockedDecrement((LONG*)CCounter::m_ulCrCommand);
				break;
			case imrtRcvFile:
				::InterlockedIncrement((LONG*)CCounter::m_ulAvrRcvFileBase);
				(*CCounter::m_ullAvrRcvFile) += dwTime;
				::InterlockedDecrement((LONG*)CCounter::m_ulCrRcvFile);
				break;
			case imrtSendFile:
				::InterlockedIncrement((LONG*)CCounter::m_ulAvrSendFileBase);
				(*CCounter::m_ullAvrSendFile) += dwTime;
				::InterlockedDecrement((LONG*)CCounter::m_ulCrSendFile);
				break;
			case imrtAlive:
				::InterlockedIncrement((LONG*)CCounter::m_ulAvrAliveBase);
				(*CCounter::m_ullAvrAlive) += dwTime;
				::InterlockedDecrement((LONG*)CCounter::m_ulCrAlive);
				break;
			default:
				break;
			}
		}
#endif
		reset();
	};

	HRESULT Init()
	{
		reset();

		HRESULT hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL, IID_IXMLDOMDocument, (void**)&m_pXMLDoc);

		CComPtr<IXMLDOMDocument2> pDoc2;
		hr = m_pXMLDoc->QueryInterface(__uuidof(IXMLDOMDocument2), (void**)&pDoc2);
		hr = pDoc2->setProperty(CComBSTR(L"SelectionLanguage"), CComVariant(CComBSTR(L"XPath")));

#ifdef _IBN_PERFORMANCE_MONITOR
		m_dwTimeStart = ::GetTickCount();
		if (CCounter::GetHasRealTimeMonitoring())
		{
			DWORD dwCounter = ::InterlockedIncrement((LONG*)CCounter::m_ulCrRequest);
			if(dwCounter > *CCounter::m_ulMaxRequest)
				::InterlockedExchange((LONG*)CCounter::m_ulMaxRequest,dwCounter);
		}
#endif
		return hr;
	}

	void reset()
	{
		m_lpISAPIExt = NULL;
		m_dwAsynchError = 0;
		m_bKeepAlive = FALSE;
		m_RequestType = rtNone;
		m_RequestState = rsReadHeader;
		m_HeaderSent = FALSE;
		m_nUserID = -1L;
		m_bsCommand.Empty();
		m_pXMLDoc.Release();
		m_WorkNode.Release();

		m_bSessionIDFound = FALSE;
		m_dwReadDataSize = 0;
		m_dwSindingSize = NULL;
		m_dwSentSize = NULL;
		m_imRequestType = imrtNone;

		if(m_hIncomingFile != 0L)
		{
			try
			{
				//SetFilePointer(m_hIncomingFile,-2,0,FILE_END);
				//SetEndOfFile(m_hIncomingFile);
				//CloseHandle(m_hIncomingFile);
			}
			catch(...){}

			m_hIncomingFile = 0L;
		}

		if(m_bsFID.Length()!=0)
		{
			try
			{
				if(m_dwErrorCode != 200 ||m_dwHTTPStatus != 200)
				{
					if(!m_bEventSent)
					{
						CDBFile::DeleteFile(m_bsFID);
					}
				}
			}
			catch(...)
			{
			}
			m_bsFID.Empty();
		}

		if(m_pbIncomingBuffer != NULL)
		{
			try
			{
				delete[] m_pbIncomingBuffer;
				m_pbIncomingBuffer = NULL;
			}catch(...){}
		}

		m_dwSentSize = 0;
		m_dwSindingSize = 0;
		m_dwOutgoingSize = 0;

		if(m_lpOutgoingBuffer != NULL)
		{
			GlobalUnlock(m_hGlobal);
			GlobalFree(m_hGlobal);
			m_hGlobal = NULL;
			m_lpOutgoingBuffer = NULL;
		}

		if(m_szFileORurl != NULL)
		{
			//if(m_dwErrorCode != 200 ||
			//   m_dwHTTPStatus != 200)
			//{
			//	if(!m_bEventSent)
			//	{
			//		::DeleteFileA(m_szFileORurl);
			//	}
			//}
			delete[] m_szFileORurl;
			m_szFileORurl = NULL;

		}
		if(m_pbIncomingBuffer != NULL)
		{
			try
			{
				delete[] m_pbIncomingBuffer;
				m_pbIncomingBuffer = NULL;
			}
			catch(...)
			{
			}
		}

		m_bEventSent = FALSE;
		m_dwErrorCode = 200;
		m_dwHTTPStatus = 200;

		memset(m_szSessionID, 0, sizeof(m_szSessionID));
	}

	HRESULT SetOutgoingBuffer()
	{
		HRESULT hr;
		hr = CXMLUtil::XML2HGlobal(m_WorkNode, &m_hGlobal, &m_dwOutgoingSize);
		if(hr != S_OK)
			return S_OK;

		if(m_dwOutgoingSize >0)
		{
			m_lpOutgoingBuffer = GlobalLock(m_hGlobal);
			return S_OK;
		}
		return S_FALSE;
	}

	HRESULT SetIncomingBuffer()
	{
		m_pbIncomingBuffer = new byte[MAX_COMMANDSIZE];
		if(m_pbIncomingBuffer != NULL)
			return S_OK;
		return E_FAIL;
	}

	CISAPIExt* m_lpISAPIExt;
	static DWORD m_dwRequestTlsCookie;

	BOOL m_bEventSent;

	LPEXTENSION_CONTROL_BLOCK m_pECB;
	CComPtr<IXMLDOMDocument> m_pXMLDoc;

	RequestType m_RequestType;
	RequestState m_RequestState;

	//User information
	TCHAR	m_szSessionID[37];
	LONG	m_nUserID;

	//Command information
	CComBSTR m_bsCommand;
	CComPtr<IXMLDOMNode> m_WorkNode;

	DWORD	m_dwAsynchError;
	CHAR*	m_szFileORurl;
	//incoming file information
	BOOL	m_bSessionIDFound;
	LONG	m_hIncomingFile;
	// Added [1/7/2004]
	CComBSTR m_bsFID;

	DWORD m_dwReadDataSize;
	DWORD m_dwReadingSize;
	PBYTE m_pbIncomingBuffer;

	//out going information
	DWORD   m_dwSindingSize;
	DWORD   m_dwSentSize;
	DWORD   m_dwOutgoingSize;
	LPVOID  m_lpOutgoingBuffer;
	HGLOBAL m_hGlobal;

	BOOL	m_bKeepAlive;

	DWORD   m_dwErrorCode;
	DWORD   m_dwHTTPStatus;

	BOOL	m_HeaderSent;

	DWORD	m_dwTimeStart;
	IMRequestType m_imRequestType;

	BOOL SetError(DWORD dwErrorCode);
	BOOL SetHttpStatus(DWORD dwHttpStatus);
};

#define GET_ISAPI_REQUEST()\
	CISAPIRequest *pRequest;\
	pRequest = (CISAPIRequest*)::TlsGetValue(CISAPIRequest::m_dwRequestTlsCookie);

#define CHECK_HR_RETURN(p,err)\
	if(hr != S_OK)\
{\
	pRequest->SetError(err);\
	return FALSE;\
}

#endif
