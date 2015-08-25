// BaseNetData.cpp: implementation of the CBaseNetData class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "BaseNetData.h"

#define WM_DOWNLOAD_BEGIN  WM_USER + 520 
#define WM_DOWNLOAD_STEP   WM_USER + 521
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
LONG CBaseNetData::m_PendingOperationCount = 0;

CBaseNetData::CBaseNetData()
{
	in_Buff = NULL;
	m_OpenRequestFlags = INTERNET_FLAG_KEEP_CONNECTION	|
						 INTERNET_FLAG_NO_CACHE_WRITE	|
						 INTERNET_FLAG_RELOAD			|
						 INTERNET_FLAG_PRAGMA_NOCACHE	|
						 INTERNET_FLAG_NO_UI			|
						 INTERNET_FLAG_NO_COOKIES		|
						 //INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTPS|
						 //INTERNET_FLAG_IGNORE_REDIRECT_TO_HTTP |
						 INTERNET_FLAG_IGNORE_CERT_CN_INVALID  |
						 INTERNET_FLAG_HYPERLINK;//|
						 //INTERNET_FLAG_SECURE;
	//Bind data
	m_hConnect = NULL;
	m_IsBind   = FALSE;
	
	//Internal data
	m_hRequest = NULL;
	m_hTimer = NULL;//CreateWaitableTimer(NULL,FALSE,NULL);
	Reset();
}

CBaseNetData::~CBaseNetData()
{
	if(in_Buff != NULL)
		delete[] in_Buff;
}

void CBaseNetData::Reset()
{
	haveMessage = 0;
	bDataSent = FALSE;
	m_hNewDataEvent  = NULL;					//только для NET_EVENT_RECEIVER_CHANNEL
	m_hCallbackEvent = NULL;					//событие завершения операции
	m_ChannelState   = NET_CHANNEL_STATE_BEGIN; //состояние канала
	m_ObjectName[0] = '\0';
//	m_OpenRequestFlags = FALSE;
	m_dwFullWriteSize = 0;
	m_dwFullReadSize = 0;
	m_dInwBufferSize = 0;
	//настройки файла
	m_CallBackHWND  = NULL;
	m_FileName[0]   = '\0';
	m_FileDirection = NET_FILE_NONE;
	
	//Input parameters
	m_pInStream.Release();
	m_hInFile = NULL;
	m_dwInFileSize = 0;
	
	memset(&m_InINET_BUFFERS,0,sizeof(m_InINET_BUFFERS));
	m_InINET_BUFFERS.dwStructSize = sizeof(m_InINET_BUFFERS);
	m_InINET_BUFFERS.lpvBuffer = m_pInBuffer;
	m_InINET_BUFFERS.dwBufferLength = 5024;
	m_FirstRead = true;
	
	//Output parameters
	//OUT STREAM
	m_pOutStream.Release();
	m_hOutGlobal = NULL;
	m_dwOutStreamBuff = NULL;
	m_dwOutStreamSize = NULL;
	
	//OUT FILE
	memset(&m_OutINET_BUFFERS,0,sizeof(m_OutINET_BUFFERS));
	m_OutINET_BUFFERS.dwStructSize = sizeof(m_OutINET_BUFFERS);
	m_hOutFile = NULL;
	m_dwOutFileSize = NULL;
	
	m_ErrorType = 0;
	m_ErrorCode = 0;
	m_bCancel = FALSE;
	if(m_hRequest != NULL)
	{
		InternetCloseHandle(m_hRequest);
		m_hRequest = NULL;
	}
	return;
}
LPCTSTR CBaseNetData::GetObjectName()
{
	return m_ObjectName;
}

DWORD CBaseNetData::GetOpenRequestFlags()
{
	if(m_ChannelType != NET_FILERECEIVE_CHANNEL)
		return  m_OpenRequestFlags | INTERNET_FLAG_NO_AUTO_REDIRECT;
	return m_OpenRequestFlags ;
}



LPCSTR CBaseNetData::GetHeaderBuffer(DWORD &dwBufferSize)
{
	if(m_ChannelType == NET_FILESEND_CHANNEL)
	{
		_stprintf(m_Header,_T("SESSION_ID: %s"),m_szSID);
		dwBufferSize = strlen(m_Header);
		return m_Header;
	}
	dwBufferSize = 0;
	return NULL;
}

DWORD CBaseNetData::GetFullSize()
{
	return m_dwOutStreamSize + m_dwOutFileSize;// + 2:m_dwOutStreamSize;
}	

LPVOID CBaseNetData::GetDataBuffer(DWORD &dwBufferSize)
{
	dwBufferSize = m_dwOutStreamSize;
	return m_dwOutStreamBuff;
}

LPCTSTR CBaseNetData::GetVerbs()
{
	if(m_ChannelType == NET_EVENT_RECEIVER_CHANNEL)
		return NULL;
	else
		return _T("POST");
}

DWORD CBaseNetData::GetSendRequestFlags()
{
	
	if(m_ChannelType == NET_FILESEND_CHANNEL)
		return /*HSR_CHUNKED | */HSR_ASYNC | HSR_USE_CONTEXT |HSR_INITIATE;
	else
		return HSR_ASYNC | HSR_USE_CONTEXT;
}

HRESULT CBaseNetData::SetStream(IStream *pStream)
{
	m_FirstRead = TRUE;
	ULARGE_INTEGER			uli;
	LARGE_INTEGER			li = {0, 0};
	ULARGE_INTEGER			uliR ={0,0},uliW = {0,0};
	HRESULT					hr = S_OK;
	CHAR					pSize[10];
	DWORD					size;
	if(m_hOutGlobal != 0)
	{
		GlobalUnlock(m_hOutGlobal);
		m_pOutStream.Release();
	}
	
	hr = pStream->Seek(li, STREAM_SEEK_END, &uli);
	hr = m_dwOutStreamSize = (int)uli.QuadPart;
	
	hr = pStream->Seek(li, STREAM_SEEK_SET, &uli);
	uli.LowPart = m_dwOutStreamSize;
	
	if(NET_FILESEND_CHANNEL == m_ChannelType)
	{
		hr = CreateStreamOnHGlobal(NULL,TRUE,&m_pOutStream);
		memset(&pSize[0],0,10);
		ltoa(m_dwOutStreamSize,&pSize[0],10);
		m_dwOutStreamSize += 5;
		hr = m_pOutStream->Write(&pSize[0],5,&size);
		hr = pStream->CopyTo(m_pOutStream,uli,&uliR,&uliW);
	}	
	else
		hr = pStream->Clone(&m_pOutStream);
		

	hr = GetHGlobalFromStream(m_pOutStream, &m_hOutGlobal);
	m_dwOutStreamBuff = GlobalLock(m_hOutGlobal);
	
	return S_OK;
}

HRESULT CBaseNetData::SetFile(LPCSTR FileName, NET_FILE_DIRECTION Direction, HWND CallBackHWND)
{
	m_FirstRead = TRUE;
	if(FileName == NULL)
		m_FileDirection = NET_FILE_NONE;
	else
		m_FileDirection = Direction;

	m_hInFile = NULL;
	m_dwInFileSize = 0;
	m_hOutFile = NULL;
	m_dwOutFileSize = 0;
	

	switch(Direction)
	{
	case NET_FILE_SEND:	
		m_hOutFile = CreateFile(FileName,GENERIC_READ, FILE_SHARE_READ|FILE_SHARE_WRITE ,NULL,OPEN_EXISTING,NULL,NULL);
		if(m_hOutFile == INVALID_HANDLE_VALUE) 
			return HRESULT_FROM_WIN32(GetLastError());
		m_dwOutFileSize = GetFileSize(m_hOutFile,NULL);
		break;
	case NET_FILE_RECEIVE:	
		m_hInFile = CreateFile(FileName,GENERIC_WRITE,NULL,NULL,CREATE_ALWAYS,NULL,NULL);
		if(m_hInFile == INVALID_HANDLE_VALUE) 
			return HRESULT_FROM_WIN32(GetLastError());
		break;
	default:
		break;
	} 
	return S_OK;
}

BOOL CBaseNetData::WriteAnswer()
{
	m_ChannelState = NET_CHANNEL_STATE_RESPONSE_READ;
	DWORD   dwWrite = 0;
	HRESULT hr = S_OK;
	if(!m_FirstRead)
	{
		switch(m_ChannelType)
		{
		case NET_EVENT_RECEIVER_CHANNEL:
			
			if(m_InINET_BUFFERS.dwBufferLength == 0)
			{
				m_ChannelState = NET_CHANNEL_STATE_BEGIN;
				m_FirstRead = true;
				if(m_hRequest)
				{
					InternetCloseHandle(m_hRequest);
					m_hRequest = NULL;
				}
				return TRUE;
			}
			else
			{
				if(m_WriteData)
					if(S_FALSE == WriteEventsBuffer((PBYTE)m_InINET_BUFFERS.lpvBuffer,m_InINET_BUFFERS.dwBufferLength))
					{
					
						m_ChannelState = NET_CHANNEL_STATE_BEGIN;
											m_FirstRead = true;
											if(m_hRequest)
											{
												InternetCloseHandle(m_hRequest);
												m_hRequest = NULL;
											}
						
					}
				return TRUE;
			}
			break;
		case NET_FILERECEIVE_CHANNEL:
			

			if(m_InINET_BUFFERS.dwBufferLength == 0)
			{
				SetEnd();
				return FALSE;
			}

			if(m_WriteData)
			{
				// New Addon 2005/06/21
				m_WriteData= FALSE;

				m_dwFullReadSize += m_InINET_BUFFERS.dwBufferLength;
			
			if(m_CallBackHWND!= 0 && m_InINET_BUFFERS.dwBufferLength != 0 &&
				m_ChannelType == NET_FILERECEIVE_CHANNEL)
				::PostMessage(m_CallBackHWND,
				WM_DOWNLOAD_STEP,
				m_CurrentHandles,
				m_dwFullReadSize);

				if(!WriteFile(m_hInFile,m_InINET_BUFFERS.lpvBuffer,m_InINET_BUFFERS.dwBufferLength,&dwWrite,NULL))
				{
					SetEnd(etFILE,GetLastError());
					return FALSE;
				}
			}
			break;
		default:
			if(m_InINET_BUFFERS.dwBufferLength == 0)
			{
				SetEnd();
				return FALSE;
			}
			
			if(m_WriteData)
				hr = m_pInStream->Write(m_InINET_BUFFERS.lpvBuffer,m_InINET_BUFFERS.dwBufferLength,&dwWrite);
			ATLTRACE("\r\nWriteAnswer result = %d \r\n",m_InINET_BUFFERS.dwBufferLength);
			ATLTRACE("\r\nWriteAnswer write = %d \r\n",dwWrite);
			
			return (m_WriteData)?dwWrite:true;
			break;
		}
		
	}
	else
	{   
		if(m_ChannelType != NET_EVENT_RECEIVER_CHANNEL
		&&m_ChannelType != NET_FILERECEIVE_CHANNEL)
		{
			m_pInStream.Release();
			hr = CreateStreamOnHGlobal(NULL,TRUE,&m_pInStream);
		}
		m_FirstRead = FALSE;

		if(m_dwFullReadSize == NULL)
			if(m_CallBackHWND!= 0 && m_ChannelType == NET_FILERECEIVE_CHANNEL)
				::PostMessage(m_CallBackHWND,
				WM_DOWNLOAD_BEGIN,
				m_CurrentHandles,
				m_dwContentLength);
	}
	return true;
}
LPINTERNET_BUFFERS CBaseNetData::GetInBuffer()
{
	memset(&m_InINET_BUFFERS,0,sizeof(m_InINET_BUFFERS));
	m_InINET_BUFFERS.dwBufferLength = 5024;
	//m_InINET_BUFFERS.dwBufferTotal = 5024;
	m_InINET_BUFFERS.dwStructSize = sizeof(m_InINET_BUFFERS);

	memset(m_pInBuffer,0,2*5024);
	//m_pInBuffer[0] = '\0'; 

	m_InINET_BUFFERS.lpvBuffer = m_pInBuffer;
	return &m_InINET_BUFFERS;
}

void CBaseNetData::SetEnd(DWORD ErrorType, DWORD ErrorCode)
{
	if(m_ChannelType == NET_COMMAND_CHANNEL)
		InterlockedDecrement(&haveMessage);

	if(m_hRequest != NULL)
	{
		InternetCloseHandle(m_hRequest);
		m_hRequest = NULL;
	}
	if(m_hOutFile != NULL)
	{
		CloseHandle(m_hOutFile);
		m_hOutFile = NULL;
	}
	if(m_hInFile)
	{
		CloseHandle(m_hInFile);
		if(ErrorType != NULL || ErrorCode != NULL)
		{
		//	DeleteFile();
		}
		m_hInFile = NULL;
	}
	m_ErrorCode = ErrorCode;
	m_ErrorType = ErrorType;
	if(m_bCancel && m_ErrorType != 0)
			m_ErrorType = etCANCEL;
	
	m_ChannelState = NET_CHANNEL_STATE_END;
	SetEvent(m_hCallbackEvent);
	InterlockedDecrement(&m_PendingOperationCount);
}

LPINTERNET_BUFFERS CBaseNetData::GetOutBuffer()
{
	if(m_ChannelType == NET_EVENT_RECEIVER_CHANNEL)
		return NULL;
	memset(&m_OutINET_BUFFERS,0,sizeof(m_OutINET_BUFFERS));
	m_OutINET_BUFFERS.lpcszHeader = (const char*)GetHeaderBuffer(m_OutINET_BUFFERS.dwHeadersLength);
	m_OutINET_BUFFERS.dwBufferTotal = GetFullSize();
	m_OutINET_BUFFERS.dwStructSize = sizeof(m_OutINET_BUFFERS);
	//m_OutINET_BUFFERS.lpvBuffer = GetDataBuffer(m_OutINET_BUFFERS.dwBufferLength);
	return &m_OutINET_BUFFERS;
}

long CBaseNetData::Bind(HINTERNET hConnect, HANDLE TimeOutHandle, NET_CHANNEL_ENUM NetChannel)
{
	if(NetChannel == NET_EVENT_RECEIVER_CHANNEL)
		InitializeCriticalSection(&m_CS);
		
	ATLASSERT(!m_IsBind);

	m_hTimer = TimeOutHandle;
	m_hConnect = hConnect;
	m_ChannelType = NetChannel;
	m_IsBind = true;

	DWORD dwTimeOutConnect,dwTimeOutReceive,dwTimeOutSend;

	dwTimeOutConnect = 10000;
	
	switch(NetChannel)
	{
	case NET_EVENT_RECEIVER_CHANNEL:
		dwTimeOutReceive = 80000;
		dwTimeOutSend  = 10000;
		break;
	case NET_COMMAND_CHANNEL:
	case NET_HISTORY_CHANNEL:
		dwTimeOutReceive = 10000;
		dwTimeOutSend    = 10000;
		break;
	case NET_FILERECEIVE_CHANNEL:
		dwTimeOutReceive = 100000;
		dwTimeOutSend    = 100000;
		break;
	case NET_FILESEND_CHANNEL:
		dwTimeOutReceive = 100000;
		dwTimeOutSend    = 100000;
		break;
	default:
		break;
	}



#ifndef _NOTIMEOUT
	InternetSetOption(m_hConnect,
					  INTERNET_OPTION_CONNECT_TIMEOUT,
					  &dwTimeOutConnect,
					  4);




	InternetSetOption(m_hConnect,
					  INTERNET_OPTION_RECEIVE_TIMEOUT,
		              &dwTimeOutReceive,
		              4);

	
	InternetSetOption(m_hConnect,
					  INTERNET_OPTION_SEND_TIMEOUT,
					  &dwTimeOutSend,
					  4);

#endif

	//=======================
	TempBuff = NULL;
	TempLength = 0;
	in_Buff = NULL;
	Length = 0;
	//---------------------
	Remain    = FALSE;
	ChunkFlag = FALSE;
	ExitFlag  = TRUE;
	
	ChunkLength=0;
	ChunkHeaderLength=0;
	return S_OK; 
}

void CBaseNetData::Unbind()
{
	m_IsBind = false;
	m_hTimer = NULL;
	if(m_hConnect != NULL)
	{
		InternetCloseHandle(m_hConnect);
		m_hConnect = NULL;
	}

	if(m_ChannelType == NET_EVENT_RECEIVER_CHANNEL)
		DeleteCriticalSection(&m_CS);
}

long CBaseNetData::SetData(LPCTSTR lpszSID, IStream *pStream, HANDLE hCallBackEvent,HANDLE hNewCallBackEvent, NET_FILE_DIRECTION FileDirection, LPCTSTR FileName,HWND CallBackHWND)
{
	LPCTSTR RootBegin=_T("<root>");
	ULONG	dwWrite;

	Reset();
	switch(m_ChannelType)
	{
	case NET_COMMAND_CHANNEL:
	case NET_HISTORY_CHANNEL:
		ATLASSERT(pStream != NULL);
		SetStream(pStream);
		break;
	case NET_FILERECEIVE_CHANNEL:
	case NET_FILESEND_CHANNEL:
		ATLASSERT(pStream != NULL);
		ATLASSERT(FileName != NULL);
		SetFile(FileName,FileDirection,CallBackHWND);
		SetStream(pStream);
		break;
	case NET_EVENT_RECEIVER_CHANNEL:
		ATLASSERT(pStream == NULL);
		ATLASSERT(FileName == NULL);
		LockInStream();
			m_pInStream.Release();
			CreateStreamOnHGlobal(NULL,TRUE,&m_pInStream);
			//m_FirstRead = FALSE;
			m_pInStream->Write(RootBegin,strlen(RootBegin)*sizeof(TCHAR),&dwWrite);
		UnLoackInStream();
		
		_stprintf(m_ObjectName,_T("?SESSION_ID=%s"),lpszSID);
		m_ObjectName[48] ='\0';
		break;
	default:
		break;
	}
	if(lpszSID != 0)
		_tcsncpy(m_szSID,lpszSID,36);
	
	
	m_hNewDataEvent  = hNewCallBackEvent;
	m_hCallbackEvent = hCallBackEvent;
	m_CallBackHWND = CallBackHWND;
	InterlockedIncrement(&m_PendingOperationCount);
	return S_OK;
}

long CBaseNetData::WriteEventsBuffer(PBYTE pBuffer, DWORD dwBufferSize)
{
	char	Value[8];
	char*	StopValue;
	char*	szBeginPacket	= "<packet>";
	DWORD	Len_BeginPacket = 8;
	char*	szEndPacket		= "</packet>";
	DWORD	Len_EndPacket	= 9;
	DWORD   m_Write;
	
try
{
	if (!Remain)
	 {
		Length = dwBufferSize;
		in_Buff = new BYTE[Length];
		::memcpy(in_Buff,pBuffer,Length);
		
		ChunkFlag = FALSE;
	///////====================================May be parsed chunk

procces:		//Check packet in begin
					DWORD k;
					for(k=0; (k<Len_BeginPacket && k<Length); k++)
					{
						if (in_Buff[k] != szBeginPacket[k])
							break;
					}
					
					//Check "packet in end"
					if (k==Len_BeginPacket)
					{
						DWORD i;
						for(i=Length-Len_EndPacket; i<Length; i++)
						{
							if (in_Buff[i] != szEndPacket[i-Length+Len_EndPacket]) 
								break;
						}
							if (i==Length)//Correct chunk detected 
							{
								//WorkClass->SetEventBuffer(in_Buff,Length);
								LockInStream();
									m_pInStream->Write(in_Buff,Length,&m_Write);
									SetEvent(m_hNewDataEvent);
								UnLoackInStream();
								//if(!m_CoolNetQueue.CoolPostMessage(WM_NEW_CHUNK,(WPARAM)in_Buff,(LPARAM)Length))
								//{delete[] in_Buff;}
								in_Buff		= NULL;
								Length		= 0;
								Remain		= FALSE;
								return S_OK; ///Received full chunk
							}
							else 
							{		Remain		= TRUE;
									TempLength	= Length;
									TempBuff	= in_Buff;
									Length		= 0;
									in_Buff		= NULL;
									return S_OK; ///Received not full chunk
							}
					}
	//=======================================================================================
	//May be unparsed Chunk
	//////Check chunk Header
chunk:
				BOOL ZeroChunk = FALSE;
				for(DWORD j=1; j<7 && j<Length-2; j++)
				{	
					if (in_Buff[j]=='\r' && in_Buff[j+1] == '\n')
					{		::CopyMemory(Value,in_Buff,j);
							Value[j] ='\0';
							ChunkLength = strtol(Value,&StopValue,16);
							ZeroChunk = (StopValue - Value == 1)&&(ChunkLength == 0);

							if ((ChunkLength!=0) || ZeroChunk )
							{
								ChunkFlag = TRUE;
								ChunkHeaderLength = j+2;
								break;
							}

					}
				}
				if (!ChunkFlag || ZeroChunk) 
				{
					delete[] in_Buff;
					in_Buff = NULL;
					Length	= 0;
					if(ZeroChunk)
						return S_FALSE;
					else
						return S_OK;
				}
			//Correct chunk but not full yet
				if (ChunkLength+ChunkHeaderLength+2>Length)
				{
						Remain		= TRUE;
						TempLength	= Length;
						TempBuff	= in_Buff;
						Length		= 0;
						in_Buff		= NULL;
						ChunkFlag	= FALSE;
						return S_OK;
				}
				else//if (ChunkLength+ChunkHeaderLength+2>Length)
				{
					if (in_Buff[ChunkLength+ChunkHeaderLength]   !='\r' || 
						in_Buff[ChunkLength+ChunkHeaderLength+1] != '\n')
					{//wrong chunk ....... del it
						delete[] in_Buff;
						in_Buff = NULL;
						Remain  = FALSE;
						return S_OK;
					}
					else//Correct chunk
					{	
						PBYTE BuffForSend;
						if (ChunkLength != 0) //Cut and send correct chhunk
						{
							BuffForSend = new BYTE[ChunkLength+10];
							memcpy(BuffForSend,in_Buff+ChunkHeaderLength,ChunkLength);
							//WorkClass->SetEventBuffer(BuffForSend,ChunkLength);
							LockInStream();
								m_pInStream->Write(in_Buff,Length,&m_Write);
								SetEvent(m_hNewDataEvent);
							UnLoackInStream();
						//	if(!m_CoolNetQueue.CoolPostMessage(WM_NEW_CHUNK,
						//						 (WPARAM)BuffForSend,(LPARAM)ChunkLength))
						//	{delete[] BuffForSend;}
							BuffForSend = NULL;
						}
						else
						{
							delete[]	in_Buff;
							in_Buff		= NULL;
							Remain		= FALSE;
							BuffForSend = NULL;
							return S_OK;			
						}
						
						//if we have remain data
						if (Length > ChunkLength+ChunkHeaderLength+2)
						{		 //|-0
							BuffForSend = in_Buff;
							in_Buff = new BYTE[Length-(ChunkLength+ChunkHeaderLength+2)+10];
							Length = Length-(ChunkLength+ChunkHeaderLength+2);
							memcpy(in_Buff,BuffForSend+Length,Length);			
							delete[]		BuffForSend;
							BuffForSend		=NULL;
							goto chunk;
						}
						else
						{
							delete[] in_Buff; in_Buff = NULL;
							Length = 0;
							Remain = FALSE;
						}
					}//end if correct chunk
				}//end if (ChunkLength+ChunkHeaderLength+2>Length)
	/////////////////////////
    } 
	//////////////////////
	//Prossec Remain packet
	else //Else if 
			  {
				PBYTE TempBuff1;
				TempBuff1 = new BYTE[TempLength+(DWORD)dwBufferSize+10];
				::memcpy(TempBuff1,TempBuff,TempLength);
				::memcpy(TempBuff1+TempLength,(PBYTE)pBuffer,(DWORD)dwBufferSize);
				
				delete[]	TempBuff;
				in_Buff		= TempBuff1;
				TempBuff1	= NULL;
				Length = TempLength+(DWORD)dwBufferSize;
				Remain    = FALSE;
				goto procces;
			  }
}
catch(...)
{
}


	return S_OK;
}

HRESULT CBaseNetData::CloneInStream(IStream **pStream)
{
	ATLASSERT(*pStream == NULL);
	HRESULT hr = S_OK;
	*pStream = NULL;
	if(m_ChannelType == NET_FILERECEIVE_CHANNEL)
	{
		*pStream = NULL;
		return S_OK;
	}
	else
	if(m_ChannelType != NET_EVENT_RECEIVER_CHANNEL)
		return m_pInStream->Clone(pStream);
	else
	{
		LockInStream();
		LPCTSTR RootEnd=_T("</root>");
		LPCTSTR RootBegin=_T("<root>");
		ULONG	dwWrite;
		if(m_pInStream != NULL)
		{
			hr = m_pInStream->Write(RootEnd,strlen(RootEnd)*sizeof(TCHAR),&dwWrite);
			hr = m_pInStream->Clone(pStream);
			m_pInStream.Release();
			m_FirstRead = TRUE;
			hr = CreateStreamOnHGlobal(NULL,TRUE,&m_pInStream);
			hr = m_pInStream->Write(RootBegin,strlen(RootBegin)*sizeof(TCHAR),&dwWrite);
		}
		UnLoackInStream();
	}
	return hr;
}

void CBaseNetData::LockInStream()
{
	EnterCriticalSection(&m_CS);
}

void CBaseNetData::UnLoackInStream()
{
	LeaveCriticalSection(&m_CS);
}

#define _MILSECOND 10000

void CBaseNetData::BeginTimout(long timeout)
{

	__int64         qwDueTime;
	LARGE_INTEGER   liDueTime;

	
	qwDueTime = -1 * _MILSECOND * timeout;
	
	// Copy the relative time into a LARGE_INTEGER.
	liDueTime.LowPart  = (DWORD) ( qwDueTime & 0xFFFFFFFF );
	liDueTime.HighPart = (LONG)  ( qwDueTime >> 32 );
	
#ifndef _NOTIMEOUT
	BOOL res = SetWaitableTimer(m_hTimer,&liDueTime,0,NULL,NULL,0);
#endif
}

void CBaseNetData::EndTimeOut()
{
	CancelWaitableTimer(m_hTimer);
}

void CBaseNetData::BeginSendRequest()
{
	if(m_ChannelType == NET_EVENT_RECEIVER_CHANNEL)
		BeginTimout(80000);
	else
	if(m_ChannelType == NET_FILESEND_CHANNEL ||
	   m_ChannelType == NET_FILERECEIVE_CHANNEL)
		BeginTimout(100000);
	else
		BeginTimout(10000);
}

void CBaseNetData::BeginWriteData()
{
	if(m_ChannelType == NET_FILESEND_CHANNEL ||
	m_ChannelType == NET_FILERECEIVE_CHANNEL)
		BeginTimout(100000);
	else
		BeginTimout(30000);
}

void CBaseNetData::BeginReadData()
{
	if(m_ChannelType == NET_EVENT_RECEIVER_CHANNEL)
		BeginTimout(80000);
	else
	if(m_ChannelType == NET_FILESEND_CHANNEL ||
	m_ChannelType == NET_FILERECEIVE_CHANNEL)
		BeginTimout(100000);
	else
		BeginTimout(10000);
}
