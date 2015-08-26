// BaseNetManager.cpp: implementation of the CBaseNetManager class.
//
//////////////////////////////////////////////////////////////////////

//#define _MILSECOND 10000

#include "stdafx.h"
#include "BaseNetManager.h"
#include "time.h"
#include "IM_Net.h"
#include "atlenc-mc.h"
#define WM_UPLOAD_BEGIN WM_USER + 523
#define WM_UPLOAD_STEP  WM_USER + 524

extern long g_ByteSent;
extern long g_ByteReceived;
extern long g_LatestSent;
extern long g_LatestReceived;


//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CBaseNetManager::CBaseNetManager()
{
	m_hInternet = NULL;
	m_IsInit = FALSE;
	for(int k = 0; k< 5 ;k++)
	{
		m_arCallBackData[k].pChannel = (NET_CHANNEL_ENUM)k;
		m_arCallBackData[k].pBaseNetManager = this;
		m_TimeoutEvents[k] = CreateWaitableTimer(NULL,FALSE,NULL);
	}
	m_TimeoutEvents[6] = CreateWaitableTimer(NULL,FALSE,NULL);
	m_TimeoutEvents[7] = CreateWaitableTimer(NULL,FALSE,NULL);
	m_TimeoutEvents[5] = CreateEvent(NULL,FALSE,FALSE,NULL);

	DWORD ThreadId = NULL;
	HANDLE hThread = CreateThread(NULL,250000,TimeOutThreadProc,this,NULL,&ThreadId);
	CloseHandle(hThread);
	
}

CBaseNetManager::~CBaseNetManager()
{
	while(CBaseNetData::m_PendingOperationCount)
		Sleep(100);

	SetEvent(m_TimeoutEvents[5]);
	for(int k = 0; k< 8 ;k++)
	{
		CloseHandle(m_TimeoutEvents[k]);
	}
	Close();
	Sleep(100);
}	
void CALLBACK CBaseNetManager::InternetCallback12(
								HINTERNET hInternet,
								DWORD dwcontext,
								DWORD dwInternetStatus,
								LPVOID lpvStatusInformation,
								DWORD dwStatusInformationLength
								)
{
	INTERNET_ASYNC_RESULT* pINTERNET_ASYNC_RESULT = NULL;
	_CALL_BACK_DATA* pCallBackData;

	DWORD Count;
	DWORD l =0;
	DWORD i = 0;

	switch (dwInternetStatus)
	{
	case INTERNET_STATUS_REQUEST_COMPLETE:
		pCallBackData = (_CALL_BACK_DATA*)dwcontext;
		pINTERNET_ASYNC_RESULT = (INTERNET_ASYNC_RESULT*)lpvStatusInformation;
		pCallBackData->pBaseNetManager->CallBack(pCallBackData->pChannel,pINTERNET_ASYNC_RESULT);		
		// Some code.
		break;
	case INTERNET_STATUS_RESPONSE_RECEIVED:
		Count = *((DWORD*)lpvStatusInformation);
		g_AddReceivedBytes(Count);
		ATLTRACE("==========================INTERNET_STATUS_RESPONSE_RECEIVED %d \r\n",Count);	

		break;
	case INTERNET_STATUS_REQUEST_SENT:
		Count = *((DWORD*)lpvStatusInformation);
		g_AddSentBytes(Count);
	
		ATLTRACE("==========================INTERNET_STATUS_REQUEST_SENT %d \r\n",Count);
		break;
	default:

		ATLTRACE("==========================++============================================");
		break;
	}

};


long CBaseNetManager::Init(BASE_NET_MANAGER_CONFIG sConfig)
{
	ATLASSERT(!m_IsInit); 
	if(m_IsInit)
		return E_FAIL;

	m_BASE_NET_MANAGER_CONFIG = sConfig;
	
	TCHAR	pString[200];
	TCHAR*  szProxy = NULL;
	if(m_BASE_NET_MANAGER_CONFIG.m_dwAccessType == INTERNET_OPEN_TYPE_PROXY)
	{

		_stprintf(pString,_T("http=http://%s:%d"),
					m_BASE_NET_MANAGER_CONFIG.m_szProxyServerName,
					m_BASE_NET_MANAGER_CONFIG.m_ProxyServerPort);
		szProxy = pString;
		MCTRACE(0,"PROXY config == %s ==",pString);
	}

	m_hInternet = InternetOpen(_T("Mediachase IM"),
							   m_BASE_NET_MANAGER_CONFIG.m_dwAccessType,
							   szProxy,NULL,INTERNET_FLAG_ASYNC);
	long err = GetLastError();
	if(m_hInternet == NULL)
		return HRESULT_FROM_WIN32(GetLastError());

	DWORD MAXConn = 10;
	InternetSetOption(NULL,
					INTERNET_OPTION_MAX_CONNS_PER_SERVER ,
					&MAXConn,
					4
					);

	MAXConn = 10;
	InternetSetOption(NULL,
					INTERNET_OPTION_MAX_CONNS_PER_1_0_SERVER,
					&MAXConn,
					4
					);	
	/*
	if(m_BASE_NET_MANAGER_CONFIG.m_bSecure)
	{
		InternetSetOption(m_hInternet,
						  INTERNET_OPTION_PROXY_USERNAME,
						  m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin,
						  strlen(m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin) +1
						  );

		InternetSetOption(m_hInternet,
						  INTERNET_OPTION_PROXY_PASSWORD,
						  m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword,
						  strlen(m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword) +1
						  );
		MCTRACE(0,"PROXY config secure == %s == %s ==",m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin
			,m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword);
	}

  */
	//INTERNET_PROXY_INFO m_proxy;

	BYTE				m_buff[3048];DWORD size = 3048;
	InternetQueryOption(m_hInternet,INTERNET_OPTION_PROXY,&m_buff,&size);
#ifdef _DOLOG
	if(((INTERNET_PROXY_INFO*)m_buff)->dwAccessType != INTERNET_OPEN_TYPE_DIRECT)
	MCTRACE(0,"PROXY config == %s ==",((INTERNET_PROXY_INFO*)m_buff)->lpszProxy);
#endif

	//TODO SetTimeouts
	INTERNET_STATUS_CALLBACK dwISC;

	dwISC = InternetSetStatusCallback(m_hInternet,InternetCallback12);
	for(int k=0; k< 5 ;k++)
	{
		HINTERNET hConnect = InternetConnect(
											m_hInternet,
											m_BASE_NET_MANAGER_CONFIG.m_szServerName,
											m_BASE_NET_MANAGER_CONFIG.m_ServerPort,
											NULL,
											NULL,
											INTERNET_SERVICE_HTTP,
											NULL,
											NULL);
		//HANDLE m_hTimer = CreateWaitableTimer(NULL,FALSE,NULL);
		m_BaseNetData[k].Bind(hConnect,m_TimeoutEvents[k],(NET_CHANNEL_ENUM)k);
		if(m_BASE_NET_MANAGER_CONFIG.m_bUseSSL)
			m_BaseNetData[k].m_OpenRequestFlags |= INTERNET_FLAG_SECURE;
		else
			m_BaseNetData[k].m_OpenRequestFlags &= ~INTERNET_FLAG_SECURE;

		if(((INTERNET_PROXY_INFO*)m_buff)->dwAccessType == INTERNET_OPEN_TYPE_DIRECT)
			m_BaseNetData[k].m_OpenRequestFlags |= INTERNET_FLAG_KEEP_CONNECTION;
		else
			m_BaseNetData[k].m_OpenRequestFlags &= ~INTERNET_FLAG_KEEP_CONNECTION;
	}
//	m_intCount407 = 0;
	m_IsInit = true;
	return S_OK;
}

long CBaseNetManager::Close()
{
	if(m_IsInit)
	for(int k=0; k< 5 ;k++)
		m_BaseNetData[k].Unbind();

	if(m_hInternet != NULL)
	{
		InternetCloseHandle(m_hInternet);
		m_hInternet = NULL;
	}
	
	m_IsInit = FALSE;
	return S_OK;
}

long CBaseNetManager::StopAllOperations()
{
	for(int k =0; k<5 ; k++)
	{
		m_BaseNetData[k].m_bCancel = true;
		if(m_BaseNetData[k].m_hRequest != NULL)
		InternetCloseHandle(m_BaseNetData[k].m_hRequest);
		m_BaseNetData[k].m_hRequest = NULL;
	}
	return S_OK;
}

long CBaseNetManager::StopOperation(NET_CHANNEL_ENUM NetChannel)
{
	if(m_BaseNetData[NetChannel].m_hRequest != NULL)
	{
		m_BaseNetData[NetChannel].m_bCancel = true;
		if(m_BaseNetData[NetChannel].m_hRequest != NULL)
		InternetCloseHandle(m_BaseNetData[NetChannel].m_hRequest);
		m_BaseNetData[NetChannel].m_hRequest = NULL;
	}
	return S_OK;
}

long CBaseNetManager::StartOperation(NET_CHANNEL_ENUM NetChannel,LPCTSTR lpszSID, DWORD Handle, IStream* pStream, HANDLE hCallBackEvent, LPCTSTR FileName, NET_FILE_DIRECTION FileDirection, HWND CallBackHWND)
{									
	ATLASSERT(m_BaseNetData[NetChannel].m_ChannelState == NET_CHANNEL_STATE_BEGIN ||
			  m_BaseNetData[NetChannel].m_ChannelState != NET_CHANNEL_STATE_BEGIN);

	if(m_BaseNetData[NetChannel].m_ChannelState == NET_CHANNEL_STATE_END)
	   m_BaseNetData[NetChannel].Reset();

	if(m_BaseNetData[NetChannel].m_ChannelState != NET_CHANNEL_STATE_BEGIN)
		return E_PENDING;

	m_BaseNetData[NetChannel].m_CurrentHandles = Handle;
	m_BaseNetData[NetChannel].SetData(lpszSID,
									  pStream,
									  hCallBackEvent,
									  NULL,
									  FileDirection,
									  FileName,
									  CallBackHWND);
	
	InetProcessing(NetChannel);
	return S_OK;
}

long CBaseNetManager::StartEventReceiver(LPCTSTR lpszSID,HANDLE hEndCallBackEvent,HANDLE hNewEventCallBackEvent)
{
	ATLASSERT(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_ChannelState == NET_CHANNEL_STATE_BEGIN ||
			  m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_ChannelState != NET_CHANNEL_STATE_BEGIN);
	
	if(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_ChannelState == NET_CHANNEL_STATE_END)
		m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].Reset();
	
	if(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_ChannelState != NET_CHANNEL_STATE_BEGIN)
		return E_PENDING;

	m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].SetData(lpszSID,
													  NULL,
													  hEndCallBackEvent,
													  hNewEventCallBackEvent);
	
	InetProcessing(NET_EVENT_RECEIVER_CHANNEL);	
	return S_OK;
}


long CBaseNetManager::StopEventReceiver()
{
	if(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_hRequest != NULL)
	{
		m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_bCancel = true;
		if(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_hRequest != NULL)
		InternetCloseHandle(m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_hRequest);
		m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].m_hRequest = NULL;
	}
	return S_OK;
}


long CBaseNetManager::GetNextEvent(IStream **lpStream)
{
	return m_BaseNetData[NET_EVENT_RECEIVER_CHANNEL].CloneInStream(lpStream);
}

void CBaseNetManager::GetResultError(NET_CHANNEL_ENUM emChannel, DWORD &ErrorType, DWORD &ErrorCode)
{
	ErrorCode = m_BaseNetData[emChannel].m_ErrorCode;
	ErrorType = m_BaseNetData[emChannel].m_ErrorType;
}

void CBaseNetManager::GetResult(NET_CHANNEL_ENUM emChannel, IStream **lpStream)
{
	HRESULT hr = S_OK;
	hr = m_BaseNetData[emChannel].CloneInStream(lpStream);
}


DWORD WINAPI CBaseNetManager::ThreadProc(LPVOID lpParameter)
{
	_CALL_BACK_DATA* pCallBackData;
	pCallBackData = (_CALL_BACK_DATA*)lpParameter;

	pCallBackData->pBaseNetManager->WorkFunction(pCallBackData->pChannel);
	
	return 0;
}

void CBaseNetManager::WorkFunction(NET_CHANNEL_ENUM NetChannel)
{
/*
	BYTE	pBuff[1024];
	DWORD	dwSize = 1024;
	DWORD	dwReadSize;
	DWORD	dwWriteSize;
	DWORD	dwFullWriteSize = 0;
	INTERNET_ASYNC_RESULT pINTERNET_ASYNC_RESULT;
	if(m_BaseNetData[NetChannel].m_CallBackHWND!= 0 &&
	   m_BaseNetData[NetChannel].m_ChannelType == NET_FILESEND_CHANNEL)
		::PostMessage(m_BaseNetData[NetChannel].m_CallBackHWND,
					WM_UPLOAD_BEGIN,
					m_BaseNetData[NetChannel].m_CurrentHandles,
					m_BaseNetData[NetChannel].m_dwOutFileSize);
	
	do
	{

		if(!ReadFile(m_BaseNetData[NetChannel].m_hOutFile,
				 pBuff,
				 dwSize,
				 &dwReadSize,
				 NULL))
		{
			m_BaseNetData[NetChannel].SetEnd(etFILE,GetLastError());
			return;
		}
		if(dwReadSize == 0) break;

		if(!InternetWriteFile(m_BaseNetData[NetChannel].m_hRequest,
						pBuff,
						dwReadSize,
						&dwWriteSize))
		{
			//m_BaseNetData[NetChannel].SetEnd(etWININET,GetLastError());
			return;
		}
		
		dwFullWriteSize =+ dwWriteSize;
		if(m_BaseNetData[NetChannel].m_CallBackHWND!= 0 &&
			m_BaseNetData[NetChannel].m_ChannelType == NET_FILESEND_CHANNEL)
			::PostMessage(m_BaseNetData[NetChannel].m_CallBackHWND,
			WM_UPLOAD_STEP,
			m_BaseNetData[NetChannel].m_CurrentHandles,
			dwFullWriteSize);

	}
	while(true);
	pINTERNET_ASYNC_RESULT.dwError = 0;
	pINTERNET_ASYNC_RESULT.dwResult = 1;
	InternetCallback12(m_BaseNetData[NetChannel].m_hRequest,
					  (ULONG)&m_arCallBackData[NetChannel],
					  INTERNET_STATUS_REQUEST_COMPLETE,
					  &pINTERNET_ASYNC_RESULT,
					  sizeof(pINTERNET_ASYNC_RESULT));*/

}


long CBaseNetManager::InetProcessing(NET_CHANNEL_ENUM NetChannel)
{
	DWORD err;
	DWORD MAXConn;
/*
	etWININET,
	etSTATUS,
	etSERVER,
	etFILE,
	etCANCEL*/

//	time_t pp;
nextstep:
	MCTRACE(NetChannel+ 3,
		"CBaseNetManager::InetProcessing *State_dump_1*\r\n"
		"Channel = %d;\r\n"
		"State = %d;\r\n",
		NetChannel,
		m_BaseNetData[NetChannel].m_ChannelState);

switch(m_BaseNetData[NetChannel].m_ChannelState)
{
	//=====================================================================
	//Begin
	case NET_CHANNEL_STATE_BEGIN:
	
	if(NetChannel == NET_COMMAND_CHANNEL)
		InterlockedIncrement(&m_BaseNetData[NetChannel].haveMessage);
	MAXConn = 10;
	InternetSetOption(NULL,
					INTERNET_OPTION_MAX_CONNS_PER_SERVER ,
					&MAXConn,
					4
					);

	MAXConn = 10;
	InternetSetOption(NULL,
					INTERNET_OPTION_MAX_CONNS_PER_1_0_SERVER,
					&MAXConn,
					4
					);	

	
		TCHAR ObjectString[300];
		if(strlen(m_BaseNetData[NetChannel].GetObjectName()))
			_stprintf(ObjectString,_T("%s%s"),m_BASE_NET_MANAGER_CONFIG.m_szPath,m_BaseNetData[NetChannel].GetObjectName());
		else
			strcpy(ObjectString,m_BASE_NET_MANAGER_CONFIG.m_szPath);

		m_BaseNetData[NetChannel].m_hRequest = 
		HttpOpenRequest(m_BaseNetData[NetChannel].m_hConnect,
						m_BaseNetData[NetChannel].GetVerbs(),
						ObjectString,
						_T("HTTP/1.1"),
						NULL,
						NULL,
						m_BaseNetData[NetChannel].GetOpenRequestFlags(),
						(ULONG)&m_arCallBackData[NetChannel]);
		//if error
		if(m_BaseNetData[NetChannel].m_hRequest == NULL)
		{
			err = GetLastError();
			m_BaseNetData[NetChannel].SetEnd(etWININET,err);
			return S_OK;
		}
		
		m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_SEND;
		
		if(m_BASE_NET_MANAGER_CONFIG.m_bSecure)
		{

			int		HeaderLen = ATLMC::ProxyAuthorizationStringGetRequiredLength(
				m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin,m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword);
			LPSTR	strHeader = new CHAR[HeaderLen+1];
			ZeroMemory(strHeader,HeaderLen+1);

			HRESULT hr = ATLMC::ProxyAuthorizationString(
				m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin,m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword,strHeader,&HeaderLen);

//			ASSERT(hr==S_OK);

			HttpAddRequestHeaders(m_BaseNetData[NetChannel].m_hRequest, strHeader, HeaderLen,HTTP_ADDREQ_FLAG_ADD );

			delete []strHeader;


	/*	char* rangeHeader = "Proxy-Authorization: Basic ZXVnOjMyMzc4ODA5MDI=\r\n";
			DWORD dwBuffSize = strlen(rangeHeader);
		
		HttpAddRequestHeaders(m_BaseNetData[NetChannel].m_hRequest, rangeHeader, dwBuffSize,
           HTTP_ADDREQ_FLAG_ADD );   // calls HttpSendRequest again - see HttpDump
	*/
 
		}
				m_BaseNetData[NetChannel].BeginSendRequest();
			if(!HttpSendRequestEx(m_BaseNetData[NetChannel].m_hRequest,
								  m_BaseNetData[NetChannel].GetOutBuffer(),
								  NULL,
								  m_BaseNetData[NetChannel].GetSendRequestFlags(),
								  (ULONG)&m_arCallBackData[NetChannel]))
			{//if error
				err = GetLastError();
				if(ERROR_IO_PENDING != err)
				{
					m_BaseNetData[NetChannel].EndTimeOut();
					m_BaseNetData[NetChannel].SetEnd(etWININET,err);
					return S_OK;
				}
			}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				goto nextstep;
			}
		
	break;
	//=====================================================================
	//Write data
	case NET_CHANNEL_STATE_REQUEST_SEND:

		if(!m_BaseNetData[NetChannel].bDataSent)
		{
			if(m_BaseNetData[NetChannel].m_CallBackHWND!= 0
					&&m_BaseNetData[NetChannel].m_ChannelType == NET_FILESEND_CHANNEL)
					::PostMessage(m_BaseNetData[NetChannel].m_CallBackHWND,
					WM_UPLOAD_BEGIN,
					m_BaseNetData[NetChannel].m_CurrentHandles,
					m_BaseNetData[NetChannel].GetFullSize());

			DWORD	dwReadSize;
			m_BaseNetData[NetChannel].bDataSent = TRUE;
			LPVOID pTempBuff = m_BaseNetData[NetChannel].GetDataBuffer(dwReadSize);
			
			if(dwReadSize != 0)
			if(!InternetWriteFile(m_BaseNetData[NetChannel].m_hRequest,
								  pTempBuff,
								  dwReadSize,
								  &m_BaseNetData[NetChannel].m_dInwBufferSize))
			{//if error
				err = GetLastError();
				if(ERROR_IO_PENDING != err)
				{
					m_BaseNetData[NetChannel].EndTimeOut();
					m_BaseNetData[NetChannel].SetEnd(etWININET,err);
					return S_OK;
				}
			}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				goto nextstep;
			}
		}
		
		if(m_BaseNetData[NetChannel].m_ChannelType == NET_FILESEND_CHANNEL)
		{
			
			DWORD	dwReadSize;
			m_BaseNetData[NetChannel].m_dwFullWriteSize +=
				m_BaseNetData[NetChannel].m_dInwBufferSize;

			if(m_BaseNetData[NetChannel].m_dwFullWriteSize == 0)
			{
				
			}
			else
			{
			 
				 if(m_BaseNetData[NetChannel].m_CallBackHWND!= 0 &&
					m_BaseNetData[NetChannel].m_ChannelType == NET_FILESEND_CHANNEL)
					 ::PostMessage(m_BaseNetData[NetChannel].m_CallBackHWND,
					 WM_UPLOAD_STEP,
					 m_BaseNetData[NetChannel].m_CurrentHandles,
					 m_BaseNetData[NetChannel].m_dwFullWriteSize);
			}
 
			if(!ReadFile(m_BaseNetData[NetChannel].m_hOutFile,
				m_BaseNetData[NetChannel].m_pInBuffer,
				5024,
				&dwReadSize,// &m_BaseNetData[NetChannel].m_dInwBufferSize,
				NULL))
			{
				m_BaseNetData[NetChannel].SetEnd(etFILE,GetLastError());
				return S_OK;
			}
			if(dwReadSize == 0) 
			{
				m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_DATA_SEND;
				//memcpy(m_BaseNetData[NetChannel].m_pInBuffer,"\r\n",2);
				//dwReadSize = 2;
				goto nextstep;
				return S_OK;
			}
			
			m_BaseNetData[NetChannel].BeginWriteData();

			if(!InternetWriteFile(m_BaseNetData[NetChannel].m_hRequest,
								  m_BaseNetData[NetChannel].m_pInBuffer,
								  dwReadSize,
								  &m_BaseNetData[NetChannel].m_dInwBufferSize))
			{//if error
				err = GetLastError();
				if(ERROR_IO_PENDING != err)
				{
					m_BaseNetData[NetChannel].EndTimeOut();
					m_BaseNetData[NetChannel].SetEnd(etWININET,err);
					return S_OK;
				}
			}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				if(WaitChanel(NetChannel))
					goto nextstep;
				
			}
		
		}
		else
		{
			m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_DATA_SEND;
			goto nextstep;
		}
			

		break;
		
	//=====================================================================
	//End Request
	case NET_CHANNEL_STATE_REQUEST_DATA_SEND:
		
		m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_END;

			m_BaseNetData[NetChannel].BeginSendRequest();
				if(!HttpEndRequest(m_BaseNetData[NetChannel].m_hRequest,
								   NULL,
								   HSR_INITIATE | HSR_ASYNC,
								   (ULONG)&m_arCallBackData[NetChannel]))
				{
					err = GetLastError();
					if(ERROR_IO_PENDING != err)
					{
						m_BaseNetData[NetChannel].EndTimeOut();
						m_BaseNetData[NetChannel].SetEnd(etWININET,err);
						return S_OK;
					}					
				}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				goto nextstep;
				
			}


	break;
	//=====================================================================
	//Check Status and IM Error
	case NET_CHANNEL_STATE_REQUEST_END:
		//add log sent header
		g_AddSentBytes(180);

		if(!CheckStatus(NetChannel)) 
			break;
	//goto nextstep;
	//=====================================================================
	//Read response
	case NET_CHANNEL_STATE_RESPONSE_READ:
		if(!m_BaseNetData[NetChannel].WriteAnswer())
			break;
		if(m_BaseNetData[NetChannel].m_ChannelState == NET_CHANNEL_STATE_BEGIN)
		{
			goto nextstep;
			return S_OK;
		}
		//m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_RESPONSE_READ;
		
		m_BaseNetData[NetChannel].BeginReadData();
		if(!InternetReadFileEx(m_BaseNetData[NetChannel].m_hRequest,
						   m_BaseNetData[NetChannel].GetInBuffer(),
						   IRF_ASYNC | IRF_USE_CONTEXT | IRF_NO_WAIT,
						   (ULONG)&m_arCallBackData[NetChannel]))
		{
			
			err = GetLastError();
			m_BaseNetData[NetChannel].m_WriteData = FALSE;
			if(ERROR_IO_PENDING != err)
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				m_BaseNetData[NetChannel].SetEnd(etWININET,err);
				return S_OK;
			}
		}
		else 
		{
			m_BaseNetData[NetChannel].EndTimeOut();
			m_BaseNetData[NetChannel].m_WriteData = TRUE;
			if(WaitChanel(NetChannel))
					goto nextstep;
		}
			
	break;
	default:
		ATLASSERT(FALSE);
	break;
}
	return S_OK;
}

void CBaseNetManager::CallBack(NET_CHANNEL_ENUM NetChannel, INTERNET_ASYNC_RESULT *pResult)
{
	ATLTRACE("\r\nCallBack result = %d \r\n",pResult->dwResult);
	MCTRACE(NetChannel+ 3,
		"CBaseNetManager::CallBack *State_dump_1*\r\n"
		"Channel = %d;\r\n"
		"State = %d;\r\n"
		"Error = %d;\r\n"
		"Result = %d;\r\n",
		NetChannel,m_BaseNetData[NetChannel].m_ChannelState, pResult->dwError,pResult->dwResult);
	m_BaseNetData[NetChannel].EndTimeOut();

	//CheckStatus(NetChannel);

	// OZ [2008-04-25] Windows Authentication Addon
	if(pResult->dwError == 12032)
	{
		/*DWORD dwError = ::InternetErrorDlg(::GetDesktopWindow(), m_BaseNetData[NetChannel].m_hRequest, 
			ERROR_INTERNET_INCORRECT_PASSWORD,
            FLAGS_ERROR_UI_FILTER_FOR_ERRORS    |
            FLAGS_ERROR_UI_FLAGS_CHANGE_OPTIONS |
            FLAGS_ERROR_UI_FLAGS_GENERATE_DATA,
            NULL);*/

			m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_SEND;
			m_BaseNetData[NetChannel].bDataSent = FALSE;
			m_BaseNetData[NetChannel].BeginSendRequest();

			if(!HttpSendRequestEx(m_BaseNetData[NetChannel].m_hRequest,
								  m_BaseNetData[NetChannel].GetOutBuffer(),
								  NULL,
								  m_BaseNetData[NetChannel].GetSendRequestFlags(),
								  (ULONG)&m_arCallBackData[NetChannel]))
			{//if error
				HRESULT err = GetLastError();
				if(ERROR_IO_PENDING != err)
				{
					m_BaseNetData[NetChannel].EndTimeOut();
					m_BaseNetData[NetChannel].SetEnd(etWININET,err);
					return;
				}
			}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
			}

			return;
	}
	else 
	// End Addon
	if(pResult->dwError != 0)
	{
		m_BaseNetData[NetChannel].SetEnd(etWININET,pResult->dwError);
		return;
	}

/*
	if(NetChannel == NET_FILESEND_CHANNEL 
	   || NetChannel == NET_FILERECEIVE_CHANNEL)
	{
		__int64         qwDueTime;
		LARGE_INTEGER   liDueTime;
		qwDueTime = -1 * 10000 * 200;
	
		// Copy the relative time into a LARGE_INTEGER.
		liDueTime.LowPart  = (DWORD) ( qwDueTime & 0xFFFFFFFF );`
		liDueTime.HighPart = (LONG)  ( qwDueTime >> 32 );
	
		BOOL res = SetWaitableTimer(m_TimeoutEvents[4+NetChannel],&liDueTime,0,NULL,NULL,0);
	}
	else*/


	InetProcessing(NetChannel);
}

BOOL CBaseNetManager::WriteFile(NET_CHANNEL_ENUM enChannel)
{
	DWORD  dwThreadId;
	HANDLE hTherad;
	hTherad = CreateThread(NULL,
						  512000,
						  ThreadProc,
						  (LPVOID)&m_arCallBackData[enChannel],
						  NULL,&dwThreadId);
	if(hTherad == NULL) return FALSE;
	CloseHandle(hTherad);
	return true;
}


BOOL CBaseNetManager::CheckStatus(NET_CHANNEL_ENUM NetChannel)
{
	DWORD dwHTTPStatus;
	DWORD dwBufferSize = 4;
//	DWORD dwOutSize;
	if(!HttpQueryInfo(m_BaseNetData[NetChannel].m_hRequest,
					 HTTP_QUERY_STATUS_CODE | HTTP_QUERY_FLAG_NUMBER,
				     &dwHTTPStatus,
				     &dwBufferSize,
				     NULL))
	{
		m_BaseNetData[NetChannel].SetEnd(etWININET,GetLastError());
		return false;
	}
	


	dwBufferSize = 0;
	HttpQueryInfo(m_BaseNetData[NetChannel].m_hRequest,
					 HTTP_QUERY_RAW_HEADERS_CRLF,
				     NULL,
				     &dwBufferSize,
				     NULL);

	g_AddReceivedBytes(dwBufferSize+2);

//========================================================================
	if((dwHTTPStatus > 300 && dwHTTPStatus < 303) && NetChannel == NET_COMMAND_CHANNEL)
	{
			TCHAR	lpBuff[1024];
//			TCHAR	*Status,*End;
			dwBufferSize = 1024;
		if(!HttpQueryInfo(m_BaseNetData[NetChannel].m_hRequest,
					HTTP_QUERY_LOCATION,
				    &lpBuff,
					&dwBufferSize,
				    NULL))
		{	
			m_BaseNetData[NetChannel].SetEnd(etWININET,GetLastError());
			return false;
		}

		//TODO
		//change server or url
		URL_COMPONENTS m_URL_COMPONENTS = {0};
	//	TCHAR	lpHost[1024];
		TCHAR	lpPath[1024];

		m_URL_COMPONENTS.dwStructSize = sizeof(URL_COMPONENTS);
		//m_URL_COMPONENTS.dwHostNameLength = 1024;
		//m_URL_COMPONENTS.lpszHostName = lpHost;
		m_URL_COMPONENTS.dwUrlPathLength = 1024;
		m_URL_COMPONENTS.lpszUrlPath = lpPath;

		LONG err = InternetCrackUrl(lpBuff,dwBufferSize,0,&m_URL_COMPONENTS);

		
		strcpy(m_BASE_NET_MANAGER_CONFIG.m_szPath, lpPath);
		//strcpy(CIM_Net::m_BASE_NET_MANAGER_CONFIG.m_szServerName,lpHost);
		//m_IsInit = false;
		//Init(CIM_Net::m_BASE_NET_MANAGER_CONFIG);
		
		m_BaseNetData[NetChannel].SetEnd(etSTATUS,dwHTTPStatus);
		return false;
	}

	

//========================================================================
/*
	if(dwHTTPStatus == 407)
	{

		m_intCount407++;
			m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_SEND;
		
			m_BaseNetData[NetChannel].BeginSendRequest();
			if(!HttpSendRequestEx(m_BaseNetData[NetChannel].m_hRequest,
								  m_BaseNetData[NetChannel].GetOutBuffer(),
								  NULL,
								  m_BaseNetData[NetChannel].GetSendRequestFlags(),
								  (ULONG)&m_arCallBackData[NetChannel]))
			{//if error
				long err = GetLastError();
				if(ERROR_IO_PENDING != err)
				{
					m_BaseNetData[NetChannel].EndTimeOut();
					m_BaseNetData[NetChannel].SetEnd(etWININET,err);
					m_BaseNetData[NetChannel].m_ChannelState = NET_CHANNEL_STATE_REQUEST_SEND;
					return true;
				}
				return false;
			}
			else
			{
				m_BaseNetData[NetChannel].EndTimeOut();
				return false;
			}
	}

*/
	if(dwHTTPStatus != 200)
	{
		m_BaseNetData[NetChannel].SetEnd(etSTATUS,dwHTTPStatus);
		return false;
	}
	dwBufferSize =4;
	HttpQueryInfo(m_BaseNetData[NetChannel].m_hRequest,
		HTTP_QUERY_CONTENT_LENGTH| HTTP_QUERY_FLAG_NUMBER,
		&m_BaseNetData[NetChannel].m_dwContentLength,
		&dwBufferSize,
		NULL);
	
	TCHAR	lpBuff[1024];
	TCHAR	*Status,*End;
	dwBufferSize = 1024;
	if(!HttpQueryInfo(m_BaseNetData[NetChannel].m_hRequest,
		HTTP_QUERY_RAW_HEADERS_CRLF ,
		&lpBuff,
		&dwBufferSize,
		NULL))
	{
		m_BaseNetData[NetChannel].SetEnd(etWININET,GetLastError());
		return false;
	}
	Status = strstr(lpBuff,_T("IMErrorCode"));
	if(Status != 0)
	{
		Status += 13;
		End = strstr(Status,_T("\r\n"));
		*End = '\0';
		dwHTTPStatus = _ttol(Status);
		if(dwHTTPStatus != 200)
		{
			m_BaseNetData[NetChannel].SetEnd(etSERVER,dwHTTPStatus);
			return false;
		}
		return true;
	}
	if(Status == 0 && m_BaseNetData[NetChannel].m_ChannelType == NET_FILERECEIVE_CHANNEL)
		return true;
	m_BaseNetData[NetChannel].SetEnd(etWININET,GetLastError());
	return false;
}

long CBaseNetManager::Reconfig(BASE_NET_MANAGER_CONFIG sConfig)
{
	return S_OK;
}

DWORD WINAPI CBaseNetManager::TimeOutThreadProc(LPVOID lpParameter)
{
	CBaseNetManager* Class;
	Class = (CBaseNetManager*)lpParameter;
	return Class->TimeOut(lpParameter);
	
}

DWORD CBaseNetManager::TimeOut(LPVOID param)
{
	while(true)
	{
		DWORD kk = WaitForMultipleObjects(8,m_TimeoutEvents,FALSE,INFINITE);
		if(kk == 5) break;

		if(kk == 6 || kk == 7)
		{
			InetProcessing((NET_CHANNEL_ENUM)(kk - 4));
		}
		else
		if(m_BaseNetData[kk].m_hRequest != NULL)
		{
			try
			{
				InternetCloseHandle(m_BaseNetData[kk].m_hRequest);
				m_BaseNetData[kk].m_hRequest = NULL;
			}catch(...)
			{
			}
		}
	}
	return S_OK;
}

BOOL CBaseNetManager::WaitChanel(NET_CHANNEL_ENUM NetChannel)
{
//	ATLASSERT((NetChannel == NET_FILESEND_CHANNEL 
//				|| NetChannel == NET_FILERECEIVE_CHANNEL));

	if((NetChannel == NET_FILESEND_CHANNEL 
		|| NetChannel == NET_FILERECEIVE_CHANNEL)
		&&(m_BaseNetData[NET_COMMAND_CHANNEL].haveMessage == 1))
	{
		__int64         qwDueTime;
		LARGE_INTEGER   liDueTime;
		
		qwDueTime = -1 * 10000 * 5000;	
		//else
		//qwDueTime = -1 * 10000 * 900;
	
		// Copy the relative time into a LARGE_INTEGER.
		liDueTime.LowPart  = (DWORD) ( qwDueTime & 0xFFFFFFFF );
		liDueTime.HighPart = (LONG)  ( qwDueTime >> 32 );
	
		BOOL res = SetWaitableTimer(m_TimeoutEvents[4+NetChannel],&liDueTime,0,NULL,NULL,0);
		return FALSE;
	}
		return TRUE;
}
