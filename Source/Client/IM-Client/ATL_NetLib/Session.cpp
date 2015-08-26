// Session.cpp : Implementation of CSession
#include "stdafx.h"
#include "Session.h"
#include "Message.h"
#include "ATLCONV.H"
#include "ATL_NetLib.h"


/////////////////////////////////////////////////////////////////////////////
// CSession

STDMETHODIMP CSession::get_SID(BSTR *pVal)
{
	return m_SID.CopyTo(pVal);
}

STDMETHODIMP CSession::get_SelfInfo(IUser **pVal)
{
	if(pVal == NULL)
	return E_POINTER;
	
	return this->QueryInterface(IID_IUser,(void**)pVal);
	
}

STDMETHODIMP CSession::get_Config(IConfig **pVal)
{
	void* pConfig;
	HRESULT hr;
	hr = this->QueryInterface(IID_IConfig, &pConfig);
	if (hr == 0)
	{
		*pVal = (IConfig*) pConfig;

		return S_OK;
	}
	else
	return hr;
}

//////////////////////////////////////////////////////////////////////////////////
// Commands

//Add User to List
STDMETHODIMP CSession::AddUser(long User_ID, BSTR Body, long ListType, long* Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_AddUser(User_ID,ListType,Body);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
			return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Add user reply
STDMETHODIMP CSession::AddUserReply(long User_ID, long Result, long *Handle)
{
	//Check Input parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_AddUserR(User_ID,Result);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;
			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
			return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Chenge self status
STDMETHODIMP CSession::ChangeStatus(long Status)
{
	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;
	if(Status == 0)
		return E_INVALIDARG;

	//Add Command
	try
	{
		m_IM_NET->m_CommandQueue.x_ChangeStatus(Status);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;
			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Confirm File
STDMETHODIMP CSession::ConfirmFile(BSTR FID, long Result, long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;
	
	if(Result<1 || Result>3)
		return E_INVALIDARG;
	
	if(GetState() != stConnected)
		return E_PENDING;	

	//Add Command
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_ConfirmFile(FID,Result);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;
			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Create File
STDMETHODIMP CSession::CreateFile(IFile **ppFile)
{
	HRESULT hr = CFile::CreateInstance(ppFile);
	if(hr != 0)
		return hr;
	CFile* mpFile;

	mpFile = static_cast<CFile*>(*ppFile);
	mpFile->m_pSession = this;
	return hr;
}

//Create Message
STDMETHODIMP CSession::CreateMessage(IMessage **ppMessage)
{
	HRESULT hr = CMessage::CreateInstance(ppMessage);
	if(hr != 0)
		return hr;
	CMessage* mpMessage;

	mpMessage = static_cast<CMessage*>(*ppMessage);
	mpMessage->m_pSession = this;
	return hr;
}

//Create Promo
STDMETHODIMP CSession::CreatePromo(IPromo **ppPromo)
{
	ATLTRACE("Before Create");
	HRESULT hr = CPromo::CreateInstance(ppPromo);
	if(hr != 0)
		return hr;
	CPromo* mpPromo;
	ATLTRACE("After Create");
	mpPromo = static_cast<CPromo*>(*ppPromo);
	ATLTRACE("After Set Session");
	mpPromo->m_pSession = this;
	return hr;
}

//Delete User
STDMETHODIMP CSession::DeleteUser(long User_ID, long ListType, long *Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Input Parametrs
	if(ListType <1 || ListType >2)
		return E_INVALIDARG;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_DeleteUser(User_ID,ListType);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
		case WRONG_PARAM:
			return E_INVALIDARG;
			break;
			
		default:
			return E_INVALIDQUEUE;
			break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Load Last Promos
STDMETHODIMP CSession::GetLastPromos(long Count, long *Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Input Parametrs
	if(Count <1 || Count >50)
		return E_INVALIDARG;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_LastPromos(Count);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
			return E_INVALIDQUEUE;
	}
	return S_OK;	
}

//Load Ignore
STDMETHODIMP CSession::LoadIgnore(long *Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_LoadList(ltIgnore);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
			return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Load Messages
STDMETHODIMP CSession::LoadMessages(BSTR SID, long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;

	if(SID == NULL)
		return E_INVALIDARG;
	
	if(GetState() != stConnected)
		return E_PENDING;
	
	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_LoadMessages(SID);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Load Off line Files
STDMETHODIMP CSession::LoadOffLineFiles(long *Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_LoadList(ltFiles);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//Load SIDs
STDMETHODIMP CSession::LoadSIDs(long From, long To, long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;
	if(To<=From)
		return E_INVALIDARG;
	if(GetState() != stConnected)
		return E_PENDING;
	
	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_LoadSIDs(From,To);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

//LogOff
STDMETHODIMP CSession::LogOff()
{

	if(GetState() == stDisconnected) return E_POINTER;
	
	try
	{
			m_IM_NET->m_CommandQueue.x_LogOff();
			m_IM_NET->SetNewCommand();
	}
	catch(long Err)
	{
		Err;
		MCTRACE(4,"Queue Error = %d\r\n", Err);
	}
	return S_OK;
}

STDMETHODIMP CSession::LogOn(BSTR UserName, BSTR Password, long Status, long* Handle)
{
MCTRACE(1,"session logon begin");
	if(bConfigChanged)
	{
		m_IM_NET->Config();
		bConfigChanged = FALSE;
	}
	

	if(GetState() != stDisconnected) return E_PENDING;
	//m_IM_NET->m_CommandQueue.OutPutQueueInit();

	CComBSTR m_TempUser(UserName);
	
	//m_sUser.???????????????????????????????????????????????
	(m_UserName == m_TempUser) ? m_UserChanged = FALSE : m_UserChanged = TRUE;
	m_UserName.Empty();
	m_UserName = m_TempUser;
	m_Password.Empty();
	m_Password = CComBSTR(Password);
	m_Status   = Status;

	if(m_UserChanged)
	{
		//generating new SID
		CComBSTR sid;
		GUID	 SID;
		CoCreateGuid(&SID);
		sid	  = CComBSTR(SID);
		m_SID = CComBSTR(36);
		wcsncpy(m_SID.m_str, sid.m_str+1,36);
		//wcscpy(m_SID.m_str, sid.m_str+1);        
		
	}

	if (GetState() == stDisconnected)
	{	
		try
		{	
			*Handle = m_IM_NET->m_CommandQueue.x_LogOn(m_SID,m_UserName,m_Password,m_Status);
			m_IM_NET->SetNewCommand();
		}
		catch(long Err)
		{
			Err;
			MCTRACE(4,"Queue Error = %d\r\n", Err);
		}
	}
	return S_OK;
}


STDMETHODIMP CSession::UserDetails(long User_ID, long InfoType, long *Handle)
{
	if(Handle == NULL)
		return E_POINTER;
	if(InfoType<1 || InfoType>2)
		return E_INVALIDARG;
	if(GetState() != stConnected)
		return E_PENDING;
	
	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_UserDetails(User_ID,InfoType);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}


STDMETHODIMP CSession::CancelOperation(long Handle)
{
	m_IM_NET->CancelCommand(Handle);
	return S_OK;
}

stState CSession::GetState()
{
	return m_ClientState;
}


STDMETHODIMP CSession::DeleteUserR(long User_ID, long *Handle)
{
	//Check Input Parametrs
	if(Handle == NULL)
		return E_POINTER;
	
	
	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;
	
	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_DeleteUserR(User_ID);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
		case WRONG_PARAM:
			return E_INVALIDARG;
			break;
			
		default:
			return E_INVALIDQUEUE;
			break;
		}
	}
	catch(...)
	{
		return E_INVALIDQUEUE;
	}
	return S_OK;
}

STDMETHODIMP CSession::get_Value(BSTR bsName, VARIANT *pVal)
{
	if(CComBSTR(bsName) ==  CComBSTR(_T("nick_name")))
	{
		return CComVariant(m_sUser.m_UserName).Detach(pVal);
	}
	else
		if(CComBSTR(bsName) ==  CComBSTR(_T("first_name")))
		{
			return CComVariant(m_sUser.m_FirstName).Detach(pVal);
		}
		else
			if(CComBSTR(bsName) ==  CComBSTR(_T("last_name")))
			{
				return CComVariant(m_sUser.m_LastName).Detach(pVal);
			}
			else
				if(CComBSTR(bsName) ==  CComBSTR(_T("role/name")))
				{
					return CComVariant(m_sUser.m_Role).Detach(pVal);
				}
				else
					if(CComBSTR(bsName) ==  CComBSTR(_T("email")))
					{
						return CComVariant(m_sUser.m_EMail).Detach(pVal);
					}
					else
						if(CComBSTR(bsName) ==  CComBSTR(_T("status")))
						{
							return CComVariant(m_sUser.m_Status).Detach(pVal);
						}
						else
							if(CComBSTR(bsName) ==  CComBSTR(_T("@id")))
							{
								return CComVariant(m_sUser.m_ID).Detach(pVal);
							}
							else
								if(CComBSTR(bsName) ==  CComBSTR(_T("role/@id")))
								{
									return CComVariant(m_sUser.m_Role_ID).Detach(pVal);
								}
								else
									if(CComBSTR(bsName) ==  CComBSTR(_T("time")))
									{
										return CComVariant(m_sUser.m_time).Detach(pVal);
									}
									else
									return E_INVALIDARG;

	return S_OK;
}

STDMETHODIMP CSession::put_Value(BSTR bsName, VARIANT newVal)
{
	if(CComBSTR(bsName) ==  CComBSTR(_T("nick_name")))
	{
		if(newVal.vt != VT_BSTR) return E_INVALIDARG;
		m_sUser.m_UserName = CComBSTR(newVal.bstrVal);
	}
	else
		if(CComBSTR(bsName) ==  CComBSTR(_T("first_name")))
		{
			if(newVal.vt != VT_BSTR) return E_INVALIDARG;
			m_sUser.m_FirstName = CComBSTR(newVal.bstrVal);
		}
		else
			if(CComBSTR(bsName) ==  CComBSTR(_T("last_name")))
			{
				if(newVal.vt != VT_BSTR) return E_INVALIDARG;
				m_sUser.m_LastName = CComBSTR(newVal.bstrVal);
			}
			else
				if(CComBSTR(bsName) ==  CComBSTR(_T("role/name")))
				{
					if(newVal.vt != VT_BSTR) return E_INVALIDARG;
					m_sUser.m_Role = CComBSTR(newVal.bstrVal);
				}
				else
					if(CComBSTR(bsName) ==  CComBSTR(_T("email")))
					{
						if(newVal.vt != VT_BSTR) return E_INVALIDARG;
						m_sUser.m_EMail = CComBSTR(newVal.bstrVal);
					}
					else
						if(CComBSTR(bsName) ==  CComBSTR(_T("status")))
						{
							if(newVal.vt != VT_I4) return E_INVALIDARG;
							m_sUser.m_Status = newVal.lVal;
						}
						else
							if(CComBSTR(bsName) ==  CComBSTR(_T("@id")))
							{
								if(newVal.vt != VT_I4) return E_INVALIDARG;
								m_sUser.m_ID = newVal.lVal;
							}
							else
								if(CComBSTR(bsName) ==  CComBSTR(_T("role/@id")))
								{
									if(newVal.vt != VT_I4) return E_INVALIDARG;
									m_sUser.m_Role_ID = newVal.lVal;
								}
								else
									if(CComBSTR(bsName) ==  CComBSTR(_T("time")))
									{
										if(newVal.vt != VT_I4) return E_INVALIDARG;
										m_sUser.m_time = newVal.lVal;
									}
									else
									return E_INVALIDARG;
								
	return S_OK;
}

STDMETHODIMP CSession::get_UseSSL(VARIANT_BOOL *pVal)
{
	if(m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_bUseSSL)
		*pVal = VARIANT_TRUE;
	else
		*pVal = VARIANT_FALSE;

	return S_OK;
}

STDMETHODIMP CSession::put_UseSSL(VARIANT_BOOL newVal)
{
	
	m_IM_NET->m_BASE_NET_MANAGER_CONFIG.m_bUseSSL = (newVal == VARIANT_TRUE);
	return S_OK;
}

STDMETHODIMP CSession::CreateChat(BSTR CID, BSTR bsName, BSTR Descr, long* Handle)
{
		//Check Input Parametrs
	if(CID == NULL)
		return E_POINTER;

	if(bsName == NULL)
		return E_POINTER;

	if(SysStringLen(CID) != 36)
		return E_INVALIDARG;

	//Check Current State
	if(GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_IM_NET->m_CommandQueue.x_ChatCreate(CID,bsName,Descr);
		m_IM_NET->SetNewCommand();
	}
	catch(long ErrorCode)
	{
		switch(ErrorCode)
		{
			case WRONG_PARAM:
			return E_INVALIDARG;
				break;

			default:
			return E_INVALIDQUEUE;
				break;
		}
	}
	catch(...)
	{
			return E_INVALIDQUEUE;
	}
	return S_OK;
}
