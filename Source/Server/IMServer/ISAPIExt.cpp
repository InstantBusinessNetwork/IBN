// ISAPIExt.cpp: implementation of the CISAPIExt class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ISAPIExt.h"
#include "SupportClass.h"
#include "ActiveSessions.h"
#include "ExternalDeclarations.h"

DWORD CISAPIRequest::m_dwRequestTlsCookie = 0;
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CISAPIExt::CISAPIExt():m_bIsInit(FALSE)
{
}

CISAPIExt::~CISAPIExt()
{
	Terminate();
}

BOOL CISAPIExt::Initialize()
{
	if(m_bIsInit)
		return TRUE;

	HRESULT hr = S_OK;

	CISAPIRequest::m_dwRequestTlsCookie = ::TlsAlloc();
	if(TLS_OUT_OF_INDEXES == CISAPIRequest::m_dwRequestTlsCookie)
		return FALSE;

	hr = m_pPostThreadPool.Initialize(this, -3);
	if(FAILED(hr))
	{
		TlsFree(CISAPIRequest::m_dwRequestTlsCookie);
		return FALSE;
	}

	hr = m_pGetThreadPool.Initialize(this, -1);
	if(FAILED(hr))
	{
		m_pPostThreadPool.Shutdown();
		TlsFree(CISAPIRequest::m_dwRequestTlsCookie);
		return FALSE;
	}

	hr = CXMLUtil::m_pClassFactory->CreateInstance(NULL, IID_IXMLDOMDocument, (void**)&m_Doc);
	m_bIsInit = TRUE;

	return TRUE;
}

VOID CISAPIExt::Terminate()
{
	if(m_bIsInit)
	{
		try
		{
			m_pPostThreadPool.Shutdown(1000);
			m_pGetThreadPool.Shutdown(1000);
			TlsFree(CISAPIRequest::m_dwRequestTlsCookie);
		}
		catch(...)
		{
		}
		m_bIsInit = FALSE;
	}
}

VOID CISAPIExt::Execute(CISAPIRequest *pRequest, OVERLAPPED *pOverlapped)
{
	try //reassurance
	{
		//Set request
		if(!TlsSetValue(CISAPIRequest::m_dwRequestTlsCookie, reinterpret_cast<LPVOID>(pRequest)))
		{
			pRequest->SetHttpStatus(500);
			EndRequest(pRequest);
			return;
		}


		//command processing
		switch(pRequest->m_RequestType)
		{
		case CISAPIRequest::rtPOST:
			if(!ProcessCommand(pRequest, pOverlapped))
			{
				EndRequest(pRequest);
				return;
			}
			return;

		case CISAPIRequest::rtGET:
			pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB->ConnID, HSE_REQ_SET_FLUSH_FLAG,
				(LPVOID)TRUE, NULL, NULL);

			pRequest->m_imRequestType = CISAPIRequest::imrtAlive;

#ifdef _IBN_PERFORMANCE_MONITOR
			if (CCounter::GetHasRealTimeMonitoring())
			{
				DWORD dwCounter = ::InterlockedIncrement((LONG*)CCounter::m_ulCrAlive);
				if(dwCounter > *CCounter::m_ulMaxAlive)
					::InterlockedExchange((LONG*)CCounter::m_ulMaxAlive, dwCounter);
			}
#endif

			if(!SessionUpdate(pRequest))
			{
				EndRequest(pRequest);
				return;
			}
			else
			{
				try
				{
					delete pRequest;
				}
				catch(...)
				{
				}
			}
			return;

		default:
			break;
		}
	}
	catch(...)//reassurance
	{
	}

	try
	{
		//ATLASSERT(FALSE);
		pRequest->SetHttpStatus(500);
		EndRequest(pRequest);
	}
	catch(...)
	{
	}
}



BOOL CISAPIExt::AddRequest(LPEXTENSION_CONTROL_BLOCK pECB)
{
	BOOL retVal = FALSE;

	if(m_bIsInit && pECB != NULL)
	{
		CISAPIRequest* pRequest = new CISAPIRequest; // TODO: suspicious
		if(pRequest != NULL)
		{
			HRESULT hr = pRequest->Init();
			if(SUCCEEDED(hr))
			{
				pRequest->m_pECB = pECB;
				pRequest->m_lpISAPIExt = this;

				//Unpack request
				if(UnpackHeader(pRequest))
				{
					if(pRequest->m_RequestType == CISAPIRequest::rtPOST)
						retVal = m_pPostThreadPool.QueueRequest(pRequest);
					else
						retVal = m_pGetThreadPool.QueueRequest(pRequest);
				}
			}

			if(!retVal)
			{
				try
				{
					delete pRequest;
				}
				catch(...)
				{
				}
			}
		}
	}

	return retVal;
}

BOOL CISAPIExt::UnpackHeader(CISAPIRequest* pRequest)
{
	BOOL retVal = FALSE;
	try
	{
		BOOL ok = FALSE;
		//Check method
		if(strcmp(pRequest->m_pECB->lpszMethod, "GET") == 0)
		{
			pRequest->m_RequestType = CISAPIRequest::rtGET;

			if(pRequest->m_pECB->cbTotalBytes == 0)
				ok = TRUE;
		}
		else if(strcmp(pRequest->m_pECB->lpszMethod, "POST") == 0)
		{
			pRequest->m_RequestType = CISAPIRequest::rtPOST;
			ok = TRUE;
		}

		if(ok)
		{
			ok = FALSE;

			//Check user agent field
			DWORD dwBufferSize = 100;
			CHAR* pBuffer = new CHAR[dwBufferSize];
			if(pBuffer != NULL)
			{
				SecureZeroMemory(pBuffer, dwBufferSize);

				if(pRequest->m_pECB->GetServerVariable(pRequest->m_pECB->ConnID, "HTTP_USER_AGENT", pBuffer, &dwBufferSize))
				{
					if(strcmp(pBuffer, "Mediachase IM") != 0)
					{
						SendRedirect(pRequest, REDIRECT_URL);
						retVal = TRUE;
					}
					else
						ok = TRUE;
				}

				if(ok)
				{
					//Check SESSION_ID field
					if(pRequest->m_RequestType == CISAPIRequest::rtPOST)
						pRequest->m_bSessionIDFound = GetCustomHeader(pRequest, _T("HTTP_SESSION_ID"), pRequest->m_szSessionID, 37);

					//check KeepAlive 
					dwBufferSize = 100;
					if(pRequest->m_pECB->GetServerVariable(pRequest->m_pECB->ConnID, "HTTP_CONNECTION", pBuffer, &dwBufferSize))
					{
						if(strcmp((const char*)pBuffer, "Keep-Alive") == 0)
							pRequest->m_bKeepAlive = TRUE;
					}

					retVal = TRUE;
				}

				delete[] pBuffer;
			}
		}
	}
	catch(...)
	{
		retVal = FALSE;
	}

	if(!retVal)
		pRequest->SetHttpStatus(500);

	return retVal;
}

BOOL CISAPIExt::SessionUpdate(CISAPIRequest* pRequest)
{
	LONG nUserID;
	try
	{
		if(strlen(pRequest->m_pECB->lpszQueryString) == 47 && pRequest->m_pECB->lpszQueryString[10] == '=')
		{
			_tcscpy_s(pRequest->m_szSessionID, 37, CA2T(&pRequest->m_pECB->lpszQueryString[11]));
		}
		else
			return pRequest->SetError(ERR_WRONG_SID);
	}
	catch(...)
	{
		return pRequest->SetHttpStatus(500);
	}

	try
	{
		try
		{
			g_ActiveSessions.GetIDbySID(pRequest->m_szSessionID, nUserID);
		}
		catch(long err)
		{
			return pRequest->SetError(err);
		}

		if(!SendHeaders(pRequest))
			return FALSE;

		g_ActiveSessions.UpdateConnection(nUserID, pRequest->m_pECB, TRUE);
		return TRUE;
	}
	catch(...)
	{
		return FALSE;
	}
}

VOID CISAPIExt::SendRedirect(CISAPIRequest* pRequest, LPSTR szURL)
{
	DWORD dwSize = (DWORD)strlen(szURL);
	if(!pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB, HSE_REQ_SEND_URL, szURL, &dwSize, NULL))
	{
		pRequest->SetError(201);
		return;
	}
	pRequest->m_RequestState = CISAPIRequest::rsEnd;
	pRequest->m_HeaderSent = TRUE;
	pRequest->m_bKeepAlive = FALSE;

}

VOID CISAPIExt::EndRequest(CISAPIRequest* pRequest)
{
	if(!pRequest->m_HeaderSent)
		SendHeaders(pRequest);


	LPVOID pKeepAlive = NULL;

	DWORD KeepAlive = HSE_STATUS_SUCCESS_AND_KEEP_CONN;
	if(pRequest->m_bKeepAlive)
		pKeepAlive = &KeepAlive;

	try
	{
		pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB->ConnID, HSE_REQ_DONE_WITH_SESSION, pKeepAlive, NULL, NULL);
	}
	catch(...)
	{
	}

	try
	{
		delete pRequest;
	}
	catch(...)
	{
	}
}

BOOL CISAPIExt::ProcessCommand(CISAPIRequest* pRequest, OVERLAPPED*)
{
	HRESULT hr = S_OK;

	if(pRequest->m_dwAsynchError != 0)
		return pRequest->SetError(ERR_UNKNOW);

	if(pRequest->m_RequestState == CISAPIRequest::rsReadHeader)
	{
		if((pRequest->m_pECB->cbTotalBytes > MAX_COMMANDSIZE ||
			pRequest->m_pECB->cbAvailable < pRequest->m_pECB->cbTotalBytes)
			&& !pRequest->m_bSessionIDFound)
			return pRequest->SetError(ERR_WRONG_XML);

		//--------------------------------------------------------------------------------
		//recieve file
		if(pRequest->m_bSessionIDFound)//file found
		{
#ifdef _IBN_PERFORMANCE_MONITOR
			pRequest->m_imRequestType = CISAPIRequest::imrtSendFile;
			if (CCounter::GetHasRealTimeMonitoring())
			{
				DWORD dwCounter = ::InterlockedIncrement((LONG*)CCounter::m_ulCrSendFile);
				if(dwCounter > *CCounter::m_ulMaxSendFile)
					::InterlockedExchange((LONG*)CCounter::m_ulMaxSendFile,dwCounter);
			}
#endif
			CHAR  szSize[6];
			DWORD dwSize;

			strncpy_s(szSize, 6, (const char *)pRequest->m_pECB->lpbData, 5);
			dwSize = atol(szSize);

			if(dwSize< 15 || dwSize > MAX_COMMANDSIZE)
				return pRequest->SetError(ERR_WRONG_XML);

			//write data to XML
			hr = CXMLUtil::MEM2XML(pRequest->m_pXMLDoc,
				pRequest->m_pECB->lpbData+5,
				dwSize);
			if(hr != NULL)
				return pRequest->SetError(ERR_WRONG_XML);

			if(!UnPackCommand(pRequest))
				return FALSE;
			//PRocessing
			try
			{
				g_ActiveSessions.GetIDbySID(&pRequest->m_szSessionID[0],pRequest->m_nUserID);
				g_SupportClass.z_ReceiveFileFirstStep(dwSize);
			}
			catch(long err)
			{
				return pRequest->SetError(err);
			}
			catch(...)
			{
				return pRequest->SetError(ERR_UNKNOW);
			}

			pRequest->m_RequestState = CISAPIRequest::rsReadClient;

			// Move Files storage to DB [1/7/2004]
			try
			{
				pRequest->m_hIncomingFile = CDBFile::CreateFile(pRequest->m_bsFID);
			}
			catch(...)
			{
				return pRequest->SetError(ERR_UNKNOW);
			}

			/*pRequest->m_hIncomingFile = ::CreateFileA(pRequest->m_szFileORurl,
			GENERIC_WRITE,
			NULL,
			NULL,
			CREATE_ALWAYS,
			FILE_FLAG_SEQUENTIAL_SCAN,
			NULL);
			if(pRequest->m_hIncomingFile == INVALID_HANDLE_VALUE)
			{
			pRequest->m_hIncomingFile = NULL;
			return pRequest->SetError(ERR_UNKNOW);
			}*/

			pRequest->m_dwReadDataSize = pRequest->m_pECB->cbAvailable;
			DWORD dwSizeToWrite = pRequest->m_pECB->cbAvailable - dwSize - 5;
			//DWORD dwSizeWritten;

			try
			{
				CDBFile::WriteFile(pRequest->m_hIncomingFile, (PBYTE)pRequest->m_pECB->lpbData + dwSize+5,dwSizeToWrite);
			}
			catch (...) 
			{
				return pRequest->SetError(ERR_UNKNOW);
			}

			/*
			if(!::WriteFile(pRequest->m_hIncomingFile,
			(PBYTE)pRequest->m_pECB->lpbData + dwSize+5,
			dwSizeToWrite,
			&dwSizeWritten,
			NULL))
			return pRequest->SetError(ERR_UNKNOW);
			*/


			if(pRequest->m_dwReadDataSize < pRequest->m_pECB->cbTotalBytes)
			{
				if(!SetCompletionCallback(pRequest))
					return pRequest->SetError(ERR_UNKNOW);

				pRequest->SetIncomingBuffer();

				pRequest->m_dwReadingSize = MAX_COMMANDSIZE;
				DWORD dwFlag = HSE_IO_ASYNC;

				if(!pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB, HSE_REQ_ASYNC_READ_CLIENT,
					pRequest->m_pbIncomingBuffer,
					&pRequest->m_dwReadingSize,
					&dwFlag))
					return pRequest->SetError(ERR_UNKNOW);

				return TRUE;
			}
			else
				pRequest->m_dwReadingSize = 0;
		}

		//---------------------------------------------------------------------------------
		//
		else //command
		{
			//write data to XML
			hr = CXMLUtil::MEM2XML(pRequest->m_pXMLDoc,
				pRequest->m_pECB->lpbData,
				pRequest->m_pECB->cbAvailable);
			if(hr != S_OK)
				return pRequest->SetError(ERR_WRONG_XML);
			//Processing
			if(!UnPackCommand(pRequest))
				return FALSE;

			try
			{
				//if logon
				if(pRequest->m_bsCommand == CComBSTR(L"c_logon"))
				{
					g_SupportClass.z_logon(pRequest);
				}
				else
				{
					g_ActiveSessions.GetIDbySID(&pRequest->m_szSessionID[0], pRequest->m_nUserID);
					if(pRequest->m_bsCommand == CComBSTR(L"c_receive_file"))
					{
#ifdef _IBN_PERFORMANCE_MONITOR
						pRequest->m_imRequestType = CISAPIRequest::imrtRcvFile;
						if (CCounter::GetHasRealTimeMonitoring())
						{
							DWORD dwCounter = ::InterlockedIncrement((LONG*)CCounter::m_ulCrRcvFile);
							if(dwCounter > *CCounter::m_ulMaxRcvFile)
								::InterlockedExchange((LONG*)CCounter::m_ulMaxRcvFile, dwCounter);
						}
#endif
						g_SupportClass.z_SendFileFirstStep();
						SendRedirect(pRequest, pRequest->m_szFileORurl);
						return FALSE;
					}
#ifdef _IBN_PERFORMANCE_MONITOR
					pRequest->m_imRequestType = CISAPIRequest::imrtCommand;
					if (CCounter::GetHasRealTimeMonitoring())
					{
						DWORD dwCounter = ::InterlockedIncrement((LONG*)CCounter::m_ulCrCommand);
						if(dwCounter > *CCounter::m_ulMaxCommand)
							::InterlockedExchange((LONG*)CCounter::m_ulMaxCommand, dwCounter);
					}
#endif
					g_SupportClass.Logic();
				}
			}
			catch(long ErrorCode)
			{
				return pRequest->SetError(ErrorCode);
			}
			catch(...)
			{
				ATLASSERT(FALSE);
				return pRequest->SetError(ERR_UNKNOW);
			}
			pRequest->m_RequestState = CISAPIRequest::rsSendHeader;
		}


	} // if rsReadClient
	//====================================================================================
	//read
	if(pRequest->m_RequestState == CISAPIRequest::rsReadClient)
	{
		pRequest->m_dwReadDataSize += pRequest->m_dwReadingSize;
		if(pRequest->m_dwReadingSize >0)
		{
			try
			{
				CDBFile::WriteFile(pRequest->m_hIncomingFile, pRequest->m_pbIncomingBuffer, pRequest->m_dwReadingSize);
			}
			catch (...) 
			{
				return pRequest->SetError(ERR_UNKNOW);
			}
			/*DWORD dwSizeWritten;
			if(!::WriteFile(pRequest->m_hIncomingFile,
			pRequest->m_pbIncomingBuffer,
			pRequest->m_dwReadingSize,
			&dwSizeWritten,
			NULL))
			return pRequest->SetError(ERR_UNKNOW);*/
		}

		if(pRequest->m_dwReadDataSize < pRequest->m_pECB->cbTotalBytes)
		{
			pRequest->m_dwReadingSize = MAX_COMMANDSIZE;
			DWORD dwFlag = HSE_IO_ASYNC;

			if(!pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB, HSE_REQ_ASYNC_READ_CLIENT,
				pRequest->m_pbIncomingBuffer,
				&pRequest->m_dwReadingSize,
				&dwFlag))
				return pRequest->SetError(ERR_UNKNOW);
			return TRUE;
		}
		else
			if(pRequest->m_dwReadDataSize == pRequest->m_pECB->cbTotalBytes)
			{
				try
				{
					g_ActiveSessions.GetIDbySID(&pRequest->m_szSessionID[0], pRequest->m_nUserID);
					g_SupportClass.Logic();
				}
				catch(long ErrorCode)
				{
					return pRequest->SetError(ErrorCode);
				}
				catch(...)
				{
					ATLASSERT(FALSE);
					return pRequest->SetError(ERR_UNKNOW);
				}

				pRequest->m_RequestState = CISAPIRequest::rsSendHeader;
			}
			else
				return pRequest->SetError(ERR_UNKNOW);
	}
	//=====================================================================================
	//Write
	if(pRequest->m_RequestState == CISAPIRequest::rsSendHeader)
	{
		pRequest->m_dwSindingSize = 0;
		hr = pRequest->SetOutgoingBuffer();
		if(FAILED(hr)) return pRequest->SetError(ERR_UNKNOW);

		if(pRequest->m_dwOutgoingSize)
			if(!SetCompletionCallback(pRequest))
				return pRequest->SetError(ERR_UNKNOW);

		if(!SendHeaders(pRequest))
			return pRequest->SetError(ERR_UNKNOW);

		if(pRequest->m_dwOutgoingSize)
		{
			pRequest->m_RequestState = CISAPIRequest::rsWriteClient;

			pRequest->m_dwSentSize += pRequest->m_dwSindingSize;
			pRequest->m_dwSindingSize = pRequest->m_dwOutgoingSize - pRequest->m_dwSentSize;

			if(!pRequest->m_pECB->WriteClient(pRequest->m_pECB->ConnID,
				pRequest->m_lpOutgoingBuffer,
				&pRequest->m_dwSindingSize,
				HSE_IO_ASYNC))
				return pRequest->SetError(ERR_UNKNOW);

			return TRUE;
		}
		else
			return FALSE;
	}
	//======================================================================================
	//write 
	if(pRequest->m_RequestState == CISAPIRequest::rsWriteClient)
	{		
		pRequest->m_dwSentSize += pRequest->m_dwSindingSize;
		if(pRequest->m_dwSentSize < pRequest->m_dwOutgoingSize)
		{
			pRequest->m_dwSindingSize = pRequest->m_dwOutgoingSize - pRequest->m_dwSentSize;
			if(!pRequest->m_pECB->WriteClient(pRequest->m_pECB->ConnID,
				(PBYTE)pRequest->m_lpOutgoingBuffer + pRequest->m_dwSentSize,
				&pRequest->m_dwSindingSize,
				HSE_IO_ASYNC))
				return pRequest->SetError(ERR_UNKNOW);
		}	
		else
			return FALSE;
	}
	//======================================================================================
	//end

	return TRUE;
}

void* WINAPI CISAPIExt::PFN_HSE_IO_COMPLETION_CALLBACK1(LPEXTENSION_CONTROL_BLOCK, PVOID pContext, DWORD cbIO, DWORD dwError)
{
	try //reassurance
	{
		CISAPIRequest* pRequest = reinterpret_cast<CISAPIRequest*>(pContext);
		ATLASSERT(pRequest != NULL);
		if(pRequest != NULL)
		{
			pRequest->m_dwSindingSize = cbIO;
			pRequest->m_dwReadingSize = cbIO;
			pRequest->m_dwAsynchError = dwError;
			if(cbIO == 0)
				pRequest->m_dwAsynchError = ERROR_OPERATION_ABORTED;

			CISAPIExt* pThis = pRequest->m_lpISAPIExt;
			ATLASSERT(pThis != NULL);
			if(pThis != NULL)
				pThis->SendRequest(pRequest);
		}
	}
	catch(...)
	{
	} //reassurance

	return NULL;
}

BOOL CISAPIRequest::SetError(DWORD dwErrorCode)
{
	return ((m_dwErrorCode = dwErrorCode) == 200);
}

BOOL CISAPIRequest::SetHttpStatus(DWORD dwHttpStatus)
{
	return ((m_dwHTTPStatus = dwHttpStatus) == 200);
}

BOOL CISAPIExt::SetCompletionCallback(CISAPIRequest* pRequest)
{
	return pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB->ConnID, HSE_REQ_IO_COMPLETION,
		PFN_HSE_IO_COMPLETION_CALLBACK1,
		NULL,
		(LPDWORD)pRequest);
}

BOOL CISAPIExt::UnPackCommand(CISAPIRequest* pRequest)
{
	HRESULT hr = S_OK;
	CComBSTR bsSID;
	CComBSTR bsCommand;

	//Check SID 
	hr = CXMLUtil::GetTextByPath(pRequest->m_pXMLDoc, CComBSTR(L"packet/@sid"), &bsSID);

	if(hr != S_OK)
		return pRequest->SetError(ERR_WRONG_XML);
	if(bsSID.Length() != 36)
		return pRequest->SetError(ERR_WRONG_SID);

	_tcscpy_s(pRequest->m_szSessionID, 37, COLE2T((BSTR)bsSID));

	// check command
	hr = CXMLUtil::GetTextByPath(pRequest->m_pXMLDoc,
		CComBSTR(L"packet/request/@value"),
		&pRequest->m_bsCommand);
	if(hr != S_OK)
		return pRequest->SetError(ERR_WRONG_XML);

	//GetWork node
	hr = pRequest->m_pXMLDoc->selectSingleNode(CComBSTR(L"packet/request"),
		&pRequest->m_WorkNode);
	if(hr != S_OK)
		return pRequest->SetError(ERR_WRONG_XML);


	return TRUE;
}

BOOL CISAPIExt::SendHeaders(CISAPIRequest* pRequest)
{
	HSE_SEND_HEADER_EX_INFO SendHeaderExInfo;
	char szStatus[] = "200 OK";
	char szHeader[500];
	szHeader[0] = '\0';

	if(pRequest->m_HeaderSent)
		return TRUE;

	if(pRequest->m_dwErrorCode == 200 && pRequest->m_dwHTTPStatus == 200)
	{
		if(pRequest->m_RequestType == CISAPIRequest::rtGET)
		{
			SendHeaderExInfo.fKeepConn = pRequest->m_bKeepAlive;
			if(pRequest->m_bKeepAlive)
			{
				sprintf_s(szHeader, 500,
					HTTP_HEADER_CACHE RN
					HTTP_HEADER_PRAGMA RN
					HTTP_HEADER_TRANSFERENCODING RN
					HTTP_HEADER_KEEPALIVE RN
					HTTP_HEADER_IMERROR RN RN,
					200);
			}
			else
			{
				sprintf_s(szHeader, 500,
					HTTP_HEADER_CACHE RN
					HTTP_HEADER_PRAGMA RN
					HTTP_HEADER_TRANSFERENCODING RN
					HTTP_HEADER_IMERROR RN RN,
					200);
			}
		}
		else
		{
			SendHeaderExInfo.fKeepConn = pRequest->m_bsCommand != CComBSTR(L"c_logoff") 
				&& pRequest->m_bKeepAlive;
			if(SendHeaderExInfo.fKeepConn)
			{
				sprintf_s(szHeader, 500,
					HTTP_HEADER_CACHE RN
					HTTP_HEADER_PRAGMA RN
					HTTP_HEADER_CONTENTLENGTH RN
					HTTP_HEADER_CONTENTTYPE RN
					HTTP_HEADER_KEEPALIVE RN
					HTTP_HEADER_IMERROR RN RN,
					pRequest->m_dwOutgoingSize,"text/xml",200);
			}
			else
			{
				sprintf_s(szHeader, 500,
					HTTP_HEADER_CACHE RN
					HTTP_HEADER_PRAGMA RN
					HTTP_HEADER_CONTENTLENGTH RN
					HTTP_HEADER_CONTENTTYPE RN
					HTTP_HEADER_IMERROR RN RN,
					pRequest->m_dwOutgoingSize,"text/xml",200);
			}
		}
	}
	else if(pRequest->m_dwErrorCode != 200)
	{
		SendHeaderExInfo.fKeepConn = pRequest->m_bsCommand != CComBSTR(L"c_logon") 
			&& pRequest->m_bKeepAlive;
		if(SendHeaderExInfo.fKeepConn)
		{
			sprintf_s(szHeader, 500,
				HTTP_HEADER_CACHE RN
				HTTP_HEADER_PRAGMA RN
				HTTP_HEADER_CONTENTLENGTH RN
				HTTP_HEADER_CONTENTTYPE RN
				HTTP_HEADER_KEEPALIVE RN
				HTTP_HEADER_IMERROR RN RN,
				0,"text/xml",pRequest->m_dwErrorCode);
		}
		else
		{
			sprintf_s(szHeader, 500,
				HTTP_HEADER_CACHE RN
				HTTP_HEADER_PRAGMA RN
				HTTP_HEADER_CONTENTLENGTH RN
				HTTP_HEADER_CONTENTTYPE RN
				HTTP_HEADER_IMERROR RN RN,
				0,"text/xml",pRequest->m_dwErrorCode);
		}

	}
	else if(pRequest->m_dwHTTPStatus != 200)
	{
		SendHeaderExInfo.fKeepConn = FALSE;
		return FALSE;
	}

	SendHeaderExInfo.pszStatus = szStatus;
	SendHeaderExInfo.pszHeader = szHeader;
	SendHeaderExInfo.cchStatus = (DWORD)strlen(szStatus);
	SendHeaderExInfo.cchHeader = (DWORD)strlen(szHeader);

	pRequest->m_HeaderSent = TRUE;
	BOOL res = pRequest->m_pECB->ServerSupportFunction(pRequest->m_pECB->ConnID, HSE_REQ_SEND_RESPONSE_HEADER_EX,
		&SendHeaderExInfo,
		NULL,
		NULL);
	pRequest->m_bKeepAlive = SendHeaderExInfo.fKeepConn && res;

	return res;
}

BOOL CISAPIExt::SendRequest(CISAPIRequest *pRequest)
{
	if(!m_pPostThreadPool.QueueRequest(pRequest))
	{
		pRequest->SetHttpStatus(500);
		EndRequest(pRequest);
		return FALSE;
	}

	return TRUE;
}


BOOL CISAPIExt::GetCustomHeader(CISAPIRequest* pRequest, LPCTSTR szName, LPTSTR szValue, size_t valueSizeChars)
{
	BOOL ret = FALSE;

	LPSTR szAllHeadersAnsi = NULL;
	DWORD dwSize = 0;

	BOOL ok = pRequest->m_pECB->GetServerVariable(pRequest->m_pECB->ConnID, "ALL_HTTP", szAllHeadersAnsi, &dwSize);
	DWORD error = GetLastError();
	if(!ok && error == ERROR_INSUFFICIENT_BUFFER)
	{
		szAllHeadersAnsi = new CHAR[dwSize];
		if(szAllHeadersAnsi != NULL)
		{
			if(pRequest->m_pECB->GetServerVariable(pRequest->m_pECB->ConnID, "ALL_HTTP", szAllHeadersAnsi, &dwSize))
			{
				CA2T szAllHeaders(szAllHeadersAnsi);
				LPTSTR szAllHeadersEnd = szAllHeaders + dwSize;
				LPTSTR szCurrent = szAllHeaders;
				while(szCurrent < szAllHeadersEnd)
				{
					LPTSTR szHeaderEnd = _tcschr(szCurrent, '\n');
					if(szHeaderEnd != NULL)
					{
						LPTSTR szColon = _tcschr(szCurrent, ':');
						if(szColon < szHeaderEnd)
						{
							if(_tcsncmp(szCurrent, szName, szColon - szCurrent) == 0)
							{
								LPTSTR szValueStart = szColon + 1;
								_tcsncpy_s(szValue, valueSizeChars, szValueStart, szHeaderEnd - szValueStart);
								ret = TRUE;
								break;
							}
						}
						szCurrent = szHeaderEnd + 1;
					}
					else
						szCurrent = szAllHeadersEnd;
				}
			}
			delete[] szAllHeadersAnsi;
		}
	}

	return ret;
}
