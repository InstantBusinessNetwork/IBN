// Session.h : Declaration of the CSession

#ifndef __SESSION_H_
#define __SESSION_H_

#include "defConst.h"	// Added by ClassView
#define WM_IMSENDER_WRNING	WM_USER+111
#define WM_IMSENDER_STAT	WM_USER+112
#define WM_IMSENDER_STATE	WM_USER+113

#include <atlwin.h>
#include "resource.h"       // main symbols
#include "IM_Net.h"
#include "ATL_NetLibCP.h"


extern long g_ByteSent;
extern long g_ByteReceived;
extern long g_LatestSent;
extern long g_LatestReceived;

extern long g_FileSent;
extern long g_FileReceived;

extern long g_MessageSend;
extern long g_MessageReceveived;

extern long g_ByteSentPerSec[200];
extern long g_ByteReceivedPerSec[200];

extern long g_forByteSentPerSec[200];
extern long g_forByteReceivedPerSec[200];

extern CComAutoCriticalSection SendLock;
extern CComAutoCriticalSection ReceivedLock;
/////////////////////////////////////////////////////////////////////////////
// CSession
class ATL_NO_VTABLE CSession : 
	public CWindowImpl<CSession>,
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CSession, &CLSID_Session>,
	public IConnectionPointContainerImpl<CSession>,
	public IDispatchImpl<ISession, &IID_ISession, &LIBID_ATL_NETLIBLib>,
	public CProxy_ISessionEvents< CSession >,
	public IDispatchImpl<IUser, &IID_IUser, &LIBID_ATL_NETLIBLib>,
	public IDispatchImpl<IConfig, &IID_IConfig, &LIBID_ATL_NETLIBLib>,
	public IDispatchImpl<IMonitor, &IID_IMonitor, &LIBID_ATL_NETLIBLib>
{
public:
	CSession()
	{	
		MCTRACE(3,"SESSION CREATED");
		bConfigChanged = TRUE;
		m_ClientState  = stDisconnected;
		InitializeCriticalSection(&CS_SelfInfo);

	}
public:
		DECLARE_WND_CLASS("CSession")
		
		BEGIN_MSG_MAP(CSession)
		MESSAGE_HANDLER(IM_CHANGE_STATE, OnChangeState)
		MESSAGE_HANDLER(IM_ANSWER_OK, OnAnswerOK)
		MESSAGE_HANDLER(IM_ANSWER_BUFF, OnAnswerBuffer)
		MESSAGE_HANDLER(IM_ANSWER_ERROR, OnAnswerError)
		MESSAGE_HANDLER(IM_NEW_EVENT, OnNewEvent)
		END_MSG_MAP()
		
public:
	stState GetState();

	LRESULT OnNewEvent(UINT uMsg, WPARAM wParam,
		LPARAM lParam, BOOL& bHandled)
	{
		m_IM_NET->m_CommandQueue.UnPackEvents((LPVOID)this);
		m_IM_NET->SetNewCommand();//отправка внутренних команд
		return 0;
	}

	LRESULT OnChangeState(UINT uMsg, WPARAM wParam,
		LPARAM lParam, BOOL& bHandled)
	{
		if(wParam == stDisconnected)
		{
			try{m_IM_NET->m_CommandQueue.DeleteAllCommands((void*)this);}catch(...){};
			m_IM_NET->m_CommandQueue.OutPutQueueInit();
		}
		
		
	//	if(wParam == stDisconnected)
	//	Beep(1000,100);
		//ATLASSERT(wParam != stConnecting);

		m_ClientState = (stState)wParam;
		MCTRACE(0,"Fire net status %d",wParam);
		Fire_ChangedState(wParam,LOWORD(lParam),HIWORD(lParam));
		return 0;
	}
	LRESULT OnAnswerOK(UINT uMsg, WPARAM wParam,
		LPARAM lParam, BOOL& bHandled)
	{
		switch(lParam)
		{
		case cmChangeStatus:
			MCTRACE(0,"Fire_SelfStatus");
			Fire_SelfStatus(this->m_IM_NET->m_CurrentSendingStatus);
			break;
		case cmAddUser:
			MCTRACE(0,"Fire_CommandOK");
			Fire_CommandOK(wParam,0);
			break;
		case cmChatStatus:
			MCTRACE(0,"Fire_CommandOK_CHAT_STATUS");
			Fire_ChatStatus(wParam,NULL,NULL);
			break;
		default:
			MCTRACE(0,"Fire_CommandOK");
			Fire_CommandOK(wParam, 0);
			break;
		}
		return 0;
	}
	LRESULT OnAnswerBuffer(UINT uMsg, WPARAM wParam,
		LPARAM lParam, BOOL& bHandled)
	{
		IUnknown*		pIUn = NULL;
		long			lTime;
		long			lListType;
		CComBSTR		AsString;
		CComPtr<IUser>  pUser;
		CComPtr<IUsers>  pUsers;
		CComPtr<IChat>  pChat;
		switch(lParam)
		{
		case cmLogOn:
			EnterCriticalSection(&CS_SelfInfo);
			m_IM_NET->m_CommandQueue.UnPackSelfInfo(m_sUser,wParam);
			LeaveCriticalSection(&CS_SelfInfo);
			break;
			
		case cmLoadList:
			AsString.Empty();
			m_IM_NET->m_CommandQueue.UnPackList(&pIUn,(LPVOID)this,wParam,lListType,&AsString);	
			switch(lListType)
			{
			case ltContact:
				MCTRACE(0,"Fire_ContactList");
				Fire_ContactList((IUsers*)pIUn);
				break;
			case ltIgnore:
				MCTRACE(0,"Fire_IgnoreList");
				Fire_IgnoreList(wParam,(IUsers*)pIUn);
				break;
			case ltFiles:
				MCTRACE(0,"Fire_OffLineFiles");
				Fire_OffLineFiles(wParam,(IFiles*)pIUn);
				break;
			case ltMessages:
				MCTRACE(0,"Fire_MessagesList");
				Fire_MessagesList(wParam,(IMessages*)pIUn);
				break;
			case ltPromos:
				MCTRACE(0,"Fire_PromosList");
				Fire_PromosList(wParam,(IPromos*)pIUn,AsString);
				break;
			case ltSIDs:
				MCTRACE(0,"Fire_SessionsList");
				Fire_SessionsList(wParam,(IlocalSIDs*)pIUn);
				break;
			case ltChats:
				MCTRACE(0,"Fire_ChatsList");
				Fire_ChatList((IChats*)pIUn);
				break;
			default:
				MCTRACE(0,"Fire_CommandError");
				Fire_CommandError(wParam,1,1);
				break;
				
			}
			if(pIUn) pIUn->Release();
			break;
			case cmDetails:
				m_IM_NET->m_CommandQueue.UnPackUserDetails(wParam,&pUser);
				MCTRACE(0,"Fire_Details");
				Fire_Details(wParam,pUser,1);
				pUser.Release();
				break;
			case cmMessage:
			case cmPromo:
			case cmSendFile:
				m_IM_NET->m_CommandQueue.UnPackTime(wParam,lTime);
				MCTRACE(0,"Fire_CommandOK");
				Fire_CommandOK(wParam,lTime);
				break;
			case cmAddUser:
				MCTRACE(0,"Fire_CommandOK");
				Fire_CommandOK(wParam,0);
				break;
			case cmChatCreate:
			case cmChatEdit:
			case cmChatAccept:
				m_IM_NET->m_CommandQueue.UnPackChat(wParam,&pChat);
				MCTRACE(0,"Fire_CreateChat");
				Fire_ChatCreate(wParam,pChat);
				pChat.Release();
				break;
			case cmChatStatus:
				m_IM_NET->m_CommandQueue.UnPackSetChatStatus(wParam,&pUsers,&AsString);
				MCTRACE(0,"Fire_CreateSetChatStatus");
				Fire_ChatStatus(wParam,pUsers,AsString);
				pUsers.Release();
				break;
			default:
				ATLASSERT(FALSE);
				break;
		}
		m_IM_NET->m_CommandQueue.DeleteAnswer(wParam);
		return 0;
	}

	LRESULT OnAnswerError(UINT uMsg, WPARAM wParam,
		LPARAM lParam, BOOL& bHandled)
	{
		m_IM_NET->m_CommandQueue.DeleteCommand(wParam);
		Fire_CommandError(wParam,LOWORD(lParam),HIWORD(lParam));
		return 0;
	}
	
	HRESULT CSession::FinalConstruct()
	{  
		HRESULT hr;
		
		memset(g_ByteSentPerSec,0,200*4);
		memset(g_ByteReceivedPerSec,0,200*4);

		memset(g_forByteSentPerSec,0,200*4);
		memset(g_forByteReceivedPerSec,0,200*4);

		//SendLock.Init();
		//ReceivedLock.Init();

		HWND hwnd;
		RECT rect;
		rect.left=0;
		rect.right=100;
		rect.top=0;
		rect.bottom=100;
		hwnd = Create( NULL, rect, "SessionWindow", WS_POPUP);
		
		if (!hwnd)
		{
			hr = HRESULT_FROM_WIN32(GetLastError());
			return hr;
		}
		m_IM_NET = new CIM_Net;
		if(m_IM_NET == NULL) return E_FAIL;
		hr = m_IM_NET->m_CommandQueue.Init();
		if(FAILED(hr)) return hr;
		m_IM_NET->m_CommandQueue.lpSession = this;
		m_IM_NET->m_ParentSession = this;
		return S_OK;
	}
	HRESULT CSession::FinalRelease()
	{  
		//SendLock.Term();
		//ReceivedLock.Term();

		if(m_ClientState != stDisconnected)
		{
			
		}
		MCTRACE(3,"SESSION RELEASED befor delete im_net");
		delete m_IM_NET;
		MCTRACE(3,"SESSION RELEASED after delete im_net");
		if (m_hWnd != NULL)
		{
			DestroyWindow();
		}
		DeleteCriticalSection(&CS_SelfInfo);

		MCTRACE(3,"SESSION RELEASED");

		
		return S_OK;
		
	}
DECLARE_REGISTRY_RESOURCEID(IDR_SESSION)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CSession)
	COM_INTERFACE_ENTRY(ISession)
	COM_INTERFACE_ENTRY(IConnectionPointContainer)
	COM_INTERFACE_ENTRY_IMPL(IConnectionPointContainer)
	COM_INTERFACE_ENTRY2(IDispatch, ISession)
	COM_INTERFACE_ENTRY(IConfig)
	COM_INTERFACE_ENTRY(IUser)
	COM_INTERFACE_ENTRY(IMonitor)
END_COM_MAP()

BEGIN_CONNECTION_POINT_MAP(CSession)
CONNECTION_POINT_ENTRY(DIID__ISessionEvents)
//CONNECTION_POINT_ENTRY(IID__ISessionEvents)
END_CONNECTION_POINT_MAP()

// ISession
public:
	STDMETHOD(CreateChat)(/*[IN]*/ BSTR CID, /*[IN]*/ BSTR bsName, /*[IN]*/ BSTR Descr, /*[OUT]*/ long* Handle);
	STDMETHOD(get_UseSSL)(/*[out, retval]*/ VARIANT_BOOL *pVal);
	STDMETHOD(put_UseSSL)(/*[in]*/ VARIANT_BOOL newVal);
	STDMETHOD(get_Value)(/*[in]*/ BSTR bsName, /*[out, retval]*/ VARIANT *pVal);
	STDMETHOD(put_Value)(/*[in]*/ BSTR bsName, /*[in]*/ VARIANT newVal);
	STDMETHOD(DeleteUserR)(/*[in]*/ long User_ID, /*[out]*/ long * Handle);
	STDMETHOD(CancelOperation)(/*[in]*/ long Handle);
	STDMETHOD(GetLastPromos)(/*[in]*/ long Count, /*[out,retval]*/ long* Handle);
	
	STDMETHOD(get_SelfInfo)(/*[out, retval]*/ IUser* *pVal);
	STDMETHOD(get_SID)(/*[out, retval]*/ BSTR *pVal);
	STDMETHOD(get_Config)(/*[out, retval]*/ IConfig* *pVal);

	STDMETHOD(LogOn)(BSTR UserName, BSTR Password, long Status, long *Handle);
	STDMETHOD(LogOff)();
	STDMETHOD(ChangeStatus)(long Status);

	STDMETHOD(CreatePromo)(/*[out, retval]*/ IPromo** ppPromo);
	STDMETHOD(CreateFile)(/*[out, retval]*/ IFile **ppFile);
	STDMETHOD(CreateMessage)(/*[out, retval]*/ IMessage **ppMessage);
	
	STDMETHOD(AddUser)(/*[in]*/long User_ID, /*[in]*/ BSTR Body,/*[in]*/ long LisType, /*[out, retval]*/long* Handle);
	STDMETHOD(AddUserReply)(/*[in]*/ long User_ID, /*[in]*/ long Result, /*[out,retval]*/ long* Handle);
	STDMETHOD(DeleteUser)(/*[in]*/ long User_ID, /*[in]*/ long ListType, /*[out, retval]*/ long* Handle);
	
	STDMETHOD(LoadIgnore)(/*[out, retval]*/ long* Handle);
	STDMETHOD(LoadOffLineFiles)(/*[out, retval]*/ long* Handle);
	STDMETHOD(ConfirmFile)(/*[in]*/ BSTR FID, /*[in]*/ long Result, /*[out,retval]*/ long* Handle);
	STDMETHOD(UserDetails)(/*[in]*/ long User_ID, /*[in]*/ long InfoType, /*[out,retval]*/ long* Handle);
	
	STDMETHOD(LoadSIDs)(/*[in]*/ long From, /*[in]*/ long To, /*[out, retVal]*/ long* Handle);
	STDMETHOD(LoadMessages)(/*[in]*/ BSTR SID, /*[out,retval]*/ long* Handle);
	
	CIM_Net*	m_IM_NET;

	CComBSTR	m_SID;
	BOOL		m_UserChanged;
	CComBSTR	m_Password;
	CComBSTR	m_UserName;
	long		m_Status;
	sUser		m_sUser;
	sConfig		m_sConfig;
	BOOL		bConfigChanged;	
	stState		m_ClientState;
	CRITICAL_SECTION CS_SelfInfo;

//////////////////////////////////////////////////////////

// IConfig
	STDMETHOD(get_Server)(BSTR * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;

		*pVal = CComBSTR(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szServerName).Detach();
		return S_OK;
	}

	STDMETHOD(put_Server)(BSTR pVal)
	{
		bConfigChanged = TRUE;
		USES_CONVERSION;
		if(pVal == NULL)
		m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szServerName[0] = '\0';
		else
		_tcscpy(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szServerName, OLE2T(pVal));
		return S_OK;
	}
	STDMETHOD(get_Port)(long * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			*pVal = m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_ServerPort;
		return S_OK;
	}
	STDMETHOD(put_Port)(long pVal)
	{
		bConfigChanged = TRUE;
		m_sConfig.m_Port = pVal;
		m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_ServerPort = pVal;
		return S_OK;
	}
	STDMETHOD(get_Path)(BSTR * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
		
		*pVal = CComBSTR(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szPath).Detach();
		return S_OK;
	}
	STDMETHOD(put_Path)(BSTR pVal)
	{
		bConfigChanged = TRUE;
		USES_CONVERSION;
		if(pVal == NULL)
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szPath[0] = '\0';
		else
			_tcscpy(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szPath, OLE2T(pVal));
		return S_OK;
	}
	STDMETHOD(get_ProxyType)(long * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;

		*pVal = m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_dwAccessType;
		return S_OK;
	}
	STDMETHOD(put_ProxyType)(long pVal)
	{
		bConfigChanged = TRUE;
		m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_dwAccessType = pVal;
		return S_OK;
	}
	STDMETHOD(get_ProxyServerName)(BSTR * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;

		*pVal = CComBSTR(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerName).Detach();
		return S_OK;
	}
	STDMETHOD(put_ProxyServerName)(BSTR pVal)
	{
		bConfigChanged = TRUE;
		USES_CONVERSION;
		if(pVal == NULL)
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerName[0] = '\0';
		else
			_tcscpy(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerName, OLE2T(pVal));
		return S_OK;
	}
	STDMETHOD(get_UseFirewall)(VARIANT_BOOL *pVal)
	{
		if(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_bSecure)
			*pVal = VARIANT_TRUE;
		else
			*pVal = VARIANT_FALSE;
		return S_OK;
	}
	
	STDMETHOD(put_UseFirewall)(VARIANT_BOOL newVal)
	{
		bConfigChanged = TRUE;
		if(newVal == VARIANT_TRUE)
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_bSecure = TRUE;
		else
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_bSecure = FALSE;
		return S_OK;
	}
	
	STDMETHOD(get_FireWallUserName)(BSTR *pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
		
		*pVal = CComBSTR(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin).Detach();
		return S_OK;	
	}
	
	STDMETHOD(put_FireWallUserName)(BSTR newVal)
	{
		bConfigChanged = TRUE;
		USES_CONVERSION;
		if(newVal == NULL)
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin[0] = '\0';
		else
			_tcscpy(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerLogin, OLE2T(newVal));
		return S_OK;
	}
	
	STDMETHOD(get_FireWallPassword)(BSTR *pVal)
	{
		return E_NOTIMPL;
		if (pVal == NULL)
			return E_POINTER;
		
		*pVal = CComBSTR(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword).Detach();
		return S_OK;
	}
	
	STDMETHOD(put_FireWallPassword)(BSTR newVal)
	{
		bConfigChanged = TRUE;
		USES_CONVERSION;
		if(newVal == NULL)
			m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword[0] = '\0';
		else
			_tcscpy(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_szProxyServerPassword, OLE2T(newVal));
		return S_OK;
	}
	
	STDMETHOD(get_ProxyServerPort)(long *pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
		
		*pVal = m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_ProxyServerPort;
		return S_OK;	
	}
	
	STDMETHOD(put_ProxyServerPort)(long newVal)
	{
		bConfigChanged = TRUE;
		m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_ProxyServerPort = newVal;
		return S_OK;
	}
	
//End IConfig

// IMonitor
	STDMETHOD(get_MessageSent)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;

		*pVal = ::InterlockedExchangeAdd(&g_MessageSend,0);	
		return S_OK;
	}
	STDMETHOD(get_MessageReceived)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			
		*pVal = ::InterlockedExchangeAdd(&g_MessageReceveived,0);
		return S_OK;
	}

	STDMETHOD(get_FileSend)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			
		*pVal = ::InterlockedExchangeAdd(&g_FileSent,0);
		return S_OK;
	}

	STDMETHOD(get_FileReceived)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			
		*pVal = ::InterlockedExchangeAdd(&g_FileReceived,0);
		return S_OK;
	}

	STDMETHOD(get_BytesSent)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			
		*pVal = ::InterlockedExchangeAdd(&g_ByteSent,0);
		return S_OK;
	}

	STDMETHOD(get_BytesReceived)(LONG * pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
			
		*pVal = ::InterlockedExchangeAdd(&g_ByteReceived,0);
		return S_OK;
	}


	STDMETHOD(put_BytesSentPerSecondInterval)(LONG pVal)
	{
		if (pVal == NULL)
			return E_POINTER;
		LONG* m_get = (LONG*)pVal;
		DWORD sec = GetTickCount()/1000;
		DWORD Point = sec % 200;

		SendLock.Lock();
		g_AddSentBytes(0);
		memcpy(m_get,&g_ByteSentPerSec[Point+1],(199-Point)*sizeof(LONG));
		memcpy(m_get + (199-Point),g_ByteSentPerSec,(Point+1)*sizeof(LONG));

		//*pVal = g_forByteSentPerSec;
		SendLock.Unlock();
		return S_OK;
	}
	STDMETHOD(put_BytesReceivedPerSecondInterval)(LONG pVal)
	{
				if (pVal == NULL)
			return E_POINTER;

		LONG* m_get = (LONG*)pVal;
		DWORD sec = GetTickCount()/1000;
		DWORD Point = sec % 200;

		ReceivedLock.Lock();
		g_AddReceivedBytes(0);
		memcpy(m_get,&g_ByteReceivedPerSec[Point+1],(199-Point)*sizeof(LONG));
		memcpy(m_get + (199-Point),g_ByteReceivedPerSec,(Point+1)*sizeof(LONG));
		ReceivedLock.Unlock();
		//*pVal = g_forByteReceivedPerSec;

		return S_OK;
	}
};

#endif //__SESSION_H_
