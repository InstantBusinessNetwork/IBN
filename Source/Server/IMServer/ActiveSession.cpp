// ActiveSession.cpp: implementation of the CActiveSession class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "ibn_server.h"
#include "ActiveSession.h"
#include "ExternalDeclarations.h"

//#define _OFS_WRITE_TRACE_INFO

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

const BYTE StrPaketBEGIN[] = "\r\n<packet>";
const int  LenPaketBEGIN   = 10;
const BYTE StrPaketEND[]   = "</packet>\r\n";
const int  LenPaketEND     = 11;
const BYTE ZeroChunk[]     = "0\r\n\r\n";
const int  LenZeroChunk    = 5;

static VOID WINAPI HseActiveSIoCompletion(EXTENSION_CONTROL_BLOCK *pECB, PVOID pContext, DWORD cbIO, DWORD dwError);
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CActiveSession::CActiveSession()
{
	iNowUse = 1;
	pSendData = NULL;
	pWaitData = NULL;
	m_SendDataLenth = 0;
	m_WaitDataLenth = 0;
	m_PoketSize = 0;
	m_pPoketSend = NULL;

	pECBlock = NULL;
	m_DissconnectTime = time(NULL) + WaitTimeAfterDissconnect;
	m_ConnectTime     = time(NULL);
	lKill = 0L;

	InitializeCriticalSection(&m_lock);

	ActiveSessionState = 0;
	lWasReconect = 1;
	sys_time_end = 0;
	sys_time_begin = GetTickCount();

}

CActiveSession::~CActiveSession()
{
	while(!TryEnterCriticalSection(&m_lock))
		Sleep(1);

	DeleteCriticalSection(&m_lock);

	try
	{
		if(pSendData != NULL)
		{
			delete[] pSendData;
			pSendData = NULL;
			m_SendDataLenth = 0;
		}

		if(pWaitData != NULL)
		{
			delete[] pWaitData;
			pWaitData = NULL;
			m_WaitDataLenth = 0;
		}
	}
	catch(...)
	{
	}
}

time_t CActiveSession::GetDissconnectTime()
{
	return m_DissconnectTime;
}

time_t CActiveSession::GetConnectTime()
{
	return m_ConnectTime;
}

long CActiveSession::GetStatus()
{
	long tmp;
	//EnterCriticalSection(&m_lock);
	//// Block

	tmp = m_Status;

	//// UnBlock 
	//LeaveCriticalSection(&m_lock);
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[GetStatus] %ld "), tmp);
	//m_SupportClass.AddToLog(1,str);
#endif

	return tmp;
}

void CActiveSession::SetStatus(long Status)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[SetStatus] %ld "), Status);
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);
	//// Block

	/// And Change
	m_Status = Status;

	//// UnBlock 
	LeaveCriticalSection(&m_lock);
}

void CActiveSession::SetConnectTime(time_t Time)
{
	m_ConnectTime = Time;
}

void CActiveSession::SetDissconnectTime(time_t Time)
{
	m_DissconnectTime = Time;
}

void CActiveSession::UpdateConnection(EXTENSION_CONTROL_BLOCK *pEcb, BOOL keepAlive)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[UpDataConnection]"));
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);

	///// Addon 17 - 02 - 2001 for Statistic
	if(lWasReconect)
	{
		if(pEcb)
		{
			sys_time_end = GetTickCount();

			if(sys_time_end >= sys_time_begin)
				sys_time_end = sys_time_end - sys_time_begin;
			else
				sys_time_end = (ULONG_MAX - sys_time_end) + sys_time_begin;
		}
		lWasReconect = 0;
	}
	//// End Addon

	BOOL bFlagStartSend = FALSE;

	if(ActiveSessionState & SEND_MESSAGE_CYCLE)
		bFlagStartSend = FALSE;
	else 
	{
		if(pEcb != NULL)
		{
			ActiveSessionState |= SEND_MESSAGE_CYCLE;
			bFlagStartSend = TRUE;
		}
	}

	SetConnectTime(time(NULL));
	SetDissconnectTime(time(NULL) + DissconnectTime);

	try
	{
		if(pECBlock != NULL)
		{
			if(pEcb != pECBlock)
			{
				if(!(ActiveSessionState & SEND_MESSAGE_NOW))
				{
					pECBlock->ServerSupportFunction(pECBlock->ConnID, HSE_REQ_DONE_WITH_SESSION, NULL, NULL, NULL);
				}
				else
				{
					/// ????
					//bFlagStartSend = TRUE; //???
					ActiveSessionState |= UPDATE_CONNECITON;
					pECBlock->ServerSupportFunction(pECBlock->ConnID, HSE_REQ_CLOSE_CONNECTION, NULL, NULL, NULL);
					/// ????
				}
				pECBlock = pEcb;
			}
			ActiveSessionState &= ~SEND_ZEROCHUNK; // Не было последнего чанка
			ActiveSessionState &= ~SIGNAL_DISCONNECT; // Не было сигнала на дисконект
			m_KeepAlive =  keepAlive;
		}
		else
		{
			//ActiveSessionState = 0UL; /// Нет состояний
			ActiveSessionState &= ~SEND_ZEROCHUNK; // Не было последнего чанка
			ActiveSessionState &= ~SIGNAL_DISCONNECT; // Не было сигнала на дисконект

			//m_SendDataLenth = m_WaitDataLenth = 0;

			pECBlock = pEcb;
			m_KeepAlive = keepAlive;
		}

		if(pECBlock != NULL)
			pECBlock->ServerSupportFunction(pECBlock->ConnID, HSE_REQ_IO_COMPLETION, HseActiveSIoCompletion, NULL, (LPDWORD)this);

	}
	catch(...)
	{
		ASSERT(FALSE);
	}


	LeaveCriticalSection(&m_lock);

	if(bFlagStartSend)
	{
		UnionEvent();
		Send();
	}
}

void CActiveSession::SetEvent(const PBYTE pData, size_t size)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[SetEvent] size = %Iu"), size);
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);

	BOOL bFlagSend = FALSE;
	//// Block
	try
	{
		/// And Change
		if(ActiveSessionState & SEND_MESSAGE_CYCLE)
		{
			if(pWaitData != NULL)
			{
				PBYTE TmpBuffer = pWaitData;
				pWaitData = new BYTE[m_WaitDataLenth + size];
				memcpy(pWaitData, TmpBuffer, m_WaitDataLenth);
				memcpy(pWaitData + m_WaitDataLenth, pData, size);
				m_WaitDataLenth += size;
				try
				{
					delete[] TmpBuffer;
				}
				catch(...)
				{
				}
			}
			else
			{
				m_WaitDataLenth = size;
				pWaitData = new BYTE[m_WaitDataLenth];
				memcpy(pWaitData, pData, m_WaitDataLenth);
			}
		}
		else
		{
			if(pECBlock != NULL)
			{
				bFlagSend = TRUE;
				ActiveSessionState |= SEND_MESSAGE_CYCLE;
			}
			if(pSendData == NULL)
			{
				m_SendDataLenth = size;
				pSendData = new BYTE[m_SendDataLenth];
				memcpy(pSendData, pData, m_SendDataLenth * sizeof(BYTE));
			}
			else
			{
				PBYTE TmpBuffer = pSendData;
				pSendData = new BYTE[m_SendDataLenth + size];
				memcpy(pSendData, TmpBuffer, m_SendDataLenth);
				memcpy(pSendData + m_SendDataLenth, pData, size);
				m_SendDataLenth += size;
				try
				{
					delete[] TmpBuffer;
				}
				catch(...)
				{
				}
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	//// UnBlock 
	LeaveCriticalSection(&m_lock);

	if(bFlagSend)
	{
		/// Начать отправку данных ...
		Send();
	}
}

DWORD CActiveSession::Send()
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[Send]"));
	//m_SupportClass.AddToLog(1,str);
#endif

	PBYTE pSendData = NULL;
	size_t size = 0;
	BOOL bChunkWithoutZero = TRUE;
	BOOL bResendOldChunk = FALSE;

	while(GetEvent(pSendData, size, bChunkWithoutZero, bResendOldChunk))
	{
		/// Create Packet ...

		if(!bResendOldChunk)
		{
			if(bChunkWithoutZero)
			{
				char szChunkLength[17];
				size_t cbChunkLength = 0;

				//_itoa_s((int)size/*+17*/, szChunkLength, 9, 16);
				sprintf_s(szChunkLength, 17, "%Ix", size);
				cbChunkLength = strlen(szChunkLength);

				const char* szNewLine = "\r\n";
				const size_t cbNewLine = 2; // strlen(szNewLine);

				if(m_pPoketSend != NULL)
				{
					delete[] m_pPoketSend;
					m_pPoketSend = NULL;
					m_PoketSize = 0;
				}

				m_PoketSize = cbChunkLength + cbNewLine + size + cbNewLine;
				m_pPoketSend = new BYTE[m_PoketSize];

				memcpy(m_pPoketSend, szChunkLength, cbChunkLength);
				memcpy(m_pPoketSend + cbChunkLength, szNewLine, cbNewLine);
				memcpy(m_pPoketSend + cbChunkLength + 2, pSendData, size);
				memcpy(m_pPoketSend + cbChunkLength + 2 + size, szNewLine, cbNewLine);
			}
			else 
			{
				/// без покета последний чанк ...
				m_pPoketSend = new BYTE[size];
				m_PoketSize = size;
				memcpy(m_pPoketSend, pSendData, size);
			}
		}

		DWORD dwError = SendMessage(m_pPoketSend, m_PoketSize);

		if(dwError != 0)
		{
			EnterCriticalSection(&m_lock);

			if(m_pPoketSend != NULL)
			{
				delete[] m_pPoketSend;
				m_pPoketSend = NULL;
				m_PoketSize = 0;
			}
			/// Разбор Ошибки ...

			if(ActiveSessionState & UPDATE_CONNECITON)
			{
				/// Происходило обновление ...
				ActiveSessionState &= ~UPDATE_CONNECITON;
				LeaveCriticalSection(&m_lock);
				/// и это не последний чанк Обьединить события и заново
				return 0;
			}
			/// сюда если ошибка 
			ActiveSessionState &= ~SEND_MESSAGE_CYCLE;/*11.30*/
			ActiveSessionState |= SEND_ZEROCHUNK;

			LeaveCriticalSection(&m_lock);
			/// Ошибка отправки ...
			CloseSession();
		}
		break;
	}

	return 0;
}

BOOL CActiveSession::GetEvent(PBYTE& pData, size_t& size, BOOL& bChunkWithoutZero, BOOL& bResendOldChunk)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[GetEvent]"));
	//m_SupportClass.AddToLog(1,str);
#endif

	EnterCriticalSection(&m_lock);

	//// Block

	pData = NULL;
	size = 0;
	bChunkWithoutZero = TRUE;
	bResendOldChunk = FALSE;

	if(pECBlock != NULL)
	{
		if(ActiveSessionState & SIGNAL_DISCONNECT)
		{
			if(!(ActiveSessionState & SEND_ZEROCHUNK))
			{
				size = 5UL;
				pData = const_cast<PBYTE>(ZeroChunk);
				bChunkWithoutZero = FALSE;
				/*23.11*///ActiveSessionState |= SEND_ZEROCHUNK;
			}
		}
		else
		{
			// Added cut pocket [9/24/2004]
			if(m_pPoketSend != NULL)
			{
				bResendOldChunk = TRUE;

				m_PoketSize -= m_SentPoketSize;
				memmove(m_pPoketSend, m_pPoketSend + m_SentPoketSize, m_PoketSize);

				pData = m_pPoketSend;
				size = m_PoketSize;
			}
			// End added [9/24/2004]
			else if(pSendData != NULL)
			{
				pData = pSendData;
				size = m_SendDataLenth;
			}
		}
	}
	else
	{
		/*30.11*/
		if(ActiveSessionState & SIGNAL_DISCONNECT)
			ActiveSessionState |= SEND_ZEROCHUNK;
	}

	//// UnBlock 
	if(pData == NULL)
		ActiveSessionState &= ~SEND_MESSAGE_CYCLE;

	LeaveCriticalSection(&m_lock);

	return (pData) ? TRUE : FALSE;
}

DWORD CActiveSession::SendMessage(PBYTE pData, size_t size)
{

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[SendMessage] Size = %Iu "), size);
	//m_SupportClass.AddToLog(1, str);
#endif

	DWORD dwError = 0UL;
	/*23.11*/EnterCriticalSection(&m_lock);
	/*23.11*/ActiveSessionState |= SEND_MESSAGE_NOW;
	/*23.11*/
	if(size == 5)
		ActiveSessionState |= SEND_ZEROCHUNK;

	if(pECBlock == NULL)
		dwError = 0xffff;
	else
	{
		DWORD dwSize = static_cast<DWORD>(size);
		if(!pECBlock->WriteClient(pECBlock->ConnID, pData, &dwSize, HSE_IO_ASYNC))
		{
			dwError = GetLastError();
			ActiveSessionState &= ~SEND_MESSAGE_NOW;
#ifdef _DEBUG

			/*LPVOID lpMsgBuf;
			FormatMessage( 
			FORMAT_MESSAGE_ALLOCATE_BUFFER | 
			FORMAT_MESSAGE_FROM_SYSTEM | 
			FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,
			dwError,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
			(LPTSTR) &lpMsgBuf,
			0,
			NULL 
			);
			//Process any inserts in lpMsgBuf.
			//...
			//Display the string.*/
			//TCHAR str[80];
			//sprintf(str,"[ActiveS]->[SendMessage Error] (*) %x (*)", dwError);
			//m_SupportClass.AddToLog(1,str);
			//Free the buffer.
			//LocalFree( lpMsgBuf );
#endif
		}
		else
		{
			GetPointer();
		}
	}
	LeaveCriticalSection(&m_lock);/*23.11*/
	return dwError;
}

PBYTE CActiveSession::PopEvent()
{
#ifdef _OFS_WRITE_TRACE_INFO
#endif

	PBYTE pData = NULL;
	//// Block
	try
	{
		if(pSendData!=NULL)
		{
			//if(cbIO>=m_SendDataLenth)
			//{
			// Was sended all packet [10/8/2002]
			pData            = pSendData;
			pSendData        = pWaitData;
			m_SendDataLenth  = m_WaitDataLenth;
			pWaitData        = NULL;
			m_WaitDataLenth  = 0;
			//}
			//else
			//{
			//	// Was sended only part of packet [10/8/2002]
			//	m_SendDataLenth	-= cbIO;
			//	memmove(pSendData,pSendData+cbIO,m_SendDataLenth);
			//	pData = NULL;
			//}
		}
		else
		{
			ASSERT(FALSE);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	//// UnBlock 

	return pData;
}

PBYTE CActiveSession::UnionEvent()
{
#ifdef _OFS_WRITE_TRACE_INFO
#endif

	EnterCriticalSection(&m_lock);
	//// Block
	try
	{
		if(pWaitData!=NULL)
		{
			PBYTE pTmpData = NULL;
			pTmpData = pSendData;
			pSendData = new BYTE[m_SendDataLenth + m_WaitDataLenth];
			memcpy(pSendData, pTmpData, m_SendDataLenth);
			memcpy(pSendData + m_SendDataLenth, pWaitData, m_WaitDataLenth);
			m_SendDataLenth += m_WaitDataLenth;
			delete[] pTmpData;
			pTmpData = NULL;
			pWaitData = NULL;
			m_WaitDataLenth = 0;
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	//// UnBlock 
	LeaveCriticalSection(&m_lock);

	return pSendData;
}

BOOL CActiveSession::CheckTime()
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[CheckTime]"));
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);
	try
	{
		time_t NowTime = time(NULL);
		if(NowTime > GetDissconnectTime())
		{

			if((ActiveSessionState&SEND_ZEROCHUNK)&&(ActiveSessionState&SIGNAL_DISCONNECT)&&(pECBlock==NULL))
				/// Время удаления пришло...
			{
#ifdef _OFS_WRITE_TRACE_INFO
				_stprintf_s(str, 80, _T("[ActiveS]->[Set Kill Time]"));
				//m_SupportClass.AddToLog(1,str);
#endif

				LeaveCriticalSection(&m_lock);
				return TRUE;
			}
			else
			{
#ifdef _OFS_WRITE_TRACE_INFO
				_stprintf_s(str, 80, _T("[ActiveS]->[Set Dissconnect Time]"));
				//m_SupportClass.AddToLog(1,str);
#endif
				LeaveCriticalSection(&m_lock);
				CloseSession();
				return FALSE;
			}
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	LeaveCriticalSection(&m_lock);

	return FALSE;
}

void CActiveSession::CloseSession()
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[CloseSession]"));
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);
	//// Block + Send Last Chunk
	try
	{
		ActiveSessionState|=SIGNAL_DISCONNECT;

		if(ActiveSessionState&SEND_ZEROCHUNK)
		{
			if(!lWasReconect)
			{
				///// Addon 17 - 02 - 2001 for Statistic
				//				m_ExternalLink.Add2Stat(SE_USERS_CONNECTED,InterlockedDecrement(&lUsersConnected));
				//				m_ExternalLink.Add2Stat(SE_USERS_RECONNECTED,InterlockedIncrement(&lUsersReconnectd));
				///// End Addon
				lWasReconect = 1;
				///// Addon 17 - 02 - 2001 for Statistic
				sys_time_begin  =  GetTickCount();
				//// End Addon
			}

			if(lKill)
			{
				//				m_ExternalLink.Add2Stat(SE_USERS_RECONNECTED,InterlockedDecrement(&lUsersReconnectd));	
				//				m_ExternalLink.Add2Stat(SE_USERS_ONLINE,InterlockedDecrement(&lUsersOnline));
			}

			if(pECBlock!=NULL)
			{
				SetConnectTime(time(NULL));
				SetDissconnectTime(time(NULL) + WaitTimeAfterDissconnect);

#ifdef _OFS_WRITE_TRACE_INFO
				_stprintf_s(str, 80, _T("[ActiveS]->[Delete Session]"));
				//m_SupportClass.AddToLog(1,str);
#endif

				pECBlock->ServerSupportFunction(pECBlock->ConnID, HSE_REQ_DONE_WITH_SESSION, NULL, NULL, NULL);
			}

			pECBlock = NULL;

		}
		else
		{
			LeaveCriticalSection(&m_lock);
			SendZeroChunk();
			return;
		}
	}
	catch(...)
	{
		//		ASSERT(FALSE);
	}
	//// UnBlock 
	ActiveSessionState &= ~SEND_MESSAGE_CYCLE;
	LeaveCriticalSection(&m_lock);
}

static VOID WINAPI HseActiveSIoCompletion(EXTENSION_CONTROL_BLOCK *pECB, PVOID pContext, DWORD cbIO, DWORD dwError)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[HseIoCompletion pSession ( %lx )]"), pContext);
	//m_SupportClass.AddToLog(1,str);
#endif
	try
	{
		if (pContext != NULL)//если есть закрепленный объект, то уведомить
		{
			try
			{
				CActiveSession* pActiveS = (CActiveSession*)pContext ;
				pActiveS->SendComplete(pECB, dwError, cbIO);
				pActiveS->ReleasePointer();
			}
			catch(...)
			{
				ASSERT(FALSE);
			}
		}
		else
		{
			//если нет закрепленного объекта, то отключить
			pECB->ServerSupportFunction(pECB->ConnID, HSE_REQ_DONE_WITH_SESSION, NULL, NULL, NULL);
		}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
}

void CActiveSession::SendComplete(EXTENSION_CONTROL_BLOCK *&pECB, DWORD ErrorCode, size_t cbIO)
{
#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[SendComplete]"));
	//m_SupportClass.AddToLog(1,str);
#endif
	EnterCriticalSection(&m_lock);
	ActiveSessionState &= ~SEND_MESSAGE_NOW; /*23.11*/
	try
	{
		// Try send Data buffer [9/24/2004]

		if(cbIO == m_PoketSize)
		{
			m_SentPoketSize = 0;

			if(m_pPoketSend != NULL)
			{
				delete[] m_pPoketSend;
				m_pPoketSend = NULL;
				m_PoketSize = 0;
			}

			if(!(ActiveSessionState & SEND_ZEROCHUNK))
			{
				PBYTE pTempData = PopEvent();
				if(pTempData)
				{
					delete[] pTempData;
					pTempData = NULL;
				}
			}
		}
		else
		{
			m_SentPoketSize = cbIO;
		}

		if(pECB != pECBlock) /*23.11*/
		{
			pECB->ServerSupportFunction(pECB->ConnID, HSE_REQ_DONE_WITH_SESSION, NULL, NULL, NULL);
			pECB = NULL;
			//LeaveCriticalSection(&m_lock);
			//return;
		}
		else
			/*23.11*/
			if(ErrorCode != 0)
			{
				m_SentPoketSize = 0;
				if(m_pPoketSend != NULL)
				{
					delete[] m_pPoketSend;
					m_pPoketSend = NULL;
					m_PoketSize = 0;
				}

				ActiveSessionState |= SEND_ZEROCHUNK;
				LeaveCriticalSection(&m_lock);
				CloseSession();
				return;
			}

			if((ActiveSessionState & SIGNAL_DISCONNECT) && (ActiveSessionState & SEND_ZEROCHUNK))
			{
				m_SentPoketSize = 0;
				if(m_pPoketSend != NULL)
				{
					delete[] m_pPoketSend;
					m_pPoketSend = NULL;
					m_PoketSize = 0;
				}

				LeaveCriticalSection(&m_lock);
				CloseSession();
				return;
			}
	}
	catch(...)
	{
		ASSERT(FALSE);
	}
	//// UnBlock 
	LeaveCriticalSection(&m_lock);

	Send();
}

void CActiveSession::SendZeroChunk()
{

#ifdef _OFS_WRITE_TRACE_INFO
	TCHAR str[80];
	_stprintf_s(str, 80, _T("[ActiveS]->[SendZeroChunk]"));
	//g_SupportClass.AddToLog(1,str);
#endif

	EnterCriticalSection(&m_lock);
	//// Block + Send Last Chunk
	BOOL bFlagSend = FALSE;

	if(!(ActiveSessionState & SEND_MESSAGE_CYCLE) && !(ActiveSessionState & SEND_ZEROCHUNK))
	{
		ActiveSessionState |= SEND_MESSAGE_CYCLE;
		bFlagSend = TRUE;
	}

	//// UnBlock 
	LeaveCriticalSection(&m_lock);

	if(bFlagSend) Send();
}

BOOL CActiveSession::IsClosed()
{
	BOOL bReturn;
	EnterCriticalSection(&m_lock);

	bReturn = (!(ActiveSessionState & SEND_MESSAGE_NOW)) && (ActiveSessionState & SEND_ZEROCHUNK);

	LeaveCriticalSection(&m_lock);

	return bReturn;
}

void CActiveSession::ClearInfo()
{
	///// Addon 17 - 02 - 2001 for Statistic
	EnterCriticalSection(&m_lock);

	lKill = 1L;

	//	if(lWasReconect)
	//	{
	//	}
	//	else
	//	{
	//		InterlockedDecrement(&lUsersConnected);
	//		m_ExternalLink.Add2Stat(SE_USERS_CONNECTED,lUsersConnected);
	//	}
	///// End Addon

	LeaveCriticalSection(&m_lock);
}

void CActiveSession::Delete(void)
{
	ReleasePointer();
}
