#include "stdafx.h"
#include "IbnPipeManger.h"
#include "ActiveSessions.h"
#include "SupportClass.h"
#include "ibn_server.h"
#include "ExternalDeclarations.h"
#include "EventLog.h"

#define BUFSIZE (4096)
#define IBN_PIPE_NAME _T("\\\\.\\pipe\\IBNServer47")

IbnPipeManger::IbnPipeManger(void)
{
	m_hThread = NULL;
	m_hPipe = NULL;
}

IbnPipeManger::~IbnPipeManger(void)
{
	Stop();
}

CString IbnPipeManger::CreatePipeName(BOOL bGlobalPipe, LPCTSTR companyId)
{
	CString retVal;

	if(bGlobalPipe)
	{
		retVal = IBN_PIPE_NAME;
	}
	else
	{
		retVal = IBN_PIPE_NAME;
		// Add Company Id
		retVal += _T("_");
		retVal += companyId;
	}

	return retVal;
}

void IbnPipeManger::StopExternalActivity(BOOL bGlobalPipe, LPCTSTR companyId)
{
	CString strPipeName = CreatePipeName(bGlobalPipe, companyId);

	HANDLE hPipe = CreateFile(strPipeName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);

	if(hPipe != INVALID_HANDLE_VALUE)
	{
		//CEventLog::AddAppLog("StopExternalActivity - Send Message");

		DWORD dwMode = PIPE_READMODE_MESSAGE;
		SetNamedPipeHandleState(hPipe, &dwMode, NULL, NULL);

		DWORD pInBuffer[4];
		pInBuffer[0] = 1; // Protocol
		pInBuffer[1] = StopActivity;
		pInBuffer[2] = 4;
		pInBuffer[3] = 0;

		BYTE pOutBuffer[4];
		DWORD bytesRead = 0;

		TransactNamedPipe(hPipe, pInBuffer, 16, pOutBuffer, 4, &bytesRead, NULL);

		CloseHandle(hPipe);
	}
}

extern HRESULT CreateSecurityDescriptor(OUT PSECURITY_DESCRIPTOR* ppSd, OUT LPBYTE* ppDacl);

HRESULT IbnPipeManger::Start(BOOL bGlobalPipe, LPCTSTR companyId)
{
	if(m_hPipe != NULL)
		return E_PENDING;

	// Create a new pipe [9/17/2004]
	m_strPipeName = CreatePipeName(bGlobalPipe, companyId);

	// Set DACL for this container to allow full control for everyone and for local system.
	PSECURITY_DESCRIPTOR pSd = NULL;
	LPBYTE pbDacl = NULL;

	CreateSecurityDescriptor(&pSd, &pbDacl);

	// Security Extension OZ 2009-07-16
	SECURITY_ATTRIBUTES m_pSecAttrib;
	m_pSecAttrib.nLength = sizeof(SECURITY_ATTRIBUTES);
	m_pSecAttrib.bInheritHandle = TRUE;
	m_pSecAttrib.lpSecurityDescriptor = pSd;

	m_hPipe = CreateNamedPipe((LPCTSTR)m_strPipeName,
		PIPE_ACCESS_DUPLEX,
		PIPE_TYPE_MESSAGE|PIPE_READMODE_MESSAGE|PIPE_WAIT,
		PIPE_UNLIMITED_INSTANCES,
		BUFSIZE,
		BUFSIZE,
		NMPWAIT_USE_DEFAULT_WAIT,
		&m_pSecAttrib);

	if (m_hPipe == INVALID_HANDLE_VALUE)
	{
		m_hPipe = NULL;
		return AtlHresultFromLastError();
	}

	// Create a new Thread [9/17/2004]
	m_hThread = (HANDLE)_beginthreadex(NULL
		, 512000
		, (unsigned int (__stdcall *)(void *)) thThreadMain
		, (LPVOID)this
		, 0
		, (unsigned int *)&m_dwThreadId
	);

	//m_hThread = (HANDLE)CreateThread(NULL, 0, 
	//	thThreadMain,
	//	(LPVOID)this, 
	//	0, 
	//	&m_dwThreadId);


	if(m_hThread==NULL)
	{
		CloseHandle(m_hPipe);
		m_hPipe = NULL;

		return AtlHresultFromLastError();
	}

	return S_OK;
}

DWORD WINAPI IbnPipeManger::thThreadMain(LPVOID param)
{
	return ((IbnPipeManger*)param)->ThreadMain();
}

DWORD IbnPipeManger::ThreadMain()
{
	BOOL bConnected = FALSE;

	while((bConnected = ConnectNamedPipe(m_hPipe, NULL)) != FALSE)
	{
		// Read Client Command [9/17/2004]
		CHeapPtr<BYTE> request;

		DWORD bufferSize = BUFSIZE;
		request.AllocateBytes(BUFSIZE);

		DWORD requestSize = 0;
		DWORD cbRealRead = 0;

		BOOL ok;
		do
		{
			cbRealRead = 0;

			if(requestSize > 0)
			{
				bufferSize += BUFSIZE;
				request.ReallocateBytes(bufferSize);
			}

			ok = ReadFile(
				m_hPipe,							// handle to pipe 
				request.m_pData + bufferSize - BUFSIZE,	// buffer to receive data 
				BUFSIZE,								// size of buffer 
				&cbRealRead,							// number of bytes read 
				NULL);									// not overlapped I/O 

			DWORD error = GetLastError();
			if(ok || error == ERROR_MORE_DATA)
			{
				requestSize += cbRealRead;
				ok = TRUE;
			}
		} 
		while(ok && cbRealRead == BUFSIZE);

		if(ok && requestSize >= (4*4))
		{
			/************************************************************************/
			/* Packet Format
			Protocol Version	: LONG
			Command ID			: LONG
			Data Size			: LONG
			DATA				: BYTE[Data Size]
			*/
			/************************************************************************/

			// Unpack Client Command [9/17/2004]
			LONG lProtocolVersion	= 0;
			LONG lCommandId			= 0;
			LONG lDataSize			= 0;

			LONG lLongValue			= 0;
			CComBSTR bsCommandText;

			memcpy(&lProtocolVersion, request, 4);
			memcpy(&lCommandId, request + 1 * 4, 4);
			memcpy(&lDataSize, request + 2 * 4, 4);
			memcpy(&lLongValue, request + 3 * 4, 4);

			HRESULT dwErrorCode = 0;

			switch(lProtocolVersion)
			{
			case 1:
				// Process Client Command [9/17/2004]
				switch(lCommandId) 
				{
				case UpdateWebStub: // LONG GroupID
					if(lDataSize == 4)
					{
						dwErrorCode = this->PipeCommand_UpdateWebStub(lLongValue);
					}
					else
						dwErrorCode = E_INVALIDARG;
					break;
				case UpdateUser: // LONG UserID
					if(lDataSize == 4)
					{
						dwErrorCode = this->PipeCommand_UpdateUser(lLongValue);
					}
					else
						dwErrorCode = E_INVALIDARG;
					break;
				case UpdateGroup: // LONG GroupID
					if(lDataSize == 4)
					{
						dwErrorCode = this->PipeCommand_UpdateGroup(lLongValue);
					}
					else
						dwErrorCode = E_INVALIDARG;
					break;
				case SendAlertToGroup: // LONG GroupID, BSTR bsParam
					{
						LPBYTE pStringBegin = request + 4 * 4;
						LONG stringByteLength = lDataSize - 4;

						if(stringByteLength > 0)
						{
							pStringBegin[stringByteLength - 2] = 0;
							pStringBegin[stringByteLength - 1] = 0;
							bsCommandText = (LPCWSTR)pStringBegin;
							dwErrorCode = this->PipeCommand_SendAlertToGroup(lLongValue, bsCommandText);
						}
						else
							dwErrorCode = E_INVALIDARG;
					}
					break;
				case SendAlertToUser: // LONG UserID, BSTR bsParam
					{
						LPBYTE pStringBegin =  request + 4 * 4;
						LONG stringByteLength = lDataSize - 4;

						if(stringByteLength > 0)
						{
							pStringBegin[stringByteLength - 2] = 0;
							pStringBegin[stringByteLength - 1] = 0;
							bsCommandText = (LPCWSTR)pStringBegin;
							dwErrorCode = this->PipeCommand_SendAlertToUser(lLongValue,bsCommandText);
						}
						else
							dwErrorCode = E_INVALIDARG;
					}
					break;
				case SendMessage://	LONG ToID, LONG FromID, LONG Message
					{
						if(requestSize >= (4*5))
						{
							LONG lFromId = 0;
							memcpy(&lFromId, request + 4 * 4, 4);

							LPBYTE pStringBegin = request + 5 * 4;
							LONG stringByteLength = lDataSize - 4;

							if(stringByteLength > 0)
							{
								pStringBegin[stringByteLength - 2] = 0;
								pStringBegin[stringByteLength - 1] = 0;
								bsCommandText = (LPCWSTR)pStringBegin;
								dwErrorCode = this->PipeCommand_SendMessage(lLongValue,lFromId,bsCommandText);
							}
							else
								dwErrorCode = E_INVALIDARG;
						}
						else
							dwErrorCode = E_INVALIDARG;
					}
					break;
				case LogOff: // LONG UserId
					if(lDataSize == 4)
					{
						dwErrorCode = this->PipeCommand_LogOff(lLongValue);
					}
					else
						dwErrorCode = E_INVALIDARG;
					break;
				case UpdateUserWebStub://	LONG UserID
					if(lDataSize == 4)
					{
						dwErrorCode = this->PipeCommand_UpdateUserWebStub(lLongValue);
					}
					else
						dwErrorCode = E_INVALIDARG;
					break;
				case StopActivity:
					// Inform Close Activity
					this->PipeCommand_StopActivity();
					lCommandId = 0x21EFFE12;
					break;
				default:
					// Send Command Error [9/17/2004]
					dwErrorCode = 0x80000004;
				}
				break;
			default:
				// Send Protocol Error [9/17/2004]
				dwErrorCode = E_INVALIDARG;
			}

			// Send Response [9/17/2004]
			DWORD dwNumberToWrite = 0;
			WriteFile(m_hPipe, &dwErrorCode, sizeof(dwErrorCode), &dwNumberToWrite, NULL);
			FlushFileBuffers(m_hPipe);

			if(lCommandId == 0x21EFFE12)
			{
				DisconnectNamedPipe(m_hPipe);
				break;
			}
		}

		// Disconnect Client [9/17/2004]
		DisconnectNamedPipe(m_hPipe);
	}

	CloseHandle(m_hPipe);
	m_hPipe = NULL;

	return 1;
}

void IbnPipeManger::Stop(void)
{
	if(m_hPipe!=NULL)
	{
		// Send StopActivity command
		while(true)
		{
			HANDLE hPipe = CreateFile(m_strPipeName, GENERIC_READ|GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);

			if(hPipe != INVALID_HANDLE_VALUE && GetLastError() == ERROR_PIPE_BUSY)
			{
				WaitNamedPipe(m_strPipeName, 3000);
				continue;
			}

			if(hPipe == INVALID_HANDLE_VALUE)
				break;

			DWORD dwMode = PIPE_READMODE_MESSAGE;
			SetNamedPipeHandleState(hPipe, &dwMode, NULL, NULL);

			DWORD pInBuffer[4];
			pInBuffer[0] = 1; // Protocol
			pInBuffer[1] = StopActivity;
			pInBuffer[2] = 4;
			pInBuffer[3] = 0;

			BYTE pOutBuffer[4];
			DWORD bytesRead = 0;

			TransactNamedPipe(hPipe, pInBuffer, 16, pOutBuffer, 4, &bytesRead, NULL);

			CloseHandle(hPipe);
			break;
		}

		DWORD waitResult = WaitForSingleObject(m_hThread, 3000);
		if(waitResult == WAIT_TIMEOUT)
		{
			//TerminateThread(m_hThread, 666);
		}
		CloseHandle(m_hThread);

		m_hThread = NULL;
		m_hPipe = NULL;
	}
}

HRESULT IbnPipeManger::PipeCommand_UpdateWebStub(/*[in]*/ LONG GroupID)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_STUBS, NULL, GroupID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_UpdateUser(/*[in]*/ LONG UserID)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_USER, NULL, UserID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_UpdateGroup(/*[in]*/ LONG GroupID)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_GROUP, NULL, GroupID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_SendAlertToGroup(LONG GroupID, BSTR bsParam)
{
	bstr_t m_bs(bsParam);
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_ALERT, (WPARAM)m_bs.Detach(), GroupID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_SendAlertToUser( LONG  UserID,  BSTR  bsParam)
{
	bstr_t m_bs(bsParam);
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_NET_ALERT, (WPARAM)m_bs.Detach(), UserID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_SendMessage(LONG ToID, LONG FromID, BSTR Message)
{
	bstr_t bsMessage(Message);

	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_NET_MESSAGE, (WPARAM)bsMessage.Detach(), (LPARAM)new FromToID(FromID, ToID));
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_LogOff(LONG UserId)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_LOGOFF_NET_MESSAGE, NULL, UserId);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_UpdateUserWebStub(/*[in]*/ LONG UserID)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_UPDATE_USERSTUBS, NULL, UserID);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_StopActivity()
{
	//CEventLog::AddAppLog("PipeCommand_StopActivity");

	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_STOP_ACTIVITY, NULL, NULL);
	return S_OK;
}

HRESULT IbnPipeManger::PipeCommand_ChangeStatus(LONG UserID, LONG Status)
{
	::PostThreadMessage(g_Extension.dwDelSessionsCallBackID, WM_CHANGE_STATUS, (WPARAM)UserID, (LPARAM)Status);
	return S_OK;
}
