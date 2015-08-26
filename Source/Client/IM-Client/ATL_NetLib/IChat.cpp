// IChat.cpp : Implementation of CIChat
#include "stdafx.h"
#include "ATL_NetLib.h"
#include "IChat.h"
#include "Session.h"
#include "Message.h"
/////////////////////////////////////////////////////////////////////////////
// CIChat


STDMETHODIMP CChat::SetStatus(long Status, long Param, long* Handle)
{
	if(Status >1 || Status <0)
		return E_INVALIDARG;

	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	try
	{
		*Handle = m_pSession->m_IM_NET->m_CommandQueue.x_ChatStatus(m_sChat.m_CID,Status,Param);
		m_pSession->m_IM_NET->SetNewCommand();
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

STDMETHODIMP CChat::CreateMessage(IMessage **ppMessage)
{
	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	HRESULT hr = CMessage::CreateInstance(ppMessage);
	if(hr != 0)
		return hr;
	CMessage* mpMessage;

	mpMessage = static_cast<CMessage*>(*ppMessage);
	mpMessage->m_pSession = m_pSession;
	mpMessage->m_sMessage.m_Chat.m_CID = m_sChat.m_CID;
	mpMessage->m_sMessage.m_bChat = TRUE;
	return hr;

}

STDMETHODIMP CChat::AddUser(long UserID)
{
	m_sChat.m_Users.push_back(UserID);
	return S_OK;
}

STDMETHODIMP CChat::Invite(BSTR Invitation, long *Handle)
{
	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	try
	{
		*Handle = m_pSession->m_IM_NET->m_CommandQueue.x_ChatInvite(&m_sChat,Invitation);
		m_pSession->m_IM_NET->SetNewCommand();
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

STDMETHODIMP CChat::Leave(long *Handle)
{
	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	try
	{
		*Handle = m_pSession->m_IM_NET->m_CommandQueue.x_ChatLeave(m_sChat.m_CID);
		m_pSession->m_IM_NET->SetNewCommand();
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

STDMETHODIMP CChat::Accept(long Result, long *Handle)
{
	if(Result <0 || Result >1)
		return E_INVALIDARG;

	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	try
	{
		*Handle = m_pSession->m_IM_NET->m_CommandQueue.x_ChatAccept(m_sChat.m_CID,Result);
		m_pSession->m_IM_NET->SetNewCommand();
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

STDMETHODIMP CChat::Edit(BSTR Name, BSTR Descr, long *Handle)
{
	//Check Input Parametrs
	if(Name == NULL)
		return E_POINTER;

	//Check Current State
	if(m_pSession->GetState() != stConnected)
		return E_PENDING;

	// Add Command to Queue
	try
	{
		*Handle = m_pSession->m_IM_NET->m_CommandQueue.x_ChatEdit(m_sChat.m_CID,Name,Descr);
		m_pSession->m_IM_NET->SetNewCommand();
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

STDMETHODIMP CChat::get_Value(BSTR bsName, VARIANT *pVal)
{
	if(CComBSTR(bsName) ==  CComBSTR(_T("@cid")))
	{
		return CComVariant(m_sChat.m_CID).Detach(pVal);
	}
	else
		if(CComBSTR(bsName) ==  CComBSTR(_T("name")))
		{
			return CComVariant(m_sChat.m_Name).Detach(pVal);
		}
		else
			if(CComBSTR(bsName) ==  CComBSTR(_T("descr")))
			{
				return CComVariant(m_sChat.m_Descr).Detach(pVal);
			}
			else
				if(CComBSTR(bsName) ==  CComBSTR(_T("time")))
				{
					return CComVariant(m_sChat.m_CreationTime).Detach(pVal);
				}
				else
					if(CComBSTR(bsName) ==  CComBSTR(_T("owner")))
					{
						return CComVariant(m_sChat.m_Creator).Detach(pVal);
					}
					else
						return E_INVALIDARG;
	return S_OK;
}