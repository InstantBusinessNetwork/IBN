// EventContainer.cpp : implementation file
//

#include "stdafx.h"
#include "EventContainer.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////

HRESULT AutoUnMarchaling(NLT_Container *pContainer, LPUNKNOWN* ppv)
{
	if(pContainer == NULL||pContainer->pMarshalStream==NULL)
		return NLT_CONTAINER_ERROR;
	
	switch(pContainer->EventType)
	{
	case NTL_ENone:
		return NLT_OK;
	case NTL_EChangeState:
		return NLT_OK;
	case NTL_EChangedStatus:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NTL_EMessage:
		return pContainer->UnMarchaling(__uuidof(IMessage),ppv);
	case NTL_EPromo:
		return pContainer->UnMarchaling(__uuidof(IPromo),ppv);
	case NTL_EFile:
		return pContainer->UnMarchaling(__uuidof(IFile),ppv);
	case NTL_EAdd:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NTL_EAddR:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NTL_EReklama:
		return NLT_OK;
	case NTL_EContactList:
		return pContainer->UnMarchaling(__uuidof(IUsers),ppv);
	case NTL_EIgnoryList:
		return pContainer->UnMarchaling(__uuidof(IUsers),ppv);
	case NLT_EDetails:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NLT_ECommandOK:
		return NLT_OK;
	case NLT_ECommandError:
		return NLT_OK;
	case NLT_ESessionsList:
		return pContainer->UnMarchaling(__uuidof(IlocalSIDs),ppv);
	case NLT_EOffLineFiles:
		return pContainer->UnMarchaling(__uuidof(IFiles),ppv);
	case NLT_EMessagesList:
		return pContainer->UnMarchaling(__uuidof(IMessages),ppv);
	case NLT_EPromosList:
		return pContainer->UnMarchaling(__uuidof(IPromos),ppv);
	case NLT_ESelfStatus:
		return NLT_OK;
	case NLT_EChatList:
		return pContainer->UnMarchaling(__uuidof(IChats),ppv);
	case NLT_EChatStatus:
		return pContainer->UnMarchaling(__uuidof(IUsers),ppv);
	case NLT_EChatCreate:
		return pContainer->UnMarchaling(__uuidof(IChat),ppv);
	case NLT_EChatUserStatus:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NLT_EChatInvite:
		return pContainer->UnMarchaling(__uuidof(IChat),ppv);
	case NLT_EChatLeave:
		return pContainer->UnMarchaling(__uuidof(IUser),ppv);
	case NLT_EChatMessage:
		return pContainer->UnMarchaling(__uuidof(IChat),ppv);
	case NLT_EChatFile:
		return pContainer->UnMarchaling(__uuidof(IChat),ppv);
	case NLT_EChatAccept:
		return pContainer->UnMarchaling(__uuidof(IChat),ppv);
	default:
		return NLT_EVENT_NO_SUPPORT;
	}
}

HRESULT AutoUnMarchaling2(NLT_Container *pContainer, LPUNKNOWN* ppv)
{
	if(pContainer == NULL||pContainer->pMarshalStream2==NULL)
		return NLT_CONTAINER_ERROR;
	
	switch(pContainer->EventType)
	{
	case NTL_ENone:
		return NLT_OK;
	case NTL_EChangeState:
		return NLT_OK;
	case NTL_EChangedStatus:
		return NLT_OK;
	case NTL_EMessage:
		return NLT_OK;
	case NTL_EPromo:
		return NLT_OK;
	case NTL_EFile:
		return NLT_OK;
	case NTL_EAdd:
		return NLT_OK;
	case NTL_EAddR:
		return NLT_OK;
	case NTL_EReklama:
		return NLT_OK;
	case NTL_EContactList:
		return NLT_OK;
	case NTL_EIgnoryList:
		return NLT_OK;
	case NLT_EDetails:
		return NLT_OK;
	case NLT_ECommandOK:
		return NLT_OK;
	case NLT_ECommandError:
		return NLT_OK;
	case NLT_ESessionsList:
		return NLT_OK;
	case NLT_EOffLineFiles:
		return NLT_OK;
	case NLT_EMessagesList:
		return NLT_OK;
	case NLT_EPromosList:
		return NLT_OK;
	case NLT_ESelfStatus:
		return NLT_OK;
	case NLT_EChatList:
		return NLT_OK;
	case NLT_EChatStatus:
		return NLT_OK;
	case NLT_EChatCreate:
		return NLT_OK;
	case NLT_EChatUserStatus:
		return pContainer->UnMarchaling2(__uuidof(IChat),ppv);
	case NLT_EChatInvite:
		return pContainer->UnMarchaling2(__uuidof(IUser),ppv);
	case NLT_EChatLeave:
		return pContainer->UnMarchaling2(__uuidof(IChat),ppv);
	case NLT_EChatMessage:
		return pContainer->UnMarchaling2(__uuidof(IMessage),ppv);
	case NLT_EChatFile:
		return NLT_OK;
	case NLT_EChatAccept:
		return pContainer->UnMarchaling2(__uuidof(IUser),ppv);
	default:
		return NLT_EVENT_NO_SUPPORT;
	}
}

HRESULT AutoUnMarchaling3(NLT_Container *pContainer, LPUNKNOWN* ppv)
{
	if(pContainer == NULL||pContainer->pMarshalStream3==NULL)
		return NLT_CONTAINER_ERROR;
	
	switch(pContainer->EventType)
	{
	case NTL_ENone:
		return NLT_OK;
	case NTL_EChangeState:
		return NLT_OK;
	case NTL_EChangedStatus:
		return NLT_OK;
	case NTL_EMessage:
		return NLT_OK;
	case NTL_EPromo:
		return NLT_OK;
	case NTL_EFile:
		return NLT_OK;
	case NTL_EAdd:
		return NLT_OK;
	case NTL_EAddR:
		return NLT_OK;
	case NTL_EReklama:
		return NLT_OK;
	case NTL_EContactList:
		return NLT_OK;
	case NTL_EIgnoryList:
		return NLT_OK;
	case NLT_EDetails:
		return NLT_OK;
	case NLT_ECommandOK:
		return NLT_OK;
	case NLT_ECommandError:
		return NLT_OK;
	case NLT_ESessionsList:
		return NLT_OK;
	case NLT_EOffLineFiles:
		return NLT_OK;
	case NLT_EMessagesList:
		return NLT_OK;
	case NLT_EPromosList:
		return NLT_OK;
	case NLT_ESelfStatus:
		return NLT_OK;
	case NLT_EChatList:
		return NLT_OK;
	case NLT_EChatStatus:
		return NLT_OK;
	case NLT_EChatCreate:
		return NLT_OK;
	case NLT_EChatUserStatus:
		return NLT_OK;
	case NLT_EChatInvite:
		return pContainer->UnMarchaling3(__uuidof(IUser),ppv);
	case NLT_EChatLeave:
		return NLT_OK;
	case NLT_EChatMessage:
		return NLT_OK;
	case NLT_EChatFile:
		return NLT_OK;
	default:
		return NLT_EVENT_NO_SUPPORT;
	}
}


/////////////////////////////////////////////////////////////////////////////
// CEventContainer

IMPLEMENT_DYNCREATE(CEventContainer, CCmdTarget)

CEventContainer::CEventContainer()
{
	EnableAutomation();
}

CEventContainer::~CEventContainer()
{
}

BEGIN_MESSAGE_MAP(CEventContainer, CCmdTarget)
	//{{AFX_MSG_MAP(CEventContainer)
		// NOTE - the ClassWizard will add and remove mapping macros here.
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

BEGIN_DISPATCH_MAP(CEventContainer, CCmdTarget)
	//{{AFX_DISPATCH_MAP(CEventContainer)
	DISP_FUNCTION_ID(CEventContainer,"ChangedState",   1, OnChangedState,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4)
	DISP_FUNCTION_ID(CEventContainer,"eChangedStatus",2, One_ChangedStatus, VT_EMPTY, VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eMessage",      3, One_Message,       VT_EMPTY, VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"ePromo",        4, One_Promo,         VT_EMPTY, VTS_DISPATCH VTS_BSTR)
	DISP_FUNCTION_ID(CEventContainer,"eFile",         5, One_File,          VT_EMPTY, VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eAdd",          6, One_Add,           VT_EMPTY, VTS_DISPATCH VTS_BSTR)
	DISP_FUNCTION_ID(CEventContainer,"eAddR",         7, One_AddR,          VT_EMPTY, VTS_DISPATCH VTS_I4)
	DISP_FUNCTION_ID(CEventContainer,"eReklama",      8, One_Reklama,       VT_EMPTY, VTS_BSTR)
	DISP_FUNCTION_ID(CEventContainer,"ContactList",   9, OnContactList,     VT_EMPTY, VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"IgnoreList",    10, OnIgnoreList,      VT_EMPTY, VTS_I4 VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"Details",       11, OnDetails,         VT_EMPTY, VTS_I4 VTS_DISPATCH VTS_I4)
	DISP_FUNCTION_ID(CEventContainer,"CommandOK",     12, OnCommandOk,       VT_EMPTY, VTS_I4 VTS_I4)
	DISP_FUNCTION_ID(CEventContainer,"CommandError",  13, OnCommandError,    VT_EMPTY, VTS_I4 VTS_I4 VTS_I4)
    DISP_FUNCTION_ID(CEventContainer,"SessionsList",  14, OnSessionsList ,   VT_EMPTY, VTS_I4 VTS_DISPATCH)	
    DISP_FUNCTION_ID(CEventContainer,"OffLineFiles",  15, OnOffLineFiles ,    VT_EMPTY, VTS_I4 VTS_DISPATCH)	
    DISP_FUNCTION_ID(CEventContainer,"MessagesList",  16, OnMessagesList ,    VT_EMPTY, VTS_I4 VTS_DISPATCH)	
    DISP_FUNCTION_ID(CEventContainer,"PromosList",    17, OnPromosList ,      VT_EMPTY, VTS_I4 VTS_DISPATCH VTS_BSTR)	
    DISP_FUNCTION_ID(CEventContainer,"SelfStatus",    18, OnSelfStatus,      VT_EMPTY, VTS_I4)	
	DISP_FUNCTION_ID(CEventContainer,"eSysMess",	  19, OneSysMess,    VT_EMPTY, VTS_I4 VTS_BSTR)	
	DISP_FUNCTION_ID(CEventContainer,"ChatList",	  20, OnChatList,    VT_EMPTY, VTS_DISPATCH)	
	DISP_FUNCTION_ID(CEventContainer,"ChatStatus",	  21, OnChatStatus,    VT_EMPTY, VTS_I4 VTS_DISPATCH VTS_BSTR)
	DISP_FUNCTION_ID(CEventContainer,"ChatCreate",	  22, OnChatCreate,    VT_EMPTY, VTS_I4 VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eChatUserStatus",	  23, OneChatUserStatus,    VT_EMPTY, VTS_DISPATCH VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eChatInvite",	  24, OneChatInvite,    VT_EMPTY, VTS_DISPATCH VTS_DISPATCH VTS_DISPATCH VTS_BSTR)
	DISP_FUNCTION_ID(CEventContainer,"eChatLeave",	  25, OneChatLeave,    VT_EMPTY, VTS_DISPATCH VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eChatMessage",  26, OneChatMessage,    VT_EMPTY, VTS_DISPATCH VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eChatFile",  27, OneChatFile,    VT_EMPTY, VTS_DISPATCH)
	DISP_FUNCTION_ID(CEventContainer,"eChatAccept", 28, OneChatAccept, VT_EMPTY, VTS_DISPATCH VTS_DISPATCH VTS_I4)
// DISP_FUNCTION_ID(CEventContainer,"", 0x, On ,    VT_EMPTY, VTS_)	
	//}}AFX_DISPATCH_MAP
END_DISPATCH_MAP()

// Note: we add support for IID_IEventContainer to support typesafe binding
//  from VBA.  This IID must match the GUID that is attached to the 
//  dispinterface in the .ODL file.
// Необходимо Добавить Интерфейсы Событий к которым вы хотите подключится
// использую IConnectionPoint
BEGIN_INTERFACE_MAP(CEventContainer, CCmdTarget)
    INTERFACE_PART(CEventContainer, __uuidof(_ISessionEvents), Dispatch)
END_INTERFACE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CEventContainer message handlers

void CEventContainer::OnChangedState(long State,long ErrorType,long ErrorCode)
{
	//MCTRACE(8,"[Event] OnChangedState State = %ld ErrorType  = %ld ErrorCode  = %ld\n",State,ErrorType,ErrorCode);
	TRACE(_T("\r\n[Event] OnChangedState State = %ld ErrorType  = %ld ErrorCode  = %ld\n"), State, ErrorType, ErrorCode);
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		
		pContainer->EventType = NTL_EChangeState;
		
		pContainer->Long1 = State;
		pContainer->Long2 = ErrorType;
		pContainer->Long3 = ErrorCode;
		
		if(!pContainer->Send(hWnd))
			delete pContainer;
	}
}

void CEventContainer::One_ChangedStatus(IUser* pUser)
{
	//MCTRACE(8,"[Event] One_ChangedStatus\n");
	TRACE(_T("\r\n[Event] One_ChangedStatus\n"));

	//  [7/24/2002]
	//CString strLogin	=	(char*)_bstr_t(pUser->GetValue(bstr_t("nick_name")));
	//TRACE("\r\n[Event] One_ChangedStatus --- %s\n",strLogin);
	//  [7/24/2002]

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		
		pContainer->EventType = NTL_EChangedStatus;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)pUser);
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr)) pUser->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_Message(IMessage* pMessage)
{
	//MCTRACE(8,"[Event] One_Message\n");
	TRACE(_T("\r\n[Event] One_Message\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EMessage;
		HRESULT hr = pContainer->Marchaling(__uuidof(IMessage),(LPUNKNOWN)pMessage);
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr)) pMessage->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_Promo(IPromo* pPromo,LPCTSTR PromoAsString)
{
	//MCTRACE(8,"[Event] One_Promo\n");
	TRACE(_T("\r\n[Event] One_Promo\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EPromo;
		pContainer->String1 = PromoAsString;
		HRESULT hr = pContainer->Marchaling(__uuidof(IPromo),(LPUNKNOWN)pPromo);
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr)) pPromo->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_File(IFile* pFile)
{
	//MCTRACE(8,"\r\n[Event] One_File\n");
	TRACE(_T("\r\n[Event] One_File\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EFile;
		HRESULT hr = pContainer->Marchaling(__uuidof(IFile),(LPUNKNOWN)pFile);
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr)) pFile->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_Add(IUser* pUser, LPCTSTR bstrMessage)
{
	//MCTRACE(8,"\r\n[Event] One_Add\n");
	TRACE(_T("\r\n[Event] One_Add\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EAdd;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)pUser);
		pContainer->String1   = bstrMessage;
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr))
			    pUser->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_AddR(IUser* User, long nResult)
{
	//MCTRACE(8,"\r\n[Event] One_AddR\n");
	TRACE(_T("\r\n[Event] One_AddR\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EAddR;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)User);
		pContainer->Long1   = nResult;
		if(FAILED(hr)||!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr))
			    User->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::One_Reklama(LPCTSTR bstrURL)
{
	//MCTRACE(8,"[Event] One_Reklama\n");
	TRACE(_T("\r\n[Event] One_Reklama\n"));
	HWND hWnd = NULL;
	m_NetLibTranslator.Lock();
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	m_NetLibTranslator.UnLock();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EReklama;
		pContainer->String1   = bstrURL;
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}
}

void CEventContainer::OnContactList(IUsers* pUsers)
{
	//MCTRACE(8,"[Event] OnContactList\n");
	TRACE(_T("\r\n[Event] OnContactList\n"));
	HWND hWnd = NULL;
	m_NetLibTranslator.Lock();
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	m_NetLibTranslator.UnLock();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EContactList;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUsers),(LPUNKNOWN)pUsers);
		if(/*FAILED(hr)||*/!pContainer->Send(hWnd))
		{
			if(SUCCEEDED(hr))
			    pUsers->Release();
			delete pContainer;
		}
	}
}

void CEventContainer::OnIgnoreList(long Handle,IUsers* pUsers)
{
	//MCTRACE(8,"[Event] OnIgnoreList\n");
	TRACE(_T("\r\n[Event] OnIgnoreList\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NTL_EIgnoryList;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUsers),(LPUNKNOWN)pUsers);
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}
}

void CEventContainer::OnDetails(long Handle, IUser* pUser,long type)
{
	//MCTRACE(8,"[Event] OnDetails\n");
	TRACE(_T("\r\n[Event] OnDetails\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EDetails;
		pContainer->Handel = Handle;
		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)pUser);
		pContainer->Send(hWnd);
	}
}

void CEventContainer::OnCommandOk(long Handle, long AddVal)
{
	//MCTRACE(8,"[Event] OnCommandOk\n");
	TRACE(_T("\r\n[Event] OnCommandOk\n"));
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_ECommandOK;
		pContainer->Handel = Handle;
		pContainer->Long1  = AddVal;
		pContainer->Send(hWnd);
	}

}

void CEventContainer::OnCommandError(long Handle,long ErrorType,long ErrorCode)
{
	//MCTRACE(8,"[Event] OnCommandError Handle =  %ld, Type = %ld Code = %ld\n",Handle, ErrorType, ErrorCode);
	TRACE(_T("\r\n[Event] OnCommandError Handle =  %ld, Type = %ld Code = %ld\n"), Handle, ErrorType, ErrorCode);

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_ECommandError;
		pContainer->Handel = Handle;		
		pContainer->Long1  = ErrorType;
		pContainer->Long2  = ErrorCode;
		pContainer->Send(hWnd);
	}
}

void CEventContainer::OnSessionsList(long Handle, IlocalSIDs* plocalSIDs)
{
	//MCTRACE(8,"[Event] OnSessionsList\n");
	TRACE(_T("\r\n[Event] OnSessionsList\n"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_ESessionsList;
		pContainer->Handel = Handle;
		HRESULT hr = pContainer->Marchaling(__uuidof(IlocalSIDs),(LPUNKNOWN)plocalSIDs);
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}	
}

void CEventContainer::OnOffLineFiles(long Handle, IFiles* pFiles)
{
	//MCTRACE(8,"[Event] OnOffLineFiles\n");
	TRACE(_T("\r\n[Event] OnOffLineFiles\n"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EOffLineFiles;
		pContainer->Handel = Handle;
		HRESULT hr = pContainer->Marchaling(__uuidof(IFiles),(LPUNKNOWN)pFiles);
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
		
	}	
}

void CEventContainer::OnMessagesList(long Handle, IMessages* pMessages)
{
	//MCTRACE(8,"[Event] OnMessagesList\n");
	TRACE(_T("\r\n[Event] OnMessagesList\n"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EMessagesList;
		pContainer->Handel = Handle;
		HRESULT hr = pContainer->Marchaling(__uuidof(IMessages),(LPUNKNOWN)pMessages);
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}	
}

void CEventContainer::OnPromosList(long Handle, IPromos* pPromos, LPCTSTR PromoAsString)
{
	//MCTRACE(8,"[Event] OnPromosList\n");
	TRACE(_T("\r\n[Event] OnPromosList\n"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EPromosList;
		pContainer->Handel = Handle;
		pContainer->String1 = PromoAsString;
		if(pPromos)
			HRESULT hr = pContainer->Marchaling(__uuidof(IPromos),(LPUNKNOWN)pPromos);
		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}	
}

void CEventContainer::OnSelfStatus(long Status)
{
	//MCTRACE(8,"[Event] OnSelfStatus\n");
	TRACE(_T("\r\n[Event] OnSelfStatus\n"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_ESelfStatus;
		pContainer->Long1 = Status;
		pContainer->Send(hWnd);
	}	
}

void CEventContainer::OneSysMess(long Code, LPCTSTR Description)
{
	//MCTRACE(8,"[Event] OneSysMess Code = %d, Description = %s\n",Code,Description);
	TRACE(_T("\r\n[Event] OneSysMess Code = %d, Description = %s\n"), Code, Description);

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_ESysMess;
		pContainer->Long1		= Code;
		pContainer->String1		= Description;

		pContainer->Send(hWnd);
	}	
}

void CEventContainer::OnChatList(IChats *pChats)
{
	//MCTRACE(8,"[Event] OnChatList");
	TRACE(_T("\r\n[Event] OnChatList"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatList;
		HRESULT hr = pContainer->Marchaling(__uuidof(IChats),(LPUNKNOWN)pChats);

		pContainer->Send(hWnd);
	}
}

void CEventContainer::OnChatStatus(long Handle, IUsers *pUsers, LPCTSTR bsLog)
{
	//MCTRACE(8,"[Event] OnChatStatus");
	TRACE(_T("\r\n[Event] OnChatStatus"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EChatStatus;
		pContainer->Handel  = Handle;

		HRESULT hr = pContainer->Marchaling(__uuidof(IUsers),(LPUNKNOWN)pUsers);

		pContainer->String1	= bsLog;	

		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}	
}

void CEventContainer::OnChatCreate(long Handle, IChat *pChat)
{
	//MCTRACE(8,"[Event] OnChatCreate");
	TRACE(_T("\r\n[Event] OnChatCreate"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetWindow(Handle);
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType = NLT_EChatCreate;
		pContainer->Handel  = Handle;

		HRESULT hr = pContainer->Marchaling(__uuidof(IChat),(LPUNKNOWN)pChat);

		if(!pContainer->Send(hWnd))
		{
			delete pContainer;
		}
	}	
}

void CEventContainer::OneChatUserStatus(IUser *pUser, IChat *pChat)
{
	//MCTRACE(8,"[Event] OneChatUserStatus");
	TRACE(_T("\r\n[Event] OneChatUserStatus"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatUserStatus;

		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)pUser);
		hr = pContainer->Marchaling2(__uuidof(IChat),(LPUNKNOWN)pChat);

		pContainer->Send(hWnd);
	}
}

void CEventContainer::OneChatInvite(IChat *pChat, IUser *pUser, IUser *pInvitedFriend, LPCTSTR Invitation)
{
	//MCTRACE(8,"[Event] OneChatInvite");
	TRACE(_T("\r\n[Event] OneChatInvite"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatInvite;

		HRESULT hr = pContainer->Marchaling(__uuidof(IChat),(LPUNKNOWN)pChat);
		hr = pContainer->Marchaling2(__uuidof(IUser),(LPUNKNOWN)pUser);
		hr = pContainer->Marchaling3(__uuidof(IUser),(LPUNKNOWN)pInvitedFriend);

		pContainer->String1	=	Invitation;

		pContainer->Send(hWnd);
	}
}

void CEventContainer::OneChatLeave(IUser *pUser, IChat *pChat)
{
	//MCTRACE(8,"[Event] OneChatLeave");
	TRACE(_T("\r\n[Event] OneChatLeave"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatLeave;

		HRESULT hr = pContainer->Marchaling(__uuidof(IUser),(LPUNKNOWN)pUser);
		hr = pContainer->Marchaling2(__uuidof(IChat),(LPUNKNOWN)pChat);

		pContainer->Send(hWnd);
	}
}

void CEventContainer::OneChatMessage(IChat *pChat, IMessage *pMessage)
{
	//MCTRACE(8,"[Event] OneChatMessage");
	TRACE(_T("\r\n[Event] OneChatMessage"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatMessage;

		HRESULT hr = pContainer->Marchaling(__uuidof(IChat),(LPUNKNOWN)pChat);
		hr = pContainer->Marchaling2(__uuidof(IMessage),(LPUNKNOWN)pMessage);

		pContainer->Send(hWnd);
	}
}

void CEventContainer::OneChatFile(IChat *pChat)
{
	//MCTRACE(8,"[Event] OneChatFile");
	TRACE(_T("\r\n[Event] OneChatFile"));
	
	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatFile;
		
		HRESULT hr = pContainer->Marchaling(__uuidof(IChat),(LPUNKNOWN)pChat);
		
		pContainer->Send(hWnd);
	}
}

void CEventContainer::OneChatAccept(IChat* pChat, IUser* pUser, long Result)
{
	//MCTRACE(8,"[Event] OneChatAccept");
	TRACE(_T("\r\n[Event] OneChatAccept"));

	HWND hWnd = NULL;
	hWnd = m_NetLibTranslator.NLT_GetEventWindow();
	/// ????? Работа с окном ...
	if(IsWindow(hWnd))
	{
		NLT_Container *pContainer = new NLT_Container;
		pContainer->EventType	= NLT_EChatAccept;
		
		HRESULT hr = pContainer->Marchaling(__uuidof(IChat),(LPUNKNOWN)pChat);
		hr = pContainer->Marchaling2(__uuidof(IUser),(LPUNKNOWN)pUser);
		pContainer->Long1 = Result;
		
		pContainer->Send(hWnd);
	}
}