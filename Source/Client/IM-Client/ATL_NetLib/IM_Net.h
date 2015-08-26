// IM_Net.h: interface for the CIM_Net class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_IM_NET_H__3AE2EAF5_1840_41BA_BBD1_E4D83EB7E34B__INCLUDED_)
#define AFX_IM_NET_H__3AE2EAF5_1840_41BA_BBD1_E4D83EB7E34B__INCLUDED_

#include "CommandQueue.h"
#include "BaseNetManager.h"
//#include "NetCommand.h"
//#include "NetEvent.h"
//#include "NetReceiveFile.h"
//#include "NetSendFile.h"

class CIM_Net  
{
public:
	CIM_Net();
	virtual ~CIM_Net();
private:
	static	DWORD WINAPI CheckQueueThread(LPVOID param);
	static	DWORD WINAPI CallBackThread(LPVOID param);
	void	CheckQueue();

public:	
	void SetNewCommand();
	void	Config();

	CRITICAL_SECTION hCS;
	CRITICAL_SECTION hNewCommandCS;
	bool	CancelCommand(long Handle);
	CCommandQueue	m_CommandQueue;
	CSession*		m_ParentSession;

	CBaseNetManager m_BaseNetManager;
	long	m_CurrentSendingStatus;
	static BASE_NET_MANAGER_CONFIG m_BASE_NET_MANAGER_CONFIG;
	BOOL	m_WaitForReconnect;
private: 
	BOOL WriteFileToServer();
	HRESULT SetStream(IStream* pStream);
	void	UnlockSending();
	void	LockSending();
	long	NewEventProcessing();	
	void	UnlockSendingOperation(NET_CHANNEL_ENUM NetChannel);
	void	LockSendingOperation(NET_CHANNEL_ENUM NetChannel);
	long	XMLfromSream(IXMLDOMDocument** lpDoc, IStream* pStream);
	long	XMLtoStream(IXMLDOMDocument* pDoc, IStream** lpStream);
	DWORD	ProcessingServerEvents();

	bool	m_EnableSend[4];
	long	m_OpenThreadCount;
	DWORD	m_CurrentHandles[4];
	DWORD	m_CurrentCommands[4];
	HANDLE	hEvents[2];
	HANDLE	hCallBackEvents[7];
	stState m_State;

	BOOL	m_bMustBeDestroied;
	BOOL	m_bWaitForDisconnect;
	long	m_StartedChannelCount; //count of active net channel	

	DWORD	m_DisErrorCode;
	DWORD	m_DisErrorType;
	
	//proxy
	DWORD  prAccessType;
	LPTSTR prProxyName;
	LPTSTR prBypass;
};

#endif // !defined(AFX_IM_NET_H__3AE2EAF5_1840_41BA_BBD1_E4D83EB7E34B__INCLUDED_)
