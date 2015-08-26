// BaseNetData.h: interface for the CBaseNetData class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_BASENETDATA_H__21FD4DC2_08A7_4A15_B1DB_25819DA1E42D__INCLUDED_)
#define AFX_BASENETDATA_H__21FD4DC2_08A7_4A15_B1DB_25819DA1E42D__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
typedef enum
{
	NET_COMMAND_CHANNEL,
	NET_HISTORY_CHANNEL,
	NET_FILERECEIVE_CHANNEL,
	NET_FILESEND_CHANNEL,
	NET_EVENT_RECEIVER_CHANNEL
} NET_CHANNEL_ENUM;

typedef enum
{
	NET_FILE_NONE,
	NET_FILE_SEND,
	NET_FILE_RECEIVE
} NET_FILE_DIRECTION;

typedef enum
{
	NET_CHANNEL_STATE_BEGIN,
	NET_CHANNEL_STATE_REQUEST_SEND,
	NET_CHANNEL_STATE_REQUEST_DATA_SEND,
	NET_CHANNEL_STATE_REQUEST_END,
	NET_CHANNEL_STATE_RESPONSE_READ,
	NET_CHANNEL_STATE_END,
}NET_CHANNEL_STATE_ENUM;

class CBaseNetManager;

class CBaseNetData  
{
	friend CBaseNetManager;
public:
	long SetData(LPCTSTR lpszSID, IStream* pStream, 
				HANDLE hCallBackEvent,
				HANDLE hNewCallBackEvent = NULL,  
				NET_FILE_DIRECTION FileDirection = NET_FILE_NONE, 
				LPCTSTR FileName = NULL, 
				HWND CallBackHWND = NULL);
	void Unbind();
	long Bind(HINTERNET hConnect,HANDLE TimeOutHandle, NET_CHANNEL_ENUM NetChannel);
	CBaseNetData();
	virtual ~CBaseNetData();
	
private:
	void BeginReadData();
	void BeginWriteData();
	void BeginSendRequest();
	void EndTimeOut();
	void BeginTimout(long timeout);
	void UnLoackInStream();
	void LockInStream();
	HRESULT CloneInStream(IStream** pStream);
	long WriteEventsBuffer(PBYTE pBuffer, DWORD dwBufferSize);
	LPINTERNET_BUFFERS GetOutBuffer();
	void	Reset();

	void	SetEnd(DWORD ErrorType = 0, DWORD ErrorCode = 0);
	HRESULT SetFile(LPCSTR FileName, NET_FILE_DIRECTION Direction, HWND CallBackHWND);
	HRESULT SetStream(IStream *pStream);

	LPINTERNET_BUFFERS GetInBuffer();


	BOOL	WriteAnswer();
	LPCTSTR GetObjectName();
	LPCTSTR GetVerbs();
	
	DWORD	GetSendRequestFlags();
	DWORD	GetOpenRequestFlags();
	
	LPVOID	GetDataBuffer(DWORD &dwBufferSize);
	DWORD	GetFullSize();
	LPCSTR	GetHeaderBuffer(DWORD &dwBufferSize);
	
static	LONG			 m_PendingOperationCount;
	//Bind data
	HINTERNET m_hConnect;					//
	BOOL	  m_IsBind;
	NET_CHANNEL_ENUM m_ChannelType;			//тип канала

	
	HANDLE	m_hNewDataEvent;				//только для NET_EVENT_RECEIVER_CHANNEL
	HANDLE  m_hCallbackEvent;				//событие завершения операции
	NET_CHANNEL_STATE_ENUM m_ChannelState;  //состояние канала

	DWORD	m_CurrentHandles;
	DWORD	m_dwFullWriteSize;
	DWORD   m_dwFullReadSize;
	DWORD   m_dwContentLength;
	//настройки файла
	HWND			   m_CallBackHWND;
	TCHAR			   m_FileName[500];
	NET_FILE_DIRECTION m_FileDirection;

	//Input parameters
	CComPtr<IStream> m_pInStream;
	HANDLE			 m_hInFile;
	DWORD			 m_dwInFileSize;

	INTERNET_BUFFERS m_InINET_BUFFERS;
	BYTE			 m_pInBuffer[2*5024];
	DWORD			 m_dInwBufferSize;
	BOOL			 m_FirstRead;
	BOOL			 m_WriteData;
	//Output parameters
	//OUT STREAM
	CRITICAL_SECTION m_CS;
	CComPtr<IStream> m_pOutStream;
	HGLOBAL			 m_hOutGlobal;
	LPVOID			 m_dwOutStreamBuff;
	DWORD			 m_dwOutStreamSize;
	//OUT FILE
	INTERNET_BUFFERS m_OutINET_BUFFERS;
	HANDLE			 m_hOutFile;
	DWORD			 m_dwOutFileSize;

	LONG			  m_ErrorType;
	LONG			  m_ErrorCode;

	HINTERNET m_hRequest;

	TCHAR m_szSID[36];
	TCHAR m_ObjectName[300];
	TCHAR m_Header[300];
	DWORD m_OpenRequestFlags;

	BOOL m_bCancel;
//===============================================
	PBYTE TempBuff;
	DWORD TempLength;
	PBYTE in_Buff;
	DWORD Length;
	//---------------------
	BOOL Remain;
	BOOL ChunkFlag;
	BOOL ExitFlag;
	
	DWORD ChunkLength;
	DWORD ChunkHeaderLength;

	BOOL bDataSent;
//====================================================
	HANDLE m_hTimer;

	LONG haveMessage;
};

#endif // !defined(AFX_BASENETDATA_H__21FD4DC2_08A7_4A15_B1DB_25819DA1E42D__INCLUDED_)
