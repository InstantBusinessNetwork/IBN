// BaseNetManager.h: interface for the CBaseNetManager class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_BASENETMANAGER_H__E32BA28F_6E71_4A9F_80BF_D38A12AAFC24__INCLUDED_)
#define AFX_BASENETMANAGER_H__E32BA28F_6E71_4A9F_80BF_D38A12AAFC24__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000
#include "basenetdata.h"

void g_AddSentBytes(long bytes);
void g_AddReceivedBytes(long bytes);


struct BASE_NET_MANAGER_CONFIG
{
	BASE_NET_MANAGER_CONFIG()
	{
		strcpy(m_szServerName,_T("im.war.ru"));
		strcpy(m_szPath,_T("ofs_server.dll"));
		//m_szServerName[0] = '\0';
		//m_szPath[0] = '\0';
		m_szProxyServerName[0] = '\0';
		m_szProxyServerLogin[0] = '\0';
		m_szProxyServerPassword[0] = '\0';
		m_ServerPort = 80;
		m_ProxyServerPort = 8080;
		m_dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
		m_bSecure = FALSE;
		m_bUseSSL = FALSE;
	}

	TCHAR m_szServerName[200];
	TCHAR m_szPath[200];
	short m_ServerPort;

	DWORD m_dwAccessType;
	TCHAR m_szProxyServerName[200];
	short m_ProxyServerPort;
	BOOL  m_bSecure;
	TCHAR m_szProxyServerLogin[100];
	TCHAR m_szProxyServerPassword[100];
	BOOL  m_bUseSSL;

	BASE_NET_MANAGER_CONFIG& operator=( const BASE_NET_MANAGER_CONFIG& s1)
	{
		strcpy(m_szServerName,s1.m_szServerName);
		strcpy(m_szPath,s1.m_szPath);
		m_ServerPort = s1.m_ServerPort;

		m_dwAccessType = s1.m_dwAccessType;
		m_ProxyServerPort = s1.m_ProxyServerPort;
		m_bSecure = s1.m_bSecure;
		strcpy(m_szProxyServerName,s1.m_szProxyServerName);
		strcpy(m_szProxyServerLogin,s1.m_szProxyServerLogin);
		strcpy(m_szProxyServerPassword,s1.m_szProxyServerPassword);
		m_bUseSSL = s1.m_bUseSSL;
		return *this;
	}
	
};

class CBaseNetManager  
{
	struct _CALL_BACK_DATA
	{
		CBaseNetManager* pBaseNetManager;
		NET_CHANNEL_ENUM pChannel;
	};

public:
	BOOL WaitChanel(NET_CHANNEL_ENUM mChannel);
	CBaseNetManager();
	virtual ~CBaseNetManager();
	
	void GetResult(NET_CHANNEL_ENUM emChannel, IStream** lpStream);
	void GetResultError(NET_CHANNEL_ENUM emChannel, DWORD& ErrorType, DWORD& ErrorCode);
	long GetNextEvent(IStream** lpStream);
	long StopEventReceiver();
	long StartEventReceiver(LPCTSTR lpszSID,HANDLE hCallBackEvent,HANDLE hNewEventCallBackEvent);
	long StartOperation(NET_CHANNEL_ENUM NetChannel,LPCTSTR lpszSID, DWORD Handle, IStream* pStream, HANDLE hCallBackEvent, LPCTSTR FileName = NULL, NET_FILE_DIRECTION FileDirection = NET_FILE_NONE, HWND CallBackHWND = NULL);
	long StopOperation(NET_CHANNEL_ENUM NetChannel);
	long StopAllOperations();
	long Close();
	long Reconfig(BASE_NET_MANAGER_CONFIG sConfig);
	long Init(BASE_NET_MANAGER_CONFIG sConfig);

private:
	DWORD TimeOut(LPVOID param);
	static DWORD WINAPI TimeOutThreadProc(LPVOID lpParameter);
	long InetProcessing(NET_CHANNEL_ENUM NetChannel);

	BOOL CheckStatus(NET_CHANNEL_ENUM enChannel);
	BOOL WriteFile(NET_CHANNEL_ENUM enChannel);
	void CallBack(NET_CHANNEL_ENUM enChannel, INTERNET_ASYNC_RESULT* pResult);
	

	static void CALLBACK InternetCallback12(
								HINTERNET hInternet,
								DWORD dwcontext,
								DWORD dwInternetStatus,
								LPVOID lpvStatusInformation,
								DWORD dwStatusInformationLength
								);



	void WorkFunction(NET_CHANNEL_ENUM NetChannel);
	static DWORD WINAPI ThreadProc(LPVOID lpParameter);

	BASE_NET_MANAGER_CONFIG m_BASE_NET_MANAGER_CONFIG;
	CBaseNetData     m_BaseNetData[5];
	_CALL_BACK_DATA  m_arCallBackData[5];
	HANDLE			 m_TimeoutEvents[8];
	HINTERNET		 m_hInternet;
	BOOL			 m_IsInit;
	BOOL			 m_bClose;
//	int				 m_intCount407;
};

#endif // !defined(AFX_BASENETMANAGER_H__E32BA28F_6E71_4A9F_80BF_D38A12AAFC24__INCLUDED_)
